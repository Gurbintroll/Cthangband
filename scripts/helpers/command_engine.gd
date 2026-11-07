# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CommandEngine
extends RefCounted

static var fear: bool
static var no_extras: bool

var player: Player
var dungeon_level: DungeonLevel
var savegame: SaveGame
var _auto_navigator: AutoNavigator


func move_player(direction: int, pick_up: bool) -> void:
	var can_pass_walls: bool = false
	var new_row: int = player.map_row + dungeon_level.keypad_direction_row_offset[direction]
	var new_col: int = player.map_col + dungeon_level.keypad_direction_col_offset[direction]
	var tile: GridTile = dungeon_level.get_grid(new_row, new_col)
	var player_tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	# Check if we can pass through walls
	if player.timed_etherealness != 0 or player.ancestry.is_incorporeal:
		can_pass_walls = true
		# Permanent features can't be passed through even if we could otherwise
		if tile.feature_type.is_permanent:
			can_pass_walls = false
	# If there's a monster we can see or an invisible monster on a tile we can move to,
	# deal with it
	if tile.monster_index != 0 and (monster.is_visible or dungeon_level.grid_passable(new_row, new_col) or can_pass_walls):
		# Check if it's a friend, and if we are in a fit state to distinguish friend from foe
		if (monster.mind.is_set(MonsterMind.FRIENDLY) and
			not (player.timed_confusion != 0 or player.timed_hallucinations != 0 or not monster.is_visible or player.timed_stun != 0) and
			(dungeon_level.grid_passable(new_row, new_col) or can_pass_walls) ):
			# Wake up the monster, and track it
			monster.sleep_level = 0
			var monster_name: String = monster.describe(0)
			savegame.health_track(tile.monster_index)
			# If we can't see it then let us push past it and tell us what happened
			if (dungeon_level.grid_passable(player.map_row, player.map_col) or
				monster.monster_type.flags_2.is_set(MonsterFlags2.PASSWALL) ):
				Profile.instance.message_print("You push past %s." % [monster_name])
				monster.map_row = player.map_row
				monster.map_col = player.map_col
				player_tile.monster_index = tile.monster_index
				tile.monster_index = 0
				dungeon_level.dungeon_monsters.update_monster_visibility(dungeon_level.get_grid(player.map_row, player.map_col).monster_index, true)
			# If we couldn't push past it, tell us it was in the way
			else:
				Profile.instance.message_print("%s is in your way!" % [monster_name])
				savegame.energy_use = 0
				return
		# If the monster wasn't friendly, attack it
		else:
			player_attack_monster(new_row, new_col)
			return
	# We didn't attack a monster or get blocked by one, so start testing terrain features
	if pick_up and tile.feature_type.is_trap:
		# If we're walking onto a known trap, assume we're trying to disarm it
		disarm_trap(new_row, new_col, direction)
		return
	# If the tile we're moving onto isn't passable then we can't move onto it
	if not dungeon_level.grid_passable(new_row, new_col) and not can_pass_walls:
		savegame.disturb(false)
		# If we can't see it and haven't memorised it, tell us what we bumped into
		if (tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED) and
			(player.timed_blindness != 0 or tile.tile_flags.is_clear(GridTile.PLAYER_LIT)) ):
			if tile.feature_type.name == "Rubble":
				Profile.instance.message_print("You feel some rubble blocking your way.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			elif tile.feature_type.category == FloorTileTypeCategory.TREE:
				Profile.instance.message_print("You feel a %s blocking your way." % [tile.feature_type.description])
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			elif tile.feature_type.name == "Pillar":
				Profile.instance.message_print("You feel a pillar blocking your way.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			elif tile.feature_type.name.contains("Water"):
				Profile.instance.message_print("Your way seems to be blocked by water.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			# If we're moving onto a border, change wilderness location
			elif tile.feature_type.name.contains("Border"):
				if savegame.island.get_region(player.wilderness_row, player.wilderness_col).town != null:
					savegame.current_town = savegame.island.get_region(player.wilderness_row, player.wilderness_col).town
					Profile.instance.message_print("You stumble out of %s." % [savegame.current_town.name])
				if new_row == 0:
					player.map_row = dungeon_level.cur_hgt - 2
					player.wilderness_row -= 1
				if new_row == dungeon_level.cur_hgt - 1:
					player.map_row = 1
					player.wilderness_row += 1
				if new_col == 0:
					player.map_col = dungeon_level.cur_wid - 2
					player.wilderness_col -= 1
				if new_col == dungeon_level.cur_wid - 1:
					player.map_col = 1
					player.wilderness_col += 1
				if savegame.island.get_region(player.wilderness_row, player.wilderness_col).town != null:
					savegame.current_town = savegame.island.get_region(player.wilderness_row, player.wilderness_col).town
					Profile.instance.message_print("You stumble into %s." % [savegame.current_town.name])
					savegame.current_town.visited = true
				# We'll need a new level
				savegame.new_level_flag = true
				savegame.came_from = LevelStart.START_WALK
				savegame.is_auto_save = true
				savegame.command_save_game()
				savegame.is_auto_save = false
			elif tile.feature_type.is_closed_door:
				Profile.instance.message_print("You feel a closed door blocking your way.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			else:
				Profile.instance.message_print("You feel a %s blocking your way." % [tile.feature_type.description])
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
		# We can see it, so give a different message
		else:
			if tile.feature_type.name == "Rubble":
				Profile.instance.message_print("There is rubble blocking your way.")
				if (not (player.timed_confusion != 0 or player.timed_stun != 0 or player.timed_hallucinations != 0)):
					savegame.energy_use = 0
			elif tile.feature_type.category == FloorTileTypeCategory.TREE:
				Profile.instance.message_print("There is a %s blocking your way." % [tile.feature_type.description])
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			elif tile.feature_type.name == "Pillar":
				Profile.instance.message_print("There is a pillar blocking your way.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			elif tile.feature_type.name.contains("Water"):
				Profile.instance.message_print("You cannot swim.")
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(new_row, new_col)
			# Again, walking onto a border means a change of wilderness grid
			elif tile.feature_type.name.contains("Border"):
				if savegame.island.get_region(player.wilderness_row, player.wilderness_col).town != null:
					savegame.current_town = savegame.island.get_region(player.wilderness_row, player.wilderness_col).town
					Profile.instance.message_print("You leave %s." % [savegame.current_town.name])
				if new_row == 0:
					player.map_row = dungeon_level.cur_hgt - 2
					player.wilderness_row -= 1
				if new_row == dungeon_level.cur_hgt - 1:
					player.map_row = 1
					player.wilderness_row += 1
				if new_col == 0:
					player.map_col = dungeon_level.cur_wid - 2
					player.wilderness_col -= 1
				if new_col == dungeon_level.cur_wid - 1:
					player.map_col = 1
					player.wilderness_col += 1
				if savegame.island.get_region(player.wilderness_row, player.wilderness_col).town != null:
					savegame.current_town = savegame.island.get_region(player.wilderness_row, player.wilderness_col).town
					Profile.instance.message_print("You enter %s." % [savegame.current_town.name])
					savegame.current_town.visited = true
				# We'll need a new level
				savegame.new_level_flag = true
				savegame.came_from = LevelStart.START_WALK
				savegame.is_auto_save = true
				savegame.command_save_game()
				savegame.is_auto_save = false
			# If we can see that we're walking into a closed door, try to open it
			elif tile.feature_type.is_closed_door:
				if easy_open_door(new_row, new_col):
					return
			else:
				Profile.instance.message_print("There is a %s blocking your way." % [tile.feature_type.description])
				if (not (player.timed_confusion != 0 or player.timed_stun != 0 or player.timed_hallucinations != 0)):
					savegame.energy_use = 0
		return
	# Assuming we didn't bump into anything, maybe we can actually move
	var old_traps_detected: bool = player_tile.tile_flags.is_set(GridTile.TRAPS_DETECTED)
	var new_traps_detected: bool = tile.tile_flags.is_set(GridTile.TRAPS_DETECTED)
	# If we're moving into or out of an area where we've detected traps, remember to redraw
	# the notification
	if old_traps_detected != new_traps_detected:
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_DTRAP)
	# If we're leaving an area where we've detected traps at a run, then stop running
	if savegame.running != 0 and old_traps_detected and not new_traps_detected:
		if (not (player.timed_confusion != 0 or player.timed_stun != 0 or player.timed_hallucinations != 0)):
			savegame.energy_use = 0
		savegame.disturb(false)
		return
	# We've run out of things that could prevent us moving, so do the move
	var old_row: int = player.map_row
	var old_col: int = player.map_col
	player.map_row = new_row
	player.map_col = new_col
	# Redraw our old and new locations
	dungeon_level.redraw_single_location(player.map_row, player.map_col)
	dungeon_level.redraw_single_location(old_row, old_col)
	# Recenter the screen if we have to
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	target_engine.recenter_screen_around_player()
	# We'll need to update and redraw various things
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	# If we're not actively searching, then have a chance of doing it passively
	if player.skill_search_frequency >= 50 or Rng.roll_die(50 - player.skill_search_frequency) == 1:
		search()
	# If we're actively searching then always do it
	if player.is_searching:
		search()
	# Pick up an object on our tile if there is one
	pick_up_items(pick_up)
	# If we've just entered a shop tile, then enter the actual shop
	if tile.feature_type.is_shop:
		savegame.disturb(false)
		User.queued_command = "_"
	# If we've just stepped on an unknown trap then activate it
	elif tile.feature_type.name == "Invis":
		savegame.disturb(false)
		Profile.instance.message_print("You found a trap!")
		savegame.dungeon_level.pick_trap(player.map_row, player.map_col)
		step_on_trap()
	# If it's a trap we couldn't (or didn't) disarm, then activate it
	elif tile.feature_type.is_trap:
		savegame.disturb(false)
		step_on_trap()


func step_on_trap() -> void:
	var damage:int
	var name:String = "a trap"
	savegame.disturb(false)
	var tile:GridTile = dungeon_level.get_grid(player.map_row,player.map_col)
	# Check the type of trap
	match tile.feature_type.name:
		"TrapDoor":
			# Trap doors can be flown over with feather fall
			if player.has_feather_fall:
				Profile.instance.message_print("You fly over a trap door.")
			else:
				Profile.instance.message_print("You fell through a trap door!")
				# Trap doors do 2d8 fall damage
				damage = Rng.roll_dice(2, 8)
				name = "a trap door"
				player.take_hit(damage, name)
				# Even if we survived, we need a new level
				if player.health >= 0:
					savegame.is_auto_save = true
					savegame.command_save_game()
					savegame.is_auto_save = false
				savegame.new_level_flag = true
				# In dungeons we fall to a deeper level, but in towers we fall to a
				# shallower level because they go up instead of down
				if savegame.current_dungeon.is_tower:
					savegame.current_depth -= 1
				else:
					savegame.current_depth += 1
		"Pit":
			# A pit can be flown over with feather fall
			if player.has_feather_fall:
				Profile.instance.message_print("You fly over a pit trap.")
			else:
				Profile.instance.message_print("You fell into a pit!")
				# Pits do 2d6 fall damage
				damage = Rng.roll_dice(2, 6)
				name = "a pit trap"
				player.take_hit(damage, name)
		"SpikedPit":
			# A pit can be flown over with feather fall
			if player.has_feather_fall:
				Profile.instance.message_print("You fly over a spiked pit.")
			else:
				Profile.instance.message_print("You fall into a spiked pit!")
				name = "a pit trap"
				# A pit does 2d6 fall damage
				damage = Rng.roll_dice(2, 6)
				# 50% chance of doing double damage plus bleeding
				if Rng.roll_die(100) <= 50:
					Profile.instance.message_print("You are impaled!")
					name = "a spiked pit"
					damage *= 2
					player.set_timed_bleeding(player.timed_bleeding + Rng.roll_die(damage))
				player.take_hit(damage, name)
		"PoisonPit":
			# A pit can be flown over with feather fall
			if player.has_feather_fall:
				Profile.instance.message_print("You fly over a spiked pit.")
			else:
				Profile.instance.message_print("You fall into a spiked pit!")
				# A pit does 2d6 fall damage
				damage = Rng.roll_dice(2, 6)
				name = "a pit trap"
				# 50% chance of doing double damage plus bleeding plus poison
				if Rng.roll_die(100) <= 50:
					Profile.instance.message_print("You are impaled on poisonous spikes!")
					name = "a spiked pit"
					damage *= 2
					player.set_timed_bleeding(player.timed_bleeding + Rng.roll_die(damage))
					# Hagarg Ryonis can save us from the poison
					if player.has_poison_resistance or player.timed_poison_resistance != 0:
						Profile.instance.message_print("The poison does not affect you!")
					elif Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_power():
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					else:
						damage *= 2
						player.set_timed_poison(player.timed_poison + Rng.roll_die(damage))
				player.take_hit(damage, name)
		"SummonRune":
			Profile.instance.message_print("There is a flash of shimmering light!")
			# Trap disappears when triggered
			tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
			dungeon_level.revert_tile_to_background(player.map_row, player.map_col)
			# Summon 1d3+2 monsters
			var num: int = 2 + Rng.roll_die(3)
			for i:int in num:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), 0)
			# Have a chance of also cursing the player
			if savegame.total_difficulty() > Rng.roll_die(100):
				savegame.activate_dread_curse()
				while Rng.roll_die(6) == 1:
					savegame.activate_dread_curse()
		"TeleportRune":
			# Teleport the player up to 100 squares
			Profile.instance.message_print("You hit a teleport trap!")
			savegame.spell_effects_handler.teleport_player(100)
		"FireTrap":
			# Do 4d6 fire damage
			Profile.instance.message_print("You are enveloped in flames!")
			damage = Rng.roll_dice(4, 6)
			savegame.spell_effects_handler.fire_dam(damage, "a fire trap")
		"AcidTrap":
			# Do 4d6 acid damage
			Profile.instance.message_print("You are splashed with acid!")
			damage = Rng.roll_dice(4, 6)
			savegame.spell_effects_handler.acid_dam(damage, "an acid trap")
		"SlowDart":
			# Dart traps need a to-hit roll
			if trap_check_hit_on_player(125):
				Profile.instance.message_print("A small dart hits you!")
				# Do 1d4 damage plus slow
				damage = Rng.roll_dice(1, 4)
				player.take_hit(damage, name)
				player.set_timed_slow(player.timed_slow + Rng.roll_die(20) + 19)
			else:
				Profile.instance.message_print("A small dart barely misses you.")
		"StrDart":
			# Dart traps need a to-hit roll
			if trap_check_hit_on_player(125):
				Profile.instance.message_print("A small dart hits you!")
				# Do 1d4 damage plus strength drain
				damage = Rng.roll_dice(1, 4)
				player.take_hit(damage, "a dart trap")
				player.try_decreasing_ability_score(Ability.STRENGTH)
			else:
				Profile.instance.message_print("A small dart barely misses you.")
		"DexDart":
			# Dart traps need a to-hit roll
			if trap_check_hit_on_player(125):
				Profile.instance.message_print("A small dart hits you!")
				# Do 1d4 damage plus dexterity drain
				damage = Rng.roll_dice(1, 4)
				player.take_hit(damage, "a dart trap")
				player.try_decreasing_ability_score(Ability.DEXTERITY)
			else:
				Profile.instance.message_print("A small dart barely misses you.")
		"ConDart":
			# Dart traps need a to-hit roll
			if trap_check_hit_on_player(125):
				Profile.instance.message_print("A small dart hits you!")
				# Do 1d4 damage plus constitution drain
				damage = Rng.roll_dice(1, 4)
				player.take_hit(damage, "a dart trap")
				player.try_decreasing_ability_score(Ability.CONSTITUTION)
			else:
				Profile.instance.message_print("A small dart barely misses you.")
		"BlindGas":
			# Blind the player
			Profile.instance.message_print("A black gas surrounds you!")
			if not player.has_blindness_resistance:
				player.set_timed_blindness(player.timed_blindness + Rng.roll_die(50) + 24)
		"ConfuseGas":
			# Confuse the player
			Profile.instance.message_print("A gas of scintillating colours surrounds you!")
			if not player.has_confusion_resistance:
				player.set_timed_confusion(player.timed_confusion + Rng.roll_die(20) + 9)
		"PoisonGas":
			# Poison the player
			Profile.instance.message_print("A pungent green gas surrounds you!")
			if not player.has_poison_resistance and player.timed_poison_resistance == 0:
				# Hagarg Ryonis may save you from the poison
				if Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
					Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
				else:
					player.set_timed_poison(player.timed_poison + Rng.roll_die(20) + 9)
		"SleepGas":
			# Paralyse the player
			Profile.instance.message_print("A strange white mist surrounds you!")
			if not player.has_free_action:
				player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(10) + 4)


func pick_up_items(pick_up: bool) -> void:
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	var item_index: int = tile.item_index
	var next: int
	while item_index != 0:
		var item: Item = dungeon_level.dungeon_items.items[item_index]
		next = item.next_in_stack
		var item_name: String = item.description(true, 3)
		savegame.disturb(false)
		# We always pick up gold
		if item.category == ItemCategory.GOLD:
			Profile.instance.message_print("You collect %d gold pieces worth of %s." % [item.type_specific_value, item_name])
			player.gold += item.type_specific_value
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_GOLD)
			dungeon_level.delete_item_by_index(item_index)
		else:
			# If we're not interested, simply say that we see it
			if not pick_up:
				Profile.instance.message_print("You see %s." % [item_name])
			# If it's worthless, stomp on it
			elif item.stompable():
				dungeon_level.delete_item_by_index(item_index)
				Profile.instance.message_print("You stomp on %s." % [item_name])
				# If we can't carry the item, let us know
			elif not player.inventory.inven_carry_okay(item):
				Profile.instance.message_print("You have no room for %s." % [item_name])
			else:
				# Actually pick up the item
				var slot: int = player.inventory.inven_carry(item)
				item = player.inventory.items[slot]
				item_name = item.description(true, 3)
				Profile.instance.message_print("You have %s (%s)." % [item_name, Util.index_to_label(slot)])
				dungeon_level.delete_item_by_index(item_index)
		item_index = next


func search() -> void:
	# The basic chance is equal to our searching skill
	var chance: int = player.skill_searching
	# If we can't see it's hard to search
	if player.timed_blindness != 0 or dungeon_level.no_light():
		chance /= 10
	# If we're confused it's hard to search
	if player.timed_confusion != 0 or player.timed_hallucinations != 0:
		chance /= 10
	# Check the eight squares around us
	for row: int in range(player.map_row - 1, player.map_row + 2):
		for col: int in range(player.map_col - 1, player.map_col + 2):
			# Check if we succeed
			if Rng.roll_die(100) <= chance:
				# If there's a trap, then find it
				var tile: GridTile = dungeon_level.get_grid(row, col)
				if tile.feature_type.name == "Invis":
					# Pick a random trap to replace the undetected one with
					dungeon_level.pick_trap(row, col)
					Profile.instance.message_print("You have found a trap.")
					savegame.disturb(false)
				if tile.feature_type.name == "SecretDoor":
					# Replace the secret door with a visible door
					Profile.instance.message_print("You have found a secret door.")
					player.gain_experience(1)
					dungeon_level.replace_secret_door(row, col)
					savegame.disturb(false)
				var item_index: int = tile.item_index
				# Check the items on the tile
				while item_index != 0:
					var item: Item = dungeon_level.dungeon_items.items[item_index]
					item_index = item.next_in_stack
					# If one of them is a chest, determine if it is trapped
					if item.category != ItemCategory.CHEST:
						continue
					if item.type_specific_value <= 0:
						continue
					if Tables.CHEST_TRAPS[item.type_specific_value] == 0:
						continue
					# It was a trapped chest - if we didn't already know that then let us know
					if not item.is_known():
						Profile.instance.message_print("You have discovered a trap on the chest!")
						item.BecomeKnown()
						savegame.disturb(false)


func player_attack_monster(row: int, col: int) -> void:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	var monster_type: MonsterType = monster.monster_type
	fear = false
	var backstab: bool = false
	var stab_fleeing: bool = false
	var do_quake: bool = false
	var drain_result: int = 0
	const MAX_HEALTH_DRAIN: int = 100
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	no_extras = false
	savegame.disturb(false)
	# If we're a rogue then we can backstab monsters
	if player.profession.has_backstab:
		if monster.sleep_level != 0 and monster.is_visible:
			backstab = true
		elif monster.fear_level != 0 and monster.is_visible:
			stab_fleeing = true
	savegame.disturb(true)
	# Being attacked always wakes a monster
	monster.sleep_level = 0
	var monster_name: String = monster.describe(0)
	# If we can see the monster, track its health
	if monster.is_visible:
		savegame.health_track(tile.monster_index)
	# if the monster is our friend and we're not confused, we can avoid hitting it
	if monster.mind.is_set(MonsterMind.FRIENDLY) and not (player.timed_stun != 0 or player.timed_confusion != 0 or player.timed_hallucinations != 0 or !monster.is_visible):
		Profile.instance.message_print("You stop to avoid hitting %s." % [monster_name])
		return
	# Can't attack if we're afraid
	if player.timed_fear != 0:
		Profile.instance.message_print("You are too afraid to attack %s!" % [monster_name] if monster.is_visible else "There is something scary in your way!")
		return
	var item: Item = player.inventory.items[InventorySlot.MELEE_WEAPON]
	var bonus: int = player.attack_bonus + item.bonus_to_hit
	var chance: int = player.skill_melee + (bonus * 3)
	# Attacking uses a full turn
	savegame.energy_use = 100
	var num: int = 0
	# We have a number of attacks per round
	while num < player.melee_attacks_per_round:
		num += 1
		# Check if we hit
		if player_check_hit_on_monster(chance, monster_type.ac, monster.is_visible):
			Console.play_sound("AttackHit")
			var player_status: PlayerStatus = PlayerStatus.new(player, dungeon_level)
			# Tell the player they hit it with the appropriate message
			if not (backstab or stab_fleeing):
				if not (player.profession.is_martial_artist and player_status.martial_arts_empty_hands()):
					Profile.instance.message_print("You hit %s." % [monster_name])
			elif backstab:
				Profile.instance.message_print("You cruelly stab the helpless, sleeping %s!" % [monster.monster_type.name])
			else:
				Profile.instance.message_print("You backstab the fleeing %s!" % [monster.monster_type.name])
			# Default to 1 damage for an unarmed hit
			var total_damage: int = 1
			# Get our weapon's flags to see if we need to do anything special
			item.get_merged_flags(f_1, f_2, f_3)
			var chaos_effect: bool = f_1.is_set(ItemFlags1.CHAOTIC) and Rng.roll_die(2) == 1
			if f_1.is_set(ItemFlags1.VAMPIRIC) or (chaos_effect and Rng.roll_die(5) < 3):
				# Vampiric overrides chaotic
				chaos_effect = false
				if not (monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or (monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING))):
					drain_result = monster.health
				else:
					drain_result = 0
			# VORPAL weapons have a chance of a deep cut
			var vorpal_cut: bool = f_1.is_set(ItemFlags1.VORPAL) and Rng.roll_die(3 if item.artifact_index == ArtifactId.SWORD_TWILIGHT else 6) == 1
			# If we're a martial artist then we have special attacks
			if player.profession.is_martial_artist and player_status.martial_arts_empty_hands():
				var special_effect: int = 0
				var stun_effect: int = 0
				var martial_arts_attack: MartialArtsAttack = Tables.martial_arts_attacks[0]
				var old_martial_arts_attack: MartialArtsAttack = Tables.martial_arts_attacks[0]
				# Monsters of various types resist being stunned by martial arts
				var resist_stun: int = 0
				if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
					resist_stun += 88
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION):
					resist_stun += 44
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_SLEEP):
					resist_stun += 44
				if monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING):
					resist_stun += 88
				# Have a number of attempts to choose a martial arts attack
				for times: int in 1 if player.level < 7 else player.level / 7:
					# Choose an attack randomly, but reject it and re-choose if it's too
					# high level or we fail a chance roll
					martial_arts_attack = Tables.martial_arts_attacks[Rng.roll_die(Tables.martial_arts_attacks.size()) - 1]
					while martial_arts_attack.min_level > player.level or Rng.roll_die(player.level) < martial_arts_attack.chance:
						martial_arts_attack = Tables.martial_arts_attacks[Rng.roll_die(Tables.martial_arts_attacks.size()) - 1]
					# We've chosen an attack, use it if it's better than the previous
					# choice (unless we're stunned or confused in which we're stuck
					# with the weakest attack type
					if martial_arts_attack.min_level > old_martial_arts_attack.min_level and not (player.timed_stun != 0 or player.timed_confusion != 0):
						old_martial_arts_attack = martial_arts_attack
					else:
						martial_arts_attack = old_martial_arts_attack
				# Get damage from the martial arts attack
				total_damage = Rng.roll_dice(martial_arts_attack.damage_dice, martial_arts_attack.damage_dice_sides)
				# If it was a knee attack and the monster is male, hit it in the groin
				if martial_arts_attack.effect == MartialArtsAttack.MA_KNEE:
					if monster_type.flags_1.is_set(MonsterFlags1.MALE):
						Profile.instance.message_print("You hit %s in the groin with your knee!" % [monster_name])
						special_effect = MartialArtsAttack.MA_KNEE
					else:
						Profile.instance.message_print(martial_arts_attack.description % [monster_name])
				# If it was an ankle kick and the monster has legs, slow it
				elif martial_arts_attack.effect == MartialArtsAttack.MA_SLOW:
					if monster_type.flags_1.is_clear(MonsterFlags1.NEVER_MOVE) and not "UjmeEv$,DdsbBFIJQSXclnw!=?".contains(monster_type.character):
						Profile.instance.message_print("You kick %s in the ankle." % [monster_name])
						special_effect = MartialArtsAttack.MA_SLOW
					else:
						Profile.instance.message_print(martial_arts_attack.description % [monster_name])
				# Have a chance of stunning based on the martial arts attack type chosen
				else:
					if martial_arts_attack.effect != 0:
						stun_effect = (martial_arts_attack.effect / 2) + Rng.roll_die(martial_arts_attack.effect / 2)
					Profile.instance.message_print(martial_arts_attack.description % [monster_name])
				# It might be a critical hit
				total_damage = player_critical_melee(player.level * Rng.roll_die(10), martial_arts_attack.min_level, total_damage)
				# Make a groin attack into a stunning attack
				if special_effect == MartialArtsAttack.MA_KNEE and total_damage + player.damage_bonus < monster.health:
					Profile.instance.message_print("%s moans in agony!" % [monster_name])
					stun_effect = 7 + Rng.roll_die(13)
					resist_stun /= 3
				# Slow if we had a knee attack
				elif special_effect == MartialArtsAttack.MA_SLOW and total_damage + player.damage_bonus < monster.health:
					if monster_type.flags_1.is_clear(MonsterFlags1.UNIQUE) and Rng.roll_die(player.level) > monster_type.level and monster.speed > 60:
						Profile.instance.message_print("%s starts limping slower." % [monster_name])
						monster.speed -= 10
				# Stun if we had a stunning attack
				if stun_effect != 0 and total_damage + player.damage_bonus < monster.health:
					if player.level > Rng.roll_die(monster_type.level + resist_stun + 10):
						Profile.instance.message_print("%s is more stunned." % [monster_name] if monster.StunLevel != 0 else "%s is stunned." % [monster_name])
						monster.StunLevel += stun_effect
			# We have a weapon
			elif item.item_type != null:
				# Roll damage for the weapon
				total_damage = Rng.roll_dice(item.damage_dice, item.damage_dice_sides)
				total_damage = item.adjust_damage_for_monster_type(total_damage, monster)
				# Extra damage for backstabbing
				if backstab:
					total_damage *= 3 + (player.level / 40)
				elif stab_fleeing:
					total_damage = 3 * total_damage / 2
				# We might need to do an earthquake, which overrides chaos effects
				if (player.has_quake_weapon and (total_damage > 50 or Rng.roll_die(7) == 1)) or (chaos_effect and Rng.roll_die(250) == 1):
					do_quake = true
					chaos_effect = false
				# Check if we did a critical
				total_damage = player_critical_melee(item.weight, item.bonus_to_hit, total_damage)
				# If we did a vorpal cut, do extra damage
				if vorpal_cut:
					var step_k: int = total_damage
					Profile.instance.message_print("Your Vorpal Blade goes snicker-snack!" if item.artifact_index == ArtifactId.SWORD_VORPAL_BLADE else "Your weapon cuts deep into %s!" % [monster_name])
					total_damage += step_k
					while Rng.roll_die(2 if item.artifact_index == ArtifactId.SWORD_VORPAL_BLADE else 4) == 1:
						total_damage += step_k
				# Add bonus damage for the weapon
				total_damage += item.bonus_damage
			# Add bonus damage for strength etc.
			total_damage += player.damage_bonus
			# Can't do negative damage
			if total_damage < 0:
				total_damage = 0
			# Apply damage to the monster
			if dungeon_level.dungeon_monsters.damage_monster(tile.monster_index, total_damage, ""):
				# Can't have any more attacks because the monster's dead
				no_extras = true
				break
			# Hitting a friend gets it angry
			if monster.mind.is_set(MonsterMind.FRIENDLY):
				Profile.instance.message_print("%s gets angry!" % [monster_name])
				monster.mind.clear_bits(MonsterMind.FRIENDLY)
			# The monster might have an aura that hurts the player
			touch_zap_player(monster)
			# If we drain health, do so
			if drain_result != 0:
				# drain_result was set to the monsters health before we hit it, so
				# subtracting the monster's new health lets us know how much damage we've done
				drain_result -= monster.health
				if drain_result > 0:
					# Draining heals us
					var drain_heal: int = Rng.roll_dice(4, drain_result / 6)
					# We have a maximum drain per attack to prevent it from getting out of
					# hand if we have multiple attacks
					if drain_heal >= MAX_HEALTH_DRAIN:
						drain_heal = MAX_HEALTH_DRAIN
					Profile.instance.message_print("Your weapon drains life from %s!" % [monster_name])
					player.restore_health(drain_heal)
			# We might have a confusing touch (or have this effect from a chaos blade)
			if player.has_confusing_touch or (chaos_effect and Rng.roll_die(10) != 1):
				# If it wasn't from a chaos blade, cancel the confusing touch and let us know
				player.has_confusing_touch = false
				if not chaos_effect:
					Profile.instance.message_print("Your hands stop glowing.")
				# Some monsters are immune
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_CONFUSION)
					Profile.instance.message_print("%s is unaffected." % [monster_name])
				# Even if not immune, the monster might resist
				elif Rng.roll_die(100) <= monster_type.level:
					Profile.instance.message_print("%s is unaffected." % [monster_name])
				# It didn't resist, so it's confused
				else:
					Profile.instance.message_print("%s appears confused." % [monster_name])
					monster.confusion_level += 9 + (Rng.roll_die(player.level) / 5)
			# A chaos blade might teleport the monster away
			elif chaos_effect and Rng.roll_die(2) == 1:
				Profile.instance.message_print("%s disappears!" % [monster_name])
				savegame.spell_effects_handler.teleport_away(tile.monster_index, 50)
				# Can't have any more attacks because the monster isn't here any more
				no_extras = true
				break
			# a chaos blade might polymorph the monster
			elif chaos_effect and dungeon_level.grid_passable(row, col) and Rng.roll_die(90) > monster_type.level:
				# Can't polymorph a unique or a guardian
				if not (monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.flags_4.is_set(MonsterFlags4.BREATHE_CHAOS) or monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN)):
					var new_monster_type_index: int = savegame.spell_effects_handler.polymorph_monster(monster.monster_type)
					if new_monster_type_index != monster.monster_type.index:
						Profile.instance.message_print("%s changes!" % [monster_name])
						dungeon_level.dungeon_monsters.delete_monster_by_index(tile.monster_index, true)
						var new_monster_type: MonsterType = Profile.instance.monster_types.types[new_monster_type_index]
						dungeon_level.dungeon_monsters.place_monster_by_type(row, col, new_monster_type, false, false, false)
						monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
						monster_name = monster.describe(0)
						fear = false
				# Monster was immune to polymorph
				else:
					Profile.instance.message_print("%s is unaffected." % [monster_name])
		# We missed
		else:
			Console.play_sound("AttackMiss")
			Profile.instance.message_print("You miss %s." % [monster_name])
	# Only make extra attacks if the monster is still there
	for natural_attack: Mutation in player.genome.natural_attacks:
		if not no_extras:
			player_natural_attack_monster(tile.monster_index, natural_attack)
	if fear and monster.is_visible and not no_extras:
		Profile.instance.message_print("%s flees in terror!" % [monster_name])
	if do_quake:
		savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 10)


func disarm_trap(row: int, col: int, direction: int) -> int:
	var more: bool = false
	# Disarming a trap costs a turn
	savegame.energy_use = 100
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var trap_name: String = tile.feature_type.description
	var i: int = player.skill_disarm_traps
	# Difficult, but possible, to disarm by feel
	if player.timed_blindness != 0 or dungeon_level.no_light():
		i /= 10
	# Difficult to disarm when we're confused
	if player.timed_confusion != 0 or player.timed_hallucinations != 0:
		i /= 10
	var power: int = 5
	var j: int = i - power
	if j < 2:
		j = 2
	# Check the modified disarm skill
	if Rng.roll_die(100) <= j:
		Profile.instance.message_print("You have disarmed the %s." % [trap_name])
		player.gain_experience(power)
		tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
		dungeon_level.revert_tile_to_background(row, col)
		move_player(direction, true)
	# We might set the trap off if we failed to disarm it
	elif i > 5 and Rng.roll_die(i) > 5:
		Profile.instance.message_print("You failed to disarm the %s." % [trap_name])
		more = true
	else:
		Profile.instance.message_print("You set off the %s!" % [trap_name])
		move_player(direction, true)
	return more


func easy_open_door(row: int, col: int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	# If it isn't closed, we can't open it
	if not tile.feature_type.is_closed_door:
		return false
	# If it's jammed we'll need to bash it
	if tile.feature_type.name.contains("Jammed"):
		Profile.instance.message_print("The door appears to be stuck.")
	# Most doors are locked, so try to pick the lock
	elif not tile.feature_type.name.contains("0"):
		var skill: int = player.skill_disarm_traps
		# Lockpicking is hard in the dark
		if player.timed_blindness != 0 or dungeon_level.no_light():
			skill /= 10
		# Lockpicking is hard when you're confused
		if player.timed_confusion != 0 or player.timed_hallucinations != 0:
			skill /= 10
		var chance: int = int(tile.feature_type.name.substr(10))
		chance = skill - (chance * 4)
		if chance < 2:
			chance = 2
		# See if we succeed
		if Rng.roll_die(100) <= chance:
			Profile.instance.message_print("You have picked the lock.")
			dungeon_level.cave_set_feat(row, col, "OpenDoor")
			player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
			player.gain_experience(1)
		# If we failed, simply let us know
		else:
			Profile.instance.message_print("You failed to pick the lock.")
	# It wasn't locked, so simply open it
	else:
		dungeon_level.cave_set_feat(row, col, "OpenDoor")
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
	return true


func run_one_step(direction: int) -> void:
	if _auto_navigator == null:
		_auto_navigator = AutoNavigator.new()
	if direction != 0:
		# Check if we can actually run in that direction
		if _auto_navigator.see_wall(direction, player.map_row, player.map_col):
			Profile.instance.message_print("You cannot run in that direction.")
			savegame.disturb(false)
			return
		player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
		# Initialise our navigation state
		_auto_navigator.set_direction(direction)
	else:
		# We're already running, so check if we have to stop
		if _auto_navigator.navigate_next_step():
			savegame.disturb(false)
			return
	# Running has a limit, just in case, but in practice we'll never reach it
	savegame.running -= 1
	if savegame.running <= 0:
		return
	# We can run, so use a move's worth of energy and actually make the move
	savegame.energy_use = 100
	move_player(_auto_navigator.current_run_direction, true)


func count_closed_doors(location: Location) -> int:
	var count: int = 0
	for dir: int in 9:
		var row: int = player.map_row + dungeon_level.ordered_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.ordered_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		# We need to be aware of the door
		if tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED):
			continue
		# It needs to be an actual door
		if not tile.feature_type.is_closed_door:
			continue
		# It can't be a secret door
		if tile.feature_type.name == "SecretDoor":
			continue
		count += 1
		# Remember the coordinate
		location.row = row
		location.col = col
	return count


func count_open_doors(location: Location) -> int:
	var count: int = 0
	for dir: int in 9:
		var row: int = player.map_row + dungeon_level.ordered_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.ordered_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		# We need to be aware of the door
		if tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED):
			continue
		# It needs to be an actual door
		if tile.feature_type.name != "OpenDoor":
			continue
		count += 1
		# Remember the coordinate
		location.row = row
		location.col = col
	return count


func count_chests(location: Location, trapped_only: bool) -> int:
	var count: int = 0
	for dir: int in 9:
		var row: int = player.map_row + dungeon_level.ordered_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.ordered_direction_col_offset[dir]
		# Get the index of first item in the tile that is a chest
		var item_index: int = dungeon_level.chest_check(row, col)
		if item_index == 0:
			# There wasn't a chest on the grid so skip
			continue
		# Get the actual item from the index
		var item: Item = dungeon_level.dungeon_items.items[item_index]
		if item.type_specific_value == 0:
			continue
		# If we're only interested in trapped chests, skip those that aren't
		if trapped_only and (not item.is_known() or Tables.CHEST_TRAPS[item.type_specific_value] == 0):
			continue
		count += 1
		# Remember the coordinate
		location.row = row
		location.col = col
	return count


func open_door(row: int, col: int) -> int:
	var more: bool = false
	# Opening a door takes an action
	savegame.energy_use = 100
	var tile: GridTile = dungeon_level.get_grid(row, col)
	# Some doors are simply jammed
	if tile.feature_type.name.contains("Jammed"):
		Console.play_sound("LockedDoor")
		Profile.instance.message_print("The door appears to be stuck.")
	# Some doors are locked
	elif tile.feature_type.name != "LockedDoor0":
		# Our disarm traps skill doubles up as a lockpicking skill
		var i: int = player.skill_disarm_traps
		# Hard to pick locks when you can't see
		if player.timed_blindness != 0 or dungeon_level.no_light():
			i /= 10
		# Hard to pick locks when you're confused or hallucinating
		if player.timed_confusion != 0 or player.timed_hallucinations != 0:
			i /= 10
		# Work out the difficulty from the feature name
		var j: int = int(tile.feature_type.name.substr(10))
		j = i - (j * 4)
		if j < 2:
			j = 2
		# Check if we succeeded in opening it
		if Rng.roll_die(100) <= j:
			Profile.instance.message_print("You have picked the lock.")
			dungeon_level.cave_set_feat(row, col, "OpenDoor")
			player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
			# Picking a lock gains you an experience point
			player.gain_experience(1)
		else:
			Console.play_sound("LockedDoor")
			Profile.instance.message_print("You failed to pick the lock.")
			more = true
	# If the door wasn't locked, simply open it
	else:
		Console.play_sound("OpenDoor")
		dungeon_level.cave_set_feat(row, col, "OpenDoor")
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
	return more


func close_door(row: int, col: int) -> bool:
	savegame.energy_use = 100
	if dungeon_level.get_grid(row, col).feature_type.name == "BrokenDoor":
		Profile.instance.message_print("The door appears to be broken.")
	else:
		dungeon_level.cave_set_feat(row, col, "LockedDoor0")
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
	return false


func get_spike() -> int:
	# Loop through the inventory
	for i: int in InventorySlot.PACK:
		var item: Item = player.inventory.items[i]
		if item.item_type == null:
			continue
		# If the item is a spike, return it
		if item.category == ItemCategory.SPIKE:
			return i
	# We found nothing, so return an invalid number
	return -1


func player_check_hit_on_monster(power: int, armour_class: int, is_visible: int) -> bool:
	# Always have a 5% chance to hit or miss
	var roll: int = Rng.roll_die(100)
	if roll <= 10:
		return roll <= 5
	if power <= 0:
		return false
	# It's hard to hit invisible monsters
	if not is_visible:
		power = (power + 1) / 2
	# Work out whether we hit or not
	return Rng.roll_die(power) > armour_class * 3 / 4


func player_critical_melee(weight: int, plus: int, damage: int) -> int:
	var i: int = weight + ((player.attack_bonus + plus) * 5) + (player.level * 3)
	if Rng.roll_die(5000) <= i:
		var k: int = weight + Rng.roll_die(650)
		if k < 400:
			Profile.instance.message_print("It was a good hit!")
			damage = (2 * damage) + 5
		elif k < 700:
			Profile.instance.message_print("It was a great hit!")
			damage = (2 * damage) + 10
		elif k < 900:
			Profile.instance.message_print("It was a superb hit!")
			damage = (3 * damage) + 15
		elif k < 1300:
			Profile.instance.message_print("It was a *GREAT* hit!")
			damage = (3 * damage) + 20
		else:
			Profile.instance.message_print("It was a *SUPERB* hit!")
			damage = (7 * damage / 2) + 25
	return damage


func touch_zap_player(monster: Monster) -> void:
	var aura_damage: int
	var monster_type: MonsterType = monster.monster_type
	# If we have a fire aura, apply it
	if monster_type.flags_2.is_set(MonsterFlags2.FIRE_AURA):
		if not player.has_fire_immunity:
			aura_damage = Rng.roll_dice(1 + (monster_type.level / 26), 1 + (monster_type.level / 17))
			var hurt_by: String = monster.describe(0x88)
			Profile.instance.message_print("You are suddenly very hot!")
			if player.timed_fire_resistance != 0:
				aura_damage = (aura_damage + 2) / 3
			if player.has_fire_resistance:
				aura_damage = (aura_damage + 2) / 3
			player.take_hit(aura_damage, hurt_by)
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.FIRE_AURA)
			savegame.notice_update_redraw()
	# If we have a lightning aura, apply it
	if monster_type.flags_2.is_set(MonsterFlags2.LIGHTNING_AURA):
		if not player.has_fire_immunity:
			aura_damage = Rng.roll_dice(1 + (monster_type.level / 26), 1 + (monster_type.level / 17))
			var hurt_by: String = monster.describe(0x88)
			Profile.instance.message_print("You get zapped!")
			if player.timed_lightning_resistance != 0:
				aura_damage = (aura_damage + 2) / 3
			if player.has_lightning_resistance:
				aura_damage = (aura_damage + 2) / 3
			player.take_hit(aura_damage, hurt_by)
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.LIGHTNING_AURA)
			savegame.notice_update_redraw()


func player_natural_attack_monster(monster_index: int, mutation: Mutation) -> void:
	fear = false
	no_extras = false
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var damage_sides: int = mutation.damage_dice_size
	var damage_dice: int = mutation.damage_dice_number
	var effective_weight: int = mutation.equivalent_weapon_weight
	var attack_description: String = mutation.attack_description
	var monster_name: String = monster.describe(0)
	# See if the player hit the monster
	var bonus: int = player.attack_bonus
	var chance: int = player.skill_melee + (bonus * 3)
	if player_check_hit_on_monster(chance, monster_type.ac, monster.is_visible):
		Console.play_sound("AttackHit")
		# It was a hit, so let the player know
		Profile.instance.message_print("You hit %s with your %s." % [monster_name, attack_description])
		# Roll the damage, with possible critical damage
		var damage: int = Rng.roll_dice(damage_dice, damage_sides)
		damage = player_critical_melee(effective_weight, player.attack_bonus, damage)
		damage += player.damage_bonus
		# Can't have negative damage
		if damage < 0:
			damage = 0
		# If it's a friend, it will get angry
		if monster.mind.is_set(MonsterMind.FRIENDLY):
			Profile.instance.message_print("%s gets angry!" % [monster_name])
			monster.mind.clear_bits(MonsterMind.FRIENDLY)
		# Apply damage of the correct type to the monster
		match mutation.mutation_attack_type:
			MutationAttackType.PHYSICAL:
				no_extras = dungeon_level.dungeon_monsters.damage_monster(monster_index, damage, "")
			MutationAttackType.POISON:
				savegame.spell_effects_handler.project(0, 0, monster.map_row, monster.map_col, damage, ProjectilePoison.new(savegame.spell_effects_handler), ProjectionFlag.KILL)
			MutationAttackType.HELLFIRE:
				savegame.spell_effects_handler.project(0, 0, monster.map_row, monster.map_col, damage, ProjectileHellFire.new(savegame.spell_effects_handler), ProjectionFlag.KILL)
		# The monster might hurt when we touch it
		touch_zap_player(monster)
	else:
		# We missed, so just let us know
		Console.play_sound("AttackMiss")
		Profile.instance.message_print("You miss %s." % [monster_name])


func count_known_traps(location: Location) -> int:
	var count: int = 0
	for dir: int in 9:
		var row: int = player.map_row + dungeon_level.ordered_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.ordered_direction_col_offset[dir]
		# We need to be aware of the trap
		if dungeon_level.get_grid(row, col).tile_flags.is_clear(GridTile.PLAYER_MEMORISED):
			continue
		# It needs to actually be a trap
		if not dungeon_level.get_grid(row, col).feature_type.is_trap:
			continue
		count += 1
		# Remember its location
		location.row = row
		location.col = col
	return count


func activate_legendary(item:Item)->void:
	var player_level:int = player.level
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var direction:int
	# If we don't have a legendary item, abort
	if not item.is_legendary():
		return
	# Big stonking match based on the activation power
	match item.bonus_power_sub_type:
		LegendaryPower.SUNLIGHT:
			# Aim a line of light in a direction of the player's choice
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			Profile.instance.message_print("A line of sunlight appears.")
			savegame.spell_effects_handler.light_line(direction)
			item.recharge_time_left = 10
		LegendaryPower.BO_MISS_1:
			# Shoot a magic missile that does 2d6 damage
			Profile.instance.message_print("It glows extremely brightly...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileMissile.new(savegame.spell_effects_handler), direction, Rng.roll_dice(2, 6))
			item.recharge_time_left = 2
		LegendaryPower.BA_POIS_1:
			# Shoot a 12-damage ball of poison
			Profile.instance.message_print("It throbs deep green...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), direction, 12, 3)
			item.recharge_time_left = Rng.roll_die(4) + 3
		LegendaryPower.BO_ELEC_1:
			#Shoot a lightning bolt that does 4d8 damage
			Profile.instance.message_print("It is covered in sparks...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileElectricity.new(savegame.spell_effects_handler), direction, Rng.roll_dice(4, 8))
			item.recharge_time_left = Rng.roll_die(6) + 5
		LegendaryPower.BO_ACID_1:
			# Shoot an acid bolt that does 5d8 damage
			Profile.instance.message_print("It is covered in acid...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileAcid.new(savegame.spell_effects_handler), direction, Rng.roll_dice(5, 8))
			item.recharge_time_left = Rng.roll_die(5) + 4
		LegendaryPower.BO_COLD_1:
			# Shoot a frost bolt that does 6d8 damage
			Profile.instance.message_print("It is covered in frost...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileCold.new(savegame.spell_effects_handler), direction, Rng.roll_dice(6, 8))
			item.recharge_time_left = Rng.roll_die(7) + 6
		LegendaryPower.BO_FIRE_1:
			# Shoot a fire bolt that does 9d8 damage
			Profile.instance.message_print("It is covered in fire...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileFire.new(savegame.spell_effects_handler), direction, Rng.roll_dice(9, 8))
			item.recharge_time_left = Rng.roll_die(8) + 7
		LegendaryPower.BA_COLD_1:
			# Shoot a frost ball that does 48 damage
			Profile.instance.message_print("It is covered in frost...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), direction, 48, 2)
			item.recharge_time_left = 400
		LegendaryPower.BA_FIRE_1:
			# Shoot a fire ball that does 72 damage
			Profile.instance.message_print("It glows an intense red...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), direction, 72, 2)
			item.recharge_time_left = 400
		LegendaryPower.DRAIN_1:
			# Drain up to 100 life from an opponent
			Profile.instance.message_print("It glows black...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			if savegame.spell_effects_handler.drain_life(direction, 100):
				item.recharge_time_left = Rng.roll_die(100) + 99
		LegendaryPower.BA_COLD_2:
			# Shoot a frost ball that does 100 damage
			Profile.instance.message_print("It glows an intense blue...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), direction, 100, 2)
			item.recharge_time_left = 300
		LegendaryPower.BA_ELEC_2:
			# Shoot a lightning storm that does 100 damage with a larger radius
			Profile.instance.message_print("It crackles with electricity...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), direction, 100, 3)
			item.recharge_time_left = 500
		LegendaryPower.DRAIN_2:
			# Drain up to 120 life from an opponent
			Profile.instance.message_print("It glows black...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.drain_life(direction, 120)
			item.recharge_time_left = 400
		LegendaryPower.VAMPIRE_1:
			# Drain up to 50 life from an opponent, and give it to the player
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			for i:int in 3:
				if savegame.spell_effects_handler.drain_life(direction, 50):
					player.restore_health(50)
			item.recharge_time_left = 400
		LegendaryPower.BO_MISS_2:
			# Shoot an arrow that does 150 damage
			Profile.instance.message_print("It grows magical spikes...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_bolt(ProjectileArrow.new(savegame.spell_effects_handler), direction, 150)
			item.recharge_time_left = Rng.roll_die(90) + 89
		LegendaryPower.BA_FIRE_2:
			# Shoot a fire ball that does 120 damage with a larger radius
			Profile.instance.message_print("It glows deep red...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), direction, 120, 3)
			item.recharge_time_left = Rng.roll_die(225) + 224
		LegendaryPower.BA_COLD_3:
			# Shoot a frost ball that does 200 damage with a larger radius
			Profile.instance.message_print("It glows bright white...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), direction, 200, 3)
			item.recharge_time_left = Rng.roll_die(325) + 324
		LegendaryPower.BA_ELEC_3:
			# Shoot a lightning storm that does 250 damage with a larger radius
			Profile.instance.message_print("It glows deep blue...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), direction, 250, 3)
			item.recharge_time_left = Rng.roll_die(425) + 424
		LegendaryPower.WHIRLWIND:
			# Make a physical attack against each adjacent monster
			for dir:int in 10:
				var row:int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
				var col:int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
				var tile:GridTile = dungeon_level.get_grid(row,col)
				var monster:Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
				if tile.monster_index != 0 and (monster.is_visible or dungeon_level.grid_passable(row, col)):
					player_attack_monster(row, col)
			item.recharge_time_left = 250
		LegendaryPower.VAMPIRE_2:
			# Drain 100 health from an opponent, and give it to the player
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			for i:int in 3:
				if savegame.spell_effects_handler.drain_life(direction, 100):
					player.restore_health(100)
			item.recharge_time_left = 400
		LegendaryPower.CALL_CHAOS:
			# Activate the Call Chaos spell
			Profile.instance.message_print("It glows in scintillating colours...")
			savegame.spell_effects_handler.call_chaos()
			item.recharge_time_left = 350
		LegendaryPower.SHARD:
			# Shoot a shard ball for 120 + level damage
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_ball(ProjectileShard.new(savegame.spell_effects_handler), direction, 120 + player_level, 2)
			item.recharge_time_left = 400
		LegendaryPower.DISP_EVIL:
			# Dispel evil with a strength of x5
			Profile.instance.message_print("It floods the area with goodness...")
			savegame.spell_effects_handler.dispel_evil(player.level * 5)
			item.recharge_time_left = Rng.roll_die(300) + 299
		LegendaryPower.DISP_GOOD:
			# Dispel good with a strength of x5
			Profile.instance.message_print("It floods the area with evil...")
			savegame.spell_effects_handler.dispel_good(player.level * 5)
			item.recharge_time_left = Rng.roll_die(300) + 299
		LegendaryPower.BA_MISS_3:
			# Shoot a 'magic missile' cone that does 300 damage
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			Profile.instance.message_print("You breathe the elements.")
			savegame.spell_effects_handler.fire_ball(ProjectileMissile.new(savegame.spell_effects_handler), direction, 300, -4)
			item.recharge_time_left = 500
		LegendaryPower.CONFUSE:
			# Confuse an opponent
			Profile.instance.message_print("It glows in scintillating colours...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.confuse_monster(direction, 20)
			item.recharge_time_left = 15
		LegendaryPower.SLEEP:
			# Activate sleep on touch
			Profile.instance.message_print("It glows deep blue...")
			savegame.spell_effects_handler.sleep_monsters_touch()
			item.recharge_time_left = 55
		LegendaryPower.QUAKE:
			# Cause an earthquake around the player
			savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 10)
			item.recharge_time_left = 50
		LegendaryPower.TERROR:
			# Scare monsters with a 40+level strength
			savegame.spell_effects_handler.turn_monsters(40 + player_level)
			item.recharge_time_left = 3 * (player.level + 10)
		LegendaryPower.TELE_AWAY:
			# Teleport away an opponent
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.fire_beam(ProjectileAwayAll.new(savegame.spell_effects_handler), direction, player_level)
			item.recharge_time_left = 200
		LegendaryPower.BANISH_EVIL:
			# Banish evil creatures
			if savegame.spell_effects_handler.banish_evil(100):
				Profile.instance.message_print("The power of the item banishes evil!")
			item.recharge_time_left = 250 + Rng.roll_die(250)
		LegendaryPower.CARNAGE:
			# Carnage a chosen creature type
			Profile.instance.message_print("It glows deep blue...")
			savegame.spell_effects_handler.carnage(true)
			item.recharge_time_left = 500
		LegendaryPower.MASS_GENO:
			# Mass carnage creatures near the player
			Profile.instance.message_print("It lets out a long, shrill note...")
			savegame.spell_effects_handler.mass_carnage(true)
			item.recharge_time_left = 1000
		LegendaryPower.CHARM_ANIMAL:
			# Charm an animal
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.charm_animal(direction, player_level)
			item.recharge_time_left = 300
		LegendaryPower.CHARM_UNDEAD:
			# Charm an undead
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.control_one_undead(direction, player_level)
			item.recharge_time_left = 333
		LegendaryPower.CHARM_OTHER:
			# Charm a monster
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.charm_monster(direction, player_level)
			item.recharge_time_left = 400
		LegendaryPower.CHARM_ANIMALS:
			# Charm multiple animals
			savegame.spell_effects_handler.charm_animals(player_level * 2)
			item.recharge_time_left = 500
		LegendaryPower.CHARM_OTHERS:
			# Charm multiple monsters
			savegame.spell_effects_handler.charm_monsters(player_level * 2)
			item.recharge_time_left = 750
		LegendaryPower.SUMMON_ANIMAL:
			# Summon animals
			dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player_level, SummonType.SUMMON_ANIMAL_RANGER, true)
			item.recharge_time_left = 200 + Rng.roll_die(300)
		LegendaryPower.SUMMON_PHANTOM:
			# Summon phantom warriors or beasts
			Profile.instance.message_print("You summon a phantasmal servant.")
			dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty(),SummonType.SUMMON_PHANTOM, true)
			item.recharge_time_left = 200 + Rng.roll_die(200)
		LegendaryPower.SUMMON_ELEMENTAL:
			# Summon an elemental, with a 1-in-3 chance of it being hostile
			if Rng.roll_die(3) == 1:
				if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player_level * 1.5, SummonType.SUMMON_ELEMENTAL):
					Profile.instance.message_print("An elemental materializes...")
					Profile.instance.message_print("You fail to control it!")
			else:
				if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player_level * 1.5, SummonType.SUMMON_ELEMENTAL, player_level == 50):
					Profile.instance.message_print("An elemental materializes...")
					Profile.instance.message_print("It seems obedient to you.")
			item.recharge_time_left = 750
		LegendaryPower.SUMMON_DEMON:
			# Summon a demon, with a 1-in-3 chance of it being hostile
			if Rng.roll_die(3) == 1:
				if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player_level * 1.5, SummonType.SUMMON_DEMON):
					Profile.instance.message_print("The area fills with a stench of sulphur and brimstone.")
					Profile.instance.message_print("'NON SERVIAM! Wretch! I shall feast on thy mortal soul!'")
			else:
				if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player_level * 1.5, SummonType.SUMMON_DEMON, player_level == 50):
					Profile.instance.message_print("The area fills with a stench of sulphur and brimstone.")
					Profile.instance.message_print("'What is thy bidding... Master?'")
			item.recharge_time_left = 666 + Rng.roll_die(333)
		LegendaryPower.SUMMON_UNDEAD:
			# Summon undead, with a 1-in-3 chance of it being hostile
			if Rng.roll_die(3) == 1:
				if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player_level * 1.5, SummonType.SUMMON_HI_UNDEAD if player_level > 47 else SummonType.SUMMON_UNDEAD):
					Profile.instance.message_print(
						"Cold winds begin to Attack around you, carrying with them the stench of decay...")
					Profile.instance.message_print("'The dead arise... to punish you for disturbing them!'")
			else:
				if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, (int)(player_level * 1.5), SummonType.SUMMON_HI_UNDEAD_NO_UNIQUES if player_level > 47 else SummonType.SUMMON_UNDEAD, player_level > 24 and Rng.roll_die(3) == 1):
					Profile.instance.message_print(
						"Cold winds begin to Attack around you, carrying with them the stench of decay...")
					Profile.instance.message_print("Ancient, long-dead forms arise from the ground to serve you!")
			item.recharge_time_left = 666 + Rng.roll_die(333)
		LegendaryPower.CURE_L_W:
			# Heal 30 health and remove fear
			player.set_timed_fear(0)
			player.restore_health(30)
			item.recharge_time_left = 10
		LegendaryPower.CURE_M_W:
			# Heal 4d8 health and reduce bleeding
			Profile.instance.message_print("It radiates deep purple...")
			player.restore_health(Rng.roll_dice(4, 8))
			player.set_timed_bleeding((player.timed_bleeding / 2) - 50)
			item.recharge_time_left = Rng.roll_die(3) + 2
		LegendaryPower.CURE_POISON:
			# Remove fear and poison
			Profile.instance.message_print("It glows deep blue...")
			player.set_timed_fear(0)
			player.set_timed_poison(0)
			item.recharge_time_left = 5
		LegendaryPower.REST_LIFE:
			# Restore lost experience
			Profile.instance.message_print("It glows a deep red...")
			player.restore_level()
			item.recharge_time_left = 450
		LegendaryPower.REST_ALL:
			# Restore all ability drain and lost experience
			Profile.instance.message_print("It glows a deep green...")
			player.try_restoring_ability_score(Ability.STRENGTH)
			player.try_restoring_ability_score(Ability.INTELLIGENCE)
			player.try_restoring_ability_score(Ability.WISDOM)
			player.try_restoring_ability_score(Ability.DEXTERITY)
			player.try_restoring_ability_score(Ability.CONSTITUTION)
			player.try_restoring_ability_score(Ability.CHARISMA)
			player.restore_level()
			item.recharge_time_left = 750
		LegendaryPower.CURE_700:
			# Heal 700 health and remove all bleding
			Profile.instance.message_print("It glows deep blue...")
			Profile.instance.message_print("You feel a warm tingling inside...")
			player.restore_health(700)
			player.set_timed_bleeding(0)
			item.recharge_time_left = 250
		LegendaryPower.CURE_1000:
			# Heal 1000 health and remove all bleeding
			Profile.instance.message_print("It glows a bright white...")
			Profile.instance.message_print("You feel much better...")
			player.restore_health(1000)
			player.set_timed_bleeding(0)
			item.recharge_time_left = 888
		LegendaryPower.ESP:
			# Give temporary telepathy
			player.set_timed_telepathy(player.timed_telepathy + Rng.roll_die(30) + 25)
			item.recharge_time_left = 200
		LegendaryPower.BERSERK:
			# Bless us and make us a superhero
			player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(50) + 50)
			player.set_timed_blessing(player.timed_blessing + Rng.roll_die(50) + 50)
			item.recharge_time_left = 100 + Rng.roll_die(100)
		LegendaryPower.PROT_EVIL:
			# Give us protection from evil
			Profile.instance.message_print("It lets out a shrill wail...")
			var k: int = 3 * player.level
			player.set_timed_protection_from_evil(player.timed_protection_from_evil + Rng.roll_die(25) + k)
			item.recharge_time_left = Rng.roll_die(225) + 224
		LegendaryPower.RESIST_ALL:
			# Give us temporary resistance to the basic elements
			Profile.instance.message_print("It glows many colours...")
			player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(40) + 40)
			player.set_timed_lightning_resistance(player.timed_lightning_resistance + Rng.roll_die(40) + 40)
			player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(40) + 40)
			player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(40) + 40)
			player.set_timed_poison_resistance(player.timed_poison_resistance + Rng.roll_die(40) + 40)
			item.recharge_time_left = 200
		LegendaryPower.SPEED:
			# Give us temporary haste
			Profile.instance.message_print("It glows bright green...")
			if player.timed_haste == 0:
				player.set_timed_haste(Rng.roll_die(20) + 20)
			else:
				player.set_timed_haste(player.timed_haste + 5)
			item.recharge_time_left = 250
		LegendaryPower.XTRA_SPEED:
			# Give us extra haste for a long time
			Profile.instance.message_print("It glows brightly...")
			if player.timed_haste == 0:
				player.set_timed_haste(Rng.roll_die(75) + 75)
			else:
				player.set_timed_haste(player.timed_haste + 5)
			item.recharge_time_left = Rng.roll_die(200) + 199
		LegendaryPower.WRAITH:
			# Give us temporary etherealness
			player.set_timed_etherealness(player.timed_etherealness + Rng.roll_die(player_level / 2) + (player_level / 2))
			item.recharge_time_left = 1000
		LegendaryPower.INVULN:
			# Give us temporary invulnerabliity
			player.set_timed_invulnerability(player.timed_invulnerability + Rng.roll_die(8) + 8)
			item.recharge_time_left = 1000
		LegendaryPower.LIGHT:
			# Light the area
			Profile.instance.message_print("It wells with clear light...")
			savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 15), 3)
			item.recharge_time_left = Rng.roll_die(10) + 9
		LegendaryPower.MAP_LIGHT:
			# Map the local area
			Profile.instance.message_print("It shines brightly...")
			dungeon_level.map_area()
			savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 15), 3)
			item.recharge_time_left = Rng.roll_die(50) + 49
		LegendaryPower.DETECT_ALL:
			# Detect everything
			Profile.instance.message_print("It glows bright white...")
			Profile.instance.message_print("An image forms in your mind...")
			savegame.spell_effects_handler.detect_all()
			item.recharge_time_left = Rng.roll_die(55) + 54
		LegendaryPower.DETECT_XTRA:
			# Detect everything and do probing and identify an item fully
			Profile.instance.message_print("It glows brightly...")
			savegame.spell_effects_handler.detect_all()
			savegame.spell_effects_handler.probing()
			savegame.spell_effects_handler.identify_fully()
			item.recharge_time_left = 1000
		LegendaryPower.ID_FULL:
			# Identify an item fully
			Profile.instance.message_print("It glows yellow...")
			savegame.spell_effects_handler.identify_fully()
			item.recharge_time_left = 750
		LegendaryPower.ID_PLAIN:
			# Identify an item
			if savegame.spell_effects_handler.identify_item():
				return
			item.recharge_time_left = 10
		LegendaryPower.RUNE_EXPLO:
			# Place a Yellow Sign
			Profile.instance.message_print("It glows a sickly yellow...")
			savegame.spell_effects_handler.yellow_sign()
			item.recharge_time_left = 200
		LegendaryPower.RUNE_PROT:
			# Place an elder_sign
			Profile.instance.message_print("It glows light blue...")
			savegame.spell_effects_handler.elder_sign()
			item.recharge_time_left = 400
		LegendaryPower.SATIATE:
			# Fill us up
			player.set_food(Constants.FOOD_GORGED - 1)
			item.recharge_time_left = 200
		LegendaryPower.DEST_DOOR:
			# Destroy nearby doors
			Profile.instance.message_print("It glows bright red...")
			savegame.spell_effects_handler.destroy_doors_touch()
			item.recharge_time_left = 10
		LegendaryPower.STONE_MUD:
			# Rock to mud
			Profile.instance.message_print("It pulsates...")
			direction = target_engine.get_direction_with_aim()
			if direction == -1:
				return
			savegame.spell_effects_handler.wall_to_mud(direction)
			item.recharge_time_left = 5
		LegendaryPower.RECHARGE:
			# Recharge an item
			savegame.spell_effects_handler.recharge(60)
			item.recharge_time_left = 70
		LegendaryPower.ALCHEMY:
			# Turn an item to gold
			Profile.instance.message_print("It glows bright yellow...")
			savegame.spell_effects_handler.alchemy()
			item.recharge_time_left = 500
		LegendaryPower.DIM_DOOR:
			# Short range teleport to a specific destination
			Profile.instance.message_print("You open a dimensional gate. Choose a destination.")
			var location:Location = Location.new()
			if not target_engine.target_point(location):
				return
			player.energy -= 60 - player_level
			if (!dungeon_level.grid_passable_no_creature(location.row, location.col) or dungeon_level.get_grid(location.row,location.col).tile_flags.is_set(GridTile.IN_VAULT) or dungeon_level.Distance(location.row, location.col, player.map_row, player.map_col) > player_level + 2 or Rng.roll_die(player_level * player_level / 2) == 1):
				Profile.instance.message_print("You fail to exit the astral plane correctly!")
				player.energy -= 100
				savegame.spell_effects_handler.teleport_player(10)
			else:
				savegame.spell_effects_handler.teleport_player_to(location.row, location.col)
			item.recharge_time_left = 100
		LegendaryPower.TELEPORT:
			# Long range teleport
			Profile.instance.message_print("It twists space around you...")
			savegame.spell_effects_handler.teleport_player(100)
			item.recharge_time_left = 45
		LegendaryPower.RECALL:
			# Word of Recall
			Profile.instance.message_print("It glows soft white...")
			player.toggle_recall()
			item.recharge_time_left = 200

func bash_closed_door(row:int, col:int, dir:int) -> bool:
	var more:bool = false
	savegame.energy_use = 100
	var tile:GridTile = dungeon_level.get_grid(row,col)
	Profile.instance.message_print("You smash into the door!")
	var bash:int = player.ability_scores[Ability.STRENGTH].str_attack_speed_component()
	var temp:int = int(tile.feature_type.name.substr(10))
	temp = bash - (temp * 10)
	if temp < 1:
		temp = 1
	if Rng.roll_die(100) <= temp:
		Profile.instance.message_print("The door crashes open!")
		dungeon_level.cave_set_feat(row, col, "BrokenDoor" if Rng.roll_die(100) <= 50 else  "OpenDoor")
		move_player(dir, false)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
	elif Rng.roll_die(100) <= player.ability_scores[Ability.DEXTERITY].dex_theft_avoidance() + player.level:
		Profile.instance.message_print("The door holds firm.")
		more = true
	else:
		Profile.instance.message_print("You are off-balance.")
		player.set_timed_paralysis(player.timed_paralysis + 1 + Rng.roll_die(2))
	return more


func brand_bolts() ->void:
	for i:int in InventorySlot.PACK:
		# Find a set of non-artifact bolts in our inventory
		var item: Item = player.inventory.items[i]
		if item.category != ItemCategory.BOLT:
			continue
		if item.is_legendary() or item.is_artifact() or item.is_rare():
			continue
		# Skip cursed or broken bolts
		if item.is_cursed() or item.is_broken():
			continue
		# Only a 25% chance of success per set of bolts
		if Rng.roll_die(100) <= 75:
			continue
		# Make the bolts into bolts of flame
		Profile.instance.message_print("Your bolts are covered in a fiery aura!")
		item.rare_index = RareItemId.AMMO_OF_FLAME
		savegame.spell_effects_handler.enchant(item, Rng.roll_die(3) + 3, EnchantFlags.TO_HIT | EnchantFlags.TO_DAM)
		# Quit after the first bolts have been upgraded
		return
	# We fell off the end of the inventory without enchanting anything
	Profile.instance.message_print("The fiery enchantment failed.")


func brand_weapon(brand_type:int) -> void:
	var item:Item = player.inventory.items[InventorySlot.MELEE_WEAPON]
	# We must have a non-rare, non-artifact weapon that isn't cursed
	if item.item_type != null and not item.is_artifact() and not item.is_rare() and not item.is_legendary() and not item.is_cursed():
		var act:String
		var item_name:String = item.description(false, 0)
		match brand_type:
			4:
				# Make it a planar weapon
				act = "seems very unstable now."
				item.rare_index = RareItemId.WEAPON_PLANAR_WEAPON
				item.type_specific_value = Rng.roll_die(2)
			3:
				# Make it a vampiric weapon
				act = "thirsts for blood!"
				item.rare_index = RareItemId.WEAPON_VAMPIRIC
			2:
				# Make it a poison brand
				act = "is coated with poison."
				item.rare_index = RareItemId.WEAPON_OF_POISONING
			1:
				# Make it a chaotic weapon
				act = "is engulfed in raw chaos!"
				item.rare_index = RareItemId.WEAPON_CHAOTIC
			_:
				# Make it a fire or ice weapon
				if Rng.roll_die(100) <= 25:
					act = "is covered in a fiery shield!"
					item.rare_index = RareItemId.WEAPON_OF_BURNING
				else:
					act = "glows deep, icy blue!"
					item.rare_index = RareItemId.WEAPON_OF_FREEZING
		# Let the player know what happened
		Profile.instance.message_print("Your %s %s" %  [item_name,act])
		savegame.spell_effects_handler.enchant(item, Rng.roll_die(3) + 3, EnchantFlags.TO_HIT | EnchantFlags.TO_DAM)
	else:
		Profile.instance.message_print("The Branding failed.")


func call_the_void() -> void:
	# Make sure we're not next to a wall
	if (dungeon_level.grid_passable(player.map_row - 1, player.map_col - 1) and dungeon_level.grid_passable(player.map_row - 1, player.map_col) and
		dungeon_level.grid_passable(player.map_row - 1, player.map_col + 1) and dungeon_level.grid_passable(player.map_row, player.map_col - 1) and
		dungeon_level.grid_passable(player.map_row, player.map_col + 1) and dungeon_level.grid_passable(player.map_row + 1, player.map_col - 1) and
		dungeon_level.grid_passable(player.map_row + 1, player.map_col) and dungeon_level.grid_passable(player.map_row + 1, player.map_col + 1)):
		# Fire area effect shards, power, and nukes in all directions
		for i:int in range(1,10):
			if i - 5 != 0:
				savegame.spell_effects_handler.fire_ball(ProjectileShard.new(savegame.spell_effects_handler), i, 175, 2)
		for i:int in range(1,10):
			if i - 5 != 0:
				savegame.spell_effects_handler.fire_ball(ProjectilePower.new(savegame.spell_effects_handler), i, 175, 3)
		for i:int in range(1,10):
			if i - 5 != 0:
				savegame.spell_effects_handler.fire_ball(ProjectileNuke.new(savegame.spell_effects_handler), i, 175, 4)
	else:
		# We were too close to a wall, so earthquake instead
		var cast:String = "recite" if player.spellcasting.Type == CastingType.DIVINE else "cast"
		var spell:String = "prayer" if player.spellcasting.Type == CastingType.DIVINE else "spell"
		Profile.instance.message_print("You %s the %s too close to a wall!" % [cast, spell])
		Profile.instance.message_print("There is a loud explosion!")
		savegame.spell_effects_handler.destroy_area(player.map_row, player.map_col, 20 + player.level)
		Profile.instance.message_print("The dungeon collapses...")
		player.take_hit(100 + Rng.roll_die(150), "a suicidal Call the Void")


func check_if_racial_power_works(min_level:int,  cost:int,  useStat:int,  difficulty:int) -> bool:
	# If we don't have enough power we'll use health instead
	var use_health:bool = player.power < cost
	# Can't use it if we're too low level
	if player.level < min_level:
		Profile.instance.message_print("You need to attain level %d to use this power." % [min_level])
		savegame.energy_use = 0
		return false
	# Can't use it if we're confused
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused to use this power.")
		savegame.energy_use = 0
		return false
	# If we're about to kill ourselves, give us chance to back out
	if use_health and player.health < cost:
		if not User.get_check("Really use the power in your weakened state? "):
			savegame.energy_use = 0
			return false
	# Harder to use powers when you're stunned
	if player.timed_stun != 0:
		difficulty += player.timed_stun
	# Easier to use powers if you're higher level than you need to be
	elif player.level > min_level:
		var level_adjustment:int = (player.level - min_level) / 3
		if level_adjustment > 10:
			level_adjustment = 10
		difficulty -= level_adjustment
	# We have a minimum difficulty
	if difficulty < 5:
		difficulty = 5
	# Using a power takes a turn
	savegame.energy_use = 100
	# Reduce our health or power
	if use_health:
		player.take_hit((cost / 2) + Rng.roll_die(cost / 2), "concentrating too hard")
	else:
		player.power -= (cost / 2) + Rng.roll_die(cost / 2)
	# We'll need to redraw
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
	# Check to see if we were successful
	if Rng.roll_die(player.ability_scores[useStat].Innate) >= (difficulty / 2) + Rng.roll_die(difficulty / 2):
		return true
	# Let us know we failed
	Profile.instance.message_print("You've failed to concentrate hard enough.")
	return false


func create_phlogiston() -> void:
	var max_phlogiston:int
	var item: Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	# Maximum phlogiston is the capacity of the light source
	if item.category == ItemCategory.LIGHT and item.sub_category == LightType.LANTERN:
		max_phlogiston = Constants.FUEL_LANTERN
	elif item.category == ItemCategory.LIGHT and item.sub_category == LightType.TORCH:
		max_phlogiston = Constants.FUEL_TORCH
	# Probably using an orb or a star essence (or maybe not holding a light source at all)
	else:
		Profile.instance.message_print("You are not wielding anything that uses phlogiston.")
		return
	# Item is already full
	if item.type_specific_value >= max_phlogiston:
		Profile.instance.message_print("No more phlogiston can be put in this item.")
		return
	# Add half the max fuel of the item to its current fuel
	item.type_specific_value += max_phlogiston / 2
	Profile.instance.message_print("You add phlogiston to your light item.")
	# Make sure it doesn't overflow
	if item.type_specific_value >= max_phlogiston:
		item.type_specific_value = max_phlogiston
		Profile.instance.message_print("Your light item is full.")
	# We need to update our light after this
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)


func curse_armour() -> bool:
	var item:Item = player.inventory.items[InventorySlot.BODY]
	# If we're not wearing armour then nothing can happen
	if item.item_type == null:
		return false
	# Artifacts can't be cursed, and normal armour has a chance to save
	var item_name: String = item.description(false, 3)
	if (item.is_legendary() or item.is_artifact()) and Rng.roll_die(100) <= 50:
		Profile.instance.message_print("A terrible black aura tries to surround your armour, but your %s resists the effects!" % [item_name])
	else:
		# Completely remake the armour into a set of blasted armour
		Profile.instance.message_print("A terrible black aura blasts your %s!" % [item_name])
		item.artifact_index = 0
		item.rare_index = RareItemId.ARMOUR_BLASTED
		item.bonus_armour_class = 0 - Rng.roll_die(5) - Rng.roll_die(5)
		item.bonus_to_hit = 0
		item.bonus_damage = 0
		item.base_armour_class = 0
		item.damage_dice = 0
		item.damage_dice_sides = 0
		item.legendary_flags_1.clear_all()
		item.legendary_flags_2.clear_all()
		item.legendary_flags_3.clear_all()
		item.identify_flags.set_bits(IdentifyFlags.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.BROKEN)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER)
	return true


func curse_weapon() -> bool:
	var item:Item = player.inventory.items[InventorySlot.MELEE_WEAPON]
	# If we don't have a weapon then nothing happens
	if item.item_type == null:
		return false
	var item_name: String = item.description(false, 3)
	# Artifacts can't be cursed, and other items have a chance to resist
	if (item.is_artifact() or item.is_legendary()) and Rng.roll_die(100) <= 50:
		Profile.instance.message_print("A terrible black aura tries to surround your weapon, but your %s resists the effects!" % [item_name])
	else:
		# Completely remake the item into a shattered weapon
		Profile.instance.message_print("A terrible black aura blasts your %s!" % [item_name])
		item.artifact_index = 0
		item.rare_index = RareItemId.WEAPON_SHATTERED
		item.bonus_to_hit = 0 - Rng.roll_die(5) - Rng.roll_die(5)
		item.bonus_damage = 0 - Rng.roll_die(5) - Rng.roll_die(5)
		item.bonus_armour_class = 0
		item.base_armour_class = 0
		item.damage_dice = 0
		item.damage_dice_sides = 0
		item.legendary_flags_1.clear_all()
		item.legendary_flags_2.clear_all()
		item.legendary_flags_3.clear_all()
		item.identify_flags.set_bits(IdentifyFlags.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.BROKEN)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER)
	return true


func disarm_chest( row:int,  col:int,  item_index:int) -> bool:
	var more:bool = false
	var item:Item = dungeon_level.dungeon_items.items[item_index]
	# Disarming a chest takes a turn
	savegame.energy_use = 100
	var i:int = player.skill_disarm_traps
	# Disarming is tricky when you can't see
	if player.timed_blindness != 0 or dungeon_level.no_light():
		i /= 10
	# Disarming is tricky when confused
	if player.timed_confusion != 0 or player.timed_hallucinations != 0:
		i /= 10
	# Penalty for difficulty of trap
	var j:int = i - item.type_specific_value
	if j < 2:
		j = 2
	# If we don't know about the traps, we don't know what to disarm
	if not item.is_known():
		Profile.instance.message_print("I don't see any traps.")
	# If it has no traps there's nothing to disarm
	elif item.type_specific_value <= 0:
		Profile.instance.message_print("The chest is not trapped.")
	# If it has a null trap then there's nothing to disarm
	elif Tables.CHEST_TRAPS[item.type_specific_value] == 0:
		Profile.instance.message_print("The chest is not trapped.")
	# If we made the skill roll then we disarmed it
	elif Rng.roll_die(100) <= j:
		Profile.instance.message_print("You have disarmed the chest.")
		player.gain_experience(item.type_specific_value)
		item.type_specific_value = 0 - item.type_specific_value
	# If we failed to disarm it there's a chance it goes off
	elif i > 5 and Rng.roll_die(i) > 5:
		more = true
		Profile.instance.message_print("You failed to disarm the chest.")
	else:
		Profile.instance.message_print("You set off a trap!")
		savegame.chest_trap(row, col, item_index)
	return more


func command_channel(item:Item) ->bool:
	var cost:int
	var price:int = item.item_type.cost
	# Never channel worthless items
	if price <= 0:
		return false
	# Cost to channel is based on how much the item is worth and what type
	match item.category:
		ItemCategory.WAND:
			cost = price / 150
		ItemCategory.SCROLL:
			cost = price / 10
		ItemCategory.POTION:
			cost = price / 20
		ItemCategory.ROD:
			cost = price / 250
		ItemCategory.STAFF:
			cost = price / 100
		_:
			Profile.instance.message_print("Tried to channel an unknown object type!")
	# Always cost at least 1 power
	if cost < 1:
		cost = 1
	# Spend the power if we can
	if cost <= player.power:
		Profile.instance.message_print("You channel your own power to power the effect.")
		player.power -= cost
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
		return true
	# Use some power in the attempt, even if we failed
	Profile.instance.message_print("You power is insufficient to power the effect.")
	player.power -= Rng.roll_die(player.power / 2)-1
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
	return false


func get_rumour() -> void:
	var rumour:String
	# Build an array of all the possible rumours we can get
	var rumour_type:Array[String]
	var rumour_index:Array[int]
	var max_rumour:int = 0
	# Add a rumour for each undiscovered quest
	for i:int in savegame.quests.quests.size():
		if savegame.quests.quests[i].depth > 0 and not savegame.quests.quests[i].discovered:
			rumour_type.append("q")
			rumour_index.append(i)
			max_rumour += 1
	for i:int in savegame.dungeons.size():
		# Add a rumour for each dungeon we don't know the depth of
		if not savegame.dungeons[i].known_depth:
			rumour_type.append("d")
			rumour_index.append(i)
			max_rumour += 1
		#Add a rumour for each dungeon we don't know the offset of
		if not savegame.dungeons[i].known_offset:
			rumour_type.append("o")
			rumour_index.append(i)
			max_rumour += 1
	# If we already know everything, we're going to need to be told something so add all
	# the dungeon depth rumours and we'll get given a repeat of one of those
	if max_rumour == 0:
		for i:int in savegame.dungeons.size():
			# Add a rumour for each dungeon we don't know the depth of
			if not savegame.dungeons[i].known_depth:
				rumour_type.append("d")
				rumour_index.append(i)
				max_rumour += 1
	# Pick a random rumour from the list
	var choice: int = Rng.roll_die(max_rumour)-1
	var type: String = rumour_type[choice]
	var index: int = rumour_index[choice]
	# Give us the appropriate information based on the rumour's type
	if type == "q":
		# The rumour describes a quest
		savegame.quests.quests[index].discovered = true
		rumour = savegame.quests.describe_quest(index)
	elif type == "d":
		# The rumour describes a dungeon depth
		var d:Dungeon = savegame.dungeons[index]
		rumour = ("They say that %s has %d floors." if d.is_tower else "They say that %s has %d levels.") % [d.name, d.max_level]
		d.known_depth = true
	else:
		# The rumour describes a dungeon difficulty
		var d: Dungeon = savegame.dungeons[index]
		rumour = "They say that %s has a relative difficulty of %d." % [d.name, d.offset]
		d.known_offset = true
	Profile.instance.message_print(rumour)


func item_filter_activatable(item:Item) -> bool:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if not item.is_known():
		return false
	item.get_merged_flags(f_1, f_2, f_3)
	return f_3.is_set(ItemFlags3.ACTIVATE)


func item_filter_high_level_book(item:Item) -> bool:
	if (item.category == ItemCategory.LIFE_BOOK or item.category == ItemCategory.SORCERY_BOOK or
		item.category == ItemCategory.NATURE_BOOK or item.category == ItemCategory.CHAOS_BOOK or
		item.category == ItemCategory.DEATH_BOOK or item.category == ItemCategory.TAROT_BOOK):
		return item.sub_category > 1
	return false


func item_filter_lantern_fuel(item:Item) -> bool:
	if item.category == ItemCategory.FLASK:
		return true
	if item.category == ItemCategory.LIGHT and item.sub_category == LightType.LANTERN:
		return true
	return false


func item_filter_torch_fuel(item:Item) -> bool:
	return item.category == ItemCategory.LIGHT and item.sub_category == LightType.TORCH


func item_filter_wearable(item:Item) -> bool:
	return player.inventory.wield_slot(item) >= InventorySlot.MELEE_WEAPON


func open_chest(row:int,  col:int,  item_index:int) -> bool:
	var opened_successfully:bool = true
	var more:bool = false
	var item:Item = dungeon_level.dungeon_items.items[item_index]
	# Opening a chest takes an action
	savegame.energy_use = 100
	# If the chest is locked, we may need to pick it
	if item.type_specific_value > 0:
		opened_successfully = false
		# Our disable traps skill also doubles up as a lockpicking skill
		var i: int = player.skill_disarm_traps
		# Hard to pick locks in the dark
		if player.timed_blindness != 0 or dungeon_level.no_light():
			i /= 10
		# Hard to pick locks when you're confused or hallucinating
		if player.timed_confusion != 0 or player.timed_hallucinations != 0:
			i /= 10
		# Some locks are harder to pick than others
		var j:int = i - item.type_specific_value
		if j < 2:
			j = 2
		# See if we succeeded
		if Rng.roll_die(100) <= j:
			Profile.instance.message_print("You have picked the lock.")
			player.gain_experience(1)
			opened_successfully = true
		else:
			more = true
			Profile.instance.message_print("You failed to pick the lock.")
	# If we successfully opened it, set of any traps and then actually open the chest
	if opened_successfully:
		savegame.chest_trap(row, col, item_index)
		savegame.open_chest(row, col, item_index)
	return more


func player_check_ranged_hit_on_monster( attack_bonus:int,  armour_class:int,  monster_is_visible:bool) -> bool:
	var k:int = Rng.roll_die(100)
	# Always a 5% chance to hit and a 5% chance to miss
	if k <= 10:
		return k <= 5
	# If we have no chance of hitting don't bother checking
	if attack_bonus <= 0:
		return false
	# Invisible monsters are hard to hit
	if not monster_is_visible:
		attack_bonus = (attack_bonus + 1) / 2
	# Return the hit or miss
	return Rng.roll_die(attack_bonus) > armour_class * 3 / 4


func player_critical_range( weight:int,  plus:int,  damage:int) -> int:
	# Chance of a critical is based on weight, level, and plusses
	var i:int = weight + ((player.attack_bonus + plus) * 4) + (player.level * 2)
	if Rng.roll_die(5000) <= i:
		var k: int = weight + Rng.roll_die(500)
		if k < 500:
			Profile.instance.message_print("It was a good hit!")
			damage = (2 * damage) + 5
		elif k < 1000:
			Profile.instance.message_print("It was a great hit!")
			damage = (2 * damage) + 10
		else:
			Profile.instance.message_print("It was a superb hit!")
			damage = (3 * damage) + 15
	return damage


func potion_effect(sub_category:int) -> bool:
	var identified:bool = false
	match sub_category:
		# Water or apple juice has no effect
		PotionType.WATER, PotionType.APPLE_JUICE:
				Profile.instance.message_print("You feel less thirsty.")
				identified = true
		# Slime mold juice has a random effect (calling this function again recusively)
		PotionType.SLIME_MOLD:
				Profile.instance.message_print("That tastes awful.")
				potion_effect(random_potion())
				identified = true
		# Slowness slows you down
		PotionType.SLOWNESS:
				if player.set_timed_slow(player.timed_slow + Rng.roll_die(25) + 15):
					identified = true
		# Salt water makes you vomit, but gets rid of poison
		PotionType.SALT_WATER:
				Profile.instance.message_print("The saltiness makes you vomit!")
				player.set_food(Constants.FOOD_STARVE - 1)
				player.set_timed_poison(0)
				player.set_timed_paralysis(player.timed_paralysis + 4)
				identified = true
		# Poison simply poisons you
		PotionType.POISON:
				if not (player.has_poison_resistance or player.timed_poison_resistance != 0):
					# Hagarg Ryonis can protect you against poison
					if Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					elif player.set_timed_poison(player.timed_poison + Rng.roll_die(15) + 9):
						identified = true
		# Blindness makes you blind
		PotionType.BLINDNESS:
				if not player.has_blindness_resistance:
					if player.set_timed_blindness(player.timed_blindness + Rng.roll_die(100) + 99):
						identified = true
		# Confusion makes you confused and possibly other effects
		PotionType.CONFUSION:
				if not (player.has_confusion_resistance or player.has_chaos_resistance):
					if player.set_timed_confusion(player.timed_confusion + Rng.roll_die(20) + 14):
						identified = true
					# 50% chance of having hallucinations
					if Rng.roll_die(2) == 1:
						if player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(150) + 149):
							identified = true
					# 1 in 13 chance of blacking out and waking up somewhere else:
					if Rng.roll_die(13) == 1:
						identified = true
						# 1 in 3 chance of losing your memories after blacking out
						if Rng.roll_die(3) == 1:
							savegame.spell_effects_handler.lose_all_info()
						else:
							dungeon_level.wiz_dark()
						savegame.spell_effects_handler.teleport_player(100)
						dungeon_level.wiz_dark()
						Profile.instance.message_print("You wake up somewhere with a sore head...")
						Profile.instance.message_print("You can't remember a thing, or how you got here!")
		# Sleep paralyses you
		PotionType.SLEEP:
				if not player.has_free_action:
					if player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(4) + 3):
						identified = true
		# Lose Memories reduces your experience
		PotionType.LOSE_MEMORIES:
				if not player.has_hold_life and player.experience_points > 0:
					Profile.instance.message_print("You feel your memories fade.")
					player.lose_experience(player.experience_points / 4)
					identified = true
		# Ruination does 10d10 damage and reduces all your ability scores, bypassing
		# sustains and divine protection
		PotionType.RUINATION:
				Profile.instance.message_print("Your nerves and muscles feel weak and lifeless!")
				player.take_hit(Rng.roll_dice(10, 10), "a potion of Ruination")
				player.decrease_ability_score(Ability.DEXTERITY, 25, true)
				player.decrease_ability_score(Ability.WISDOM, 25, true)
				player.decrease_ability_score(Ability.CONSTITUTION, 25, true)
				player.decrease_ability_score(Ability.STRENGTH, 25, true)
				player.decrease_ability_score(Ability.CHARISMA, 25, true)
				player.decrease_ability_score(Ability.INTELLIGENCE, 25, true)
				identified = true
		# Weakness tries to reduce your strength
		PotionType.DEC_STR:
				if player.try_decreasing_ability_score(Ability.STRENGTH):
					identified = true
		# Stupidity tries to reduce your intelligence
		PotionType.DEC_INT:
				if player.try_decreasing_ability_score(Ability.INTELLIGENCE):
					identified = true
		# Naivety tries to reduce your wisdom
		PotionType.DEC_WIS:
				if player.try_decreasing_ability_score(Ability.WISDOM):
					identified = true
		# Clumsiness tries to reduce your dexterity
		PotionType.DEC_DEX:
				if player.try_decreasing_ability_score(Ability.DEXTERITY):
					identified = true
		# Sickliness tries to reduce your constitution
		PotionType.DEC_CON:
				if player.try_decreasing_ability_score(Ability.CONSTITUTION):
					identified = true
		# Ugliness tries to reduce your charisma
		PotionType.DEC_CHA:
				if player.try_decreasing_ability_score(Ability.CHARISMA):
					identified = true
		# Detonations does 50d20 damage, stuns you, and gives you a stupid amount of bleeding
		PotionType.DETONATIONS:
				Profile.instance.message_print("Massive explosions rupture your body!")
				player.take_hit(Rng.roll_dice(50, 20), "a potion of Detonation")
				player.set_timed_stun(player.timed_stun + 75)
				player.set_timed_bleeding(player.timed_bleeding + 5000)
				identified = true
		# Iocaine simply does 5000 damage
		PotionType.DEATH:
				Profile.instance.message_print("A feeling of death flows through your body.")
				player.take_hit(5000, "a potion of Iocaine")
				identified = true
		# Infravision gives you timed infravision
		PotionType.INFRAVISION:
				if player.set_timed_infravision(player.timed_infravision + 100 + Rng.roll_die(100)):
					identified = true
		# Detect invisible gives you times see invisibility
		PotionType.DETECT_INVIS:
				if player.set_timed_see_invisibility(player.timed_see_invisibility + 12 + Rng.roll_die(12)):
					identified = true
		# Slow poison halves the remaining duration of any poison you have
		PotionType.SLOW_POISON:
				if player.set_timed_poison(player.timed_poison / 2):
					identified = true
		# Cure poison removes any poison you have
		PotionType.CURE_POISON:
				if player.set_timed_poison(0):
					identified = true
		# Boldness stops you being afraid
		PotionType.BOLDNESS:
				if player.set_timed_fear(0):
					identified = true
		# Speed temporarily hastes you
		PotionType.SPEED:
				if player.timed_haste == 0:
					if player.set_timed_haste(Rng.roll_die(25) + 15):
						identified = true
				else:
					player.set_timed_haste(player.timed_haste + 5)
		# Resist heat gives you timed fire resistance
		PotionType.RESIST_HEAT:
				if player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(10) + 10):
					identified = true
		# Resist cold gives you timed frost resistance
		PotionType.RESIST_COLD:
				if player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(10) + 10):
					identified = true
		# Heroism removes fear, cures 10 health, and gives you timed heroism
		PotionType.HEROISM:
				if player.set_timed_fear(0):
					identified = true
				if player.set_timed_heroism(player.timed_heroism + Rng.roll_die(25) + 25):
					identified = true
				if player.restore_health(10):
					identified = true
		# Berserk strength removes fear, heals 30 health, and gives you timed super heroism
		PotionType.BESERK_STRENGTH:
				if player.set_timed_fear(0):
					identified = true
				if player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(25) + 25):
					identified = true
				if player.restore_health(30):
					identified = true
		# Cure light wounds heals you 2d8 health and reduces bleeding
		PotionType.CURE_LIGHT:
				if player.restore_health(Rng.roll_dice(2, 8)):
					identified = true
				if player.set_timed_bleeding(player.timed_bleeding - 10):
					identified = true
		# Cure serious wounds heals you 4d8 health, cures blindness and confusion, and
		# reduces bleeding
		PotionType.CURE_SERIOUS:
				if player.restore_health(Rng.roll_dice(4, 8)):
					identified = true
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_bleeding((player.timed_bleeding / 2) - 50):
					identified = true
		# Cure critical wounds heals you 6d8 health, and cures blindness, confusion, stun,
		# poison, and bleeding
		PotionType.CURE_CRITICAL:
				if player.restore_health(Rng.roll_dice(6, 8)):
					identified = true
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
		# Healing heals you 300 health, and cures blindness, confusion, stun, poison, and bleeding
		PotionType.HEALING:
				if player.restore_health(300):
					identified = true
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
		# *Healing* heals you 1200 health, and cures blindness, confusion, stun, poison,
		# and bleeding
		PotionType.STAR_HEALING:
				if player.restore_health(1200):
					identified = true
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
		# Life heals you 5000 health, removes experience and ability score drains, and
		# cures blindness, confusion, stun, poison, and bleeding
		PotionType.LIFE:
				Profile.instance.message_print("You feel life flow through your body!")
				player.restore_level()
				player.restore_health(5000)
				player.set_timed_poison(0)
				player.set_timed_blindness(0)
				player.set_timed_confusion(0)
				player.set_timed_hallucinations(0)
				player.set_timed_stun(0)
				player.set_timed_bleeding(0)
				player.try_restoring_ability_score(Ability.STRENGTH)
				player.try_restoring_ability_score(Ability.CONSTITUTION)
				player.try_restoring_ability_score(Ability.DEXTERITY)
				player.try_restoring_ability_score(Ability.WISDOM)
				player.try_restoring_ability_score(Ability.INTELLIGENCE)
				player.try_restoring_ability_score(Ability.CHARISMA)
				identified = true
		# Restore power restores your to maximum power
		PotionType.RESTORE_POWER:
				if player.power < player.max_power:
					player.power = player.max_power
					player.fractional_power = 0
					Profile.instance.message_print("Your feel your head clear.")
					player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
					identified = true
		# Restore life levels restores any lost experience
		PotionType.RESTORE_EXP:
				if player.restore_level():
					identified = true
		# Restore strength restores your strength
		PotionType.RES_STR:
				if player.try_restoring_ability_score(Ability.STRENGTH):
					identified = true
		# Restore intelligence restores your intelligence
		PotionType.RES_INT:
				if player.try_restoring_ability_score(Ability.INTELLIGENCE):
					identified = true
		# Restore wisdom restores your wisdom
		PotionType.RES_WIS:
				if player.try_restoring_ability_score(Ability.WISDOM):
					identified = true
		# Restore dexterity restores your dexterity
		PotionType.RES_DEX:
				if player.try_restoring_ability_score(Ability.DEXTERITY):
					identified = true
		# Restore constitution restores your constitution
		PotionType.RES_CON:
				if player.try_restoring_ability_score(Ability.CONSTITUTION):
					identified = true
		# Restore charisma restores your charisma
		PotionType.RES_CHA:
				if player.try_restoring_ability_score(Ability.CHARISMA):
					identified = true
		# Strength increases your strength
		PotionType.INC_STR:
				if player.try_increasing_ability_score(Ability.STRENGTH):
					identified = true
		# Intelligence increases your intelligence
		PotionType.INC_INT:
				if player.try_increasing_ability_score(Ability.INTELLIGENCE):
					identified = true
		# Wisdom increases your wisdom
		PotionType.INC_WIS:
				if player.try_increasing_ability_score(Ability.WISDOM):
					identified = true
		# Dexterity increases your dexterity
		PotionType.INC_DEX:
				if player.try_increasing_ability_score(Ability.DEXTERITY):
					identified = true
		# Constitution increases your constitution
		PotionType.INC_CON:
				if player.try_increasing_ability_score(Ability.CONSTITUTION):
					identified = true
		# Charisma increases your charisma
		PotionType.INC_CHA:
				if player.try_increasing_ability_score(Ability.CHARISMA):
					identified = true
		# Augmentation increases all ability scores
		PotionType.AUGMENTATION:
				if player.try_increasing_ability_score(Ability.STRENGTH):
					identified = true
				if player.try_increasing_ability_score(Ability.INTELLIGENCE):
					identified = true
				if player.try_increasing_ability_score(Ability.WISDOM):
					identified = true
				if player.try_increasing_ability_score(Ability.DEXTERITY):
					identified = true
				if player.try_increasing_ability_score(Ability.CONSTITUTION):
					identified = true
				if player.try_increasing_ability_score(Ability.CHARISMA):
					identified = true
		# Enlightenment shows you the whole level
		PotionType.ENLIGHTENMENT:
				Profile.instance.message_print("An image of your surroundings forms in your mind...")
				dungeon_level.wiz_light()
				identified = true
		# *Enlightenment* shows you the whole level, increases your intelligence and
		# wisdom, identifies all your items, and detects everything
		PotionType.STAR_ENLIGHTENMENT:
				Profile.instance.message_print("You begin to feel more enlightened...")
				Profile.instance.message_print("")
				dungeon_level.wiz_light()
				player.try_increasing_ability_score(Ability.INTELLIGENCE)
				player.try_increasing_ability_score(Ability.WISDOM)
				savegame.spell_effects_handler.detect_traps()
				savegame.spell_effects_handler.detect_doors()
				savegame.spell_effects_handler.detect_stairs()
				savegame.spell_effects_handler.detect_treasure()
				savegame.spell_effects_handler.detect_items_gold()
				savegame.spell_effects_handler.detect_items_normal()
				savegame.spell_effects_handler.identify_pack()
				savegame.spell_effects_handler.self_knowledge()
				identified = true
		# Self knowledge gives you information about yourself
		PotionType.SELF_KNOWLEDGE:
				Profile.instance.message_print("You begin to know yourself a little better...")
				Profile.instance.message_print("")
				savegame.spell_effects_handler.self_knowledge()
				identified = true
		# Experience increases your experience points by 50%, with a minimum of +10 and
		# maximuum of +10,000
		PotionType.EXPERIENCE:
				if player.experience_points < Constants.MAX_EXPERIENCE:
					var ee:int = (player.experience_points / 2) + 10
					if ee > 100000:
						ee = 100000
					Profile.instance.message_print("You feel more experienced.")
					player.gain_experience(ee)
					identified = true
		# Resistance gives you all timed resistances
		PotionType.RESISTANCE:
				player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(20) + 20)
				player.set_timed_lightning_resistance(player.timed_lightning_resistance + Rng.roll_die(20) + 20)
				player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(20) + 20)
				player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(20) + 20)
				player.set_timed_poison_resistance(player.timed_poison_resistance + Rng.roll_die(20) + 20)
				identified = true
		# Curing heals you 50 health, and cures blindness, confusion, stun, poison,
		# bleeding, and hallucinations
		PotionType.CURING:
				if player.restore_health(50):
					identified = true
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
				if player.set_timed_hallucinations(0):
					identified = true
		# Invulnerability gives you temporary invulnerability
		PotionType.INVULNERABILITY:
				player.set_timed_invulnerability(player.timed_invulnerability + Rng.roll_die(7) + 7)
				identified = true
		# New life rerolls your health, cures all mutations, and restores you to your birth race
		PotionType.NEW_LIFE:
				player.reroll_hit_points()
				if player.genome.has_mutations():
					Profile.instance.message_print("You are cured of all mutations.")
					player.genome.lose_all_mutations()
					player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
					savegame.notice_update_redraw()
				if player.current_ancestry != player.birth_ancestry:
					var old_ancestry:String = Ancestry.named_ancestry(player.birth_ancestry).title
					Profile.instance.message_print("You feel more %s again." % [old_ancestry])
					player.change_ancestry(player.birth_ancestry)
					savegame.dungeon_level.redraw_single_location(player.map_row, player.map_col)
				identified = true
	return identified


func random_potion() -> int:
	var sub_category:int = 0
	match Rng.roll_die(47):
		1:
			sub_category = PotionType.WATER
		2:
			sub_category = PotionType.APPLE_JUICE
		3:
			sub_category = PotionType.SLOWNESS
		4:
			sub_category = PotionType.SALT_WATER
		5:
			sub_category = PotionType.POISON
		6:
			sub_category = PotionType.BLINDNESS
		7:
			sub_category = PotionType.CONFUSION
		8:
			sub_category = PotionType.SLEEP
		9:
			sub_category = PotionType.INFRAVISION
		10:
			sub_category = PotionType.DETECT_INVIS
		11:
			sub_category = PotionType.SLOWNESS
		12:
			sub_category = PotionType.CURE_POISON
		13:
			sub_category = PotionType.BOLDNESS
		14:
			sub_category = PotionType.SPEED
		15:
			sub_category = PotionType.RESIST_HEAT
		16:
			sub_category = PotionType.RESIST_COLD
		17:
			sub_category = PotionType.HEROISM
		18:
			sub_category = PotionType.BESERK_STRENGTH
		19:
			sub_category = PotionType.CURE_LIGHT
		20:
			sub_category = PotionType.CURE_SERIOUS
		21:
			sub_category = PotionType.CURE_CRITICAL
		22:
			sub_category = PotionType.HEALING
		23:
			sub_category = PotionType.STAR_HEALING
		24:
			sub_category = PotionType.LIFE
		25:
			sub_category = PotionType.RESTORE_POWER
		26:
			sub_category = PotionType.RESTORE_EXP
		27:
			sub_category = PotionType.RES_STR
		28:
			sub_category = PotionType.RES_INT
		29:
			sub_category = PotionType.RES_WIS
		30:
			sub_category = PotionType.RES_DEX
		31:
			sub_category = PotionType.RES_CON
		32:
			sub_category = PotionType.RES_CHA
		33:
			sub_category = PotionType.INC_STR
		34:
			sub_category = PotionType.INC_INT
		35:
			sub_category = PotionType.INC_WIS
		36:
			sub_category = PotionType.INC_DEX
		37:
			sub_category = PotionType.INC_CON
		38:
			sub_category = PotionType.INC_CHA
		39:
			sub_category = PotionType.AUGMENTATION
		40:
			sub_category = PotionType.ENLIGHTENMENT
		41:
			sub_category = PotionType.STAR_ENLIGHTENMENT
		42:
			sub_category = PotionType.SELF_KNOWLEDGE
		43:
			sub_category = PotionType.EXPERIENCE
		44:
			sub_category = PotionType.RESISTANCE
		45:
			sub_category = PotionType.CURING
		46:
			sub_category = PotionType.INVULNERABILITY
		47:
			sub_category = PotionType.NEW_LIFE
	return sub_category


func ring_of_set_power(direction:int) -> void:
	match Rng.roll_die(10):
		1, 2:
				# Decrease all the players ability scores, bypassing sustain and divine protection
				Profile.instance.message_print("You are surrounded by a malignant aura.")
				player.decrease_ability_score(Ability.STRENGTH, 50, true)
				player.decrease_ability_score(Ability.INTELLIGENCE, 50, true)
				player.decrease_ability_score(Ability.WISDOM, 50, true)
				player.decrease_ability_score(Ability.DEXTERITY, 50, true)
				player.decrease_ability_score(Ability.CONSTITUTION, 50, true)
				player.decrease_ability_score(Ability.CHARISMA, 50, true)
				# Reduce both experience and maximum experience
				player.experience_points -= player.experience_points / 4
				player.max_experience_gained -= player.experience_points / 4
				player.check_experience()
		3:
				# Dispel monsters
				Profile.instance.message_print("You are surrounded by a powerful aura.")
				savegame.spell_effects_handler.dispel_monsters(1000)
		4, 5, 6:
				# Do a 300 damage power ball
				savegame.spell_effects_handler.fire_ball(ProjectilePower.new(savegame.spell_effects_handler), direction, 300, 3)
		7, 8, 9, 10:
				# Do a 250 damage power bolt
				savegame.spell_effects_handler.fire_bolt(ProjectilePower.new(savegame.spell_effects_handler), direction, 250)


func rustproof() -> void:
	# Get a piece of armour
	savegame.item_filter_function = savegame.spell_effects_handler.item_filter_armour
	var item_index: int = savegame.get_item("Rustproof which piece of armour? ", true, true, true)
	savegame.item_filter_function = Callable()
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to rustproof.")
		return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var item_name: String = item.description(false, 0)
	# Set the ignore acid flag
	item.legendary_flags_3.set_bits(ItemFlags3.IGNORE_ACID)
	# Make sure the grammar of the message is correct
	var your:String = "Your" if item_index > 0  else "The"
	var s:String = "" if item.Count > 1 else "s"
	if item.bonus_armour_class < 0 and item.identify_flags.is_clear(IdentifyFlags.CURSED):
		Profile.instance.message_print("%s %s look%s as good as new!" % [your,item_name,s])
		item.bonus_armour_class = 0
	s =  "are" if item.Count > 1 else "is"
	Profile.instance.message_print("%s %s %s now protected against corrosion." % [your,item_name,s])


func summon_item(dir:int,  max_weight:int,  require_los:bool) -> void:
	var target_row:int
	var target_col:int
	var tile:GridTile
	# Can't summon something if we're already standing on something
	if dungeon_level.get_grid(player.map_row,player.map_col).item_index != 0:
		Profile.instance.message_print("You can't fetch when you're already standing on something.")
		return
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# If we didn't have a direction, we might have an existing target
	if dir == 5 and target_engine.target_okay():
		target_col = savegame.target_col
		target_row = savegame.target_row
		# Check the range
		if dungeon_level.distance(player.map_row, player.map_col, target_row, target_col) > DungeonLevel.MAX_SPELL_RANGE:
			Profile.instance.message_print("You can't fetch something that far away!")
			return
		# Check the line of sight if needed
		tile = dungeon_level.get_grid(target_row,target_col)
		if require_los and not dungeon_level.player_has_los_bold(target_row, target_col):
			Profile.instance.message_print("You have no direct line of sight to that location.")
			return
	else:
		# We have a direction, so move along it until we find an item
		target_row = player.map_row
		target_col = player.map_col
		tile = dungeon_level.get_grid(target_row,target_col)
		while tile.item_index == 0:
			target_row += dungeon_level.keypad_direction_row_offset[dir]
			target_col += dungeon_level.keypad_direction_col_offset[dir]
			tile = dungeon_level.get_grid(target_row,target_col)
			# Stop if we hit max range or we're blocked by something
			if dungeon_level.distance(player.map_row, player.map_col, target_row, target_col) > DungeonLevel.MAX_SPELL_RANGE or not dungeon_level.grid_passable(target_row, target_col):
				return
	var item:Item = dungeon_level.dungeon_items.items[tile.item_index]
	# Check the weight of the item
	if item.weight > max_weight:
		Profile.instance.message_print("The object is too heavy.")
		return
	# Remove the entire item stack from the tile and move it to the player's tile
	var item_index: int = tile.item_index
	tile.item_index = 0
	dungeon_level.get_grid(player.map_row,player.map_col).item_index = item_index
	item.map_row = player.map_row
	item.map_col = player.map_col
	dungeon_level.note_spot(player.map_row, player.map_col)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func tunnel_through_tile( row:int,  col:int) -> bool:
	var repeat:bool = false
	# Tunnelling uses an entire turn
	savegame.energy_use = 100
	var tile: GridTile = dungeon_level.get_grid(row,col)
	# Trees are easy to chop down
	if tile.feature_type.category == FloorTileTypeCategory.TREE:
		if player.skill_digging >= 40 + Rng.roll_die(100) and remove_tile_via_tunnelling(row, col):
			Profile.instance.message_print("You have chopped down the %s." % [tile.feature_type.description])
		else:
			Profile.instance.message_print("You hack away at the %s." % [tile.feature_type.description])
			repeat = true
	# Pillars are a bit easier than walls
	elif tile.feature_type.name == "Pillar":
		if player.skill_digging >= 40 + Rng.roll_die(300) and remove_tile_via_tunnelling(row, col):
			Profile.instance.message_print("You have broken down the pillar.")
		else:
			Profile.instance.message_print("You hack away at the pillar.")
			repeat = true
	# We can't tunnel through water
	elif tile.feature_type.name == "Water":
		Profile.instance.message_print("The water fills up your tunnel as quickly as you dig!")
	# Permanent features resist being tunneled through
	elif tile.feature_type.is_permanent:
		Profile.instance.message_print("The %s resists your attempts to tunnel through it." % [tile.feature_type.description])
	# It's a wall, so we tunnel normally
	elif tile.feature_type.name.contains("Wall"):
		if player.skill_digging >= 40 + Rng.roll_die(1600) and remove_tile_via_tunnelling(row, col):
			Profile.instance.message_print("You have finished the tunnel.")
		else:
			Profile.instance.message_print("You tunnel into the granite wall.")
			repeat = true
	# It's a rock seam, so it might have treasure
	elif tile.feature_type.name.contains("Magma") or tile.feature_type.name.contains("Quartz"):
		var okay:bool
		var has_value:bool = false
		var is_magma:bool = false
		if tile.feature_type.name.contains("Treas"):
			has_value = true
		if tile.feature_type.name.contains("Magma"):
			is_magma = true
		# Magma needs a higher tunneling skill than quartz
		if is_magma:
			okay = player.skill_digging >= 20 + Rng.roll_die(800)
		else:
			okay = player.skill_digging >= 10 + Rng.roll_die(400)
		# Do the actual tunnelling
		if okay and remove_tile_via_tunnelling(row, col):
			if has_value:
				savegame.dungeon_level.place_gold(row, col)
				Profile.instance.message_print("You have found something!")
			else:
				Profile.instance.message_print("You have finished the tunnel.")
		# Tunnelling failed, so let us know
		elif is_magma:
			Profile.instance.message_print("You tunnel into the magma vein.")
			repeat = true
		else:
			Profile.instance.message_print("You tunnel into the quartz vein.")
			repeat = true
	# Rubble is easy to tunnel through
	elif tile.feature_type.name == "Rubble":
		if player.skill_digging >= Rng.roll_die(200) and remove_tile_via_tunnelling(row, col):
			Profile.instance.message_print("You have removed the rubble.")
			# 10% chance of finding something
			if Rng.roll_die(100) <= 10:
				savegame.dungeon_level.place_item(row, col, false, false)
				if dungeon_level.player_can_see_bold(row, col):
					Profile.instance.message_print("You have found something!")
		else:
			Profile.instance.message_print("You dig in the rubble.")
			repeat = true
	# We don't recognise what we're tunnelling into, so just assume it's permanent
	else:
		Profile.instance.message_print("The %s resists your attempts to tunnel through it." % [tile.feature_type.description])
		repeat = true
		search()
	# If we successfully made the tunnel,
	if not dungeon_level.grid_passable(row, col):
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_MONSTERS)
	return repeat


func remove_tile_via_tunnelling( row:int,  col:int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row,col)
	# If we can already get through it, we can't tunnel through it
	if dungeon_level.grid_passable(row, col):
		return false
	# clear_bits the tile
	tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
	dungeon_level.revert_tile_to_background(row, col)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_MONSTERS)
	return true


func trap_check_hit_on_player(attack_strength:int) -> bool:
	# Always a 5% chance to hit and 5% chance to miss
	var k: int = Rng.roll_die(100)
	if k <= 10:
		return k <= 5
	# If negative chance we miss
	if attack_strength <= 0:
		return false
	# Roll for the attack
	var armour_class: int = player.base_armour_class + player.armour_class_bonus
	return Rng.roll_die(attack_strength) > armour_class * 3 / 4
