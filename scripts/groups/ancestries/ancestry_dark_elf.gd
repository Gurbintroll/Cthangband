# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryDarkElf
extends Ancestry


func _init() -> void:
	ability_bonus = [-1, 3, 2, 2, -2, 1]
	article = "a"
	age_range = 75
	base_age = 75
	base_device_bonus = 15
	base_disarm_bonus = 5
	base_melee_attack_bonus = 10
	base_ranged_attack_bonus = 8
	base_save_bonus = 20
	base_search_bonus = 8
	base_search_frequency = 12
	base_stealth_bonus = 3
	description_line_1 = "Dark elves are underground elves who have a kinship with"
	description_line_2 = "fungi the way that surface elves have a kinship with trees."
	description_line_3 = "The innately magical nature of dark elves lets them learn"
	description_line_4 = "to fire magical missiles at their opponents (at lvl 2)."
	description_line_5 = "They also resist dark-based attacks and can learn to see"
	description_line_6 = "invisible creatures (at lvl 20)."
	doesnt_eat = false
	experience_factor = 150
	feeds_on_nether = false
	female_base_height = 54
	female_base_weight = 80
	female_height_range = 4
	female_weight_range = 6
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 9
	infravision = 5
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 60
	male_base_weight = 100
	male_height_range = 4
	male_weight_range = 6
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Dark Elf"
	background_starting_chart = 69


func apply_ancestral_status(player: Player) -> void:
	player.has_dark_resistance = true
	if player.level > 19:
		player.has_see_invisibility = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _elf_syllable_1[Rng.roll_die(_elf_syllable_1.size() - 1)]
		name += _elf_syllable_2[Rng.roll_die(_elf_syllable_2.size() - 1)]
		name += _elf_syllable_3[Rng.roll_die(_elf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_DARK)
	if player.level > 19:
		f_3.set_bits(ItemFlags3.SEE_INVIS)


func get_ancestral_power_text(player: Player) -> String:
	return "magic missile       (racial, unusable until level 2)" if player.level < 2 else "magic missile       (racial, cost 2, INT based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 1:
		list.append(
			"You can cast a Magic Missile, dam %d (cost 2)." % [3 + ((player.level - 1) / 5)]
		)
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(2, 2, Ability.INTELLIGENCE, 9):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You cast a magic missile.")
		savegame.spell_effects_handler.fire_bolt(
			ProjectileMissile.new(savegame.spell_effects_handler),
			dir,
			Rng.roll_dice(3 + ((player.level - 1) / 5), 4),
		)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
