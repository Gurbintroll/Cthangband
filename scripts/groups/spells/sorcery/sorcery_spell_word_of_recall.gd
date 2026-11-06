# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SorcerySpellWordOfRecall
extends Spell


func _init() -> void:
	default_base_failure = 75
	default_level = 25
	default_power_cost = 25
	first_cast_experience = 19
	name = "Word of Recall"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.toggle_recall()


func comment(player: Player) -> String:
	return "delay 1d21+15"
