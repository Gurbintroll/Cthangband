# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryTchoTcho
extends Ancestry


func _init() -> void:
	ability_bonus = [3, -2, -1, 1, 2, -1]
	article = "a"
	age_range = 8
	base_age = 14
	base_device_bonus = -10
	base_disarm_bonus = -2
	base_melee_attack_bonus = 12
	base_ranged_attack_bonus = 10
	base_save_bonus = 2
	base_search_bonus = 1
	base_search_frequency = 7
	base_stealth_bonus = -1
	description_line_1 = ""
	description_line_2 = "Tcho-Tchos are hairless cannibalistic near-humans who dwell"
	description_line_3 = "in isolated parts of the world away from more civilised"
	description_line_4 = "places where their dark rituals and sacrifices go unseen."
	description_line_5 = "Tcho-Tchos are immune to fear, and can also learn to create"
	description_line_6 = "The Yellow Sign (at lvl 25)."
	doesnt_eat = false
	experience_factor = 120
	feeds_on_nether = false
	female_base_height = 78
	female_base_weight = 190
	female_height_range = 6
	female_weight_range = 15
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 0
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 82
	male_base_weight = 200
	male_height_range = 5
	male_weight_range = 20
	mutates = false
	resists_fear = true
	sanity_resistant = true
	spills_potions = false
	title = "Tcho Tcho"
	background_starting_chart = 138


func apply_ancestral_status(player: Player) -> void:
	player.has_fear_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _cthuloid_syllable_1[Rng.roll_die(_cthuloid_syllable_1.size() - 1)]
		name += _cthuloid_syllable_2[Rng.roll_die(_cthuloid_syllable_2.size() - 1)]
		name += _cthuloid_syllable_3[Rng.roll_die(_cthuloid_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_FEAR)


func get_ancestral_power_text(player: Player) -> String:
	return "Yellow Sign     (racial, unusable until level 25)" if player.level < 25 else "Yellow Sign     (racial, cost 35, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 24:
		list.append("You can set an Yellow Sign (cost 35).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(25, 35, Ability.INTELLIGENCE, 15):
		Profile.instance.message_print("You carefully draw the Yellow Sign...")
		savegame.spell_effects_handler.yellow_sign()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
