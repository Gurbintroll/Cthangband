# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionRogue
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [2, 1, -2, 3, 1, -1]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 30
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 32
	base_disarm_bonus = 45
	base_melee_attack_bonus = 60
	base_ranged_attack_bonus = 66
	base_save_bonus = 28
	base_search_bonus = 32
	base_search_frequency = 24
	base_stealth_bonus = 5
	casting_type = CastingType.ARCANE
	description_line_2 = "Stealth based characters who are adept at picking locks,"
	description_line_3 = "searching, and disarming traps. Rogues can use stealth to"
	description_line_4 = "their advantage in order to backstab sleeping or fleeing"
	description_line_5 = "foes. They also dabble in INT based magic, learning spells"
	description_line_6 = "from the Tarot, Sorcery, Death, or Folk realms."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 15
	experience_factor = 25
	first_realm_choice = Realm.SORCERY | Realm.DEATH | Realm.TAROT | Realm.FOLK
	has_backstab = true
	has_detailed_sense_inventory = true
	has_spells = true
	hit_die_bonus = 6
	legendary_item_bias = LegendaryItemBias.ROGUE
	melee_attack_bonus_per_level = 40
	pet_upkeep_divider = 20
	prime_ability_score = Ability.DEXTERITY
	ranged_attack_bonus_per_level = 10
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 350
	stealth_bonus_per_level = 0
	title = "Rogue"


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.DAGGER),
		ItemIdentifier.new(ItemCategory.SOFT_ARMOUR, SoftArmourType.SOFT_LEATHER),
	]


func get_profession_sub_name(realm: int) -> String:
	match realm:
		Realm.SORCERY:
			return "Burglar"
		Realm.DEATH:
			return "Assassin"
		Realm.TAROT:
			return "Card Sharp"
		_:
			return "Thief"


func spell_base_failure_multiplier(realm: int) -> float:
	return 1.2 if (realm == Realm.SORCERY or realm == Realm.DEATH or realm == Realm.TAROT or realm == Realm.FOLK) else 99.0


func spell_level_multiplier(realm: int) -> float:
	return 1.4 if (realm == Realm.SORCERY or realm == Realm.DEATH or realm == Realm.TAROT or realm == Realm.FOLK) else 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 1.4 if (realm == Realm.SORCERY or realm == Realm.DEATH or realm == Realm.TAROT or realm == Realm.FOLK) else 99.0


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(20000 / ((player_level * player_level) + 40)) == 1
