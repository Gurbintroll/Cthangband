# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name God
extends Resource

const PATRON_MULTIPLIER: int = 2

@export var long_name: String
@export var short_name: String
@export var favour: int
@export var is_patron: bool
@export var god_name: int
@export var resting_favour: int


func set_god_name(name: int) -> void:
	god_name = name
	match name:
		GodName.LOBON:
			short_name = "Lobon"
			long_name = "Lobon, god of youth"
		GodName.NATH_HORTHATH:
			short_name = "Nath-Horthath"
			long_name = "Nath-Horthath, god of war"
		GodName.TAMASH:
			short_name = "Tamash"
			long_name = "Tamash, god of illusion and magic"
		GodName.ZO_KALAR:
			short_name = "Zo-Kalar"
			long_name = "Zo-Kalar, god of birth and death"
		GodName.HAGARG_RYONIS:
			short_name = "Hagarg Ryonis"
			long_name = "Hagarg Ryonis, goddess of beasts"


func adjusted_favour() -> int:
	if favour <= 0:
		return 0
	var adjusted: int = (favour * PATRON_MULTIPLIER) if is_patron else favour
	return min(str(adjusted).length(), 10)
