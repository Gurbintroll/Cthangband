# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryImp
extends Ancestry


func _init() -> void:
	ability_bonus = [-1, -1, -1, 1, 2, -3]
	article = "an"
	age_range = 13
	base_age = 1
	base_device_bonus = 2
	base_disarm_bonus = -3
	base_melee_attack_bonus = 5
	base_ranged_attack_bonus = -5
	base_save_bonus = -1
	base_search_bonus = -1
	base_search_frequency = 10
	base_stealth_bonus = 1
	description_line_1 = ""
	description_line_2 = "Imps are minor demons that have escaped their binding and"
	description_line_3 = "are able to run free in the world. Imps naturally resist"
	description_line_4 = "fire, and can learn to throw bolt of flame (at lvl 10),"
	description_line_5 = "see invisible creatures (at lvl 10), become completely"
	description_line_6 = "immune to fire (at lvl 20), and cast fireballs (at lvl 30)."
	doesnt_eat = false
	experience_factor = 110
	feeds_on_nether = false
	female_base_height = 64
	female_base_weight = 120
	female_height_range = 1
	female_weight_range = 5
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 3
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 68
	male_base_weight = 150
	male_height_range = 1
	male_weight_range = 5
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Imp"
	background_starting_chart = 94


func apply_ancestral_status(player: Player) -> void:
	player.has_fire_resistance = true
	if player.level > 9:
		player.has_see_invisibility = true
	if player.level > 19:
		player.has_fire_immunity = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _angel_syllable_1[Rng.roll_die(_angel_syllable_1.size() - 1)]
		name += _angel_syllable_2[Rng.roll_die(_angel_syllable_2.size() - 1)]
		name += _angel_syllable_3[Rng.roll_die(_angel_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_FIRE)
	if player.level > 9:
		f_3.set_bits(ItemFlags3.SEE_INVIS)
	if player.level > 19:
		f_2.set_bits(ItemFlags2.IM_FIRE)


func get_ancestral_power_text(player: Player) -> String:
	return "fire bolt/ball     (racial, unusable until level 9/30)" if player.level < 9 else "fire bolt/ball(30) (racial, cost 15, dam lvl, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 29:
		list.append("You can cast a Fire Ball, dam %d (cost 15)." % [player.level])
	elif player.level > 8:
		list.append("You can cast a Fire Bolt, dam %d (cost 15)." % [player.level])
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(9, 15, Ability.WISDOM, 15):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		if player.level > 29:
			Profile.instance.message_print("You cast a ball of fire.")
			savegame.spell_effects_handler.fire_ball(
				ProjectileFire.new(savegame.spell_effects_handler),
				dir,
				player.level,
				2,
			)
		else:
			Profile.instance.message_print("You cast a bolt of fire.")
			savegame.spell_effects_handler.fire_bolt(
				ProjectileFire.new(savegame.spell_effects_handler),
				dir,
				player.level,
			)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
