# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DangerRating
extends Object

const UNKNOWN: int = 0
const SPECIAL: int = 1
const HORRIBLE_VISIONS: int = 2
const VERY_DANGEROUS: int = 3
const VERY_BAD: int = 4
const BAD: int = 5
const NERVOUS: int = 6
const UNSAFE: int = 7
const DONT_LIKE: int = 8
const REASONABLY_SAFE: int = 9
const BORING: int = 10

static var feeling_text: Array[String] = [
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


static func rating_to_feeling(rating: int) -> int:
	if rating > 100:
		return HORRIBLE_VISIONS
	elif rating > 80:
		return VERY_DANGEROUS
	elif rating > 60:
		return VERY_BAD
	elif rating > 40:
		return BAD
	elif rating > 30:
		return NERVOUS
	elif rating > 20:
		return UNSAFE
	elif rating > 10:
		return DONT_LIKE
	elif rating > 0:
		return REASONABLY_SAFE
	else:
		return BORING
