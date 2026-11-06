# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionSummoner
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [3, 1, -1, 0, 2, 2]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 30
	attacks_per_turn_weight_multiplier = 4
	base_device_bonus = 26
	base_disarm_bonus = 24
	base_melee_attack_bonus = 60
	base_ranged_attack_bonus = 65
	base_save_bonus = 28
	base_search_bonus = 12
	base_search_frequency = 2
	base_stealth_bonus = 1
	casting_type = CastingType.ARCANE
	description_line_2 = "Experts at Tarot magic, and also capable of fighting,"
	description_line_3 = "summoners fight alongside their summoned allies. They"
	description_line_4 = "can have more summoned creatures than other characters."
	description_line_5 = "Summoners are INT based partial spell casters who learn"
	description_line_6 = "and cast spells from the Tarot realm."
	device_bonus_per_level = 11
	disarm_bonus_per_level = 7
	experience_factor = 35
	first_realm_choice = Realm.TAROT
	has_detailed_sense_inventory = true
	has_minimum_spell_failure = true
	has_spells = true
	hit_die_bonus = 6
	legendary_item_bias = LegendaryItemBias.RANGER
	melee_attack_bonus_per_level = 40
	pet_upkeep_divider = 5
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 40
	save_bonus_per_level = 11
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 400
	stealth_bonus_per_level = 0
	title = "Summoner"
	warrior_artifact_bias = 30


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.BROAD_SWORD),
		ItemIdentifier.new(ItemCategory.SOFT_ARMOUR, SoftArmourType.SOFT_LEATHER),
	]


func spell_base_failure_multiplier(realm: int) -> float:
	return 1 if realm == Realm.TAROT else 99.0


func spell_level_multiplier(realm: int) -> float:
	return 1 if realm == Realm.TAROT else 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 1.1 if realm == Realm.TAROT else 99.0


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(80000 / (player_level * player_level + 40)) == 1
