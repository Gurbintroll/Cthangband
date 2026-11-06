# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellResistanceTrue
extends Spell


func _init() -> void:
	default_base_failure = 85
	default_level = 15
	default_power_cost = 20
	first_cast_experience = 60
	name = "Resistance True"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(20) + 20)
	player.set_timed_lightning_resistance(player.timed_lightning_resistance + Rng.roll_die(20) + 20)
	player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(20) + 20)
	player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(20) + 20)
	player.set_timed_poison_resistance(player.timed_poison_resistance + Rng.roll_die(20) + 20)


func comment(player: Player) -> String:
	return "dur 1d20+20"
