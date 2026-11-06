# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryYeek
extends Ancestry


func _init() -> void:
	ability_bonus = [-2, 1, 1, 1, -2, -7]
	article = "a"
	age_range = 3
	base_age = 14
	base_device_bonus = 4
	base_disarm_bonus = 2
	base_melee_attack_bonus = -5
	base_ranged_attack_bonus = -5
	base_save_bonus = 10
	base_search_bonus = 5
	base_search_frequency = 15
	base_stealth_bonus = 3
	description_line_1 = ""
	description_line_2 = "Yeeks are long-eared furry creatures that look vaguely"
	description_line_3 = "like humanoid rabbits. Although physically weak, they make"
	description_line_4 = "passable spell casters. They are resistant to acid, and can"
	description_line_5 = "learn to scream to terrify their foes (at lvl 15) and"
	description_line_6 = "become completely immune to acid (at lvl 20)."
	doesnt_eat = false
	experience_factor = 100
	feeds_on_nether = false
	female_base_height = 50
	female_base_weight = 75
	female_height_range = 3
	female_weight_range = 3
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 7
	infravision = 2
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 50
	male_base_weight = 90
	male_height_range = 3
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Yeek"
	background_starting_chart = 78


func apply_ancestral_status(player: Player) -> void:
	player.has_acid_resistance = true
	if player.level > 19:
		player.has_acid_immunity = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _yeek_syllable_1[Rng.roll_die(_yeek_syllable_1.size() - 1)]
		name += _yeek_syllable_2[Rng.roll_die(_yeek_syllable_2.size() - 1)]
		name += _yeek_syllable_3[Rng.roll_die(_yeek_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_ACID)
	if player.level > 19:
		f_2.set_bits(ItemFlags2.IM_ACID)


func get_ancestral_power_text(player: Player) -> String:
	return "scare monster      (racial, unusable until level 15)" if player.level < 15 else "scare monster      (racial, cost 15, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 14:
		list.append("You can make a terrifying scream (cost 15).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(15, 15, Ability.WISDOM, 10):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You make a horrible scream!")
		savegame.spell_effects_handler.fear_monster(dir, player.level)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
