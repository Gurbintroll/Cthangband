# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryGnome
extends Ancestry


func _init() -> void:
	ability_bonus = [-1, 2, 0, 2, 1, -2]
	article = "a"
	age_range = 40
	base_age = 50
	base_device_bonus = 12
	base_disarm_bonus = 10
	base_melee_attack_bonus = -8
	base_ranged_attack_bonus = 12
	base_save_bonus = 12
	base_search_bonus = 6
	base_search_frequency = 13
	base_stealth_bonus = 3
	description_line_1 = "Gnomes are small, playful, and talented at magic. However,"
	description_line_2 = "they are almost chronically incapable of taking anything"
	description_line_3 = "seriously. Gnomes are constantly fidgeting and always on"
	description_line_4 = "the move, and this makes them impossible to paralyse or"
	description_line_5 = "magically slow. Gnomes are even able to learn how to"
	description_line_6 = "teleport short distances (at lvl 5)."
	doesnt_eat = false
	experience_factor = 135
	feeds_on_nether = false
	female_base_height = 39
	female_base_weight = 75
	female_height_range = 3
	female_weight_range = 3
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 8
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 42
	male_base_weight = 90
	male_height_range = 3
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Gnome"
	background_starting_chart = 13


func apply_ancestral_status(player: Player) -> void:
	player.has_free_action = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _gnome_syllable_1[Rng.roll_die(_gnome_syllable_1.size() - 1)]
		name += _gnome_syllable_2[Rng.roll_die(_gnome_syllable_2.size() - 1)]
		name += _gnome_syllable_3[Rng.roll_die(_gnome_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.FREE_ACT)


func get_ancestral_power_text(player: Player) -> String:
	return "teleport           (racial, unusable until level 5)" if player.level < 5 else "teleport           (racial, cost 5 + lvl/5, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 4:
		list.append(
			"You can teleport, range %d (cost %d)." % [10 + player.level, 5 + (player.level / 5)]
		)
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(
		5,
		5 + (player.level / 5),
		Ability.INTELLIGENCE,
		12,
	):
		Profile.instance.message_print("Blink!")
		savegame.spell_effects_handler.teleport_player(10 + player.level)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
