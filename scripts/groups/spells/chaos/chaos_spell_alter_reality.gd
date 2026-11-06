# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ChaosSpellAlterReality
extends Spell


func _init() -> void:
	default_base_failure = 85
	default_level = 32
	default_power_cost = 25
	first_cast_experience = 150
	name = "Alter Reality"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	Profile.instance.message_print("The world changes!")
	savegame.is_auto_save = true
	savegame.command_save_game()
	savegame.is_auto_save = false
	savegame.new_level_flag = true
	savegame.came_from = LevelStart.START_RANDOM


func comment(player: Player) -> String:
	return ""
