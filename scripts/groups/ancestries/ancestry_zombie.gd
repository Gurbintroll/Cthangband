# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryZombie
extends Ancestry


func _init() -> void:
	ability_bonus = [2, -6, -6, 1, 4, 5]
	article = "a"
	age_range = 30
	base_age = 100
	base_device_bonus = -5
	base_disarm_bonus = -5
	base_melee_attack_bonus = 15
	base_ranged_attack_bonus = 0
	base_save_bonus = 8
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = -1
	description_line_1 = ""
	description_line_2 = "Zombies are undead creatures. Their decayed flesh resists"
	description_line_3 = "nether and poison, and having their life force drained."
	description_line_4 = "Zombies digest food slowly, and can see invisible monsters."
	description_line_5 = "They can learn to restore their life force (at lvl 30)."
	description_line_6 = ""
	doesnt_eat = true
	experience_factor = 135
	feeds_on_nether = false
	female_base_height = 66
	female_base_weight = 100
	female_height_range = 4
	female_weight_range = 20
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 13
	infravision = 2
	is_incorporeal = false
	is_nocturnal = true
	is_sunlight_sensitive = false
	male_base_height = 72
	male_base_weight = 100
	male_height_range = 6
	male_weight_range = 25
	mutates = false
	resists_fear = false
	sanity_resistant = true
	spills_potions = false
	title = "Zombie"
	background_starting_chart = 107


func apply_ancestral_status(player: Player) -> void:
	player.has_nether_resistance = true
	player.has_hold_life = true
	player.has_see_invisibility = true
	player.has_poison_resistance = true
	player.has_slow_digestion = true
	if player.level > 4:
		player.has_cold_resistance = true


func consume_food(player: Player, item: Item) -> void:
	Profile.instance.message_print("The food of mortals is poor sustenance for you.")
	player.set_food(player.food + item.type_specific_value / 20)


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_3.set_bits(ItemFlags3.SEE_INVIS)
	f_2.set_bits(ItemFlags2.HOLD_LIFE)
	f_2.set_bits(ItemFlags2.RES_NETHER)
	f_2.set_bits(ItemFlags2.RES_POIS)
	f_3.set_bits(ItemFlags3.SLOW_DIGEST)
	if player.level > 4:
		f_2.set_bits(ItemFlags2.RES_COLD)


func get_ancestral_power_text(player: Player) -> String:
	return "restore life       (racial, unusable until level 30)" if player.level < 30 else "restore life       (racial, cost 30, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 29:
		list.append("You can restore lost life forces (cost 30).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(30, 30, Ability.WISDOM, 18):
		Profile.instance.message_print("You attempt to restore your lost energies.")
		player.restore_level()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
