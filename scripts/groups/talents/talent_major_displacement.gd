# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentMajorDisplacement
extends Talent


func initialise(profession: Profession) -> void:
	name = "Major Displacement"
	level = 7
	power_cost = 6
	base_failure = 35


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	savegame.spell_effects_handler.teleport_player(player.level * 5)
	if player.level > 29:
		savegame.spell_effects_handler.banish_monsters(player.level)


func comment(player: Player) -> String:
	return "range %d" % [player.level * 5]
