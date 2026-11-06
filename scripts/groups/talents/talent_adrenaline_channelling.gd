# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentAdrenalineChannelling
extends Talent


func initialise(profession: Profession) -> void:
	name = "Adrenaline Channelling"
	level = 23
	power_cost = 15
	base_failure = 50


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	player.set_timed_fear(0)
	player.set_timed_stun(0)
	player.restore_health(player.level)
	var i: int = Rng.roll_die(player.level * 3 / 2)
	if player.level < 35:
		player.set_timed_heroism(player.timed_heroism + i)
	else:
		player.set_timed_superheroism(player.timed_super_heroism + i)
	player.set_timed_haste(player.timed_haste + i)


func comment(player: Player) -> String:
	return "dur 10+d%d" % [player.level * 3 / 2]
