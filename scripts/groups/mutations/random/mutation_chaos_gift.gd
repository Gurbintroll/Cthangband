# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationChaosGift
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You attract the notice of a Great Old One!"
	have_message = "A Great Old One gives you gifts."
	lose_message = "You lose the attention of the Great Old Ones."


func on_gain(genome: Genome) -> void:
	genome.chaos_gift = true


func on_lose(genome: Genome) -> void:
	genome.chaos_gift = false
