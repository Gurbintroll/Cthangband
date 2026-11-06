# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionWarrior
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [5, -2, -2, 1, 1, -1]
	attacks_per_turn_max = 6
	attacks_per_turn_min_weapon_weight = 30
	attacks_per_turn_weight_multiplier = 5
	base_device_bonus = 18
	base_disarm_bonus = 25
	base_melee_attack_bonus = 70
	base_ranged_attack_bonus = 60
	base_save_bonus = 18
	base_search_bonus = 14
	base_search_frequency = 2
	base_stealth_bonus = 1
	casting_type = CastingType.NONE
	description_line_2 = "Straightforward, no-nonsense fighters. They are the best"
	description_line_3 = "characters at melee combat, and require the least amount"
	description_line_4 = "of experience to increase in level. They can learn to"
	description_line_5 = "resist fear (at lvl 30). The ideal class for novices."
	device_bonus_per_level = 7
	disarm_bonus_per_level = 12
	experience_factor = 0
	first_realm_choice = Realm.NONE
	has_detailed_sense_inventory = true
	hit_die_bonus = 9
	legendary_item_bias = LegendaryItemBias.WARRIOR
	melee_attack_bonus_per_level = 45
	pet_upkeep_divider = 20
	prime_ability_score = Ability.STRENGTH
	ranged_attack_bonus_per_level = 45
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 400
	stealth_bonus_per_level = 0
	title = "Warrior"
	warrior_artifact_bias = 1


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.RING, RingType.RES_FEAR),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.BROAD_SWORD),
		ItemIdentifier.new(ItemCategory.HARD_ARMOUR, HardArmourType.CHAIN_MAIL),
	]


func apply_attack_and_damage_bonus(player: Player) -> void:
	player.attack_bonus += player.level / 5
	player.damage_bonus += player.level / 5
	player.displayed_attack_bonus += player.level / 5
	player.displayed_damage_bonus += player.level / 5


func apply_bonus_melee_attacks(player: Player) -> void:
	player.melee_attacks_per_round += player.level / 15


func apply_bonus_missile_attacks(player: Player) -> void:
	if player.ammunition_item_category <= ItemCategory.BOLT and player.ammunition_item_category >= ItemCategory.SHOT:
		if player.level >= 25:
			player.missile_attacks_per_round += 1
		if player.level >= 50:
			player.missile_attacks_per_round += 1


func apply_statuses_from_profession(player: Player) -> void:
	if player.level >= 29:
		player.has_fear_resistance = true


func gets_experience_for_destroying(_player: Player, _category: int) -> bool:
	return true


func get_abilities_as_item_flags(player: Player, _f1: FlagSet, f2: FlagSet, _f3: FlagSet) -> void:
	if player.level > 29:
		f2.set_bits(ItemFlags2.RES_FEAR)


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(9000 / ((player_level * player_level) + 40)) == 1
