# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ArtificialIntelligence
extends RefCounted

const MON_MULT_ADJ: int = 8
const FOLLOW_DISTANCE: int = 4
const BREAK_ELDER_SIGN: int = 550
const BREAK_YELLOW_SIGN: int = 99
const MAX_REPRO: int = 100

var player: Player
var dungeon_level: DungeonLevel
var savegame: SaveGame


func _init(the_player: Player, the_level: DungeonLevel) -> void:
	player = the_player
	dungeon_level = the_level
	savegame = SaveGame.instance


func process_all_monsters() -> void:
	# The noise the player is making is based on their stealth score
	var noise: int = 1 << (30 - player.skill_stealth)
	# Go through all the monster slots on the level
	for i: int in range(dungeon_level.monster_count_max - 1, 0, -1):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		# If the monster slot is empty, skip it
		if monster.monster_type == null:
			continue
		# Keep count of how many are our friends
		if monster.mind.is_set(MonsterMind.FRIENDLY):
			savegame.total_friends += 1
			savegame.total_friend_levels += monster.monster_type.level
		# dungeon_monsters.monsters that have just been spawned don't act in their first turn
		if monster.individual_monster_flags.is_set(IndividualMonsterFlags.BORN):
			monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.BORN)
			continue
		# Check the monster's speed to see if it should get a turn
		monster.energy += Tables.EXTRACT_ENERGY[monster.speed]
		if monster.energy < 100:
			continue
		# The monster is going to take a turn, even if it does nothing
		monster.energy -= 100
		# If the monster is too far away, don't even bother
		if monster.distance_from_player >= 100:
			continue
		var monster_type: MonsterType = monster.monster_type
		var monster_col: int = monster.map_col
		var monster_row: int = monster.map_row
		var test: bool = false
		# Check to see if the monster notices the player
		# 1) We're in range
		if monster.distance_from_player <= monster_type.notice_range:
			test = true
		# 2) We're aggravating
		elif monster.distance_from_player <= dungeon_level.MAX_SIGHT_RADIUS and (dungeon_level.player_has_los_bold(monster_row, monster_col) or player.has_aggravation):
			test = true
		# 3) We've left scent where the monster is so it can smell us
		elif (dungeon_level.get_grid(player.map_row, player.map_col).scent_age == dungeon_level.get_grid(monster_row, monster_col).scent_age and
			dungeon_level.get_grid(monster_row, monster_col).scent_strength < dungeon_level.SCENT_LIMIT and
			dungeon_level.get_grid(monster_row, monster_col).scent_strength < monster_type.notice_range ):
			test = true
		# If it didn't notice us, skip to the next monster
		if not test:
			continue
		dungeon_level.dungeon_monsters.currently_acting_monster = i
		# Process the individual monster
		process_monster(i, noise)
		# If the monster killed the player or sent us to a new level, then stop processing
		if (not savegame.playing or player.is_dead or savegame.new_level_flag):
			break
	dungeon_level.dungeon_monsters.currently_acting_monster = 0


func process_monster(monster_index: int, noise: int) -> void:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# Is the monster asleep?
	if monster.sleep_level != 0:
		# if the player aggravates, notice them more
		var notice: int = 0
		if not player.has_aggravation:
			notice = Rng.roll_die(1024) - 1
		# If the player makes too much noise (or aggravates)
		if notice * notice * notice <= noise:
			var wake_amount: int = 1
			if monster.distance_from_player < 50:
				wake_amount = 100 / monster.distance_from_player
			# Aggravate wakes the monster fully, if it notices at all
			if player.has_aggravation:
				wake_amount = monster.sleep_level
			# If we're not awake yet, just sleep less deeply
			if monster.sleep_level > wake_amount:
				monster.sleep_level -= wake_amount
				# The player may notice
				if monster.is_visible and monster_type.knowledge.r_ignore < 255:
					monster_type.knowledge.r_ignore += 1
			else:
				# Wake us up
				monster.sleep_level = 0
				# If the player sees us wake up, let them know
				if monster.is_visible:
					var monster_name: String = monster.describe(0)
					Profile.instance.message_print("%s wakes up." % [monster_name])
					# And let the player notice how easily we wake
					if monster_type.knowledge.r_wake < 255:
						monster_type.knowledge.r_wake += 1
		# If we're still asleep after all that, do nothing else for our turn
		if monster.sleep_level != 0:
			return
	# If we're stunned, then reduce our stun level
	if monster.stun_level != 0:
		var stun_relief: int = 1
		# We have a level-based chance of shaking off the stun completely
		if Rng.roll_die(5000) - 1 <= monster_type.level * monster_type.level:
			stun_relief = monster.stun_level
		# Reduce our stun if the relief is not enough to get rid of it
		if monster.stun_level > stun_relief:
			monster.stun_level -= stun_relief
		else:
			# Get rid of all of our stun
			monster.stun_level = 0
			# If the player sees us, let them know we're no longer stunned
			if monster.is_visible:
				var monster_name: String = monster.describe(0)
				Profile.instance.message_print("%s is no longer stunned." % [monster_name])
		# If we are still stunned, don't take a turn
		if monster.stun_level != 0:
			return
	# If we're confused
	if monster.confusion_level != 0:
		# Reduce our confusion by an amount based on our level
		var confusion_relief: int = Rng.roll_die((monster_type.level / 10) + 1)
		if monster.confusion_level > confusion_relief:
			monster.confusion_level -= confusion_relief
		else:
			# If we're no longer confused, the player will see this
			monster.confusion_level = 0
			if monster.is_visible:
				var monster_name: String = monster.describe(0)
				Profile.instance.message_print("%s is no longer confused." % [monster_name])
	# If we're curently friendly and the player aggravates, then stop being friendly
	var gets_angry: bool = false
	if monster.mind.is_set(MonsterMind.FRIENDLY) and player.has_aggravation:
		gets_angry = true
	# If we're unique, don't stay friendly
	if monster.mind.is_set(MonsterMind.FRIENDLY) and not player.is_wizard and monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		gets_angry = true
	# If we got angry, let the player see that
	if gets_angry:
		var monster_name: String = monster.describe(0)
		Profile.instance.message_print("%s suddenly becomes hostile!" % [monster_name])
		monster.mind.clear_bits(MonsterMind.FRIENDLY)
	# Are we afraid?
	if monster.fear_level != 0:
		# Reduce our fear by an amount based on our level
		var fear_relief: int = Rng.roll_die((monster_type.level / 10) + 1)
		if monster.fear_level > fear_relief:
			monster.fear_level -= fear_relief
		else:
			monster.fear_level = 0
			# If the player can see us, they can see we're no longer afraid
			if monster.is_visible:
				var monster_name: String = monster.describe(0)
				var monster_possessive: String = monster.describe(0x22)
				Profile.instance.message_print("%s recovers %s courage." % [monster_name, monster_possessive])
	var old_row: int = monster.map_row
	var old_col: int = monster.map_col
	# If it's suitable for us to reproduce
	if monster_type.flags_2.is_set(MonsterFlags2.MULTIPLY) and dungeon_level.dungeon_monsters.num_repro < MAX_REPRO and monster.generation < 10:
		# Find how many spaces we've got near us
		var k: int = 0
		for row: int in range(old_row - 1, old_row + 2):
			for col: int in range(old_col - 1, old_col + 2):
				if dungeon_level.get_grid(row, col).monster_index != 0:
					k += 1
		# If we're friendly, then our babies are friendly too
		var is_friend: bool = monster.mind.is_set(MonsterMind.FRIENDLY)
		# If there's lots of space, then pop out a baby
		if k < 4 and (k == 0 or Rng.roll_die(k * MON_MULT_ADJ) == 1):
			if dungeon_level.dungeon_monsters.multiply_monster(monster_index, is_friend, false):
				# If the player saw this, they now know we can multiply
				if monster.is_visible:
					monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.MULTIPLY)
				# Having a baby takes our entire turn
				return
	# If we can usefully cast a spell against the player, then that's our turn
	if try_casting_spell_against_player(monster_index):
		return
	# If we can usefully cast a spell against another monster, then that's our turn
	if try_casting_spell_against_another_monster(monster_index):
		return
	# Initialise our eight possible moves
	var potential_moves: Array[int] = [0, 0, 0, 0, 0, 0, 0, 0]
	# If we're confused, have four attempts to move randomly
	if monster.confusion_level != 0:
		potential_moves[0] = 5
		potential_moves[1] = 5
		potential_moves[2] = 5
		potential_moves[3] = 5
	# If we move randomly most of the time, have a high chance of putting four random moves
	# in the matrix
	elif monster_type.flags_1.is_set(MonsterFlags1.RANDOM_MOVE_50) and monster_type.flags_1.is_set(MonsterFlags1.RANDOM_MOVE_25) and Rng.roll_die(100) <= 75:
		# If the player sees us, then they learn about our random movement
		if monster.is_visible:
			monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.RANDOM_MOVE_50)
			monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.RANDOM_MOVE_25)
		potential_moves[0] = 5
		potential_moves[1] = 5
		potential_moves[2] = 5
		potential_moves[3] = 5
	# If we have a moderate chance of moving randomly, maybe put four random moves in our matrix
	elif monster_type.flags_1.is_set(MonsterFlags1.RANDOM_MOVE_50) and Rng.roll_die(100) <= 50:
		# If the player sees us, then they learn about our random movement
		if monster.is_visible:
			monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.RANDOM_MOVE_50)
		potential_moves[0] = 5
		potential_moves[1] = 5
		potential_moves[2] = 5
		potential_moves[3] = 5
	# If we have a low chance of moving randomly, maybe put four random moves in our matrix
	elif monster_type.flags_1.is_set(MonsterFlags1.RANDOM_MOVE_25) and Rng.roll_die(100) <= 25:
		# If the player sees us, then they learn about our random movement
		if monster.is_visible:
			monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.RANDOM_MOVE_25)
		potential_moves[0] = 5
		potential_moves[1] = 5
		potential_moves[2] = 5
		potential_moves[3] = 5
	# If we're the player's friend and we're too far away, add sensible moves to our matrix
	elif monster.mind.is_set(MonsterMind.FRIENDLY):
		if monster.distance_from_player > FOLLOW_DISTANCE:
			get_moves_toward_player(monster_index, potential_moves)
		else:
			# If we're close to the player (and friendly) just use random moves
			potential_moves[0] = 5
			potential_moves[1] = 5
			potential_moves[2] = 5
			potential_moves[3] = 5
			# Possibly override these random moves with attacks on enemies
			get_moves_toward_enemy_monster(monster, potential_moves)
	# If all the above fail, we must be a hostile monster who wants to move towards the player
	else:
		# If we fail to get sensible moves, give up on our turn
		if not get_moves_toward_player(monster_index, potential_moves):
			return
	var do_turn: int = false
	var do_move: int = false
	var do_view: int = false
	var did_open_door: int = false
	var did_bash_door: int = false
	var did_take_item: int = false
	var did_kill_item: int = false
	var did_move_body: int = false
	var did_kill_body: int = false
	var did_passwall: int = false
	var did_kill_wall: int = false
	# Go through our possible moves until we come to the limit of the ones we've had suggested
	var direction_index: int = 0
	while potential_moves[direction_index] != 0:
		var d: int = potential_moves[direction_index]
		direction_index += 1
		# Moves of '5' (i.e. 'stay still') are placeholders for random moves
		if d == 5:
			d = dungeon_level.ordered_direction[Rng.roll_die(8) - 1]
		# Work out where the move will take us
		var new_row: int = old_row + dungeon_level.keypad_direction_row_offset[d]
		var new_col: int = old_col + dungeon_level.keypad_direction_col_offset[d]
		var tile: GridTile = dungeon_level.get_grid(new_row, new_col)
		var monster_in_target_tile: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
		# If we can simply move there, then we will do so
		if dungeon_level.grid_passable(new_row, new_col):
			do_move = true
		# Bushes don't actually block us, so we can move there too
		elif dungeon_level.get_grid(new_row, new_col).feature_type.name == "Bush":
			do_move = true
		# We can always attack the player, even if the move would otherwse not be allowed
		elif new_row == player.map_row and new_col == player.map_col:
			do_move = true
		# We can always attack another monster, even if the move would otherwise not be allowed
		elif tile.monster_index != 0:
			do_move = true
		# We can never go through permanent walls even if we can phase through or destroy
		# normal walls
		elif tile.feature_type.is_permanent:
			pass
		# If we can phase through walls then they don't block us
		elif monster_type.flags_2.is_set(MonsterFlags2.PASSWALL):
			do_move = true
			did_passwall = true
		# If we can tunnel through walls then they don't block us
		elif monster_type.flags_2.is_set(MonsterFlags2.KILL_WALL):
			do_move = true
			did_kill_wall = true
			# Occasionally make a noise if we're going to tunnel
			if Rng.roll_die(20) == 1:
				Profile.instance.message_print("There is a grinding sound.")
			# Remove the wall (and the player's memory of it) and remind ourselves to
			# update the view if the player can see it
			tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
			dungeon_level.revert_tile_to_background(new_row, new_col)
			if dungeon_level.player_has_los_bold(new_row, new_col):
				do_view = true
		# If we're trying to get through a door
		elif tile.feature_type.is_closed_door:
			var may_bash: int = true
			do_turn = true
			# If we can open the door then try to do so
			if monster_type.flags_2.is_set(MonsterFlags2.OPEN_DOOR):
				# We can always open unlocked doors
				if tile.feature_type.name == "LockedDoor0" or tile.feature_type.name == "SecretDoor":
					did_open_door = true
					may_bash = false
				# We have a chance to unlock locked doors
				elif tile.feature_type.name.contains("Locked"):
					var k: int = int(tile.feature_type.name.substr(10))
					if Rng.roll_die(monster.health / 10) - 1 > k:
						dungeon_level.cave_set_feat(new_row, new_col, "LockedDoor0")
						may_bash = false
			# If we can't open doors (or failed to unlock the door), then we can bash it down
			if may_bash and monster_type.flags_2.is_set(MonsterFlags2.BASH_DOOR):
				var k: int = int(tile.feature_type.name.substr(10))
				# If we succeeded, let the player hear it
				if Rng.roll_die(monster.health / 10) - 1 > k:
					Profile.instance.message_print("You hear a door burst open!")
					did_bash_door = true
					do_move = true
			# If we opened it or bashed it, replace the closed door with the relevant open
			# or broken one
			if did_open_door or did_bash_door:
				if did_bash_door and Rng.roll_die(100) <= 50:
					dungeon_level.cave_set_feat(new_row, new_col, "BrokenDoor")
				else:
					dungeon_level.cave_set_feat(new_row, new_col, "OpenDoor")
				# If the player can see, remind ourselves to update the view later
				if dungeon_level.player_has_los_bold(new_row, new_col):
					do_view = true
		# If we're going to move onto an Elder Sign and we're capable of doing attacks
		if do_move and tile.feature_type.name == "ElderSign" and monster_type.flags_1.is_clear(MonsterFlags1.NEVER_ATTACK):
			# Assume we're not moving
			do_move = false
			# We have a chance of breaking the sign based on our level
			if Rng.roll_die(BREAK_ELDER_SIGN) < monster_type.level:
				# If the player knows the sign is there, let them know it was broken
				if tile.tile_flags.is_set(GridTile.PLAYER_MEMORISED):
					Profile.instance.message_print("The Elder Sign is broken!")
				tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.revert_tile_to_background(new_row, new_col)
				# Breaking the sign means we can move after all
				do_move = true
		# If we're going to move onto a Yellow Sign and we can attack
		elif do_move and tile.feature_type.name == "YellowSign" and monster_type.flags_1.is_clear(MonsterFlags1.NEVER_ATTACK):
			# Assume we're not moving
			do_move = false
			# We have a chance to break the sign
			if Rng.roll_die(BREAK_YELLOW_SIGN) < monster_type.level:
				# If the player knows about the sign, let them know it was broken
				if tile.tile_flags.is_set(GridTile.PLAYER_MEMORISED):
					# If the player was on the sign, hurt them
					if new_row == player.map_row and new_col == player.map_col:
						Profile.instance.message_print("The rune explodes!")
						savegame.spell_effects_handler.fire_ball(ProjectilePower.new(savegame.spell_effects_handler), 0, 2 * ((player.level / 2) + Rng.roll_dice(7, 7)), 2)
					else:
						Profile.instance.message_print("An Yellow Sign was disarmed.")
				tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.revert_tile_to_background(new_row, new_col)
				# We can do the move after all
				do_move = true
		# If we're going to attack the player, but our monster_type never attacks, then cancel the move
		if do_move and new_row == player.map_row and new_col == player.map_col and monster_type.flags_1.is_set(MonsterFlags1.NEVER_ATTACK):
			do_move = false
		# If we're trying to move onto the player, then attack them instead
		if do_move and new_row == player.map_row and new_col == player.map_col:
			savegame.combat_engine.monster_attack_player(monster_index)
			do_move = false
			do_turn = true
		# If we're trying to move onto another monster
		if do_move and tile.monster_index != 0:
			var target_monster_type: MonsterType = monster_in_target_tile.monster_type
			# Assume for the moment we're not doing the move
			do_move = false
			# If we can trample other monsters on our team and we're tougher than the one
			# that's in our way...
			if (monster_type.flags_2.is_set(MonsterFlags2.KILL_BODY) and monster_type.experience > target_monster_type.experience and
				dungeon_level.grid_passable(new_row, new_col) and
				not (monster.mind.is_set(MonsterMind.FRIENDLY) and monster_in_target_tile.mind.is_set(MonsterMind.FRIENDLY)) ):
				# Remove the other monster and replace it
				do_move = true
				did_kill_body = true
				dungeon_level.delete_monster_at(new_row, new_col)
				monster_in_target_tile = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
			# If we're not on the same team as the other monster or we're confused
			elif monster.mind.is_set(MonsterMind.FRIENDLY) != monster_in_target_tile.mind.is_set(MonsterMind.FRIENDLY) or monster.confusion_level != 0:
				do_move = false
				# attack the monster in the target tile
				if monster_in_target_tile.monster_type != null and monster_in_target_tile.health >= 0:
					if attack_another_monster(monster_index, tile.monster_index):
						return
			# If the other monster is on our team and we can't trample it, maybe we can
			# push past
			elif (monster_type.flags_2.is_set(MonsterFlags2.MOVE_BODY) and monster_type.experience > target_monster_type.experience and
				dungeon_level.grid_passable(new_row, new_col) and dungeon_level.grid_passable(monster.map_row, monster.map_col) ):
				do_move = true
				did_move_body = true
		# If we're going to do a move but we can't move, cancel it
		if do_move and monster_type.flags_1.is_set(MonsterFlags1.NEVER_MOVE):
			do_move = false
		# If we're going to do a move
		if do_move:
			do_turn = true
			# Swap positions with the monster that is in the tile we're aiming for
			dungeon_level.get_grid(old_row, old_col).monster_index = tile.monster_index
			# If it was actually a monster then update it accordingly
			if tile.monster_index != 0:
				monster_in_target_tile.map_row = old_row
				monster_in_target_tile.map_col = old_col
				dungeon_level.dungeon_monsters.update_monster_visibility(tile.monster_index, true)
				# Pushing past something wakes it up
				dungeon_level.dungeon_monsters.monsters[tile.monster_index].sleep_level = 0
			# Update our position
			tile.monster_index = monster_index
			monster.map_row = new_row
			monster.map_col = new_col
			dungeon_level.dungeon_monsters.update_monster_visibility(monster_index, true)
			dungeon_level.redraw_single_location(old_row, old_col)
			dungeon_level.redraw_single_location(new_row, new_col)
			# If we are hostile and the player saw us move, then disturb them
			if monster.is_visible and monster.individual_monster_flags.is_set(IndividualMonsterFlags.VIEW):
				if monster.mind.is_clear(MonsterMind.FRIENDLY):
					savegame.disturb(false)
			# Check through the items in the tile we just entered
			var item_index: int = tile.item_index
			while item_index != 0:
				var item: Item = dungeon_level.dungeon_items.items[item_index]
				var next_item_index: int = item.next_in_stack
				# We ignore gold
				if item.category == ItemCategory.GOLD:
					item_index = next_item_index
					continue
				# If we pick up or stomp on items, check the item type
				if (monster_type.flags_2.is_set(MonsterFlags2.TAKE_ITEM) or monster_type.flags_2.is_set(MonsterFlags2.KILL_ITEM)) and monster.mind.is_clear(MonsterMind.FRIENDLY):
					var f_1: FlagSet = FlagSet.new()
					var f_2: FlagSet = FlagSet.new()
					var f_3: FlagSet = FlagSet.new()
					var flags_3: FlagSet = FlagSet.new()
					item.get_merged_flags(f_1, f_2, f_3)
					var item_name: String = item.description(true, 3)
					var monster_name: String = monster.describe(0x04)
					if f_1.is_set(ItemFlags1.KILL_DRAGON):
						flags_3.set_bits(MonsterFlags3.DRAGON)
					if f_1.is_set(ItemFlags1.SLAY_DRAGON):
						flags_3.set_bits(MonsterFlags3.DRAGON)
					if f_1.is_set(ItemFlags1.SLAY_TROLL):
						flags_3.set_bits(MonsterFlags3.TROLL)
					if f_1.is_set(ItemFlags1.SLAY_GIANT):
						flags_3.set_bits(MonsterFlags3.GIANT)
					if f_1.is_set(ItemFlags1.SLAY_ORC):
						flags_3.set_bits(MonsterFlags3.ORC)
					if f_1.is_set(ItemFlags1.SLAY_DEMON):
						flags_3.set_bits(MonsterFlags3.DEMON)
					if f_1.is_set(ItemFlags1.SLAY_UNDEAD):
						flags_3.set_bits(MonsterFlags3.UNDEAD)
					if f_1.is_set(ItemFlags1.SLAY_ANIMAL):
						flags_3.set_bits(MonsterFlags3.ANIMAL)
					if f_1.is_set(ItemFlags1.SLAY_EVIL):
						flags_3.set_bits(MonsterFlags3.EVIL)
					# dungeon_monsters.monsters won't pick up artifacts or items that hurt them
					if item.is_artifact() or monster_type.flags_3.is_set(flags_3.value) or item.is_legendary():
						if monster_type.flags_2.is_set(MonsterFlags2.TAKE_ITEM):
							did_take_item = true
							if monster.is_visible and dungeon_level.player_has_los_bold(new_row, new_col):
								Profile.instance.message_print("%s tries to pick up %s, but fails." % [monster_name, item_name])
					# If we picked up the item and the player saw, let them know
					elif monster_type.flags_2.is_set(MonsterFlags2.TAKE_ITEM):
						did_take_item = true
						if dungeon_level.player_has_los_bold(new_row, new_col):
							Profile.instance.message_print("%s picks up %s." % [monster_name, item_name])
						# And pick up the actual item
						dungeon_level.clear_item_index(item_index)
						item.marked = false
						item.map_row = 0
						item.map_col = 0
						item.holding_monster_index = monster_index
						item.next_in_stack = monster.first_held_item_index
						monster.first_held_item_index = item_index
					else:
						# We can't pick up the item, so just stomp on it
						did_kill_item = true
						# If the player saw us, let them know
						if dungeon_level.player_has_los_bold(new_row, new_col):
							Profile.instance.message_print("%s crushes %s." % [monster_name, item_name])
						savegame.level.delete_item_by_index(item_index)
				item_index = next_item_index
		# If we did something, then don't try another potential move
		if do_turn:
			break
	# If all our moves failed, have another go at casting a spell at the player
	if not do_turn and not do_move and monster.fear_level == 0 and monster.mind.is_clear(MonsterMind.FRIENDLY):
		if try_casting_spell_against_player(monster_index):
			return
	# Update the view if necessary
	if do_view:
		player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_MONSTERS)
	# If we did something unusual and the player saw, let them remember we can do that
	if monster.is_visible:
		if did_open_door:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.OPEN_DOOR)
		if did_bash_door:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.BASH_DOOR)
		if did_take_item:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.TAKE_ITEM)
		if did_kill_item:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.KILL_ITEM)
		if did_move_body:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.MOVE_BODY)
		if did_kill_body:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.KILL_BODY)
		if did_passwall:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.PASSWALL)
		if did_kill_wall:
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.KILL_WALL)
	# If we couldn't do anything because we were afraid and cornered, lose our fear
	if not do_turn and not do_move and monster.fear_level != 0:
		monster.fear_level = 0
		if monster.is_visible:
			var monster_name: String = monster.describe(0)
			Profile.instance.message_print("%s turns to fight!" % [monster_name])


func try_casting_spell_against_player(monster_index: int) -> bool:
	var spell: Array[int]
	var num: int = 0
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var no_innate: bool = false
	var col_player: int = player.map_col
	var row_player: int = player.map_row
	var count: int = 0
	var player_is_blind: bool = player.timed_blindness != 0
	var seen_by_player: bool = not player_is_blind and monster.is_visible
	# If the player's already dead or off the level, don't cast
	if not savegame.playing or player.is_dead or savegame.new_level_flag:
		return false
	# No spells if we're confused
	if monster.confusion_level != 0:
		return false
	# No spells if we're being nice because we've just been summoned
	if monster.individual_monster_flags.is_set(IndividualMonsterFlags.NICE):
		return false
	# No spells on the player if they're our friend
	if monster.mind.is_set(MonsterMind.FRIENDLY):
		return false
	# We aren't guaranteed to cast a spell even if we can
	var chance: int = (monster_type.freq_innate + monster_type.freq_spell) / 2
	if chance == 0:
		return false
	if Rng.roll_die(100) > chance:
		return false
	# Innate abilities are inherently less likely than actual spells
	if Rng.roll_die(100) > chance * 2:
		no_innate = true
	# If we're too far from the player don't cast a spell
	if monster.distance_from_player > dungeon_level.MAX_SIGHT_RADIUS:
		return false
	# If we have no line of sight to the player, don't cast a spell
	if not dungeon_level.projectable(monster.map_row, monster.map_col, row_player, col_player):
		return false
	# Gather together the abilities we have
	var monster_level: int = monster_type.level if monster_type.level >= 1 else 1
	var f_4: FlagSet = monster_type.flags_4
	var f_5: FlagSet = monster_type.flags_5
	var f_6: FlagSet = monster_type.flags_6
	# If we're not allowed innate abilities, then things on flags_4 can't be used
	if no_innate:
		f_4.clear_all()
	# If we're smart and badly injured, we may want to prioritise spells that disable the
	# player, summon help, or let us escape over spells that do direct damage
	if monster_type.flags_2.is_set(MonsterFlags2.SMART) and monster.health < monster.max_health / 10 and Rng.roll_die(100) <= 50:
		f_4.keep_bits(MonsterFlags4.INT_MASK)
		f_5.keep_bits(MonsterFlags5.INT_MASK)
		f_6.keep_bits(MonsterFlags6.INT_MASK)
		# If we just got rid of all our spells then don't cast
		if f_4.is_all_clear() and f_5.is_all_clear() and f_6.is_all_clear():
			return false
	# Ditch any spells that we've seen the player resist before so we know they'll be ineffective
	remove_ineffective_spells(monster_index, f_4, f_5, f_6)
	# If we just got rid of all our spells then don't cast
	if f_4.is_all_clear() and f_5.is_all_clear() and f_6.is_all_clear():
		return false
	# If we don't have a clean shot, and we're not stupid, remove bolt spells
	if monster_type.flags_2.is_clear(MonsterFlags2.STUPID) and not clean_shot(monster.map_row, monster.map_col, player.map_row, player.map_col):
		f_4.clear_bits(MonsterFlags4.BOLT_MASK)
		f_5.clear_bits(MonsterFlags5.BOLT_MASK)
		f_6.clear_bits(MonsterFlags6.BOLT_MASK)
	# If there's nowhere around the player to put a summoned creature, then remove
	# summoning spells
	if monster_type.flags_2.is_clear(MonsterFlags2.STUPID) and not summon_possible(player.map_row, player.map_col):
		f_4.clear_bits(MonsterFlags4.SUMMON_MASK)
		f_5.clear_bits(MonsterFlags5.SUMMON_MASK)
		f_6.clear_bits(MonsterFlags6.SUMMON_MASK)
	# If we've removed all our spells, don't cast anything
	if f_4.is_all_clear() and f_5.is_all_clear() and f_6.is_all_clear():
		return false
	# Gather up all the spells that are left and put them in an array of spells
	# Each is given an index of flag number (4-6) * 32 + bit number (0-31)
	# flags_4 spells are 96-127
	for k: int in 32:
		if f_4.is_set(1 << k):
			spell.append(k + 32 * 3)
	# flags_5 spells are 128-159
	for k: int in 32:
		if f_5.is_set(1 << k):
			spell.append(k + 32 * 4)
	# flags_6 spells are 160-191
	for k: int in 32:
		if f_6.is_set(1 << k):
			spell.append(k + 32 * 5)
	# If we ended up with no spells, don't cast
	num = spell.size()
	var monster_name: String = monster.describe(0x00)
	var monster_possessive: String = monster.describe(0x22)
	var monster_description: String = monster.describe(0x88)
	# Pick one of our spells to cast, based on our priorities
	var thrown_spell: int = choose_spell_against_player(monster_index, spell, num)
	# If we decided not to cast, don't
	if thrown_spell == 0:
		return false
	# STUPID monsters don't fail at innate abilities
	var fail_rate: int = 25 - ((monster_level + 3) / 4)
	if monster_type.flags_2.is_set(MonsterFlags2.STUPID):
		fail_rate = 0
	# Only check for actual spells - nothing is so stupid it fails to breathe
	if thrown_spell >= 128 and Rng.roll_die(100) <= fail_rate:
		Profile.instance.message_print("%s tries to cast a spell, but fails." % [monster_name])
		return true
	# Now do whatever the actual spell does
	match thrown_spell:
		# MonsterFlags4.Shriek
		96 + 0:
			savegame.disturb(true)
			Profile.instance.message_print("%s makes a high pitched shriek." % [monster_name])
			savegame.spell_effects_handlerAggravateMonsters(monster_index)
		# MonsterFlags4.Xxx2
		96 + 1:
			return false
		# MonsterFlags4.Xxx3
		96 + 2:
			return false
		# MonsterFlags4.ShardBall
		96 + 3:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles something." % [monster_name] if player_is_blind else "%s fires a shard ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileShard.new(savegame.spell_effects_handler), 800 if monster.health / 4 > 800 else monster.health / 4, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.SHARD)
		# MonsterFlags4.Arrow1D6
		96 + 4:
			savegame.disturb(true)
			Profile.instance.message_print("%s makes a strange noise." % [monster_name] if player_is_blind else "%s fires an arrow." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileArrow.new(savegame.spell_effects_handler), Rng.roll_dice(1, 6))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags4.Arrow3D6
		96 + 5:
			savegame.disturb(true)
			Profile.instance.message_print("%s makes a strange noise." % [monster_name] if player_is_blind else "%s fires an arrow!" % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileArrow.new(savegame.spell_effects_handler), Rng.roll_dice(3, 6))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags4.Arrow5D6
		96 + 6:
			savegame.disturb(true)
			Profile.instance.message_print("%s makes a strange noise." % [monster_name] if player_is_blind else "%s fires a missile." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileArrow.new(savegame.spell_effects_handler), Rng.roll_dice(5, 6))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags4.Arrow7D6
		96 + 7:
			savegame.disturb(true)
			Profile.instance.message_print("%s makes a strange noise." % [monster_name] if player_is_blind else "%s fires a missile!" % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileArrow.new(savegame.spell_effects_handler), Rng.roll_dice(7, 6))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags4.BreatheAcid
		96 + 8:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes acid." % [monster_name])
			breathe_at_player(monster_index, ProjectileAcid.new(savegame.spell_effects_handler), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ACID)
		# MonsterFlags4.BreatheLightning
		96 + 9:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes lightning." % [monster_name])
			breathe_at_player(monster_index, ProjectileElectricity.new(savegame.spell_effects_handler), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ELECTRICITY)
		# MonsterFlags4.BreatheFire
		96 + 10:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes fire." % [monster_name])
			breathe_at_player(monster_index, ProjectileFire.new(savegame.spell_effects_handler), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FIRE)
		# MonsterFlags4.BreatheCold
		96 + 11:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes frost." % [monster_name])
			breathe_at_player(monster_index, ProjectileCold.new(savegame.spell_effects_handler), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.COLD)
		# MonsterFlags4.BreathePoison
		96 + 12:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes gas." % [monster_name])
			breathe_at_player(monster_index, ProjectilePoison.new(savegame.spell_effects_handler), 800 if monster.health / 3 > 800 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
		# MonsterFlags4.BreatheNether
		96 + 13:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes nether." % [monster_name])
			breathe_at_player(monster_index, ProjectileNether.new(savegame.spell_effects_handler), 550 if monster.health / 6 > 550 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.NETHER)
		# MonsterFlags4.BreatheLight
		96 + 14:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes light." % [monster_name])
			breathe_at_player(monster_index, ProjectileLight.new(savegame.spell_effects_handler), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.LIGHT)
		# MonsterFlags4.BreatheDark
		96 + 15:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes darkness." % [monster_name])
			breathe_at_player(monster_index, ProjectileDark.new(savegame.spell_effects_handler), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.DARK)
		# MonsterFlags4.BreatheConfusion
		96 + 16:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes confusion." % [monster_name])
			breathe_at_player(monster_index, ProjectileConfusion.new(savegame.spell_effects_handler), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.CONFUSION)
		# MonsterFlags4.BreatheSound
		96 + 17:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes sound." % [monster_name])
			breathe_at_player(monster_index, ProjectileSound.new(savegame.spell_effects_handler), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.SOUND)
		# MonsterFlags4.BreatheChaos
		96 + 18:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes chaos." % [monster_name])
			breathe_at_player(monster_index, ProjectileChaos.new(savegame.spell_effects_handler), 600 if monster.health / 6 > 600 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.CHAOS)
		# MonsterFlags4.BreatheDisenchant
		96 + 19:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes disenchantment." % [monster_name])
			breathe_at_player(monster_index, ProjectileDisenchantment.new(savegame.spell_effects_handler), 500 if monster.health / 6 > 500 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.DISENCHANTMENT)
		# MonsterFlags4.BreatheNexus
		96 + 20:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes nexus." % [monster_name])
			breathe_at_player(monster_index, ProjectileNexus.new(savegame.spell_effects_handler), 250 if monster.health / 3 > 250 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.NEXUS)
		# MonsterFlags4.BreatheTime
		96 + 21:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes time." % [monster_name])
			breathe_at_player(monster_index, ProjectileTime.new(savegame.spell_effects_handler), 150 if monster.health / 3 > 150 else monster.health / 3, 0)
		# MonsterFlags4.BreatheInertia
		96 + 22:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes inertia." % [monster_name])
			breathe_at_player(monster_index, ProjectileInertia.new(savegame.spell_effects_handler), 200 if monster.health / 6 > 200 else monster.health / 6, 0)
		# MonsterFlags4.BreatheGravity
		96 + 23:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes gravity." % [monster_name])
			breathe_at_player(monster_index, ProjectileGravity.new(savegame.spell_effects_handler), 200 if monster.health / 3 > 200 else monster.health / 3, 0)
		# MonsterFlags4.BreatheShards
		96 + 24:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes shards." % [monster_name])
			breathe_at_player(monster_index, ProjectileExplode.new(savegame.spell_effects_handler), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.SHARD)
		# MonsterFlags4.BreathePlasma
		96 + 25:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes plasma." % [monster_name])
			breathe_at_player(monster_index, ProjectilePlasma.new(savegame.spell_effects_handler), 150 if monster.health / 6 > 150 else monster.health / 6, 0)
		# MonsterFlags4.BreatheForce
		96 + 26:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes force." % [monster_name])
			breathe_at_player(monster_index, ProjectileForce.new(savegame.spell_effects_handler), 200 if monster.health / 6 > 200 else monster.health / 6, 0)
		# MonsterFlags4.BreathePower
		96 + 27:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes magical energy." % [monster_name])
			breathe_at_player(monster_index, ProjectilePower.new(savegame.spell_effects_handler), 250 if monster.health / 3 > 250 else monster.health / 3, 0)
		# MonsterFlags4.RadiationBall
		96 + 28:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a ball of radiation." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileNuke.new(savegame.spell_effects_handler), monster_level + Rng.roll_dice(10, 6), 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
		# MonsterFlags4.BreatheRadiation
		96 + 29:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes toxic waste." % [monster_name])
			breathe_at_player(monster_index, ProjectileNuke.new(savegame.spell_effects_handler), 800 if monster.health / 3 > 800 else monster.health / 3, 0)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
		# MonsterFlags4.ChaosBall
		96 + 30:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles frighteningly." % [monster_name] if player_is_blind else "%s invokes raw chaos." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileChaos.new(savegame.spell_effects_handler), (monster_level * 2) + Rng.roll_dice(10, 10), 4)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.CHAOS)
		# MonsterFlags4.BreatheDisintegration
		96 + 31:
			savegame.disturb(true)
			Profile.instance.message_print("%s breathes." % [monster_name] if player_is_blind else "%s breathes disintegration." % [monster_name])
			breathe_at_player(monster_index, ProjectileDisintegrate.new(savegame.spell_effects_handler), 300 if monster.health / 3 > 300 else monster.health / 3, 0)
		# MonsterFlags5.AcidBall
		128 + 0:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts an acid ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileAcid.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 3) + 15, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ACID)
		# MonsterFlags5.LightningBall
		128 + 1:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a lightning ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileElectricity.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 3 / 2) + 8, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ELECTRICITY)
		# MonsterFlags5.FireBall
		128 + 2:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a fire ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileFire.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 7 / 2) + 10, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FIRE)
		# MonsterFlags5.ColdBall
		128 + 3:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a frost ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileCold.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 3 / 2) + 10, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.COLD)
		# MonsterFlags5.PoisonBall
		128 + 4:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a stinking cloud." % [monster_name])
			fire_ball_at_player(monster_index, ProjectilePoison.new(savegame.spell_effects_handler), Rng.roll_dice(12, 2), 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
		# MonsterFlags5.NetherBall
		128 + 5:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a nether ball." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileNether.new(savegame.spell_effects_handler), 50 + Rng.roll_dice(10, 10) + monster_level, 2)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.NETHER)
		# MonsterFlags5.WaterBall
		128 + 6:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s gestures fluidly." % [monster_name])
			Profile.instance.message_print("You are engulfed in a whirlpool.")
			fire_ball_at_player(monster_index, ProjectileWater.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 5 / 2) + 50, 4)
		# MonsterFlags5.PowerBall
		128 + 7:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles powerfully." % [monster_name] if player_is_blind else "%s invokes a vis storm." % [monster_name])
			fire_ball_at_player(monster_index, ProjectilePower.new(savegame.spell_effects_handler), (monster_level * 5) + Rng.roll_dice(10, 10), 4)
		# MonsterFlags5.DarkBall
		128 + 8:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles powerfully." % [monster_name] if player_is_blind else "%s invokes a darkness storm." % [monster_name])
			fire_ball_at_player(monster_index, ProjectileDark.new(savegame.spell_effects_handler), (monster_level * 5) + Rng.roll_dice(10, 10), 4)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.DARK)
		# MonsterFlags5.DrainVis
		128 + 9:
			if player.power != 0:
				savegame.disturb(true)
				Profile.instance.message_print("%s draws psychic energy from you!" % [monster_name])
				var r1: int = (Rng.roll_die(monster_level) / 2) + 1
				if r1 >= player.power:
					r1 = player.power
					player.power = 0
					player.fractional_power = 0
				else:
					player.power -= r1
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
				if monster.health < monster.max_health:
					monster.health += 6 * r1
					if monster.health > monster.max_health:
						monster.health = monster.max_health
					if savegame.tracked_monster_index == monster_index:
						player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
					if seen_by_player:
						Profile.instance.message_print("%s appears healthier." % [monster_name])
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POWER)
		# MonsterFlags5.MindBlast
		128 + 10:
			savegame.disturb(true)
			Profile.instance.message_print("You feel something focusing on your mind." if not seen_by_player else "%s gazes deep into your eyes." % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				Profile.instance.message_print("Your mind is blasted by psionic energy.")
				if not player.has_confusion_resistance:
					player.set_timed_confusion(player.timed_confusion + Rng.roll_die(4) + 3)
				if not player.has_chaos_resistance and Rng.roll_die(3) == 1:
					player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(250) + 149)
				player.take_hit(Rng.roll_dice(8, 8), monster_description)
		# MonsterFlags5.BrainSmash
		128 + 11:
			savegame.disturb(true)
			Profile.instance.message_print("You feel something focusing on your mind." if not seen_by_player else "%s looks deep into your eyes." % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				Profile.instance.message_print("Your mind is blasted by psionic energy.")
				player.take_hit(Rng.roll_dice(12, 15), monster_description)
				if not player.has_blindness_resistance:
					player.set_timed_blindness(player.timed_blindness + 7 + Rng.roll_die(8))
				if not player.has_confusion_resistance:
					player.set_timed_confusion(player.timed_confusion + Rng.roll_die(4) + 3)
				if not player.has_free_action:
					player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(4) + 3)
				player.set_timed_slow(player.timed_slow + Rng.roll_die(4) + 3)
				while Rng.roll_die(100) > player.skill_saving_throw + 1:
					player.try_decreasing_ability_score(Ability.INTELLIGENCE)
				while Rng.roll_die(100) > player.skill_saving_throw + 1:
					player.try_decreasing_ability_score(Ability.WISDOM)
				if not player.has_chaos_resistance:
					player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(250) + 149)
		# MonsterFlags5.CauseLightWounds
		128 + 12:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s points at you and curses." % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.curse_equipment(33, 0)
				player.take_hit(Rng.roll_dice(3, 8), monster_description)
		# MonsterFlags5.CauseSeriousWounds
		128 + 13:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s points at you and curses horribly." % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.curse_equipment(50, 5)
				player.take_hit(Rng.roll_dice(8, 8), monster_description)
		# MonsterFlags5.CauseCriticalWounds
		128 + 14:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles loudly." % [monster_name] if player_is_blind else "%s points at you, incanting terribly!" % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.curse_equipment(80, 15)
				player.take_hit(Rng.roll_dice(10, 15), monster_description)
		# MonsterFlags5.CauseMortalWounds
		128 + 15:
			savegame.disturb(true)
			Profile.instance.message_print("%s screams the word 'DIE!'" % [monster_name] if player_is_blind else "%s points at you, screaming the word DIE!" % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.take_hit(Rng.roll_dice(15, 15), monster_description)
				player.set_timed_bleeding(player.timed_bleeding + Rng.roll_dice(10, 10))
		# MonsterFlags5.AcidBolt
		128 + 16:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a acid bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileAcid.new(savegame.spell_effects_handler), Rng.roll_dice(7, 8) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ACID)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.LightningBolt
		128 + 17:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a lightning bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileElectricity.new(savegame.spell_effects_handler), Rng.roll_dice(4, 8) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ELECTRICITY)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.FireBolt
		128 + 18:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a fire bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileFire.new(savegame.spell_effects_handler), Rng.roll_dice(9, 8) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FIRE)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.ColdBolt
		128 + 19:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a frost bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileCold.new(savegame.spell_effects_handler), Rng.roll_dice(6, 8) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.COLD)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.PoisonBolt
		128 + 20:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a poison bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectilePoison.new(savegame.spell_effects_handler), Rng.roll_dice(6, 6) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.NetherBolt
		128 + 21:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a nether bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileNether.new(savegame.spell_effects_handler), 30 + Rng.roll_dice(5, 5) + (monster_level * 3 / 2))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.NETHER)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.WaterBolt
		128 + 22:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a water bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileWater.new(savegame.spell_effects_handler), Rng.roll_dice(10, 10) + monster_level)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.VisBolt
		128 + 23:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a vis bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectilePower.new(savegame.spell_effects_handler), Rng.roll_die(monster_level * 7 / 2) + 50)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.PlasmaBolt
		128 + 24:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a plasma bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectilePlasma.new(savegame.spell_effects_handler), 10 + Rng.roll_dice(8, 7) + monster_level)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.IceBolt
		128 + 25:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts an ice bolt." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileIce.new(savegame.spell_effects_handler), Rng.roll_dice(6, 6) + monster_level)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.COLD)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.MagicMissile
		128 + 26:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s casts a magic missile." % [monster_name])
			fire_bolt_at_player(monster_index, ProjectileMissile.new(savegame.spell_effects_handler), Rng.roll_dice(2, 6) + (monster_level / 3))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.REFLECTION)
		# MonsterFlags5.Scare
		128 + 27:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles, and you hear scary noises." % [monster_name] if player_is_blind else "%s casts a fearful illusion." % [monster_name])
			if player.has_fear_resistance:
				Profile.instance.message_print("You refuse to be frightened.")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You refuse to be frightened.")
			else:
				player.set_timed_fear(player.timed_fear + Rng.roll_die(4) + 3)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FEAR)
		# MonsterFlags5.Blindness
		128 + 28:
			savegame.disturb(true)
			Profile.instance.message_print(
				"%s mumbles." % [monster_name] if player_is_blind else "%s casts a spell, burning your eyes!" % [monster_name],
			)
			if player.has_blindness_resistance:
				Profile.instance.message_print("You are unaffected!")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.set_timed_blindness(11 + Rng.roll_die(4))
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.BLIND)
		# MonsterFlags5.Confuse
		128 + 29:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles, and you hear puzzling noises." % [monster_name] if player_is_blind else "%s creates a mesmerising illusion." % [monster_name])
			if player.has_confusion_resistance:
				Profile.instance.message_print("You disbelieve the feeble spell.")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You disbelieve the feeble spell.")
			else:
				player.set_timed_confusion(player.timed_confusion + Rng.roll_die(4) + 3)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.CONFUSION)
		# MonsterFlags5.Slow
		128 + 30:
			savegame.disturb(true)
			Profile.instance.message_print("%s drains power from your muscles!" % [monster_name])
			if player.has_free_action:
				Profile.instance.message_print("You are unaffected!")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.set_timed_slow(player.timed_slow + Rng.roll_die(4) + 3)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FREE_ACTION)
		# MonsterFlags5.Hold
		128 + 31:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s stares deep into your eyes!" % [monster_name])
			if player.has_free_action:
				Profile.instance.message_print("You are unaffected!")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(4) + 3)
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FREE_ACTION)
		# MonsterFlags6.Haste
		160 + 0:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s concentrates on {monster_possessive} body." % [monster_name])
			if monster.speed < monster_type.speed + 10:
				Profile.instance.message_print("%s starts moving faster." % [monster_name])
				monster.speed += 10
			elif monster.speed < monster_type.speed + 20:
				Profile.instance.message_print("%s starts moving faster." % [monster_name])
				monster.speed += 2
		# MonsterFlags6.DreadCurse
		160 + 1:
			savegame.disturb(true)
			Profile.instance.message_print("%s invokes the Dread Curse of Azathoth!" % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				var dummy: int = (65 + Rng.roll_die(25)) * player.health / 100
				Profile.instance.message_print("Your feel your life fade away!")
				player.take_hit(dummy, monster_name)
				player.curse_equipment(100, 20)
				if player.health < 1:
					player.health = 1
		# MonsterFlags6.Heal
		160 + 2:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s concentrates on {monster_possessive} wounds." % [monster_name])
			monster.health += monster_level * 6
			if monster.health >= monster.max_health:
				monster.health = monster.max_health
				Profile.instance.message_print("%s looks completely healed!" % [monster_name] if seen_by_player else "%s sounds completely healed!" % [monster_name])
			else:
				Profile.instance.message_print("%s looks healthier." % [monster_name] if seen_by_player else "%s sounds healthier." % [monster_name])
			if savegame.tracked_monster_index == monster_index:
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
			if monster.fear_level != 0:
				monster.fear_level = 0
				Profile.instance.message_print("%s recovers {monster_possessive} courage." % [monster_name])
		# MonsterFlags6.Xxx2
		160 + 3:
			return false
		# MonsterFlags6.Blink
		160 + 4:
			savegame.disturb(true)
			Profile.instance.message_print("%s blinks away." % [monster_name])
			savegame.spell_effects_handler.teleport_away(monster_index, 10)
		# MonsterFlags6.TeleportSelf
		160 + 5:
			savegame.disturb(true)
			Profile.instance.message_print("%s teleports away." % [monster_name])
			savegame.spell_effects_handlerteleport_away(monster_index, (DungeonLevel.MAX_SIGHT_RADIUS * 2) + 5)
		# MonsterFlags6.Xxx3
		160 + 6:
			return false
		# MonsterFlags6.Xxx4
		160 + 7:
			return false
		# MonsterFlags6.TeleportTo
		160 + 8:
			savegame.disturb(true)
			Profile.instance.message_print("%s commands you to return." % [monster_name])
			savegame.spell_effects_handlerteleport_player_to(monster.map_row, monster.map_col)
		# MonsterFlags6.teleport_away
		160 + 9:
			savegame.disturb(true)
			Profile.instance.message_print("%s teleports you away." % [monster_name])
			savegame.spell_effects_handlerteleport_player(100)
		# MonsterFlags6.TeleportLevel
		160 + 10:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles strangely." % [monster_name] if player_is_blind else "%s gestures at your feet." % [monster_name])
			if player.has_nexus_resistance:
				Profile.instance.message_print("You are unaffected!")
			elif Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				savegame.spell_effects_handlerteleport_player_level()
			dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.NEXUS)
		# MonsterFlags6.Xxx5
		160 + 11:
			return false
		# MonsterFlags6.Darkness
		160 + 12:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s gestures in shadow." % [monster_name])
			savegame.spell_effects_handlerunlight_area(0, 3)
		# MonsterFlags6.CreateTraps
		160 + 13:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles, and then cackles evilly." % [monster_name] if player_is_blind else "%s casts a spell and cackles evilly." % [monster_name])
			savegame.spell_effects_handlertrap_creation()
		# MonsterFlags6.Forget
		160 + 14:
			savegame.disturb(true)
			Profile.instance.message_print("%s tries to blank your mind." % [monster_name])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			elif (savegame.spell_effects_handlerlose_all_info()):
				Profile.instance.message_print("Your memories fade away.")
		# MonsterFlags6.Xxx6
		160 + 15:
			return false
		# MonsterFlags6.SummonKin
		160 + 16:
			savegame.disturb(true)
			if player_is_blind:
				Profile.instance.message_print("%s mumbles." % [monster_name])
			else:
				var kin: String = "minions" if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) else "kin"
				Profile.instance.message_print("%s magically summons %s %s." % [monster_name, monster_possessive, kin])
			dungeon_level.dungeon_monsters.summon_kin_type = monster_type.character
			for k: int in 6:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_KIN):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonReaver
		160 + 17:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons Black Reavers!" % [monster_name])
			if player_is_blind:
				Profile.instance.message_print("You hear heavy steps nearby.")
			savegame.spell_effects_handlersummon_reaver()
		# MonsterFlags6.SummonMonster
		160 + 18:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons help!" % [monster_name])
			for k: int in 1:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, 0):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear something appear nearby.")
		# MonsterFlags6.SummonMonsters
		160 + 19:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons monsters!" % [monster_name])
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, 0):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonAnt
		160 + 20:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons ants." % [monster_name])
			for k: int in 6:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_ANT):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonSpider
		160 + 21:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons spiders." % [monster_name])
			for k: int in 6:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_SPIDER):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonHound
		160 + 22:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons hounds." % [monster_name])
			for k: int in 6:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_HOUND):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonHydra
		160 + 23:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons hydras." % [monster_name])
			for k: int in 6:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_HYDRA):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many things appear nearby.")
		# MonsterFlags6.SummonCthuloid
		160 + 24:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons a Cthuloid entity!" % [monster_name])
			for k: int in 1:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_CTHULOID):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear something appear nearby.")
		# MonsterFlags6.SummonDemon
		160 + 25:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons a demon!" % [monster_name])
			for k: int in 1:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_DEMON):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear something appear nearby.")
		# MonsterFlags6.SummonUndead
		160 + 26:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons an undead adversary!" % [monster_name])
			for k: int in 1:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_UNDEAD):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear something appear nearby.")
		# MonsterFlags6.SummonDragon
		160 + 27:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons a dragon!" % [monster_name])
			for k: int in 1:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_DRAGON):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear something appear nearby.")
		# MonsterFlags6.SummonHiUndead
		160 + 28:
			savegame.disturb(true)
			Profile.instance.message_print(
				"%s mumbles." % [monster_name] if player_is_blind else "%s magically summons greater undead!" % [monster_name],
			)
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_HI_UNDEAD):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many creepy things appear nearby.")
		# MonsterFlags6.SummonHiDragon
		160 + 29:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons ancient dragons!" % [monster_name])
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_HI_DRAGON):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many powerful things appear nearby.")
		# MonsterFlags6.SummonGreatOldOne
		160 + 30:
			savegame.disturb(true)
			Profile.instance.message_print(
				"%s mumbles." % [monster_name] if player_is_blind else "%s magically summons Great Old Ones!" % [monster_name],
			)
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_GOO):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear immortal beings appear nearby.")
		# MonsterFlags6.SummonUnique
		160 + 31:
			savegame.disturb(true)
			Profile.instance.message_print("%s mumbles." % [monster_name] if player_is_blind else "%s magically summons special opponents!" % [monster_name])
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_UNIQUE):
					count += 1
			for k: int in 8:
				if dungeon_level.dungeon_monsters.summon_specific(row_player, col_player, monster_level, SummonType.SUMMON_HI_UNDEAD):
					count += 1
			if player_is_blind and count != 0:
				Profile.instance.message_print("You hear many powerful things appear nearby.")
	# If the player saw us cast the spell, let them learn we can do that
	if seen_by_player:
		if thrown_spell < 32 * 4:
			monster_type.knowledge.r_flags_4.set_bits(1 << (thrown_spell - (32 * 3)))
			if monster_type.knowledge.r_cast_innate < 255:
				monster_type.knowledge.r_cast_innate += 1
		elif thrown_spell < 32 * 5:
			monster_type.knowledge.r_flags_5.set_bits(1 << (thrown_spell - (32 * 4)))
			if monster_type.knowledge.r_cast_spell < 255:
				monster_type.knowledge.r_cast_spell += 1
		elif thrown_spell < 32 * 6:
			monster_type.knowledge.r_flags_6.set_bits(1 << (thrown_spell - (32 * 5)))
			if monster_type.knowledge.r_cast_spell < 255:
				monster_type.knowledge.r_cast_spell += 1
	# If we killed the player, let their descendants remember that
	if player.is_dead and monster_type.knowledge.r_deaths < 65535:
		monster_type.knowledge.r_deaths += 1
	# We did cast a spell
	return true


func get_moves_toward_enemy_monster(monster: Monster, moves_list: Array[int]) -> void:
	const SPIRAL_GRID_OFFSETS: Array = [
		[-1, 0],
		[0, -1],
		[1, 0],
		[0, +1],
		[-1, -1],
		[1, -1],
		[1, 1],
		[-1, 1],
		[-2, 0],
		[0, -2],
		[2, 0],
		[0, 2],
		[-2, -1],
		[1, -2],
		[2, 1],
		[-1, 2],
		[-1, -2],
		[2, -1],
		[1, 2],
		[-2, 1],
		[-2, -2],
		[2, -2],
		[2, 2],
		[-2, 2],
		[-3, 0],
		[0, -3],
		[3, 0],
		[0, 3],
		[-3, -1],
		[1, -3],
		[3, 1],
		[-1, 3],
		[-3, 1],
		[-1, -3],
		[3, -1],
		[1, 3],
		[-3, -2],
		[2, -3],
		[3, 2],
		[-2, 3],
		[-3, 2],
		[-2, -3],
		[3, -2],
		[2, 3],
		[-3, -3],
		[3, -3],
		[3, 3],
		[-3, 3],
		[-4, 0],
		[0, -4],
		[4, 0],
		[0, 4],
		[1, -4],
		[4, 1],
		[-1, 4],
		[-4, -1],
		[-4, 1],
		[-1, -4],
		[4, -1],
		[1, 4],
		[-4, -2],
		[2, -4],
		[4, 2],
		[-2, 4],
		[-4, 2],
		[-2, -4],
		[4, -2],
		[2, 4],
		[-4, -3],
		[3, -4],
		[4, 3],
		[-3, 4],
		[-4, 3],
		[-3, -4],
		[4, -3],
		[3, 4],
		[-4, -4],
		[4, -4],
		[4, 4],
		[-4, 4],
	]
	# Go through the possible places an enemy could be
	for pair: Array in SPIRAL_GRID_OFFSETS:
		# Get the location of the place (these are ordered so they start close and spiral out)
		var row: int = monster.map_row + pair[0]
		var col: int = monster.map_col + pair[1]
		# Check if we're in bounds and have a monster
		if dungeon_level.in_inner_bounds(row, col):
			if dungeon_level.get_grid(row, col).monster_index != 0:
				var enemy: Monster = dungeon_level.dungeon_monsters.monsters[dungeon_level.get_grid(row, col).monster_index]
				# Only go for monsters who are awake and on the opposing side
				if enemy.mind.is_set(MonsterMind.FRIENDLY) != monster.mind.is_set(MonsterMind.FRIENDLY) and enemy.sleep_level == 0:
					# Add moves directly towards and either side of the enemy based on its
					# relative location
					col -= monster.map_col
					row -= monster.map_row
					if row < 0 and col == 0:
						moves_list[0] = 8
						moves_list[1] = 7
						moves_list[2] = 9
					elif row > 0 and col == 0:
						moves_list[0] = 2
						moves_list[1] = 1
						moves_list[2] = 3
					elif col > 0 and row == 0:
						moves_list[0] = 6
						moves_list[1] = 9
						moves_list[2] = 3
					elif col < 0 and row == 0:
						moves_list[0] = 4
						moves_list[1] = 7
						moves_list[2] = 1
					if row < 0 and col < 0:
						moves_list[0] = 7
						moves_list[1] = 4
						moves_list[2] = 8
					elif row < 0 and col > 0:
						moves_list[0] = 9
						moves_list[1] = 6
						moves_list[2] = 8
					elif row > 0 and col < 0:
						moves_list[0] = 1
						moves_list[1] = 4
						moves_list[2] = 2
					elif row > 0 and col > 0:
						moves_list[0] = 3
						moves_list[1] = 6
						moves_list[2] = 2
					break


func get_moves_toward_player(monster_index: int, moves_list: Array[int]) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var move_value: int = 0
	var done: bool = false
	# Default to moving towards the player's exact location
	var target_location: Location = Location.new()
	target_location.row = player.map_row
	target_location.col = player.map_col
	# Adjust our target based on the player's scent if we can't move in a straight line to them
	track_player_by_scent(monster_index, target_location)
	# Get the relative move needed to reach our target location
	var desired_relative_movement: Location = Location.new()
	desired_relative_movement.row = monster.map_row - target_location.row
	desired_relative_movement.col = monster.map_col - target_location.col
	if monster.mind.is_clear(MonsterMind.FRIENDLY):
		# If we're a pack animal that can't go through walls
		if (monster_type.flags_1.is_set(MonsterFlags1.FRIENDS) and monster_type.flags_3.is_set(MonsterFlags3.ANIMAL) and
			monster_type.flags_2.is_clear(MonsterFlags2.PASSWALL) and monster_type.flags_2.is_clear(MonsterFlags2.KILL_WALL) ):
			var room: int = 0
			# Check if the player is in a room by counting the room tiles around them
			for i: int in 8:
				if dungeon_level.get_grid(player.map_row + dungeon_level.ordered_direction_row_offset[i], player.map_col + dungeon_level.ordered_direction_col_offset[i]).tile_flags.is_set(GridTile.IN_ROOM):
					room += 1
			# If the player isn't in a room and they're healthy, wait to ambush them rather
			# than running headlong into the corridor after them and queueing up to get hit
			if room < 8 and player.health > player.max_health * 3 / 4:
				if find_ambush_spot(monster_index, desired_relative_movement):
					done = true
		# If we're not done and we have friends make our movement slightly depend on our
		# index so we spread out and don't block each other
		if not done and monster_type.flags_1.is_set(MonsterFlags1.FRIENDS):
			for i: int in 8:
				target_location.row = player.map_row + dungeon_level.ordered_direction_row_offset[(monster_index + i) & 7]
				target_location.col = player.map_col + dungeon_level.ordered_direction_col_offset[(monster_index + i) & 7]
				# We might have got a '5' meaning stay where we are, so replace that with
				# moving towards the player
				if monster.map_row == target_location.row and monster.map_col == target_location.col:
					target_location.row = player.map_row
					target_location.col = player.map_col
					break
				# Repeat till we get a direction we can move in
				if not dungeon_level.grid_passable_no_creature(target_location.row, target_location.col):
					continue
				break
			desired_relative_movement.row = monster.map_row - target_location.row
			desired_relative_movement.col = monster.map_col - target_location.col
			done = true
	# If we're an ally then check if we should retreat
	if monster.mind.is_set(MonsterMind.FRIENDLY):
		if monster_should_retreat(monster_index):
			# If we should be scared, simply move the opposite way to the player
			desired_relative_movement.row = -desired_relative_movement.row
			desired_relative_movement.col = -desired_relative_movement.col
	else:
		# If we're not an ally then check if we should retreat
		if not done and monster_should_retreat(monster_index):
			# If we should retreat, then try to find a safe spot where the player can't
			# shoot or see us
			if not find_safe_spot(monster_index, desired_relative_movement):
				# If we failed to find one, just back off
				desired_relative_movement.row = -desired_relative_movement.row
				desired_relative_movement.col = -desired_relative_movement.col
			else:
				# We found a safe spot, so head there, but prioritise avoiding the player's scent
				avoid_player_scent(monster_index, desired_relative_movement)
	# If our best move is to stand still, don't move
	if desired_relative_movement.col == 0 and desired_relative_movement.row == 0:
		return false
	# Convert the target location to an actual direction
	var abs_col: int = absi(desired_relative_movement.col)
	var abs_row: int = absi(desired_relative_movement.row)
	if desired_relative_movement.row < 0:
		move_value += 8
	if desired_relative_movement.col > 0:
		move_value += 4
	if abs_row > abs_col << 1:
		move_value += 1
		move_value += 1
	elif abs_col > abs_row << 1:
		move_value += 1
	# Add some potential moves to the list based on the direction we've decided to go in
	match move_value:
		0:
			moves_list[0] = 9
			if abs_row > abs_col:
				moves_list[1] = 8
				moves_list[2] = 6
				moves_list[3] = 7
				moves_list[4] = 3
			else:
				moves_list[1] = 6
				moves_list[2] = 8
				moves_list[3] = 3
				moves_list[4] = 7
		1, 9:
			moves_list[0] = 6
			if desired_relative_movement.row < 0:
				moves_list[1] = 3
				moves_list[2] = 9
				moves_list[3] = 2
				moves_list[4] = 8
			else:
				moves_list[1] = 9
				moves_list[2] = 3
				moves_list[3] = 8
				moves_list[4] = 2
		2, 6:
			moves_list[0] = 8
			if desired_relative_movement.col < 0:
				moves_list[1] = 9
				moves_list[2] = 7
				moves_list[3] = 6
				moves_list[4] = 4
			else:
				moves_list[1] = 7
				moves_list[2] = 9
				moves_list[3] = 4
				moves_list[4] = 6
		4:
			moves_list[0] = 7
			if abs_row > abs_col:
				moves_list[1] = 8
				moves_list[2] = 4
				moves_list[3] = 9
				moves_list[4] = 1
			else:
				moves_list[1] = 4
				moves_list[2] = 8
				moves_list[3] = 1
				moves_list[4] = 9
		5, 13:
			moves_list[0] = 4
			if desired_relative_movement.row < 0:
				moves_list[1] = 1
				moves_list[2] = 7
				moves_list[3] = 2
				moves_list[4] = 8
			else:
				moves_list[1] = 7
				moves_list[2] = 1
				moves_list[3] = 8
				moves_list[4] = 2
		8:
			moves_list[0] = 3
			if abs_row > abs_col:
				moves_list[1] = 2
				moves_list[2] = 6
				moves_list[3] = 1
				moves_list[4] = 9
			else:
				moves_list[1] = 6
				moves_list[2] = 2
				moves_list[3] = 9
				moves_list[4] = 1
		10, 14:
			moves_list[0] = 2
			if desired_relative_movement.col < 0:
				moves_list[1] = 3
				moves_list[2] = 1
				moves_list[3] = 6
				moves_list[4] = 4
			else:
				moves_list[1] = 1
				moves_list[2] = 3
				moves_list[3] = 4
				moves_list[4] = 6
		12, 15:
			moves_list[0] = 1
			if abs_row > abs_col:
				moves_list[1] = 2
				moves_list[2] = 4
				moves_list[3] = 3
				moves_list[4] = 7
			else:
				moves_list[1] = 4
				moves_list[2] = 2
				moves_list[3] = 7
				moves_list[4] = 3
	return true


func track_player_by_scent(monster_index: int, target: Location) -> void:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# If we can move through walls then we don't need to adjust anything
	if monster_type.flags_2.is_set(MonsterFlags2.PASSWALL):
		return
	if monster_type.flags_2.is_set(MonsterFlags2.KILL_WALL):
		return
	var row_1: int = monster.map_row
	var col_1: int = monster.map_col
	var tile: GridTile = dungeon_level.get_grid(row_1, col_1)
	# If we have no scent of the player then don't change where we were going
	if tile.scent_age < dungeon_level.get_grid(player.map_row, player.map_col).scent_age:
		if tile.scent_age == 0:
			return
	if tile.scent_strength > dungeon_level.SCENT_LIMIT:
		return
	if tile.scent_strength > monster_type.notice_range:
		return
	# If we can actually see the player then don't change where we are going
	if dungeon_level.player_has_los_bold(row_1, col_1):
		return
	var when_scented: int = 0
	var cost: int = 999
	# Check the eight directions we can move to see which has the most recent or strongest scent
	for i: int in range(7, -1, -1):
		var row: int = row_1 + dungeon_level.ordered_direction_row_offset[i]
		var col: int = col_1 + dungeon_level.ordered_direction_col_offset[i]
		if dungeon_level.get_grid(row, col).scent_age == 0:
			continue
		if dungeon_level.get_grid(row, col).scent_age < when_scented:
			continue
		if dungeon_level.get_grid(row, col).scent_strength > cost:
			continue
		when_scented = dungeon_level.get_grid(row, col).scent_age
		cost = dungeon_level.get_grid(row, col).scent_strength
		# Give us a target in the general direction of the strongest scent
		target.row = player.map_row + (16 * dungeon_level.ordered_direction_row_offset[i])
		target.col = player.map_col + (16 * dungeon_level.ordered_direction_col_offset[i])


func attack_another_monster(monster_index: int, target_index: int) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var target: Monster = dungeon_level.dungeon_monsters.monsters[target_index]
	var monster_type: MonsterType = monster.monster_type
	var target_monster_type: MonsterType = target.monster_type
	var touched: bool = false
	var row_saved: int = target.map_row
	var col_saved: int = target.map_col
	# If we never attack then we shouldn't this time
	if monster_type.flags_1.is_set(MonsterFlags1.NEVER_ATTACK):
		return false
	var armour_class: int = target_monster_type.ac
	var monster_level: int = monster_type.level if monster_type.level >= 1 else 1
	var monster_name: String = monster.describe(0)
	var target_name: String = target.describe(0)
	var blinked: bool = false
	# If the player can't see either monster, they just hear noise
	if not (monster.is_visible or target.is_visible):
		Profile.instance.message_print("You hear noise.")
	# We have up to four attacks
	for attack_number: int in 4:
		var visible: int = false
		var obvious: int = false
		var power: int = 0
		var damage: int = 0
		var act: String = ""
		var effect: int = monster_type.attack[attack_number].effect
		var method: int = monster_type.attack[attack_number].method
		var damage_dice: int = monster_type.attack[attack_number].damage_dice
		var damage_sides: int = monster_type.attack[attack_number].damage_dice_sides
		# Re-select the target in case the original has died and been replaced by an empty monster slot
		target = dungeon_level.dungeon_monsters.monsters[target_index]
		# Can't attack ourselves
		if target == monster:
			break
		# If the target has moved, abort
		if target.map_col != col_saved or target.map_row != row_saved:
			break
		# If we don't have an attack in this attack slot, abort
		if method == 0:
			break
		# If we blinked away after stealing on a previous attack, abort
		if blinked:
			break
		if monster.is_visible:
			visible = true
		# Get the power of the attack based on the attack type
		match effect:
			AttackEffect.HURT:
				power = 60
			AttackEffect.POISON:
				power = 5
			AttackEffect.UN_BONUS:
				power = 20
			AttackEffect.UN_POWER:
				power = 15
			AttackEffect.EAT_GOLD:
				power = 5
			AttackEffect.EAT_ITEM:
				power = 5
			AttackEffect.EAT_FOOD:
				power = 5
			AttackEffect.EAT_LIGHT:
				power = 5
			AttackEffect.ACID:
				power = 0
			AttackEffect.ELECTRICITY:
				power = 10
			AttackEffect.FIRE:
				power = 10
			AttackEffect.COLD:
				power = 10
			AttackEffect.BLIND:
				power = 2
			AttackEffect.CONFUSE:
				power = 10
			AttackEffect.TERRIFY:
				power = 10
			AttackEffect.PARALYZE:
				power = 2
			AttackEffect.LOSE_STR:
				power = 0
			AttackEffect.LOSE_DEX:
				power = 0
			AttackEffect.LOSE_CON:
				power = 0
			AttackEffect.LOSE_INT:
				power = 0
			AttackEffect.LOSE_WIS:
				power = 0
			AttackEffect.LOSE_CHA:
				power = 0
			AttackEffect.LOSE_ALL:
				power = 2
			AttackEffect.SHATTER:
				power = 60
			AttackEffect.EXP_10:
				power = 5
			AttackEffect.EXP_20:
				power = 5
			AttackEffect.EXP_40:
				power = 5
			AttackEffect.EXP_80:
				power = 5
		# If we hit the monster, describe the type of hit
		if effect == 0 or check_monster_hit_versus_another_monster(power, monster_level, armour_class):
			savegame.disturb(true)
			match method:
				AttackType.HIT:
					act = "hits %s."
					touched = true
				AttackType.TOUCH:
					act = "touches %s."
					touched = true
				AttackType.PUNCH:
					act = "punches %s."
					touched = true
				AttackType.KICK:
					act = "kicks %s."
					touched = true
				AttackType.CLAW:
					act = "claws %s."
					touched = true
				AttackType.BITE:
					act = "bites %s."
					touched = true
				AttackType.STING:
					act = "stings %s."
					touched = true
				AttackType.BUTT:
					act = "butts %s."
					touched = true
				AttackType.CRUSH:
					act = "crushes %s."
					touched = true
				AttackType.ENGULF:
					act = "engulfs %s."
					touched = true
				AttackType.CHARGE:
					act = "charges %s."
					touched = true
				AttackType.CRAWL:
					act = "crawls on %s."
					touched = true
				AttackType.DROOL:
					act = "drools on %s."
					touched = false
				AttackType.SPIT:
					act = "spits on %s."
					touched = false
				AttackType.GAZE:
					act = "gazes at %s."
					touched = false
				AttackType.WAIL:
					act = "wails at %s."
					touched = false
				AttackType.SPORE:
					act = "releases spores at %s."
					touched = false
				AttackType.HERO_WORSHIP:
					act = "hero worships %s."
					touched = false
				AttackType.BEG:
					act = "begs %s for money."
					touched = false
					target.sleep_level = 0
				AttackType.INSULT:
					act = "insults %s."
					touched = false
					target.sleep_level = 0
				AttackType.MOAN:
					act = "moans at %s."
					touched = false
					target.sleep_level = 0
				AttackType.SHOW:
					act = "sings to %s."
					touched = false
					target.sleep_level = 0
			# Display the attack description
			if act != "":
				var temp: String = act % target_name
				if monster.is_visible or target.is_visible:
					Profile.instance.message_print("%s %s." % [monster_name, temp])
			obvious = true
			damage = Rng.roll_dice(damage_dice, damage_sides)
			# Default to a missile attack
			var projectile: Projectile = ProjectileMissile.new(savegame.spell_effects_handler)
			# Choose the correct type of attack to display, as well as any other special
			# effects for the attack
			match effect:
				0:
					damage = 0
					projectile = null
				AttackEffect.HURT:
					damage -= damage * (armour_class if armour_class < 150 else 150) / 250
				AttackEffect.POISON:
					projectile = ProjectilePoison.new(savegame.spell_effects_handler)
				AttackEffect.UN_BONUS, \
				AttackEffect.UN_POWER:
					projectile = ProjectileDisenchantment.new(savegame.spell_effects_handler)
				AttackEffect.EAT_FOOD, \
				AttackEffect.EAT_LIGHT:
					projectile = null
					damage = 0
				AttackEffect.EAT_ITEM, \
				AttackEffect.EAT_GOLD:
					# dungeon_monsters.monsters don't actually steal from other monsters
					projectile = null
					damage = 0
					if Rng.roll_die(2) == 1:
						blinked = true
				AttackEffect.ACID:
					projectile = ProjectileAcid.new(savegame.spell_effects_handler)
				AttackEffect.ELECTRICITY:
					projectile = ProjectileElectricity.new(savegame.spell_effects_handler)
				AttackEffect.FIRE:
					projectile = ProjectileFire.new(savegame.spell_effects_handler)
				AttackEffect.COLD:
					projectile = ProjectileCold.new(savegame.spell_effects_handler)
				AttackEffect.BLIND:
					pass
				AttackEffect.CONFUSE:
					projectile = ProjectileConfusion.new(savegame.spell_effects_handler)
				AttackEffect.TERRIFY:
					projectile = ProjectileTurnAll.new(savegame.spell_effects_handler)
				AttackEffect.PARALYZE:
					projectile = ProjectileOldSleep.new(savegame.spell_effects_handler)
					damage = monster_type.level
				AttackEffect.LOSE_STR, \
				AttackEffect.LOSE_INT, \
				AttackEffect.LOSE_WIS, \
				AttackEffect.LOSE_DEX, \
				AttackEffect.LOSE_CON, \
				AttackEffect.LOSE_CHA, \
				AttackEffect.LOSE_ALL:
					pass
				AttackEffect.SHATTER:
					if damage > 23:
						savegame.spell_effects_handler.earthquake(monster.map_row, monster.map_col, 8)
				AttackEffect.EXP_10, \
				AttackEffect.EXP_20, \
				AttackEffect.EXP_40, \
				AttackEffect.EXP_80:
					projectile = ProjectileNether.new(savegame.spell_effects_handler)
				_:
					projectile = null
			# Implement the attack as a projectile
			if projectile != null:
				savegame.spell_effects_handler.project(monster_index, 0, target.map_row, target.map_col, damage, projectile, ProjectionFlag.KILL | ProjectionFlag.STOP)
				# If we touched the target we might get burned or zapped
				if touched:
					if target_monster_type.flags_2.is_set(MonsterFlags2.FIRE_AURA) and monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_FIRE):
						if monster.is_visible or target.is_visible:
							# Auras prevent us blinking away
							blinked = false
							# The player may see and learn that the target has the aura
							Profile.instance.message_print("%s is suddenly very hot!" % [monster_name])
							if target.is_visible:
								target_monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.FIRE_AURA)
						savegame.spell_effects_handler.Project(target_index, 0, monster.map_row, monster.map_col, Rng.roll_dice(1 + (target_monster_type.level / 26), 1 + (target_monster_type.level / 17)), ProjectileFire.new(savegame.spell_effects_handler), ProjectionFlag.KILL | ProjectionFlag.STOP)
					if target_monster_type.flags_2.is_set(MonsterFlags2.LIGHTNING_AURA) and monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_LIGHTNING):
						if monster.is_visible or target.is_visible:
							# Auras prevent us blinking away
							blinked = false
							# The player may see and learn that the target has the aura
							Profile.instance.message_print("%s gets zapped!" % [monster_name])
							if target.is_visible:
								target_monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.LIGHTNING_AURA)
						savegame.spell_effects_handler.project(target_index, 0, monster.map_row, monster.map_col, Rng.roll_dice(1 + (target_monster_type.level / 26), 1 + (target_monster_type.level / 17)), ProjectileElectricity.new(savegame.spell_effects_handler), ProjectionFlag.KILL | ProjectionFlag.STOP)
		else:
			# We didn't hit, so just let the player know that if we are visible
			match method:
				AttackType.HIT, \
				AttackType.TOUCH, \
				AttackType.PUNCH, \
				AttackType.KICK, \
				AttackType.CLAW, \
				AttackType.BITE, \
				AttackType.STING, \
				AttackType.BUTT, \
				AttackType.CRUSH, \
				AttackType.ENGULF, \
				AttackType.CHARGE:
					if monster.is_visible:
						savegame.disturb(false)
						Profile.instance.message_print("%s misses %s." % [monster_name, target_name])
		# If the player saw what happened, they know more about what attacks we have
		if visible:
			if obvious or damage != 0 or monster_type.knowledge.r_blows[attack_number] > 10:
				if monster_type.knowledge.r_blows[attack_number] < Constants.MAX_INT:
					monster_type.knowledge.r_blows[attack_number] += 1
	# If we stole and should therefore blink away, then do so.
	if blinked:
		Profile.instance.message_print("The thief flees laughing!" if monster.is_visible else "You hear laughter!")
		savegame.spell_effects_handler.teleport_away(monster_index, (DungeonLevel.MAX_SIGHT_RADIUS * 2) + 5)
	# We made the attack
	return true


func find_ambush_spot(monster_index: int, relative_target: Location) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var f_row: int = monster.map_row
	var f_col: int = monster.map_col
	var row_hide: int = 0
	var col_hide: int = 0
	var shortest_distance: int = 999
	var too_close_to_player: int = (dungeon_level.distance(player.map_row, player.map_col, f_row, f_col) * 3 / 4) + 2
	# Start with a short search radius and slowly increase
	for d: int in range(1, 10):
		for row: int in range(f_row - d, f_row + d + 1):
			for col: int in range(f_col - d, f_row + d + 1):
				# Make sure the spot is valid
				if not dungeon_level.in_inner_bounds(row, col):
					continue
				if not dungeon_level.grid_passable(row, col):
					continue
				# Make sure the spot is the right distance
				if dungeon_level.distance(row, col, f_row, f_col) != d:
					continue
				# Make sure the spot is actually hidden
				if not dungeon_level.player_can_see_bold(row, col) and clean_shot(f_row, f_col, row, col):
					# If the spot is closer to the player than any previously found spot
					# (but not too close), remember it
					var dist: int = dungeon_level.distance(row, col, player.map_row, player.map_col)
					if dist < shortest_distance and dist >= too_close_to_player:
						row_hide = row
						col_hide = col
						shortest_distance = dist
		# If we found at least one spot, return true with the coordinates
		if shortest_distance < 999:
			relative_target.row = f_row - row_hide
			relative_target.col = f_col - col_hide
			return true
	# We didn't find a spot
	return false


func find_safe_spot(monster_index: int, relative_target: Location) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var f_row: int = monster.map_row
	var f_col: int = monster.map_col
	var row_safe: int = 0
	var col_safe: int = 0
	var longest_distance: int = 0
	# Start with a short search radius and slowly increase
	for d: int in 10:
		for row: int in range(f_row - d, f_row + d + 1):
			for col: int in range(f_col - d, f_row + d + 1):
				# Make sure the spot is valid
				if not dungeon_level.in_inner_bounds(row, col):
					continue
				if not dungeon_level.grid_passable(row, col):
					continue
				# Make sure the spot is the right distance
				if dungeon_level.distance(row, col, f_row, f_col) != d:
					continue
				# Reject spots that smell too strongly of the player
				if dungeon_level.get_grid(row, col).scent_age < dungeon_level.get_grid(player.map_row, player.map_col).scent_age:
					continue
				if dungeon_level.get_grid(row, col).scent_strength > dungeon_level.get_grid(f_row, f_col).scent_strength + (2 * d):
					continue
				# Make sure the spot is actually hidden
				if not dungeon_level.projectable(row, col, player.map_row, player.map_col):
					# If the spot is further from the player than any previously found
					# spot, remember it
					var dist: int = dungeon_level.distance(row, col, player.map_row, player.map_col)
					if dist > longest_distance:
						row_safe = row
						col_safe = col
						longest_distance = dist
		# If we found a spot then save its coordinates and return true
		if longest_distance > 0:
			relative_target.row = f_row - row_safe
			relative_target.col = f_col - col_safe
			return true
	# We found nowhere suitable
	return false


func monster_should_retreat(monster_index: int) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# Don't move away if we're already too far away to see the player
	if monster.distance_from_player > DungeonLevel.MAX_SIGHT_RADIUS + 5:
		return false
	# If we're the player's friend then don't move away from them
	if monster.mind.is_set(MonsterMind.FRIENDLY):
		return false
	# If we're scared then definitely move away
	if monster.fear_level != 0:
		return true
	# If we're very close, stay close for potential melee
	if monster.distance_from_player <= 5:
		return false
	var player_level: int = player.level
	var monster_level: int = monster_type.level + (monster_index & 0x08) + 25
	# If we're tougher than the player, don't move away
	if monster_level > player_level + 4:
		return false
	# If we're significantly weaker than the player, move away
	if monster_level + 4 <= player_level:
		return true
	# If we're significantly less healthy than the player, move away
	var player_health: int = player.health
	var player_max_health: int = player.max_health
	var monster_health: int = monster.health
	var monster_max_health: int = monster.max_health
	var player_health_factor: int = (player_level * player_max_health) + (player_health << 2)
	var monster_health_factor: int = (monster_level * monster_max_health) + (monster_health << 2)
	return player_health_factor * monster_max_health > monster_health_factor * player_max_health


func avoid_player_scent(monster_index: int, location: Location) -> void:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var row_monster: int = monster.map_row
	var col_monster: int = monster.map_col
	var row_d: int = row_monster - location.row
	var col_d: int = col_monster - location.col
	# If the scent too strong, keep going where we were going
	if dungeon_level.get_grid(row_monster, col_monster).scent_age < dungeon_level.get_grid(player.map_row, player.map_col).scent_age:
		return
	if dungeon_level.get_grid(row_monster, col_monster).scent_strength > dungeon_level.SCENT_LIMIT:
		return
	if dungeon_level.get_grid(row_monster, col_monster).scent_strength > monster_type.notice_range:
		return
	var row_g: int = 0
	var col_g: int = 0
	var scent_when: int = 0
	var score: int = -1
	# Check each of the eight directions
	for i: int in range(7, -1, -1):
		var row: int = row_monster + dungeon_level.ordered_direction_row_offset[i]
		var col: int = col_monster + dungeon_level.ordered_direction_col_offset[i]
		# If we have no scent there, or the scent is too recent, ignore it
		if dungeon_level.get_grid(row, col).scent_age == 0:
			continue
		if dungeon_level.get_grid(row, col).scent_age < scent_when:
			continue
		# If the scent is weaker than in the other directions, go that way
		var dist: int = dungeon_level.distance(row, col, row_d, col_d)
		var s: int = (5000 / (dist + 3)) - (500 / (dungeon_level.get_grid(row, col).scent_strength + 1))
		if s < 0:
			s = 0
		if s < score:
			continue
		scent_when = dungeon_level.get_grid(row, col).scent_age
		score = s
		row_g = row
		col_g = col
	# If we didn't find any scent at all, keep going the way we were going
	if scent_when == 0:
		return
	# Go towards the weakest scent
	location.row = row_monster - row_g
	location.col = col_monster - col_g


func clean_shot(row_start: int, col_start: int, row_end: int, col_end: int) -> bool:
	var row: int = row_start
	var col: int = col_start
	# Loop from the start to the maximumm range allowed
	for distance: int in DungeonLevel.MAX_SPELL_RANGE:
		# If our location is blocked, give up
		if distance != 0 and !dungeon_level.grid_passable(row, col):
			break
		# If there's another monster in the way and it is friendly, give up
		if distance != 0 and dungeon_level.get_grid(row, col).monster_index > 0:
			if dungeon_level.dungeon_monsters.monsters[dungeon_level.get_grid(row, col).monster_index].mind.is_clear(MonsterMind.FRIENDLY):
				break
		# If we've reached the target then we have a clean shot
		if col == col_end and row == row_end:
			return true
		# Move closer
		var location: Location = dungeon_level.move_one_step_towards(row, col, row_start, col_start, row_end, col_end)
		row = location.row
		col = location.col
	# If we gave up or ran out of distance we don't have a clean shot
	return false


func check_monster_hit_versus_another_monster(power: int, level: int, armour_class: int) -> bool:
	# Base 5% chance to hit and 5% chance to miss
	var k: int = Rng.roll_die(100)
	if k <= 10:
		return k <= 5
	# If we didn't auto hit or miss, use the standard formula for attacking
	var i: int = power + (level * 3)
	return i > 0 and Rng.roll_die(i) > armour_class * 3 / 4


func remove_ineffective_spells(monster_index: int, f_4: FlagSet, f_5: FlagSet, f_6: FlagSet) -> void:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# If we're stupid, we won't realise how ineffective things are
	if monster_type.flags_2.is_set(MonsterFlags2.STUPID):
		return
	# Tiny chance of forgetting what we've seen, clearing all smart flags except for ally
	# and clone
	if monster.mind.is_any_set() and Rng.roll_die(100) == 1:
		monster.mind.keep_bits(MonsterMind.FRIENDLY | MonsterMind.CLONED)
	var mind_flags: FlagSet = monster.mind
	# If we're not aware of any of the player's resistances, don't bother going through them
	if mind_flags.is_all_clear():
		return
	# If we know the player is immune to acid, don't do acid spells
	if mind_flags.is_set(MonsterMind.SEEN_ACID_IMMUNITY):
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.BREATHE_ACID)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.ACID_BALL)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.ACID_BOLT)
	# If we know the player resists acid both temporarily and permanently, probably don't
	# do acid spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_ACID_RES) and mind_flags.is_set(MonsterMind.SEEN_ACID_RES):
		if realise_spell_is_useless(monster_type, 80):
			f_4.clear_bits(MonsterFlags4.BREATHE_ACID)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.ACID_BALL)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.ACID_BOLT)
	# If we know the player resists acid at all, maybe don't do acid spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_ACID_RES) or mind_flags.is_set(MonsterMind.SEEN_ACID_RES):
		if realise_spell_is_useless(monster_type, 30):
			f_4.clear_bits(MonsterFlags4.BREATHE_ACID)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.ACID_BALL)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.ACID_BOLT)
	# If we know the player is immune to lightning, don't do lightning spells
	if mind_flags.is_set(MonsterMind.SEEN_ELECTRIC_IMMUNITY):
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.BREATHE_LIGHTNING)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BALL)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BOLT)
	# If we know the player resists lightning both temporarily and permanently, probably
	# don't do lightning spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_ELECTRIC_RES) and mind_flags.is_set(MonsterMind.SEEN_ELECTRIC_RES):
		if realise_spell_is_useless(monster_type, 80):
			f_4.clear_bits(MonsterFlags4.BREATHE_LIGHTNING)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BALL)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BOLT)
	# If we know the player resists lightning at all, maybe don't do lightning spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_ELECTRIC_RES) or mind_flags.is_set(MonsterMind.SEEN_ELECTRIC_RES):
		if realise_spell_is_useless(monster_type, 30):
			f_4.clear_bits(MonsterFlags4.BREATHE_LIGHTNING)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BALL)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BOLT)
	# If we know the player is immune to fire, don't do fire spells
	if mind_flags.is_set(MonsterMind.SEEN_FIRE_IMMUNITY):
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.BREATHE_FIRE)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.FIRE_BALL)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.FIRE_BOLT)
	# If we know the player resists fire both temporarily and permanently, probably don't
	# do fire spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_FIRE_RES) and mind_flags.is_set(MonsterMind.SEEN_FIRE_RES):
		if realise_spell_is_useless(monster_type, 80):
			f_4.clear_bits(MonsterFlags4.BREATHE_FIRE)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.FIRE_BALL)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.FIRE_BOLT)
	# If we know the player resists fire at all, maybe don't do fire spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_FIRE_RES) or mind_flags.is_set(MonsterMind.SEEN_FIRE_RES):
		if realise_spell_is_useless(monster_type, 30):
			f_4.clear_bits(MonsterFlags4.BREATHE_FIRE)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.FIRE_BALL)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.FIRE_BOLT)
	# If we know the player is immune to cold, don't do fire spells
	if mind_flags.is_set(MonsterMind.SEEN_COLD_IMMUNITY):
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.BREATHE_COLD)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.COLD_BALL)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.COLD_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.ICE_BOLT)
	# If we know the player resists cold both temporarily and permanently, probably don't
	# do cold spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_COLD_RES) and mind_flags.is_set(MonsterMind.SEEN_COLD_RES):
		if realise_spell_is_useless(monster_type, 80):
			f_4.clear_bits(MonsterFlags4.BREATHE_COLD)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.COLD_BALL)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.COLD_BOLT)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.ICE_BOLT)
	# If we know the player resists cold at all, maybe don't do cold spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_COLD_RES) or mind_flags.is_set(MonsterMind.SEEN_COLD_RES):
		if realise_spell_is_useless(monster_type, 30):
			f_4.clear_bits(MonsterFlags4.BREATHE_COLD)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.COLD_BALL)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.COLD_BOLT)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.ICE_BOLT)
	# If we know the player resists poison both temporarily and permanently, probably don't
	# do poison spells
	if mind_flags.is_set(MonsterMind.SEEN_TEMP_POISON_RES) and mind_flags.is_set(MonsterMind.SEEN_POISON_RES):
		if realise_spell_is_useless(monster_type, 80):
			f_4.clear_bits(MonsterFlags4.BREATHE_POISON)
		if realise_spell_is_useless(monster_type, 80):
			f_5.clear_bits(MonsterFlags5.POISON_BALL)
		if realise_spell_is_useless(monster_type, 40):
			f_4.clear_bits(MonsterFlags4.RADIATION_BALL)
		if realise_spell_is_useless(monster_type, 40):
			f_4.clear_bits(MonsterFlags4.BREATHE_RADIATION)
	# If we know the player resists poison at all, maybe don't do cold spells
	elif mind_flags.is_set(MonsterMind.SEEN_TEMP_POISON_RES) or mind_flags.is_set(MonsterMind.SEEN_POISON_RES):
		if realise_spell_is_useless(monster_type, 30):
			f_4.clear_bits(MonsterFlags4.BREATHE_POISON)
		if realise_spell_is_useless(monster_type, 30):
			f_5.clear_bits(MonsterFlags5.POISON_BALL)
	# If we know the player resists nether, maybe don't do nether spells
	if mind_flags.is_set(MonsterMind.SEEN_NETHER_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_NETHER)
		if realise_spell_is_useless(monster_type, 50):
			f_5.clear_bits(MonsterFlags5.NETHER_BALL)
		if realise_spell_is_useless(monster_type, 50):
			f_5.clear_bits(MonsterFlags5.NETHER_BOLT)
	# If we know the player resists light, maybe don't do light spells
	if mind_flags.is_set(MonsterMind.SEEN_LIGHT_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_LIGHT)
	# If we know the player resists darkness, maybe don't do darkness spells
	if mind_flags.is_set(MonsterMind.SEEN_DARK_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_DARK)
		if realise_spell_is_useless(monster_type, 50):
			f_5.clear_bits(MonsterFlags5.DARK_BALL)
	# If we know the player resists fear, don't do fear spells
	if mind_flags.is_set(MonsterMind.SEEN_FEAR_RES):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.SCARE)
	# If we know the player resists confiusion, maybe don't do confusion spells
	if mind_flags.is_set(MonsterMind.SEEN_CONF_RES):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.CONFUSE)
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_CONFUSION)
	# If we know the player resists chaos, maybe don't do chaos or confusion spells
	if mind_flags.is_set(MonsterMind.SEEN_CHAOS_RES):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.CONFUSE)
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_CONFUSION)
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_CONFUSION)
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.CHAOS_BALL)
	# If we know the player resists disenchantment, don't do disenchantment spells
	if mind_flags.is_set(MonsterMind.SEEN_DISEN_RES):
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.BREATHE_DISENCHANT)
	# If we know the player resists blindness, don't do blindness spells
	if mind_flags.is_set(MonsterMind.SEEN_BLIND_RES):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.BLINDNESS)
	# If we know the player resists nexus, maybe don't do nexus or teleport spells
	if mind_flags.is_set(MonsterMind.SEEN_NEXUS_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_NEXUS)
		if realise_spell_is_useless(monster_type, 50):
			f_6.clear_bits(MonsterFlags6.TELEPORT_LEVEL)
	# If we know the player resists sound, maybe don't do sound spells
	if mind_flags.is_set(MonsterMind.SEEN_SOUND_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_SOUND)
	# If we know the player resists shards, maybe don't do shard spells
	if mind_flags.is_set(MonsterMind.SEEN_SHARD_RES):
		if realise_spell_is_useless(monster_type, 50):
			f_4.clear_bits(MonsterFlags4.BREATHE_SHARDS)
		if realise_spell_is_useless(monster_type, 20):
			f_4.clear_bits(MonsterFlags4.SHARD_BALL)
	# If we know the player reflects bolts, don't do bolt spells
	if mind_flags.is_set(MonsterMind.SEEN_REFLECTION):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.COLD_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.FIRE_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.ACID_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.LIGHTNING_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.POISON_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.NETHER_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.WATER_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.VIS_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.PLASMA_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.ICE_BOLT)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.MAGIC_MISSILE)
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.ARROW_1D6)
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.ARROW_3D6)
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.ARROW_5D6)
		if realise_spell_is_useless(monster_type, 100):
			f_4.clear_bits(MonsterFlags4.ARROW_7D6)
	# If we know the player has free action, don't do slow or hold spells
	if mind_flags.is_set(MonsterMind.SEEN_FREE_ACTION):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.HOLD)
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.SLOW)
	# If we know the player has no vis, don't do vis drain
	if mind_flags.is_set(MonsterMind.SEEN_NO_POWER):
		if realise_spell_is_useless(monster_type, 100):
			f_5.clear_bits(MonsterFlags5.DRAIN_POWER)


func realise_spell_is_useless(monster_type: MonsterType, percentage: int) -> bool:
	if monster_type.flags_2.is_clear(MonsterFlags2.SMART):
		percentage /= 2
	return Rng.roll_die(100) <= percentage


func summon_possible(row_target: int, col_target: int) -> bool:
	for row: int in range(row_target - 2, row_target + 3):
		for col: int in range(col_target - 2, col_target + 3):
			# Can't summon out of bounds
			if not dungeon_level.in_inner_bounds(row, col):
				continue
			# Can't summon too far away from the target location
			if dungeon_level.distance(row_target, col_target, row, col) > 2:
				continue
			# Can't summon onto an Elder Sign
			if dungeon_level.get_grid(row, col).feature_type.name == "ElderSign":
				continue
			# Can't summon onto a Yellow Sign
			if dungeon_level.get_grid(row, col).feature_type.name == "YellowSign":
				continue
			# An empty tile in line of sight is acceptable
			if dungeon_level.grid_passable_no_creature(row, col) and dungeon_level.line_of_sight(row_target, col_target, row, col):
				return true
	# We didn't find anywhere and ran out of places to look
	return false


func choose_spell_against_player(monster_index: int, spells: Array[int], list_size: int) -> int:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# If the monster is stupid, cast a random spell
	if monster_type.flags_2.is_set(MonsterFlags2.STUPID):
		return spells[Rng.roll_die(list_size) - 1]
	# Deposit the spells into a number of buckets based on what they are used for
	var escape: Array[int]
	var attack: Array[int]
	var summon: Array[int]
	var tactic: Array[int]
	var annoy: Array[int]
	var haste: Array[int]
	var heal: Array[int]
	for spell: int in spells:
		if spell_is_for_escape(spell):
			escape.append(spell)
		if spell_is_for_attack(spell):
			attack.append(spell)
		if spell_is_for_summoning(spell):
			summon.append(spell)
		if spell_is_tactical(spell):
			tactic.append(spell)
		if spell_is_for_annoyance(spell):
			annoy.append(spell)
		if spell_is_for_haste(spell):
			haste.append(spell)
		if spell_is_for_healing(spell):
			heal.append(spell)
	# Priority One: If we're afraid or hurt, always use a random escape spell if we have one
	if monster.health < monster.max_health / 3 or monster.fear_level != 0:
		if escape.size() > 0:
			return escape[Rng.roll_die(escape.size()) - 1]
	# Priority Two: If we're hurt, always use a random healing spell if we have one
	if monster.health < monster.max_health / 3:
		if heal.size() != 0:
			return heal[Rng.roll_die(heal.size()) - 1]
	# Priority Three: If we're near the player and have no attack spells, probably use a
	# tactical spell
	if dungeon_level.distance(player.map_row, player.map_col, monster.map_row, monster.map_col) < 4 and attack.size() != 0 and Rng.roll_die(100) <= 75:
		if tactic.size() != 0:
			return tactic[Rng.roll_die(tactic.size()) - 1]
	# Priority Four: If we're at less than full health, probably use a healing spell
	if monster.health < monster.max_health * 3 / 4 and Rng.roll_die(100) <= 75:
		if heal.size() != 0:
			return heal[Rng.roll_die(heal.size()) - 1]
	# Priority Five: If we have a summoning spell, maybe use it
	if summon.size() != 0 and Rng.roll_die(100) <= 50:
		return summon[Rng.roll_die(summon.size()) - 1]
	# Priority Six: If we have a direct attack spell, probably use it
	if attack.size() != 0 and Rng.roll_die(100) <= 85:
		return attack[Rng.roll_die(attack.size()) - 1]
	# Priority Seven: If we have a tactical spell, maybe use it
	if tactic.size() != 0 and Rng.roll_die(100) <= 50:
		return tactic[Rng.roll_die(tactic.size()) - 1]
	# Priority Eight: If we have a haste spell, maybe use it
	if haste.size() != 0 and Rng.roll_die(100) <= 20 + monster_type.speed - monster.speed:
		return haste[Rng.roll_die(haste.size()) - 1]
	# Priority Nine: If we have an annoying spell, probably use it
	if annoy.size() != 0 and Rng.roll_die(100) <= 85:
		return annoy[Rng.roll_die(annoy.size()) - 1]
	# Priority Ten: Give up on using a spell
	return 0


func spell_is_for_annoyance(spell: int) -> bool:
	# MonsterFlags4.Shriek
	if spell == 96 + 0:
		return true
	# MonsterFlags5.DrainVis
	# MonsterFlags5.MindBlast
	# MonsterFlags5.BrainSmash
	# MonsterFlags5.CauseLightWounds
	# MonsterFlags5.CauseSeriousWounds
	# MonsterFlags5.CauseCriticalWounds
	if spell >= 128 + 9 and spell <= 128 + 14:
		return true
	# MonsterFlags5.Scare
	# MonsterFlags5.Blindness
	# MonsterFlags5.Confuse
	# MonsterFlags5.Slow
	# MonsterFlags5.Hold
	if spell >= 128 + 27 and spell <= 128 + 31:
		return true
	# MonsterFlags6.TeleportTo
	if spell == 160 + 8:
		return true
	# MonsterFlags6.Darkness
	# MonsterFlags6.CreateTraps
	# MonsterFlags6.Forget
	if spell >= 160 + 12 and spell <= 160 + 14:
		return true
	return false


func spell_is_for_attack(spell: int) -> bool:
	# MonsterFlags4.ShardBall MonsterFlags4.Arrow1D6 MonsterFlags4.Arrow3D6
	# MonsterFlags4.Arrow5D6 MonsterFlags4.Arrow7D6 MonsterFlags4.BreatheAcid
	# MonsterFlags4.BreatheLightning MonsterFlags4.BreatheFire MonsterFlags4.BreatheCold
	# MonsterFlags4.BreathePoison MonsterFlags4.BreatheNether MonsterFlags4.BreatheLight
	# MonsterFlags4.BreatheDark MonsterFlags4.BreatheConfusion MonsterFlags4.BreatheSound
	# MonsterFlags4.BreatheChaos MonsterFlags4.BreatheDisenchant MonsterFlags4.BreatheNexus
	# MonsterFlags4.BreatheTime MonsterFlags4.BreatheInertia MonsterFlags4.BreatheGravity
	# MonsterFlags4.BreatheShards MonsterFlags4.BreathePlasma MonsterFlags4.BreatheForce
	# MonsterFlags4.BreatheVis MonsterFlags4.RadiationBall MonsterFlags4.BreatheRadiation
	# MonsterFlags4.ChaosBall MonsterFlags4.BreatheDisintegration
	if spell < 128 and spell > 96:
		return true
	# MonsterFlags5.AcidBall MonsterFlags5.LightningBall MonsterFlags5.FireBall
	# MonsterFlags5.ColdBall MonsterFlags5.PoisonBall MonsterFlags5.NetherBall
	# MonsterFlags5.WaterBall MonsterFlags5.VisBall MonsterFlags5.DarkBall
	if spell >= 128 and spell <= 128 + 8:
		return true
	# MonsterFlags5.CauseLightWounds MonsterFlags5.CauseSeriousWounds
	# MonsterFlags5.CauseCriticalWounds MonsterFlags5.CauseMortalWounds MonsterFlags5.AcidBolt
	# MonsterFlags5.LightningBolt MonsterFlags5.FireBolt MonsterFlags5.ColdBolt
	# MonsterFlags5.PoisonBolt MonsterFlags5.NetherBolt MonsterFlags5.WaterBolt
	# MonsterFlags5.VisBolt MonsterFlags5.PlasmaBolt MonsterFlags5.IceBolt
	# MonsterFlags5.MagicMissile MonsterFlags5.Scare
	if spell >= 128 + 12 and spell <= 128 + 27:
		return true
	# MonsterFlags6.DreadCurse
	if spell == 160 + 1:
		return true
	return false


func spell_is_for_escape(spell: int) -> bool:
	# MonsterFlags6.Blink MonsterFlags6.TeleportSelf
	if spell == 160 + 4 or spell == 160 + 5:
		return true
	# MonsterFlags6.teleport_away MonsterFlags6.TeleportLevel
	if spell == 160 + 9 or spell == 160 + 10:
		return true
	return false


func spell_is_for_haste(spell: int) -> bool:
	# MonsterFlags6.Haste
	return spell == 160 + 0


func spell_is_for_healing(spell: int) -> bool:
	# MonsterFlags6.Heal
	return spell == 160 + 2


func spell_is_for_summoning(spell: int) -> bool:
	# MonsterFlags6.SummonKin MonsterFlags6.SummonReaver MonsterFlags6.SummonMonster
	# MonsterFlags6.SummonMonsters MonsterFlags6.SummonAnt MonsterFlags6.SummonSpider
	# MonsterFlags6.SummonHound MonsterFlags6.SummonHydra MonsterFlags6.SummonCthuloid
	# MonsterFlags6.SummonDemon MonsterFlags6.SummonUndead MonsterFlags6.SummonDragon
	# MonsterFlags6.SummonHiUndead MonsterFlags6.SummonHiDragon
	# MonsterFlags6.SummonGreatOldOne MonsterFlags6.SummonUnique
	return spell >= 160 + 16


func spell_is_tactical(spell: int) -> bool:
	# MonsterFlags6.Blink
	return spell == 160 + 4


func fire_ball_at_player(monster_index: int, projectile: Projectile, damage: int, radius: int) -> void:
	var flags: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	# Radius 0 means use the default radius
	var monster_type: MonsterType = monster.monster_type
	if radius < 1:
		radius = 3 if monster_type.flags_2.is_set(MonsterFlags2.POWERFUL) else 2
	savegame.spell_effects_handler.project(monster_index, radius, player.map_row, player.map_col, damage, projectile, flags)


func fire_bolt_at_monster(monster_index: int, targetY: int, targetX: int, projectile: Projectile, damage: int) -> void:
	var flags: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	savegame.spell_effects_handler.project(monster_index, 0, targetY, targetX, damage, projectile, flags)


func fire_bolt_at_player(monster_index: int, projectile: Projectile, damage: int) -> void:
	var flags: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	savegame.spell_effects_handler.project(monster_index, 0, player.map_row, player.map_col, damage, projectile, flags)


func breathe_at_monster(monster_index: int, targetY: int, targetX: int, projectile: Projectile, damage: int, radius: int) -> void:
	var flags: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	# Radius 0 means use the default radius
	var monster_type: MonsterType = monster.monster_type
	if radius < 1:
		radius = 3 if monster_type.flags_2.is_set(MonsterFlags2.POWERFUL) else 2
	# Make the radius negative to indicate we need a cone instead of a ball
	radius = 0 - radius
	savegame.spell_effects_handler.project(monster_index, radius, targetY, targetX, damage, projectile, flags)


func breathe_at_player(monster_index: int, projectile: Projectile, damage: int, radius: int) -> void:
	var flags: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	# Radius 0 means use the default radius
	if radius < 1:
		radius = 3 if monster_type.flags_2.is_set(MonsterFlags2.POWERFUL) else 2
	# Make the radius negative to indicate we need a cone instead of a ball
	radius = 0 - radius
	savegame.spell_effects_handler.project(monster_index, radius, player.map_row, player.map_col, damage, projectile, flags)


func try_casting_spell_against_another_monster(monster_index:int)->bool:
	var count:int = 0
	var spell:Array[int]
	var num:int = 0
	var monster:Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type:MonsterType = monster.monster_type
	var wake_up:bool = false
	var blind:bool = player.timed_blindness != 0
	var seen:bool = not blind and monster.is_visible
	var friendly:bool = monster.mind.is_set(MonsterMind.FRIENDLY)
	# Can't cast a spell if we're confused
	if monster.confusion_level != 0:
		return false
	# We have a chance to cast a spell based on our monster_type
	var chance:int = (monster_type.freq_innate + monster_type.freq_spell) / 2
	if chance == 0:
		return false
	if Rng.roll_die(100) > chance:
		return false
	# Look through the monster list to find a potential target
	for i:int in dungeon_level.monster_count_max:
		var target_index:int = i
		var target:Monster = dungeon_level.dungeon_monsters.monsters[target_index]
		var target_monster_type:MonsterType = target.monster_type
		# Can't cast spells on ourself
		if target == monster:
			continue
		# Can't cast spells on empty monster slots
		if target.monster_type == null:
			continue
		# Don't cast spells on monsters from the same team
		if monster.mind.is_set(MonsterMind.FRIENDLY) == target.mind.is_set(MonsterMind.FRIENDLY):
			continue
		# If the target is too far away from the player, don't cast a spell
		if target.distance_from_player > DungeonLevel.MAX_SPELL_RANGE:
			continue
		# If we don't have line of sight to the target, don't cast a spell
		if not dungeon_level.projectable(monster.map_row, monster.map_col, target.map_row, target.map_col):
			continue
		var row:int = target.map_row
		var col:int = target.map_col
		var monster_level:int = monster_type.level if monster_type.level >= 1 else 1
		var f_4: FlagSet = FlagSet.new()
		var f_5: FlagSet = FlagSet.new()
		var f_6: FlagSet = FlagSet.new()
		f_4.copy(monster_type.flags_4)
		f_5.copy(monster_type.flags_5)
		f_6.copy(monster_type.flags_6)
		# If we're smart and badly injured, we may want to prioritise spells that disable
		# the target, summon help, or let us escape over spells that do direct damage
		if monster_type.flags_2.is_set(MonsterFlags2.SMART) and monster.health < monster.max_health / 10 and Rng.roll_die(100) <= 50:
			f_4.keep_bits(MonsterFlags4.INT_MASK)
			f_5.keep_bits(MonsterFlags5.INT_MASK)
			f_6.keep_bits(MonsterFlags6.INT_MASK)
			# If we just got rid of all our spells then abort
			if f_4.is_all_clear() and f_5.is_all_clear() and f_6.is_all_clear():
				return false
		# If there's nowhere around the target to put a summoned creature, then remove
		# summoning spells
		if (f_4.is_set(MonsterFlags4.SUMMON_MASK) or f_5.is_set(MonsterFlags5.SUMMON_MASK) or f_6.is_set(MonsterFlags6.SUMMON_MASK)) and (monster_type.flags_2.is_clear(MonsterFlags2.STUPID) and not summon_possible(target.map_row, target.map_col)):
			f_4.clear_bits(MonsterFlags4.SUMMON_MASK)
			f_5.clear_bits(~MonsterFlags5.SUMMON_MASK)
			f_6.clear_bits(~MonsterFlags6.SUMMON_MASK)
		# If we've removed all our spells, don't cast anything
		if f_4.is_all_clear() and f_5.is_all_clear() and f_6.is_all_clear():
			return false
		# Gather up all the spells that are left and put them in an array flags_4 spells are 96
		# + bit number (0-31)
		for k:int in 32:
			if f_4.is_set(1 << k):
				spell.append(k + 32 * 3)
		for k:int in 32:
			if f_5.is_set(1 << k):
				spell.append(k + 32 * 4)
		for k:int in 32:
			if f_6.is_set(1 << k):
				spell.append(k + 32 * 5)
		num = spell.size()
		# If we ended up with no spells, don't cast
		if num == 0:
			return false
		# If the player's already dead or off the level, don't cast
		if not savegame.playing or player.is_dead or savegame.new_level_flag:
			return false
		var monster_name:String = monster.describe(0x00)
		var monster_possessive:String = monster.describe(0x22)
		var target_name:String = target.describe(0x00)
		# Against other monsters we pick spells randomly
		var thrown_spell:int = spell[Rng.roll_die(num)-1]
		var see_monster:bool = seen
		var see_target:bool = not blind and target.is_visible
		var see_either:bool = see_monster or see_target
		var see_both:bool = see_monster and see_target
		var s_e_h:SpellEffectsHandler = savegame.spell_effects_handler
		# If we decided not to cast, don't
		match thrown_spell:
			# MonsterFlags4.Shriek
			96 + 0:
				savegame.disturb(true)
				Profile.instance.message_print("You hear a shriek." if not see_monster else "%s shrieks at %s." % [monster_name, target_name])
				wake_up = true
			# MonsterFlags4.Xxx2
			96 + 1:
				pass
			# MonsterFlags4.Xxx3
			96 + 2:
				pass
			# MonsterFlags4.ShardBall
			96 + 3:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a shard ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileShard.new(s_e_h), 800 if monster.health / 4 > 800 else monster.health / 4, 2)
			# MonsterFlags4.Arrow1D6
			96 + 4:
				savegame.disturb(true)
				Profile.instance.message_print("You hear a strange noise." if not see_either else "%s fires an arrow at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileArrow.new(s_e_h), Rng.roll_dice(1, 6))
			# MonsterFlags4.Arrow3D6
			96 + 5:
				savegame.disturb(true)
				Profile.instance.message_print("You hear a strange noise." if not see_either else "%s fires an arrow at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileArrow.new(s_e_h), Rng.roll_dice(3, 6))
			# MonsterFlags4.Arrow5D6
			96 + 6:
				savegame.disturb(true)
				Profile.instance.message_print("You hear a strange noise." if not see_either else "%s fires a missile at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileArrow.new(s_e_h), Rng.roll_dice(5, 6))
			# MonsterFlags4.Arrow7D6
			96 + 7:
				savegame.disturb(true)
				Profile.instance.message_print("You hear a strange noise." if not see_either else "%s fires a missile at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileArrow.new(s_e_h), Rng.roll_dice(7, 6))
			# MonsterFlags4.BreatheAcid
			96 + 8:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes acid at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileAcid.new(s_e_h), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			# MonsterFlags4.BreatheLightning
			96 + 9:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes lightning at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileElectricity.new(s_e_h), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			# MonsterFlags4.BreatheFire
			96 + 10:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes fire at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileFire.new(s_e_h), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			# MonsterFlags4.BreatheCold
			96 + 11:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes frost at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileCold.new(s_e_h), 1600 if monster.health / 3 > 1600 else monster.health / 3, 0)
			# MonsterFlags4.BreathePoison
			96 + 12:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes gas at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectilePoison.new(s_e_h), 800 if monster.health / 3 > 800 else monster.health / 3, 0)
			# MonsterFlags4.BreatheNether
			96 + 13:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes nether at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileNether.new(s_e_h), 550 if monster.health / 6 > 550 else monster.health / 6, 0)
			# MonsterFlags4.BreatheLight
			96 + 14:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes light at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileLight.new(s_e_h), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			# MonsterFlags4.BreatheDark
			96 + 15:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes darkness at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileDark.new(s_e_h), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			# MonsterFlags4.BreatheConfusion
			96 + 16:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes confusion at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileConfusion.new(s_e_h), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			# MonsterFlags4.BreatheSound
			96 + 17:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes sound at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileSound.new(s_e_h), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			# MonsterFlags4.BreatheChaos
			96 + 18:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes chaos at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileChaos.new(s_e_h), 600 if monster.health / 6 > 600 else monster.health / 6, 0)
			# MonsterFlags4.BreatheDisenchant
			96 + 19:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes disenchantment at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileDisenchantment.new(s_e_h), 500 if monster.health / 6 > 500 else monster.health / 6, 0)
			# MonsterFlags4.BreatheNexus
			96 + 20:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes nexus at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileNexus.new(s_e_h), 250 if monster.health / 3 > 250 else monster.health / 3, 0)
			# MonsterFlags4.BreatheTime
			96 + 21:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes time at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileTime.new(s_e_h), 150 if monster.health / 3 > 150 else monster.health / 3, 0)
			# MonsterFlags4.BreatheInertia
			96 + 22:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes inertia at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileInertia.new(s_e_h), 200 if monster.health / 6 > 200 else monster.health / 6, 0)
			# MonsterFlags4.BreatheGravity
			96 + 23:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes gravity at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileGravity.new(s_e_h), 200 if monster.health / 3 > 200 else monster.health / 3, 0)
			# MonsterFlags4.BreatheShards
			96 + 24:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes shards at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileExplode.new(s_e_h), 400 if monster.health / 6 > 400 else monster.health / 6, 0)
			# MonsterFlags4.BreathePlasma
			96 + 25:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes plasma at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectilePlasma.new(s_e_h), 150 if monster.health / 6 > 150 else monster.health / 6, 0)
			# MonsterFlags4.BreatheForce
			96 + 26:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes force at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileForce.new(s_e_h), 200 if monster.health / 6 > 200 else monster.health / 6, 0)
			# MonsterFlags4.BreatheVis
			96 + 27:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes magical energy at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectilePower.new(s_e_h), 250 if monster.health / 3 > 250 else monster.health / 3, 0)
			# MonsterFlags4.RadiationBall
			96 + 28:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a ball of radiation at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileNuke.new(s_e_h), monster_level + Rng.roll_dice(10, 6), 2)
			# MonsterFlags4.BreatheRadiation
			96 + 29:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes toxic waste at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileNuke.new(s_e_h), 800 if monster.health / 3 > 800 else monster.health / 3, 0)
			# MonsterFlags4.ChaosBall
			96 + 30:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble frighteningly." if not see_either else "%s invokes raw chaos upon %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileChaos.new(s_e_h), (monster_level * 2) + Rng.roll_dice(10, 10), 4)
			# MonsterFlags4.BreatheDisintegration
			96 + 31:
				savegame.disturb(true)
				Profile.instance.message_print("You hear breathing noise." if not see_either else "%s breathes disintegration at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileDisintegrate.new(s_e_h), 300 if monster.health / 3 > 300 else monster.health / 3, 0)
			# MonsterFlags5.AcidBall
			128 + 0:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts an acid ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileAcid.new(s_e_h), Rng.roll_die(monster_level * 3) + 15, 2)
			# MonsterFlags5.LightningBall
			128 + 1:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a lightning ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileElectricity.new(s_e_h), Rng.roll_die(monster_level * 3 / 2) + 8, 2)
			# MonsterFlags5.FireBall
			128 + 2:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a fire ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileFire.new(s_e_h), Rng.roll_die(monster_level * 7 / 2) + 10, 2)
			# MonsterFlags5.ColdBall
			128 + 3:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a frost ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileCold.new(s_e_h), Rng.roll_die(monster_level * 3 / 2) + 10, 2)
			# MonsterFlags5.PoisonBall
			128 + 4:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a stinking cloud at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectilePoison.new(s_e_h), Rng.roll_dice(12, 2), 2)
			# MonsterFlags5.NetherBall
			128 + 5:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s casts a nether ball at %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileNether.new(s_e_h), 50 + Rng.roll_dice(10, 10) + monster_level,
					2)
			# MonsterFlags5.WaterBall
			128 + 6:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble." if not see_either else "%s gestures fluidly at %s." % [monster_name, target_name])
				Profile.instance.message_print("%s is engulfed in a whirlpool." % [target_name])
				breathe_at_monster(monster_index, row, col, ProjectileWater.new(s_e_h), Rng.roll_die(monster_level * 5 / 2) + 50, 4)
			# MonsterFlags5.VisBall
			128 + 7:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble powerfully." if not see_either else "%s invokes a power storm upon %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectilePower.new(s_e_h), (monster_level * 5) + Rng.roll_dice(10, 10), 4)
			# MonsterFlags5.DarkBall
			128 + 8:
				savegame.disturb(true)
				Profile.instance.message_print("You hear someone mumble powerfully." if not see_either else "%s invokes a darkness storm upon %s." % [monster_name, target_name])
				breathe_at_monster(monster_index, row, col, ProjectileDark.new(s_e_h), (monster_level * 5) + Rng.roll_dice(10, 10), 4)
			# MonsterFlags5.DrainVis
			128 + 9:
				var r1: int = (Rng.roll_die(monster_level) / 2) + 1
				if see_monster:
					Profile.instance.message_print("%s draws psychic energy from %s." % [monster_name, target_name])
				if monster.health < monster.max_health:
					if not (target_monster_type.flags_4.is_any_set() or target_monster_type.flags_5.is_any_set() or target_monster_type.flags_6.is_any_set()):
						if see_both:
							Profile.instance.message_print("%s is unaffected!" % [target_name])
					else:
						monster.health += 6 * r1
						if monster.health > monster.max_health:
							monster.health = monster.max_health
						if savegame.tracked_monster_index == monster_index:
							player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
						if seen:
							Profile.instance.message_print("%s appears healthier." % [monster_name])
				wake_up = true
			# MonsterFlags5.MindBlast
			128 + 10:
				savegame.disturb(true)
				if seen:
					Profile.instance.message_print("%s gazes intently at %s." % [monster_name, target_name])
				if target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION) or target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION) and seen:
						target_monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_CONFUSION)
					if see_target:
						Profile.instance.message_print("%s is unaffected!" % [target_name])
				else:
					Profile.instance.message_print("%s is blasted by psionic energy." % [target_name])
					target.confusion_level += Rng.roll_die(4) + 3
					take_damage_from_another_monster(target_index, Rng.roll_dice(8, 8), " collapses, a mindless husk.")
				wake_up = true
			# MonsterFlags5.BrainSmash
			128 + 11:
				savegame.disturb(true)
				if seen:
					Profile.instance.message_print("%s gazes intently at %s." % [monster_name, target_name])
				if (target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION) or
					target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10):
					if (target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION) and seen):
						target_monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_CONFUSION)
					if see_target:
						Profile.instance.message_print("%s is unaffected!" % [target_name])
				else:
					if see_target:
						Profile.instance.message_print("%s is blasted by psionic energy." % [target_name])
					target.confusion_level += Rng.roll_die(4) + 3
					target.speed -= Rng.roll_die(4) + 3
					target.stun_level += Rng.roll_die(4) + 3
					take_damage_from_another_monster(target_index, Rng.roll_dice(12, 15), " collapses, a mindless husk.")
				wake_up = true
			# MonsterFlags5.CauseLightWounds
			128 + 12:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s points at %s and curses." % [monster_name, target_name])
				if target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s resists!" % [target_name])
				else:
					take_damage_from_another_monster(target_index, Rng.roll_dice(3, 8), " is destroyed.")
				wake_up = true
			# MonsterFlags5.CauseSeriousWounds
			128 + 13:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s points at %s and curses horribly." % [monster_name, target_name])
				if target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s resists!" % [target_name])
				else:
					take_damage_from_another_monster(target_index, Rng.roll_dice(8, 8), " is destroyed.")
				wake_up = true
			# MonsterFlags5.CauseCriticalWounds
			128 + 14:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s points at %s, incanting terribly!" % [monster_name, target_name])
				if target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s resists!" % [target_name])
				else:
					take_damage_from_another_monster(target_index, Rng.roll_dice(10, 15), " is destroyed.")
				wake_up = true
			# MonsterFlags5.CauseMortalWounds
			128 + 15:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s points at %s, screaming the word 'DIE!'" % [monster_name, target_name])
				if target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s resists!" % [target_name])
				else:
					take_damage_from_another_monster(target_index, Rng.roll_dice(15, 15), " is destroyed.")
				wake_up = true
			# MonsterFlags5.AcidBolt
			128 + 16:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a acid bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileAcid.new(s_e_h), Rng.roll_dice(7, 8) + (monster_level / 3))
			# MonsterFlags5.LightningBolt
			128 + 17:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a lightning bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileElectricity.new(s_e_h), Rng.roll_dice(4, 8) + (monster_level / 3))
			# MonsterFlags5.FireBolt
			128 + 18:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a fire bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileFire.new(s_e_h), Rng.roll_dice(9, 8) + (monster_level / 3))
			# MonsterFlags5.ColdBolt
			128 + 19:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a frost bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileCold.new(s_e_h), Rng.roll_dice(6, 8) + (monster_level / 3))
			# MonsterFlags5.PoisonBolt
			128 + 20:
				pass
			# MonsterFlags5.NetherBolt
			128 + 21:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a nether bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileNether.new(s_e_h),
					30 + Rng.roll_dice(5, 5) + (monster_level * 3 / 2))
			# MonsterFlags5.WaterBolt
			128 + 22:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a water bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileWater.new(s_e_h), Rng.roll_dice(10, 10) + monster_level)
			# MonsterFlags5.VisBolt
			128 + 23:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a power bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectilePower.new(s_e_h), Rng.roll_die(monster_level * 7 / 2) + 50)
			# MonsterFlags5.PlasmaBolt
			128 + 24:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a plasma bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectilePlasma.new(s_e_h), 10 + Rng.roll_dice(8, 7) + monster_level)
			# MonsterFlags5.IceBolt
			128 + 25:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts an ice bolt at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileIce.new(s_e_h), Rng.roll_dice(6, 6) + monster_level)
			# MonsterFlags5.MagicMissile
			128 + 26:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s casts a magic missile at %s." % [monster_name, target_name])
				fire_bolt_at_monster(monster_index, row, col, ProjectileMissile.new(s_e_h), Rng.roll_dice(2, 6) + (monster_level / 3))
			# MonsterFlags5.Scare
			128 + 27:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles, and you hear scary noises.")
				else:
					Profile.instance.message_print("%s casts a fearful illusion at %s." % [monster_name, target_name])
				if target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_FEAR):
					if see_target:
						Profile.instance.message_print("%s refuses to be frightened." % [target_name])
				elif target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s refuses to be frightened." % [target_name])
				else:
					if target.fear_level == 0 and see_target:
						Profile.instance.message_print("%s flees in terror!" % [target_name])
					target.fear_level += Rng.roll_die(4) + 3
				wake_up = true
			# MonsterFlags5.Blindness
			128 + 28:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					var it:String = "s" if target_name == "it" else "s"
					Profile.instance.message_print("%s casts a spell, burning %s%s eyes." % [monster_name, target_name, it])
				if target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION):
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				elif target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				else:
					if see_target:
						Profile.instance.message_print("%s is blinded!" % [target_name])
					target.confusion_level += 11 + Rng.roll_die(4)
				wake_up = true
			# MonsterFlags5.Confuse
			128 + 29:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles, and you hear puzzling noises." % [monster_name])
				else:
					Profile.instance.message_print("%s creates a mesmerising illusion in front of %s." % [monster_name, target_name])
				if target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_CONFUSION):
					if see_target:
						Profile.instance.message_print("%s disbelieves the feeble spell." % [target_name])
				elif target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s disbelieves the feeble spell." % [target_name])
				else:
					if see_target:
						Profile.instance.message_print("%s seems confused." % [target_name])
					target.confusion_level += 11 + Rng.roll_die(4)
				wake_up = true
			# MonsterFlags5.Slow
			128 + 30:
				savegame.disturb(true)
				if not blind and see_either:
					var it:String = "s" if target_name == "it" else "'s" 
					Profile.instance.message_print("%s drains power from %s%s muscles." % [monster_name, target_name, it])
				if target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				elif target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				else:
					target.speed -= 10
					if see_target:
						Profile.instance.message_print("%s starts moving slower." % [target_name])
				wake_up = true
			# MonsterFlags5.Hold
			128 + 31:
				savegame.disturb(true)
				if not blind and see_monster:
					Profile.instance.message_print("%s stares intently at %s." % [monster_name, target_name])
				if target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or target_monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_STUN):
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				elif target_monster_type.level > Rng.roll_die(1 if monster_level - 10 < 1 else monster_level - 10) + 10:
					if see_target:
						Profile.instance.message_print("%s is unaffected." % [target_name])
				else:
					target.stun_level += Rng.roll_die(4) + 4
					if see_target:
						Profile.instance.message_print("%s is paralyzed!" % [target_name])
				wake_up = true
			# MonsterFlags6.Haste
			160 + 0:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s concentrates on %s body." % [monster_name, monster_possessive])
				if monster.speed < monster_type.speed + 10:
					if see_monster:
						Profile.instance.message_print("%s starts moving faster." % [monster_name])
					monster.speed += 10
				elif monster.speed < monster_type.speed + 20:
					if see_monster:
						Profile.instance.message_print("%s starts moving faster." % [monster_name])
					monster.speed += 2
			# MonsterFlags6.DreadCurse
			160 + 1:
				savegame.disturb(false)
				Profile.instance.message_print("You hear someone invoke the Dread Curse of Azathoth!" if not see_monster else "%s invokes the Dread Curse of Azathoth on %s" % [monster_name, target_name])
				if target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
					if not blind and see_target:
						Profile.instance.message_print("%s is unaffected!" % [target_name])
				else:
					if monster_type.level + Rng.roll_die(20) > target_monster_type.level + 10 + Rng.roll_die(20):
						target.health -= (65 + Rng.roll_die(25)) * target.health / 100
						if target.health < 1:
							target.health = 1
					else:
						if see_target:
							Profile.instance.message_print("%s resists!" % [target_name])
				wake_up = true
			# MonsterFlags6.Heal
			160 + 2:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s concentrates on %s wounds." % [monster_name, monster_possessive])
				monster.health += monster_level * 6
				if monster.health >= monster.max_health:
					monster.health = monster.max_health
					Profile.instance.message_print("%s looks completely healed!" % [monster_name] if seen else "%s sounds completely healed!" % [monster_name])
				else:
					Profile.instance.message_print("%s looks healthier." % [monster_name] if seen else "%s sounds healthier." % [monster_name])
				if savegame.tracked_monster_index == monster_index:
					player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
				if monster.fear_level != 0:
					monster.fear_level = 0
					if see_monster:
						Profile.instance.message_print("%s recovers %s courage." % [monster_name, monster_possessive])
			# MonsterFlags6.Xxx2
			160 + 3:
				pass
			# MonsterFlags6.Blink
			160 + 4:
				savegame.disturb(true)
				if see_monster:
					Profile.instance.message_print("%s blinks away." % [monster_name])
				savegame.spell_effects_handler.teleport_away(monster_index, 10)
			# MonsterFlags6.TeleportSelf
			160 + 5:
				savegame.disturb(true)
				if see_monster:
					Profile.instance.message_print("%s teleports away.")
				savegame.spell_effects_handler.teleport_away(monster_index, (DungeonLevel.MAX_SPELL_RANGE * 2) + 5)
			# MonsterFlags6.Xxx3
			160 + 6:
				pass
			# MonsterFlags6.Xxx4
			160 + 7:
				pass
			# MonsterFlags6.TeleportTo
			160 + 8:
				pass
			# MonsterFlags6.teleport_away
			160 + 9:
				var resists_teleport: bool = false
				savegame.disturb(true)
				Profile.instance.message_print("%s teleports %s away." % [monster_name, target_name])
				if target_monster_type.flags_3.is_set(MonsterFlags3.RESIST_TELEPORT):
					if target_monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
						if see_target:
							target_monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.RESIST_TELEPORT)
							Profile.instance.message_print("%s is unaffected!" % [target_name])
						resists_teleport = true
					elif target_monster_type.level > Rng.roll_die(100):
						if see_target:
							target_monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.RESIST_TELEPORT)
							Profile.instance.message_print("%s resists!" % [target_name])
						resists_teleport = true
				if not resists_teleport:
					savegame.spell_effects_handler.teleport_away(target_index, (DungeonLevel.MAX_SPELL_RANGE * 2) + 5)
			# MonsterFlags6.TeleportLevel
			160 + 10:
				pass
			# MonsterFlags6.Xxx5
			160 + 11:
				pass
			# MonsterFlags6.Darkness
			160 + 12:
				savegame.disturb(true)
				Profile.instance.message_print("%s mumbles." % [monster_name] if blind else "%s gestures in shadow." % [monster_name])
				if seen:
					Profile.instance.message_print("%s is surrounded by darkness." % [target_name])
				savegame.spell_effects_handler.project(monster_index, 3, row, col, 0, ProjectileDarkWeak.new(s_e_h), ProjectionFlag.GRID | ProjectionFlag.KILL)
				savegame.spell_effects_handler.unlight_room(row, col)
			# MonsterFlags6.CreateTraps
			160 + 13:
				pass
			# MonsterFlags6.Forget
			160 + 14:
				pass
			# MonsterFlags6.Xxx6
			160 + 15:
				pass
			# MonsterFlags6.SummonKin
			160 + 16:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					var kin:String = "minions" if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) else "kin"
					Profile.instance.message_print("%s magically summons %s %s." % [monster_name, monster_possessive, kin])
				dungeon_level.dungeon_monsters.summon_kin_type = monster_type.character
				for k:int in 6:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_KIN, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_KIN):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
			# MonsterFlags6.SummonReaver
			160 + 17:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons Black Reavers!" % [monster_name])
				if blind and count != 0:
					Profile.instance.message_print("You hear heavy steps nearby." % [monster_name])
				if friendly:
					dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_REAVER, true)
				else:
					savegame.spell_effects_handler.SummonReaver()
			# MonsterFlags6.SummonMonster
			160 + 18:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons help!" % [monster_name])
				for k:int in 1:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_NO_UNIQUES, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, 0):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear something appear nearby.")
			# MonsterFlags6.SummonMonsters
			160 + 19:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons monsters!" % [monster_name])
				for k:int in 8:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_NO_UNIQUES, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, 0):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
			# MonsterFlags6.SummonAnt
			160 + 20:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons ants." % [monster_name])
				for k:int in 6:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_ANT, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_ANT):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
			# MonsterFlags6.SummonSpider
			160 + 21:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons spiders.")
				for k:int in 6:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_SPIDER, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_SPIDER):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
			# MonsterFlags6.SummonHound
			160 + 22:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons hounds." % [monster_name])
				for k:int in 6:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_HOUND, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_HOUND):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
				break

			# MonsterFlags6.SummonHydra
			160 + 23:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons hydras.")
				for k:int in 6:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_HYDRA, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_HYDRA):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many things appear nearby.")
			# MonsterFlags6.SummonCthuloid
			160 + 24:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons a Cthuloid entity!" % [monster_name])
				for k:int in 1:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_CTHULOID, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_CTHULOID):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear something appear nearby.")
			# MonsterFlags6.SummonDemon
			160 + 25:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons a demon!" % [monster_name])
				for k:int in 1:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_DEMON, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_DEMON):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear something appear nearby.")
			# MonsterFlags6.SummonUndead
			160 + 26:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons an undead adversary!" % [monster_name])
				for k:int in 1:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_UNDEAD, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_UNDEAD):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear something appear nearby.")
			# MonsterFlags6.SummonDragon
			160 + 27:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons a dragon!" % [monster_name])
				for k:int in 1:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_DRAGON, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_DRAGON):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear something appear nearby.")
			# MonsterFlags6.SummonHiUndead
			160 + 28:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons greater undead!" % [monster_name])
				for k:int in 8:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_HI_UNDEAD_NO_UNIQUES, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_HI_UNDEAD):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many creepy things appear nearby.")
			# MonsterFlags6.SummonHiDragon
			160 + 29:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons ancient dragons!" % [monster_name])
				for k:int in 8:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_HI_DRAGON_NO_UNIQUES, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_HI_DRAGON):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many powerful things appear nearby.")
				break

			# MonsterFlags6.SummonGreatOldOne
			160 + 30:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons Great Old Ones!" % [monster_name])
				for k:int in 8:
					if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_GOO):
						count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear immortal beings appear nearby.")
			# MonsterFlags6.SummonUnique
			160 + 31:
				savegame.disturb(true)
				if blind or not see_monster:
					Profile.instance.message_print("%s mumbles." % [monster_name])
				else:
					Profile.instance.message_print("%s magically summons special opponents!" % [monster_name])
				for k:int in 8:
					if not friendly:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_UNIQUE):
							count += 1
				for k:int in 8:
					if friendly:
						if dungeon_level.dungeon_monsters.summon_specific_friendly(row, col, monster_level, SummonType.SUMMON_HI_UNDEAD_NO_UNIQUES, true):
							count += 1
					else:
						if dungeon_level.dungeon_monsters.summon_specific(row, col, monster_level, SummonType.SUMMON_HI_UNDEAD):
							count += 1
				if blind and count != 0:
					Profile.instance.message_print("You hear many powerful things appear nearby.")
		# Most spells will wake up the target if it's asleep
		if wake_up:
			target.sleep_level = 0
		# If the player saw us cast the spell, let them learn we can do that
		if seen:
			if thrown_spell < 32 * 4:
				monster_type.knowledge.r_flags_4.set_bits(1 << (thrown_spell - (32 * 3)))
				if monster_type.knowledge.r_cast_innate < 255:
					monster_type.knowledge.r_cast_innate += 1
			elif thrown_spell < 32 * 5:
				monster_type.knowledge.r_flags_5.set_bits(1 << (thrown_spell - (32 * 4)))
				if monster_type.knowledge.r_cast_spell < 255:
					monster_type.knowledge.r_cast_spell += 1
			elif thrown_spell < 32 * 6:
				monster_type.knowledge.r_flags_6.set_bits(1 << (thrown_spell - (32 * 5)))
				if monster_type.knowledge.r_cast_spell < 255:
					monster_type.knowledge.r_cast_spell += 1
		# If we killed the player, let their descendants remember that
		if player.is_dead and monster_type.knowledge.r_deaths < Constants.MAX_INT:
			monster_type.knowledge.r_deaths += 1
		# We did cast a spell
		return true
	return false




func take_damage_from_another_monster(monster_index:int, damage:int, note:String)->void:
	CommandEngine.fear = false
	var monster:Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type:MonsterType = monster.monster_type
	# Track the monster that has just taken damage
	if savegame.tracked_monster_index == monster_index:
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
	monster.sleep_level = 0
	# Take the damage
	monster.health -= damage
	# Did it kill us?
	if monster.health < 0:
		# Uniques and guardians can never be killed by other monsters
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
			monster.health = 1
		else:
			# Construct a message telling the player what happened
			var monster_name:String = monster.describe(0)
			# Append the note if there is one
			if note != "":
				Profile.instance.message_print(monster_name + note)
			# Don't tell the player if the monster is not visible
			elif not monster.is_visible:
				pass
			# Some non-living things are destroyed rather than dying
			elif (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or
				monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_3.is_set(MonsterFlags2.STUPID) or
				monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
				Profile.instance.message_print("%s is destroyed.")
			else:
				Profile.instance.message_print("%s is killed.")
			# Let the save game know we've died
			savegame.monster_death(monster_index)
			# Delete us from the monster list
			dungeon_level.dungeon_monsters.delete_monster_by_index(monster_index, true)
			CommandEngine.fear = false
			return
	# Assuming we survived, check if the attack made us afraid
	if monster.fear_level != 0 and damage > 0:
		# If we're already afraid, we might get desperate and overcome our fear
		var tmp:int = Rng.roll_die(damage)
		if tmp < monster.fear_level:
			monster.fear_level -= tmp
		else:
			monster.fear_level = 0
			CommandEngine.fear = false
	# If we weren't already afraid, this attack might make us afraid
	if monster.fear_level == 0 and monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_FEAR):
		var percentage:int = 100 * monster.health / monster.max_health
		if (percentage <= 10 and Rng.roll_die(10) <= percentage) or (damage >= monster.health and Rng.roll_die(100) <= 80):
			CommandEngine.fear = true
			monster.fear_level = Rng.roll_die(10) + (20 if damage >= monster.health and percentage > 7 else (11 - percentage) * 5)
