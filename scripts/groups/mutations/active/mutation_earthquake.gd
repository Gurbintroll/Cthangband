# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationEarthquake
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(12, 12, Ability.STRENGTH, 16):
		return
	if not savegame.quests.is_quest_level(savegame.current_depth) and savegame.current_depth != 0:
		savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 10)


func activation_summary(level: int) -> String:
	return "earthquake       (unusable until level 12)" if level < 12 else "earthquake       (cost 12, STR based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You gain the ability to wreck the dungeon."
	have_message = "You can bring down the dungeon around your ears."
	lose_message = "You lose the ability to wreck the dungeon."
