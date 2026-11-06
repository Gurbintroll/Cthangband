# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationScales
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "Your skin turns into black scales!"
	have_message = "Your skin has turned into scales (-1 CHA, +10 AC)."
	lose_message = "Your scales vanish!"
	group = MutationGroup.SKIN


func on_gain(genome: Genome) -> void:
	genome.cha_bonus -= 1
	genome.armour_class_bonus += 10


func on_lose(genome: Genome) -> void:
	genome.cha_bonus += 1
	genome.armour_class_bonus += 10
