# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryHobbit
extends Ancestry


func _init() -> void:
	ability_bonus = [-2, 2, 1, 3, 2, 1]
	article = "a"
	age_range = 12
	base_age = 21
	base_device_bonus = 18
	base_disarm_bonus = 15
	base_melee_attack_bonus = -10
	base_ranged_attack_bonus = 20
	base_save_bonus = 18
	base_search_bonus = 12
	base_search_frequency = 15
	base_stealth_bonus = 5
	description_line_1 = ""
	description_line_2 = "Hobbits are small and surprisingly dextrous given their"
	description_line_3 = "propensity for plumpness. They make excellent burglars"
	description_line_4 = "and are adept at spell casting too. Hobbits can't have"
	description_line_5 = "their dexterity reduced, and they can learn to put together"
	description_line_6 = "nourishing meals from the barest scraps (at lvl 15)."
	doesnt_eat = false
	experience_factor = 110
	feeds_on_nether = false
	female_base_height = 33
	female_base_weight = 50
	female_height_range = 3
	female_weight_range = 3
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 7
	infravision = 4
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 36
	male_base_weight = 60
	male_height_range = 3
	male_weight_range = 3
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Hobbit"
	background_starting_chart = 10


func apply_ancestral_status(player: Player) -> void:
	player.has_sustain_dexterity = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _hobbit_syllable_1[Rng.roll_die(_hobbit_syllable_1.size() - 1)]
		name += _hobbit_syllable_2[Rng.roll_die(_hobbit_syllable_2.size() - 1)]
		name += _hobbit_syllable_3[Rng.roll_die(_hobbit_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.SUST_DEX)


func get_ancestral_power_text(player: Player) -> String:
	return "create food        (racial, unusable until level 15)" if player.level < 15 else "create food        (racial, cost 10, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 1:
		list.append("You can produce food (cost 10).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(15, 10, Ability.INTELLIGENCE, 10):
		var item: Item = Item.new()
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(
				ItemCategory.FOOD,
				FoodType.RATION,
			))
		dungeon_level.dungeon_items.drop_near(item, -1, player.map_row, player.map_col)
		Profile.instance.message_print("You cook some food.")
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
