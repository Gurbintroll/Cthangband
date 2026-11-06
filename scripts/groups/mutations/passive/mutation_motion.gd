# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationMotion
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "You move with new assurance."
	have_message = "Your movements are precise and forceful (+1 STL)."
	lose_message = "You move with less assurance."


func on_gain(genome: Genome) -> void:
	genome.stealth_bonus += 1
	genome.free_action = true


func on_lose(genome: Genome) -> void:
	genome.stealth_bonus -= 1
	genome.free_action = false
