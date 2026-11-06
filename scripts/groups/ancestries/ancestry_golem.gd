# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryGolem
extends Ancestry


func _init() -> void:
	ability_bonus = [4, -5, -5, 0, 4, -4]
	article = "a"
	age_range = 100
	base_age = 1
	base_device_bonus = -5
	base_disarm_bonus = -5
	base_melee_attack_bonus = 20
	base_ranged_attack_bonus = 0
	base_save_bonus = 10
	base_search_bonus = -1
	base_search_frequency = 8
	base_stealth_bonus = -1
	description_line_1 = "Golems are animated statues. Their inorganic bodies make it"
	description_line_2 = "hard for them to digest food properly, but they have innate"
	description_line_3 = "natural armour and can't be stunned or made to bleed. They"
	description_line_4 = "also resist poison and can see invisible creatures. Golems"
	description_line_5 = "can learn to use their armour more efficiently (at lvl 20)"
	description_line_6 = "and avoid having their life force drained (at lvl 35)."
	doesnt_eat = true
	experience_factor = 200
	feeds_on_nether = false
	female_base_height = 62
	female_base_weight = 180
	female_height_range = 1
	female_weight_range = 6
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 12
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 66
	male_base_weight = 200
	male_height_range = 1
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Golem"
	background_starting_chart = 98


func apply_ancestral_status(player: Player) -> void:
	player.has_slow_digestion = true
	player.has_free_action = true
	player.has_see_invisibility = true
	player.has_poison_resistance = true
	if player.level > 34:
		player.has_hold_life = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _dwarf_syllable_1[Rng.roll_die(_dwarf_syllable_1.size() - 1)]
		name += _dwarf_syllable_2[Rng.roll_die(_dwarf_syllable_2.size() - 1)]
		name += _dwarf_syllable_3[Rng.roll_die(_dwarf_syllable_3.size() - 1)]
	return name


func doesnt_bleed(player: Player) -> bool:
	return true


func doesnt_stun(player: Player) -> bool:
	return true


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_3.set_bits(ItemFlags3.SEE_INVIS)
	f_2.set_bits(ItemFlags2.FREE_ACT)
	f_2.set_bits(ItemFlags2.RES_POIS)
	f_3.set_bits(ItemFlags3.SLOW_DIGEST)
	if player.level > 34:
		f_2.set_bits(ItemFlags2.HOLD_LIFE)


func get_ancestral_power_text(player: Player) -> String:
	return "stone skin         (racial, unusable until level 20)" if player.level < 20 else "stone skin         (racial, cost 15, dur 30+d20, CON based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 19:
		list.append("You can turn your skin to stone, dur d20+30 (cost 15).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(20, 15, Ability.CONSTITUTION, 8):
		player.set_timed_stoneskin(player.timed_stoneskin + Rng.roll_die(20) + 30)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
