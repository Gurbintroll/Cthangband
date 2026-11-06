# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Room
extends RefCounted

var row_start: int
var row_end: int
var col_start: int
var col_end: int
var min_level: int


func _init(rs: int, re: int, cs: int, ce: int, ml: int) -> void:
	row_start = rs
	row_end = re
	col_start = cs
	col_end = ce
	min_level = ml
