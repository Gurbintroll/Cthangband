# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestrySprite
extends Ancestry


func _init() -> void:
	ability_bonus = [-4, 3, 3, 3, -2, -2]
	article = "a"
	age_range = 25
	base_age = 50
	base_device_bonus = 10
	base_disarm_bonus = 10
	base_melee_attack_bonus = -12
	base_ranged_attack_bonus = 0
	base_save_bonus = 10
	base_search_bonus = 10
	base_search_frequency = 10
	base_stealth_bonus = 4
	description_line_1 = "Sprites are tiny fairies, distantly related to elves. They"
	description_line_2 = "share their relatives' resistance to light based attacks,"
	description_line_3 = "and their wings both protect them from falling damage and"
	description_line_4 = "allow them to move progressively faster if unencumbered."
	description_line_5 = "Sprites glow in the dark and can learn to throw fairy dust"
	description_line_6 = "to send their enemies to sleep (at lvl 12)."
	doesnt_eat = false
	experience_factor = 175
	feeds_on_nether = false
	female_base_height = 29
	female_base_weight = 65
	female_height_range = 2
	female_weight_range = 2
	glows = true
	has_speed_bonus = true
	hit_die_bonus = 7
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 32
	male_base_weight = 75
	male_height_range = 2
	male_weight_range = 2
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Sprite"
	background_starting_chart = 124


func apply_ancestral_status(player: Player) -> void:
	player.has_light_resistance = true
	player.has_feather_fall = true
	player.has_glow = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _elf_syllable_1[Rng.roll_die(_elf_syllable_1.size() - 1)]
		name += _elf_syllable_2[Rng.roll_die(_elf_syllable_2.size() - 1)]
		name += _elf_syllable_3[Rng.roll_die(_elf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_LIGHT)
	f_3.set_bits(ItemFlags3.FEATHER)
	f_3.set_bits(ItemFlags3.LIGHT_SOURCE)


func get_ancestral_power_text(player: Player) -> String:
	return "sleeping dust      (racial, unusable until level 12)" if player.level < 12 else "sleeping dust      (racial, cost 12, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 11:
		list.append("You can throw magic dust which induces sleep (cost 12).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(12, 12, Ability.INTELLIGENCE, 15):
		Profile.instance.message_print("You throw some magic dust...")
		if player.level < 25:
			savegame.spell_effects_handler.sleep_monsters_touch()
		else:
			savegame.spell_effects_handler.sleep_monsters()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
