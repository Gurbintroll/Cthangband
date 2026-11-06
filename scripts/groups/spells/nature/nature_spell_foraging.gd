# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellForaging
extends Spell


func _init() -> void:
	default_base_failure = 35
	default_level = 4
	default_power_cost = 4
	first_cast_experience = 4
	name = "Foraging"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.set_food(Constants.FOOD_GORGED - 1)


func comment(player: Player) -> String:
	return ""
