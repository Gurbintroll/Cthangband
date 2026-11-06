# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationMagicResistant
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You become resistant to magic."
	have_message = "You are resistant to magic."
	lose_message = "You become susceptible to magic again."


func on_gain(genome: Genome) -> void:
	genome.magic_resistance = true


func on_lose(genome: Genome) -> void:
	genome.magic_resistance = false
