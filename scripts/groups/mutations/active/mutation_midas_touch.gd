# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationMidasTouch
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(10, 5, Ability.INTELLIGENCE, 12):
		return
	savegame.spell_effects_handler.alchemy()


func activation_summary(level: int) -> String:
	return "midas touch      (unusable until level 10)" if level < 10 else "midas touch      (cost 5, INT based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You gain the Midas touch."
	have_message = "You can turn ordinary items to gold."
	lose_message = "You lose the Midas touch."
