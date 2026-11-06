# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfElf
extends Ancestry


func _init() -> void:
	ability_bonus = [-1, 1, 1, 1, -1, 1]
	article = "a"
	age_range = 16
	base_age = 24
	base_device_bonus = 3
	base_disarm_bonus = 2
	base_melee_attack_bonus = -1
	base_ranged_attack_bonus = 5
	base_save_bonus = 3
	base_search_bonus = 6
	base_search_frequency = 11
	base_stealth_bonus = 1
	description_line_1 = ""
	description_line_2 = "alf-Elves inherit better ability scores and skills from"
	description_line_3 = "their elven parent, but none of that parent's special"
	description_line_4 = "abilities. However, a half elf will advance in level more"
	description_line_5 = "quickly than a full elf."
	description_line_6 = ""
	doesnt_eat = false
	experience_factor = 110
	feeds_on_nether = false
	female_base_height = 62
	female_base_weight = 100
	female_height_range = 6
	female_weight_range = 10
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 2
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 66
	male_base_weight = 130
	male_height_range = 6
	male_weight_range = 15
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Elf"
	background_starting_chart = 4


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _elf_syllable_1[Rng.roll_die(_elf_syllable_1.size() - 1)]
		name += _elf_syllable_2[Rng.roll_die(_elf_syllable_2.size() - 1)]
		name += _elf_syllable_3[Rng.roll_die(_elf_syllable_3.size() - 1)]
	return name
