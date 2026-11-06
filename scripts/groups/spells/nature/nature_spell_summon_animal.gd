# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellSummonAnimal
extends Spell


func _init() -> void:
	default_base_failure = 90
	default_level = 25
	default_power_cost = 25
	first_cast_experience = 30
	name = "Summon Animal"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_ANIMAL, true):
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return ""
