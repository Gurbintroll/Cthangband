# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHyperInt
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "Your brain evolves into a living computer!"
	have_message = "Your brain is a living computer (+4 INT/WIS)."
	lose_message = "Your brain reverts to normal."
	group = MutationGroup.SMARTS


func on_gain(genome: Genome) -> void:
	genome.int_bonus += 4
	genome.wis_bonus += 4


func on_lose(genome: Genome) -> void:
	genome.int_bonus -= 4
	genome.wis_bonus -= 4
