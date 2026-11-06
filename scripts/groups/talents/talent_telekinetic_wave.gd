# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentTelekineticWave
extends Talent


func initialise(profession: Profession) -> void:
	name = "Telekinetic Wave"
	level = 28
	power_cost = 20
	base_failure = 45


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	Profile.instance.message_print("A wave of pure physical force radiates out from your body!")
	savegame.spell_effects_handler.project(0, 3 + (player.level / 10), player.map_row, player.map_col, player.level * (4 if player.level > 39 else 3), ProjectileTelekinesis.new(savegame.spell_effects_handler), ProjectionFlag.KILL | ProjectionFlag.ITEM | ProjectionFlag.GRID)


func comment(player: Player) -> String:
	return "dam %d" % [player.level * (4 if player.level > 39 else 3)]
