# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryMindFlayer
extends Ancestry


func _init() -> void:
	ability_bonus = [-3, 4, 4, 0, -2, -5]
	article = "a"
	age_range = 25
	base_age = 100
	base_device_bonus = 25
	base_disarm_bonus = 10
	base_melee_attack_bonus = -10
	base_ranged_attack_bonus = -5
	base_save_bonus = 15
	base_search_bonus = 5
	base_search_frequency = 12
	base_stealth_bonus = 2
	description_line_1 = ""
	description_line_2 = "Mind-Flayers are slimy humanoids with squid-like tentacles"
	description_line_3 = "around their mouths. They are all psychic, and neither"
	description_line_4 = "their intelligence nor their wisdom can be reduced. They"
	description_line_5 = "can learn to see invisible (at lvl 15), blast people's"
	description_line_6 = "minds (at lvl 15), and gain telepathy (at lvl 30)."
	doesnt_eat = false
	experience_factor = 140
	feeds_on_nether = false
	female_base_height = 63
	female_base_weight = 112
	female_height_range = 6
	female_weight_range = 10
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 68
	male_base_weight = 142
	male_height_range = 6
	male_weight_range = 15
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Mind Flayer"
	background_starting_chart = 92


func apply_ancestral_status(player: Player) -> void:
	player.has_sustain_intelligence = true
	player.has_sustain_wisdom = true
	if player.level > 14:
		player.has_see_invisibility = true
	if player.level > 29:
		player.has_telepathy = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _cthuloid_syllable_1[Rng.roll_die(_cthuloid_syllable_1.size() - 1)]
		name += _cthuloid_syllable_2[Rng.roll_die(_cthuloid_syllable_2.size() - 1)]
		name += _cthuloid_syllable_3[Rng.roll_die(_cthuloid_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.SUST_INT)
	f_2.set_bits(ItemFlags2.SUST_WIS)
	if player.level > 14:
		f_3.set_bits(ItemFlags3.SEE_INVIS)
	if player.level > 29:
		f_3.set_bits(ItemFlags3.TELEPATHY)


func get_ancestral_power_text(player: Player) -> String:
	return "mind blast         (racial, unusable until level 15)" if player.level < 15 else "mind blast         (racial, cost 12, dam lvl, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 14:
		list.append("You can mind blast your enemies, dam %d (cost 12)." % [player.level])
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(15, 12, Ability.INTELLIGENCE, 14):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You concentrate and your eyes glow red...")
		savegame.spell_effects_handler.fire_bolt(
			ProjectilePsi.new(savegame.spell_effects_handler),
			dir,
			player.level,
		)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
