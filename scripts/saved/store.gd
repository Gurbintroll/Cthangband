# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Store
extends Resource

const STORE_CHOICES: int = 48
const STORE_INVEN_MAX: int = 26
const STORE_ITEM_LEVEL: int = 5
const STORE_MAX_KEEP: int = 18
const STORE_MIN_KEEP: int = 6
const STORE_SHUFFLE: int = 21
const STORE_TURNOVER: int = 9
const STORE_TURNS: int = 1000
const COMMENT_AGREE: Array[String] = ["Okay.", "Fine.", "Accepted!", "Agreed!", "Done!", "Taken!"]
const COMMENT_OUTRAGED: Array[String] = ["Arrgghh!", "You bastard!", "You hear someone sobbing...", "The shopkeeper howls in agony!"]
const COMMENT_ANNOYED: Array[String] = ["Damn!", "You bastard!", "The shopkeeper curses at you.", "The shopkeeper glares at you."]
const COMMENT_PLEASED: Array[String] = ["Cool!", "You've made my day!", "The shopkeeper giggles.", "The shopkeeper laughs loudly."]
const COMMENT_DELIGHTED: Array[String] = ["Yipee!", "I think I'll retire!", "The shopkeeper jumps for joy.", "The shopkeeper smiles gleefully."]

@export var door_row: int
@export var door_col: int
@export var feature_type: String
@export var stock: Array[Item]
@export var stock_size: int
@export var store_type: int
@export var leave_store: bool
@export var owner: StoreOwner
@export var stock_num: int
@export var store_top: int
@export var table: Array[int]

var command_handler: CommandHandler
var savegame: SaveGame
var player: Player


static func new_store(new_store_type: int) -> Store:
	var store: Store = Store.new()
	store.initialise(new_store_type)
	return store


func initialise(new_store_type: int) -> void:
	store_type = new_store_type
	stock_size = STORE_INVEN_MAX
	for i: int in stock_size:
		stock.append(Item.new())
	match store_type:
		StoreType.ALCHEMIST:
			feature_type = "Alchemist"
		StoreType.ARMOURY:
			feature_type = "Armoury"
		StoreType.BLACK_MARKET:
			feature_type = "BlackMarket"
		StoreType.BOOKSTORE:
			feature_type = "Bookstore"
		StoreType.EMPTY_LOT:
			# Doesn't get used
			feature_type = "GeneralStore"
		StoreType.GENERAL_STORE:
			feature_type = "GeneralStore"
		StoreType.HALL_OF_RECORDS:
			feature_type = "HallOfRecords"
		StoreType.HOME:
			feature_type = "Home"
		StoreType.INN:
			feature_type = "Inn"
		StoreType.MAGIC_STORE:
			feature_type = "MagicShop"
		StoreType.PAWNBROKERS:
			feature_type = "Pawnbrokers"
		StoreType.TEMPLE:
			feature_type = "Temple"
		StoreType.WEAPONSMITH:
			feature_type = "Weaponsmiths"
	if (store_type == StoreType.BLACK_MARKET or store_type == StoreType.HOME
		or store_type == StoreType.PAWNBROKERS or store_type == StoreType.HALL_OF_RECORDS
		or store_type == StoreType.EMPTY_LOT ):
		return
	var master: Array[ItemIdentifier] = StoreFactory.get_store_table(store_type)
	for item_id: ItemIdentifier in master:
		for item_index: int in SaveGame.instance.item_types.types.size():
			var item_type: ItemType = SaveGame.instance.item_types.types[item_index]
			if item_type.item_category == item_id.item_category and item_type.sub_category == item_id.item_type:
				table.append(item_index)


func store_init() -> void:
	owner = StoreFactory.get_random_owner(store_type)
	stock_num = 0
	stock = []
	for i: int in stock_size:
		stock.append(Item.new())


func store_maint() -> void:
	# Only stores with moving stock get maintained
	if store_type == StoreType.HOME or store_type == StoreType.HALL_OF_RECORDS or store_type == StoreType.PAWNBROKERS or store_type == StoreType.EMPTY_LOT:
		return
	# Creating items changes the current level's rating, so remember it so we can restore it
	var old_rating: int
	if SaveGame.instance.dungeon_level != null:
		old_rating = SaveGame.instance.dungeon_level.treasure_rating
	var new_stock_num: int = stock_num
	new_stock_num -= Rng.roll_die(STORE_TURNOVER)
	new_stock_num = clampi(new_stock_num, STORE_MIN_KEEP, STORE_MAX_KEEP)
	while stock_num > new_stock_num:
		store_delete()
	new_stock_num = stock_num
	new_stock_num += Rng.roll_die(STORE_TURNOVER)
	new_stock_num = clampi(new_stock_num, STORE_MIN_KEEP, min(stock_size-1, STORE_MAX_KEEP))
	if store_type == StoreType.INN:
		new_stock_num = mini(new_stock_num, 4)
	while stock_num < new_stock_num:
		store_create()
	# Restore the level rating if needed
	if SaveGame.instance.dungeon_level != null:
		SaveGame.instance.dungeon_level.treasure_rating = old_rating


func store_delete() -> void:
	var which: int = Rng.roll_die(stock_num) - 1
	var num: int = stock[which].count
	if Rng.roll_die(2) == 1:
		num = (num + 1) / 2
	if Rng.roll_die(2) == 1:
		num = 1
	store_item_increase(which, -num)
	store_item_optimise(which)


func store_item_increase(which: int, num: int) -> void:
	var item: Item = stock[which]
	var new_num: int = item.count + num
	new_num = clampi(new_num, 0, 255)
	item.count = new_num


func store_item_optimise(which: int) -> void:
	var item: Item = stock[which]
	if item.item_type == null:
		return
	if item.count > 0:
		return
	stock_num -= 1
	for j: int in range(which, stock_num):
		stock[j] = stock[j + 1]
	stock[stock_num] = Item.new()


func store_create() -> void:
	if stock_num >= stock_size:
		return
	for tries: int in 4:
		var item_level: int
		var item_type: ItemType
		if store_type == StoreType.BLACK_MARKET:
			item_level = 34 + Rng.roll_die(35)
			item_type = ItemType.random_item_type(item_level)
			if item_type == null:
				continue
		else:
			var i: int = table[Rng.roll_die(table.size()) - 1]
			item_level = Rng.roll_die(STORE_ITEM_LEVEL)
			item_type = SaveGame.instance.item_types.types[i]
		var item: Item = Item.new()
		item.assign_item_type(item_type)
		item.apply_magic(item_level, false, false, false)
		if item.category == ItemCategory.LIGHT:
			if item.sub_category == LightType.TORCH:
				item.type_specific_value = Constants.FUEL_TORCH / 2
			if item.sub_category == LightType.LANTERN:
				item.type_specific_value = Constants.FUEL_LANTERN / 2
		item.become_known()
		item.identify_flags.set_bits(IdentifyFlags.STOREBOUGHT)
		if item.category == ItemCategory.CHEST:
			continue
		if store_type == StoreType.BLACK_MARKET:
			if item.value() < 10:
				continue
		else:
			if item.value() <= 0:
				continue
		mass_produce(item)
		store_carry(item)
		break


func mass_produce(item: Item) -> void:
	var size: int = 1
	var discount: int = 0
	var cost: int = item.value()
	match item.category:
		ItemCategory.FOOD, ItemCategory.FLASK, ItemCategory.LIGHT:
			if cost <= 5:
				size += Rng.roll_dice(3, 5) - 3
			if cost <= 20:
				size += Rng.roll_dice(3, 5) - 3
		ItemCategory.POTION, ItemCategory.SCROLL:
			if cost <= 60:
				size += Rng.roll_dice(3, 5) - 3
			if cost <= 240:
				size += Rng.roll_dice(1, 5) - 1
		ItemCategory.LIFE_BOOK, ItemCategory.SORCERY_BOOK, ItemCategory.NATURE_BOOK, ItemCategory.CHAOS_BOOK:
			if cost <= 50:
				size += Rng.roll_dice(2, 3) - 2
			if cost <= 500:
				size += Rng.roll_dice(1, 3) - 1
		ItemCategory.DEATH_BOOK, ItemCategory.TAROT_BOOK, ItemCategory.FOLK_BOOK, ItemCategory.CORPOREAL_BOOK:
			if cost <= 50:
				size += Rng.roll_dice(2, 3) - 2
			if cost <= 500:
				size += Rng.roll_dice(1, 3) - 1
		ItemCategory.SOFT_ARMOUR, ItemCategory.HARD_ARMOUR, ItemCategory.SHIELD, ItemCategory.GLOVES:
			if item.rare_index == 0:
				if cost <= 10:
					size += Rng.roll_dice(3, 5) - 3
				if cost <= 100:
					size += Rng.roll_dice(3, 5) - 3
		ItemCategory.BOOTS, ItemCategory.CLOAK, ItemCategory.HELM, ItemCategory.CROWN:
			if item.rare_index == 0:
				if cost <= 10:
					size += Rng.roll_dice(3, 5) - 3
				if cost <= 100:
					size += Rng.roll_dice(3, 5) - 3
		ItemCategory.POLEARM, ItemCategory.HAFTED, ItemCategory.DIGGING, ItemCategory.BOW:
			if item.rare_index == 0:
				if cost <= 10:
					size += Rng.roll_dice(3, 5) - 3
				if cost <= 100:
					size += Rng.roll_dice(3, 5) - 3
		ItemCategory.SPIKE, ItemCategory.SHOT, ItemCategory.ARROW, ItemCategory.BOLT:
			if cost <= 5:
				size += Rng.roll_dice(5, 5) - 5
			if cost <= 50:
				size += Rng.roll_dice(5, 5) - 5
			if cost <= 500:
				size += Rng.roll_dice(5, 5) - 5
	if cost < 5:
		discount = 0
	elif Rng.roll_die(25) == 1:
		discount = 25
	elif Rng.roll_die(150) == 1:
		discount = 50
	elif Rng.roll_die(300) == 1:
		discount = 75
	elif Rng.roll_die(500) == 1:
		discount = 90
	if item.is_legendary():
		discount = 0
	item.discount = discount
	item.count = size - (size * discount / 100)


## Put an item into the shop, returning the slot it went into
func store_carry(item: Item) -> int:
	# If the item is worthless, reject it
	var value: int = item.value()
	if value <= 0:
		return -1
	# If we're not in the pawnshop, identify the item
	if store_type != StoreType.PAWNBROKERS:
		item.identify_flags.set_bits(IdentifyFlags.MENTAL)
		item.inscription = ""
	# If we can just add to the quantity of an existing item, then do so
	for slot: int in stock_num:
		var stock_item: Item = stock[slot]
		if store_object_similar(stock_item, item):
			store_object_absorb(stock_item, item)
			return slot
	# If there's no room in the store for an extra item, then reject it
	if stock_num >= stock_size:
		return -1
	# Find the correct slot to put the item into
	var insertion_slot: int = 0
	# loop to stock_num + 1 because we need to be able to fall off the end
	for slot: int in stock_num + 1:
		insertion_slot = slot
		if slot == stock_num:
			# If we've fallen off the end, quit (after insertion_slot has been updated)
			break
		var stock_item: Item = stock[insertion_slot]
		if item.category > stock_item.category:
			# If we're a higher category, quit
			break
		if item.category < stock_item.category:
			# If we're a lower category, move on regardless of other properties
			continue
		if item.sub_category < stock_item.sub_category:
			# If we're a lower sub_category, quit
			break
		if item.sub_category > stock_item.sub_category:
			# If we're a higher sub_category, move on regardless of other properties
			continue
		if item.category == ItemCategory.ROD:
			if item.type_specific_value < stock_item.type_specific_value:
				# If we have a shorter recharge, quit
				break
			if item.type_specific_value > stock_item.type_specific_value:
				# If we have a longer recharge, move on regardless of other properties
				continue
		var stock_value: int = stock_item.value()
		if value > stock_value:
			# If we're more expensive, quit
			break
	# Shuffle the remaining items up by one
	for i: int in range(stock_num, insertion_slot, -1):
		stock[i] = stock[i - 1]
	# Insert the item into the stock
	stock_num += 1
	stock[insertion_slot] = item
	return insertion_slot


func store_object_absorb(store_item: Item, item: Item) -> void:
	var total: int = store_item.count + item.count
	store_item.count = total if total <= 99 else 99


func store_object_similar(item: Item, store_item: Item) -> bool:
	if item == store_item:
		return false
	if item.item_type != store_item.item_type:
		return false
	if item.type_specific_value != store_item.type_specific_value:
		return false
	if item.bonus_to_hit != store_item.bonus_to_hit:
		return false
	if item.bonus_damage != store_item.bonus_damage:
		return false
	if item.bonus_armour_class != store_item.bonus_armour_class:
		return false
	if item.artifact_index != store_item.artifact_index:
		return false
	if item.rare_index != store_item.rare_index:
		return false
	if item.is_legendary() or store_item.is_legendary():
		return false
	if (item.legendary_flags_1.value != store_item.legendary_flags_1.value or
		item.legendary_flags_2.value != store_item.legendary_flags_2.value or
		item.legendary_flags_3.value != store_item.legendary_flags_3.value ):
		return false
	if item.bonus_power_type != 0 or store_item.bonus_power_type != 0:
		return false
	if item.recharge_time_left != 0 or store_item.recharge_time_left != 0:
		return false
	if item.base_armour_class != store_item.base_armour_class:
		return false
	if item.damage_dice != store_item.damage_dice:
		return false
	if item.damage_dice_sides != store_item.damage_dice_sides:
		return false
	if item.category == ItemCategory.CHEST:
		return false
	if item.discount != store_item.discount:
		return false
	return true


func enter_store(the_player: Player, the_command_handler: CommandHandler) -> void:
	player = the_player
	command_handler = the_command_handler
	savegame = SaveGame.instance
	store_top = 0
	display_store()
	leave_store = false
	while not leave_store:
		Console.clear_row(1)
		var temp_cha: int = player.ability_scores[Ability.CHARISMA].adjusted
		Console.clear_from(41)
		Console.print_line_at(42, 0, " ESC) Exit from Building.", Ink.WHITE)
		match store_type:
			StoreType.GENERAL_STORE:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Hire an escort.", Ink.WHITE)
			StoreType.ARMOURY:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Enchant your armour.", Ink.WHITE)
			StoreType.WEAPONSMITH:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Enchant your weapon.", Ink.WHITE)
			StoreType.TEMPLE:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 0, " v) Sacrifice Item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) buy Remove Curse.", Ink.WHITE)
			StoreType.ALCHEMIST:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) buy Restoration.", Ink.WHITE)
			StoreType.MAGIC_STORE:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Research an item.", Ink.WHITE)
			StoreType.BLACK_MARKET:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
			StoreType.HOME:
				Console.print_line_at(42, 31, " g) Get an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " d) Drop an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Rest a while.", Ink.WHITE)
			StoreType.BOOKSTORE:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Research a spell.", Ink.WHITE)
			StoreType.INN:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) hire a Room.", Ink.WHITE)
			StoreType.HALL_OF_RECORDS:
				Console.print_line_at(42, 31, " v) view racial Heroes.", Ink.WHITE)
				Console.print_line_at(43, 31, " c) view Class heroes.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Buy a house.", Ink.WHITE)
			StoreType.PAWNBROKERS:
				Console.print_line_at(42, 31, " p) Purchase an item.", Ink.WHITE)
				Console.print_line_at(42, 56, " x) eXamine an item.", Ink.WHITE)
				Console.print_line_at(43, 31, " s) Sell an item.", Ink.WHITE)
				Console.print_line_at(43, 56, " r) Identify all.", Ink.WHITE)
		Console.print_at(41, 0, "You may:", Ink.WHITE)
		User.get_command(true)
		store_process_command()
		Console.full_screen_overlay = true
		savegame.notice_stuff()
		savegame.notice_update_redraw()
		if player.inventory.items[InventorySlot.PACK].item_type != null:
			var item: Item = player.inventory.items[InventorySlot.PACK]
			if store_type != StoreType.HOME:
				Profile.instance.message_print("Your pack is so full that you flee the Stores...")
				leave_store = true
			elif not store_check_num(item):
				Profile.instance.message_print("Your pack is so full that you flee your home...")
				leave_store = true
			else:
				Profile.instance.message_print("Your pack overflows!")
				var overflow_item: Item = item.get_copy()
				var item_name: String = overflow_item.description(true, 3)
				Profile.instance.message_print("You drop %s (%s)." % [item_name, Util.index_to_label(InventorySlot.PACK)])
				player.inventory.InvenItemIncrease(InventorySlot.PACK, -255)
				player.inventory.InvenItemDescribe(InventorySlot.PACK)
				player.inventory.InvenItemOptimize(InventorySlot.PACK)
				savegame.notice_update_redraw()
				var item_position: int = home_carry(overflow_item)
				if item_position >= 0:
					store_top = item_position / 26 * 26
					display_inventory()
		if temp_cha != player.ability_scores[Ability.CHARISMA].adjusted:
			display_inventory()
	savegame.energy_use = 0
	Console.full_screen_overlay = false
	User.queued_command = ""
	savegame.viewing_item_list = false
	Profile.instance.message_print("")
	Console.clear_screen()
	Console.set_background(BackgroundImage.DEFAULT)
	Console.set_mask (1,13,42,66)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_BASIC | RedrawFlags.REDRAW_EXTRA | RedrawFlags.REDRAW_EQUIPPY)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func display_entry(pos: int) -> void:
	var item_name: String
	var max_width: int
	var item: Item = stock[pos]
	var i: int = pos % 26
	var to_print: String = "%s) " % [Util.index_to_label(i)]
	Console.print_line_at(i + 6, 0, to_print, Ink.WHITE)
	var a: int = item.item_type.colour
	var c: String = item.item_type.character
	Console.print_at(i + 6, 3, c, a)
	if store_type == StoreType.HOME:
		max_width = 75
		max_width -= 10
		item_name = item.description(true, 3)
		if max_width < item_name.length():
			item_name = item_name.substr(0, max_width)
		Console.print_at(i + 6, 5, item_name, Util.item_category_colour(item.category))
		var weight: int = item.weight
		to_print = "%3d.%d lb" % [weight / 10, weight % 10]
		Console.print_at(i + 6, 68, to_print, Ink.WHITE)
	else:
		max_width = 65
		max_width -= 7
		item_name = item.description(true, 3) if store_type == StoreType.PAWNBROKERS else item.store_description(true, 3)
		if max_width < item_name.length():
			item_name = item_name.substr(0, max_width)
		Console.print_at(i + 6, 5, item_name, Util.item_category_colour(item.category))
		var weight: int = item.weight
		to_print = "%3d.%d" % [weight / 10, weight % 10]
		Console.print_at(i + 6, 61, to_print, Ink.WHITE)
		var x: int
		if item.identify_flags.is_set(IdentifyFlags.FIXED):
			x = price_item(item, owner.min_markup, false)
			to_print = "%9d F" % [x]
			Console.print_at(i + 6, 68, to_print, Ink.WHITE)
		else:
			x = price_item(item, owner.min_markup, false)
			x += x / 10
			to_print = "%9d  " % [x]
			Console.print_at(i + 6, 68, to_print, Ink.WHITE)


func display_inventory() -> void:
	var k: int
	for j: int in 26:
		k = j
		if store_top + k >= stock_num:
			for i: int in range(k, 27):
				Console.clear_row(i + 6)
			break
		display_entry(store_top + k)
	Console.print_at(5, 20, "        ", Ink.WHITE)
	if stock_num > 26:
		Console.print_line_at(k + 6, 3, "-more-", Ink.WHITE)
		Console.print_at(5, 20, "(Page %d)" % [store_top / 26], Ink.WHITE)


func display_store() -> void:
	Console.clear_screen()
	Console.set_mask (-1,-1,-1,-1)
	Console.set_background(BackgroundImage.DEFAULT)
	if store_type == StoreType.HOME:
		Console.print_at(3, 30, "Your Home", Ink.WHITE)
		Console.print_at(5, 3, "Item Description", Ink.WHITE)
		Console.print_at(5, 70, "Weight", Ink.WHITE)
	if store_type == StoreType.HALL_OF_RECORDS:
		Console.print_at(3, 30, "Hall Of Records", Ink.WHITE)
	if store_type != StoreType.HOME and store_type != StoreType.HALL_OF_RECORDS:
		var store_name: String = StaticData.tile_types[feature_type].description
		var buf: String = "%s (%s)" % [owner.name, owner.ancestry]
		Console.print_at(3, 10, buf, Ink.WHITE)
		buf = "%s (%d)" % [store_name, owner.max_cost]
		Console.print_line_at(3, 50, buf, Ink.WHITE)
		Console.print_at(5, 3, "Item Description", Ink.WHITE)
		Console.print_at(5, 60, "Weight", Ink.WHITE)
		Console.print_at(5, 72, "Price", Ink.WHITE)
	store_print_gold()
	display_inventory()


func store_check_num(item: Item) -> bool:
	var other_item: Item
	if stock_num < stock_size:
		return true
	if store_type == StoreType.HOME:
		for i: int in stock_num:
			other_item = stock[i]
			if other_item.can_absorb(item):
				return true
	else:
		for i: int in stock_num:
			other_item = stock[i]
			if store_object_similar(other_item, item):
				return true
	return false


func home_carry(item: Item) -> int:
	var slot: int
	var other_item: Item
	for i: int in stock_num:
		slot = i
		other_item = stock[slot]
		if other_item.can_absorb(item):
			other_item.absorb(item)
			return slot
	if stock_num >= stock_size:
		return -1
	var value: int = item.value()
	for i: int in stock_num:
		slot = i
		other_item = stock[slot]
		if item.category > other_item.category:
			break
		if item.category < other_item.category:
			continue
		if not item.is_flavour_aware():
			continue
		if not other_item.is_flavour_aware():
			break
		if item.sub_category < other_item.sub_category:
			break
		if item.sub_category > other_item.sub_category:
			continue
		if not item.is_known():
			continue
		if not other_item.is_known():
			break
		if item.category == ItemCategory.ROD:
			if item.type_specific_value < other_item.type_specific_value:
				break
			if item.type_specific_value > other_item.type_specific_value:
				continue
		var other_value: int = other_item.value()
		if value > other_value:
			break
		if value < other_value:
			pass
	for i: int in range(stock_num, slot, -1):
		stock[i] = stock[i - 1].get_copy()
	stock_num += 1
	stock[slot] = item.get_copy()
	return slot


func price_item(item: Item, greed: int, flip: bool) -> int:
	var adjust: int
	var price: int = item.value()
	if price <= 0:
		return 0
	var factor: int = 100
	factor += player.ability_scores[Ability.CHARISMA].cha_price_adjustment()
	if flip:
		adjust = 100 + (300 - (greed + factor))
		if adjust > 100:
			adjust = 100
		if store_type == StoreType.BLACK_MARKET:
			price /= 2
		if store_type == StoreType.PAWNBROKERS:
			price /= 3
	else:
		adjust = 100 + (greed + factor - 300)
		if adjust < 100:
			adjust = 100
		if store_type == StoreType.BLACK_MARKET:
			price *= 2
		if store_type == StoreType.PAWNBROKERS:
			price /= 3
	price = ((price * adjust) + 50) / 100
	if price <= 0:
		return 1
	return price


func store_print_gold() -> void:
	Console.print_line_at(39, 53, "Gold Remaining: ", Ink.WHITE)
	Console.print_line_at(39, 68, "%9d" % [player.gold], Ink.GOLD)


func store_process_command() -> void:
	match User.current_command:
		"Escape":
			leave_store = true
		"g":
			store_purchase()
		"d":
			store_sell()
		"x":
			store_examine()
		"c":
			if store_type == StoreType.HALL_OF_RECORDS:
				Console.save_screen()
				Console.high_score_table.profession_filter = player.current_profession
				Console.high_score_table.display_scores_around_living(HighScore.new_highscore(player, savegame))
				Console.high_score_table.profession_filter = ""
				Console.restore_screen()
			else:
				Profile.instance.message_print("That command does not work in this Stores.")
		"v":
			if store_type == StoreType.HALL_OF_RECORDS:
				Console.save_screen()
				Console.high_score_table.ancestry_filter = player.current_ancestry
				Console.high_score_table.display_scores_around_living(HighScore.new_highscore(player, savegame))
				Console.high_score_table.ancestry_filter = ""
				Console.restore_screen()
			elif store_type == StoreType.TEMPLE:
				sacrifice_item()
			else:
				Profile.instance.message_print("That command does not work in this Stores.")
		"h":
			command_handler.command_manual()
		"r":
			var price: int
			match store_type:
				StoreType.GENERAL_STORE:
					var escortable: Dictionary[String, Town]
					for town: Town in savegame.towns:
						if town.visited and town.name != savegame.current_town.name and town.character != "K":
							escortable.get_or_add(town.character, town)
					if escortable.size() == 0:
						Profile.instance.message_print("There are no valid destinations to be escorted to.")
						Profile.instance.message_print("You must have visited a town before you can be escorted there.")
					else:
						var destination: Town = get_escort_destination(escortable)
						if destination != null:
							price = service_haggle(200)
							if price != Constants.MAX_INT:
								if price > player.gold:
									Profile.instance.message_print("You do not have the gold!")
								else:
									player.gold -= price
									say_comment_agree()
									store_print_gold()
									player.wilderness_row = destination.row
									player.wilderness_col = destination.col
									savegame.current_town = destination
									savegame.new_level_flag = true
									savegame.came_from = LevelStart.START_RANDOM
									Profile.instance.message_print("The journey takes all day.")
									player.game_time.to_next_dusk()
									leave_store = true
					savegame.notice_update_redraw()
				StoreType.ARMOURY:
					price = service_haggle(400)
					if price != Constants.MAX_INT:
						if price > player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							savegame.spell_effects_handler.enchant_spell(0, 0, 4)
						savegame.notice_update_redraw()
				StoreType.WEAPONSMITH:
					price = service_haggle(800)
					if price != Constants.MAX_INT:
						if price > player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							savegame.spell_effects_handler.enchant_spell(4, 4, 0)
						savegame.notice_update_redraw()
				StoreType.TEMPLE:
					price = service_haggle(500)
					if price != Constants.MAX_INT:
						if price > player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							savegame.spell_effects_handler.remove_curse()
						savegame.notice_update_redraw()
				StoreType.ALCHEMIST:
					price = service_haggle(750)
					if price != Constants.MAX_INT:
						if price > player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							player.try_restoring_ability_score(Ability.STRENGTH)
							player.try_restoring_ability_score(Ability.INTELLIGENCE)
							player.try_restoring_ability_score(Ability.WISDOM)
							player.try_restoring_ability_score(Ability.DEXTERITY)
							player.try_restoring_ability_score(Ability.CONSTITUTION)
							player.try_restoring_ability_score(Ability.CHARISMA)
							player.restore_level()
						savegame.notice_update_redraw()
				StoreType.MAGIC_STORE:
					price = service_haggle(2000)
					if price != Constants.MAX_INT:
						if price > player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							savegame.spell_effects_handler.identify_fully()
						savegame.notice_update_redraw()
				StoreType.BLACK_MARKET:
					Profile.instance.message_print("That command does not work in this Stores.")
				StoreType.HOME:
					if player.timed_poison > 0 or player.timed_bleeding > 0:
						Profile.instance.message_print("Your wounds prevent you from sleeping.")
					else:
						if player.ancestry.is_nocturnal:
							room_rest(true)
						else:
							room_rest(false)
				StoreType.BOOKSTORE:
					command_handler.command_study()
				StoreType.INN:
					if player.timed_poison > 0 or player.timed_bleeding > 0:
						Profile.instance.message_print("You need a healer, not a room!")
						Profile.instance.message_print("I'm sorry, but  I don't want anyone dying in here.")
					else:
						price = service_haggle(10)
						if price != Constants.MAX_INT:
							if price >= player.gold:
								Profile.instance.message_print("You do not have the gold!")
							else:
								player.gold -= price
								say_comment_agree()
								store_print_gold()
								room_rest(player.ancestry.is_nocturnal)
				StoreType.HALL_OF_RECORDS:
					if player.town_with_house == savegame.current_town.index:
						Profile.instance.message_print("You already have the deeds!")
					else:
						price = service_haggle(savegame.current_town.house_price)
						if price != Constants.MAX_INT:
							if price >= player.gold:
								Profile.instance.message_print("You do not have the gold!")
							else:
								player.gold -= price
								say_comment_agree()
								store_print_gold()
								var old_town_with_house: int = player.town_with_house
								player.town_with_house = savegame.current_town.index
								if old_town_with_house == -1:
									Profile.instance.message_print("You may move in at once.")
								else:
									Profile.instance.message_print(
										"I've sold your old house to pay for the removal service.",
									)
									move_house(old_town_with_house, player.town_with_house)
							savegame.notice_update_redraw()
				StoreType.PAWNBROKERS:
					price = service_haggle(500)
					if price != Constants.MAX_INT:
						if price >= player.gold:
							Profile.instance.message_print("You do not have the gold!")
						else:
							player.gold -= price
							say_comment_agree()
							store_print_gold()
							savegame.spell_effects_handler.identify_pack()
							Profile.instance.message_print("All your goods have been identified.")
						savegame.notice_update_redraw()
		"w":
			command_handler.command_wield()
		"t":
			command_handler.command_take_off()
		"k":
			command_handler.command_destroy()
		"K":
			command_handler.command_destroy_all()
		"e":
			command_handler.command_equipment()
		"i":
			command_handler.command_inventory()
		"I":
			command_handler.command_examine()
		"b":
			command_handler.command_browse(-1)
		"/":
			command_handler.command_query_symbol()
		"C":
			command_handler.command_view_character()
			display_store()
		"O":
			command_handler.command_message_one()
		"J":
			command_handler.command_journal()
			display_store()
		"P":
			command_handler.command_messages()
		" ", "Enter":
			# Space and enter do nothing, as they're likely to be "left over" from other commands
			pass
		_:
			Profile.instance.message_print("That command does not work in stores.")


func store_purchase() -> void:
	var new_item_index: int
	var item_name: String
	if stock_num <= 0:
		Profile.instance.message_print("Your home is empty." if store_type == StoreType.HOME else "I am currently out of stock.")
		return
	var i: int = stock_num - store_top
	if i > 26:
		i = 26
	var print_text: String = "Which item do you want to take? " if store_type == StoreType.HOME else "Which item are you interested in? "
	var item_index: int = get_stock(print_text, 0, i - 1)
	if item_index < 0:
		return
	item_index += store_top
	var item: Item = stock[item_index]
	var amt: int = 1
	var item_copy: Item = item.get_copy()
	item_copy.count = amt
	if not player.inventory.inven_carry_okay(item_copy):
		Profile.instance.message_print("You cannot carry that many different items.")
		return
	var best: int = price_item(item_copy, owner.min_markup, false)
	if item.identify_flags.is_clear(IdentifyFlags.FIXED):
		best += best / 10
	if item.count > 1:
		var max_buy: int = item.count if store_type == StoreType.HOME else min(player.gold / best, item.count)
		if max_buy < 2:
			amt = 1
		else:
			amt = User.get_quantity("", max_buy, false)
			if amt <= 0:
				return
	item_copy = item.get_copy()
	item_copy.count = amt
	if not player.inventory.inven_carry_okay(item_copy):
		Profile.instance.message_print("You cannot carry that many items.")
		return
	if store_type != StoreType.HOME:
		var price: int = best * item_copy.count
		if player.gold >= price:
			Console.play_sound("ShopTransaction")
			say_comment_agree()
			player.gold -= price
			store_print_gold()
			if store_type != StoreType.PAWNBROKERS:
				item_copy.become_flavour_aware()
			item_copy.identify_flags.clear_bits(IdentifyFlags.FIXED)
			item_name = item_copy.description(true, 3)
			var back: String = "back " if store_type == StoreType.PAWNBROKERS else ""
			Profile.instance.message_print("You bought %s%s for %s gold." % [back, item_name, price])
			item_copy.inscription = ""
			new_item_index = player.inventory.inven_carry(item_copy)
			item_name = player.inventory.items[new_item_index].description(true, 3)
			Profile.instance.message_print("You have %s (%s)." % [item_name, Util.index_to_label(new_item_index)])
			savegame.notice_update_redraw()
			i = stock_num
			store_item_increase(item_index, -amt)
			store_item_optimise(item_index)
			if stock_num == 0:
				if store_type == StoreType.PAWNBROKERS:
					store_top = 0
					display_inventory()
				else:
					if Rng.roll_die(STORE_SHUFFLE) == 1:
						Profile.instance.message_print("The shopkeeper retires.")
						store_shuffle()
					else:
						Profile.instance.message_print("The shopkeeper brings out some new stock.")
					for x: int in 10:
						store_maint()
					store_top = 0
					display_inventory()
			elif stock_num != i:
				if store_top >= stock_num:
					store_top -= 26
				display_inventory()
			else:
				display_entry(item_index)
		else:
			Profile.instance.message_print("You do not have enough gold.")
	else:
		new_item_index = player.inventory.inven_carry(item_copy)
		item_name = player.inventory.items[new_item_index].description(true, 3)
		Profile.instance.message_print("You have %s (%s)." % [item_name, Util.index_to_label(new_item_index)])
		savegame.notice_update_redraw()
		i = stock_num
		store_item_increase(item_index, -amt)
		store_item_optimise(item_index)
		if i == stock_num:
			display_entry(item_index)
		else:
			if stock_num == 0:
				store_top = 0
			elif store_top >= stock_num:
				store_top -= 26
			display_inventory()


func store_sell() -> void:
	var item_index: int
	var prompt: String = "Drop which item? " if store_type == StoreType.HOME else "Sell which item? "
	savegame.item_filter_function = item_filter_store_will_buy
	item_index = savegame.get_item(prompt, true, true, false)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing that I want.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else savegame.dungeon_level.dungeon_items.items[0 - item_index]
	if item_index >= InventorySlot.MELEE_WEAPON and item.is_cursed():
		Profile.instance.message_print("Hmmm, it seems to be cursed.")
		return
	var amt: int = 1
	if item.count > 1:
		amt = User.get_quantity("", item.count, true)
		if amt <= 0:
			return
	var item_copy: Item = item.get_copy()
	item_copy.count = amt
	var item_name: String = item_copy.description(true, 3)
	if store_type != StoreType.HOME:
		item_copy.inscription = ""
	if not store_check_num(item_copy):
		Profile.instance.message_print("Your home is full." if store_type == StoreType.HOME else "I have not the room in my Stores to keep it.")
		return
	if store_type != StoreType.HOME:
		Profile.instance.message_print("Selling %s (%s)." % [item_name, Util.index_to_label(item_index)])
		Profile.instance.message_print()
		var price: int = sell_haggle(item_copy)
		if price >= 0:
			say_comment_agree()
			player.gold += price
			store_print_gold()
			var unidentified_price: int = item_copy.value() * item_copy.count
			if store_type != StoreType.PAWNBROKERS:
				item.become_flavour_aware()
				item.become_known()
			player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
			item_copy = item.get_copy()
			item_copy.count = amt
			var real_value: int
			if store_type == StoreType.PAWNBROKERS:
				real_value = unidentified_price
			else:
				real_value = item_copy.value() * item_copy.count
				item_name = item_copy.description(true, 3)
			var pawn: String = "pawned" if store_type == StoreType.PAWNBROKERS else "sold"
			Profile.instance.message_print("You %s %s for %d gold." % [pawn, item_name, price])
			purchase_analyse(price, real_value, unidentified_price)
			player.inventory.inven_item_increase(item_index, -amt)
			player.inventory.inven_item_describe(item_index)
			player.inventory.inven_item_optimise(item_index)
			savegame.notice_update_redraw()
			item_index = store_carry(item_copy) if store_type != StoreType.PAWNBROKERS else home_carry(item_copy)
			if item_index >= 0:
				store_top = item_index / 26 * 26
				display_inventory()
	else:
		Profile.instance.message_print("You drop %s (%s)." % [item_name, Util.index_to_label(item_index)])
		player.inventory.inven_item_increase(item_index, -amt)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
		savegame.notice_update_redraw()
		item_index = home_carry(item_copy)
		if item_index >= 0:
			store_top = item_index / 26 * 26
			display_inventory()


func store_examine() -> void:
	if stock_num <= 0:
		Profile.instance.message_print("Your home is empty." if store_type == StoreType.HOME else "I am currently out of stock.")
		return
	var i: int = stock_num - store_top
	if i > 26:
		i = 26
	var prompt: String = "Which item do you want to examine? "
	var item_index: int = get_stock(prompt, 0, i - 1)
	if item_index < 0:
		return
	item_index += store_top
	var item: Item = stock[item_index]
	if (item.category == ItemCategory.LIFE_BOOK or item.category == ItemCategory.SORCERY_BOOK or
		item.category == ItemCategory.NATURE_BOOK or item.category == ItemCategory.CHAOS_BOOK or
		item.category == ItemCategory.DEATH_BOOK or item.category == ItemCategory.CORPOREAL_BOOK or
		item.category == ItemCategory.TAROT_BOOK or item.category == ItemCategory.FOLK_BOOK ):
		match item.category:
			ItemCategory.LIFE_BOOK:
				if player.realm_1 == Realm.LIFE or player.realm_2 == Realm.LIFE:
					do_store_browse(item)
					return
			ItemCategory.SORCERY_BOOK:
				if player.realm_1 == Realm.SORCERY or player.realm_2 == Realm.SORCERY:
					do_store_browse(item)
					return
			ItemCategory.NATURE_BOOK:
				if player.realm_1 == Realm.NATURE or player.realm_2 == Realm.NATURE:
					do_store_browse(item)
					return
			ItemCategory.CHAOS_BOOK:
				if player.realm_1 == Realm.CHAOS or player.realm_2 == Realm.CHAOS:
					do_store_browse(item)
					return
			ItemCategory.DEATH_BOOK:
				if player.realm_1 == Realm.DEATH or player.realm_2 == Realm.DEATH:
					do_store_browse(item)
					return
			ItemCategory.CORPOREAL_BOOK:
				if player.realm_1 == Realm.CORPOREAL or player.realm_2 == Realm.CORPOREAL:
					do_store_browse(item)
					return
			ItemCategory.TAROT_BOOK:
				if player.realm_1 == Realm.TAROT or player.realm_2 == Realm.TAROT:
					do_store_browse(item)
					return
			ItemCategory.FOLK_BOOK:
				if player.realm_1 == Realm.FOLK or player.realm_2 == Realm.FOLK:
					do_store_browse(item)
					return
		Profile.instance.message_print("The spells in the book are unintelligible to you.")
		return
	if item.identify_flags.is_clear(IdentifyFlags.MENTAL):
		Profile.instance.message_print("You have no special knowledge about that item_index.")
		return
	var item_name: String = item.description(true, 3)
	Profile.instance.message_print("Examining %s..." % [item_name])
	if not item.identify_fully():
		Profile.instance.message_print("You see nothing special.")


func sacrifice_item() -> void:
	var god_name: int = get_sacrifice_target()
	if god_name == GodName.NONE:
		return
	var deity: God = player.religion.gods[god_name]
	var prompt: String = "Sacrifice which item? "
	savegame.item_filter_function = Callable()
	var item_index: int = savegame.get_item(prompt, true, true, false)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to sacrifice.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else savegame.dungeon_level.dungeon_items.items[0 - item_index]
	if item_index >= InventorySlot.MELEE_WEAPON and item.is_cursed():
		Profile.instance.message_print("Hmmm, it seems to be cursed.")
		return
	var amt: int = 1
	if item.Count > 1:
		amt = User.get_quantity("", item.Count, true)
		if amt <= 0:
			return
	var item_copy: Item = item.get_copy()
	item_copy.count = amt
	item_copy.inscription = ""
	var item_value: int = price_item(item_copy, owner.min_markup, true) * item_copy.count
	player.inventory.inven_item_increase(item_index, -amt)
	player.inventory.inven_item_describe(item_index)
	player.inventory.inven_item_optimise(item_index)
	savegame.notice_update_redraw()
	var deity_name: String = deity.short_name
	if item_value <= 0:
		item_value = -100
	var favour: int = item_value / 10
	var old_favour: int = deity.adjusted_favour()
	player.religion.add_favour(god_name, favour)
	var new_favour: int = deity.AdjustedFavour
	var change: int = new_favour - old_favour
	if change < 0:
		Profile.instance.message_print("%s is displeased with your sacrifice!" % [deity_name])
	elif change == 0:
		Profile.instance.message_print("%s is indifferent to your sacrifice!" % [deity_name])
	elif change == 1:
		Profile.instance.message_print("%s approves of your sacrifice!" % [deity_name])
	elif change == 2:
		Profile.instance.message_print("%s likes your sacrifice!" % [deity_name])
	elif change == 3:
		Profile.instance.message_print("%s loves your sacrifice!" % [deity_name])
	else:
		Profile.instance.message_print("%s is delighted by your sacrifice!" % [deity_name])
	player.updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER)


func get_sacrifice_target() -> int:
	Console.save_screen()
	var deities: Dictionary[int, God] = player.religion.gods
	var names: Array[String]
	var keys: Array[String]
	for key: int in deities:
		names.append(deities[key].long_name)
		keys.append(deities[key].long_name[0])
	names.sort()
	keys.sort()
	var prompt: String = "Sacrifice to whom (%s to %s)? " % [keys[0].to_lower(), keys[-1].to_lower()]
	for i: int in keys.size():
		Console.print_line_at(i + 1, 20, "%s) %s" % [keys[i], names[i]], Ink.WHITE)
	Console.print_line_at(keys.size() + 1, 20, "", Ink.WHITE)
	while true:
		var choice: String = User.get_simple_choice(prompt).to_upper()[0]
		if choice == "Escape":
			break
		for index: int in deities:
			if deities[index].short_name[0] == choice:
				Console.restore_screen()
				return deities[index].god_name
	Console.restore_screen()
	return GodName.NONE


func get_escort_destination(towns: Dictionary[String, Town]) -> Town:
	Console.save_screen()
	var keys: Array[String] = towns.keys()
	keys.sort()
	var prompt: String = "Destination town (%s to %s)? " % [keys[0].to_lower(), keys[-1].to_lower()]
	for i: int in keys.size():
		Console.print_line_at(i + 1, 20, " %s) %s" % [keys[i].to_lower(), towns[keys[i]].name], Ink.WHITE)
	Console.print_line_at(keys.size() + 1, 20, "", Ink.WHITE)
	while true:
		var choice: String = User.get_simple_choice(prompt)
		if choice == "":
			break
		choice = choice[0].to_upper()
		for c: String in keys:
			if choice == c:
				Console.restore_screen()
				return towns[c]
	Console.restore_screen()
	return null


func service_haggle(price: int) -> int:
	Profile.instance.message_print("You quickly agree upon the price.")
	Profile.instance.message_print("")
	price += price / 10
	var factor: int = 100
	var greed: int = owner.min_markup
	factor += player.ability_scores[Ability.CHARISMA].cha_price_adjustment()
	var adjust: int = 100 + (greed + factor - 300)
	if adjust < 100:
		adjust = 100
	price = ((price * adjust) + 50) / 100
	if price <= 0:
		return 1
	var prompt: String = "Final Offer : %d" % [price]
	Console.print_at(1, 0, prompt, Ink.WHITE)
	if User.get_check("Accept deal? "):
		Console.play_sound("ShopTransaction")
		return price
	else:
		return Constants.MAX_INT


func say_comment_agree() -> void:
	Profile.instance.message_print(COMMENT_AGREE[Rng.roll_die(COMMENT_AGREE.size()) - 1])


func room_rest(to_dusk: bool) -> void:
	if to_dusk:
		player.game_time.to_next_dusk()
		Profile.instance.message_print("You awake, ready for the night.")
		Profile.instance.message_print("You eat a tasty supper.")
	else:
		player.game_time.to_next_dawn()
		Profile.instance.message_print("You awake refreshed for the new day.")
		Profile.instance.message_print("You eat a hearty breakfast.")
	player.religion.decay_favour()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER)
	player.set_food(Constants.FOOD_GORGED - 1)
	for town: Town in savegame.towns:
		for store: Store in town.stores:
			match store.store_type:
				StoreType.GENERAL_STORE, \
				StoreType.ARMOURY, \
				StoreType.WEAPONSMITH, \
				StoreType.TEMPLE, \
				StoreType.ALCHEMIST, \
				StoreType.MAGIC_STORE, \
				StoreType.BLACK_MARKET, \
				StoreType.BOOKSTORE:
					store.store_maint()
	player.timed_haste = 0
	player.timed_slow = 0
	player.timed_blindness = 0
	player.timed_paralysis = 0
	player.timed_confusion = 0
	player.timed_fear = 0
	player.timed_hallucinations = 0
	player.timed_poison = 0
	player.timed_bleeding = 0
	player.timed_stun = 0
	player.timed_protection_from_evil = 0
	player.timed_invulnerability = 0
	player.timed_heroism = 0
	player.timed_super_heroism = 0
	player.timed_stoneskin = 0
	player.timed_blessing = 0
	player.timed_see_invisibility = 0
	player.timed_etherealness = 0
	player.timed_infravision = 0
	player.timed_acid_resistance = 0
	player.timed_lightning_resistance = 0
	player.timed_fire_resistance = 0
	player.timed_cold_resistance = 0
	player.timed_poison_resistance = 0
	player.health = player.max_health
	player.power = player.max_power
	savegame.new_level_flag = true
	savegame.came_from = LevelStart.START_WALK


func move_house(old_town: int, new_town: int) -> void:
	var old_home: Store
	var new_home: Store
	for store: Store in savegame.towns[old_town].stores:
		if store.store_type == StoreType.HOME:
			old_home = store
			break
	for store: Store in savegame.towns[new_town].stores:
		if store.store_type == StoreType.HOME:
			new_home = store
			break
	if old_home == null:
		return
	if new_home == null:
		return
	new_home.stock_num = old_home.stock_num
	for i: int in old_home.stock.size():
		new_home.stock[i] = old_home.stock[i].get_copy()
		old_home.stock[i] = Item.new()
	old_home.stock_num = 0


func get_stock(pmt: String, i: int, j: int) -> int:
	Profile.instance.message_print()
	var prompt: String = "(Items %s-%s, ESC to exit) %s" % [Util.index_to_label(i), Util.index_to_label(j), pmt]
	while true:
		var command: String = User.get_simple_choice(prompt)
		if command == "":
			Console.clear_row(0)
			return -1
		if command.length() == 1:
			var index: int = Util.label_to_index(command.to_lower())
			if index >= i and index <= j:
				Console.clear_row(0)
				return index
	return -1


func store_shuffle() -> void:
	if (store_type == StoreType.HOME or store_type == StoreType.HALL_OF_RECORDS or
		store_type == StoreType.PAWNBROKERS or store_type == StoreType.EMPTY_LOT ):
		return
	owner = StoreFactory.get_random_owner(store_type)
	for i: int in stock_num:
		var item: Item = stock[i]
		if not item.is_legendary():
			item.discount = 50
		item.identify_flags.clear(IdentifyFlags.FIXED)
		item.inscription = "on sale"


func item_filter_store_will_buy(item: Item) -> bool:
	match store_type:
		StoreType.GENERAL_STORE:
			match item.category:
				ItemCategory.FOOD, \
				ItemCategory.LIGHT, \
				ItemCategory.FLASK, \
				ItemCategory.SPIKE, \
				ItemCategory.SHOT, \
				ItemCategory.ARROW, \
				ItemCategory.BOLT, \
				ItemCategory.DIGGING, \
				ItemCategory.CLOAK, \
				ItemCategory.BOTTLE:
					return item.value() > 0
				_:
					return false
		StoreType.ARMOURY:
			match item.category:
				ItemCategory.BOOTS, \
				ItemCategory.GLOVES, \
				ItemCategory.CROWN, \
				ItemCategory.HELM, \
				ItemCategory.SHIELD, \
				ItemCategory.CLOAK, \
				ItemCategory.SOFT_ARMOUR, \
				ItemCategory.HARD_ARMOUR, \
				ItemCategory.DRAGON_ARMOUR:
					return item.value() > 0
				_:
					return false
		StoreType.WEAPONSMITH:
			match item.category:
				ItemCategory.SHOT, \
				ItemCategory.BOLT, \
				ItemCategory.ARROW, \
				ItemCategory.BOW, \
				ItemCategory.DIGGING, \
				ItemCategory.HAFTED, \
				ItemCategory.POLEARM, \
				ItemCategory.SWORD:
					return item.value() > 0
				_:
					return false
		StoreType.TEMPLE:
			match item.category:
				ItemCategory.LIFE_BOOK, \
				ItemCategory.SCROLL, \
				ItemCategory.POTION, \
				ItemCategory.HAFTED:
					return item.value() > 0
				ItemCategory.POLEARM, \
				ItemCategory.SWORD:
					if item.is_blessed():
						return item.value() > 0
					return false
				_:
					return false
		StoreType.ALCHEMIST:
			match item.category:
				ItemCategory.SCROLL, \
				ItemCategory.POTION:
					return item.value() > 0
				_:
					return false
		StoreType.MAGIC_STORE:
			match item.category:
				ItemCategory.SORCERY_BOOK, \
				ItemCategory.NATURE_BOOK, \
				ItemCategory.CHAOS_BOOK, \
				ItemCategory.DEATH_BOOK, \
				ItemCategory.TAROT_BOOK, \
				ItemCategory.FOLK_BOOK, \
				ItemCategory.CORPOREAL_BOOK, \
				ItemCategory.AMULET, \
				ItemCategory.RING, \
				ItemCategory.STAFF, \
				ItemCategory.WAND, \
				ItemCategory.ROD, \
				ItemCategory.SCROLL, \
				ItemCategory.POTION:
					return item.value() > 0
				_:
					return false
		StoreType.HOME:
			return true
		StoreType.BOOKSTORE:
			match item.category:
				ItemCategory.SORCERY_BOOK, \
				ItemCategory.NATURE_BOOK, \
				ItemCategory.CHAOS_BOOK, \
				ItemCategory.DEATH_BOOK, \
				ItemCategory.LIFE_BOOK, \
				ItemCategory.TAROT_BOOK, \
				ItemCategory.FOLK_BOOK, \
				ItemCategory.CORPOREAL_BOOK:
					return item.value() > 0
				_:
					return false
		StoreType.HALL_OF_RECORDS:
			return false
		StoreType.INN:
			return false
		StoreType.PAWNBROKERS:
			return item.value() > 0
		StoreType.EMPTY_LOT:
			return false
	return false


func sell_haggle(item: Item) -> int:
	var final_ask: int = price_item(item, owner.min_markup, true)
	var purse: int = owner.max_cost
	if final_ask > purse:
		Profile.instance.message_print("Sorry, I can only afford to pay this much.")
		Profile.instance.message_print()
		final_ask = purse
	else:
		Profile.instance.message_print("You quickly agree upon the price.")
		Profile.instance.message_print()
		final_ask -= final_ask / 10
	final_ask *= item.count
	var prompt: String = "Final Offer :  %d" % [final_ask]
	Console.print_at(1, 0, prompt, Ink.WHITE)
	if User.get_check("Accept deal? "):
		Console.play_sound("ShopTransaction")
		return final_ask
	else:
		return -1


func purchase_analyse(price: int, real_value: int, unidentified_price: int) -> void:
	if real_value <= 0 and price > real_value:
		Profile.instance.message_print(COMMENT_OUTRAGED[Rng.roll_die(COMMENT_OUTRAGED.size()) - 1])
	elif real_value < unidentified_price and price > real_value:
		Profile.instance.message_print(COMMENT_ANNOYED[Rng.roll_die(COMMENT_ANNOYED.size()) - 1])
	elif real_value > unidentified_price and real_value < 4 * unidentified_price and price < real_value:
		Profile.instance.message_print(COMMENT_PLEASED[Rng.roll_die(COMMENT_PLEASED.size()) - 1])
	elif real_value > unidentified_price and price < real_value:
		Profile.instance.message_print(COMMENT_DELIGHTED[Rng.roll_die(COMMENT_DELIGHTED.size()) - 1])


func do_store_browse(item: Item) -> void:
	var num: int = 0
	var spells: Array[int]
	for i: int in 64:
		spells.append(0)
	var sub_category: int = item.sub_category
	for spell: int in 32:
		if (Tables.BOOK_SPELL_FLAGS[sub_category] & (1 << spell)) != 0:
			spells[num] = spell
			num += 1
	Console.save_screen()
	player.print_spells(spells, num, 1, 20, Util.spellbook_to_realm(item.category))
	Console.clear_row(0)
	Console.print_at(0, 23, "[Press any key to continue]", Ink.WHITE)
	User.get_key()
	Console.restore_screen()
