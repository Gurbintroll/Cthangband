# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentMindWave
extends Talent


func initialise(profession: Profession) -> void:
	name = "Mind Wave"
	level = 18
	power_cost = 10
	base_failure = 45


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	Profile.instance.message_print("Mind-warping forces emanate from your brain!")
	if player.level < 25:
		savegame.spell_effects_handler.project(0, 2 + (player.level / 10), player.map_row, player.map_col, player.level * 3 / 2, ProjectilePsi.new(savegame.spell_effects_handler), ProjectionFlag.KILL)
	else:
		savegame.spell_effects_handler.mind_blast_monsters(player.level * (((player.level - 5) / 10) + 1))


func comment(player: Player) -> String:
	if player.level < 25:
		return "dam %d" % [player.level * 3 / 2]
	else:
		return "dam %d" % [player.level * (((player.level - 5) / 10) + 1)]
