# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Item
extends Resource

const MAX_STACK_SIZE: int = 100

static var forced_coin_type: int

## The index of the artifact we are, if we are an artifact
@export var artifact_index: int
## The item's base armour class when worn
@export var base_armour_class: int
## The bonus the item gives to armour class when worn
@export var bonus_armour_class: int
@export var bonus_damage: int
## The of bonus power we have if we are a legendary item
@export var bonus_power_type: int
@export var bonus_power_sub_type: int
@export var bonus_to_hit: int
@export var category: int
@export var count: int
@export var damage_dice: int
@export var damage_dice_sides: int
@export var discount: int
## How much the player knows about us
@export var identify_flags: FlagSet
@export var inscription: String
## The type of item we are
@export var item_type: ItemType
## The flags holding our basic abilities if we're a legendary item
@export var legendary_flags_1: FlagSet
@export var legendary_flags_2: FlagSet
@export var legendary_flags_3: FlagSet
## Our name if we're a legendary item
@export var legendary_name: String
@export var next_in_stack: int
@export var marked: bool
## The index of the rare item we are, if we are a rare item
@export var rare_index: int
@export var recharge_time_left: int
@export var sub_category: int
## The value of our bonus
@export var type_specific_value: int
@export var weight: int
@export var holding_monster_index: int
@export var map_row: int
@export var map_col: int


func _init() -> void:
	identify_flags = FlagSet.new()
	legendary_flags_1 = FlagSet.new()
	legendary_flags_2 = FlagSet.new()
	legendary_flags_3 = FlagSet.new()


func is_legendary() -> bool:
	return (legendary_name != "")


## Fill in the supplied flagsets from the item's abilities regardless of where those abilities are from
func get_merged_flags(f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	# Clear the flagsets before we start
	f_1.clear_all()
	f_2.clear_all()
	f_3.clear_all()
	# If we don't have a type, we have no abilities
	if item_type == null:
		return
	# First of all, simply set the flags based on our type's flags
	f_1.set_flags(item_type.flags_1)
	f_2.set_flags(item_type.flags_2)
	f_3.set_flags(item_type.flags_3)
	# If we're an artifact flag, also set flags based on our artifact type
	if artifact_index != ArtifactId.NONE:
		var artifact: Artifact = SaveGame.instance.artifacts[artifact_index]
		f_1.set_flags(artifact.flags_1)
		f_2.set_flags(artifact.flags_2)
		f_3.set_flags(artifact.flags_3)
	# If we're a rare item, also set flags base on our rare item type
	if rare_index != RareItemId.NONE:
		var rare_type: RareItemType = StaticData.rare_item_types[rare_index]
		f_1.set_flags(rare_type.flags_1)
		f_2.set_flags(rare_type.flags_2)
		f_3.set_flags(rare_type.flags_3)
	# Add any legendary item flags
	f_1.set_flags(legendary_flags_1)
	f_2.set_flags(legendary_flags_2)
	f_3.set_flags(legendary_flags_3)
	# If we're legendary then we might have a special type
	if is_legendary():
		match bonus_power_type:
			RareItemId.SPECIAL_SUSTAIN:
				match bonus_power_sub_type % 6:
					0:
						f_2.set_bits(ItemFlags2.SUST_STR)
					1:
						f_2.set_bits(ItemFlags2.SUST_INT)
					2:
						f_2.set_bits(ItemFlags2.SUST_WIS)
					3:
						f_2.set_bits(ItemFlags2.SUST_DEX)
					4:
						f_2.set_bits(ItemFlags2.SUST_CON)
					5:
						f_2.set_bits(ItemFlags2.SUST_CHA)
			RareItemId.SPECIAL_POWER:
				match bonus_power_sub_type % 11:
					0:
						f_2.set_bits(ItemFlags2.RES_BLIND)
					1:
						f_2.set_bits(ItemFlags2.RES_CONF)
					2:
						f_2.set_bits(ItemFlags2.RES_SOUND)
					3:
						f_2.set_bits(ItemFlags2.RES_SHARDS)
					4:
						f_2.set_bits(ItemFlags2.RES_NETHER)
					5:
						f_2.set_bits(ItemFlags2.RES_NEXUS)
					6:
						f_2.set_bits(ItemFlags2.RES_CHAOS)
					7:
						f_2.set_bits(ItemFlags2.RES_DISEN)
					8:
						f_2.set_bits(ItemFlags2.RES_POIS)
					9:
						f_2.set_bits(ItemFlags2.RES_DARK)
					10:
						f_2.set_bits(ItemFlags2.RES_LIGHT)
			RareItemId.SPECIAL_ABILITY:
				match bonus_power_sub_type % 8:
					0:
						f_3.set_bits(ItemFlags3.FEATHER)
					1:
						f_3.set_bits(ItemFlags3.LIGHT_SOURCE)
					2:
						f_3.set_bits(ItemFlags3.SEE_INVIS)
					3:
						f_3.set_bits(ItemFlags3.TELEPATHY)
					4:
						f_3.set_bits(ItemFlags3.SLOW_DIGEST)
					5:
						f_3.set_bits(ItemFlags3.REGEN)
					6:
						f_2.set_bits(ItemFlags2.FREE_ACT)
					7:
						f_2.set_bits(ItemFlags2.HOLD_LIFE)


func get_known_flags(f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	f_1.clear_all()
	f_2.clear_all()
	f_3.clear_all()
	if is_known():
		get_merged_flags(f_1, f_2, f_3)


func is_known() -> bool:
	if item_type == null:
		return false
	if identify_flags.is_set(IdentifyFlags.KNOWN):
		return true
	if item_type.easy_know and item_type.flavour_aware:
		return true
	return false


func apply_random_resistance(specific: int)->void:
	var item_factory: ItemFactory = ItemFactory.new(self)
	item_factory.apply_random_resistance(specific)

func become_flavour_aware() -> void:
	item_type.flavour_aware = true


func assign_item_type(new_item_type: ItemType) -> void:
	item_type = new_item_type
	category = item_type.item_category
	sub_category = item_type.sub_category
	type_specific_value = item_type.type_specific_value
	count = 1
	weight = item_type.weight
	bonus_to_hit = item_type.to_hit
	bonus_damage = item_type.to_damage
	bonus_armour_class = item_type.to_ac
	base_armour_class = item_type.ac
	damage_dice = item_type.damage_dice
	damage_dice_sides = item_type.damage_dice_sides
	if item_type.cost <= 0:
		identify_flags.set_bits(IdentifyFlags.BROKEN)
	if item_type.flags_3.is_set(ItemFlags3.CURSED):
		identify_flags.set_bits(IdentifyFlags.CURSED)


func apply_magic(level: int, okay: bool, good: bool, great: bool) -> void:
	var factory: ItemFactory = ItemFactory.new(self)
	factory.apply_magic(level, okay, good, great)


func become_known() -> void:
	if inscription != "" and identify_flags.is_set(IdentifyFlags.SENSED):
		var q: String = inscription
		if (q == "cursed" or q == "broken" or q == "good" or q == "average" or q == "excellent" or
			q == "worthless" or q == "special" or q == "terrible" ):
			inscription = ""
	identify_flags.clear_bits(IdentifyFlags.SENSED | IdentifyFlags.EMPTY)
	identify_flags.set_bits(IdentifyFlags.KNOWN)


func value() -> int:
	var v: int
	if is_known():
		if is_broken():
			return 0
		if is_cursed():
			return 0
		v = real_value()
	else:
		if identify_flags.is_set(IdentifyFlags.SENSED) and is_broken():
			return 0
		if identify_flags.is_set(IdentifyFlags.SENSED) and is_cursed():
			return 0
		v = base_value()
	if discount != 0:
		v -= v * discount / 100
	return v


func is_broken() -> bool:
	return identify_flags.is_set(IdentifyFlags.BROKEN)


func is_cursed() -> bool:
	return identify_flags.is_set(IdentifyFlags.CURSED)


func is_artifact() -> bool:
	return (artifact_index != 0)


func is_tried() -> bool:
	return item_type.tried


func is_flavour_aware() -> bool:
	if item_type == null:
		return false
	return item_type.flavour_aware


func base_value() -> int:
	var type: ItemType = item_type
	if is_flavour_aware():
		return type.cost
	match category:
		ItemCategory.FOOD:
			return 5
		ItemCategory.POTION:
			return 20
		ItemCategory.SCROLL:
			return 20
		ItemCategory.STAFF:
			return 70
		ItemCategory.WAND:
			return 50
		ItemCategory.ROD:
			return 90
		ItemCategory.RING:
			return 45
		ItemCategory.AMULET:
			return 45
	return 0


func real_value() -> int:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	var type: ItemType = item_type
	if type.cost == 0:
		return 0
	var v: int = type.cost
	get_merged_flags(f_1, f_2, f_3)
	if legendary_flags_1.is_any_set() or legendary_flags_2.is_any_set() or legendary_flags_3.is_any_set():
		v += flag_based_cost(type_specific_value)
	elif artifact_index != 0:
		var artifact: Artifact = SaveGame.instance.artifacts[artifact_index]
		if artifact.cost == 0:
			return 0
		v = artifact.cost
	elif rare_index != RareItemId.NONE:
		var rare_type: RareItemType = StaticData.rare_item_types[rare_index]
		if rare_type.cost == 0:
			return 0
		v += rare_type.cost
	match category:
		ItemCategory.SHOT, ItemCategory.ARROW, ItemCategory.BOLT, ItemCategory.BOW, \
		ItemCategory.DIGGING, ItemCategory.HAFTED, ItemCategory.POLEARM, ItemCategory.SWORD, \
		ItemCategory.BOOTS, ItemCategory.GLOVES, ItemCategory.HELM, ItemCategory.CROWN, \
		ItemCategory.SHIELD, ItemCategory.CLOAK, ItemCategory.SOFT_ARMOUR, ItemCategory.HARD_ARMOUR, \
		ItemCategory.DRAGON_ARMOUR, ItemCategory.LIGHT, ItemCategory.AMULET, ItemCategory.RING:
			if type_specific_value < 0:
				return 0
			if type_specific_value > 0:
				if f_1.is_set(ItemFlags1.STR):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.INT):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.WIS):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.DEX):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.CON):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.CHA):
					v += type_specific_value * 200
				if f_1.is_set(ItemFlags1.STEALTH):
					v += type_specific_value * 100
				if f_1.is_set(ItemFlags1.SEARCH):
					v += type_specific_value * 100
				if f_1.is_set(ItemFlags1.INFRA):
					v += type_specific_value * 50
				if f_1.is_set(ItemFlags1.TUNNEL):
					v += type_specific_value * 50
				if f_1.is_set(ItemFlags1.BLOWS):
					v += type_specific_value * 5000
				if f_1.is_set(ItemFlags1.SPEED):
					v += type_specific_value * 3000
	match category:
		ItemCategory.WAND, ItemCategory.STAFF:
			v += v / 20 * type_specific_value
		ItemCategory.RING, ItemCategory.AMULET:
			if bonus_armour_class < 0:
				return 0
			if bonus_to_hit < 0:
				return 0
			if bonus_damage < 0:
				return 0
			v += (bonus_to_hit + bonus_damage + bonus_armour_class) * 100
		ItemCategory.BOOTS, ItemCategory.GLOVES, ItemCategory.CLOAK, ItemCategory.CROWN, \
		ItemCategory.HELM, ItemCategory.SHIELD, ItemCategory.SOFT_ARMOUR, ItemCategory.HARD_ARMOUR, \
		ItemCategory.DRAGON_ARMOUR:
			if bonus_armour_class < 0:
				return 0
			v += (bonus_to_hit + bonus_damage + bonus_armour_class) * 100
		ItemCategory.BOW, ItemCategory.DIGGING, ItemCategory.HAFTED, ItemCategory.SWORD, ItemCategory.POLEARM:
			if bonus_to_hit + bonus_damage < 0:
				return 0
			v += (bonus_to_hit + bonus_damage + bonus_armour_class) * 100
			if damage_dice > item_type.damage_dice and damage_dice_sides == item_type.damage_dice_sides:
				v += (damage_dice - item_type.damage_dice) * damage_dice_sides * 100
		ItemCategory.SHOT, ItemCategory.ARROW, ItemCategory.BOLT:
			if bonus_to_hit + bonus_damage < 0:
				return 0
			v += (bonus_to_hit + bonus_damage) * 5
			if damage_dice > item_type.damage_dice and damage_dice_sides == item_type.damage_dice_sides:
				v += (damage_dice - item_type.damage_dice) * damage_dice_sides * 5
	return v


func flag_based_cost(plusses: int) -> int:
	var total: int = 0
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	get_merged_flags(f_1, f_2, f_3)
	if f_1.is_set(ItemFlags1.STR):
		total += 1000 * plusses
	if f_1.is_set(ItemFlags1.INT):
		total += 1000 * plusses
	if f_1.is_set(ItemFlags1.WIS):
		total += 1000 * plusses
	if f_1.is_set(ItemFlags1.DEX):
		total += 1000 * plusses
	if f_1.is_set(ItemFlags1.CON):
		total += 1000 * plusses
	if f_1.is_set(ItemFlags1.CHA):
		total += 250 * plusses
	#if f_1.is_set(ItemFlags1.UNUSED_1):
	#total += 0
	#if f_1.is_set(ItemFlags1.UNUSED_2):
	#total += 0
	if f_1.is_set(ItemFlags1.STEALTH):
		total += 250 * plusses
	if f_1.is_set(ItemFlags1.SEARCH):
		total += 100 * plusses
	if f_1.is_set(ItemFlags1.INFRA):
		total += 150 * plusses
	if f_1.is_set(ItemFlags1.TUNNEL):
		total += 175 * plusses
	if f_1.is_set(ItemFlags1.SPEED) and plusses > 0:
		total += 30000 * plusses
	if f_1.is_set(ItemFlags1.BLOWS) and plusses > 0:
		total += 2000 * plusses
	if f_1.is_set(ItemFlags1.CHAOTIC):
		total += 10000
	if f_1.is_set(ItemFlags1.VAMPIRIC):
		total += 13000
	if f_1.is_set(ItemFlags1.SLAY_ANIMAL):
		total += 3500
	if f_1.is_set(ItemFlags1.SLAY_EVIL):
		total += 4500
	if f_1.is_set(ItemFlags1.SLAY_UNDEAD):
		total += 3500
	if f_1.is_set(ItemFlags1.SLAY_DEMON):
		total += 3500
	if f_1.is_set(ItemFlags1.SLAY_ORC):
		total += 3000
	if f_1.is_set(ItemFlags1.SLAY_TROLL):
		total += 3500
	if f_1.is_set(ItemFlags1.SLAY_GIANT):
		total += 3500
	if f_1.is_set(ItemFlags1.SLAY_DRAGON):
		total += 3500
	if f_1.is_set(ItemFlags1.KILL_DRAGON):
		total += 5500
	if f_1.is_set(ItemFlags1.VORPAL):
		total += 5000
	if f_1.is_set(ItemFlags1.IMPACT):
		total += 5000
	if f_1.is_set(ItemFlags1.BRAND_POISON):
		total += 7500
	if f_1.is_set(ItemFlags1.BRAND_ACID):
		total += 7500
	if f_1.is_set(ItemFlags1.BRAND_ELECTRIC):
		total += 7500
	if f_1.is_set(ItemFlags1.BRAND_FIRE):
		total += 5000
	if f_1.is_set(ItemFlags1.BRAND_COLD):
		total += 5000
	if f_2.is_set(ItemFlags2.SUST_STR):
		total += 850
	if f_2.is_set(ItemFlags2.SUST_INT):
		total += 850
	if f_2.is_set(ItemFlags2.SUST_WIS):
		total += 850
	if f_2.is_set(ItemFlags2.SUST_DEX):
		total += 850
	if f_2.is_set(ItemFlags2.SUST_CON):
		total += 850
	if f_2.is_set(ItemFlags2.SUST_CHA):
		total += 250
	#if f_2.is_set(ItemFlags2.UNUSED_1):
	#total += 0
	#if f_2.is_set(ItemFlags2.UNUSED_2):
	#total += 0
	if f_2.is_set(ItemFlags2.IM_ACID):
		total += 10000
	if f_2.is_set(ItemFlags2.IM_ELEC):
		total += 10000
	if f_2.is_set(ItemFlags2.IM_FIRE):
		total += 10000
	if f_2.is_set(ItemFlags2.IM_COLD):
		total += 10000
	#if f_2.is_set(ItemFlags2.UNUSED_3):
	#total += 0
	if f_2.is_set(ItemFlags2.REFLECT):
		total += 10000
	if f_2.is_set(ItemFlags2.FREE_ACT):
		total += 4500
	if f_2.is_set(ItemFlags2.HOLD_LIFE):
		total += 8500
	if f_2.is_set(ItemFlags2.RES_ACID):
		total += 1250
	if f_2.is_set(ItemFlags2.RES_ELEC):
		total += 1250
	if f_2.is_set(ItemFlags2.RES_FIRE):
		total += 1250
	if f_2.is_set(ItemFlags2.RES_COLD):
		total += 1250
	if f_2.is_set(ItemFlags2.RES_POIS):
		total += 2500
	if f_2.is_set(ItemFlags2.RES_FEAR):
		total += 2500
	if f_2.is_set(ItemFlags2.RES_LIGHT):
		total += 1750
	if f_2.is_set(ItemFlags2.RES_DARK):
		total += 1750
	if f_2.is_set(ItemFlags2.RES_BLIND):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_CONF):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_SOUND):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_SHARDS):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_NETHER):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_NEXUS):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_CHAOS):
		total += 2000
	if f_2.is_set(ItemFlags2.RES_DISEN):
		total += 10000
	if f_3.is_set(ItemFlags3.SH_FIRE):
		total += 5000
	if f_3.is_set(ItemFlags3.SH_ELEC):
		total += 5000
	#if f_3.is_set(ItemFlags3.UNUSED_1):
	#total += 0
	if f_3.is_set(ItemFlags3.ANTI_THEFT):
		total += 1000
	if f_3.is_set(ItemFlags3.NO_TELE):
		total += 2500
	if f_3.is_set(ItemFlags3.NO_MAGIC):
		total += 2500
	if f_3.is_set(ItemFlags3.WRAITH):
		total += 250000
	if f_3.is_set(ItemFlags3.DREAD_CURSE):
		total -= 15000
	if f_3.is_set(ItemFlags3.EASY_KNOW):
		total += 0
	if f_3.is_set(ItemFlags3.HIDE_TYPE):
		total += 0
	if f_3.is_set(ItemFlags3.SHOW_MODS):
		total += 0
	if f_3.is_set(ItemFlags3.INSTA_ART):
		total += 0
	if f_3.is_set(ItemFlags3.FEATHER):
		total += 1250
	if f_3.is_set(ItemFlags3.LIGHT_SOURCE):
		total += 1250
	if f_3.is_set(ItemFlags3.SEE_INVIS):
		total += 2000
	if f_3.is_set(ItemFlags3.TELEPATHY):
		total += 12500
	if f_3.is_set(ItemFlags3.SLOW_DIGEST):
		total += 750
	if f_3.is_set(ItemFlags3.REGEN):
		total += 2500
	if f_3.is_set(ItemFlags3.XTRA_MIGHT):
		total += 2250
	if f_3.is_set(ItemFlags3.XTRA_SHOTS):
		total += 10000
	if f_3.is_set(ItemFlags3.IGNORE_ACID):
		total += 100
	if f_3.is_set(ItemFlags3.IGNORE_ELEC):
		total += 100
	if f_3.is_set(ItemFlags3.IGNORE_FIRE):
		total += 100
	if f_3.is_set(ItemFlags3.IGNORE_COLD):
		total += 100
	if f_3.is_set(ItemFlags3.ACTIVATE):
		total += 100
	if f_3.is_set(ItemFlags3.DRAIN_EXP):
		total -= 12500
	if f_3.is_set(ItemFlags3.TELEPORT):
		if identify_flags.is_set(IdentifyFlags.CURSED):
			total -= 7500
		else:
			total += 250
	if f_3.is_set(ItemFlags3.AGGRAVATE):
		total -= 10000
	if f_3.is_set(ItemFlags3.BLESSED):
		total += 750
	if f_3.is_set(ItemFlags3.CURSED):
		total -= 5000
	if f_3.is_set(ItemFlags3.HEAVY_CURSE):
		total -= 12500
	if f_3.is_set(ItemFlags3.PERMA_CURSE):
		total -= 15000
	if is_legendary() and legendary_flags_3.is_set(ItemFlags3.ACTIVATE):
		var type: int = bonus_power_sub_type
		if type == LegendaryPower.SUNLIGHT:
			total += 250
		elif type == LegendaryPower.BO_MISS_1:
			total += 250
		elif type == LegendaryPower.BA_POIS_1:
			total += 300
		elif type == LegendaryPower.BO_ELEC_1:
			total += 250
		elif type == LegendaryPower.BO_ACID_1:
			total += 250
		elif type == LegendaryPower.BO_COLD_1:
			total += 250
		elif type == LegendaryPower.BO_FIRE_1:
			total += 250
		elif type == LegendaryPower.BA_COLD_1:
			total += 750
		elif type == LegendaryPower.BA_FIRE_1:
			total += 1000
		elif type == LegendaryPower.DRAIN_1:
			total += 500
		elif type == LegendaryPower.BA_COLD_2:
			total += 1250
		elif type == LegendaryPower.BA_ELEC_2:
			total += 1500
		elif type == LegendaryPower.BA_ELEC_3:
			total += 2000
		elif type == LegendaryPower.DRAIN_2:
			total += 750
		elif type == LegendaryPower.VAMPIRE_1:
			total = 1000
		elif type == LegendaryPower.BO_MISS_2:
			total += 1000
		elif type == LegendaryPower.BA_FIRE_2:
			total += 1750
		elif type == LegendaryPower.BA_COLD_3:
			total += 2500
		elif type == LegendaryPower.BA_ELEC_3:
			total += 2500
		elif type == LegendaryPower.WHIRLWIND:
			total += 7500
		elif type == LegendaryPower.VAMPIRE_2:
			total += 2500
		elif type == LegendaryPower.CALL_CHAOS:
			total += 5000
		elif type == LegendaryPower.SHARD:
			total += 5000
		elif type == LegendaryPower.DISP_EVIL:
			total += 4000
		elif type == LegendaryPower.DISP_GOOD:
			total += 3500
		elif type == LegendaryPower.BA_MISS_3:
			total += 5000
		elif type == LegendaryPower.CONFUSE:
			total += 500
		elif type == LegendaryPower.SLEEP:
			total += 750
		elif type == LegendaryPower.QUAKE:
			total += 600
		elif type == LegendaryPower.TERROR:
			total += 2500
		elif type == LegendaryPower.TELE_AWAY:
			total += 2000
		elif type == LegendaryPower.BANISH_EVIL:
			total += 3000
		elif type == LegendaryPower.CARNAGE:
			total += 10000
		elif type == LegendaryPower.MASS_GENO:
			total += 10000
		elif type == LegendaryPower.CHARM_ANIMAL:
			total += 7500
		elif type == LegendaryPower.CHARM_UNDEAD:
			total += 10000
		elif type == LegendaryPower.CHARM_OTHER:
			total += 10000
		elif type == LegendaryPower.CHARM_ANIMALS:
			total += 12500
		elif type == LegendaryPower.CHARM_OTHERS:
			total += 17500
		elif type == LegendaryPower.SUMMON_ANIMAL:
			total += 10000
		elif type == LegendaryPower.SUMMON_PHANTOM:
			total += 12000
		elif type == LegendaryPower.SUMMON_ELEMENTAL:
			total += 15000
		elif type == LegendaryPower.SUMMON_DEMON:
			total += 20000
		elif type == LegendaryPower.SUMMON_UNDEAD:
			total += 20000
		elif type == LegendaryPower.CURE_L_W:
			total += 500
		elif type == LegendaryPower.CURE_M_W:
			total += 750
		elif type == LegendaryPower.CURE_POISON:
			total += 1000
		elif type == LegendaryPower.REST_LIFE:
			total += 7500
		elif type == LegendaryPower.REST_ALL:
			total += 15000
		elif type == LegendaryPower.CURE_700:
			total += 10000
		elif type == LegendaryPower.CURE_1000:
			total += 15000
		elif type == LegendaryPower.ESP:
			total += 1500
		elif type == LegendaryPower.BERSERK:
			total += 800
		elif type == LegendaryPower.PROT_EVIL:
			total += 5000
		elif type == LegendaryPower.RESIST_ALL:
			total += 5000
		elif type == LegendaryPower.SPEED:
			total += 15000
		elif type == LegendaryPower.XTRA_SPEED:
			total += 25000
		elif type == LegendaryPower.WRAITH:
			total += 25000
		elif type == LegendaryPower.INVULN:
			total += 25000
		elif type == LegendaryPower.LIGHT:
			total += 150
		elif type == LegendaryPower.MAP_LIGHT:
			total += 500
		elif type == LegendaryPower.DETECT_ALL:
			total += 1000
		elif type == LegendaryPower.DETECT_XTRA:
			total += 12500
		elif type == LegendaryPower.ID_FULL:
			total += 10000
		elif type == LegendaryPower.ID_PLAIN:
			total += 1250
		elif type == LegendaryPower.RUNE_EXPLO:
			total += 4000
		elif type == LegendaryPower.RUNE_PROT:
			total += 10000
		elif type == LegendaryPower.SATIATE:
			total += 2000
		elif type == LegendaryPower.DEST_DOOR:
			total += 100
		elif type == LegendaryPower.STONE_MUD:
			total += 1000
		elif type == LegendaryPower.RECHARGE:
			total += 1000
		elif type == LegendaryPower.ALCHEMY:
			total += 10000
		elif type == LegendaryPower.DIM_DOOR:
			total += 10000
		elif type == LegendaryPower.TELEPORT:
			total += 2000
		elif type == LegendaryPower.RECALL:
			total += 7500
	return total


func get_copy() -> Item:
	var copy: Item = Item.new()
	copy.base_armour_class = base_armour_class
	copy.legendary_flags_1.copy(legendary_flags_1)
	copy.legendary_flags_2.copy(legendary_flags_2)
	copy.legendary_flags_3.copy(legendary_flags_3)
	copy.legendary_name = legendary_name
	copy.damage_dice = damage_dice
	copy.discount = discount
	copy.damage_dice_sides = damage_dice_sides
	copy.holding_monster_index = holding_monster_index
	copy.identify_flags.copy(identify_flags)
	copy.map_row = map_row
	copy.map_col = map_col
	copy.item_type = item_type
	copy.marked = marked
	copy.artifact_index = artifact_index
	copy.rare_index = rare_index
	copy.next_in_stack = next_in_stack
	copy.inscription = inscription
	copy.count = count
	copy.type_specific_value = type_specific_value
	copy.sub_category = sub_category
	copy.recharge_time_left = recharge_time_left
	copy.bonus_armour_class = bonus_armour_class
	copy.bonus_damage = bonus_damage
	copy.bonus_to_hit = bonus_to_hit
	copy.category = category
	copy.weight = weight
	copy.bonus_power_type = bonus_power_type
	copy.bonus_power_sub_type = bonus_power_sub_type
	return copy


func can_absorb(other: Item) -> bool:
	var total: int = count + other.count
	if item_type != other.item_type:
		return false
	match category:
		ItemCategory.CHEST:
			return false
		ItemCategory.FOOD, ItemCategory.POTION, ItemCategory.SCROLL:
			pass
		ItemCategory.STAFF, ItemCategory.WAND:
			if not is_known() or not other.is_known():
				return false
			if type_specific_value != other.type_specific_value:
				return false
		ItemCategory.ROD:
			if type_specific_value != other.type_specific_value:
				return false
		ItemCategory.BOW, ItemCategory.DIGGING, ItemCategory.HAFTED, ItemCategory.POLEARM, \
		ItemCategory.SWORD, ItemCategory.BOOTS, ItemCategory.GLOVES, ItemCategory.HELM, \
		ItemCategory.CROWN, ItemCategory.SHIELD, ItemCategory.CLOAK, ItemCategory.SOFT_ARMOUR, \
		ItemCategory.HARD_ARMOUR, ItemCategory.DRAGON_ARMOUR, ItemCategory.RING, ItemCategory.AMULET, \
		ItemCategory.LIGHT:
			if not is_known() or not other.is_known():
				return false
			if bonus_to_hit != other.bonus_to_hit:
				return false
			if bonus_damage != other.bonus_damage:
				return false
			if bonus_armour_class != other.bonus_armour_class:
				return false
			if type_specific_value != other.type_specific_value:
				return false
			if artifact_index != other.artifact_index:
				return false
			if is_legendary() or other.is_legendary():
				return false
			if rare_index != other.rare_index:
				return false
			if bonus_power_type != 0 or other.bonus_power_type != 0:
				return false
			if recharge_time_left != 0 or other.recharge_time_left != 0:
				return false
			if base_armour_class != other.base_armour_class:
				return false
			if damage_dice != other.damage_dice:
				return false
			if damage_dice_sides != other.damage_dice_sides:
				return false
		ItemCategory.BOLT, ItemCategory.ARROW, ItemCategory.SHOT:
			if not is_known() or not other.is_known():
				return false
			if bonus_to_hit != other.bonus_to_hit:
				return false
			if bonus_damage != other.bonus_damage:
				return false
			if bonus_armour_class != other.bonus_armour_class:
				return false
			if type_specific_value != other.type_specific_value:
				return false
			if artifact_index != other.artifact_index:
				return false
			if is_legendary() or other.is_legendary():
				return false
			if rare_index != other.rare_index:
				return false
			if bonus_power_type != 0 or other.bonus_power_type != 0:
				return false
			if recharge_time_left != 0 or other.recharge_time_left != 0:
				return false
			if base_armour_class != other.base_armour_class:
				return false
			if damage_dice != other.damage_dice:
				return false
			if damage_dice_sides != other.damage_dice_sides:
				return false
		_:
			if not is_known() or not other.is_known():
				return false
	if (legendary_flags_1.value != other.legendary_flags_1.value or
		legendary_flags_2.value != legendary_flags_2.value or
		legendary_flags_3.value != other.legendary_flags_3.value ):
		return false
	if not identify_flags.bits_match(other.identify_flags, IdentifyFlags.CURSED | IdentifyFlags.BROKEN):
		return false
	if inscription != other.inscription:
		return false
	if discount != other.discount:
		return false
	return total < MAX_STACK_SIZE


func make_item(good: bool, great: bool) -> bool:
	var factory: ItemFactory = ItemFactory.new(self)
	return factory.make_item(good, great)


func make_gold() -> bool:
	var money_type: int = ((Rng.roll_die(SaveGame.instance.dungeon_level.item_level + 2) + 2) / 2) - 1
	if Rng.roll_die(ItemType.GREAT_OBJECT_CHANCE) == 1:
		money_type += Rng.roll_die(SaveGame.instance.dungeon_level.item_level + 1)
	if forced_coin_type != 0:
		money_type = forced_coin_type
	if money_type > MoneyType.ADAMANTITE:
		money_type = MoneyType.ADAMANTITE
	var type: ItemType
	match money_type:
		0:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.COPPER_LOW)
		1:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.COPPER_MED)
		2:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.COPPER_HI)
		3:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.SILVER_LOW)
		4:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.SILVER_MED)
		5:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.SILVER_HI)
		6:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.GARNETS_LOW)
		7:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.GARNETS_HI)
		8:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.GOLD_LOW)
		9:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.GOLD_MED)
		10:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.GOLD_HIGH)
		11:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.OPALS)
		12:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.SAPPHIRES)
		13:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.RUBIES)
		14:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.DIAMONDS)
		15:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.EMERALDS)
		16:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.MITHRIL)
		17:
			type = SaveGame.instance.item_types.look_up_type(ItemCategory.GOLD, MoneyType.ADAMANTITE)
	if type == null:
		return false
	assign_item_type(type)
	var bbase: int = type.cost
	type_specific_value = bbase + (8 * Rng.roll_die(bbase)) + Rng.roll_die(8)
	return true


## Describe the item
## if use_article is true, the description will include an article (a, an, the)
## mode 0 = just the name ("a sword")
## mode 1 = include basics ("a sword [+1,+1] (3d6)"
## mode 2 = incude details ("a sword [+1,+1] (3d6) (+1 to speed)")
## mode 3 = include inscription ("a sword [+1,+1] (3d6) (+1 to speed) {25% off}")
func description(use_article: bool, mode: int) -> String:
	var aware: bool = false
	var known: bool = false
	var append_name: bool = false
	var show_weapon_mods: bool = false
	var show_armour_mods: bool = false
	var s: String
	var inscription_text: String = ""
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if item_type == null:
		return "(nothing)"
	get_merged_flags(f_1, f_2, f_3)
	if is_flavour_aware():
		aware = true
	if is_known():
		known = true
	var base_name: String = item_type.name
	var modifier_name: String = ""
	match category:
		ItemCategory.SKELETON, \
		ItemCategory.BOTTLE, \
		ItemCategory.JUNK, \
		ItemCategory.SPIKE, \
		ItemCategory.FLASK, \
		ItemCategory.CHEST:
			pass
		ItemCategory.SHOT, \
		ItemCategory.BOLT, \
		ItemCategory.ARROW, \
		ItemCategory.BOW, \
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.SWORD, \
		ItemCategory.DIGGING: \
			show_weapon_mods = true
		ItemCategory.BOOTS, \
		ItemCategory.GLOVES, \
		ItemCategory.CLOAK, \
		ItemCategory.CROWN, \
		ItemCategory.HELM, \
		ItemCategory.SHIELD, \
		ItemCategory.SOFT_ARMOUR, \
		ItemCategory.HARD_ARMOUR, \
		ItemCategory.DRAGON_ARMOUR:
			show_armour_mods = true
		ItemCategory.LIGHT:
			pass
		ItemCategory.AMULET:
			if is_artifact() and aware:
				pass
			modifier_name = SaveGame.instance.amulet_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Amulet~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Amulet~"
		ItemCategory.RING:
			if is_artifact() and aware:
				pass
			modifier_name = SaveGame.instance.ring_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Ring~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Ring~"
		ItemCategory.STAFF:
			modifier_name = SaveGame.instance.staff_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Staff~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Staff~"
		ItemCategory.WAND:
			modifier_name = SaveGame.instance.wand_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Wand~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Wand~"
		ItemCategory.ROD:
			modifier_name = SaveGame.instance.rod_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Rod~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Rod~"
		ItemCategory.SCROLL:
			modifier_name = SaveGame.instance.scroll_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Scroll~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& Scroll~ titled \"#\""
		ItemCategory.POTION:
			modifier_name = SaveGame.instance.potion_flavours[sub_category].name
			if aware:
				append_name = true
			base_name = "& Potion~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Potion~"
		ItemCategory.FOOD:
			if sub_category >= FoodType.MIN_FOOD:
				pass
			else:
				modifier_name = SaveGame.instance.mushroom_flavours[sub_category].name
				if aware:
					append_name = true
				base_name = "& Mushroom~" if identify_flags.is_set(IdentifyFlags.STOREBOUGHT) else "& # Mushroom~"
		ItemCategory.LIFE_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Life Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Life Spellbook~ #"
		ItemCategory.SORCERY_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Sorcery #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Sorcery Spellbook~ #"
		ItemCategory.NATURE_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Nature Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Nature Spellbook~ #"
		ItemCategory.CHAOS_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Chaos Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Chaos Spellbook~ #"
		ItemCategory.DEATH_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Death Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Death Spellbook~ #"
		ItemCategory.TAROT_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Tarot Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Tarot Spellbook~ #"
		ItemCategory.FOLK_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Folk Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Folk Spellbook~ #"
		ItemCategory.CORPOREAL_BOOK:
			modifier_name = base_name
			base_name = "& Book~ of Corporeal Magic #" if SaveGame.instance.player.spellcasting.casting_type == CastingType.DIVINE else "& Corporeal Spellbook~ #"
		ItemCategory.GOLD:
			return base_name
		_:
			return "(nothing)"
	var t: String = ""
	if base_name[0] == "&":
		s = base_name.substr(2)
		if not use_article:
			pass
		elif count <= 0:
			t += "no more "
		elif count > 1:
			t += str(count)
			t += " "
		elif known and (is_artifact() or is_legendary()):
			t += "The "
		elif s.begins_with("#") and Util.is_vowel(modifier_name[0]):
			t += "an "
		elif Util.is_vowel(s[0]):
			t += "an "
		else:
			t += "a "
	else:
		s = base_name
		if not use_article:
			pass
		elif count <= 0:
			t += "no more "
		elif count > 1:
			t += str(count)
			t += " "
		elif known and (is_artifact() or is_legendary()):
			t += "The "
	for ch: String in s:
		if ch == "~":
			if count != 1:
				var k: String = t[t.length() - 1]
				if k == "s" or k == "h":
					t += "e"
				t += "s"
		elif ch == "#":
			t += modifier_name
		else:
			t += ch
	if append_name:
		t += " of "
		t += item_type.name
	if known:
		if is_legendary():
			t += " '"
			t += legendary_name
			t += "'"
		elif artifact_index != 0:
			var artifact: Artifact = SaveGame.instance.artifacts[artifact_index]
			t += " "
			t += artifact.name
		elif rare_index != RareItemId.NONE:
			var rare_type: RareItemType = StaticData.rare_item_types[rare_index]
			t += " "
			t += rare_type.name
	if mode < 1:
		return t
	if category == ItemCategory.CHEST:
		if not known:
			pass
		elif type_specific_value == 0:
			t += " (empty)"
		elif type_specific_value < 0:
			if Tables.CHEST_TRAPS[-type_specific_value] != 0:
				t += " (disarmed)"
			else:
				t += " (unlocked)"
		else:
			match Tables.CHEST_TRAPS[type_specific_value]:
				0:
					t += " (Locked)"
				ChestTrap.LOSE_STR:
					t += " (Poison Needle)"
				ChestTrap.LOSE_CON:
					t += " (Poison Needle)"
				ChestTrap.POISON:
					t += " (Gas Trap)"
				ChestTrap.PARALYSE:
					t += " (Gas Trap)"
				ChestTrap.EXPLODE:
					t += " (Explosion Device)"
				ChestTrap.SUMMON:
					t += " (Summoning Runes)"
				_:
					t += " (Multiple Traps)"
	if f_3.is_set(ItemFlags3.SHOW_MODS):
		show_weapon_mods = true
	if bonus_to_hit != 0 and bonus_damage != 0:
		show_weapon_mods = true
	if base_armour_class != 0:
		show_armour_mods = true
	match category:
		ItemCategory.SHOT, \
		ItemCategory.BOLT, \
		ItemCategory.ARROW, \
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.SWORD, \
		ItemCategory.DIGGING:
			t += " ("
			t += str(damage_dice)
			t += "d"
			t += str(damage_dice_sides)
			t += ")"
		ItemCategory.BOW:
			var power: int = sub_category % 10
			if f_3.is_set(ItemFlags3.XTRA_MIGHT):
				power += 1
			t += " (x"
			t += str(power)
			t += ")"
	if known:
		if show_weapon_mods:
			t += " ("
			if bonus_to_hit >= 0:
				t += "+"
			t += str(bonus_to_hit)
			t += ","
			if bonus_damage >= 0:
				t += "+"
			t += str(bonus_damage)
			t += ")"
		elif bonus_to_hit != 0:
			t += " ("
			if bonus_to_hit >= 0:
				t += "+"
			t += str(bonus_to_hit)
			t += ")"
		elif bonus_damage != 0:
			t += ' '
			t += "("
			if bonus_damage >= 0:
				t += "+"
			t += str(bonus_damage)
			t += ")"
	if known:
		if show_armour_mods:
			t += " ["
			t += str(base_armour_class)
			t += ','
			if bonus_armour_class >= 0:
				t += "+"
			t += str(bonus_armour_class)
			t += "]"
		elif bonus_armour_class != 0:
			t += " ["
			if bonus_armour_class >= 0:
				t += "+"
			t += str(bonus_armour_class)
			t += "]"
	elif show_armour_mods:
		t += " ["
		t += str(base_armour_class)
		t += "]"
	if mode < 2:
		return t
	if known and (category == ItemCategory.STAFF or category == ItemCategory.WAND):
		t += " ("
		t += str(type_specific_value)
		t += " charge"
		if type_specific_value != 1:
			t += "s"
		t += ")"
	elif known and category == ItemCategory.ROD:
		if type_specific_value != 0:
			t += " (charging)"
	elif category == ItemCategory.LIGHT and (sub_category == LightType.TORCH or sub_category == LightType.LANTERN):
		if type_specific_value == 1:
			t += " (with 1 turn of light)"
		else:
			t += " (with "
			t += str(type_specific_value)
			t += " turns of light)"
	if known and f_1.is_set(ItemFlags1.PvalMask):
		t += " ("
		if type_specific_value >= 0:
			t += "+"
		t += str(type_specific_value)
		if f_3.is_set(ItemFlags3.HIDE_TYPE):
			pass
		elif f_1.is_set(ItemFlags1.SPEED):
			t += " speed"
		elif f_1.is_set(ItemFlags1.BLOWS):
			if type_specific_value > 1:
				t += " attacks"
			else:
				t += " attack"
		elif f_1.is_set(ItemFlags1.STEALTH):
			t += " stealth"
		elif f_1.is_set(ItemFlags1.SEARCH):
			t += " searching"
		elif f_1.is_set(ItemFlags1.INFRA):
			t += " infravision"
		elif f_1.is_set(ItemFlags1.TUNNEL):
			pass
		t += ")"
	if known and recharge_time_left != 0:
		t += " (charging)"
	if mode < 3:
		return t
	if inscription != "":
		inscription_text = inscription
	elif is_cursed() and (known or identify_flags.is_set(IdentifyFlags.SENSED)):
		inscription_text = "cursed"
	elif not known and identify_flags.is_set(IdentifyFlags.EMPTY):
		inscription_text = "empty"
	elif not aware and is_tried():
		inscription_text = "tried"
	elif discount != 0:
		inscription_text = str(discount)
		inscription_text += "% off"
	if inscription_text != "":
		t += " {"
		t += inscription_text
		t += "}"
	if t.length() > 75:
		t = t.left(75)
	return t


func stompable() -> bool:
	if not is_known():
		if Inventory.item_type_has_flavour(item_type):
			if is_flavour_aware():
				return item_type.stompable[0]
		if identify_flags.is_clear(IdentifyFlags.SENSED):
			return false
	if category == ItemCategory.RING or category == ItemCategory.AMULET:
		if bonus_damage < 0 or bonus_armour_class < 0 or bonus_to_hit < 0 or type_specific_value < 0:
			return true
	if item_type.has_quality():
		match get_detailed_feeling():
			"terrible", "worthless", "cursed", "broken":
				return item_type.stompable[0]
			"average":
				return item_type.stompable[1]
			"good":
				return item_type.stompable[2]
			"excellent":
				return item_type.stompable[3]
			"special":
				return false
	if category == ItemCategory.CHEST:
		if not is_known():
			return false
		elif type_specific_value == 0:
			return item_type.stompable[0]
		elif type_specific_value < 0:
			return item_type.stompable[1]
		else:
			match Tables.CHEST_TRAPS[type_specific_value]:
				0:
					return item_type.stompable[2]
				_:
					return item_type.stompable[3]
	return item_type.stompable[0]


func get_detailed_feeling() -> String:
	if is_artifact() or is_legendary():
		if is_cursed() or is_broken():
			return "terrible"
		return "special"
	if is_rare():
		if is_cursed() or is_broken():
			return "worthless"
		return "excellent"
	if is_cursed():
		return "cursed"
	if is_broken():
		return "broken"
	if bonus_armour_class > 0:
		return "good"
	if bonus_to_hit + bonus_damage > 0:
		return "good"
	return "average"


func is_rare() -> bool:
	return (rare_index != 0)


func absorb(other: Item) -> void:
	var total: int = count + other.count
	count = total if total < MAX_STACK_SIZE else MAX_STACK_SIZE - 1
	if other.is_known():
		become_known()
	if (identify_flags.is_set(IdentifyFlags.STOREBOUGHT) or (other.identify_flags.is_set(IdentifyFlags.STOREBOUGHT) and
			not (identify_flags.is_set(IdentifyFlags.STOREBOUGHT) and other.identify_flags.is_set(IdentifyFlags.STOREBOUGHT)) ) ):
		if other.identify_flags.is_set(IdentifyFlags.STOREBOUGHT):
			other.identify_flags.clear_bits(IdentifyFlags.STOREBOUGHT)
		if identify_flags.is_set(IdentifyFlags.STOREBOUGHT):
			identify_flags.clear_bits(IdentifyFlags.STOREBOUGHT)
	if other.identify_flags.is_set(IdentifyFlags.MENTAL):
		identify_flags.set_bits(IdentifyFlags.MENTAL)
	if other.inscription != "":
		inscription = other.inscription
	if discount < other.discount:
		discount = other.discount


func get_vague_feeling() -> String:
	if is_cursed():
		return "cursed"
	if is_broken():
		return "broken"
	if is_artifact() or is_legendary():
		return "special"
	if is_rare():
		return "good"
	if bonus_armour_class > 0:
		return "good"
	if bonus_to_hit + bonus_damage > 0:
		return "good"
	return ""


func adjust_damage_for_monster_type(damage: int, monster: Monster) -> int:
	var multiplier: int = 1
	var monster_type: MonsterType = monster.monster_type
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	get_merged_flags(f_1, f_2, f_3)
	match category:
		ItemCategory.SHOT, \
		ItemCategory.ARROW, \
		ItemCategory.BOLT, \
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.SWORD, \
		ItemCategory.DIGGING:
			if f_1.is_set(ItemFlags1.SLAY_ANIMAL) and monster_type.flags_3.is_set(MonsterFlags3.ANIMAL):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.ANIMAL)
				if multiplier < 2:
					multiplier = 2
			if f_1.is_set(ItemFlags1.SLAY_EVIL) and monster_type.flags_3.is_set(MonsterFlags3.EVIL):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.EVIL)
				if multiplier < 2:
					multiplier = 2
			if f_1.is_set(ItemFlags1.SLAY_UNDEAD) and monster_type.flags_3.is_set(MonsterFlags3.UNDEAD):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.UNDEAD)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.SLAY_DEMON) and monster_type.flags_3.is_set(MonsterFlags3.DEMON):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.DEMON)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.SLAY_ORC) and monster_type.flags_3.is_set(MonsterFlags3.ORC):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.ORC)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.SLAY_TROLL) and monster_type.flags_3.is_set(MonsterFlags3.TROLL):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.TROLL)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.SLAY_GIANT) and monster_type.flags_3.is_set(MonsterFlags3.GIANT):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.GIANT)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.SLAY_DRAGON) and monster_type.flags_3.is_set(MonsterFlags3.DRAGON):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.DRAGON)
				if multiplier < 3:
					multiplier = 3
			if f_1.is_set(ItemFlags1.KILL_DRAGON) and monster_type.flags_3.is_set(MonsterFlags3.DRAGON):
				if monster.is_visible:
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.DRAGON)
				if multiplier < 5:
					multiplier = 5
				if artifact_index == ArtifactId.SWORD_LIGHTNING:
					multiplier *= 3
			if f_1.is_set(ItemFlags1.BRAND_ACID):
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_ACID):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_ACID)
				else:
					if multiplier < 3:
						multiplier = 3
			if f_1.is_set(ItemFlags1.BRAND_ELECTRIC):
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_LIGHTNING):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_LIGHTNING)
				else:
					if multiplier < 3:
						multiplier = 3
			if f_1.is_set(ItemFlags1.BRAND_FIRE):
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_FIRE):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_FIRE)
				else:
					if multiplier < 3:
						multiplier = 3
			if f_1.is_set(ItemFlags1.BRAND_COLD):
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_COLD):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_COLD)
				else:
					if multiplier < 3:
						multiplier = 3
			if f_1.is_set(ItemFlags1.BRAND_POISON):
				if monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_POISON):
					if monster.is_visible:
						monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_POISON)
				else:
					if multiplier < 3:
						multiplier = 3
	return damage * multiplier


func store_description(pref: bool, mode: int) -> String:
	var previously_aware: bool = item_type.flavour_aware
	var previously_known: bool = identify_flags.is_set(IdentifyFlags.KNOWN)
	identify_flags.set_bits(IdentifyFlags.KNOWN)
	item_type.flavour_aware = true
	var buf: String = description(pref, mode)
	item_type.flavour_aware = previously_aware
	if not previously_known:
		identify_flags.clear_bits(IdentifyFlags.KNOWN)
	return buf

func breakage_chance() -> int:
	match category:
		ItemCategory.FLASK, ItemCategory.POTION, ItemCategory.BOTTLE, ItemCategory.FOOD, ItemCategory.JUNK:
			return 100
		ItemCategory.LIGHT, ItemCategory.SCROLL, ItemCategory.SKELETON:
			return 50
		ItemCategory.WAND, ItemCategory.ARROW, ItemCategory.SHOT, ItemCategory.BOLT:
			return 25
	return 10

func create_legendary(from_scroll:bool)->bool:
	var forge: ItemFactory = ItemFactory.new(self)
	return forge.create_legendary(from_scroll)

func get_artifact_resistances()->void:
	var forge: ItemFactory = ItemFactory.new(self)
	forge.get_artifact_resistances()


func hates_acid()->bool:
	match category:
		ItemCategory.ARROW, ItemCategory.BOLT, ItemCategory.BOW, ItemCategory.SWORD, \
		ItemCategory.HAFTED, ItemCategory.POLEARM, ItemCategory.HELM, ItemCategory.CROWN, \
		ItemCategory.SHIELD, ItemCategory.BOOTS, ItemCategory.GLOVES, ItemCategory.CLOAK, \
		ItemCategory.SOFT_ARMOUR, ItemCategory.HARD_ARMOUR, ItemCategory.DRAGON_ARMOUR:
			return true
		ItemCategory.STAFF, ItemCategory.SCROLL:
			return true
		ItemCategory.CHEST:
			return true
		ItemCategory.SKELETON, ItemCategory.BOTTLE, ItemCategory.JUNK:
			return true
	return false

func hates_cold()->bool:
	match category:
		ItemCategory.POTION, ItemCategory.FLASK, ItemCategory.BOTTLE:
			return true
	return false

func hates_elec()->bool:
	match category:
		ItemCategory.RING, ItemCategory.WAND:
			return true
	return false

func hates_fire()->bool:
	match category:
		ItemCategory.LIGHT, ItemCategory.ARROW, ItemCategory.BOW, ItemCategory.HAFTED, \
		ItemCategory.POLEARM, ItemCategory.BOOTS, ItemCategory.GLOVES, ItemCategory.CLOAK, \
		ItemCategory.SOFT_ARMOUR:
			return true
		ItemCategory.LIFE_BOOK, ItemCategory.SORCERY_BOOK, ItemCategory.NATURE_BOOK, \
		ItemCategory.CHAOS_BOOK, ItemCategory.DEATH_BOOK, ItemCategory.TAROT_BOOK, \
		ItemCategory.FOLK_BOOK, ItemCategory.CORPOREAL_BOOK:
			return true
		ItemCategory.CHEST:
			return true
		ItemCategory.STAFF, ItemCategory.SCROLL:
			return true
	return false


func identify_fully()->bool:
	var f_1:FlagSet = FlagSet.new()
	var f_2:FlagSet = FlagSet.new()
	var f_3:FlagSet = FlagSet.new()
	var info:Array[String] = []
	get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.ACTIVATE):
		info.append ("It can be activated for...")
		info.append (describe_activation_effect())
		info.append ("...if it is being worn.")
	if category == ItemCategory.LIGHT:
		if is_artifact():
			info.append ("It provides light (radius 3) forever.")
		elif sub_category == LightType.LANTERN:
			info.append ("It provides light (radius 2) when fueled.")
		elif sub_category == LightType.TORCH:
			info.append ("It provides light (radius 1) when fueled.")
		else:
			info.append ("It provides light (radius 2) forever.")
	if f_1.is_set(ItemFlags1.STR):
		info.append ("It affects your strength.")
	if f_1.is_set(ItemFlags1.INT):
		info.append ("It affects your intelligence.")
	if f_1.is_set(ItemFlags1.WIS):
		info.append ("It affects your wisdom.")
	if f_1.is_set(ItemFlags1.DEX):
		info.append ("It affects your dexterity.")
	if f_1.is_set(ItemFlags1.CON):
		info.append ("It affects your constitution.")
	if f_1.is_set(ItemFlags1.CHA):
		info.append ("It affects your charisma.")
	if f_1.is_set(ItemFlags1.STEALTH):
		info.append ("It affects your stealth.")
	if f_1.is_set(ItemFlags1.SEARCH):
		info.append ("It affects your searching.")
	if f_1.is_set(ItemFlags1.INFRA):
		info.append ("It affects your infravision.")
	if f_1.is_set(ItemFlags1.TUNNEL):
		info.append ("It affects your ability to tunnel.")
	if f_1.is_set(ItemFlags1.SPEED):
		info.append ("It affects your movement speed.")
	if f_1.is_set(ItemFlags1.BLOWS):
		info.append ("It affects your attack speed.")
	if f_1.is_set(ItemFlags1.BRAND_ACID):
		info.append ("It does extra damage from acid.")
	if f_1.is_set(ItemFlags1.BRAND_ELECTRIC):
		info.append ("It does extra damage from electricity.")
	if f_1.is_set(ItemFlags1.BRAND_FIRE):
		info.append ("It does extra damage from fire.")
	if f_1.is_set(ItemFlags1.BRAND_COLD):
		info.append ("It does extra damage from frost.")
	if f_1.is_set(ItemFlags1.BRAND_POISON):
		info.append ("It poisons your foes.")
	if f_1.is_set(ItemFlags1.CHAOTIC):
		info.append ("It produces chaotic effects.")
	if f_1.is_set(ItemFlags1.VAMPIRIC):
		info.append ("It drains life from your foes.")
	if f_1.is_set(ItemFlags1.IMPACT):
		info.append ("It can cause earthquakes.")
	if f_1.is_set(ItemFlags1.VORPAL):
		info.append ("It is very sharp and can cut your foes.")
	if f_1.is_set(ItemFlags1.KILL_DRAGON):
		info.append ("It is a great bane of dragons.")
	elif f_1.is_set(ItemFlags1.SLAY_DRAGON):
		info.append ("It is especially deadly against dragons.")
	if f_1.is_set(ItemFlags1.SLAY_ORC):
		info.append ("It is especially deadly against orcs.")
	if f_1.is_set(ItemFlags1.SLAY_TROLL):
		info.append ("It is especially deadly against trolls.")
	if f_1.is_set(ItemFlags1.SLAY_GIANT):
		info.append ("It is especially deadly against giants.")
	if f_1.is_set(ItemFlags1.SLAY_DEMON):
		info.append ("It strikes at demons with holy wrath.")
	if f_1.is_set(ItemFlags1.SLAY_UNDEAD):
		info.append ("It strikes at undead with holy wrath.")
	if f_1.is_set(ItemFlags1.SLAY_EVIL):
		info.append ("It fights against evil with holy fury.")
	if f_1.is_set(ItemFlags1.SLAY_ANIMAL):
		info.append ("It is especially deadly against natural creatures.")
	if f_2.is_set(ItemFlags2.SUST_STR):
		info.append ("It sustains your strength.")
	if f_2.is_set(ItemFlags2.SUST_INT):
		info.append ("It sustains your intelligence.")
	if f_2.is_set(ItemFlags2.SUST_WIS):
		info.append ("It sustains your wisdom.")
	if f_2.is_set(ItemFlags2.SUST_DEX):
		info.append ("It sustains your dexterity.")
	if f_2.is_set(ItemFlags2.SUST_CON):
		info.append ("It sustains your constitution.")
	if f_2.is_set(ItemFlags2.SUST_CHA):
		info.append ("It sustains your charisma.")
	if f_2.is_set(ItemFlags2.IM_ACID):
		info.append ("It provides immunity to acid.")
	if f_2.is_set(ItemFlags2.IM_ELEC):
		info.append ("It provides immunity to electricity.")
	if f_2.is_set(ItemFlags2.IM_FIRE):
		info.append ("It provides immunity to fire.")
	if f_2.is_set(ItemFlags2.IM_COLD):
		info.append ("It provides immunity to cold.")
	if f_2.is_set(ItemFlags2.FREE_ACT):
		info.append ("It provides immunity to paralysis.")
	if f_2.is_set(ItemFlags2.HOLD_LIFE):
		info.append ("It provides resistance to life draining.")
	if f_2.is_set(ItemFlags2.RES_FEAR):
		info.append ("It makes you completely fearless.")
	if f_2.is_set(ItemFlags2.RES_ACID):
		info.append ("It provides resistance to acid.")
	if f_2.is_set(ItemFlags2.RES_ELEC):
		info.append ("It provides resistance to electricity.")
	if f_2.is_set(ItemFlags2.RES_FIRE):
		info.append ("It provides resistance to fire.")
	if f_2.is_set(ItemFlags2.RES_COLD):
		info.append ("It provides resistance to cold.")
	if f_2.is_set(ItemFlags2.RES_POIS):
		info.append ("It provides resistance to poison.")
	if f_2.is_set(ItemFlags2.RES_LIGHT):
		info.append ("It provides resistance to light.")
	if f_2.is_set(ItemFlags2.RES_DARK):
		info.append ("It provides resistance to dark.")
	if f_2.is_set(ItemFlags2.RES_BLIND):
		info.append ("It provides resistance to blindness.")
	if f_2.is_set(ItemFlags2.RES_CONF):
		info.append ("It provides resistance to confusion.")
	if f_2.is_set(ItemFlags2.RES_SOUND):
		info.append ("It provides resistance to sound.")
	if f_2.is_set(ItemFlags2.RES_SHARDS):
		info.append ("It provides resistance to shards.")
	if f_2.is_set(ItemFlags2.RES_NETHER):
		info.append ("It provides resistance to nether.")
	if f_2.is_set(ItemFlags2.RES_NEXUS):
		info.append ("It provides resistance to nexus.")
	if f_2.is_set(ItemFlags2.RES_CHAOS):
		info.append ("It provides resistance to chaos.")
	if f_2.is_set(ItemFlags2.RES_DISEN):
		info.append ("It provides resistance to disenchantment.")
	if f_3.is_set(ItemFlags3.WRAITH):
		info.append ("It renders you incorporeal.")
	if f_3.is_set(ItemFlags3.FEATHER):
		info.append ("It allows you to levitate.")
	if f_3.is_set(ItemFlags3.LIGHT_SOURCE):
		info.append ("It provides permanent light.")
	if f_3.is_set(ItemFlags3.SEE_INVIS):
		info.append ("It allows you to see invisible monsters.")
	if f_3.is_set(ItemFlags3.TELEPATHY):
		info.append ("It gives telepathic powers.")
	if f_3.is_set(ItemFlags3.SLOW_DIGEST):
		info.append ("It slows your metabolism.")
	if f_3.is_set(ItemFlags3.REGEN):
		info.append ("It speeds your regenerative powers.")
	if f_2.is_set(ItemFlags2.REFLECT):
		info.append ("It reflects bolts and arrows.")
	if f_3.is_set(ItemFlags3.SH_FIRE):
		info.append ("It produces a fiery sheath.")
	if f_3.is_set(ItemFlags3.SH_ELEC):
		info.append ("It produces an electric sheath.")
	if f_3.is_set(ItemFlags3.NO_MAGIC):
		info.append ("It produces an anti-magic shell.")
	if f_3.is_set(ItemFlags3.NO_TELE):
		info.append ("It prevents teleportation.")
	if f_3.is_set(ItemFlags3.XTRA_MIGHT):
		info.append ("It fires missiles with extra might.")
	if f_3.is_set(ItemFlags3.XTRA_SHOTS):
		info.append ("It fires missiles excessively fast.")
	if f_3.is_set(ItemFlags3.DRAIN_EXP):
		info.append ("It drains experience.")
	if f_3.is_set(ItemFlags3.TELEPORT):
		info.append ("It induces random teleportation.")
	if f_3.is_set(ItemFlags3.AGGRAVATE):
		info.append ("It aggravates nearby creatures.")
	if f_3.is_set(ItemFlags3.BLESSED):
		info.append ("It has been blessed by the gods.")
	if is_cursed():
		if f_3.is_set(ItemFlags3.PERMA_CURSE):
			info.append ("It is permanently cursed.")
		elif f_3.is_set(ItemFlags3.HEAVY_CURSE):
			info.append ("It is heavily cursed.")
		else:
			info.append ("It is cursed.")
	if f_3.is_set(ItemFlags3.DREAD_CURSE):
		info.append ("It carries an ancient foul curse.")
	if f_3.is_set(ItemFlags3.IGNORE_ACID):
		info.append ("It cannot be harmed by acid.")
	if f_3.is_set(ItemFlags3.IGNORE_ELEC):
		info.append ("It cannot be harmed by electricity.")
	if f_3.is_set(ItemFlags3.IGNORE_FIRE):
		info.append ("It cannot be harmed by fire.")
	if f_3.is_set(ItemFlags3.IGNORE_COLD):
		info.append ("It cannot be harmed by cold.")
	if info.size() == 0:
		return false
	Console.save_screen()
	for k:int in range(1,45):
		Console.print_line_at(k, 13, "", Ink.WHITE)
	Console.print_line_at(1, 15, "     Item Attributes:", Ink.WHITE)
	var offset:int = 2
	for k:int in info.size():
		Console.print_line_at(k + offset, 15, info[k], Ink.WHITE)
		if k+offset == 43 and k < info.size()-1:
			Console.print_line_at (44,15,"-- more --", Ink.WHITE)
			User.get_key()
			offset -= 44
			for line:int in range(2,45):
				Console.print_line_at(line, 15, "", Ink.WHITE)
	Console.any_key(44)
	Console.restore_screen()
	return true


func describe_activation_effect()->String:
	var f_1:FlagSet = FlagSet.new()
	var f_2:FlagSet = FlagSet.new()
	var f_3:FlagSet = FlagSet.new()
	get_merged_flags(f_1, f_2, f_3)
	if f_3.is_clear(ItemFlags3.ACTIVATE):
		return ""
	if artifact_index == 0 and rare_index == 0 and bonus_power_type == 0 and bonus_power_sub_type != 0:
		match bonus_power_sub_type:
			LegendaryPower.SUNLIGHT:
					return "beam of sunlight every 10 turns"
			LegendaryPower.BO_MISS_1:
					return "magic missile (2d6) every 2 turns"
			LegendaryPower.BA_POIS_1:
					return "stinking cloud (12), rad. 3, every 4+d4 turns"
			LegendaryPower.BO_ELEC_1:
					return "lightning bolt (4d8) every 6+d6 turns"
			LegendaryPower.BO_ACID_1:
					return "acid bolt (5d8) every 5+d5 turns"
			LegendaryPower.BO_COLD_1:
					return "frost bolt (6d8) every 7+d7 turns"
			LegendaryPower.BO_FIRE_1:
					return "fire bolt (9d8) every 8+d8 turns"
			LegendaryPower.BA_COLD_1:
					return "ball of cold (48) every 400 turns"
			LegendaryPower.BA_FIRE_1:
					return "ball of fire (72) every 400 turns"
			LegendaryPower.DRAIN_1:
					return "drain life (100) every 100+d100 turns"
			LegendaryPower.BA_COLD_2:
					return "ball of cold (100) every 300 turns"
			LegendaryPower.BA_ELEC_2:
					return "ball of lightning (100) every 500 turns"
			LegendaryPower.DRAIN_2:
					return "drain life (120) every 400 turns"
			LegendaryPower.VAMPIRE_1:
					return "vampiric drain (3*50) every 400 turns"
			LegendaryPower.BO_MISS_2:
					return "arrows (150) every 90+d90 turns"
			LegendaryPower.BA_FIRE_2:
					return "fire ball (120) every 225+d225 turns"
			LegendaryPower.BA_COLD_3:
					return "ball of cold (200) every 325+d325 turns"
			LegendaryPower.WHIRLWIND:
					return "whirlwind attack every 250 turns"
			LegendaryPower.VAMPIRE_2:
					return "vampiric drain (3*100) every 400 turns"
			LegendaryPower.CALL_CHAOS:
					return "call chaos every 350 turns"
			LegendaryPower.SHARD:
					return "shard ball (120+level) every 400 turns"
			LegendaryPower.DISP_EVIL:
					return "dispel evil (level*5) every 300+d300 turns"
			LegendaryPower.DISP_GOOD:
					return "dispel good (level*5) every 300+d300 turns"
			LegendaryPower.BA_MISS_3:
					return "elemental breath (300) every 500 turns"
			LegendaryPower.CONFUSE:
					return "confuse monster every 15 turns"
			LegendaryPower.SLEEP:
					return "sleep nearby monsters every 55 turns"
			LegendaryPower.QUAKE:
					return "earthquake (rad 10) every 50 turns"
			LegendaryPower.TERROR:
					return "terror every 3 * (level+10) turns"
			LegendaryPower.TELE_AWAY:
					return "teleport away every 200 turns"
			LegendaryPower.BANISH_EVIL:
					return "banish evil every 250+d250 turns"
			LegendaryPower.CARNAGE:
					return "carnage every 500 turns"
			LegendaryPower.MASS_GENO:
					return "mass carnage every 1000 turns"
			LegendaryPower.CHARM_ANIMAL:
					return "charm animal every 300 turns"
			LegendaryPower.CHARM_UNDEAD:
					return "enslave undead every 333 turns"
			LegendaryPower.CHARM_OTHER:
					return "charm monster every 400 turns"
			LegendaryPower.CHARM_ANIMALS:
					return "animal friendship every 500 turns"
			LegendaryPower.CHARM_OTHERS:
					return "mass charm every 750 turns"
			LegendaryPower.SUMMON_ANIMAL:
					return "summon animal every 200+d300 turns"
			LegendaryPower.SUMMON_PHANTOM:
					return "summon phantasmal servant every 200+d200 turns"
			LegendaryPower.SUMMON_ELEMENTAL:
					return "summon elemental every 750 turns"
			LegendaryPower.SUMMON_DEMON:
					return "summon demon every 666+d333 turns"
			LegendaryPower.SUMMON_UNDEAD:
					return "summon undead every 666+d333 turns"
			LegendaryPower.CURE_L_W:
					return "remove fear & heal 30 hp every 10 turns"
			LegendaryPower.CURE_M_W:
					return "heal 4d8 & wounds every 3+d3 turns"
			LegendaryPower.CURE_POISON:
					return "remove fear and cure poison every 5 turns"
			LegendaryPower.REST_LIFE:
					return "restore life levels every 450 turns"
			LegendaryPower.REST_ALL:
					return "restore stats and life levels every 750 turns"
			LegendaryPower.CURE_700:
					return "heal 700 hit points every 250 turns"
			LegendaryPower.CURE_1000:
					return "heal 1000 hit points every 888 turns"
			LegendaryPower.ESP:
					return "temporary ESP (dur 25+d30) every 200 turns"
			LegendaryPower.BERSERK:
					return "heroism and berserk (dur 50+d50) every 100+d100 turns"
			LegendaryPower.PROT_EVIL:
					return "protect evil (dur level*3 + d25) every 225+d225 turns"
			LegendaryPower.RESIST_ALL:
					return "resist elements (dur 40+d40) every 200 turns"
			LegendaryPower.SPEED:
					return "speed (dur 20+d20) every 250 turns"
			LegendaryPower.XTRA_SPEED:
					return "speed (dur 75+d75) every 200+d200 turns"
			LegendaryPower.WRAITH:
					return "wraith form (level/2 + d(level/2)) every 1000 turns"
			LegendaryPower.INVULN:
					return "invulnerability (dur 8+d8) every 1000 turns"
			LegendaryPower.LIGHT:
					return "light area (dam 2d15) every 10+d10 turns"
			LegendaryPower.MAP_LIGHT:
					return "light (dam 2d15) & map area every 50+d50 turns"
			LegendaryPower.DETECT_ALL:
					return "detection every 55+d55 turns"
			LegendaryPower.DETECT_XTRA:
					return "detection, probing and identify true every 1000 turns"
			LegendaryPower.ID_FULL:
					return "identify true every 750 turns"
			LegendaryPower.ID_PLAIN:
					return "identify spell every 10 turns"
			LegendaryPower.RUNE_EXPLO:
					return "Yellow Sign every 200 turns"
			LegendaryPower.RUNE_PROT:
					return "rune of protection every 400 turns"
			LegendaryPower.SATIATE:
					return "satisfy hunger every 200 turns"
			LegendaryPower.DEST_DOOR:
					return "destroy doors every 10 turns"
			LegendaryPower.STONE_MUD:
					return "stone to mud every 5 turns"
			LegendaryPower.RECHARGE:
					return "recharging every 70 turns"
			LegendaryPower.ALCHEMY:
					return "alchemy every 500 turns"
			LegendaryPower.DIM_DOOR:
					return "dimension door every 100 turns"
			LegendaryPower.TELEPORT:
					return "teleport (range 100) every 45 turns"
			LegendaryPower.RECALL:
					return "word of recall every 200 turns"
			_:
					return "something undefined"
	match artifact_index:
		ArtifactId.DAGGER_OF_FAITH:
				return "fire bolt (9d8) every 8+d8 turns"
		ArtifactId.DAGGER_OF_HOPE:
				return "frost bolt (6d8) every 7+d7 turns"
		ArtifactId.DAGGER_OF_CHARITY:
				return "lightning bolt (4d8) every 6+d6 turns"
		ArtifactId.DAGGER_OF_THOTH:
				return "stinking cloud (12) every 4+d4 turns"
		ArtifactId.DAGGER_ICICLE:
				return "frost ball (48) every 5+d5 turns"
		ArtifactId.BOOTS_OF_DANCING:
				return "remove fear and cure poison every 5 turns"
		ArtifactId.SWORD_EXCALIBUR:
				return "frost ball (100) every 300 turns"
		ArtifactId.SWORD_OF_THE_DAWN:
				return "summon a Black Reaver every 500+d500 turns"
		ArtifactId.SWORD_OF_EVERFLAME:
				return "fire ball (72) every 400 turns"
		ArtifactId.MORNING_STAR_FIRESTARTER:
				return "large fire ball (72) every 100 turns"
		ArtifactId.BOOTS_OF_ITHAQUA:
				return "haste self (20+d20 turns) every 200 turns"
		ArtifactId.AXE_OF_THEODEN:
				return "drain life (120) every 400 turns"
		ArtifactId.HAMMER_JUSTICE:
				return "drain life (90) every 70 turns"
		ArtifactId.ARMOUR_OF_THE_OGRE_LORDS:
				return "door and trap destruction every 10 turns"
		ArtifactId.SCYTHE_OF_GHARNE:
				return "word of recall every 200 turns"
		ArtifactId.MACE_THUNDER:
				return "haste self (20+d20 turns) every 100+d100 turns"
		ArtifactId.QUARTERSTAFF_ERIRIL:
				return "identify every 10 turns"
		ArtifactId.QUARTERSTAFF_OF_ATAL:
				return "probing, detection and full id  every 1000 turns"
		ArtifactId.AXE_OF_THE_TROLLS:
				return "mass carnage every 1000 turns"
		ArtifactId.AXE_SPLEENSLICER:
				return "cure wounds (4d7) every 3+d3 turns"
		ArtifactId.CROSSBOW_OF_DEATH:
				return "fire branding of bolts every 999 turns"
		ArtifactId.SWORD_OF_KARAKAL:
				return "a getaway every 35 turns"
		ArtifactId.SPEAR_GUNGNIR:
				return "lightning ball (100) every 500 turns"
		ArtifactId.SPEAR_OF_DESTINY:
				return "stone to mud every 5 turns"
		ArtifactId.PLATE_MAIL_SOULKEEPER:
				return "heal (1000) every 888 turns"
		ArtifactId.ARMOUR_OF_THE_VAMPIRE_HUNTER:
				return "heal (777), curing and heroism every 300 turns"
		ArtifactId.ARMOUR_OF_THE_ORCS:
				return "carnage every 500 turns"
		ArtifactId.SHADOW_CLOAK_OF_NYOGTHA:
				return "restore life levels every 450 turns"
		ArtifactId.TRIDENT_OF_THE_GNORRI:
				return "teleport away every 150 turns"
		ArtifactId.CLOAK_OF_BARZAI:
				return "resistance (20+d20 turns) every 111 turns"
		ArtifactId.CLOAK_DARKNESS:
				return "Sleep II every 55 turns"
		ArtifactId.CLOAK_OF_THE_SWASHBUCKLER:
				return "recharge item I every 70 turns"
		ArtifactId.CLOAK_SHIFTER:
				return "teleport every 45 turns"
		ArtifactId.FLAIL_TOTILA:
				return "confuse monster every 15 turns"
		ArtifactId.GLOVES_OF_LIGHT:
				return "magic missile (2d6) every 2 turns"
		ArtifactId.GAUNTLET_IRON_FIST:
				return "fire bolt (9d8) every 8+d8 turns"
		ArtifactId.GAUNTLETS_OF_GHOULS:
				return "frost bolt (6d8) every 7+d7 turns"
		ArtifactId.GAUNTLETS_WHITESPARK:
				return "lightning bolt (4d8) every 6+d6 turns"
		ArtifactId.GAUNTLETS_OF_THE_DEAD:
				return "acid bolt (5d8) every 5+d5 turns"
		ArtifactId.CESTI_OF_COMBAT:
				return "a magical arrow (150) every 90+d90 turns"
		ArtifactId.HELM_SKULLKEEPER:
				return "detection every 55+d55 turns"
		ArtifactId.CROWN_OF_THE_SUN:
				return "heal (700) every 250 turns"
		ArtifactId.DRAGONSCALE_RAZORBACK:
				return "star ball (150) every 1000 turns"
		ArtifactId.DRAGONSCALE_BLADETURNER:
				return "breathe elements (300), berserk rage, bless, and resistance"
		ArtifactId.STAR_ESSENCE_OF_POLARIS:
				return "illumination every 10+d10 turns"
		ArtifactId.STAR_ESSENCE_OF_XOTH:
				return "magic mapping and light every 50+d50 turns"
		ArtifactId.SHINING_TRAPEZOHEDRON:
				return "clairvoyance and recall, draining you"
		ArtifactId.AMULET_OF_ABDUL_ALHAZRED:
				return "dispel evil (x5) every 300+d300 turns"
		ArtifactId.AMULET_OF_LOBON:
				return "protection from evil every 225+d225 turns"
		ArtifactId.RING_OF_MAGIC:
				return "a strangling attack (100) every 100+d100 turns"
		ArtifactId.RING_OF_BAST:
				return "haste self (75+d75 turns) every 150+d150 turns"
		ArtifactId.RING_OF_ELEMENTAL_POWER_FIRE:
				return "large fire ball (120) every 225+d225 turns"
		ArtifactId.RING_OF_ELEMENTAL_POWER_ICE:
				return "large frost ball (200) every 325+d325 turns"
		ArtifactId.RING_OF_ELEMENTAL_POWER_STORM:
				return "large lightning ball (250) every 425+d425 turns"
		ArtifactId.RING_OF_SET:
				return "bizarre things every 450+d450 turns"
		ArtifactId.DRAGON_HELM_OF_POWER, ArtifactId.HELM_TERROR_MASK:
				return "rays of fear in every direction"
	if rare_index == RareItemId.WEAPON_PLANAR_WEAPON:
		return "teleport every 50+d50 turns"
	if category == ItemCategory.RING:
		match sub_category:
			RingType.FLAMES:
				return "ball of fire and resist fire"
			RingType.ICE:
				return "ball of cold and resist cold"
			RingType.ACID:
				return "ball of acid and resist acid"
			_:
				return ""
	if category != ItemCategory.DRAGON_ARMOUR:
		return ""
	match sub_category:
		DragonArmour.BLUE:
				return "breathe lightning (100) every 450+d450 turns"
		DragonArmour.WHITE:
				return "breathe frost (110) every 450+d450 turns"
		DragonArmour.BLACK:
				return "breathe acid (130) every 450+d450 turns"
		DragonArmour.GREEN:
				return "breathe poison gas (150) every 450+d450 turns"
		DragonArmour.RED:
				return "breathe fire (200) every 450+d450 turns"
		DragonArmour.MULTI_HUED:
				return "breathe multi-hued (250) every 225+d225 turns"
		DragonArmour.BRONZE:
				return "breathe confusion (120) every 450+d450 turns"
		DragonArmour.GOLD:
				return "breathe sound (130) every 450+d450 turns"
		DragonArmour.CHAOS:
				return "breathe chaos/disenchant (220) every 300+d300 turns"
		DragonArmour.LAW:
				return "breathe sound/shards (230) every 300+d300 turns"
		DragonArmour.BALANCE:
				return "You breathe balance (250) every 300+d300 turns"
		DragonArmour.PSEUDO:
				return "breathe light/darkness (200) every 300+d300 turns"
		DragonArmour.POWER:
				return "breathe the elements (300) every 300+d300 turns"
	return ""


func item_tried()->void:
	item_type.tried = true
