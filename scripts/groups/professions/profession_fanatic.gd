# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionFanatic
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [2, -2, 0, 1, 2, 1]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 30
	attacks_per_turn_weight_multiplier = 4
	base_device_bonus = 24
	base_disarm_bonus = 20
	base_melee_attack_bonus = 66
	base_ranged_attack_bonus = 40
	base_save_bonus = 30
	base_search_bonus = 14
	base_search_frequency = 12
	base_stealth_bonus = 1
	casting_type = CastingType.DIVINE
	description_line_2 = "Warriors who dabble in WIS based Chaos magic. Have a cult"
	description_line_3 = "patron who will randomly give them rewards or punishments"
	description_line_4 = "as they increase in level. Learn to resist chaos"
	description_line_5 = "(at lvl 30) and fear (at lvl 40)."
	device_bonus_per_level = 11
	disarm_bonus_per_level = 7
	experience_factor = 35
	first_realm_choice = Realm.CHAOS
	has_detailed_sense_inventory = true
	has_goo_patron = true
	has_spells = true
	hit_die_bonus = 6
	legendary_item_bias = LegendaryItemBias.CHAOS
	melee_attack_bonus_per_level = 35
	pet_upkeep_divider = 20
	prime_ability_score = Ability.WISDOM
	ranged_attack_bonus_per_level = 30
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 400
	stealth_bonus_per_level = 0
	title = "Fanatic"
	warrior_artifact_bias = 40


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.BROAD_SWORD),
		ItemIdentifier.new(ItemCategory.HARD_ARMOUR, HardArmourType.SCALE_MAIL),
	]


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	if player.level > 39:
		f2.set_bits(ItemFlags2.RES_FEAR)
	if player.level > 29:
		f2.set_bits(ItemFlags2.RES_CHAOS)


func apply_statuses_from_profession(player: Player) -> void:
	if player.level > 39:
		player.has_fear_resistance = true
	if player.level > 29:
		player.has_chaos_resistance = true


func spell_base_failure_multiplier(realm: int) -> float:
	return 1.0 if realm == Realm.CHAOS else 99.0


func spell_level_multiplier(realm: int) -> float:
	return 1.0 if realm == Realm.CHAOS else 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 1.1 if realm == Realm.CHAOS else 99.0


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(80000 / ((player_level * player_level) + 40)) == 1
