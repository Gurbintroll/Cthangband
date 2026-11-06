# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationPaperSkin
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your skin becomes paper-thin! You feel frail..."
	have_message = "You have paper-thin skin (-4 CON)."
	lose_message = "Your skin returns to normal thickness!"
	group = MutationGroup.SKIN


func on_gain(genome: Genome) -> void:
	genome.con_bonus -= 4


func on_lose(genome: Genome) -> void:
	genome.con_bonus += 4
