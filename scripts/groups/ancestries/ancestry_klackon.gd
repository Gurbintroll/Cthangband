# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryKlackon
extends Ancestry


func _init() -> void:
	ability_bonus = [2, -1, -2, 1, 2, -2]
	article = "a"
	age_range = 3
	base_age = 20
	base_device_bonus = 5
	base_disarm_bonus = 10
	base_melee_attack_bonus = 5
	base_ranged_attack_bonus = 5
	base_save_bonus = 5
	base_search_bonus = -1
	base_search_frequency = 10
	base_stealth_bonus = 0
	description_line_1 = "Klackons are humanoid insects. Although most stay safe in"
	description_line_2 = "their hive cities, a small number venture forth in search"
	description_line_3 = "of adventure. The chitin of a klackon resists acid, and"
	description_line_4 = "their ordered minds cannot be confused. They can learn to"
	description_line_5 = "spit acid (at lvl 9) and they get progressively faster if"
	description_line_6 = "unencumbered by armour."
	doesnt_eat = false
	experience_factor = 135
	feeds_on_nether = false
	female_base_height = 54
	female_base_weight = 70
	female_height_range = 3
	female_weight_range = 4
	glows = false
	has_speed_bonus = true
	hit_die_bonus = 12
	infravision = 2
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 60
	male_base_weight = 80
	male_height_range = 3
	male_weight_range = 4
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Klackon"
	background_starting_chart = 84


func apply_ancestral_status(player: Player) -> void:
	player.has_confusion_resistance = true
	player.has_acid_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _klackon_syllable_1[Rng.roll_die(_klackon_syllable_1.size() - 1)]
		name += _klackon_syllable_2[Rng.roll_die(_klackon_syllable_2.size() - 1)]
		name += _klackon_syllable_3[Rng.roll_die(_klackon_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_ACID)
	f_2.set_bits(ItemFlags2.RES_CONF)
	if player.level > 9:
		f_1.set_bits(ItemFlags1.SPEED)


func get_ancestral_power_text(player: Player) -> String:
	return "spit acid          (racial, unusable until level 9)" if player.level < 9 else "spit acid          (racial, cost 9, dam lvl, DEX based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 8:
		list.append("You can cast a spit acid, dam %d (cost 9)." % [player.level])
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(9, 9, Ability.DEXTERITY, 14):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You spit acid.")
		if player.level < 25:
			savegame.spell_effects_handler.fire_ball(
				ProjectileAcid.new(savegame.spell_effects_handler),
				dir,
				player.level,
				2,
			)
		else:
			savegame.spell_effects_handler.fire_bolt(
				ProjectileAcid.new(savegame.spell_effects_handler),
				dir,
				player.level,
			)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
