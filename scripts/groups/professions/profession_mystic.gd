# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProfessionMystic
extends Profession


func _init() -> void:
	# Override our base class's variables
	ability_bonus = [2, -1, 2, 2, 2, 1]
	attacks_per_turn_max = 4
	attacks_per_turn_min_weapon_weight = 40
	attacks_per_turn_weight_multiplier = 3
	base_device_bonus = 30
	base_disarm_bonus = 45
	base_melee_attack_bonus = 64
	base_ranged_attack_bonus = 50
	base_save_bonus = 28
	base_search_bonus = 32
	base_search_frequency = 24
	base_stealth_bonus = 5
	casting_type = CastingType.MENTALISM
	description_line_2 = "Mystics master both martial and psionic arts, which they"
	description_line_3 = "power using WIS. Can resist confusion (at lvl 10), fear"
	description_line_4 = "(lvl 25), paralysis (lvl 30). Telepathy (lvl 40). While"
	description_line_5 = "wearing only light armour they can move faster and dodge,"
	description_line_6 = "and while not wielding a weapon they do increased damage."
	device_bonus_per_level = 11
	disarm_bonus_per_level = 14
	experience_factor = 40
	first_realm_choice = Realm.NONE
	hit_die_bonus = 6
	is_martial_artist = true
	legendary_item_bias = LegendaryItemBias.PRIESTLY
	melee_attack_bonus_per_level = 40
	pet_upkeep_divider = 7
	prime_ability_score = Ability.WISDOM
	ranged_attack_bonus_per_level = 30
	save_bonus_per_level = 11
	search_bonus_per_level = 0
	search_frequency_per_level = 0
	second_realm_choice = Realm.NONE
	spell_weight_limit = 300
	stealth_bonus_per_level = 0
	title = "Mystic"


func get_starting_items() -> Array[ItemIdentifier]:
	return [
		ItemIdentifier.new(ItemCategory.RING, RingType.SUSTAIN_WIS),
		ItemIdentifier.new(ItemCategory.POTION, PotionType.HEALING),
		ItemIdentifier.new(ItemCategory.SOFT_ARMOUR, SoftArmourType.SOFT_LEATHER),
	]


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	if player.level > 9:
		f2.set_bits(ItemFlags2.RES_CONF)
	if player.level > 24:
		f2.set_bits(ItemFlags2.RES_FEAR)
	if player.level > 39:
		f3.set_bits(ItemFlags3.TELEPATHY)
	if not martial_artist_heavy_armour(player):
		if player.level > 9:
			f1.set_bits(ItemFlags1.SPEED)
		if player.level > 29:
			f2.set_bits(ItemFlags2.FREE_ACT)


func has_speed_bonus(player: Player) -> bool:
	return martial_artist_heavy_armour(player)


func apply_martial_artist_armour_class(player: Player) -> void:
	if martial_artist_heavy_armour(player):
		return
	if player.inventory.items[InventorySlot.BODY].item_type == null:
		player.armour_class_bonus += player.level * 3 / 2
		player.displayed_armour_class_bonus += player.level * 3 / 2
	if player.inventory.items[InventorySlot.CLOAK].item_type == null and player.level > 15:
		player.armour_class_bonus += (player.level - 13) / 3
		player.displayed_armour_class_bonus += (player.level - 13) / 3
	if player.inventory.items[InventorySlot.ARM].item_type == null and player.level > 10:
		player.armour_class_bonus += (player.level - 8) / 3
		player.displayed_armour_class_bonus += (player.level - 8) / 3
	if player.inventory.items[InventorySlot.HEAD].item_type == null and player.level > 4:
		player.armour_class_bonus += (player.level - 2) / 3
		player.displayed_armour_class_bonus += (player.level - 2) / 3
	if player.inventory.items[InventorySlot.HANDS].item_type == null:
		player.armour_class_bonus += player.level / 2
		player.displayed_armour_class_bonus += player.level / 2
	if player.inventory.items[InventorySlot.FEET].item_type == null:
		player.armour_class_bonus += player.level / 3
		player.displayed_armour_class_bonus += player.level / 3


func martial_artist_heavy_armour(player: Player) -> bool:
	var weight: int = 0
	weight += player.inventory.items[InventorySlot.BODY].weight
	weight += player.inventory.items[InventorySlot.HEAD].weight
	weight += player.inventory.items[InventorySlot.ARM].weight
	weight += player.inventory.items[InventorySlot.CLOAK].weight
	weight += player.inventory.items[InventorySlot.HANDS].weight
	weight += player.inventory.items[InventorySlot.FEET].weight
	return (weight > 100 + (player.level * 4))


func apply_statuses_from_profession(player: Player) -> void:
	if player.level > 9:
		player.has_confusion_resistance = true
	if player.level > 24:
		player.has_fear_resistance = true
	if player.level > 39:
		player.has_telepathy = true
	if player.level > 24 and not martial_artist_heavy_armour(player):
		player.has_free_action = true


func try_sense_inventory(player_level: int) -> bool:
	return Rng.roll_die(20000 / ((player_level * player_level) + 40)) == 1
