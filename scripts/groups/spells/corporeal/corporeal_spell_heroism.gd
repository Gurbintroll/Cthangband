# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CorporealSpellHeroism
extends Spell


func _init() -> void:
	default_base_failure = 40
	default_level = 14
	default_power_cost = 10
	first_cast_experience = 20
	name = "Heroism"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.set_timed_heroism(player.timed_heroism + Rng.roll_die(25) + 25)
	player.restore_health(10)
	player.set_timed_fear(0)


func comment(player: Player) -> String:
	return "dur 1d25+25"
