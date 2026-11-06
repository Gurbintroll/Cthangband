# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionHighMage
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-5, 4, 0, 0, -2, 1]
	attacks_per_turn_max = 4
	attacks_per_turn_min_weapon_weight = 40
	attacks_per_turn_weight_multiplier = 2
	base_device_bonus = 38
	base_disarm_bonus = 30
	base_melee_attack_bonus = 34
	base_ranged_attack_bonus = 20
	base_save_bonus = 30
	base_search_bonus = 16
	base_search_frequency = 20
	base_stealth_bonus = 2
	casting_type = CastingType.ARCANE
	description_line_2 = "INT based spell casters who specialise in a single realm"
	description_line_3 = "of magic. They may choose any realm, and are better at"
	description_line_4 = "casting spells from that realm than a normal mage. High"
	description_line_5 = "mages also get more vis than other spell casters do."
	description_line_6 = "Wearing too much armour disrupts their casting."
	device_bonus_per_level = 13
	disarm_bonus_per_level = 7
	experience_factor = 30
	first_realm_choice = Realm.LIFE | Realm.SORCERY | Realm.NATURE | Realm.CHAOS | Realm.DEATH | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	has_spells = true
	hit_die_bonus = 0
	legendary_item_bias = LegendaryItemBias.MAGE
	melee_attack_bonus_per_level = 15
	pet_upkeep_divider = 12
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 15
	save_bonus_per_level = 9
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_ball_size_factor = 2
	spell_weight_limit = 300
	stealth_bonus_per_level = 0
	title = "High-Mage"
	warrior_artifact_bias = 0


func spell_base_failure_multiplier(realm: int) -> float:
	return 0.8


func spell_beam_chance(player_level: int) -> int:
	return player_level + 10


func spell_level_multiplier(realm: int) -> float:
	return 0.8


func spell_power_cost_multiplier(realm: int) -> float:
	return 0.8


func get_profession_sub_name(realm: int) -> String:
	match realm:
		Realm.LIFE:
			return "Vivimancer"
		Realm.SORCERY:
			return "Sorcerer"
		Realm.NATURE:
			return "Naturist"
		Realm.CHAOS:
			return "Warlock"
		Realm.DEATH:
			return "Necromancer"
		Realm.TAROT:
			return "Cartomancer"
		Realm.FOLK:
			return "Hedge Wizard"
		Realm.CORPOREAL:
			return "Zen Master"
		_:
			return "High-Mage"


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.DAGGER),
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_INT),
	]


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(240000 / (player_level + 5)) == 1
