# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionChosenOne
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [3, -2, -2, 2, 2, -1]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 18
	base_disarm_bonus = 25
	base_melee_attack_bonus = 50
	base_ranged_attack_bonus = 32
	base_save_bonus = 20
	base_search_bonus = 16
	base_search_frequency = 4
	base_stealth_bonus = 1
	casting_type = CastingType.NONE
	description_line_2 = "Warriors of fate, who have no spell casting abilities but"
	description_line_3 = "gain a large number of passive magical abilities (too long"
	description_line_4 = "to list here) as they increase in level."
	device_bonus_per_level = 7
	disarm_bonus_per_level = 12
	experience_factor = 20
	first_realm_choice = Realm.NONE
	glows = true
	has_detailed_sense_inventory = true
	hit_die_bonus = 4
	legendary_item_bias = LegendaryItemBias.WARRIOR
	melee_attack_bonus_per_level = 20
	pet_upkeep_divider = 20
	prime_ability_score = Ability.STRENGTH
	ranged_attack_bonus_per_level = 20
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	stealth_bonus_per_level = 0
	title = "Chosen One"
	warrior_artifact_bias = 0


func apply_statuses_from_profession(player: Player) -> void:
	player.has_glow = true
	if player.level >= 2:
		player.has_confusion_resistance = true
	if player.level >= 4:
		player.has_fear_resistance = true
	if player.level >= 6:
		player.has_blindness_resistance = true
	if player.level >= 8:
		player.has_feather_fall = true
	if player.level >= 10:
		player.has_see_invisibility = true
	if player.level >= 12:
		player.has_slow_digestion = true
	if player.level >= 14:
		player.has_sustain_constitution = true
	if player.level >= 16:
		player.has_poison_resistance = true
	if player.level >= 18:
		player.has_sustain_dexterity = true
	if player.level >= 20:
		player.has_sustain_strength = true
	if player.level >= 22:
		player.has_hold_life = true
	if player.level >= 24:
		player.has_free_action = true
	if player.level >= 26:
		player.has_telepathy = true
	if player.level >= 28:
		player.has_dark_resistance = true
	if player.level >= 30:
		player.has_light_resistance = true
	if player.level >= 32:
		player.has_sustain_charisma = true
	if player.level >= 34:
		player.has_sound_resistance = true
	if player.level >= 36:
		player.has_disenchant_resistance = true
	if player.level >= 38:
		player.has_regeneration = true
	if player.level >= 40:
		player.has_sustain_intelligence = true
	if player.level >= 42:
		player.has_chaos_resistance = true
	if player.level >= 44:
		player.has_sustain_wisdom = true
	if player.level >= 46:
		player.has_nexus_resistance = true
	if player.level >= 48:
		player.has_shard_resistance = true
	if player.level >= 50:
		player.has_nether_resistance = true


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	f3.set_bits(ItemFlags3.LIGHT_SOURCE)
	if player.level >= 2:
		f2.set_bits(ItemFlags2.RES_CONF)
	if player.level >= 4:
		f2.set_bits(ItemFlags2.RES_FEAR)
	if player.level >= 6:
		f2.set_bits(ItemFlags2.RES_BLIND)
	if player.level >= 8:
		f3.set_bits(ItemFlags3.FEATHER)
	if player.level >= 10:
		f3.set_bits(ItemFlags3.SEE_INVIS)
	if player.level >= 12:
		f3.set_bits(ItemFlags3.SLOW_DIGEST)
	if player.level >= 14:
		f2.set_bits(ItemFlags2.SUST_CON)
	if player.level >= 16:
		f2.set_bits(ItemFlags2.RES_POIS)
	if player.level >= 18:
		f2.set_bits(ItemFlags2.SUST_DEX)
	if player.level >= 20:
		f2.set_bits(ItemFlags2.SUST_STR)
	if player.level >= 22:
		f2.set_bits(ItemFlags2.HOLD_LIFE)
	if player.level >= 24:
		f2.set_bits(ItemFlags2.FREE_ACT)
	if player.level >= 26:
		f3.set_bits(ItemFlags3.TELEPATHY)
	if player.level >= 28:
		f2.set_bits(ItemFlags2.RES_DARK)
	if player.level >= 30:
		f2.set_bits(ItemFlags2.RES_LIGHT)
	if player.level >= 32:
		f2.set_bits(ItemFlags2.SUST_CHA)
	if player.level >= 34:
		f2.set_bits(ItemFlags2.RES_SOUND)
	if player.level >= 36:
		f2.set_bits(ItemFlags2.RES_DISEN)
	if player.level >= 38:
		f3.set_bits(ItemFlags3.REGEN)
	if player.level >= 40:
		f2.set_bits(ItemFlags2.SUST_INT)
	if player.level >= 42:
		f2.set_bits(ItemFlags2.RES_CHAOS)
	if player.level >= 44:
		f2.set_bits(ItemFlags2.SUST_WIS)
	if player.level >= 46:
		f2.set_bits(ItemFlags2.RES_NEXUS)
	if player.level >= 48:
		f2.set_bits(ItemFlags2.RES_SHARDS)
	if player.level >= 50:
		f2.set_bits(ItemFlags2.RES_NETHER)


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.WAND, WandType.MAGIC_MISSILE),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.DAGGER),
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_CHA),
	]


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(9000 / ((player_level * player_level) + 40)) == 1
