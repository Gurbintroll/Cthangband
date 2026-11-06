# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionWarriorMage
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [2, 2, 0, 1, 0, 1]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 30
	base_disarm_bonus = 30
	base_melee_attack_bonus = 50
	base_ranged_attack_bonus = 26
	base_save_bonus = 28
	base_search_bonus = 18
	base_search_frequency = 16
	base_stealth_bonus = 2
	casting_type = CastingType.ARCANE
	description_line_2 = "A blend of both warrior and mage, getting the abilities of"
	description_line_3 = "both but not being the best at either. They use INT based"
	description_line_4 = "spell casting, getting access to the Folk realm plus a"
	description_line_5 = "second realm of their choice. They pay for their extreme"
	description_line_6 = "flexibility by increasing in level only slowly."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 7
	experience_factor = 50
	first_realm_choice = Realm.CHAOS | Realm.CORPOREAL | Realm.DEATH | Realm.FOLK | Realm.LIFE | Realm.NATURE | Realm.SORCERY | Realm.TAROT
	has_spells = true
	hit_die_bonus = 4
	legendary_item_bias = LegendaryItemBias.MAGE
	melee_attack_bonus_per_level = 20
	pet_upkeep_divider = 20
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 20
	save_bonus_per_level = 9
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.CHAOS | Realm.CORPOREAL | Realm.DEATH | Realm.FOLK | Realm.LIFE | Realm.NATURE | Realm.SORCERY | Realm.TAROT
	spell_weight_limit = 350
	stealth_bonus_per_level = 0
	title = "Warrior-Mage"
	warrior_artifact_bias = 40


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.NATURE_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.SHORT_SWORD),
		ItemIdentifier.new(ItemCategory.DEATH_BOOK, 0),
	]


func spell_base_failure_multiplier(realm: int) -> float:
	return 1.0


func spell_level_multiplier(realm: int) -> float:
	return 1.1


func spell_power_cost_multiplier(realm: int) -> float:
	return 1.1


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(75000 / ((player_level * player_level) + 40)) == 1
