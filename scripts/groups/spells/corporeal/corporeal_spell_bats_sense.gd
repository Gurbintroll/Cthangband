# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CorporealSpellBatsSense
extends Spell


func _init() -> void:
	default_base_failure = 30
	default_level = 3
	default_power_cost = 3
	first_cast_experience = 1
	name = "Bat's Sense"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	dungeon_level.map_area()


func comment(player: Player) -> String:
	return ""
