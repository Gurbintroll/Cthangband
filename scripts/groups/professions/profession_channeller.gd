# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionChanneller
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-1, 0, 2, -1, -1, 3]
	attacks_per_turn_max = 4
	attacks_per_turn_min_weapon_weight = 40
	attacks_per_turn_weight_multiplier = 2
	base_device_bonus = 40
	base_disarm_bonus = 40
	base_melee_attack_bonus = 40
	base_ranged_attack_bonus = 30
	base_save_bonus = 30
	base_search_bonus = 16
	base_search_frequency = 20
	base_stealth_bonus = 2
	casting_type = CastingType.CHANNELLING
	description_line_2 = "Similar to a spell caster, but rather than casting spells"
	description_line_3 = "from a book, they can use their CHA to channel power into"
	description_line_4 = "most types of item, powering the effects of the items"
	description_line_5 = "without depleting them."
	device_bonus_per_level = 13
	disarm_bonus_per_level = 12
	experience_factor = 30
	first_realm_choice = Realm.NONE
	has_detailed_sense_inventory = true
	hit_die_bonus = 1
	legendary_item_bias = LegendaryItemBias.MAGE
	melee_attack_bonus_per_level = 15
	pet_upkeep_divider = 20
	prime_ability_score = Ability.CHARISMA
	ranged_attack_bonus_per_level = 15
	save_bonus_per_level = 9
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 400
	stealth_bonus_per_level = 0
	title = "Channeller"
	warrior_artifact_bias = 0


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.WAND, WandType.MAGIC_MISSILE),
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.DAGGER),
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_CHA),
	]


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(9000 / ((player_level * player_level) + 40)) == 1
