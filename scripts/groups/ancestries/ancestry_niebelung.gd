# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryNiebelung
extends Ancestry


func _init() -> void:
	ability_bonus = [1, -1, 2, 0, 2, -4]
	article = "a"
	age_range = 12
	base_age = 40
	base_device_bonus = 5
	base_disarm_bonus = 3
	base_melee_attack_bonus = 9
	base_ranged_attack_bonus = 0
	base_save_bonus = 10
	base_search_bonus = 5
	base_search_frequency = 10
	base_stealth_bonus = 1
	description_line_1 = ""
	description_line_2 = "Nibelungen are also known as dark dwarves and are famous"
	description_line_3 = "as the makers of (often cursed) magical items. They can"
	description_line_4 = "resist darkness and protect the items they are carrying"
	description_line_5 = "from disenchantment. They can also learn to detect traps,"
	description_line_6 = "stairs, and secret doors (at lvl 5)."
	doesnt_eat = false
	experience_factor = 135
	feeds_on_nether = false
	female_base_height = 40
	female_base_weight = 70
	female_height_range = 3
	female_weight_range = 3
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 5
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 43
	male_base_weight = 92
	male_height_range = 3
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Niebelung"
	background_starting_chart = 69


func apply_ancestral_status(player: Player) -> void:
	player.has_dark_resistance = true
	player.has_disenchant_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _dwarf_syllable_1[Rng.roll_die(_dwarf_syllable_1.size() - 1)]
		name += _dwarf_syllable_2[Rng.roll_die(_dwarf_syllable_2.size() - 1)]
		name += _dwarf_syllable_3[Rng.roll_die(_dwarf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_DARK)
	f_2.set_bits(ItemFlags2.RES_DISEN)


func get_ancestral_power_text(player: Player) -> String:
	return "detect doors+traps (racial, unusable until level 5)" if player.level < 5 else "detect doors+traps (racial, cost 5, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 4:
		list.append("You can find traps, doors and stairs (cost 5).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(5, 5, Ability.WISDOM, 10):
		Profile.instance.message_print("You examine your surroundings.")
		savegame.spell_effects_handler.detect_traps()
		savegame.spell_effects_handler.detect_doors()
		savegame.spell_effects_handler.detect_stairs()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
