# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DisplayCell
extends Label
## A single cell in the display, capable of displaying one character

var character: String
var colour: Color
var changed: bool


func set_both(new_character: String, new_colour: Color) -> void:
	if new_character != character:
		changed = true
		character = new_character
	if new_colour != colour:
		colour = new_colour
		changed = true


func refresh() -> void:
	if changed:
		text = character
		modulate = colour
		changed = false
