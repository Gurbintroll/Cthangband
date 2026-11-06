# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHyperStrength
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "You turn into a superhuman muscle-freak!"
	have_message = "You are superhumanly strong (+4 STR)."
	lose_message = "Your muscles revert to normal."
	group = MutationGroup.STRENGTH


func on_gain(genome: Genome) -> void:
	genome.str_bonus += 4


func on_lose(genome: Genome) -> void:
	genome.str_bonus -= 4
