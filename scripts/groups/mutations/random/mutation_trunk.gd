# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationTrunk
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your nose grows into an elephant-like trunk."
	have_message = "You have an elephantine trunk (dam 1d4)."
	lose_message = "Your nose returns to a normal length."
	group = MutationGroup.MOUTH
	damage_dice_number = 1
	damage_dice_size = 4
	equivalent_weapon_weight = 35
	attack_description = "trunk"


func on_gain(genome: Genome) -> void:
	genome.natural_attacks.append(self)


func on_lose(genome: Genome) -> void:
	genome.natural_attacks.remove_at(genome.natural_attacks.find(self))
