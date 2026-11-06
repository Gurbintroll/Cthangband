# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryVampire
extends Ancestry


func _init() -> void:
	ability_bonus = [3, 3, -1, -1, 1, 2]
	article = "a"
	age_range = 30
	base_age = 100
	base_device_bonus = 10
	base_disarm_bonus = 4
	base_melee_attack_bonus = 5
	base_ranged_attack_bonus = 0
	base_save_bonus = 10
	base_search_bonus = 1
	base_search_frequency = 8
	base_stealth_bonus = 4
	description_line_1 = ""
	description_line_2 = "Vampires are powerful undead. They resist darkness, nether,"
	description_line_3 = "cold, poison, and having their life force drained. Vampires"
	description_line_4 = "produce their own ethereal light in the dark, but are hurt"
	description_line_5 = "by direct sunlight. They can learn to drain the life force"
	description_line_6 = "from their foes (at lvl 2)."
	doesnt_eat = true
	experience_factor = 200
	feeds_on_nether = false
	female_base_height = 66
	female_base_weight = 150
	female_height_range = 4
	female_weight_range = 20
	glows = true
	has_speed_bonus = false
	hit_die_bonus = 11
	infravision = 5
	is_incorporeal = false
	is_nocturnal = true
	is_sunlight_sensitive = true
	male_base_height = 72
	male_base_weight = 180
	male_height_range = 6
	male_weight_range = 25
	mutates = false
	resists_fear = false
	sanity_resistant = true
	spills_potions = false
	title = "Vampire"
	background_starting_chart = 113


func apply_ancestral_status(player: Player) -> void:
	player.has_dark_resistance = true
	player.has_hold_life = true
	player.has_nether_resistance = true
	player.has_cold_resistance = true
	player.has_poison_resistance = true
	player.has_glow = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _human_syllable_1[Rng.roll_die(_human_syllable_1.size() - 1)]
		name += _human_syllable_2[Rng.roll_die(_human_syllable_2.size() - 1)]
		name += _human_syllable_3[Rng.roll_die(_human_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.HOLD_LIFE)
	f_2.set_bits(ItemFlags2.RES_DARK)
	f_2.set_bits(ItemFlags2.RES_NETHER)
	f_3.set_bits(ItemFlags3.LIGHT_SOURCE)
	f_2.set_bits(ItemFlags2.RES_POIS)
	f_2.set_bits(ItemFlags2.RES_COLD)


func get_ancestral_power_text(player: Player) -> String:
	return "drain life         (racial, unusable until level 2)" if player.level < 2 else "drain life         (racial, cost 1 + lvl/3, based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 1:
		list.append(
			"You can steal life from a foe, dam. %d-%d (cost %d)."
			% [
				player.level + maxi(1, player.level / 10),
				player.level + (player.level * maxi(1, player.level / 10)),
				1 + (player.level / 3),
			]
		)
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(
		2,
		1 + player.level / 3,
		Ability.CONSTITUTION,
		9,
	):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_no_aim()
		if dir == -1:
			return
		var row: int = player.map_row + DungeonLevel.keypad_direction_row_offset[dir]
		var col: int = player.map_col + DungeonLevel.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		if tile.monster_index == 0:
			Profile.instance.message_print("You bit into thin air!")
			return
		Profile.instance.message_print("You grin and bare your fangs...")
		var dummy: int = player.level + Rng.roll_die(player.level) * maxi(1, player.level / 10)
		if savegame.spell_effects_handler.drain_life(dir, dummy):
			if player.food < Constants.FOOD_FULL:
				player.restore_health(dummy)
			else:
				Profile.instance.message_print("You were not hungry.")
			dummy = player.food + mini(5000, 100 * dummy)
			if player.food < Constants.FOOD_GORGED:
				player.set_food(Constants.FOOD_GORGED if dummy > Constants.FOOD_GORGED else dummy)
		else:
			Profile.instance.message_print("Yechh. That tastes foul.")
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
