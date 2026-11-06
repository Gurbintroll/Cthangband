# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Ink
extends Object

const BACKGROUND: int = 0
const BEIGE: int = 1
const BLACK: int = 2
const BLUE: int = 3
const BRIGHT_BEIGE: int = 4
const BRIGHT_BLUE: int = 5
const BRIGHT_BROWN: int = 6
const BRIGHT_CHARTREUSE: int = 7
const BRIGHT_GREEN: int = 8
const BRIGHT_GREY: int = 9
const BRIGHT_ORANGE: int = 10
const BRIGHT_PINK: int = 11
const BRIGHT_PURPLE: int = 12
const BRIGHT_RED: int = 13
const BRIGHT_TURQUOISE: int = 14
const BRIGHT_WHITE: int = 15
const BRIGHT_YELLOW: int = 16
const BROWN: int = 17
const CHARTREUSE: int = 18
const COPPER: int = 19
const DIAMOND: int = 20
const GOLD: int = 21
const GREEN: int = 22
const GREY: int = 23
const ORANGE: int = 24
const PINK: int = 25
const PURPLE: int = 26
const RED: int = 27
const SILVER: int = 28
const TURQUOISE: int = 29
const WHITE: int = 30
const YELLOW: int = 31


static func dim_colour(colour: int) -> int:
	match colour:
		Ink.BRIGHT_BLUE:
			return Ink.BLUE
		Ink.BRIGHT_GREEN:
			return Ink.GREEN
		Ink.BRIGHT_RED:
			return Ink.RED
		Ink.BRIGHT_WHITE:
			return Ink.WHITE
		Ink.BRIGHT_BEIGE:
			return Ink.BEIGE
		Ink.BRIGHT_CHARTREUSE:
			return Ink.CHARTREUSE
		Ink.BRIGHT_GREY:
			return Ink.GREY
		Ink.BRIGHT_ORANGE:
			return Ink.ORANGE
		Ink.BRIGHT_YELLOW:
			return Ink.YELLOW
		Ink.BRIGHT_BROWN:
			return Ink.BROWN
		Ink.BRIGHT_TURQUOISE:
			return Ink.TURQUOISE
		Ink.BRIGHT_PINK:
			return Ink.PINK
		Ink.GREY:
			return Ink.BLACK
		Ink.WHITE:
			return Ink.GREY
		Ink.YELLOW:
			return Ink.BRIGHT_BROWN
	return colour
