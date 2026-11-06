# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfOrc
extends Ancestry


func _init() -> void:
	ability_bonus = [2, -1, 0, 0, 1, -4]
	article = "a"
	age_range = 4
	base_age = 11
	base_device_bonus = -3
	base_disarm_bonus = -3
	base_melee_attack_bonus = 12
	base_ranged_attack_bonus = -5
	base_save_bonus = -3
	base_search_bonus = 0
	base_search_frequency = 7
	base_stealth_bonus = -1
	description_line_1 = ""
	description_line_2 = "Half-Orcs are stronger than humans, and less dimwitted"
	description_line_3 = "heir orcish parentage would lead you to assume."
	description_line_4 = "Half-Orcs are born of darkness and are resistant to that"
	description_line_5 = "form of attack. They are also able to learn to shrug off"
	description_line_6 = "magical fear (at lvl 5)."
	doesnt_eat = false
	experience_factor = 110
	feeds_on_nether = false
	female_base_height = 62
	female_base_weight = 120
	female_height_range = 1
	female_weight_range = 5
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 66
	male_base_weight = 150
	male_height_range = 1
	male_weight_range = 5
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Orc"
	background_starting_chart = 19


func apply_ancestral_status(player: Player) -> void:
	player.has_dark_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _orc_syllable_1[Rng.roll_die(_orc_syllable_1.size() - 1)]
		name += _orc_syllable_2[Rng.roll_die(_orc_syllable_2.size() - 1)]
		name += _orc_syllable_3[Rng.roll_die(_orc_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_DARK)


func get_ancestral_power_text(player: Player) -> String:
	return "remove fear        (racial, unusable until level 3)" if player.level < 3 else "remove fear        (racial, cost 5, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 2:
		list.append("You can remove fear (cost 5).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(
		3,
		5,
		Ability.WISDOM,
		5 if player.profession.prime_ability_score == Ability.STRENGTH else 10,
	):
		Profile.instance.message_print("You play tough.")
		player.set_timed_fear(0)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
