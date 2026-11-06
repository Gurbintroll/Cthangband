# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AncestryCyclops
extends Ancestry


func _init() -> void:
	ability_bonus = [4, -3, -3, -3, 4, -6]
	article = "a"
	age_range = 24
	base_age = 50
	base_device_bonus = -5
	base_disarm_bonus = -4
	base_melee_attack_bonus = 20
	base_ranged_attack_bonus = 12
	base_save_bonus = -5
	base_search_bonus = -2
	base_search_frequency = 5
	base_stealth_bonus = -2
	description_line_1 = ""
	description_line_2 = "Cyclopes are one eyed giants, often seen as freaks by the"
	description_line_3 = "other races. They can learn to throw boulders (at lvl 20)"
	description_line_4 = "and although they have weak eyesight their hearing is very"
	description_line_5 = "keen and hard to damage, so they are resistant to sound"
	description_line_6 = "based attacks."
	doesnt_eat = false
	experience_factor = 130
	feeds_on_nether = false
	female_base_height = 80
	female_base_weight = 235
	female_height_range = 8
	female_weight_range = 60
	glows = false
	has_speed_bonus = false
	hit_die_bonus = 13
	infravision = 1
	is_incorporeal = false
	is_nocturnal = false
	is_sunlight_sensitive = false
	male_base_height = 92
	male_base_weight = 255
	male_height_range = 10
	male_weight_range = 60
	mutates = false
	resists_fear = false
	sanity_immune = false
	spills_potions = false
	title = "Cyclops"
	background_starting_chart = 77


func apply_ancestral_status(player: Player) -> void:
	if player.level > 19:
		player.has_sound_resistance = true


func create_random_name() -> String:
	var name: String = "Seed name that is too long to be valid"
	while name.length() > 12:
		name = _dwarf_syllable_1[Rng.roll_die(_dwarf_syllable_1.size() - 1)]
		name += _dwarf_syllable_2[Rng.roll_die(_dwarf_syllable_2.size() - 1)]
		name += _dwarf_syllable_3[Rng.roll_die(_dwarf_syllable_3.size() - 1)]
	return name


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_2.set_bits(ItemFlags2.RES_SOUND)


func get_ancestral_power_text(player: Player) -> String:
	return "throw boulder      (racial, unusable until level 20)" if player.level < 20 else "throw boulder      (racial, cost 15, dam 3*lvl, STR based)"


func self_knowledge(player: Player) -> Array[String]:
	var list: Array[String]
	if player.level > 19:
		list.append("You can throw a boulder, dam. %d (cost 15)." % [3 * player.level])
	return list


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if savegame.command_engine.check_if_racial_power_works(20, 15, Ability.STRENGTH, 12):
		var target_engine: TargetEngine = TargetEngine.new_target_engine(
			player,
			dungeon_level,
			savegame,
		)
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		Profile.instance.message_print("You throw a huge boulder.")
		savegame.spell_effects_handler.fire_bolt(
			ProjectileMissile.new(savegame.spell_effects_handler),
			dir,
			3 * player.level / 2,
		)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
