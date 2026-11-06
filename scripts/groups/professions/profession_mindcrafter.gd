# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionMindcrafter
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [-1, 0, 3, -1, -1, 2]
	attacks_per_turn_max = 5
	attacks_per_turn_min_weapon_weight = 35
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 30
	base_disarm_bonus = 30
	base_melee_attack_bonus = 50
	base_ranged_attack_bonus = 40
	base_save_bonus = 30
	base_search_bonus = 22
	base_search_frequency = 16
	base_stealth_bonus = 3
	casting_type = CastingType.MENTALISM
	description_line_2 = "Disciples of the psionic arts, Mindcrafters learn a range"
	description_line_3 = "of mental abilities; which they power using WIS. As well"
	description_line_4 = "as their powers, they learn to resist fear (at lvl 10),"
	description_line_5 = "prevent wis drain (at lvl 20), resist confusion"
	description_line_6 = "(at lvl 30), and gain telepathy (at lvl 40)."
	device_bonus_per_level = 10
	disarm_bonus_per_level = 10
	experience_factor = 25
	first_realm_choice = Realm.NONE
	hit_die_bonus = 2
	legendary_item_bias = LegendaryItemBias.PRIESTLY
	melee_attack_bonus_per_level = 20
	pet_upkeep_divider = 7
	prime_ability_score = Ability.WISDOM
	ranged_attack_bonus_per_level = 30
	save_bonus_per_level = 10
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 350
	stealth_bonus_per_level = 0
	title = "Mindcrafter"
	warrior_artifact_bias = 60


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.SWORD, SwordType.SMALL_SWORD),
		ItemIdentifier.new(ItemCategory.POTION, PotionType.RESTORE_POWER),
		ItemIdentifier.new(ItemCategory.SOFT_ARMOUR, SoftArmourType.SOFT_LEATHER),
	]


func apply_statuses_from_profession(player: Player) -> void:
	if player.level > 9:
		player.has_fear_resistance = true
	if player.level > 19:
		player.has_sustain_wisdom = true
	if player.level > 29:
		player.has_resist_confusion = true
	if player.level > 39:
		player.has_telepathy = true


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	if player.level > 9:
		f2.set_bits(ItemFlags2.RES_FEAR)
	if player.level > 19:
		f2.set_bits(ItemFlags2.SUST_WIS)
	if player.level > 29:
		f2.set_bits(ItemFlags2.RES_CONF)
	if player.level > 39:
		f3.set_bits(ItemFlags3.TELEPATHY)


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(55000 / (player_level * player_level + 40)) == 1
