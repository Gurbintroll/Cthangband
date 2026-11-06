# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBeak
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your mouth turns into a sharp, powerful beak!"
	have_message = "You have a beak (dam. 2d4)."
	lose_message = "Your mouth reverts to normal!"
	group = MutationGroup.MOUTH
	damage_dice_size = 4
	damage_dice_number = 2
	equivalent_weapon_weight = 5
	attack_description = "beak"


func on_gain(genome: Genome) -> void:
	genome.natural_attacks.append(self)


func on_lose(genome: Genome) -> void:
	genome.natural_attacks.remove_at(genome.natural_attacks.find(self))
