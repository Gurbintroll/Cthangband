# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHalfGiant
extends Ancestry


func _init() -> void:
	ability_bonus = [4, -2, -2, -2, 3, -3]
	article = "a"
	age_range = 10
	base_age = 40
	base_device_bonus = -8
	base_disarm_bonus = -6
	base_melee_attack_bonus = 25
	base_ranged_attack_bonus = 5
	base_save_bonus = -6
	base_search_bonus = -1
	base_search_frequency = 5
	base_stealth_bonus = -2
	description_line_1 = ""
	description_line_2 = "Half-Giants are immensely strong and tough, and their skin"
	description_line_3 = "is stony. They can't have their strength reduced, and they"
	description_line_4 = "resist damage from explosions that throw out shards of"
	description_line_5 = "stone and metal. They can learn to soften rock into mud"
	description_line_6 = "(at lvl 20)."
	doesnt_eat = false
	experience_factor = 150
	feeds_on_nether = false
	female_base_height = 80
	female_base_weight = 240
	female_height_range = 10
	female_weight_range = 64
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 13
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 100
	male_base_weight = 255
	male_height_range = 10
	male_weight_range = 65
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Half Giant"
	background_starting_chart = 75


func apply_ancestral_status(player: Player) -> void:
	player.has_sustain_strength = true
	player.has_shard_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _dwarf_syllable_1[Rng.roll_die(_dwarf_syllable_1.size() - 1)]
		name += _dwarf_syllable_2[Rng.roll_die(_dwarf_syllable_2.size() - 1)]
		name += _dwarf_syllable_3[Rng.roll_die(_dwarf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_SHARDS)
	f_2.set_bits(ItemFlags2.SUST_STR)


func get_ancestral_power_text(player: Player) -> String:
	return "stone to mud       (racial, unusable until level 20)" if player.level < 20 else "stone to mud       (racial, cost 10, STR based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 19:
		list.append("You can break stone walls (cost 10).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(20, 10, Ability.STRENGTH, 12):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You bash at a stone wall.")
		savegame.spell_effects_handler.wall_to_mud(dir)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
