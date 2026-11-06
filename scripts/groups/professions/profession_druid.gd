# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionDruid
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [1, -3, 4, -2, 0, 3]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 30
	base_disarm_bonus = 30
	base_melee_attack_bonus = 48
	base_ranged_attack_bonus = 36
	base_save_bonus = 32
	base_search_bonus = 20
	base_search_frequency = 8
	base_stealth_bonus = 3
	casting_type = CastingType.DIVINE
	description_line_2 = "Nature priests who use WIS based spell casting and who are"
	description_line_3 = "limited to the Nature realm. As priests, they can't use"
	description_line_4 = "edged weapons unless those weapons are holy; but they can"
	description_line_5 = "wear heavy armour without it disrupting their casting."
	description_line_6 = "Druids resist poison and pay less mana upkeep for allies."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 8
	experience_factor = 20
	first_realm_choice = Realm.NATURE
	has_minimum_spell_failure = false
	has_spells = true
	hit_die_bonus = 3
	legendary_item_bias = LegendaryItemBias.PRIESTLY
	melee_attack_bonus_per_level = 20
	pet_upkeep_divider = 10
	prime_ability_score = Ability.WISDOM
	ranged_attack_bonus_per_level = 20
	save_bonus_per_level = 12
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 350
	stealth_bonus_per_level = 0
	title = "Druid"
	warrior_artifact_bias = 0


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SORCERY_BOOK, 0),
		ItemIdentifier.new(ItemCategory.HAFTED, HaftedType.QUARTERSTAFF),
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_WIS),
	]


func spell_base_failure_multiplier(realm: int) -> float:
	return 0.8 if realm == Realm.NATURE else 99.0


func spell_level_multiplier(realm: int) -> float:
	return 0.7 if realm == Realm.NATURE else 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 0.8 if realm == Realm.NATURE else 99.0


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	f2.set_bits(ItemFlags2.RES_POIS)


func apply_statuses_from_profession(player: Player) -> void:
	player.has_poison_resistance = true


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(10000 / ((player_level * player_level) + 40)) == 1
