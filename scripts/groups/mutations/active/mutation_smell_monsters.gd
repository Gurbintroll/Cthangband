# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSmellMonsters
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(5, 4, Ability.INTELLIGENCE, 15):
		return
	savegame.spell_effects_handler.detect_monsters_normal()


func activation_summary(level: int) -> String:
	return "smell monsters   (unusable until level 5)" if level < 5 else "smell monsters   (cost 4, INT based)"


func initialise() -> void:
	frequency = 4
	gain_message = "You smell filthy monsters."
	have_message = "You can smell nearby monsters."
	lose_message = "You no longer smell filthy monsters."
