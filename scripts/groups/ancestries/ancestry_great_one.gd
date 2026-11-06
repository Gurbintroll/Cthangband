# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryGreatOne
extends Ancestry


func _init() -> void:
	ability_bonus = [1, 2, 2, 2, 3, 2]
	article = "a"
	age_range = 50
	base_age = 50
	base_device_bonus = 5
	base_disarm_bonus = 4
	base_melee_attack_bonus = 15
	base_ranged_attack_bonus = 10
	base_save_bonus = 5
	base_search_bonus = 3
	base_search_frequency = 13
	base_stealth_bonus = 2
	description_line_1 = ""
	description_line_2 = "Great-Ones are the offspring of the petty gods that rule"
	description_line_3 = "Dreamlands. As such they are somewhat more than human."
	description_line_4 = "Their constitution cannot be reduced, and they heal"
	description_line_5 = "quickly. They can also learn to travel through dreams"
	description_line_6 = "(at lvl 30) and restore their health (at lvl 40)."
	doesnt_eat = false
	experience_factor = 225
	feeds_on_nether = false
	female_base_height = 78
	female_base_weight = 180
	female_height_range = 6
	female_weight_range = 15
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 0
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 82
	male_base_weight = 190
	male_height_range = 5
	male_weight_range = 20
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Great One"
	background_starting_chart = 67


func apply_ancestral_status(player: Player) -> void:
	player.has_sustain_constitution = true
	player.has_regeneration = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.SUST_CON)
	f_3.set_bits(ItemFlags3.REGEN)


func get_ancestral_power_text(player: Player) -> String:
	return "dream powers    (unusable until level 30/40)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 29:
		list.append("You can dream travel (cost 50).")
	if player.level > 39:
		list.append("You can dream a better self (cost 75).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var dream_power: int = 0
	while dream_power == 0:
		var choice: String = User.get_simple_choice("Use Dream [T]ravel or [D]reaming? ")
		if choice == "":
			return
		if choice == "D" or choice == "d":
			dream_power = 1
		if choice == "T" or choice == "t":
			dream_power = 2
	if dream_power == 1:
		if savegame.command_engine.check_if_racial_power_works(40, 75, Ability.WISDOM, 50):
			Profile.instance.message_print("You dream of a time of health and peace...")
			player.set_timed_poison(0)
			player.set_timed_hallucinations(0)
			player.set_timed_stun(0)
			player.set_timed_bleeding(0)
			player.set_timed_blindness(0)
			player.set_timed_fear(0)
			player.try_restoring_ability_score(Ability.STRENGTH)
			player.try_restoring_ability_score(Ability.INTELLIGENCE)
			player.try_restoring_ability_score(Ability.WISDOM)
			player.try_restoring_ability_score(Ability.DEXTERITY)
			player.try_restoring_ability_score(Ability.CONSTITUTION)
			player.try_restoring_ability_score(Ability.CHARISMA)
			player.restore_level()
	elif dream_power == 2:
		Profile.instance.message_print("You start walking around. Your surroundings change.")
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
		savegame.new_level_flag = true
		savegame.came_from = LevelStart.START_RANDOM
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
