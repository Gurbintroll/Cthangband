# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryDwarf
extends Ancestry


func _init() -> void:
	ability_bonus = [2, -2, 2, -2, 2, -3]
	article = "a"
	age_range = 15
	base_age = 35
	base_device_bonus = 9
	base_disarm_bonus = 2
	base_melee_attack_bonus = 15
	base_ranged_attack_bonus = 0
	base_save_bonus = 10
	base_search_bonus = 7
	base_search_frequency = 10
	base_stealth_bonus = -1
	description_line_1 = "Dwarves are short and stocky, and although not noted for"
	description_line_2 = "their intelligence or subtlety they are generally very"
	description_line_3 = "pious. They are also rather resistant to spells. As natural"
	description_line_4 = "miners, used to feeling their way around in the dark,"
	description_line_5 = "dwarves are immune to all forms of blindness and can learn"
	description_line_6 = "to detect secret doors and traps (at lvl 5)."
	doesnt_eat = false
	experience_factor = 125
	feeds_on_nether = false
	female_base_height = 46
	female_base_weight = 120
	female_height_range = 3
	female_weight_range = 10
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 5
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 48
	male_base_weight = 150
	male_height_range = 3
	male_weight_range = 10
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Dwarf"
	background_starting_chart = 16


func apply_ancestral_status(player: Player) -> void:
	player.has_blindness_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _dwarf_syllable_1[Rng.roll_die(_dwarf_syllable_1.size() - 1)]
		name += _dwarf_syllable_2[Rng.roll_die(_dwarf_syllable_2.size() - 1)]
		name += _dwarf_syllable_3[Rng.roll_die(_dwarf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_BLIND)


func get_ancestral_power_text(player: Player) -> String:
	return "detect doors+traps (racial, unusable until level 5)" if player.level < 5 else "detect doors+traps (racial, cost 5, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 4:
		list.append("You can find traps, doors and stairs (cost 5).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(5, 5, Ability.WISDOM, 12):
		Profile.instance.message_print("You examine your surroundings.")
		savegame.spell_effects_handler.detect_traps()
		savegame.spell_effects_handler.detect_doors()
		savegame.spell_effects_handler.detect_stairs()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
