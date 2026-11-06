# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationExtraEyes
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "You grow an extra pair of eyes!"
	have_message = "You have an extra pair of eyes (+15 search)."
	lose_message = "Your extra eyes vanish!"


func on_gain(genome: Genome) -> void:
	genome.search_bonus += 15


func on_lose(genome: Genome) -> void:
	genome.search_bonus -= 15
