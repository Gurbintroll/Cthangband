# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHighElf
extends Ancestry


func _init() -> void:
	ability_bonus = [1, 3, 2, 3, 1, 5]
	article = "a"
	age_range = 30
	base_age = 100
	base_device_bonus = 20
	base_disarm_bonus = 4
	base_melee_attack_bonus = 10
	base_ranged_attack_bonus = 25
	base_save_bonus = 20
	base_search_bonus = 3
	base_search_frequency = 14
	base_stealth_bonus = 4
	description_line_1 = ""
	description_line_2 = "High-Elves are the leaders of the elven race. They are"
	description_line_3 = "more magical than their lesser cousins, but retain their"
	description_line_4 = "affinity with nature. High-elves resist light based attacks"
	description_line_5 = "and their acute senses are able to see invisible creatures."
	description_line_6 = ""
	doesnt_eat = false
	experience_factor = 200
	feeds_on_nether = false
	female_base_height = 82
	female_base_weight = 180
	female_height_range = 10
	female_weight_range = 15
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 90
	male_base_weight = 190
	male_height_range = 10
	male_weight_range = 20
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "High Elf"
	background_starting_chart = 7


func apply_ancestral_status(player: Player) -> void:
	player.has_see_invisibility = true
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
	f_3.set_bits(ItemFlags3.SEE_INVIS)
