# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationRadiation
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(15, 15, Ability.CONSTITUTION, 14):
		return
	Profile.instance.message_print("Radiation flows from your body!")
	savegame.spell_effects_handler.fire_ball(ProjectileNuke.new(savegame.spell_effects_handler), 0, player.level * 2, 3 + (player.level / 20))


func activation_summary(level: int) -> String:
	return "produce radiation   (unusable until level 15)" if level < 15 else "produce radiation   (cost 15, CON based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You start emitting radiation."
	have_message = "You can emit radiation at will."
	lose_message = "You stop emitting radiation."
