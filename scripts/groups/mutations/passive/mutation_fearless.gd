# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationFearless
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "You become completely fearless."
	have_message = "You are completely fearless."
	lose_message = "You begin to feel fear again."
	group = MutationGroup.BRAVERY


func on_gain(genome: Genome) -> void:
	genome.res_fear = true


func on_lose(genome: Genome) -> void:
	genome.res_fear = false
