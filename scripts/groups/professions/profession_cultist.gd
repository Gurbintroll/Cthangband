# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionCultist
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-5, 3, 0, 1, -2, 1]
	attacks_per_turn_max = 4
	attacks_per_turn_min_weapon_weight = 40
	attacks_per_turn_weight_multiplier = 2
	base_device_bonus = 36
	base_disarm_bonus = 30
	base_melee_attack_bonus = 30
	base_ranged_attack_bonus = 20
	base_save_bonus = 32
	base_search_bonus = 16
	base_search_frequency = 18
	base_stealth_bonus = 1
	casting_type = CastingType.ARCANE
	description_line_2 = "INT based spell casters, who use Chaos and another realm"
	description_line_3 = "of their choice. Can't wield weapons except for powerful"
	description_line_4 = "chaos blades. Learn to resist chaos (at lvl 20). Have a"
	description_line_5 = "cult patron who will randomly give them rewards or"
	description_line_6 = "punishments as they increase in level."
	device_bonus_per_level = 13
	disarm_bonus_per_level = 7
	experience_factor = 30
	first_realm_choice = Realm.CHAOS
	has_minimum_spell_failure = false
	has_goo_patron = true
	has_spells = true
	hit_die_bonus = 0
	legendary_item_bias = LegendaryItemBias.MAGE
	melee_attack_bonus_per_level = 15
	pet_upkeep_divider = 20
	prime_ability_score = Ability.INTELLIGENCE
	ranged_attack_bonus_per_level = 15
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.LIFE | Realm.SORCERY | Realm.NATURE | Realm.DEATH | Realm.TAROT | Realm.FOLK | Realm.CORPOREAL
	spell_weight_limit = 300
	stealth_bonus_per_level = 0
	title = "Cultist"
	warrior_artifact_bias = 0


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.DEATH_BOOK, 0),
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_INT),
	]


func apply_statuses_from_profession(player: Player) -> void:
	if player.level > 19:
		player.has_chaos_resistance = true


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	if player.level > 19:
		f2.set_bits(ItemFlags2.RES_CHAOS)


func spell_base_failure_multiplier(realm: int) -> float:
	return 0.9 if realm == Realm.CHAOS else 1.1


func spell_level_multiplier(realm: int) -> float:
	return 0.9 if realm == Realm.CHAOS else 1.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 0.9 if realm == Realm.CHAOS else 1.2


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(240000 / (player_level + 5)) == 1
