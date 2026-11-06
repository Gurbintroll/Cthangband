# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfOgre
extends Ancestry


func _init() -> void:
	ability_bonus = [3, -1, -1, -1, 3, -3]
	article = "a"
	age_range = 10
	base_age = 40
	base_device_bonus = -5
	base_disarm_bonus = -3
	base_melee_attack_bonus = 20
	base_ranged_attack_bonus = 0
	base_save_bonus = -5
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = 2
	description_line_1 = ""
	description_line_2 = "Half-Ogres are both strong and naturally magical, although"
	description_line_3 = "they don't usually have the intelligence to make the most"
	description_line_4 = "of their magic. They resist darkness and can't have their"
	description_line_5 = "strength reduced. They can also can enter a berserk"
	description_line_6 = "rage (at lvl 8)."
	doesnt_eat = false
	experience_factor = 130
	feeds_on_nether = false
	female_base_height = 80
	female_base_weight = 235
	female_height_range = 8
	female_weight_range = 60
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 12
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 92
	male_base_weight = 255
	male_height_range = 10
	male_weight_range = 60
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Ogre"
	background_starting_chart = 74


func apply_ancestral_status(player: Player) -> void:
	player.has_dark_resistance = true
	player.has_sustain_strength = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _orc_syllable_1[Rng.roll_die(_orc_syllable_1.size() - 1)]
		name += _orc_syllable_2[Rng.roll_die(_orc_syllable_2.size() - 1)]
		name += _orc_syllable_3[Rng.roll_die(_orc_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_DARK)
	f_2.set_bits(ItemFlags2.SUST_STR)


func get_ancestral_power_text(player: Player) -> String:
	return "berserk            (racial, unusable until level 8)" if player.level < 8 else "berserk            (racial, cost 10, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 7:
		list.append("You can enter berserk fury (cost 10).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(
		8,
		10,
		Ability.WISDOM,
		6 if player.profession.prime_ability_score == Ability.STRENGTH else 12,
	):
		Profile.instance.message_print("Raaagh!")
		player.set_timed_fear(0)
		player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(10) + player.level)
		player.restore_health(30)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
