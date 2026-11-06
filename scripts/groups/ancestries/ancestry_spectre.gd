# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestrySpectre
extends Ancestry


func _init() -> void:
	ability_bonus = [-5, 4, 4, 2, -3, -6]
	article = "a"
	age_range = 30
	base_age = 100
	base_device_bonus = 25
	base_disarm_bonus = 10
	base_melee_attack_bonus = -15
	base_ranged_attack_bonus = -5
	base_save_bonus = 20
	base_search_bonus = 5
	base_search_frequency = 14
	base_stealth_bonus = 5
	description_line_1 = "Spectres are ethereal and they can pass through walls and"
	description_line_2 = "other obstacles. They resist nether, attacks, poison, and"
	description_line_3 = "cold; and they need little food. They also resist having"
	description_line_4 = "their life force drained and can see invisible creatures."
	description_line_5 = "Finally, they glow with their own light, can learn to"
	description_line_6 = "scare monsters (at lvl 4) and gain telepathy (at lvl 35)."
	doesnt_eat = true
	experience_factor = 180
	feeds_on_nether = true
	female_base_height = 66
	female_base_weight = 100
	female_height_range = 4
	female_weight_range = 20
	glows = true
	has_speed_bonus = false
	hit_die_bonus = 7
	infravision = 5
	is_incorporeal = true
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 72
	male_base_weight = 100
	male_height_range = 6
	male_weight_range = 25
	mutates = false
	resists_fear = false
	sanity_resistant = true
	spills_potions = false
	title = "Spectre"
	background_starting_chart = 118


func apply_ancestral_status(player: Player) -> void:
	player.has_feather_fall = true
	player.has_nether_resistance = true
	player.has_see_invisibility = true
	player.has_poison_resistance = true
	player.has_slow_digestion = true
	player.has_cold_resistance = true
	player.has_glow = true
	player.has_hold_life = true
	if player.level > 34:
		player.has_telepathy = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_COLD)
	f_3.set_bits(ItemFlags3.SEE_INVIS)
	f_2.set_bits(ItemFlags2.HOLD_LIFE)
	f_2.set_bits(ItemFlags2.RES_NETHER)
	f_2.set_bits(ItemFlags2.RES_POIS)
	f_3.set_bits(ItemFlags3.SLOW_DIGEST)
	f_3.set_bits(ItemFlags3.LIGHT_SOURCE)
	if player.level > 34:
		f_3.set_bits(ItemFlags3.TELEPATHY)


func get_ancestral_power_text(player: Player) -> String:
	return "scare monster      (racial, unusable until level 4)" if player.level < 4 else "scare monster      (racial, cost 3, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 3:
		list.append("You can wail to terrify your enemies (cost 3).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(4, 6, Ability.INTELLIGENCE, 3):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You emit an eldritch howl!")
		savegame.spell_effects_handler.fear_monster(dir, player.level)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
