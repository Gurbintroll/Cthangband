# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSustain
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You feel like you can recover from anything."
	have_message = "Your body resists serious damage."
	lose_message = "You no longer feel like you can recover from anything."


func on_gain(genome: Genome) -> void:
	genome.sustain_all = true


func on_lose(genome: Genome) -> void:
	genome.sustain_all = false
