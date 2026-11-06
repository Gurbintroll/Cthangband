# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationResist
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(10, 12, Ability.CONSTITUTION, 12):
		return
	var num: int = player.level / 10
	var dur: int = Rng.roll_die(20) + 20
	if Rng.roll_die(5) <= num:
		player.set_timed_acid_resistance(player.timed_acid_resistance + dur)
		num -= 1
	if Rng.roll_die(4) <= num:
		player.set_timed_lightning_resistance(player.timed_lightning_resistance + dur)
		num -= 1
	if Rng.roll_die(3) <= num:
		player.set_timed_fire_resistance(player.timed_fire_resistance + dur)
		num -= 1
	if Rng.roll_die(2) <= num:
		player.set_timed_cold_resistance(player.timed_cold_resistance + dur)
	if num != 0:
		player.set_timed_poison_resistance(player.timed_poison_resistance + dur)


func activation_summary(level: int) -> String:
	return "resist elements  (unusable until level 10)" if level < 10 else "resist elements  (cost 12, CON based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You feel like you can protect yourself."
	have_message = "You can harden yourself to the ravages of the elements."
	lose_message = "You feel like you might be vulnerable."
