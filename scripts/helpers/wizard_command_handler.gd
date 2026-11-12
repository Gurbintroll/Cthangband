# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name WizardCommandHandler
extends RefCounted

const HEADERS: Array[String] = ["a", "A", "0"]

var dungeon_level: DungeonLevel
var player: Player
var savegame: SaveGame

func _init(my_dungeon_level: DungeonLevel, my_player: Player) -> void:
	dungeon_level = my_dungeon_level
	player = my_player
	savegame = SaveGame.instance

func command_wiz_mode() -> void:
	Console.print_line_at(0, 0, "Enter Wizard Code: ", Ink.WHITE)
	var input: Dictionary[bool, String] = User.get_input(0, 19, "", 31)
	if not input.has(true):
		return
	Console.clear_row(0)
	if input[true] == "Rincewind":
		player.is_wizard = true
		Profile.instance.message_print("Wizard mode activated.")
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_TITLE)


func command_wizard() -> void:
	var cmd: String = User.get_simple_choice("Wizard Command:")
	if User.command_argument <= 0:
		User.command_argument = 0
	match cmd:
		"?":
			command_wiz_help()
		"a":
			command_wiz_cure_all()
		"b":
			command_wiz_bamf()
		"c":
			command_wiz_create_item()
		"C":
			print("command_wiz_create_artifact is not yet implemented")
			#command_wiz_create_artifact()
		"d":
			savegame.spell_effects_handler.detect_all()
		"D":
			command_wiz_debug()
		"e":
			command_wiz_change()
		"f":
			savegame.spell_effects_handler.identify_fully()
		"g":
			dungeon_level.acquirement(player.map_row, player.map_col, 1, false)
		"h":
			player.reroll_hit_points()
		"H":
			print("command_wiz_summon horde is not yet implemented")
			#command_wiz_summon_horde()
		"i":
			savegame.spell_effects_handler.identify_pack()
		"j":
			command_wiz_jump()
		"k":
			savegame.spell_effects_handler.self_knowledge()
		"l":
			command_wiz_learn()
		"m":
			dungeon_level.map_area()
		"M":
			player.genome.gain_mutation()
		"r":
			player.gain_level_reward()
		"N":
			print("command_wiz_named_friendly is not yet implemented")
			#command_wiz_named_friendly(1, true)
		"n":
			print("command_wiz_named is not yet implemented")
			#command_wiz_named(1, true)
		"o":
			print("command_wiz_play is not yet implemented")
			#command_wiz_play()
		"p":
			savegame.spell_effects_handler.teleport_player(10)
		"s":
			print("command_wiz_summon is not yet implemented")
			#command_wiz_summon(1)
		"t":
			savegame.spell_effects_handler.teleport_player(100)
		"v":
			dungeon_level.acquirement(player.map_row, player.map_col, 1, true)
		"w":
			dungeon_level.wiz_light()
		"W":
			player.is_winner = true
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_TITLE)
			Profile.instance.message_print("*** CONGRATULATIONS ***")
			Profile.instance.message_print("You have won the game!")
			Profile.instance.message_print("You may retire (\"Q\") when you are ready.")
		"x":
			player.gain_experience(player.experience_points + 1)
		"Z":
			command_wiz_zap()
		"z":
			command_wiz_bolt()
		" ":
			return
		"":
			return
		_:
			Profile.instance.message_print("That is not a valid wizard command.")


func command_wiz_learn() -> void:
	for item_type: ItemType in savegame.item_types.types:
		if item_type.has_flavour:
			item_type.flavour_aware = true


func command_wiz_help() -> void:
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.clear_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	Console.print_at(1, 31, "Wizard Commands", Ink.RED)
	Console.print_at(2, 31, "===============", Ink.RED)
	Console.print_at(4, 1, "Character Editing", Ink.RED)
	Console.print_at(5, 1, "=================", Ink.RED)
	Console.print_at(7, 1, "a = Cure All", Ink.WHITE)
	Console.print_at(8, 1, "e = Edit Stats", Ink.WHITE)
	Console.print_at(9, 1, "h = Reroll Hitpoints", Ink.WHITE)
	Console.print_at(10, 1, "k = Self Knowledge", Ink.WHITE)
	Console.print_at(11, 1, "M = Gain Mutation", Ink.WHITE)
	Console.print_at(12, 1, "r = Gain Level Reward", Ink.WHITE)
	Console.print_at(13, 1, "x = Gain Experience", Ink.WHITE)
	Console.print_at(15, 1, "Movement", Ink.RED)
	Console.print_at(16, 1, "========", Ink.RED)
	Console.print_at(18, 1, "b = Teleport to Target", Ink.WHITE)
	Console.print_at(19, 1, "j = Jump Levels", Ink.WHITE)
	Console.print_at(20, 1, "p = Phase Door", Ink.WHITE)
	Console.print_at(21, 1, "t = Teleport", Ink.WHITE)
	Console.print_at(4, 26, "Monsters", Ink.RED)
	Console.print_at(5, 26, "========", Ink.RED)
	Console.print_at(7, 26, "s = Summon Monster", Ink.WHITE)
	Console.print_at(8, 26, "n = Summon Named Monster", Ink.WHITE)
	Console.print_at(9, 26, "N = Summon Named Pet", Ink.WHITE)
	Console.print_at(10, 26, "H = Summon Horde", Ink.WHITE)
	Console.print_at(11, 26, "Z = Carnage True", Ink.WHITE)
	Console.print_at(12, 26, "z = Zap (Wizard Bolt)", Ink.WHITE)
	Console.print_at(14, 26, "General Commands", Ink.RED)
	Console.print_at(15, 26, "================", Ink.WHITE)
	Console.print_at(17, 26, "\" = Generate spoilers", Ink.WHITE)
	Console.print_at(18, 26, "d = Detect All", Ink.WHITE)
	Console.print_at(19, 26, "m = Map Area", Ink.WHITE)
	Console.print_at(20, 26, "w = Wizard Light", Ink.WHITE)
	Console.print_at(4, 51, "Object Commands", Ink.RED)
	Console.print_at(5, 51, "===============", Ink.RED)
	Console.print_at(7, 51, "c = Create Item", Ink.WHITE)
	Console.print_at(8, 51, "C = Create Named artifact", Ink.WHITE)
	Console.print_at(9, 51, "f = Identify Fully", Ink.WHITE)
	Console.print_at(10, 51, "g = Generate Good Object", Ink.WHITE)
	Console.print_at(11, 51, "i = Identify Pack", Ink.WHITE)
	Console.print_at(12, 51, "l = Learn About Objects", Ink.WHITE)
	Console.print_at(13, 51, "o = Object Editor", Ink.WHITE)
	Console.print_at(14, 51, "v = Generate Very Good Object", Ink.WHITE)
	Console.print_centred(43, "Hit any key to continue", Ink.ORANGE)
	User.get_key()
	Console.restore_screen()
	Console.set_background(BackgroundImage.IN_GAME)
	Console.full_screen_overlay = false


func command_wiz_change() -> void:
	var prompt: String
	var default: String
	var result: Dictionary[bool, String]
	var value: int
	for i: int in 6:
		prompt = "%s (3-118): " % [Tables.ABILITY_NAMES[i]]
		default = "%d" % [player.ability_scores[i].innate_max]
		Console.print_line_at(0, 0, prompt, Ink.WHITE)
		result = User.get_input(0, prompt.length(), default, 4)
		if not (result.has(true)):
			return
		value = result[true] as int
		value = clampi(value, 3, 118)
		player.ability_scores[i].innate = value
		player.ability_scores[i].innate_max = value
	prompt = "Gold: "
	default = "%d" % [player.gold]
	Console.print_line_at(0, 0, prompt, Ink.WHITE)
	result = User.get_input(0, prompt.length(), default, 9)
	if not (result.has(true)):
		return
	value = result[true] as int
	player.gold = value
	prompt = "Experience: "
	default = "%d" % [player.max_experience_gained]
	Console.print_line_at(0, 0, prompt, Ink.WHITE)
	result = User.get_input(0, prompt.length(), default, 9)
	if not (result.has(true)):
		return
	value = result[true] as int
	player.experience_points = value
	player.max_experience_gained = value
	player.check_experience()
	command_wiz_redraw()

func command_wiz_zap() -> void:
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		if monster.monster_type == null:
			continue
		if monster.distance_from_player <= DungeonLevel.MAX_SIGHT_RADIUS:
			dungeon_level.dungeon_monsters.delete_monster_by_index(i, true)


func command_wiz_bolt() -> void:
	var flg: int = ProjectionFlag.STOP | ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	var col_target: int = player.map_col + (99 * dungeon_level.keypad_direction_col_offset[dir])
	var row_target: int = player.map_col + (99 * dungeon_level.keypad_direction_row_offset[dir])
	if dir == 5 and target_engine.target_okay():
		flg &= ~ProjectionFlag.STOP
		col_target = savegame.target_col
		row_target = savegame.target_row
	savegame.spell_effects_handler.project(0, 0, row_target, col_target, 1000000, ProjectileWizardBolt.new(savegame.spell_effects_handler), flg)


func command_wiz_cure_all() ->void:
	savegame.spell_effects_handler.remove_all_curses()
	player.restore_ability_score(Ability.STRENGTH)
	player.restore_ability_score(Ability.INTELLIGENCE)
	player.restore_ability_score(Ability.WISDOM)
	player.restore_ability_score(Ability.CONSTITUTION)
	player.restore_ability_score(Ability.DEXTERITY)
	player.restore_ability_score(Ability.CHARISMA)
	player.restore_level()
	player.health = player.max_health
	player.fractional_health = 0
	player.power = player.max_power
	player.fractional_power = 0
	player.set_timed_blindness(0)
	player.set_timed_confusion(0)
	player.set_timed_poison(0)
	player.set_timed_fear(0)
	player.set_timed_paralysis(0)
	player.set_timed_hallucinations(0)
	player.set_timed_stun(0)
	player.set_timed_bleeding(0)
	player.set_timed_slow(0)
	player.set_food(Constants.FOOD_GORGED - 1)
	command_wiz_redraw()

func command_wiz_redraw()->void:
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_REMOVE_VIEW | UpdateFlags.UPDATE_REMOVE_VIEW)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_WIPE | RedrawFlags.REDRAW_BASIC | RedrawFlags.REDRAW_EXTRA | RedrawFlags.REDRAW_MAP | RedrawFlags.REDRAW_EQUIPPY)
	savegame.notice_update_redraw()


func command_wiz_bamf()->void:
	if savegame.target_who == 0:
		return
	savegame.spell_effects_handler.teleport_player_to(savegame.target_row, savegame.target_col)


func command_wiz_jump()->void:
	if User.command_argument <= 0:
		var prompt: String = "Jump to level (0-%d): " % [savegame.current_dungeon.max_level]
		var default: String = "%d" % [savegame.current_depth]
		Console.print_line_at(0, 0, prompt, Ink.WHITE)
		var result: Dictionary[bool, String] = User.get_input(0, prompt.length(), default, 4)
		if not (result.has(true)):
			return
		User.command_argument = result[true] as int
		User.command_argument = clampi(User.command_argument, 0, savegame.current_dungeon.max_level)
	if User.command_argument == 0:
		Profile.instance.message_print("You jump to the surface.")
	else:
		Profile.instance.message_print("You jump to dungeon level %d." % [User.command_argument])
	savegame.is_auto_save = true
	savegame.command_save_game()
	savegame.is_auto_save = false
	savegame.current_depth = User.command_argument
	savegame.new_level_flag = true


func command_wiz_create_item() -> void:
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	var item_type_index: int = wiz_create_item_type()
	Console.restore_screen()
	Console.full_screen_overlay = false
	Console.set_background(BackgroundImage.IN_GAME)
	if item_type_index == 0:
		return
	var item: Item = Item.new()
	item.assign_item_type(savegame.item_types.types[item_type_index])
	item.apply_magic(savegame.total_difficulty(), false, false, false)
	dungeon_level.dungeon_items.drop_near(item, -1, player.map_row, player.map_col)


func wiz_create_item_type() -> int:
	var num: int
	var row: int
	var col: int
	var ch: String
	var choice: Array[int]
	var choice_key: Array[String]
	Console.clear_screen()
	num = 0
	for i: int in ItemCategoryDescription.all.size():
		row = 2 + (i % 26)
		col = 30 * (i / 26)
		ch = char(ord(HEADERS[i / 26]) + (i % 26))
		Console.print_line_at(row, col, "[%s] %s" % [ch, ItemCategoryDescription.all[i].name], Ink.WHITE)
		choice_key.append(ch)
		num += 1
	var max_num: int = num
	ch = User.get_simple_choice("Get what type of object? ")
	ch = User.strip_movement_keys(ch)
	if ch == "":
		return 0
	num = -1
	if ord(ch) >= ord(HEADERS[0]) and ord(ch) < ord(HEADERS[0]) + 26:
		num = ord(ch) - ord(HEADERS[0])
	if ord(ch) >= ord(HEADERS[1]) and ord(ch) < ord(HEADERS[1]) + 26:
		num = ord(ch) - ord(HEADERS[1]) + 26
	if ord(ch) >= ord(HEADERS[2]) and ord(ch) < ord(HEADERS[2]) + 16:
		num = ord(ch) - ord(HEADERS[2]) + 52
	if num < 0 or num >= max_num:
		return 0
	var item_category: int = ItemCategoryDescription.all[num].item_category
	var item_name: String = ItemCategoryDescription.all[num].name
	Console.clear_screen()
	num = 0
	for i: int in savegame.item_types.types.size():
		var item_type: ItemType = savegame.item_types.types[i]
		if item_type.item_category == item_category:
			if item_type.flags_3.is_set(ItemFlags3.INSTA_ART):
				continue
			row = 2 + (num % 26)
			col = 30 * (num / 26)
			ch = char(ord(HEADERS[num / 26]) + (num % 26))
			var buf: String = strip_name(i)
			Console.print_line_at(row, col, "[%s] %s" % [ch, buf], Ink.WHITE)
			choice.append (i)
			num += 1
	max_num = num
	ch = User.get_simple_choice("What kind of %s?" % [item_name])
	ch = User.strip_movement_keys(ch)
	if ch == "":
		return 0
	num = -1
	if ord(ch) >= ord(HEADERS[0]) and ord(ch) < ord(HEADERS[0]) + 26:
		num = ord(ch) - ord(HEADERS[0])
	if ord(ch) >= ord(HEADERS[1]) and ord(ch) < ord(HEADERS[1]) + 26:
		num = ord(ch) - ord(HEADERS[1]) + 26
	if ord(ch) >= ord(HEADERS[2]) and ord(ch) < ord(HEADERS[2]) + 16:
		num = ord(ch) - ord(HEADERS[2]) + 52
	if num < 0 or num >= max_num:
		return 0
	return choice[num]


func strip_name(item_type_index: int)->String:
	var item_type: ItemType = savegame.item_types.types[item_type_index]
	return item_type.name.replace("$","").replace("~","").replace("&","").strip_edges()


func command_wiz_debug() -> void:
	for monster_type:MonsterType in StaticData.monster_types:
		print (monster_type.name, "\t", monster_type.character, "\t", monster_type.colour)
	breakpoint
	pass
