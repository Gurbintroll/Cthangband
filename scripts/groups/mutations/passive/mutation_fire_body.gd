# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationFireBody
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your body is enveloped in flames!"
	have_message = "Your body is enveloped in flames."
	lose_message = "Your body is no longer enveloped in flames."


func on_gain(genome: Genome) -> void:
	genome.fire_hit = true


func on_lose(genome: Genome) -> void:
	genome.fire_hit = false
