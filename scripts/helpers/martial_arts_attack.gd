# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MartialArtsAttack
extends RefCounted

const MA_KNEE: int = 1
const MA_SLOW: int = 2

var chance: int
var damage_dice: int
var damage_dice_sides: int
var description: String
var effect: int
var min_level: int


func _init(desc: String, min_lev: int, ch: int, dd: int, ds: int, eff: int) -> void:
	description = desc
	damage_dice = dd
	damage_dice_sides = ds
	chance = ch
	effect = eff
	min_level = min_lev
