# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellBattleFrenzy
extends Spell


func _init() -> void:
	default_base_failure = 75
	default_level = 30
	default_power_cost = 25
	first_cast_experience = 50
	name = "Battle Frenzy"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(25) + 25)
	player.restore_health(30)
	player.set_timed_fear(0)
	if player.timed_haste != 0:
		player.set_timed_haste(Rng.roll_die(20 + player.level / 2) + player.level / 2)
	else:
		player.set_timed_haste(player.timed_haste + Rng.roll_die(5))


func comment(player: Player) -> String:
	return "dur 1d25+25"
