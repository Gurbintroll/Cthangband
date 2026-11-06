# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionRanger
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [2, 2, 0, 1, 1, 1]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 4
	base_device_bonus = 32
	base_disarm_bonus = 30
	base_melee_attack_bonus = 56
	base_ranged_attack_bonus = 72
	base_save_bonus = 28
	base_search_bonus = 24
	base_search_frequency = 16
	base_stealth_bonus = 3
	casting_type = CastingType.DIVINE
	description_line_2 = "Masters of ranged combat, especiallly using bows. Rangers"
	description_line_3 = "supplement their shooting and stealth with INT based spell"
	description_line_4 = "casting from the Nature realm plus another realm of their"
	description_line_5 = "choice from Death, Corporeal, Tarot, Chaos, and Folk."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 8
	experience_factor = 30
	first_realm_choice = Realm.NATURE
	has_detailed_sense_inventory = true
	has_spells = true
	hit_die_bonus = 4
	legendary_item_bias = LegendaryItemBias.RANGER
	melee_attack_bonus_per_level = 30
	pet_upkeep_divider = 20
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 45
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.CHAOS | Realm.DEATH | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	spell_weight_limit = 400
	stealth_bonus_per_level = 0
	title = "Ranger"
	warrior_artifact_bias = 30


func apply_bonus_missile_attacks(player: Player) -> void:
	if player.ammunition_item_category == ItemCategory.ARROW:
		if player.level >= 20:
			player.missile_attacks_per_round += 1
		if player.level >= 40:
			player.missile_attacks_per_round += 1


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.NATURE_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.BROAD_SWORD),
		ItemIdentifier.new(ItemCategory.DEATH_BOOK, 0),
	]


func spell_base_failure_multiplier(realm: int) -> float:
	return 1.0 if (realm == Realm.NATURE) else 1.2


func spell_level_multiplier(realm: int) -> float:
	return 1.0 if (realm == Realm.NATURE) else 1.3


func spell_power_cost_multiplier(realm: int) -> float:
	return 1.0 if (realm == Realm.NATURE) else 1.3


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(95000 / ((player_level * player_level) + 40)) == 1
