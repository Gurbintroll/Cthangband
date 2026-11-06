# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellRayOfSunlight
extends Spell


func _init() -> void:
	default_base_failure = 30
	default_level = 9
	default_power_cost = 6
	first_cast_experience = 6
	name = "Ray of Sunlight"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	Profile.instance.message_print("A line of sunlight appears.")
	savegame.spell_effects_handler.light_line(dir)


func comment(player: Player) -> String:
	return "dam 6d8"
