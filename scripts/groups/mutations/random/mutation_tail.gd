# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationTail
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You grow a scorpion tail!"
	have_message = "You have a scorpion tail (poison, 3d7)."
	lose_message = "You lose your scorpion tail!"
	damage_dice_number = 3
	damage_dice_size = 7
	equivalent_weapon_weight = 5
	attack_description = "tail"
	mutation_attack_type = MutationAttackType.POISON


func on_gain(genome: Genome) -> void:
	genome.natural_attacks.append(self)


func on_lose(genome: Genome) -> void:
	genome.natural_attacks.remove_at(genome.natural_attacks.find(self))
