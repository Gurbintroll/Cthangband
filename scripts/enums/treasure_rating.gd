# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TreasureRating
extends Object

const UNKNOWN: int = 0
const SPECIAL: int = 1
const TREASURE_GALORE: int = 2
const VERITABLE_HOARD: int = 3
const POWERFUL_MAGIC: int = 4
const MAGIC_IN_AIR: int = 5
const WEALTH_FOUND: int = 6
const SIGNIFICANT_TREASURE: int = 7
const NOT_MUCH: int = 8
const NOTHING_OF_WORTH: int = 9
const MEAGRE_PICKINGS: int = 10

static var feeling_text: Array[String] = [
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


static func rating_to_feeling(rating: int) -> int:
	if rating > 100:
		return TREASURE_GALORE
	elif rating > 80:
		return VERITABLE_HOARD
	elif rating > 60:
		return POWERFUL_MAGIC
	elif rating > 40:
		return MAGIC_IN_AIR
	elif rating > 30:
		return WEALTH_FOUND
	elif rating > 20:
		return SIGNIFICANT_TREASURE
	elif rating > 10:
		return NOT_MUCH
	elif rating > 0:
		return NOTHING_OF_WORTH
	else:
		return MEAGRE_PICKINGS
