# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfTroll
extends Ancestry


func _init() -> void:
	ability_bonus = [4, -4, -2, -4, 3, -6]
	article = "a"
	age_range = 10
	base_age = 20
	base_device_bonus = -8
	base_disarm_bonus = -5
	base_melee_attack_bonus = 20
	base_ranged_attack_bonus = 10
	base_save_bonus = -8
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = -2
	description_line_1 = "Half-Trolls make up for their stupidity by being almost"
	description_line_2 = "pure muscle, as strong as creatures much larger than they."
	description_line_3 = "They can't have their strength reduced, and as they grow"
	description_line_4 = "stronger they can go into a berserk rage (at lvl 10),"
	description_line_5 = "regenerate wounds (at lvl 15), and survive on less food"
	description_line_6 = "(at lvl 15)."
	doesnt_eat = false
	experience_factor = 137
	feeds_on_nether = false
	female_base_height = 84
	female_base_weight = 225
	female_height_range = 8
	female_weight_range = 40
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 12
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 96
	male_base_weight = 250
	male_height_range = 10
	male_weight_range = 50
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Troll"
	background_starting_chart = 22


func apply_ancestral_status(player: Player) -> void:
	player.has_sustain_strength = true
	if player.level > 14:
		player.has_regeneration = true
		player.has_slow_digestion = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _orc_syllable_1[Rng.roll_die(_orc_syllable_1.size() - 1)]
		name += _orc_syllable_2[Rng.roll_die(_orc_syllable_2.size() - 1)]
		name += _orc_syllable_3[Rng.roll_die(_orc_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.SUST_STR)
	if player.level > 14:
		f_3.set_bits(ItemFlags3.SLOW_DIGEST)
		f_3.set_bits(ItemFlags3.REGEN)


func get_ancestral_power_text(player: Player) -> String:
	return "berserk            (racial, unusable until level 10)" if player.level < 10 else "berserk            (racial, cost 12, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 1:
		list.append("You enter berserk fury (cost 12).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(
		10,
		12,
		Ability.WISDOM,
		6 if player.profession.prime_ability_score == Ability.STRENGTH else 12,
	):
		Profile.instance.message_print("RAAAGH!")
		player.set_timed_fear(0)
		player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(player.level) + 10)
		player.restore_health(30)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
