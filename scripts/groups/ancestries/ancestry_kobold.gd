# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryKobold
extends Ancestry


func _init() -> void:
	ability_bonus = [1, -1, 0, 1, 0, -4]
	article = "a"
	age_range = 3
	base_age = 11
	base_device_bonus = -3
	base_disarm_bonus = -2
	base_melee_attack_bonus = 10
	base_ranged_attack_bonus = -8
	base_save_bonus = 2
	base_search_bonus = 1
	base_search_frequency = 8
	base_stealth_bonus = -1
	description_line_1 = ""
	description_line_2 = "Kobolds are small reptillian creatures whose claims to be"
	description_line_3 = "related to dragons are generally not taken seriously. They"
	description_line_4 = "are resistant to poison, and can learn to throw poison"
	description_line_5 = "darts (at lvl 9)."
	description_line_6 = ""
	doesnt_eat = false
	experience_factor = 125
	feeds_on_nether = false
	female_base_height = 55
	female_base_weight = 100
	female_height_range = 1
	female_weight_range = 5
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 60
	male_base_weight = 130
	male_height_range = 1
	male_weight_range = 5
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Kobold"
	background_starting_chart = 82


func apply_ancestral_status(player: Player) -> void:
	player.has_poison_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _hobbit_syllable_1[Rng.roll_die(_hobbit_syllable_1.size() - 1)]
		name += _hobbit_syllable_2[Rng.roll_die(_hobbit_syllable_2.size() - 1)]
		name += _hobbit_syllable_3[Rng.roll_die(_hobbit_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_POIS)


func get_ancestral_power_text(player: Player) -> String:
	return "poison dart        (racial, unusable until level 12)" if player.level < 12 else "poison dart        (racial, cost 8, dam lvl, DEX based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 11:
		list.append("You can throw a dart of poison, dam. %d (cost 8)." % [player.level])
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(12, 8, Ability.DEXTERITY, 14):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You throw a dart of poison.")
		savegame.spell_effects_handler.fire_bolt(
			ProjectilePoison.new(savegame.spell_effects_handler),
			dir,
			player.level,
		)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
