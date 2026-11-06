# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfTitan
extends Ancestry


func _init() -> void:
	ability_bonus = [5, 1, 1, -2, 3, 1]
	article = "a"
	age_range = 30
	base_age = 100
	base_device_bonus = 5
	base_disarm_bonus = -5
	base_melee_attack_bonus = 25
	base_ranged_attack_bonus = 0
	base_save_bonus = 2
	base_search_bonus = 1
	base_search_frequency = 8
	base_stealth_bonus = -2
	description_line_1 = ""
	description_line_2 = "Half-Titans are massively strong, being descended from the"
	description_line_3 = "predecessors of the gods that grew from primal chaos. This"
	description_line_4 = "legacy lets them resist damage from chaos, and half-titans"
	description_line_5 = "can learn to magically probe their foes to find out their"
	description_line_6 = "strengths and weaknesses (at lvl 35)."
	doesnt_eat = false
	experience_factor = 255
	feeds_on_nether = false
	female_base_height = 99
	female_base_weight = 250
	female_height_range = 11
	female_weight_range = 86
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 14
	infravision = 0
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 111
	male_base_weight = 255
	male_height_range = 11
	male_weight_range = 86
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Titan"
	background_starting_chart = 76


func apply_ancestral_status(player: Player) -> void:
	player.has_chaos_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_CHAOS)


func get_ancestral_power_text(player: Player) -> String:
	return "probing            (racial, unusable until level 35)" if player.level < 35 else "probing            (racial, cost 20, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 34:
		list.append("You can probe monsters (cost 20).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(35, 20, Ability.INTELLIGENCE, 12):
		Profile.instance.message_print("You examine your foes...")
		savegame.spell_effects_handler.probing()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
