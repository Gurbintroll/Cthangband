# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionPriest
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-1, -3, 3, -1, 0, 2]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 30
	base_disarm_bonus = 25
	base_melee_attack_bonus = 48
	base_ranged_attack_bonus = 36
	base_save_bonus = 32
	base_search_bonus = 16
	base_search_frequency = 8
	base_stealth_bonus = 2
	blunt_weapons_only = true
	casting_type = CastingType.DIVINE
	description_line_2 = "Devout followers of the Great Ones, Priests use WIS based"
	description_line_3 = "spell casting. They may choose either Life or Death magic,"
	description_line_4 = "and another realm of their choice. Priests can't use edged"
	description_line_5 = "weapons unless they are blessed, but can use any armour."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 7
	experience_factor = 20
	first_realm_choice = Realm.LIFE | Realm.DEATH
	has_deity = true
	has_minimum_spell_failure = true
	has_spells = true
	hit_die_bonus = 2
	legendary_item_bias = LegendaryItemBias.PRIESTLY
	melee_attack_bonus_per_level = 20
	pet_upkeep_divider = 20
	prime_ability_score = Ability.WISDOM
	ranged_attack_bonus_per_level = 20
	save_bonus_per_level = 12
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NATURE | Realm.CHAOS | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	spell_weight_limit = 350
	stealth_bonus_per_level = 0
	title = "Priest"


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.HAFTED, HaftedType.MACE),
		ItemIdentifier.new(ItemCategory.DEATH_BOOK, 0),
	]


func get_profession_sub_name(realm: int) -> String:
	return "Priest" if realm == Realm.LIFE else "Exorcist"


func spell_base_failure_multiplier(realm: int) -> float:
	return 0.8 if (realm == Realm.LIFE or realm == Realm.DEATH) else 1.0 if realm != Realm.SORCERY else 99.0


func spell_level_multiplier(realm: int) -> float:
	return 0.7 if (realm == Realm.LIFE or realm == Realm.DEATH) else 1.1 if realm != Realm.SORCERY else 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 0.8 if (realm == Realm.LIFE or realm == Realm.DEATH) else 1.1 if realm != Realm.SORCERY else 99.0


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(10000 / ((player_level * player_level) + 40)) == 1
