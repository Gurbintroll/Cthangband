# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryMiriNigri
extends Ancestry


func _init() -> void:
	ability_bonus = [2, -1, -1, -1, 2, -4]
	article = "a"
	age_range = 6
	base_age = 14
	base_device_bonus = -2
	base_disarm_bonus = -5
	base_melee_attack_bonus = 12
	base_ranged_attack_bonus = 5
	base_save_bonus = -1
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = -1
	description_line_1 = "Miri-Nigri are squat, toad-like chaos beasts. Their"
	description_line_2 = "close ties to chaos render them resistant to sound and"
	description_line_3 = "immune to confusion. However, their chaotic nature also"
	description_line_4 = "makes them prone to random mutation. Also, the outer gods"
	description_line_5 = "pay special attention to miri-nigri servants and they"
	description_line_6 = "are more likely to interfere with them for good or ill."
	doesnt_eat = false
	experience_factor = 140
	feeds_on_nether = false
	female_base_height = 61
	female_base_weight = 120
	female_height_range = 6
	female_weight_range = 15
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 0
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 65
	male_base_weight = 150
	male_height_range = 6
	male_weight_range = 20
	mutates = true
	resists_fear = false
	sanity_immune = true
	spills_potions = false
	title = "Miri Nigri"
	background_starting_chart = 129


func apply_ancestral_status(player: Player) -> void:
	player.has_confusion_resistance = true
	player.has_sound_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _cthuloid_syllable_1[Rng.roll_die(_cthuloid_syllable_1.size() - 1)]
		name += _cthuloid_syllable_2[Rng.roll_die(_cthuloid_syllable_2.size() - 1)]
		name += _cthuloid_syllable_3[Rng.roll_die(_cthuloid_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_CHAOS)
	f_2.set_bits(ItemFlags2.RES_SOUND)
