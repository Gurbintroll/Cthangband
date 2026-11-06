# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationTentacles
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Evil-looking tentacles sprout from your sides."
	have_message = "You have evil looking tentacles (dam 2d5)."
	lose_message = "Your tentacles vanish from your sides."
	damage_dice_number = 2
	damage_dice_size = 5
	equivalent_weapon_weight = 5
	attack_description = "tentacles"
	mutation_attack_type = MutationAttackType.HELLFIRE


func on_gain(genome: Genome) -> void:
	genome.natural_attacks.append(self)


func on_lose(genome: Genome) -> void:
	genome.natural_attacks.remove_at(genome.find(self))
