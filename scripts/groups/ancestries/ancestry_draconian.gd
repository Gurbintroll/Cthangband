# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryDraconian
extends Ancestry


func _init() -> void:
	ability_bonus = [2, 1, 1, 1, 2, -3]
	article = "a"
	age_range = 33
	base_age = 75
	base_device_bonus = 5
	base_disarm_bonus = 2
	base_melee_attack_bonus = 5
	base_ranged_attack_bonus = 5
	base_save_bonus = 3
	base_search_bonus = 1
	base_search_frequency = 10
	base_stealth_bonus = 0
	description_line_1 = "Draconians are related to dragons and this shows both in"
	description_line_2 = "their physical superiority and their legendary arrogance."
	description_line_3 = "As well as having a breath weapon, their wings let them"
	description_line_4 = "avoid falling damage, and they can learn to resist fire"
	description_line_5 = "(at lvl 5), cold (at lvl 10), acid (at lvl 15), lightning"
	description_line_6 = "(at lvl 20), and poison (at lvl 35)."
	doesnt_eat = false
	experience_factor = 250
	feeds_on_nether = false
	female_base_height = 72
	female_base_weight = 130
	female_height_range = 1
	female_weight_range = 5
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 2
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 76
	male_base_weight = 160
	male_height_range = 1
	male_weight_range = 2
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Draconian"
	background_starting_chart = 89


func apply_ancestral_status(player: Player) -> void:
	player.has_feather_fall = true
	if player.level > 4:
		player.has_fire_resistance = true
	if player.level > 9:
		player.has_cold_resistance = true
	if player.level > 14:
		player.has_acid_resistance = true
	if player.level > 19:
		player.has_lightning_resistance = true
	if player.level > 34:
		player.has_poison_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _gnome_syllable_1[Rng.roll_die(_gnome_syllable_1.size() - 1)]
		name += _gnome_syllable_2[Rng.roll_die(_gnome_syllable_2.size() - 1)]
		name += _gnome_syllable_3[Rng.roll_die(_gnome_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_3.set_bits(ItemFlags3.FEATHER)
	if player.level > 4:
		f_2.set_bits(ItemFlags2.RES_FIRE)
	if player.level > 9:
		f_2.set_bits(ItemFlags2.RES_COLD)
	if player.level > 14:
		f_2.set_bits(ItemFlags2.RES_ACID)
	if player.level > 19:
		f_2.set_bits(ItemFlags2.RES_ELEC)
	if player.level > 34:
		f_2.set_bits(ItemFlags2.RES_POIS)


func get_ancestral_power_text(player: Player) -> String:
	return "breath weapon      (racial, cost lvl, dam 2*lvl, CON based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	list.append(
		"You can breathe fire or cold, dam %d (cost %d)." % [2 * player.level, player.level]
	)
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var proj: Projectile
	var desc: String
	if Rng.roll_die(3) == 1:
		proj = ProjectileCold.new(savegame.spell_effects_handler)
		desc = "cold"
	else:
		proj = ProjectileFire.new(savegame.spell_effects_handler)
		desc = "fire"
	if savegame.command_engine.check_if_racial_power_works(
		1,
		player.level,
		Ability.CONSTITUTION,
		12,
	):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You breathe %s." % desc)
		savegame.spell_effects_handler.fire_ball(
			proj,
			dir,
			player.level * 2,
			-(player.level / 14) + 1,
		)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
