# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentCharacterArmour
extends Talent


func initialise(profession: Profession) -> void:
	name = "Character Armour"
	level = 13
	power_cost = 12
	base_failure = 50


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	player.set_timed_stoneskin(player.timed_stoneskin + player.level)
	if player.level > 14:
		player.set_timed_acid_resistance(player.timed_acid_resistance + player.level)
	if player.level > 19:
		player.set_timed_fire_resistance(player.timed_fire_resistance + player.level)
	if player.level > 24:
		player.set_timed_cold_resistance(player.timed_cold_resistance + player.level)
	if player.level > 29:
		player.set_timed_lightning_resistance(player.timed_lightning_resistance + player.level)
	if player.level > 34:
		player.set_timed_poison_resistance(player.timed_poison_resistance + player.level)


func comment(player: Player) -> String:
	return "dur %sd" % [player.level]
