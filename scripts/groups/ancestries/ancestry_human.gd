# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHuman
extends Ancestry


func _init() -> void:
	ability_bonus = [0, 0, 0, 0, 0, 0]
	article = "a"
	age_range = 6
	base_age = 14
	base_device_bonus = 0
	base_disarm_bonus = 0
	base_melee_attack_bonus = 0
	base_ranged_attack_bonus = 0
	base_save_bonus = 0
	base_search_bonus = 0
	base_search_frequency = 10
	base_stealth_bonus = 0
	description_line_1 = ""
	description_line_2 = "Hopefully you know all about humans already because you"
	description_line_3 = "are one! In game terms, humans are the average around which"
	description_line_4 = "the other races are measured. As such, humans get no"
	description_line_5 = "special abilities, but they increase in level quicker than"
	description_line_6 = "any other race. Humans are recommended for new players."
	doesnt_eat = false
	experience_factor = 100
	feeds_on_nether = false
	female_base_height = 66
	female_base_weight = 150
	female_height_range = 4
	female_weight_range = 20
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 0
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 72
	male_base_weight = 180
	male_height_range = 6
	male_weight_range = 25
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Human"
	background_starting_chart = 1


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name
