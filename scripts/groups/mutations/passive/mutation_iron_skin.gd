# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationIronSkin
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your skin turns to steel!"
	have_message = "Your skin is made of steel (-1 DEX, +25 AC)."
	lose_message = "Your skin reverts to flesh!"
	group = MutationGroup.SKIN


func on_gain(genome: Genome) -> void:
	genome.dex_bonus -= 1
	genome.armour_class_bonus += 25


func on_lose(genome: Genome) -> void:
	genome.dex_bonus += 1
	genome.armour_class_bonus -= 25
