# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationRecall
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(17, 50, Ability.INTELLIGENCE, 16):
		return
	player.toggle_recall()


func activation_summary(level: int) -> String:
	return "recall           (unusable until level 17)" if level < 17 else "recall           (cost 50, INT based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You feel briefly homesick, but it passes."
	have_message = "You can travel between town and the depths."
	lose_message = "You feel briefly homesick."
