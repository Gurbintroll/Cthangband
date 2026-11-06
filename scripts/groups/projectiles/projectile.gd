@abstract # Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Projectile
extends RefCounted

var spell_effects: SpellEffectsHandler
var savegame: SaveGame
var player: Player
var dungeon_level: DungeonLevel
var bolt_graphic: String = "WhiteBolt"
var effect_animation: String = ""
var impact_graphic: String = "WhiteSplat"
var projectile_mon_num: int = 0
var projectile_mon_col: int = 0
var projectile_mon_row: int = 0


func _init(effects: SpellEffectsHandler) -> void:
	spell_effects = effects
	savegame = SaveGame.instance
	player = savegame.player
	dungeon_level = savegame.dungeon_level


func fire(who: int, radius: int, row_target: int, col_target: int, dam: int, flag: int) -> bool:
	var row_start: int
	var col_start: int
	var tile: GridTile
	var notice: bool = false
	var visual: bool = false
	var drawn: bool = false
	var breath: bool = false
	var blind: bool = player.timed_blindness != 0
	var affected_tiles: int = 0
	var affected_cols: Array[int]
	var affected_rows: Array[int]
	# Initialise our affected location arrays
	affected_cols.resize(256)
	affected_cols.fill(0)
	affected_rows.resize(256)
	affected_rows.fill(0)
	var frame_indices: Array[int]
	frame_indices.resize(32)
	frame_indices.fill(0)
	# Assume we will have a number of frames equal to our radius unless told otherwise
	var frame_count: int = radius
	# Get our projectile, impact (which is also used if we have a beam), and animation
	var projectile_entity: Flavour = null if bolt_graphic == "" else StaticData.projectiles[bolt_graphic]
	var impact_entity: Flavour = null if impact_graphic == "" else StaticData.projectiles[impact_graphic]
	var animation_entity: AsciiAnimation = null if effect_animation == "" else StaticData.animations[effect_animation]
	# If we should jump straight to our target, start there
	if flag & ProjectionFlag.JUMP != 0:
		col_start = col_target
		row_start = row_target
	# Start at the player or monster that produced us
	elif who == 0:
		col_start = player.map_col
		row_start = player.map_row
	else:
		col_start = dungeon_level.dungeon_monsters.monsters[who].map_col
		row_start = dungeon_level.dungeon_monsters.monsters[who].map_row
	var row_saver: int = row_start
	var col_saver: int = col_start
	var row_end: int = row_target
	var col_end: int = col_target
	# If we go through an enemy, we shouldn't continue going through more enemies
	if flag & ProjectionFlag.THRU != 0:
		if col_start == col_end and row_start == row_end:
			flag &= ~ProjectionFlag.THRU
	# A negative radius means use a cone (an expanding ball) rather than a projection followed by a ball
	if radius < 0:
		radius = 0 - radius
		breath = true
		flag |= ProjectionFlag.HIDE
	# Redraw the screen if necessary
	savegame.notice_update_redraw()
	# Our target location will move from our start location to our end location (or until we hit something)
	col_target = col_start
	row_target = row_start
	var distance_travelled: int = 0
	# Loop until we hit something
	while true:
		# If we have a beam, add our location to the list of locations we have affected/drawn
		if flag & ProjectionFlag.BEAM != 0:
			affected_rows[affected_tiles] = row_target
			affected_cols[affected_tiles] = col_target
			affected_tiles += 1
		# If we're a beam, and visible, and on the panel, draw our impact entity if we have one
		if not blind and flag & ProjectionFlag.HIDE == 0 and distance_travelled != 0 and flag & ProjectionFlag.BEAM != 0 and dungeon_level.panel_contains(row_target, col_target) and dungeon_level.player_has_los_bold(row_target, col_target):
			if impact_entity != null:
				dungeon_level.print_character_at_map_location(impact_entity.character, impact_entity.colour, row_target, col_target)
		tile = dungeon_level.get_grid(row_target, col_target)
		# If we have moved, and we've hit something, then stop
		if distance_travelled != 0 and not dungeon_level.grid_passable(row_target, col_target):
			break
		# If we've reached our destination and shouldn't go through, then stop
		if flag & ProjectionFlag.THRU == 0 and col_target == col_end and row_target == row_end:
			break
		# If we've hit a monster and should stop, then stop
		if tile.monster_index != 0 and distance_travelled != 0 and (flag & ProjectionFlag.STOP) != 0:
			break
		# Move one step closer to our end
		var location: Location = dungeon_level.move_one_step_towards(row_target, col_target, row_start, col_start, row_end, col_end)
		var new_row: int = location.row
		var new_col: int = location.col
		# If we're a ball and our new location is impassable, stop
		if not dungeon_level.grid_passable(new_row, new_col) and radius > 0:
			break
		# If we've moved as far as the maximum spell range, then stop
		distance_travelled += 1
		if distance_travelled > DungeonLevel.MAX_SPELL_RANGE:
			break
		# If we shouldn't hide our projectile, and we can see it, draw it
		if not blind and flag & ProjectionFlag.HIDE == 0:
			if dungeon_level.player_has_los_bold(new_row, new_col) and dungeon_level.panel_contains(new_row, new_col):
				if projectile_entity != null:
					var directional_character: String = projectile_entity.character
					# If our bolt is a line character, replace it with the appropriate one for its direction
					if directional_character == "|":
						directional_character = bolt_char(row_target, col_target, new_row, new_col)
					dungeon_level.print_character_at_map_location(directional_character, projectile_entity.colour, new_row, new_col)
					# Note that we drew something, wait for an animation tick, and then remove what we drew
					visual = true
					User.wait_for_animation()
					dungeon_level.redraw_single_location(new_row, new_col)
			elif visual:
				# We couldn't see it, but we have drawn previous, so wait for an animation tick anyway
				User.wait_for_animation()
		# Advance to our new location
		row_target = new_row
		col_target = new_col
	# We've either reached our target or stopped short, so either way the point we reached is now our target
	row_end = row_target
	col_end = col_target
	# Our first animation frame will be the grids we affected with a beam (if any)
	frame_indices[0] = 0
	frame_indices[1] = affected_tiles
	var distance_hack: int = distance_travelled
	# Only process an impact if we didn't run out of range
	if distance_travelled <= DungeonLevel.MAX_SPELL_RANGE:
		# If we're a beam then revert the last tile affected so we don't double-dip with the impact
		if flag & ProjectionFlag.BEAM != 0 and affected_tiles > 0:
			affected_tiles -= 1
		# If we are a breath weapon, we need to travel down the line expanding a circle
		if breath:
			var breath_radius: int = 0
			var breath_distance: int = 0
			var done: bool = false
			flag &= ~ProjectionFlag.HIDE
			var row_breath: int = row_start
			var col_breath: int = col_start
			while breath_distance <= distance_travelled + radius:
				for cone_distance: int in breath_radius + 1:
					# Check all tiles in the current radius for range and line of sight to the centre
					for row_check: int in range(row_breath - cone_distance, row_breath + cone_distance + 1):
						for col_check: int in range(col_breath - cone_distance, col_breath + cone_distance + 1):
							if not dungeon_level.in_inner_bounds(row_check, col_check):
								continue
							if dungeon_level.distance(row_start, col_start, row_check, col_check) != breath_distance:
								continue
							if dungeon_level.distance(row_breath, col_breath, row_check, col_check) != cone_distance:
								continue
							if not dungeon_level.line_of_sight(row_breath, col_breath, row_check, col_check):
								continue
							# If the tile is affected, add it to the list
							affected_rows[affected_tiles] = row_check
							affected_cols[affected_tiles] = col_check
							affected_tiles += 1
				# Mark where the next frame in the list will start
				frame_indices[breath_distance + 1] = affected_tiles
				# Check for reaching the end of the breath
				if row_breath == row_end and col_breath == col_end:
					done = true
				if done:
					breath_distance += 1
					continue
				# Move the breath along the line
				var location: Location = dungeon_level.move_one_step_towards(row_breath, col_breath, row_start, col_start, row_end, col_end)
				row_breath = location.row
				col_breath = location.col
				breath_distance += 1
				# Expand the radius proportionally
				breath_radius = radius * breath_distance / distance_travelled
			# Re-set the frame count to however many frames we used
			frame_count = breath_distance
		else:
			# We are a ball, not a breath (a non-ball is technically a ball with 0 radius
			# Expand the radius outwards in an explosion
			for ball_distance: int in radius + 1:
				# Check all tiles in the current radius for range and line of sight to the centre
				for row_check: int in range(row_end - ball_distance, row_end + ball_distance + 1):
					for col_check: int in range(col_end - ball_distance, col_end + ball_distance + 1):
						if not dungeon_level.in_outer_bounds(row_check, col_check):
							continue
						if dungeon_level.distance(row_end, col_end, row_check, col_check) != ball_distance:
							continue
						if get_class() == "ProjectileDisintegrate":
							if dungeon_level.cave_valid_bold(row_check, col_check):
								dungeon_level.revert_tile_to_background(row_check, col_check)
							player.updated_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_MONSTERS)
						else:
							if not dungeon_level.line_of_sight(row_end, col_end, row_check, col_check):
								continue
						# If the tile is affected, add it to the list
						affected_rows[affected_tiles] = row_check
						affected_cols[affected_tiles] = col_check
						affected_tiles += 1
				# Mark where the next frame in the list will start
				frame_indices[ball_distance + 1] = affected_tiles
	# We have done our ball or cone, so return if we didn't affect any tiles
	if affected_tiles == 0:
		return false
	# We now have a list of affected tiles at each frame, if we're not blind draw it a frame at a time
	# Note that frames are drawn over each other and only erased at the end
	if not blind and flag & ProjectionFlag.HIDE == 0:
		for t: int in frame_count + 1:
			# Loop through the affected tiles for this frame
			for i: int in range(frame_indices[t], frame_indices[t + 1]):
				row_target = affected_rows[i]
				col_target = affected_cols[i]
				# If the tile is visible, draw the impact character at it
				if dungeon_level.player_has_los_bold(row_target, col_target) and dungeon_level.panel_contains(row_target, col_target):
					if impact_entity != null:
						drawn = true
						dungeon_level.print_character_at_map_location(impact_entity.character, impact_entity.colour, row_target, col_target)
			# If we drew something this frame, wait a tick for the animation
			if impact_entity != null:
				if visual or drawn:
					User.wait_for_animation()
		# If we drew on at least one tile, wipe them all after we've finished
		if drawn:
			for i: int in affected_tiles:
				row_target = affected_rows[i]
				col_target = affected_cols[i]
				if dungeon_level.player_has_los_bold(row_target, col_target) and dungeon_level.panel_contains(row_target, col_target):
					dungeon_level.redraw_single_location(row_target, col_target)
	# We've finished drawing the bolt/beam/ball/cone, but we might need an animation afterwards
	if animation_entity != null:
		animation_entity.animate_cells(dungeon_level, affected_rows, affected_cols)
	# All drawing is now finished, so actually affect the affected tiles
	# If we affect floor tiles, do so
	if flag & ProjectionFlag.GRID != 0:
		distance_travelled = 0
		for i: int in affected_tiles:
			if frame_indices[distance_travelled + 1] == i:
				distance_travelled += 1
			row_target = affected_rows[i]
			col_target = affected_cols[i]
			# Try affecting the floor (virtual function call overridden by the specific projectile type)
			if affect_floor(row_target, col_target):
				notice = true
	# If we affect items, do so
	if flag & ProjectionFlag.ITEM != 0:
		distance_travelled = 0
		for i: int in affected_tiles:
			if frame_indices[distance_travelled + 1] == i:
				distance_travelled += 1
			row_target = affected_rows[i]
			col_target = affected_cols[i]
			# Try affecting the item (virtual function call overridden by the specific projectile type)
			if affect_item(who, row_target, col_target):
				notice = true
	# If we affect monsters, then do so
	if flag & ProjectionFlag.KILL != 0:
		projectile_mon_num = 0
		projectile_mon_col = 0
		projectile_mon_row = 0
		distance_travelled = 0
		for i: int in affected_tiles:
			# Track the distance because it will affect damage
			if frame_indices[distance_travelled + 1] == i:
				distance_travelled += 1
			row_target = affected_rows[i]
			col_target = affected_cols[i]
			# If we are not at the epicentre of the blast, just affect any monster there
			if affected_tiles > 1:
				# Try affecting the monster (virtual function call overridden by the specific projectile type)
				if affect_monster(who, distance_travelled, row_target, col_target, dam):
					notice = true
			else:
				# We are at the epicentre of the blast, so the monster could reflect it
				if tile.monster_index == 0:
					continue
				var reflecting_mon_type: MonsterType = dungeon_level.dungeon_monsters.monsters[tile.monster_index].monster_type
				# If the monster has reflection and the projectile is not a Wizard Bolt, then possible reflect it
				if reflecting_mon_type.flags_2.is_set(MonsterFlags2.REFLECTING) and Rng.roll_die(10) != 1 and distance_hack > 1 and get_class() != "ProjectileWizardBolt":
					var row_t: int = row_saver - 1 + Rng.roll_die(3)
					var col_t: int = col_saver - 1 + Rng.roll_die(3)
					var maxAttempts: int = 10
					while maxAttempts > 0 and dungeon_level.in_outer_bounds(row_t, col_t) and not dungeon_level.line_of_sight(row_target, col_target, row_t, col_t):
						row_t = row_saver - 1 + Rng.roll_die(3)
						col_t = col_saver - 1 + Rng.roll_die(3)
						maxAttempts -= 1
					if maxAttempts < 1:
						row_t = row_saver
						col_t = col_saver
					# Let the player learn that the monster reflects bolts and balls
					if dungeon_level.dungeon_monsters.monsters[tile.monster_index].is_visible:
						Profile.instance.message_print("The attack bounces!")
						reflecting_mon_type.knowledge.r_flags_2.set_bits(MonsterFlags2.REFLECTING)
					# Recursively call this function to re-fire the bolt to its new destination
					fire(tile.monster_index, 0, row_t, col_t, dam, flag)
				else:
					# The monster did not reflect the bolt/ball
					# Try affecting the monster (virtual function call overridden by the specific projectile type)
					if affect_monster(who, distance_travelled, row_target, col_target, dam):
						notice = true
		# If the plater shot the projectile and it only hit one monster, track that monster
		if who == 0 and projectile_mon_num == 1 and (flag & ProjectionFlag.JUMP) == 0:
			col_target = projectile_mon_col
			row_target = projectile_mon_row
			tile = dungeon_level.get_grid(row_target, col_target)
			if tile.monster_index != 0:
				var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
				if monster.is_visible:
					savegame.health_track(tile.monster_index)
	# If the bolt affects monsters, check to see if the player is caught in it
	if flag & ProjectionFlag.KILL != 0:
		distance_travelled = 0
		for i: int in affected_tiles:
			if frame_indices[distance_travelled + 1] == i:
				distance_travelled += 1
			row_target = affected_rows[i]
			col_target = affected_cols[i]
			# Try affecting the player (virtual function call overridden by the specific projectile type)
			if affect_player(who, distance_travelled, row_target, col_target, dam, radius):
				notice = true
	# Return whether or not we saw what the projetile did (for purposes of identifying wands that fire them)
	return notice


func override_graphics(new_bolt_graphic: String, new_impact_graphic: String, new_effect_animation: String) -> void:
	bolt_graphic = new_bolt_graphic
	impact_graphic = new_impact_graphic
	effect_animation = new_effect_animation


func affect_floor(row_target: int, col_target: int) -> bool:
	return false


func affect_item(who: int, row_target: int, col_target: int) -> bool:
	return false


func affect_monster(who: int, r: int, row_target: int, col_target: int, dam: int) -> bool:
	return false


func affect_player(who: int, r: int, row_target: int, col_target: int, dam: int, rad_a: int) -> bool:
	return false


func bolt_char(row_target: int, col_target: int, row_n: int, col_n: int) -> String:
	if row_n == row_target and col_n == col_target:
		return "*"
	if row_n == row_target:
		return "-"
	if col_n == col_target:
		return "|"
	if row_n - row_target == col_target - col_n:
		return "/"
	if row_n - row_target == col_n - col_target:
		return "\\"
	return "*"
