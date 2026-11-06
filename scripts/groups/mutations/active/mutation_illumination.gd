# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationIllumination
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(3, 2, Ability.INTELLIGENCE, 10):
		return
	savegame.spell_effects_handler.light_area(Rng.roll_dice(2, player.level / 2), player.level / 10 + 1)


func activation_summary(level: int) -> String:
	return "illuminate       (unusable until level 3)" if level < 3 else "illuminate       (cost 2, INT based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You can light up rooms with your presence."
	have_message = "You can emit bright light."
	lose_message = "You can no longer light up rooms with your presence."
