# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHorns
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Horns pop forth into your forehead!"
	have_message = "You have horns (dam. 2d6)."
	lose_message = "Your horns vanish from your forehead!"
	damage_dice_number = 2
	damage_dice_size = 6
	equivalent_weapon_weight = 15
	attack_description = "horns"


func on_gain(genome: Genome) -> void:
	genome.natural_attacks.append(self)


func on_lose(genome: Genome) -> void:
	genome.natural_attacks.remove_at(genome.natural_attacks.find(self))
