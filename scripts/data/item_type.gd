# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemType
extends Resource

const GREAT_OBJECT_CHANCE: int = 20

static var item_validation_function: Callable = Callable()

@export var ac: int
@export var chance: Array[int]
@export var character: String
@export var colour: int
@export var cost: int
@export var damage_dice: int
@export var damage_dice_sides: int
@export var easy_know: bool
@export var flags_1: FlagSet
@export var flags_2: FlagSet
@export var flags_3: FlagSet
@export var flavour_aware: bool
@export var has_flavour: bool
@export var item_category: int
@export var level: int
@export var locale: Array[int]
@export var name: String
@export var type_specific_value: int
@export var stompable: Array[bool]
@export var sub_category: int
@export var to_ac: int
@export var to_damage: int
@export var to_hit: int
@export var tried: bool
@export var weight: int


func get_copy_for_new_savegame() -> ItemType:
	var new_type: ItemType = ItemType.new()
	new_type.ac = ac
	new_type.chance.append_array(chance)
	new_type.character = character
	new_type.colour = colour
	new_type.cost = cost
	new_type.damage_dice = damage_dice
	new_type.damage_dice_sides = damage_dice_sides
	new_type.easy_know = easy_know
	new_type.flags_1 = FlagSet.new()
	new_type.flags_1.copy(flags_1)
	new_type.flags_2 = FlagSet.new()
	new_type.flags_2.copy(flags_2)
	new_type.flags_3 = FlagSet.new()
	new_type.flags_3.copy(flags_3)
	new_type.flavour_aware = flavour_aware
	new_type.has_flavour = has_flavour
	new_type.item_category = item_category
	new_type.level = level
	new_type.locale.append_array(locale)
	new_type.name = name
	new_type.stompable.append_array(stompable)
	new_type.sub_category = sub_category
	new_type.to_ac = to_ac
	new_type.to_damage = to_damage
	new_type.to_hit = to_hit
	new_type.type_specific_value = type_specific_value
	new_type.tried = tried
	new_type.weight = weight
	return new_type


static func random_item_type(desired_level: int) -> ItemType:
	var table: Array[FindChance] = SaveGame.instance.random_item_table
	if desired_level > 0:
		if Rng.roll_die(GREAT_OBJECT_CHANCE) == 1:
			desired_level = 1 + desired_level * DungeonLevel.MAX_DEPTH / Rng.roll_die(DungeonLevel.MAX_DEPTH)
	var total: int = 0
	for i: int in SaveGame.instance.random_item_table_size:
		if table[i].level > desired_level:
			break
		table[i].final_probability = 0
		var type_index: int = table[i].index
		var item_type: ItemType = SaveGame.instance.item_types.types[type_index]
		if SaveGame.instance.dungeon_level != null:
			if SaveGame.instance.dungeon_level.opening_chest and item_type.item_category == ItemCategory.CHEST:
				continue
		table[i].final_probability = table[i].filtered_probability
		total += table[i].final_probability
	if total <= 0:
		return null
	var roll: int = Rng.roll_die(total) - 1
	var index: int = 0
	for i: int in SaveGame.instance.random_item_table_size:
		if roll < table[i].final_probability:
			index = i
			break
		roll -= table[i].final_probability
	var p: int = Rng.roll_die(100)
	if p <= 60:
		var previous: int = index
		roll = Rng.roll_die(total) - 1
		for i: int in SaveGame.instance.random_item_table_size:
			if roll < table[i].final_probability:
				index = i
				break
			roll -= table[i].final_probability
		if table[previous].level < table[index].level:
			index = previous
	if p <= 10:
		var previous: int = index
		roll = Rng.roll_die(total) - 1
		for i: int in SaveGame.instance.random_item_table_size:
			if roll < table[i].final_probability:
				index = i
				break
			roll -= table[i].final_probability
		if table[previous].level < table[index].level:
			index = previous
	return SaveGame.instance.item_types.types[table[index].index]


static func prepare_random_item_table() -> void:
	for entry: FindChance in SaveGame.instance.random_item_table:
		if item_validation_function.is_null() == false:
			# If we have a validation function then monsters that fail validation have zero probability
			if item_validation_function.call(entry.index):
				entry.filtered_probability = entry.base_probability
			else:
				entry.filtered_probability = 0
		else:
			# If we have no validation function then all monsters have their base probability
			entry.filtered_probability = entry.base_probability


static func type_is_good(type_index: int) -> bool:
	var type: ItemType = SaveGame.instance.item_types.types[type_index]
	match type.item_category:
		ItemCategory.HARD_ARMOUR, \
		ItemCategory.SOFT_ARMOUR, \
		ItemCategory.DRAGON_ARMOUR, \
		ItemCategory.SHIELD, \
		ItemCategory.CLOAK, \
		ItemCategory.BOOTS, \
		ItemCategory.GLOVES, \
		ItemCategory.HELM, \
		ItemCategory.CROWN:
			return type.to_ac >= 0
		ItemCategory.BOW, \
		ItemCategory.SWORD, \
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.DIGGING:
			if type.to_hit < 0:
				return false
			if type.to_damage < 0:
				return false
			return true
		ItemCategory.BOLT, \
		ItemCategory.ARROW:
			return true
		ItemCategory.LIFE_BOOK, \
		ItemCategory.SORCERY_BOOK, \
		ItemCategory.NATURE_BOOK, \
		ItemCategory.CHAOS_BOOK, \
		ItemCategory.DEATH_BOOK, \
		ItemCategory.TAROT_BOOK, \
		ItemCategory.CORPOREAL_BOOK:
			return type.sub_category >= ItemSubCategory.BOOK_MIN_GOOD
		ItemCategory.RING:
			return type.sub_category == RingType.SPEED
		ItemCategory.AMULET:
			if type.sub_category == AmuletType.THE_MAGI:
				return true
			if type.sub_category == AmuletType.RESISTANCE:
				return true
			return false
	return false


func has_quality() -> bool:
	match item_category:
		ItemCategory.ARROW, ItemCategory.BOLT, ItemCategory.BOOTS:
			return true
		ItemCategory.BOW, ItemCategory.CLOAK, ItemCategory.CROWN:
			return true
		ItemCategory.DIGGING, ItemCategory.DRAGON_ARMOUR, ItemCategory.GLOVES:
			return true
		ItemCategory.HAFTED, ItemCategory.HARD_ARMOUR, ItemCategory.HELM:
			return true
		ItemCategory.POLEARM, ItemCategory.SHIELD, ItemCategory.SHOT:
			return true
		ItemCategory.SOFT_ARMOUR, ItemCategory.SWORD:
			return true
		ItemCategory.LIGHT:
			return (sub_category == LightType.ORB)
		_:
			return false
