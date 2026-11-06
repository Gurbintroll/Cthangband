# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemCategoryDescription
extends RefCounted

var item_category: int
var name: String

static var all: Array[ItemCategoryDescription] = [
	ItemCategoryDescription.new(ItemCategory.HAFTED, "Hafted Weapon"),
	ItemCategoryDescription.new(ItemCategory.POLEARM, "Polearm"),
	ItemCategoryDescription.new(ItemCategory.SWORD, "Sword"),
	ItemCategoryDescription.new(ItemCategory.BOW, "Bow"),
	ItemCategoryDescription.new(ItemCategory.ARROW, "Arrow"),
	ItemCategoryDescription.new(ItemCategory.BOLT, "Bolt"),
	ItemCategoryDescription.new(ItemCategory.SHOT, "Shot"),
	ItemCategoryDescription.new(ItemCategory.DIGGING, "Digging Tool"),
	ItemCategoryDescription.new(ItemCategory.LIGHT, "Light Source"),
	ItemCategoryDescription.new(ItemCategory.CHEST, "Chest"),
	ItemCategoryDescription.new(ItemCategory.SOFT_ARMOUR, "Soft Armour"),
	ItemCategoryDescription.new(ItemCategory.HARD_ARMOUR, "Hard Armour"),
	ItemCategoryDescription.new(ItemCategory.DRAGON_ARMOUR, "Dragon Scale Mail"),
	ItemCategoryDescription.new(ItemCategory.CLOAK, "Cloak"),
	ItemCategoryDescription.new(ItemCategory.SHIELD, "Shield"),
	ItemCategoryDescription.new(ItemCategory.CROWN, "Crown"),
	ItemCategoryDescription.new(ItemCategory.HELM, "Helm"),
	ItemCategoryDescription.new(ItemCategory.GLOVES, "Gloves"),
	ItemCategoryDescription.new(ItemCategory.BOOTS, "Boots"),
	ItemCategoryDescription.new(ItemCategory.AMULET, "Amulet"),
	ItemCategoryDescription.new(ItemCategory.POTION, "Potion"),
	ItemCategoryDescription.new(ItemCategory.RING, "Ring"),
	ItemCategoryDescription.new(ItemCategory.ROD, "Rod"),
	ItemCategoryDescription.new(ItemCategory.SCROLL, "Scroll"),
	ItemCategoryDescription.new(ItemCategory.STAFF, "Staff"),
	ItemCategoryDescription.new(ItemCategory.WAND, "Wand"),
	ItemCategoryDescription.new(ItemCategory.CHAOS_BOOK, "Chaos Spellbook"),
	ItemCategoryDescription.new(ItemCategory.CORPOREAL_BOOK, "Corporeal Spellbook"),
	ItemCategoryDescription.new(ItemCategory.DEATH_BOOK, "Death Spellbook"),
	ItemCategoryDescription.new(ItemCategory.FOLK_BOOK, "Folk Spellbook"),
	ItemCategoryDescription.new(ItemCategory.LIFE_BOOK, "Life Spellbook"),
	ItemCategoryDescription.new(ItemCategory.NATURE_BOOK, "Nature Spellbook"),
	ItemCategoryDescription.new(ItemCategory.SORCERY_BOOK, "Sorcery Spellbook"),
	ItemCategoryDescription.new(ItemCategory.TAROT_BOOK, "Tarot Spellbook"),
	ItemCategoryDescription.new(ItemCategory.BOTTLE, "Bottle"),
	ItemCategoryDescription.new(ItemCategory.FLASK, "Flask"),
	ItemCategoryDescription.new(ItemCategory.FOOD, "Food"),
	ItemCategoryDescription.new(ItemCategory.JUNK, "Junk"),
	ItemCategoryDescription.new(ItemCategory.SKELETON, "Skeleton"),
	ItemCategoryDescription.new(ItemCategory.SPIKE, "Spike")
	]

func _init(the_item_category: int, the_name: String) -> void:
	item_category = the_item_category
	name = the_name
