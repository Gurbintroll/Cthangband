# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestrySandDweller
extends Ancestry


func _init() -> void:
	ability_bonus = [3, -1, -1, 1, 3, -2]
	article = "a"
	age_range = 15
	base_age = 22
	base_device_bonus = 6
	base_disarm_bonus = 5
	base_melee_attack_bonus = 12
	base_ranged_attack_bonus = -5
	base_save_bonus = 3
	base_search_bonus = 1
	base_search_frequency = 5
	base_stealth_bonus = 6
	description_line_1 = "Sand dwellers are sentient humanoid conglomerations of"
	description_line_2 = "sand that come from desert regions. Their sand-based"
	description_line_3 = "bodies are able to merge with rock and earth, allowing"
	description_line_4 = "them to walk through walls and similar obstacles."
	description_line_5 = "Sand dwellers are resistant to acid and become completely"
	description_line_6 = "immune to acid (at lvl 20)."
	doesnt_eat = false
	experience_factor = 170
	feeds_on_nether = false
	female_base_height = 66
	female_base_weight = 180
	female_height_range = 4
	female_weight_range = 20
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 3
	is_incorporeal = true
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 72
	male_base_weight = 200
	male_height_range = 6
	male_weight_range = 25
	mutates = false
	resists_fear = false
	sanity_resistant = true
	spills_potions = false
	title = "Sand Dweller"
	background_starting_chart = 145


func apply_ancestral_status(player: Player) -> void:
	player.has_acid_resistance = true
	if player.level > 19:
		player.has_acid_immunity = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _cthuloid_syllable_1[Rng.roll_die(_cthuloid_syllable_1.size() - 1)]
		name += _cthuloid_syllable_2[Rng.roll_die(_cthuloid_syllable_2.size() - 1)]
		name += _cthuloid_syllable_3[Rng.roll_die(_cthuloid_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_ACID)
	if player.level > 19:
		f_2.set_bits(ItemFlags2.IM_ACID)
