# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryElf
extends Ancestry


func _init() -> void:
	ability_bonus = [-1, 2, 2, 1, -2, 2]
	article = "an"
	age_range = 75
	base_age = 75
	base_device_bonus = 6
	base_disarm_bonus = 5
	base_melee_attack_bonus = -5
	base_ranged_attack_bonus = 15
	base_save_bonus = 6
	base_search_bonus = 8
	base_search_frequency = 12
	base_stealth_bonus = 2
	description_line_1 = ""
	description_line_2 = "Elves are creatures of the woods, and cultivate a symbiotic"
	description_line_3 = "relationship with trees. While not the sturdiest of races,"
	description_line_4 = "they are dextrous and have excellent mental faculties."
	description_line_5 = "Because they are partially photosynthetic, elves are able"
	description_line_6 = "to resist light based attacks."
	doesnt_eat = false
	experience_factor = 120
	feeds_on_nether = false
	female_base_height = 54
	female_base_weight = 80
	female_height_range = 4
	female_weight_range = 6
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 8
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 60
	male_base_weight = 100
	male_height_range = 4
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Elf"
	background_starting_chart = 7


func apply_ancestral_status(player: Player) -> void:
	player.has_light_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _elf_syllable_1[Rng.roll_die(_elf_syllable_1.size() - 1)]
		name += _elf_syllable_2[Rng.roll_die(_elf_syllable_2.size() - 1)]
		name += _elf_syllable_3[Rng.roll_die(_elf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_LIGHT)
