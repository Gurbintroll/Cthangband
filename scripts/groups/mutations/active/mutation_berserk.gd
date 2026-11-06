# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBerserk
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(8, 8, Ability.STRENGTH, 14):
		return
	player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(25) + 25)
	player.restore_health(30)
	player.set_timed_fear(0)


func activation_summary(level: int) -> String:
	return "berserk          (unusable until level 8)" if level < 8 else "berserk          (cost 8, STR based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You feel a controlled rage."
	have_message = "You can drive yourself into a berserk frenzy."
	lose_message = "You no longer feel a controlled rage."
