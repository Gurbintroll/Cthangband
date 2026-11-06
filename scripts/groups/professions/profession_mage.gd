# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionMage
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-5, 3, 0, 1, -2, 1]
	attacks_per_turn_max = 4
	attacks_per_turn_min_weapon_weight = 40
	attacks_per_turn_weight_multiplier = 2
	base_device_bonus = 36
	base_disarm_bonus = 30
	base_melee_attack_bonus = 34
	base_ranged_attack_bonus = 20
	base_save_bonus = 30
	base_search_bonus = 16
	base_search_frequency = 20
	base_stealth_bonus = 2
	casting_type = CastingType.ARCANE
	description_line_2 = "Flexible INT based spell casters who can cast magic from"
	description_line_3 = "any two realms of their choice. However, they can't wear"
	description_line_4 = "much armour before it starts disrupting their casting."
	device_bonus_per_level = 13
	disarm_bonus_per_level = 7
	experience_factor = 30
	first_realm_choice = Realm.LIFE | Realm.SORCERY | Realm.NATURE | Realm.CHAOS | Realm.DEATH | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	has_spells = true
	hit_die_bonus = 0
	legendary_item_bias = LegendaryItemBias.MAGE
	melee_attack_bonus_per_level = 15
	pet_upkeep_divider = 20
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 15
	save_bonus_per_level = 9
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.LIFE | Realm.SORCERY | Realm.NATURE | Realm.CHAOS | Realm.DEATH | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	spell_ball_size_factor = 2
	spell_weight_limit = 300
	stealth_bonus_per_level = 0
	title = "Mage"
	warrior_artifact_bias = 0


func spell_base_failure_multiplier(realm: int) -> float:
	return 1


func spell_beam_chance(player_level: int) -> int:
	return player_level


func spell_level_multiplier(realm: int) -> float:
	return 1


func spell_power_cost_multiplier(realm: int) -> float:
	return 1


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.DAGGER),
		ItemIdentifier.new(ItemCategory.DEATH_BOOK, 0),
	]


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(240000 / (player_level + 5)) == 1
