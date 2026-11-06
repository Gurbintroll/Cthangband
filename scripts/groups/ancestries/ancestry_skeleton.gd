# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestrySkeleton extends Ancestry

func _init() -> void:
	ability_bonus = [0, -2, -2, 0, 1, -4]
	article = "a"
	age_range = 35
	base_age = 100
	base_device_bonus = -5
	base_disarm_bonus = -5
	base_melee_attack_bonus = 10
	base_ranged_attack_bonus = 8
	base_save_bonus = 5
	base_search_bonus = -1
	base_search_frequency = 8
	base_stealth_bonus = -1
	description_line_1 = ""
	description_line_2 = "Skeletons are undead creatures. Being without eyes, they"
	description_line_3 = "use magical sight which can see invisible creatures. Their"
	description_line_4 = "lack of flesh means that they resist poison and shards, and"
	description_line_5 = "their life force is hard to drain. They can learn to resist"
	description_line_6 = "cold (at lvl 10), and restore their life force (at lvl 30)."
	doesnt_eat = true
	experience_factor = 145
	feeds_on_nether = false
	female_base_height = 66
	female_base_weight = 50
	female_height_range = 4
	female_weight_range = 5
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 10
	infravision = 2
	is_incorporeal = false
	is_nocturnal = true
	is_sunlight_sensitive = false
	male_base_height = 72
	male_base_weight = 50
	male_height_range = 4
	male_weight_range = 5
	mutates = false
	resists_fear = false
	sanity_resistant = true
	spills_potions = true
	title = "Skeleton"
	background_starting_chart = 102


func apply_ancestral_status(player: Player) -> void:
	player.has_shard_resistance = true
	player.has_hold_life = true
	player.has_see_invisibility = true
	player.has_poison_resistance = true
	if player.level > 9:
		player.has_cold_resistance = true


func consume_food(player:Player, item:Item)->void:
	if not (item.sub_category == FoodType.WAYBREAD or
		item.sub_category == FoodType.WARPSTONE or
		item.sub_category < FoodType.BISCUIT):
		var floor_item:Item = Item.new()
		Profile.instance.message_print("The food falls though your jaws!")
		floor_item.assign_item_type(SaveGame.instance.item_types.look_up_type(item.category, item.sub_category))
		SaveGame.instance.dungeon_level.dungeon_items.drop_near(floor_item, -1, player.map_row, player.map_col)
	else:
		Profile.instance.message_print("The food falls through your jaws and vanishes!")

func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_SHARDS)
	f_2.set_bits(ItemFlags2.HOLD_LIFE)
	f_2.set_bits(ItemFlags2.RES_POIS)
	f_3.set_bits(ItemFlags3.SEE_INVIS)
	if player.level > 9:
		f_2.set_bits(ItemFlags2.RES_COLD)


func get_ancestral_power_text(player: Player) -> String:
	return "restore life       (racial, unusable until level 30)" if player.level < 30 else "restore life       (racial, cost 30, WIS based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 29:
		list.append("You can restore lost life forces (cost 30).")
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(30, 30, Ability.WISDOM, 18):
		Profile.instance.message_print("You attempt to restore your lost energies.")
		player.restore_level()
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
