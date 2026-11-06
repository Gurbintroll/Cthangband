# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryShadowling
extends Ancestry


func _init() -> void:
	ability_bonus = [-3, 3, 1, 1, -2, 1]
	article = "a"
	age_range = 8
	base_age = 14
	base_device_bonus = 16
	base_disarm_bonus = 5
	base_melee_attack_bonus = -12
	base_ranged_attack_bonus = -10
	base_save_bonus = 4
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = 6
	description_line_1 = "Shadowlings are creatures of darkness with mutable form"
	description_line_2 = "that seep into reality through the cracks in people's"
	description_line_3 = "dreams. Because of their shadow nature, shadowlings shun"
	description_line_4 = "bright light, but their mutable form lets them resist"
	description_line_5 = "chaos and develop unusual abilities as they increase"
	description_line_6 = "in level."
	doesnt_eat = false
	experience_factor = 150
	feeds_on_nether = false
	female_base_height = 78
	female_base_weight = 190
	female_height_range = 6
	female_weight_range = 15
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 3
	is_incorporeal = false
	is_nocturnal = true
	is_sunlight_sensitive = true
	male_base_height = 82
	male_base_weight = 200
	male_height_range = 5
	male_weight_range = 20
	mutates = true
	resists_fear = false
	sanity_resistant = true
	spills_potions = false
	title = "Shadowling"
	background_starting_chart = 143


func apply_ancestral_status(player: Player) -> void:
	player.has_chaos_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _cthuloid_syllable_1[Rng.roll_die(_cthuloid_syllable_1.size() - 1)]
		name += _cthuloid_syllable_2[Rng.roll_die(_cthuloid_syllable_2.size() - 1)]
		name += _cthuloid_syllable_3[Rng.roll_die(_cthuloid_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_CHAOS)
