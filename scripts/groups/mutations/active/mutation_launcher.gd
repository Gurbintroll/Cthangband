# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationLauncher
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(1, player.level, Ability.STRENGTH, 6):
		return
	var handler: ActivationHandler = ActivationHandler.new_activation_handler(player, dungeon_level, savegame)
	handler.command_throw(2 + player.level / 16)


func activation_summary(level: int) -> String:
	return "throw object     (cost lev, STR based)"


func initialise() -> void:
	frequency = 2
	gain_message = "Your throwing arm feels much stronger."
	have_message = "You can hurl objects with great force."
	lose_message = "Your throwing arm feels much weaker."
