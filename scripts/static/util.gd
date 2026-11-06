# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Util
extends Object
## Miscellaneous static utility functions, such as those for string manipulation

# The extra 'z' is needed for when a pack overflows
const ALPHABET: String = "abcdefghijklmnopqrstuvwzyzz"


## Pad a string to a larger size so that the original string is in the centre
static func pad_center(text: String, width: int) -> String:
	var spaces_needed: int = width - text.length()
	var left_pad: int = spaces_needed / 2 + text.length()
	text = text.lpad(left_pad)
	text = text.rpad(width)
	return text


## Convert an ability score value to a printable string
static func stat_to_string(stat: int) -> String:
	if stat > 18:
		var bonus: int = stat - 18
		if bonus > 220:
			return "   40+"
		bonus = (bonus - 1) / 10
		return "    %2s" % [bonus + 19]
	return "    %2s" % [stat]


static func to_roman(number: int, for_generation: bool) -> String:
	var numerals: Dictionary[int, String] = {
		1000: "M",
		900: "CM",
		500: "D",
		400: "CD",
		100: "C",
		90: "XC",
		50: "L",
		40: "XL",
		10: "X",
		9: "IX",
		5: "V",
		4: "IV",
		1: "I",
	}
	var roman: String
	# If we're for the generation of a character, we want to skip 'I'
	# (you're simply 'John', not 'John I') and prefix with a space
	# if not 'I'
	if for_generation:
		if number == 1:
			return ""
		else:
			roman = " "
	# Simply run down the list of numerals adding them till we run out of number
	for numeral:int in numerals:
		while number >= numeral:
			roman += numerals[numeral]
			number -= numeral
	return roman


## Convert a numeric inventory index (0-37) to a pack or equipment slot
static func index_to_label(index: int) -> String:
	# If it's in our pack, just return the pack slot
	if index < InventorySlot.MELEE_WEAPON:
		return ALPHABET[index]
	# It's outside our pack, so return the equipment slot
	return ALPHABET[index - InventorySlot.MELEE_WEAPON]


static func label_to_index(label: String) -> int:
	return ALPHABET.find(label)


## Work out the realm of a spell book by its item category
static func spellbook_to_realm(category: int) -> int:
	match category:
		ItemCategory.LIFE_BOOK:
			return Realm.LIFE
		ItemCategory.SORCERY_BOOK:
			return Realm.SORCERY
		ItemCategory.NATURE_BOOK:
			return Realm.NATURE
		ItemCategory.CHAOS_BOOK:
			return Realm.CHAOS
		ItemCategory.DEATH_BOOK:
			return Realm.DEATH
		ItemCategory.TAROT_BOOK:
			return Realm.TAROT
		ItemCategory.FOLK_BOOK:
			return Realm.FOLK
		ItemCategory.CORPOREAL_BOOK:
			return Realm.CORPOREAL
		_:
			return Realm.NONE


static func realm_to_spellbook(realm: int) -> int:
	match realm:
		Realm.LIFE:
			return ItemCategory.LIFE_BOOK
		Realm.SORCERY:
			return ItemCategory.SORCERY_BOOK
		Realm.NATURE:
			return ItemCategory.NATURE_BOOK
		Realm.CHAOS:
			return ItemCategory.CHAOS_BOOK
		Realm.DEATH:
			return ItemCategory.DEATH_BOOK
		Realm.TAROT:
			return ItemCategory.TAROT_BOOK
		Realm.FOLK:
			return ItemCategory.FOLK_BOOK
		Realm.CORPOREAL:
			return ItemCategory.CORPOREAL_BOOK
		_:
			return ItemCategory.NONE


static func is_vowel(letter: String) -> bool:
	return ("aeiouAEIOU".contains(letter))


static func is_letter(letter: String) -> bool:
	return ("abcdefghiklmnopqrstuvwxyz".contains(letter))


static func pluralise_monster_name(name: String) -> String:
	var plural: String
	# "X of Y" -> "Xs of Y"
	if name.contains(" of "):
		var i: int = name.find(" of ")
		plural = name.left(i)
		# "XS of Y -> XSes of Y"
		if plural.ends_with("s"):
			plural += "es"
		# "Young of Y" or "Spawn of Y" are unchanged
		elif plural.ends_with("ng") or plural.ends_with("wn"):
			# Plural matches singular
			pass
		else:
			plural += "s"
		plural += name.substr(i)
	# "Pile of X coins" -> "Piles of X coins"
	elif name.contains("coins"):
		plural = "piles of " + name
	# "Manes" and "Mi-Go" are their own plurals
	elif name == "Manes" or name == "Mi-Go":
		plural = name
	# "Homonculous" -> "Homonculi"
	elif name == "Homonculous":
		plural = "Homonculi"
	# "Stairway to hell" -> "Stairways to hell"
	elif name == "Stairway to hell":
		plural = "Stairways to hell"
	# "Harpy" or similar -> "Harpies" or similar
	elif name.ends_with("y"):
		plural = name.left(name.length() - 1) + "ies"
	# "Mouse" or similar -> "Mice" or similar
	elif name.ends_with("ouse"):
		plural = name.left(name.length() - 4) + "ice"
	# "Pukelman" -> "Pukelmen"
	elif name.ends_with("kelman") or name.ends_with(" man"):
		plural = name.left(name.length() - 2) + "en"
	# "X child" -> "X children"
	elif name.ends_with("hild"):
		plural = name + "ren"
	# "X vortex" -> "X vortices"
	elif name.ends_with("ex"):
		plural = name.left(name.length() - 2) + "ices"
	# "X wolf" and "X thief" -> "X wolves" and "X thieves"
	elif name.ends_with("olf") or name.ends_with("ief"):
		plural = name.left(name.length() - 1) + "ves"
	# "Leech" (or similar) -> "Leeches"
	elif name.ends_with("ch") or name.ends_with("s"):
		plural = name + "es"
	# If all else fails, just add an 's'.
	else:
		plural = name + "s"
	return plural


static func item_category_colour(item_category: int) -> int:
	match item_category:
		ItemCategory.SKELETON:
			return Ink.BEIGE
		ItemCategory.BOTTLE, \
		ItemCategory.JUNK:
			return Ink.WHITE
		ItemCategory.CHEST:
			return Ink.GREY
		ItemCategory.SHOT, \
		ItemCategory.BOLT, \
		ItemCategory.ARROW:
			return Ink.BRIGHT_BROWN
		ItemCategory.LIGHT:
			return Ink.BRIGHT_YELLOW
		ItemCategory.SPIKE:
			return Ink.GREY
		ItemCategory.BOW:
			return Ink.BROWN
		ItemCategory.DIGGING:
			return Ink.GREY
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.SWORD:
			return Ink.BRIGHT_WHITE
		ItemCategory.BOOTS, \
		ItemCategory.GLOVES, \
		ItemCategory.CROWN, \
		ItemCategory.HELM, \
		ItemCategory.SHIELD, \
		ItemCategory.CLOAK:
			return Ink.BRIGHT_BROWN
		ItemCategory.SOFT_ARMOUR, \
		ItemCategory.HARD_ARMOUR, \
		ItemCategory.DRAGON_ARMOUR:
			return Ink.GREY
		ItemCategory.AMULET:
			return Ink.ORANGE
		ItemCategory.RING:
			return Ink.GOLD
		ItemCategory.STAFF:
			return Ink.PURPLE
		ItemCategory.WAND:
			return Ink.CHARTREUSE
		ItemCategory.ROD:
			return Ink.TURQUOISE
		ItemCategory.SCROLL:
			return Ink.BRIGHT_BEIGE
		ItemCategory.POTION:
			return Ink.BLUE
		ItemCategory.FLASK:
			return Ink.YELLOW
		ItemCategory.FOOD:
			return Ink.GREEN
		ItemCategory.LIFE_BOOK:
			return Ink.BRIGHT_WHITE
		ItemCategory.SORCERY_BOOK:
			return Ink.BRIGHT_BLUE
		ItemCategory.NATURE_BOOK:
			return Ink.BRIGHT_GREEN
		ItemCategory.CHAOS_BOOK:
			return Ink.BRIGHT_RED
		ItemCategory.DEATH_BOOK:
			return Ink.BLACK
		ItemCategory.TAROT_BOOK:
			return Ink.PINK
		ItemCategory.FOLK_BOOK:
			return Ink.BRIGHT_PURPLE
		ItemCategory.CORPOREAL_BOOK:
			return Ink.BRIGHT_YELLOW
	return Ink.WHITE
