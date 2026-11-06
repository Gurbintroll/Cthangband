# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Inventory
extends Resource

static var item_filter_useable_spell_book: bool
static var item_filter_category: int

@export var items: Array[Item]
@export var inven_count: int

var player: Player:
	get = _get_player, set = _set_player
var _player: Player


static func item_type_is_easy_know(i: int) -> bool:
	var type: ItemType = SaveGame.instance.item_types.types[i]
	match type.item_category:
		ItemCategory.LIFE_BOOK, ItemCategory.SORCERY_BOOK, ItemCategory.NATURE_BOOK:
			return true
		ItemCategory.CHAOS_BOOK, ItemCategory.DEATH_BOOK, ItemCategory.TAROT_BOOK:
			return true
		ItemCategory.FOLK_BOOK, ItemCategory.CORPOREAL_BOOK, ItemCategory.FLASK:
			return true
		ItemCategory.JUNK, ItemCategory.BOTTLE, ItemCategory.SKELETON:
			return true
		ItemCategory.SPIKE, ItemCategory.FOOD, ItemCategory.POTION:
			return true
		ItemCategory.SCROLL, ItemCategory.ROD:
			return true
		ItemCategory.RING, ItemCategory.AMULET, ItemCategory.LIGHT:
			return type.flags_3.is_set(ItemFlags3.EASY_KNOW)
	return false


static func item_type_has_flavour(type: ItemType) -> bool:
	match type.item_category:
		ItemCategory.AMULET, ItemCategory.RING, ItemCategory.STAFF:
			return true
		ItemCategory.WAND, ItemCategory.SCROLL, ItemCategory.POTION:
			return true
		ItemCategory.ROD:
			return true
		ItemCategory.FOOD:
			return type.sub_category < FoodType.MIN_FOOD
	return false


func _init() -> void:
	for i: int in InventorySlot.TOTAL:
		items.append(Item.new())


func combine_pack() -> void:
	var flag: bool = false
	for i: int in InventorySlot.PACK:
		var item: Item = items[i]
		if item.item_type == null:
			continue
		for j: int in i:
			var other: Item = items[j]
			if other.item_type == null:
				continue
			if other.can_absorb(item):
				flag = true
				other.absorb(item)
				inven_count -= 1
				for k: int in range(i, InventorySlot.PACK):
					items[k] = items[k + 1]
				items[InventorySlot.PACK - 1] = Item.new()
				break
	if flag:
		Profile.instance.message_print("You combine some items in your pack.")


func reorder_pack() -> void:
	var flag: bool = false
	for i: int in InventorySlot.PACK:
		var item: Item = items[i]
		if item.item_type == null:
			continue
		var item_value: int = item.value()
		var other_index: int
		for j: int in InventorySlot.PACK:
			other_index = j
			var other: Item = items[j]
			if other.item_type == null:
				break
			if (
				Util.spellbook_to_realm(item.category) == player.realm_1
				and Util.spellbook_to_realm(other.category) != player.realm_1
			):
				break
			if (
				Util.spellbook_to_realm(other.category) == player.realm_1
				and Util.spellbook_to_realm(item.category) != player.realm_1
			):
				continue
			if (
				Util.spellbook_to_realm(item.category) == player.realm_2
				and Util.spellbook_to_realm(other.category) != player.realm_2
			):
				break
			if (
				Util.spellbook_to_realm(other.category) == player.realm_2
				and Util.spellbook_to_realm(item.category) != player.realm_2
			):
				continue
			if item.category > other.category:
				break
			if item.category < other.category:
				continue
			if not item.is_flavour_aware():
				continue
			if not other.is_flavour_aware():
				break
			if item.sub_category < other.sub_category:
				break
			if item.sub_category > other.sub_category:
				continue
			if not item.is_known():
				continue
			if not other.is_known():
				break
			if item.category == ItemCategory.ROD:
				if item.type_specific_value < other.type_specific_value:
					break
				if item.type_specific_value > other.type_specific_value:
					continue
			var other_value: int = other.value()
			if item_value > other_value:
				break
		if other_index >= i:
			continue
		flag = true
		var swap: Item = items[i]
		for k: int in range(i, other_index, -1):
			items[k] = items[k - 1]
		items[other_index] = swap
	if flag:
		Profile.instance.message_print("You reorder some items in your pack.")


func inven_carry_okay(item: Item) -> bool:
	if inven_count < InventorySlot.PACK:
		return true
	for j: int in InventorySlot.PACK:
		var other: Item = items[j]
		if other.item_type == null:
			continue
		if other.can_absorb(item):
			return true
	return false


## Put an item into the player's pack
func inven_carry(item: Item) -> int:
	var last_item: int = -1
	var other: Item
	# First, check if any existing inventory item can absorb it
	for j: int in InventorySlot.PACK:
		other = items[j]
		if other.item_type == null:
			continue
		# Keep track of the last item in the inventory while we're here
		last_item = j
		if item.can_absorb(other):
			other.absorb(item)
			player.weight_carried += item.count * item.weight
			player.updates_needed.is_set(UpdateFlags.UPDATE_BONUSES)
			return j
	# We need a new slot, so return if there aren't any
	if inven_count > InventorySlot.PACK:
		return -1
	# Look for the first empty inventory slot
	var index: int
	for j: int in InventorySlot.PACK + 1:
		other = items[j]
		if other.item_type == null:
			index = j
			break
	# We don't actually care about the index unless it was the last item
	if index < InventorySlot.PACK:
		# Try to find where to put the item
		# For each property, we continue if we should go after the item, break if we should go before it, or do nothing if we match it
		var item_value: int = item.value()
		for j: int in InventorySlot.PACK:
			other = items[j]
			index = j
			# We go before empty slots
			if other.item_type == null:
				break
			# Spell books are sorted in realm order
			if (
				Util.spellbook_to_realm(item.category) == player.realm_1
				and Util.spellbook_to_realm(other.category) != player.realm_1
			):
				break
			if (
				Util.spellbook_to_realm(other.category) == player.realm_1
				and Util.spellbook_to_realm(item.category) != player.realm_1
			):
				continue
			if (
				Util.spellbook_to_realm(item.category) == player.realm_2
				and Util.spellbook_to_realm(other.category) != player.realm_2
			):
				break
			if (
				Util.spellbook_to_realm(other.category) == player.realm_2
				and Util.spellbook_to_realm(item.category) != player.realm_2
			):
				continue
			# Items are sorted in category order
			if item.category > other.category:
				break
			if item.category < other.category:
				continue
			# Unidentified item flavours come after identified ones
			if not item.is_flavour_aware():
				continue
			if not other.is_flavour_aware():
				break
			# Items are sorted in sub_category order
			if item.sub_category < other.sub_category:
				break
			if item.sub_category > other.sub_category:
				continue
			# Identified items come before unidentified
			if not item.is_known():
				continue
			if not other.is_known():
				break
			# Rods are sorted by time to recharge
			if item.category == ItemCategory.ROD:
				if item.type_specific_value < other.type_specific_value:
					break
				if item.type_specific_value > other.type_specific_value:
					continue
			var other_value: int = other.value()
			if item_value > other_value:
				break
		# We found the item's place, so shuffle all further items up by one to make room
		for k: int in range(last_item, index - 1, -1):
			items[k + 1] = items[k].get_copy()
	# Place a copy of the item in the slot we found for it
	items[index] = item.get_copy()
	item = items[index]
	item.map_row = 0
	item.map_col = 0
	item.next_in_stack = 0
	item.holding_monster_index = 0
	player.weight_carried += item.count * item.weight
	inven_count += 1
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	return index


func inven_damage(item_filter_function: Callable, perc: int) -> int:
	var k: int = 0
	for i: int in InventorySlot.PACK:
		var item: Item = items[i]
		if item.item_type == null:
			continue
		if item.is_artifact() or item.is_legendary():
			continue
		if item_filter_function.call(item):
			var amt: int = 0
			for j: int in range(1, item.count + 1):
				if Rng.roll_die(100) <= perc:
					amt += 1
			if amt != 0:
				var item_name: String = item.description(false, 3)
				var y: String = (
					"All of y" if amt == item.count else ("Some of y" if amt > 1 else "One of y")
				) if item.count > 1 else "Y"
				var w: String = "were" if amt > 1 else "was"
				Profile.instance.message_print(
					"%sour %s (%s) %s destroyed!" % [y, item_name, Util.index_to_label(i), w]
				)
				if item.item_type.item_category == ItemCategory.POTION:
					SaveGame.instance.spell_effects_handler.potion_smash_effect(
						0,
						_player.map_row,
						_player.map_col,
						item.item_sub_category,
					)
				inven_item_increase(i, -amt)
				inven_item_optimise(i)
				k += amt
	return k


func wield_slot(item: Item) -> int:
	match item.category:
		ItemCategory.DIGGING:
			return InventorySlot.DIGGER
		ItemCategory.HAFTED, ItemCategory.POLEARM, ItemCategory.SWORD:
			return InventorySlot.MELEE_WEAPON
		ItemCategory.BOW:
			return InventorySlot.RANGED_WEAPON
		ItemCategory.RING:
			if items[InventorySlot.RIGHT_HAND].item_type == null:
				return InventorySlot.RIGHT_HAND
			return InventorySlot.LEFT_HAND
		ItemCategory.AMULET:
			return InventorySlot.NECK
		ItemCategory.LIGHT:
			return InventorySlot.LIGHT_SOURCE
		ItemCategory.DRAGON_ARMOUR, ItemCategory.HARD_ARMOUR, ItemCategory.SOFT_ARMOUR:
			return InventorySlot.BODY
		ItemCategory.CLOAK:
			return InventorySlot.CLOAK
		ItemCategory.SHIELD:
			return InventorySlot.ARM
		ItemCategory.CROWN, ItemCategory.HELM:
			return InventorySlot.HEAD
		ItemCategory.GLOVES:
			return InventorySlot.HANDS
		ItemCategory.BOOTS:
			return InventorySlot.FEET
	return -1


func show_inventory() -> void:
	var item: Item
	var out_index: Array[int] = []
	out_index.resize(26)
	out_index.fill(0)
	var out_colour: Array[int]
	out_colour.resize(26)
	out_colour.fill(0)
	var out_desc: Array[String]
	out_desc.resize(26)
	out_desc.fill("")
	var col: int = SaveGame.instance.item_display_column
	var max_length: int = 79 - col
	var lim: int = 79 - 3
	lim -= 9
	lim -= 2
	var z: int = 0
	for i: int in InventorySlot.PACK:
		item = items[i]
		if item.item_type == null:
			continue
		z = i + 1
	var k: int = 0
	for i: int in z:
		item = items[i]
		if not item_matches_filter(item):
			continue
		var item_name: String = item.description(true, 3)
		if item_name.length() > lim:
			item_name = item_name.substr(0, lim)
		out_index[k] = i
		out_colour[k] = Util.item_category_colour(item.category)
		out_desc[k] = item_name
		var l: int = out_desc[k].length() + 5
		l += 9
		l += 2
		if l > max_length:
			max_length = l
		k += 1
	col = 0 if max_length > 76 else 79 - max_length
	for j: int in k:
		var i: int = out_index[j]
		item = items[i]
		Console.clear_to_row_end(j + 1, col - 2 if col != 0 else col)
		var text: String = Util.index_to_label(i) + ")"
		Console.print_at(j + 1, col, text, Ink.WHITE)
		var a: int = item.item_type.colour
		var c: String = item.item_type.character
		Console.print_at(j + 1, col + 3, c, a)
		Console.print_at(j + 1, col + 5, out_desc[j], out_colour[j])
		var wgt: int = item.weight * item.count
		text = "%2d.%d lb" % [wgt / 10, wgt % 10]
		Console.print_at(j + 1, 71, text, Ink.WHITE)
	if k != 0 and k < 26:
		Console.clear_to_row_end(k + 1, col - 2 if col != 0 else col)
	SaveGame.instance.item_display_column = col


func item_matches_filter(item: Item) -> bool:
	if item.item_type == null:
		return false
	if SaveGame.instance.item_filter_all:
		return true
	if item.category == ItemCategory.GOLD:
		return false
	if item_filter_useable_spell_book:
		return check_book_realm(item.category)
	if item_filter_category != 0:
		if item_filter_category != item.category:
			return false
	if SaveGame.instance.item_filter_function != Callable():
		if not SaveGame.instance.item_filter_function.call(item):
			return false
	return true


func check_book_realm(item_category: int) -> bool:
	return (
		Util.realm_to_spellbook(_player.realm_1) == item_category
		or Util.realm_to_spellbook(_player.realm_2) == item_category
	)


func show_equipment() -> void:
	var item: Item
	var out_index: Array[int] = []
	out_index.resize(26)
	out_index.fill(0)
	var out_colour: Array[int]
	out_colour.resize(26)
	out_colour.fill(0)
	var out_desc: Array[String]
	out_desc.resize(26)
	out_desc.fill("")
	var col: int = SaveGame.instance.item_display_column
	var max_length: int = 79 - col
	var lim: int = 79 - 3
	lim -= 14 + 2
	lim -= 9
	lim -= 2
	var k: int = 0
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		item = items[i]
		if not item_matches_filter(item):
			continue
		var item_name: String = item.description(true, 3)
		if item_name.length() > lim:
			item_name = item_name.substr(0, lim)
		out_index[k] = i
		out_colour[k] = Util.item_category_colour(item.category)
		out_desc[k] = item_name
		var l: int = out_desc[k].length() + 5
		l += 16
		l += 9
		l += 2
		if l > max_length:
			max_length = l
		k += 1
	col = 0 if max_length > 76 else 79 - max_length
	for j: int in k:
		var i: int = out_index[j]
		item = items[i]
		Console.clear_to_row_end(j + 1, col - 2 if col != 0 else col)
		var text: String = Util.index_to_label(i) + ")"
		Console.print_at(j + 1, col, text, Ink.WHITE)
		if item.item_type != null:
			var a: int = item.item_type.colour
			var c: String = item.item_type.character
			Console.print_at(j + 1, col + 3, c, a)
		else:
			Console.print_at(j + 1, col + 3, " ", Ink.WHITE)
		Console.print_at(j + 1, col + 5, mention_use(i), Ink.WHITE)
		Console.print_at(j + 1, col + 21, out_desc[j], out_colour[j])
		var wgt: int = item.weight * item.count
		text = "%2d.%d lb" % [wgt / 10, wgt % 10]
		Console.print_at(j + 1, 71, text, Ink.WHITE)
	if k != 0 and k < 23:
		Console.clear_to_row_end(k + 1, col - 2 if col != 0 else col)
	SaveGame.instance.item_display_column = col


func mention_use(slot: int) -> String:
	var p: String
	match slot:
		InventorySlot.MELEE_WEAPON:
			p = "Wielding"
		InventorySlot.RANGED_WEAPON:
			p = "Shooting"
		InventorySlot.DIGGER:
			p = "Digging with"
		InventorySlot.LEFT_HAND:
			p = "On left hand"
		InventorySlot.RIGHT_HAND:
			p = "On right hand"
		InventorySlot.NECK:
			p = "Around neck"
		InventorySlot.LIGHT_SOURCE:
			p = "Light source"
		InventorySlot.BODY:
			p = "On body"
		InventorySlot.CLOAK:
			p = "About body"
		InventorySlot.ARM:
			p = "On arm"
		InventorySlot.HEAD:
			p = "On head"
		InventorySlot.HANDS:
			p = "On hands"
		InventorySlot.FEET:
			p = "On feet"
		_:
			p = "In pack"
	if slot == InventorySlot.MELEE_WEAPON:
		var item: Item = items[slot]
		if player.ability_scores[Ability.STRENGTH].str_max_weapon_weight() < item.weight / 10:
			p = "Just lifting"
	if slot == InventorySlot.RANGED_WEAPON:
		var item: Item = items[slot]
		if player.ability_scores[Ability.STRENGTH].str_max_weapon_weight() < item.weight / 10:
			p = "Just holding"
	return p


func get_item_okay(i: int) -> bool:
	if i < 0 or i > InventorySlot.TOTAL:
		return false
	return item_matches_filter(items[i])


func label_to_inven(label: String) -> int:
	return "abcdefghijklmnopqrstuvwxyz".find(label)


func label_to_equip(label: String) -> int:
	return "___________________________abcdefghijklmnopqrstuvwxyz".find(label)


func report_charge_usage_from_inventory (item_index: int) ->void:
	var item: Item = items[item_index]
	if item.category != ItemCategory.STAFF and item.category != ItemCategory.WAND:
		return
	if not item.is_known():
		return
	var s: String = "" if item.type_specific_value == 1 else "s"
	Profile.instance.message_print("You have %d charge%s remaining." % [item.type_specific_value, s])


func inven_drop(item_index: int, amount: int) -> void:
	if amount <= 0:
		return
	var item: Item = items[item_index]
	if amount > item.count:
		amount = item.count
	if item_index >= InventorySlot.MELEE_WEAPON:
		item_index = inven_take_off(item_index, amount)
		item = items[item_index]
	var dropped: Item = item.get_copy()
	dropped.count = amount
	var item_name: String = dropped.description(true, 3)
	Profile.instance.message_print(
		"You drop %s (%s)." % [item_name, Util.index_to_label(item_index)]
	)
	SaveGame.instance.dungeon_level.dungeon_items.drop_near(
		dropped,
		0,
		player.map_row,
		player.map_col,
	)
	inven_item_increase(item_index, -amount)
	inven_item_describe(item_index)
	inven_item_optimise(item_index)


func inven_item_describe(item_index: int) -> void:
	var item: Item = items[item_index]
	var item_name: String = item.description(true, 3)
	Profile.instance.message_print("You have %s." % [item_name])


func inven_item_increase(item_index: int, amount: int) -> void:
	var item: Item = items[item_index]
	amount += item.count
	amount = clampi(amount, 0, 255)
	amount -= item.count
	if amount != 0:
		item.count += amount
		player.weight_carried += amount * item.weight
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_POWER)
		player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE)


func inven_item_optimise(item_index: int) -> void:
	var item: Item = items[item_index]
	if item.item_type == null:
		return
	if item.count > 0:
		return
	if item_index < InventorySlot.MELEE_WEAPON:
		inven_count -= 1
		for i: int in range(item_index, InventorySlot.PACK):
			items[i] = items[i + 1]
		items[InventorySlot.PACK] = Item.new()
	else:
		items[item_index] = Item.new()
		player.updates_needed.set_bits(
			UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_TORCH_RADIUS | UpdateFlags.UPDATE_POWER
		)


func inven_take_off(item_index: int, amount: int) -> int:
	var act: String
	var item: Item = items[item_index]
	if amount <= 0:
		return -1
	if amount > item.count:
		amount = item.count
	var taken_off: Item = item.get_copy()
	taken_off.count = amount
	var item_name: String = taken_off.description(true, 3)
	if item_index == InventorySlot.MELEE_WEAPON:
		act = "You were wielding"
	elif item_index == InventorySlot.RANGED_WEAPON:
		act = "You were holding"
	elif item_index == InventorySlot.LIGHT_SOURCE:
		act = "You were holding"
	else:
		act = "You were wearing"
	inven_item_increase(item_index, -amount)
	inven_item_optimise(item_index)
	var slot: int = inven_carry(taken_off)
	Profile.instance.message_print("%s %s (%s)." % [act, item_name, Util.index_to_label(slot)])
	return slot


func _get_player() -> Player:
	if _player == null:
		_player = SaveGame.instance.player
	return _player


func _set_player(value: Player) -> void:
	_player = value
