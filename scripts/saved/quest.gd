# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Quest
extends Resource

@export var discovered: bool
@export var dungeon: int
@export var killed: int
@export var depth: int
@export var monster_type_index: int
@export var to_kill: int


func is_active() -> bool:
	return (depth != 0 and killed < to_kill)
