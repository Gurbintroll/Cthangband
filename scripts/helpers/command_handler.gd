# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CommandHandler
extends RefCounted

const DANGER_FEELING_TEXT: Array[String] = [
	"You're not sure about this level yet",
	"You feel there is something special about this level.",
	"You nearly faint as horrible visions of death fill your mind",
	"This level looks very dangerous",
	"You have a very bad feeling",
	"You have a bad feeling",
	"You feel nervous",
	"You feel unsafe",
	"You don't like the look of this place",
	"This level looks reasonably safe",
	"What a boring place",
]
const TREASURE_FEELING_TEXT: Array[String] = [
	"You're not sure about this level yet.",
	"you feel it contains something special",
	"treasure galore!",
	"with a veritable hoard.",
	"powerful magic can be found here.",
	"there's magic in the air.",
	"there's wealth to be found.",
	"with significant treasure.",
	"there's not much of value here.",
	"with nothing of worth.",
	"what meagre pickings...",
]

var player: Player
var dungeon_level: DungeonLevel
var savegame: SaveGame


## Process the current command
func process_command() -> void:
	# Big honking match statement to call the correct function to handle the command
	match User.current_command:
		"Escape":
			# Escape = pop up menu
			command_popup_menu()
		" ", "Enter":
			# Space and enter do nothing, as they're likely to be "left over" from other commands
			pass
		"a":
			# a = aim wand
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_aim_wand(-999)
		"b":
			# b = browse book
			command_browse(-999)
		"c":
			# c = close door
			command_close()
		"d":
			# d = drop item
			command_drop()
		"e":
			# e = equip item
			command_equipment()
		"f":
			# f = fire ranged weapon
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_fire()
		"g":
			# g = pick up (get) item on floor
			command_stay(true)
		"h":
			# h = help
			command_manual()
		"i":
			# i = inventory
			command_inventory()
		"j":
			# j = jam a door closed
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_spike()
		"k":
			# k = kill (destroy) an item
			command_destroy()
		"l":
			# l = look around
			command_look()
		"m":
			# m = magic
			var handler: CastingHandler = CastingHandler.new(dungeon_level, player)
			handler.command_cast()
		"o":
			# o = open a door or chest
			command_open()
		"p":
			# p = use a mutant power
			command_mutant_power()
		"q":
			# q = quaff a potion
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_quaff_potion(-999)
		"r":
			# r = read a scroll
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_read_scroll(-999)
		"s":
			# s = search
			command_search()
		"t":
			# t = take of an item
			command_take_off()
		"u":
			# u = use a staff
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_use_staff(-999)
		"v":
			# v = throw item
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_throw(1)
		"w":
			# w = wield an item
			command_wield()
		"x":
			# x = examine an item
			command_examine()
		"z":
			# z = zap a rod
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_zap_rod(-999)
		"A":
			# A = activate an artifact
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_activate(-999)
		"B":
			# B = bash a door
			command_bash()
		"C":
			# C = view character
			command_view_character()
		"D":
			# D = disarm a trap
			command_disarm()
		"E":
			# E = eat food
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_eat_food(-999)
		"F":
			# F = (re)fill a torch/lamp
			var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
			handler.command_refill(-999)
		"H":
			# H = Show level feeling
			command_feeling(false)
		"J":
			# J = show journal
			command_journal()
		"K":
			# K = kill (destroy) all worthless items
			command_destroy_all()
		"L":
			# L = locate us
			command_locate()
		"M":
			# M = show map
			command_view_map()
		"O":
			# O = show one previous message
			command_message_one()
		"P":
			# P = show previous messages
			command_messages()
			Console.set_background(BackgroundImage.IN_GAME)
		"Q":
			# Q = quit game
			command_retire()
		"R":
			# R = rest
			command_rest()
		"S":
			# S = toggle auto-search
			command_toggle_search()
		"T":
			# T = tunnel
			command_tunnel()
		"V":
			# V = show version
			command_version()
		"W":
			# W = wizard command
			if player.is_wizard:
				var wizard: WizardCommandHandler = WizardCommandHandler.new(dungeon_level, player)
				wizard.command_wizard()
			else:
				var wizard: WizardCommandHandler = WizardCommandHandler.new(dungeon_level, player)
				wizard.command_wiz_mode()
		"+":
			# + = alter (dig/open/disarm) a tile
			command_alter()
		";":
			# ; = walk and pick up items
			command_walk(true)
		"-":
			# - = walk without picking up
			command_walk(false)
		".":
			# . = run
			command_run()
		",":
			# = stay still without picking up items
			command_stay(false)
		"_":
			# Enter a store
			command_store()
		"<":
			# < = go up stairs
			command_go_up()
		">":
			# > = go down stairs
			command_go_down()
		"*":
			# * = target
			command_target()
		"/":
			# / = query symbol
			command_query_symbol()
		"?":
			# ? = show command list
			command_command_list()
		_:
			Console.print_line_at(0, 0, "Type '?' for a list of commands.", Ink.WHITE)


func command_browse(item_index: int) -> void:
	var spell_index: int = 0
	var spells: Array[int]
	for i: int in 64:
		spells.append(0)
	# Make sure we can read
	if player.realm_1 == 0 and player.realm_2 == 0:
		Profile.instance.message_print("You cannot read books!")
		return
	# Get a book to read if we don't already have one
	Inventory.item_filter_useable_spell_book = true
	if item_index < 0:
		item_index = savegame.get_item("Browse which book? ", false, true, true)
		if item_index == 99:
			Inventory.item_filter_useable_spell_book = false
			return
		if item_index == -2:
			Profile.instance.message_print("You have no books that you can read.")
			Inventory.item_filter_useable_spell_book = false
			return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Check that the book is useable by the player
	Inventory.item_filter_useable_spell_book = true
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("You can't read that.")
		Inventory.item_filter_useable_spell_book = false
		return
	Inventory.item_filter_useable_spell_book = false
	var book_sub_category: int = item.sub_category
	savegame.notice_update_redraw()
	# Find all spells in the book and add them to the array
	for spell: int in 32:
		if (Tables.BOOK_SPELL_FLAGS[book_sub_category] & (1 << spell)) != 0:
			spells[spell_index] = spell
			spell_index += 1
	# Save the screen and overprint the spells in the book
	Console.save_screen()
	player.print_spells(spells, spell_index, 1, 20, Util.spellbook_to_realm(item.category))
	Console.clear_row(0)
	# Wait for a keypress and re-load the screen
	Console.print_at(0, 23, "[Press any key to continue]", Ink.WHITE)
	User.get_key()
	Console.restore_screen()


func command_go_down() -> void:
	var is_trap_door: bool = false
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	if tile.feature_type.category == FloorTileTypeCategory.TRAP_DOOR:
		is_trap_door = true
	# Need to be on a staircase or trapdoor
	if tile.feature_type.name != "DownStair" and not is_trap_door:
		Profile.instance.message_print("I see no down staircase here.")
		savegame.energy_use = 0
		return
	# Going onto a new level takes no energy, so the monsters on that level don't get to
	# move before us
	savegame.energy_use = 0
	if is_trap_door:
		Profile.instance.message_print("You deliberately jump through the trap door.")
	else:
		# If we're on the surface, enter the relevant dungeon
		if savegame.current_depth == 0:
			savegame.current_dungeon = savegame.island.get_region(player.wilderness_row, player.wilderness_col).dungeon
			Profile.instance.message_print("You enter %s." % [savegame.current_dungeon.name])
		else:
			Profile.instance.message_print("You enter a maze of down staircases.")
		# Save the game, just in case
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
	# If we're in a tower, a down staircase reduces our level number
	if savegame.current_dungeon.is_tower:
		var stair_length: int = Rng.roll_die(5)
		if stair_length > savegame.current_depth:
			stair_length = 1
		savegame.current_depth -= stair_length
		if savegame.current_depth < 0:
			savegame.current_depth = 0
		# If we left the dungeon, remember where we are
		if stair_length == 0:
			player.wilderness_row = savegame.current_dungeon.wild_row
			player.wilderness_col = savegame.current_dungeon.wild_col
			savegame.came_from = LevelStart.START_STAIRS
	else:
		# We're not in a tower, so a down staircase increases our level number
		var stair_length: int = Rng.roll_die(5)
		if stair_length > savegame.current_depth:
			stair_length = 1
		# Check if we're about to go past a quest level
		for i: int in stair_length:
			savegame.current_depth += 1
			if savegame.quests.is_quest_level(savegame.current_depth):
				# Stop on the quest level
				break
		# Don't go past the max dungeon level
		if savegame.current_depth > savegame.current_dungeon.max_level:
			savegame.current_depth = savegame.current_dungeon.max_level
		# From the surface we always go to the first level
		if savegame.current_depth == 0:
			savegame.current_depth += 1
	# We need a new level
	savegame.new_level_flag = true
	if not is_trap_door:
		# Create an up staircase if we went down a staircase
		savegame.create_up_stair = true


func command_go_up() -> void:
	# We need to actually be on an up staircase
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	if tile.feature_type.name != "UpStair":
		Profile.instance.message_print("I see no up staircase here.")
		savegame.energy_use = 0
		return
	# Use no energy, so monsters in the new level don't get to go first
	savegame.energy_use = 0
	# If we're outside then we must be entering a tower
	if savegame.current_depth == 0:
		savegame.current_dungeon = savegame.island.get_region(player.wilderness_row, player.wilderness_col).dungeon
		Profile.instance.message_print("You enter %s." % [savegame.current_dungeon.name])
	else:
		Profile.instance.message_print("You enter a maze of up staircases.")
	# Autosave, just in case
	savegame.is_auto_save = true
	savegame.command_save_game()
	savegame.is_auto_save = false
	# In a tower, going up increases our level number
	if savegame.current_dungeon.is_tower:
		var stair_length: int = Rng.roll_die(5)
		if stair_length > savegame.current_depth:
			stair_length = 1
		# Make sure we don't go past a quest level
		for i: int in stair_length:
			savegame.current_depth += 1
			if savegame.quests.is_quest_level(savegame.current_depth):
				break
		# Make sure we don't go deeper than the dungeon depth
		if savegame.current_depth > savegame.current_dungeon.max_level:
			savegame.current_depth = savegame.current_dungeon.max_level
	else:
		# We're not in a tower, so going up decreases our level number
		var j: int = Rng.roll_die(5)
		if j > savegame.current_depth:
			j = 1
		savegame.current_depth -= j
		if savegame.current_depth < 0:
			savegame.current_depth = 0
		if savegame.current_depth == 0:
			player.wilderness_col = savegame.current_dungeon.wild_col
			player.wilderness_row = savegame.current_dungeon.wild_row
			savegame.came_from = LevelStart.START_STAIRS
	savegame.new_level_flag = true
	savegame.create_down_stair = true


func command_view_map() -> void:
	var cursor_location: Location
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.clear_screen()
	# If we're on the surface, display the island map
	if savegame.current_depth == 0:
		Console.set_background(BackgroundImage.WILD_MAP)
		cursor_location = savegame.display_wild_map()
	else:
		# We're not on the surface, so draw the level map
		Console.set_background(BackgroundImage.MAP)
		cursor_location = dungeon_level.display_map()
	# Give us a prompt, and display the cursor in the player's location
	Console.show_cursor(cursor_location.row, cursor_location.col)
	# Wait for a keypress, and restore the screen (looking at the map takes no time)
	Console.any_key(44)
	Console.hide_cursor()
	Console.restore_screen()
	Console.full_screen_overlay = false
	Console.set_background(BackgroundImage.IN_GAME)


func command_popup_menu() -> void:
	var menu_items: Array[String] = ["Resume Game", "Cthangpedia", "Options", "Save & Menu", "Save & Exit"]
	var menu: PopupTextMenu = PopupTextMenu.new(menu_items)
	Console.play_sound("MenuSelect")
	var result: String = menu.show()
	match result:
		"Resume Game":
			Console.play_sound("MenuAccept")
			return
		"Cthangpedia":
			Console.play_sound("MenuAccept")
			Console.show_cthangpedia()
			return
		"Options":
			Console.play_sound("MenuAccept")
			var options: OptionsScreen = OptionsScreen.new()
			options.show()
			Console.set_background(BackgroundImage.IN_GAME)
			return
		"Save & Menu":
			Console.play_sound("MenuAccept")
			SaveGame.instance.playing = false
			return
		"Save & Exit":
			Console.play_sound("MenuAccept")
			SaveGame.instance.playing = false
			Console.exit_to_desktop = true
			return
	Console.play_sound("MenuSelect")


# Walk a single step (also called when running)
func command_walk(pick_up: bool) -> void:
	var disturb: bool = false
	# We might be walking or running more than one step
	if User.command_argument > 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir >= 0:
		savegame.energy_use = 100
		savegame.command_engine.move_player(dir, pick_up)
		disturb = true
	if not disturb:
		savegame.disturb(false)


## Show the manual
func command_manual() -> void:
	Console.show_cthangpedia()


## Show the feeling and optionally our quest status
func command_feeling(feeling_only: bool) -> void:
	# Some sanity checks
	dungeon_level.danger_feeling = clampi(dungeon_level.danger_feeling, 0, 10)
	dungeon_level.treasure_feeling = clampi(dungeon_level.treasure_feeling, 0, 10)
	if savegame.current_depth <= 0:
		# If we need to say where we are, do so
		if not feeling_only:
			var region: WildernessRegion = savegame.island.get_region(player.wilderness_row, player.wilderness_col)
			if region.town != null:
				Profile.instance.message_print("You are in %s." % [region.town.name])
			elif region.dungeon != null:
				Profile.instance.message_print("You are outside %s." % [region.dungeon.name])
			else:
				Profile.instance.message_print("You are wandering around outside.")
		# If we're not in a dungeon, there's no feeling to be had
		return
	# If we need to say where we are, do so
	if not feeling_only:
		Profile.instance.message_print("You are in %s." % [savegame.current_dungeon.name])
		if savegame.quests.is_quest_level(savegame.current_depth):
			savegame.quests.print_quest_message()
	# Special feeling overrides the normal two-part feeling
	if dungeon_level.danger_feeling == 1 or dungeon_level.treasure_feeling == 1:
		Profile.instance.message_print(DANGER_FEELING_TEXT[1] if player.game_time.level_feel else DANGER_FEELING_TEXT[0])
	else:
		# Make the two-part feeling make a bit more sense by using the correct conjunction
		var conjunction: String = ", and "
		if absi(dungeon_level.danger_feeling - dungeon_level.treasure_feeling) > 3:
			conjunction = ", but "
		var message: String = DANGER_FEELING_TEXT[dungeon_level.danger_feeling] + conjunction + TREASURE_FEELING_TEXT[dungeon_level.treasure_feeling]
		Profile.instance.message_print(message if player.game_time.level_feel else DANGER_FEELING_TEXT[0])


func command_inventory() -> void:
	# We're not viewing equipment
	savegame.viewing_equipment = false
	Console.save_screen()
	# We want to see everything
	savegame.item_filter_all = true
	player.inventory.show_inventory()
	savegame.item_filter_all = false
	# Get a new command
	var weight_summary: String = "Inventory: carrying %d.%d pounds (%d%% of capacity). Command: " % [player.weight_carried / 10, player.weight_carried % 10, player.weight_carried * 100 / (player.ability_scores[Ability.STRENGTH].str_carrying_capacity() * 100 / 2)]
	Console.print_line_at(0, 0, weight_summary, Ink.WHITE)
	User.queued_command = User.get_key()
	Console.restore_screen()
	# Display details if the player wants
	if User.queued_command == "Escape":
		User.queued_command = ""
		savegame.item_display_column = 50
	else:
		savegame.viewing_item_list = true


func command_equipment() -> void:
	# We're viewing equipment
	savegame.viewing_equipment = true
	Console.save_screen()
	# We're interested in seeing everything
	savegame.item_filter_all = true
	player.inventory.show_equipment()
	savegame.item_filter_all = false
	# Get a command
	var weight_summary: String = "Equipment: carrying %d.%d pounds (%d%% of capacity). Command: " % [player.weight_carried / 10, player.weight_carried % 10, player.weight_carried * 100 / (player.ability_scores[Ability.STRENGTH].str_carrying_capacity() * 100 / 2)]
	Console.print_line_at(0, 0, weight_summary, Ink.WHITE)
	User.queued_command = User.get_key()
	Console.restore_screen()
	# Display details if the player wants
	if User.queued_command == "Escape":
		User.queued_command = ""
		savegame.item_display_column = 50
	else:
		savegame.viewing_item_list = true


func command_drop() -> void:
	var amount: int = 1
	# Get an item from the inventory/equipment
	var item_index: int = savegame.get_item("Drop which item? ", true, true, false)
	# User escaped from item selection
	if item_index == 99:
		return
	# player has nothing to drop
	if item_index == -2:
		Profile.instance.message_print("You have nothing to drop.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Can't drop nothing
	if item.item_type == null:
		return
	# Can't drop a cursed item
	if item_index >= InventorySlot.MELEE_WEAPON and item.is_cursed():
		Profile.instance.message_print("Hmmm, it seems to be cursed.")
		return
	# It's a stack, so find out how many to drop
	if item.count > 1:
		amount = User.get_quantity("", item.count, true)
		if amount <= 0:
			return
	# Dropping things takes half a turn
	SaveGame.instance.energy_use = 50
	# Drop it
	player.inventory.inven_drop(item_index, amount)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EQUIPPY)


func command_take_off() -> void:
	# Get the item to take off
	var item_index: int = SaveGame.instance.get_item("Take off which item? ", true, false, false)
	# User escaped without selecting something
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You are not wearing anything to take off.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Can't take of cursed items
	if item.is_cursed():
		Profile.instance.message_print("Hmmm, it seems to be cursed.")
		return
	# Take off the item
	SaveGame.instance.energy_use = 50
	player.inventory.inven_take_off(item_index, 255)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EQUIPPY)


func command_wield() -> void:
	var wield_phrase: String
	var item_name: String
	# Only interested in wearable items
	savegame.item_filter_function = item_filter_wearable
	var item_index: int = savegame.get_item("Wear/Wield which item? ", false, true, true)
	# If we escaped the prompt instead of choosing an item, abort the action
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing you can wear or wield.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Find the correct item slot
	var slot: int = player.inventory.wield_slot(item)
	# Can't replace a cursed item
	if player.inventory.items[slot].is_cursed():
		item_name = player.inventory.items[slot].description(false, 0)
		Profile.instance.message_print("The %s you are %s appears to be cursed." % [item_name, player.describe_wield_location(slot)])
		return
	# If we know the item to be cursed, confirm its wearing
	if item.is_cursed() and (item.is_known() or item.identify_flags.is_set(IdentifyFlags.SENSED)):
		item_name = item.description(false, 0)
		var dummy: String = "Really use the %s {cursed}?" % [item_name]
		if not User.get_check(dummy):
			return
	# Use some energy
	savegame.energy_use = 100
	# Pull one item out of the item stack
	var worn_item:Item = item.get_copy()
	worn_item.count = 1
	# Reduce the count of the item stack accordingly
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_optimise(0 - item_index)
	# Take off the old item
	item = player.inventory.items[slot]
	if item.item_type != null:
		player.inventory.inven_take_off(slot, 255)
	# Put the item into the wield slot
	player.inventory.items[slot] = worn_item
	# Add the weight of the item
	player.weight_carried += worn_item.weight
	# Inform us what we did
	if slot == InventorySlot.MELEE_WEAPON:
		wield_phrase = "You are wielding"
	elif slot == InventorySlot.RANGED_WEAPON:
		wield_phrase = "You are shooting with"
	elif slot == InventorySlot.LIGHT_SOURCE:
		wield_phrase = "Your light source is"
	elif slot == InventorySlot.DIGGER:
		wield_phrase = "You are digging with"
	else:
		wield_phrase = "You are wearing"
	item_name = worn_item.description(true, 3)
	Profile.instance.message_print("%s %s (%s)." % [wield_phrase, item_name, Util.index_to_label(slot)])
	# Let us know if it's cursed
	if worn_item.is_cursed():
		Profile.instance.message_print("Oops! It feels deathly cold!")
		worn_item.identity_flags.set_bits(IdentifyFlags.SENSED)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EQUIPPY)


func item_filter_wearable(item: Item) -> bool:
	return player.inventory.wield_slot(item) >= InventorySlot.MELEE_WEAPON


func command_retire() -> void:
	# If we're a winner it's a simple question with a more positive connotation
	if player.is_winner:
		if not User.get_check("Do you want to retire? "):
			return
	else:
		# If we're not a winner, only ask if we're not also a wizard - giving up a wizard
		# character doesn't need a prompt/confirmation
		if not player.is_wizard:
			if not User.get_check("Do you really want to give up? "):
				return
			# Require a confirmation to make sure the player doesn't accidentally give up a
			# long-running character
			Console.print_line_at(0, 0, "Type the '@' sign to give up (this character will no longer be playable): ", Ink.WHITE)
			var key: String = User.get_key()
			Console.clear_row(0)
			if key != "@":
				return
	# Assuming whe player didn't give up, "kill" the character by quitting
	savegame.playing = false
	player.is_dead = true
	savegame.died_from = "quitting"


func command_run() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# Can't run if we're confused
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused!")
		return
	# Get a direction if we don't already have one
	var dir: int = target_engine.get_direction_no_aim()
	if dir != -1:
		# If we don't have a distance, assume we'll run for 1,000 steps
		savegame.running = User.command_argument if User.command_argument != 0 else 1000
		# Run one step in the chosen direction
		savegame.command_engine.run_one_step(dir)


func command_stay(pick_up: bool) -> void:
	# We might be staying still for multiple turns
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Standing still takes a turn
	savegame.energy_use = 100
	# Periodically search if we're not actively in search mode
	if player.skill_search_frequency >= 50 or Rng.roll_die(50 - player.skill_search_frequency) == 1:
		savegame.command_engine.search()
	# Always search if we are actively in search mode
	if player.is_searching:
		savegame.command_engine.search()
	# Pick up items if we should
	savegame.command_engine.pick_up_items(pick_up)
	# If we're in a shop doorway, enter the shop
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	if tile.feature_type.is_shop:
		savegame.disturb(false)
		User.queued_command = '_'


func command_search() -> void:
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	savegame.energy_use = 100
	savegame.command_engine.search()


func command_message_one() -> void:
	Console.print_line_at(0, 0, ">" + Profile.instance.get_message(0), Ink.WHITE)


func command_messages() -> void:
	var message_number: int = Profile.instance.get_message_count()
	var index: int = 0
	var horizontal_offset: int = 0
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	# Infinite loop showing a page of messages from the index
	while true:
		# Clear the screen
		Console.clear_screen()
		# Print the messages
		var max_row: int = mini(40, message_number - index)
		for row: int in max_row:
			var msg: String = Profile.instance.get_message(index + row)
			msg = msg.substr(horizontal_offset) if msg.length() >= horizontal_offset else ""
			Console.print_at(41 - row, 0, msg, Ink.WHITE)
		# Get a command
		Console.print_at(0, 0, "Message Recall (%d-%d of %d), Offset %d" % [index + 1, index + max_row, message_number, horizontal_offset], Ink.WHITE)
		Console.print_at(43, 0, "[Press 'p' for older, 'n' for newer, <dir> to scroll, or ESCAPE]", Ink.WHITE)
		var key_code: String = User.get_key()
		if key_code == "Escape":
			# Break out of the infinite loop
			break
		if key_code == "Left":
			horizontal_offset = horizontal_offset if horizontal_offset >= 40 else 0
			continue
		if key_code == "Right":
			horizontal_offset += 40
			continue
		if key_code == "Up" or key_code == "Enter":
			if index + 1 < message_number:
				index += 1
		if key_code == "p" or key_code == " ":
			if index + 40 < message_number:
				index += 40
		if key_code == "n":
			index = index - 40 if index >= 40 else 0
		if key_code == "Down":
			index = index - 1 if index >= 1 else 0
	# Tidy up after ourselves
	Console.restore_screen()
	Console.full_screen_overlay = false


func command_open() -> void:
	var disturb: bool = false
	# Check if there's only one thing we can open
	var location: Location = Location.new()
	var num_doors: int = savegame.command_engine.count_closed_doors(location)
	var num_chests: int = savegame.command_engine.count_chests(location, false)
	if num_doors != 0 or num_chests != 0:
		var too_many: bool = (num_doors != 0 and num_chests != 0) or num_doors > 1 or num_chests > 1
		if not too_many:
			# There's only one thing we can open, so assume we mean that thing
			User.command_direction = dungeon_level.coords_to_dir(location.row, location.col)
	# If we're repeatedly trying to open something, mark the repeat flag
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# If we don't already have a direction, prompt for one
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir != -1:
		var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		var item_index: int = dungeon_level.chest_check(row, col)
		# Make sure there is something to open in the direction we chose
		if not tile.feature_type.is_closed_door and item_index == 0:
			Profile.instance.message_print("You see nothing there to open.")
		# Can't open something if there's a monster in the way
		elif tile.monster_index != 0:
			savegame.energy_use = 100
			Profile.instance.message_print("There is a monster in the way!")
			savegame.commandengine.player_attack_monster(row, col)
		# Open the chest or door
		elif item_index != 0:
			Console.play_sound("OpenDoor")
			disturb = savegame.command_engine.open_chest(row, col, item_index)
		else:
			disturb = savegame.command_engine.open_door(row, col)
	if not disturb:
		savegame.disturb(false)


func command_close() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var coord: Location = Location.new()
	var disturb: bool = false
	# If there's only one door, assume we mean that one and don't ask for a direction
	if savegame.command_engine.count_open_doors(coord) == 1:
		User.command_direction = dungeon_level.coords_to_dir(coord.row, coord.col)
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Test the location to close
	var dir: int = target_engine.get_direction_no_aim()
	if dir != -1:
		var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		# Can only close actual open doors
		if tile.feature_type.category != FloorTileTypeCategory.OPEN_DOORWAY:
			Profile.instance.message_print("You see nothing there to close.")
		# Can't close if there's a monster in the way
		elif tile.monster_index != 0:
			savegame.energy_use = 100
			Profile.instance.message_print("There is a monster in the way!")
			savegame.command_engine.player_attack_monster(row, col)
		# Actually close the door
		else:
			Console.play_sound("CloseDoor")
			disturb = savegame.command_engine.close_door(row, col)
	if not disturb:
		savegame.disturb(false)


func command_command_list() -> void:
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.clear_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	Console.print_at(1, 1, "Numpad", Ink.YELLOW)
	Console.print_at(3, 1, "7 8 9", Ink.WHITE)
	Console.print_at(4, 1, " \\|/", Ink.WHITE)
	Console.print_at(5, 1, "4- -6 = Move", Ink.WHITE)
	Console.print_at(6, 1, " /|\\    (+Shift = run)", Ink.WHITE)
	Console.print_at(7, 1, "1 2 3", Ink.WHITE)
	Console.print_at(8, 1, "5 = Stand still", Ink.WHITE)
	Console.print_at(10, 1, "Other Symbols", Ink.YELLOW)
	Console.print_at(12, 1, ". = Run", Ink.WHITE)
	Console.print_at(13, 1, ", = Walk (no pick-up)", Ink.WHITE)
	Console.print_at(14, 1, "< = Go up stairs", Ink.WHITE)
	Console.print_at(15, 1, "> = Go down stairs", Ink.WHITE)
	Console.print_at(16, 1, "+ = Auto-alter a space", Ink.WHITE)
	Console.print_at(17, 1, "* = Target a creature", Ink.WHITE)
	Console.print_at(18, 1, "/ = Identify a symbol", Ink.WHITE)
	Console.print_at(19, 1, "? = Command list", Ink.WHITE)
	Console.print_at(21, 1, "Esc = Save and quit", Ink.WHITE)
	Console.print_at(1, 25, "Without Shift", Ink.YELLOW)
	Console.print_at(3, 25, "a = Aim a wand", Ink.WHITE)
	Console.print_at(4, 25, "b = Browse a book", Ink.WHITE)
	Console.print_at(5, 25, "c = Close a door", Ink.WHITE)
	Console.print_at(6, 25, "d = Drop object", Ink.WHITE)
	Console.print_at(7, 25, "e = Show equipment", Ink.WHITE)
	Console.print_at(8, 25, "f = Fire a missile weapon", Ink.WHITE)
	Console.print_at(9, 25, "g = Get (pick up) object", Ink.WHITE)
	Console.print_at(10, 25, "h = View game help", Ink.WHITE)
	Console.print_at(11, 25, "i = Show Inventory", Ink.WHITE)
	Console.print_at(12, 25, "j = Jam spike in a door", Ink.WHITE)
	Console.print_at(13, 25, "k = Destroy an item", Ink.WHITE)
	Console.print_at(14, 25, "l = Look around", Ink.WHITE)
	Console.print_at(15, 25, "m = Cast spell/Use talent", Ink.WHITE)
	Console.print_at(16, 25, "n =", Ink.WHITE)
	Console.print_at(17, 25, "o = Open a door/chest", Ink.WHITE)
	Console.print_at(18, 25, "p = Mutant/Ancestral power", Ink.WHITE)
	Console.print_at(19, 25, "q = Quaff a potion", Ink.WHITE)
	Console.print_at(20, 25, "r = Read a scroll", Ink.WHITE)
	Console.print_at(21, 25, "s = Search for traps", Ink.WHITE)
	Console.print_at(22, 25, "t = Take off an item", Ink.WHITE)
	Console.print_at(23, 25, "u = Use a staff", Ink.WHITE)
	Console.print_at(24, 25, "v = Throw object", Ink.WHITE)
	Console.print_at(25, 25, "w = Wield/wear an item", Ink.WHITE)
	Console.print_at(26, 25, "x = Examine an object", Ink.WHITE)
	Console.print_at(27, 25, "y =", Ink.WHITE)
	Console.print_at(28, 25, "z = Zap a rod", Ink.WHITE)
	Console.print_at(1, 52, "With Shift", Ink.YELLOW)
	Console.print_at(3, 52, "A = Activate an artifact", Ink.WHITE)
	Console.print_at(4, 52, "B = Bash a stuck door", Ink.WHITE)
	Console.print_at(5, 52, "C = View your character", Ink.WHITE)
	Console.print_at(6, 52, "D = Disarm a trap", Ink.WHITE)
	Console.print_at(7, 52, "E = Eat some food", Ink.WHITE)
	Console.print_at(8, 52, "F = Fuel a light source", Ink.WHITE)
	Console.print_at(10, 52, "H = How you feel here", Ink.WHITE)
	Console.print_at(12, 52, "J = View your journal", Ink.WHITE)
	Console.print_at(13, 52, "K = Destroy trash objects", Ink.WHITE)
	Console.print_at(14, 52, "L = Locate player", Ink.WHITE)
	Console.print_at(15, 52, "M = View the map", Ink.WHITE)
	Console.print_at(17, 52, "O = Show last message", Ink.WHITE)
	Console.print_at(18, 52, "P = Show previous messages", Ink.WHITE)
	Console.print_at(19, 52, "Q = Quit (Retire character)", Ink.WHITE)
	Console.print_at(20, 52, "R = Rest", Ink.WHITE)
	Console.print_at(21, 52, "S = Auto-search on/off", Ink.WHITE)
	Console.print_at(22, 52, "T = Tunnel", Ink.WHITE)
	Console.print_at(24, 52, "V = Version info", Ink.WHITE)
	if player.is_wizard:
		Console.print_at(25, 52, "W = Wizard command", Ink.WHITE)
	Console.any_key(44)
	Console.restore_screen()
	Console.set_background(BackgroundImage.IN_GAME)
	Console.full_screen_overlay = false


func command_target() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	if target_engine.target_set(TargetEngine.TARGET_KILL):
		Profile.instance.message_print("Target Selected." if savegame.target_who > 0 else "Location Targeted.")
	else:
		Profile.instance.message_print("Target Aborted.")


func command_look() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	if target_engine.target_set(TargetEngine.TARGET_LOOK):
		Profile.instance.message_print("Target Selected." if savegame.target_who > 0 else "Location Targeted.")


func command_view_character() -> void:
	# Save the current screen
	Console.full_screen_overlay = true
	Console.save_screen()
	Console.set_background(BackgroundImage.PAPER)
	# Load the character viewer
	var character_viewer: CharacterViewer = CharacterViewer.new(player)
	while true:
		character_viewer.display_player()
		Console.print_centred(43, "[Press 'c' to change name, or ESC]", Ink.ORANGE)
		var key_press: String = User.get_key()
		# Escape breaks us out of the loop
		if key_press == "Escape":
			break
		# 'c' changes name
		if key_press == 'c' or key_press == 'C':
			player.input_player_name()
		Profile.instance.message_print()
	# Restore the screen
	Console.set_background(BackgroundImage.IN_GAME)
	Console.restore_screen()
	Console.full_screen_overlay = false
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_WIPE | RedrawFlags.REDRAW_BASIC | RedrawFlags.REDRAW_EXTRA | RedrawFlags.REDRAW_MAP | RedrawFlags.REDRAW_EQUIPPY)
	savegame.notice_update_redraw()


func command_disarm() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var disturb: bool = false
	var coord: Location = Location.new()
	var trap_count: int = savegame.command_engine.count_known_traps(coord)
	var chest_count: int = savegame.command_engine.count_chests(coord, true)
	# count the possible traps and chests we might want to disarm
	if trap_count != 0 or chest_count != 0:
		var too_many: bool = (trap_count != 0 and chest_count != 0) or trap_count > 1 or chest_count > 1
		# If only one then we have our target
		if not too_many:
			User.command_direction = dungeon_level.coords_to_dir(coord.row, coord.col)
	# We might be repeating the command
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Get a direction if we don't already have one
	var dir: int = target_engine.get_direction_no_aim()
	if dir >= 0:
		var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		# Check for a chest
		var item_index: int = dungeon_level.chest_check(row, col)
		if not tile.feature_type.IsTrap and item_index == 0:
			Profile.instance.message_print("You see nothing there to disarm.")
		# Can't disarm with a monster in the way
		elif tile.monster_index != 0:
			Profile.instance.message_print("There is a monster in the way!")
			savegame.command_engine.player_attack_monster(row, col)
		# Disarm the chest or trap
		elif item_index != 0:
			disturb = savegame.command_engine.disarm_chest(row, col, item_index)
		else:
			disturb = savegame.command_engine.disarm_trap(row, col, dir)
	if (!disturb):
		savegame.disturb(false)


func command_rest() -> void:
	if User.command_argument <= 0:
		const prompt: String = "Rest (0-9999, '*' for HP/PP, '&' as needed): "
		Console.print_line_at(0, 0, prompt, Ink.WHITE)
		var result: Dictionary[bool, String] = User.get_input(0, prompt.length(), "&", 4)
		Console.clear_row(0)
		var choice: String
		if result.has(true):
			choice = result[true]
		else:
			return
		# Default to resting until we're fine
		if choice == "":
			choice = "&"
		# -2 means rest until we're fine
		if choice[0] == "&":
			User.command_argument = -2
		# -1 means rest until we're at full health, but don't worry about waiting for other
		# status effects to go away
		elif choice[0] == "*":
			User.command_argument = -1
		else:
			# A number means rest for that many turns
			User.command_argument = int(choice)
			# The player might not have put a number in - so abandon if they didn't
			if User.command_argument <= 0:
				return
	# Can't rest for more than 9999 turns
	if User.command_argument > 9999:
		User.command_argument = 9999
	# Resting takes at least one turn (we'll also be skipping future turns)
	savegame.energy_use = 100
	savegame.resting = User.command_argument
	player.is_searching = false
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
	savegame.notice_update_redraw()


func command_store() -> void:
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	# Make sure we're actually on a shop tile
	if not tile.feature_type.is_shop:
		Profile.instance.message_print("You see no Stores here.")
		return
	var which: Store = savegame.get_which_store()
	# We can't enter a house unless we own it
	if which.store_type == StoreType.HOME and player.town_with_house != savegame.current_town.index:
		Profile.instance.message_print("The door is locked.")
		return
	# Switch from the normal game interface to the store interface
	dungeon_level.forget_light()
	dungeon_level.forget_view()
	Console.full_screen_overlay = true
	User.command_argument = 0
	User.command_repeat = 0
	User.queued_command = ""
	if which.store_type != StoreType.HOME:
		Console.play_sound("EnterShop")
	which.enter_store(player, self)


func command_study() -> void:
	var spell_type: String = "spell" if player.spellcasting.casting_type == CastingType.ARCANE else "prayer"
	# If we don't have a realm then we can't do anything
	if player.realm_1 == 0:
		Profile.instance.message_print("You cannot read books!")
		return
	# We can't learn spells if we're blind or confused
	if player.timed_blindness != 0:
		Profile.instance.message_print("You cannot see!")
		return
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused!")
		return
	# We can only learn new spells if we have spare slots
	if player.spare_spell_slots == 0:
		Profile.instance.message_print("You cannot learn any new %ss!" % [spell_type])
		return
	var plural: String = "" if player.spare_spell_slots == 1 else "s"
	Profile.instance.message_print("You can learn %d new %s%s." % [player.spare_spell_slots, spell_type, plural])
	Profile.instance.message_print("")
	# Get the spell books we have
	Inventory.item_filter_useable_spell_book = true
	var item_index: int = savegame.get_item("Study which book?", false, true, true)
	if item_index == 99:
		Inventory.item_filter_useable_spell_book = false
		return
	if item_index == -2:
		Profile.instance.message_print("You have no books that you can read.")
		Inventory.item_filter_useable_spell_book = false
		return
	Inventory.item_filter_useable_spell_book = false
	# Check each book
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var sub_category: int = item.sub_category
	var use_set_two: bool = item.category == Util.realm_to_spellbook(player.realm_2)
	savegame.notice_update_redraw()
	var spell_index: int
	# ARCANE casters can choose their spell
	if player.spellcasting.casting_type != CastingType.DIVINE:
		var casting_handler: CastingHandler = CastingHandler.new(dungeon_level, player)
		spell_index = casting_handler.get_spell("study", sub_category, false, use_set_two)
		if spell_index == -1:
			return
	else:
		# We need to choose a spell at random
		var k: int = 0
		var gift: int = -1
		# Gather the potential spells from the book
		for index: int in 32:
			if Tables.BOOK_SPELL_FLAGS[sub_category] & (1 << index) != 0:
				if not player.spell_okay(index, false, use_set_two):
					continue
				k += 1
				if Rng.roll_die(k) == 1:
					gift = index
		spell_index = gift
	# If we failed to get a spell, return
	if spell_index < 0:
		Profile.instance.message_print("You cannot learn any %ss from that book." % [spell_type])
		return
	if spell_index == 99:
		return
	# Learning a spell takes a turn (although that's not very relevant)
	savegame.energy_use = 100
	# Mark the spell as learned
	var spell: Spell = player.spellcasting.spells[1].spells[spell_index] if use_set_two else player.spellcasting.spells[0].spells[spell_index]
	spell.learned = true
	# Mark the spell as the last spell learned, in case we need to start forgetting them
	for i: int in 64:
		if player.spellcasting.spell_order[i] == 99:
			player.spellcasting.spell_order[i] = spell_index
			break
	# Let the player know they've learned a spell
	Profile.instance.message_print("You have learned the %s of %s." % [spell_type, spell.name])
	player.spare_spell_slots -= 1
	if player.spare_spell_slots != 0:
		plural = "s" if player.spare_spell_slots != 1 else ""
		Profile.instance.message_print("You can learn %d more %s%s." % [player.spare_spell_slots, spell_type, plural])
	player.old_spare_spell_slots = player.spare_spell_slots
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_STUDY)


func command_destroy() -> void:
	var amount:int = 1
	var force:bool = User.command_argument > 0
	# Get an item to destroy
	var item_index:int = savegame.get_item("Destroy which item? ", false, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to destroy.")
		return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# If we have more than one we might not want to destroy all of them
	if item.count > 1:
		amount = User.get_quantity("", item.count, true)
		if amount <= 0:
			return
	var old_number:int = item.count
	item.count = amount
	var item_name:String = item.description(true, 3)
	item.count = old_number
	#Only confirm if it's not a worthless item
	if not force:
		if not item.stompable():
			var out_val: String = "Really destroy %s? " % [item_name]
			if not User.get_check(out_val):
				return
			# If it was something we might want to destroy again, ask
			if not item.item_type.has_quality() and item.item_type.item_category != ItemCategory.CHEST:
				if item.is_known():
					if User.get_check("Always destroy %s?" % [item_name]):
						item.item_type.stompable[0] = true
	# Destroying something takes a turn
	savegame.energy_use = 100
	# Can't destroy an artifact or legendary item
	if item.is_artifact() or item.is_legendary():
		var feel: String = "special"
		savegame.energy_use = 0
		Profile.instance.message_print("You cannot destroy {item_name}.")
		if item.is_cursed() or item.is_broken():
			feel = "terrible"
		item.inscription = feel
		item.identity_flags.set_bits(IdentifyFlags.SENSED)
		player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE)
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_EQUIPPY)
		return
	Profile.instance.message_print("You destroy %s." % [item_name])
	# Warriors and paladins get experience for destroying magic books
	if savegame.command_engine.item_filter_high_level_book(item):
		var gain_exp:bool = false
		if player.profession.experience_for_destroying(player, item.category):
			gain_exp = true
		if gain_exp and player.experience_points < Constants.MAX_EXPERIENCE:
			var exp_to_add:int = player.MaxExperienceGained / 20
			if exp_to_add > 10000:
				exp_to_add = 10000
			if item.sub_category < 3:
				exp_to_add /= 4
			if exp_to_add < 1:
				exp_to_add = 1
			Profile.instance.message_print("You feel more experienced.")
			player.gain_experience(exp_to_add * amount)
	# Tidy up the player's inventory
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -amount)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -amount)
		dungeon_level.floor_idem_describe(0 - item_index)
		dungeon_level.floor_item_optimise(0 - item_index)


func command_destroy_all()->void:
	var count:int = 0
	# Look for worthless items
	for i:int in range (InventorySlot.PACK - 1,-1,-1):
		var item:Item = player.inventory.items[i]
		if item.item_type == null:
			continue
		# Only destroy if it's stompable (i.e. worthless or marked as unwanted)
		if not item.stompable():
			continue
		var item_name:String = item.description(true, 3)
		Profile.instance.message_print("You destroy %s." % [item_name])
		count += 1
		var amount:int = item.count
		player.inventory.inven_item_increase(i, -amount)
		player.inventory.inven_item_optimise(i)
	if count == 0:
		Profile.instance.message_print("You are carrying nothing worth destroying.")
		savegame.energy_use = 0
	else:
		# If we destroyed at least one thing, take a turn
		savegame.energy_use = 100


func command_examine() -> void:
	# Get the item to examine
	var item_index:int = savegame.get_item("Examine which item? ", true, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to examine.")
		return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Do we know anything about it?
	if item.identity_flags.is_clear(IdentifyFlags.MENTAL):
		Profile.instance.message_print("You have no special knowledge about that item.")
		return
	var item_name: String = item.description(true, 3)
	Profile.instance.message_print("Examining %s..." % [item_name])
	# We're not actually identifying it, because it's already itentified, but we want to
	# repeat the identification text
	if not item.identify_fully():
		Profile.instance.message_print("You see nothing special.")


func command_journal()->void:
	var journal:Journal = Journal.new(player, dungeon_level, savegame)
	journal.show_menu()


func command_query_symbol()->void:
	Console.save_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	var symbols: SymbolDisplay = SymbolDisplay.new()
	symbols.display(false)
	Console.any_key(44)
	Console.restore_screen()
	Console.set_background(BackgroundImage.IN_GAME)

func command_alter()->void:
	# Assume we won't disturb the player
	var disturb:bool = false
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# We might have a repeat command
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Get the direction in which to alter something
	var dir:int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row:int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col:int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile:GridTile = dungeon_level.get_grid(row,col)
	# Altering a tile will take a turn
	savegame.energy_use = 100
	# We 'alter' a tile by attacking it
	if tile.monster_index != 0:
		savegame.command_engine.player_attack_monster(row, col)
	else:
		# Check the action based on the type of tile
		match tile.feature_type.alter_action:
			FloorTileAlterAction.NOTHING:
				Profile.instance.message_print("You're not sure what you can do with that...")
			FloorTileAlterAction.TUNNEL:
				disturb = savegame.command_engine.tunnel_through_tile(row, col)
			FloorTileAlterAction.DISARM:
				disturb = savegame.command_engine.disarm_trap(row, col, dir)
			FloorTileAlterAction.OPEN:
				disturb = savegame.command_engine.open_door(row, col)
			FloorTileAlterAction.CLOSE:
				disturb = savegame.command_engine.close_door(row, col)
			FloorTileAlterAction.BASH:
				disturb = savegame.command_engine.bash_closed_door(row, col, dir)
	if not disturb:
		savegame.disturb(false)


func command_bash()->void:
	var disturb:bool = false
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# We might have a repeat command
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Get the direction to bash
	var dir:int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row:int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col:int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile:GridTile = dungeon_level.get_grid(row,col)
	# Can only bash closed doors
	if not tile.feature_type.is_closed_door:
		Profile.instance.message_print("You see nothing there to bash.")
	elif tile.monster_index != 0:
		# Oops - a montser got in the way
		savegame.energy_use = 100
		Profile.instance.message_print("There is a monster in the way!")
		savegame.command_engine.player_attack_monster(row, col)
	else:
		# Bash the door
		disturb = savegame.command_engine.bash_closed_door(row, col, dir)
	if disturb:
		savegame.disturb(false)

func command_locate()->void:
	var start_row:int = dungeon_level.panel_row
	var start_col:int = dungeon_level.panel_col
	var current_row:int = start_row
	var current_col:int = start_col
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# Enter a loop so the player can browse the level
	while true:
		# Describe the location being viewed
		var offset_text:String
		if current_row == start_row and current_col == start_col:
			offset_text = ""
		else:
			var north_south:String = " North" if current_row < start_row else " South" if current_row > start_row else ""
			var east_west:String = " West" if current_col < start_col else " East" if current_col > start_col else ""
			offset_text = "%s%s of" % [north_south,east_west]
		var message:String = "Map sector [%s,%s], which is%s your sector. Direction?" % [current_row, current_col, offset_text]
		# Get a direction command or escape
		var dir:int = 0
		while dir == 0:
			var command:String = User.get_simple_choice(message)
			if command == "":
				break
			dir = User.key_to_keypad_direction(command)
		if dir == 0:
			break
		# Move the view based on the direction
		current_row += dungeon_level.keypad_direction_row_offset[dir]
		current_col += dungeon_level.keypad_direction_col_offset[dir]
		if current_row > dungeon_level.max_panel_rows:
			current_row = dungeon_level.max_panel_rows
		elif current_row < 0:
			current_row = 0
		if current_col > dungeon_level.max_panel_cols:
			current_col = dungeon_level.max_panel_cols
		elif current_col < 0:
			current_col = 0
		# Update the view if necessary
		if current_row != dungeon_level.panel_row or current_col != dungeon_level.panel_col:
			dungeon_level.panel_row = current_row
			dungeon_level.panel_col = current_col
			target_engine.panel_bounds()
			player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
			savegame.notice_update_redraw()
	# We've finished, so snap back to the player's location
	target_engine.recenter_screen_around_player()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	savegame.notice_update_redraw()

func command_mutant_power()->void:
	var num:int = 0
	var powers:Array[int]
	var power_descriptions:Array[String]
	var pets:int = 0
	var all_pets:bool = false
	var monster:Monster
	var has_ancestral:bool = false
	var ancestral_power:String = "(none)"
	var i:int
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused to use any powers!")
		savegame.energy_use = 0
		return
	ancestral_power = player.ancestry.get_ancestral_power_text(player)
	if ancestral_power != "":
		has_ancestral = true
	for pet_index:int in range (dungeon_level.monster_count_max - 1, 0, -1):
		monster = dungeon_level.dungeon_monsters.monsters[pet_index]
		if monster.mind.is_set(MonsterMind.FRIENDLY):
			pets += 1
	var active_mutations:Array[Mutation] = player.genome.activatable_mutations(player)
	if not has_ancestral and active_mutations.size() == 0 and pets == 0:
		Profile.instance.message_print("You have no powers to activate.")
		savegame.energy_use = 0
		return
	if has_ancestral:
		powers.append(Constants.MAX_INT)
		power_descriptions.append(ancestral_power)
		num += 1
	for j:int in active_mutations.size():
		powers.append(j + 100)
		power_descriptions.append(active_mutations[j].activation_summary(player.level))
		num += 1
	if pets > 0:
		power_descriptions.append("dismiss pets")
		powers.append(3)
		num += 1
	var flag:bool = false
	var redraw:bool = false
	var out_val: String = "(Powers a-%s, *=List, ESC=exit) Use which power? " % [Util.index_to_label(num-1)]
	while not flag:
		var choice:String = User.get_simple_choice(out_val)
		if choice == "":
			break
		if choice == " " or choice == "*" or choice == "?":
			if not redraw:
				var row:int = 1
				var col:int = 13
				var ctr:int = 0
				redraw = true
				Console.save_screen()
				Console.print_line_at(row, col, "", Ink.WHITE)
				while ctr < num:
					var dummy:String = "%s) %s" % [Util.index_to_label(ctr), power_descriptions[ctr]]
					Console.print_line_at(row + ctr, col, dummy, Ink.WHITE)
					ctr += 1
				Console.print_line_at(row + ctr, col, "", Ink.WHITE)
			else:
				redraw = false
				Console.restore_screen()
			continue
		if choice == 'Enter' and num == 1:
			choice = 'a'
		choice = choice.to_lower()
		i = Util.label_to_index(choice)
		if i < 0 or i >= num:
			continue
		flag = true
	if redraw:
		Console.restore_screen()
	if not flag:
		savegame.energy_use = 0
		return
	if powers[i] == Constants.MAX_INT:
		player.ancestry.use_ancestral_power(savegame, player, dungeon_level)
	elif powers[i] == 3:
		var dismissed: int = 0
		if User.get_check("Dismiss all pets? "):
			all_pets = true
		for pet_index:int in range (dungeon_level.monster_count_max - 1, 0, -1):
			monster = dungeon_level.dungeon_monsters.monsters[pet_index]
			if monster.mind.is_set(MonsterMind.FRIENDLY):
				var delete_this_monster:bool = false
				if all_pets:
					delete_this_monster = true
				else:
					var friend_name:String = monster.describe(0x80)
					var check_friend:String = "Dismiss %s? " % [friend_name]
					if User.get_check(check_friend):
						delete_this_monster = true
				if delete_this_monster:
					dungeon_level.dungeon_monsters.delete_monster_by_index(pet_index, true)
					dismissed += 1
		var s:String = "" if dismissed == 1 else "s"
		Profile.instance.message_print("You have dismissed %d pet%s." % [dismissed, s])
	else:
		savegame.energy_use = 100
		active_mutations[powers[i] - 100].activate(savegame, player, dungeon_level)

func command_toggle_search()->void:
	if player.is_searching:
		player.is_searching = false
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE | RedrawFlags.REDRAW_SPEED)
	else:
		player.is_searching = true
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE | RedrawFlags.REDRAW_SPEED)


func command_tunnel()->void:
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var disturb:bool = false
	# We might be tunnelling for multiple turns
	if User.command_argument != 0:
		User.command_repeat = User.command_argument - 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
		User.command_argument = 0
	# Get the direction in which we wish to tunnel
	var dir:int = target_engine.get_direction_no_aim()
	if dir != -1:
		# Pick up the tile that the player wishes to tunnel through
		var tile_row:int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var tile_col:int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile:GridTile = dungeon_level.get_grid(tile_row,tile_col)
		# Check if it can be tunneled through
		if tile.feature_type.is_passable or tile.feature_type.name == "YellowSign":
			Profile.instance.message_print("You cannot tunnel through air.")
		elif tile.feature_type.is_closed_door:
			Profile.instance.message_print("You cannot tunnel through doors.")
		# Can't tunnel if there's a monster there - so attack the monster instead
		elif tile.monster_index != 0:
			savegame.energy_use = 100
			Profile.instance.message_print("There is a monster in the way!")
			savegame.command_engine.player_attack_monster(tile_row, tile_col)
		else:
			# Tunnel through the tile
			disturb = savegame.command_engine.tunnel_through_tile(tile_row, tile_col)
	# Something might have disturbed us
	if not disturb:
		savegame.disturb(false)


func command_version()->void:
	Profile.instance.message_print("You are playing Cthangband %s." % [Constants.VERSION_STAMP])
