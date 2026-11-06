# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemFactory
extends RefCounted

const WEIRD_LUCK: int = 12
const ARTIFACT_CURSE_CHANCE: int = 13
const ACTIVATION_CHANCE: int = 3

var _item: Item
var _legendary_item_bias: int


func _init(item: Item) -> void:
	_item = item


# Apply random magic to an item, based on the item's level
func apply_magic(level: int, allow_artifact: bool, force_good: bool, force_great: bool) -> void:
	if level > DungeonLevel.MAX_DEPTH - 1:
		level = DungeonLevel.MAX_DEPTH - 1
	var f1: int = level + 10
	if f1 > 75:
		f1 = 75
	var f2: int = f1 / 2
	if f2 > 20:
		f2 = 20
	var power: int = 0
	# Chance of being good even if not forced to be
	if force_good or Rng.roll_chance(f1):
		power = 1
		# Chance of being great even if not forced to be
		if force_great or Rng.roll_chance(f2):
			power = 2
	# If not good, then might be bad
	elif Rng.roll_chance(f1):
		power = -1
		# If bad, then might be terrible
		if Rng.roll_chance(f2):
			power = -2
	# Number of rolls to see if we get an artifact is based on power level
	var artifact_rolls: int = 0
	if power >= 2:
		artifact_rolls = 1
	if force_great:
		artifact_rolls = 4
	if not allow_artifact or _item.artifact_index != 0:
		artifact_rolls = 0
	# Try to get an artifact
	for i: int in artifact_rolls:
		if apply_artifact():
			break
	# If we became an artifact, copy the artifact's power set
	if _item.artifact_index != 0:
		var artifact: Artifact = SaveGame.instance.artifacts[_item.artifact_index]
		artifact.current_number = 1
		_item.type_specific_value = artifact.type_specific_value
		_item.base_armour_class = artifact.ac
		_item.damage_dice = artifact.damage_dice
		_item.damage_dice_sides = artifact.damage_dice_sides
		_item.bonus_armour_class = artifact.to_ac
		_item.bonus_to_hit = artifact.to_hit
		_item.bonus_damage = artifact.to_damage
		_item.weight = artifact.weight
		if artifact.cost == 0:
			_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
		if artifact.flags_3.is_set(ItemFlags3.CURSED):
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)
		if SaveGame.instance.dungeon_level != null:
			SaveGame.instance.dungeon_level.treasure_rating += 10
			if artifact.cost > 50000:
				SaveGame.instance.dungeon_level.treasure_rating += 10
			SaveGame.instance.dungeon_level.special_treasure = true
		return
	# We weren't an artifact, so apply the correct type of magic based on our item category
	match _item.category:
		ItemCategory.DIGGING, ItemCategory.HAFTED, ItemCategory.POLEARM, ItemCategory.SWORD:
			if power != 0:
				apply_magic_to_weapon(level, power)
		ItemCategory.BOW, ItemCategory.SHOT, ItemCategory.ARROW, ItemCategory.BOLT:
			if power != 0:
				apply_magic_to_weapon(level, power)
		ItemCategory.DRAGON_ARMOUR, ItemCategory.HARD_ARMOUR, ItemCategory.SOFT_ARMOUR, ItemCategory.SHIELD:
			if (power != 0 or
				(_item.category == ItemCategory.HELM and _item.sub_category == HelmType.DRAGON_HELM) or
				(_item.category == ItemCategory.SHIELD and _item.sub_category == ShieldType.DRAGON_SHIELD) or
				(_item.category == ItemCategory.CLOAK and _item.sub_category == CloakType.ELVEN_CLOAK) ):
				apply_magic_to_armour(level, power)
		ItemCategory.HELM, ItemCategory.CROWN, ItemCategory.CLOAK, ItemCategory.GLOVES, ItemCategory.BOOTS:
			if (power != 0 or
				(_item.category == ItemCategory.HELM and _item.sub_category == HelmType.DRAGON_HELM) or
				(_item.category == ItemCategory.SHIELD and _item.sub_category == ShieldType.DRAGON_SHIELD) or
				(_item.category == ItemCategory.CLOAK and _item.sub_category == CloakType.ELVEN_CLOAK) ):
				apply_magic_to_armour(level, power)
		ItemCategory.RING, ItemCategory.AMULET:
			if power == 0 and Rng.roll_die(100) <= 50:
				power = -1
			apply_magic_to_jewellrey(level, power)
		ItemCategory.LIGHT:
			apply_magic_to_light_source(power)
		_:
			apply_magic_to_misc_item()
	# check if the item was made legendary, and if so increase the level rating
	if _item.is_legendary():
		if SaveGame.instance.dungeon_level != null:
			SaveGame.instance.dungeon_level.treasure_rating += 40
	# If not legendary, it might be one of the rare types, so assign the correct additional ability
	elif _item.rare_index != RareItemId.NONE:
		var rare_type: RareItemType = StaticData.rare_item_types[_item.rare_index]
		match _item.rare_index:
			RareItemId.WEAPON_ELDER_SIGN:
				_item.bonus_power_type = RareItemId.SPECIAL_SUSTAIN
			RareItemId.WEAPON_DEFENDER:
				_item.bonus_power_type = RareItemId.SPECIAL_SUSTAIN
			RareItemId.WEAPON_BLESSED:
				_item.bonus_power_type = RareItemId.SPECIAL_ABILITY
			RareItemId.WEAPON_PLANAR_WEAPON:
				if Rng.roll_die(7) == 1:
					_item.bonus_power_type = RareItemId.SPECIAL_ABILITY
			RareItemId.ARMOUR_OF_PERMANENCE:
				_item.bonus_power_type = RareItemId.SPECIAL_POWER
			RareItemId.ARMOUR_OF_YITH:
				_item.bonus_power_type = RareItemId.SPECIAL_POWER
			RareItemId.HAT_OF_THE_MAGI:
				_item.bonus_power_type = RareItemId.SPECIAL_ABILITY
			RareItemId.CLOAK_OF_AMAN:
				_item.bonus_power_type = RareItemId.SPECIAL_POWER
		if _item.bonus_power_type != 0 and not _item.is_legendary():
			_item.bonus_power_sub_type = Rng.roll_die(256)
		if rare_type.cost == 0:
			_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
		if rare_type.flags_3.is_set(ItemFlags3.CURSED):
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)
		if _item.is_cursed() or _item.is_broken():
			if rare_type.max_to_hit != 0:
				_item.bonus_to_hit -= Rng.roll_die(rare_type.max_to_hit)
			if rare_type.max_to_damage != 0:
				_item.bonus_damage -= Rng.roll_die(rare_type.max_to_damage)
			if rare_type.max_to_ac != 0:
				_item.bonus_armour_class -= Rng.roll_die(rare_type.max_to_ac)
			if rare_type.max_type_specifc_value != 0:
				_item.type_specific_value -= Rng.roll_die(rare_type.max_type_specifc_value)
		else:
			if rare_type.max_to_hit != 0:
				_item.bonus_to_hit += Rng.roll_die(rare_type.max_to_hit)
			if rare_type.max_to_damage != 0:
				_item.bonus_damage += Rng.roll_die(rare_type.max_to_damage)
			if rare_type.max_to_ac != 0:
				_item.bonus_armour_class += Rng.roll_die(rare_type.max_to_ac)
			if rare_type.max_type_specifc_value != 0:
				_item.type_specific_value += Rng.roll_die(rare_type.max_type_specifc_value)
		if SaveGame.instance.dungeon_level != null:
			SaveGame.instance.dungeon_level.treasure_rating += rare_type.rating
		return
	if _item.item_type != null:
		var item_type: ItemType = _item.item_type
		if item_type.cost == 0:
			_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
		if item_type.flags_3.is_set(ItemFlags3.CURSED):
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)


func apply_artifact() -> bool:
	if _item.count != 1:
		return false
	for artifact_id: int in SaveGame.instance.artifacts:
		var artifact: Artifact = SaveGame.instance.artifacts[artifact_id]
		if artifact.has_own_type:
			continue
		if artifact.current_number != 0:
			continue
		if artifact.item_category != _item.category:
			continue
		if artifact.item_sub_category != _item.sub_category:
			continue
		if artifact.level > SaveGame.instance.total_difficulty():
			var d: int = (artifact.level - SaveGame.instance.total_difficulty()) * 2
			if Rng.roll_die(d + 1) != 1:
				continue
		if Rng.roll_die(artifact.rarity) != 1:
			continue
		_item.artifact_index = artifact_id
		get_artifact_resistances()
		return true
	return false


func get_artifact_resistances() -> void:
	var give_resistance: bool = false
	var give_power: bool = false
	# Special - Terror Mask is good for warrior types, but bad for everyone else
	if _item.artifact_index == ArtifactId.HELM_TERROR_MASK:
		if SaveGame.instance.player.profession.legendary_item_bias == LegendaryItemBias.WARRIOR:
			give_power = true
			give_resistance = true
		else:
			_item.legendary_flags_3.set_bits(ItemFlags3.CURSED | ItemFlags3.HEAVY_CURSE | ItemFlags3.AGGRAVATE | ItemFlags3.DREAD_CURSE)
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)
			return
	match _item.artifact_index:
		# Some just get resistances
		ArtifactId.ARMOUR_OF_THE_ORCS, ArtifactId.CHAIN_MAIL_HEARTGUARD, ArtifactId.ARMOUR_OF_THE_OGRE_LORDS:
			give_resistance = true
		ArtifactId.ARMOUR_OF_THE_KOBOLD_CHIEF, ArtifactId.ARMOUR_OF_SERPENTS, ArtifactId.SHIELD_RAWHIDE:
			give_resistance = true
		ArtifactId.SHIELD_OF_STABILITY, ArtifactId.CAP_OF_THE_MINDCRAFTER, ArtifactId.SHADOW_CLOAK_OF_NYOGTHA:
			give_resistance = true
		ArtifactId.BOOTS_OF_THE_BLACK_REAVER, ArtifactId.SHIELD_VITRIOL, ArtifactId.DAGGER_OF_HOPE:
			give_resistance = true
		ArtifactId.DAGGER_OF_CHARITY, ArtifactId.DAGGER_OF_FAITH, ArtifactId.SMALL_SWORD_STING:
			give_resistance = true
		ArtifactId.HAMMER_JUSTICE, ArtifactId.SCALE_MAIL_WYVERNSCALE:
			give_resistance = true
		# Some get either resistances or powers
		ArtifactId.MAIN_GAUCHE_OF_DEFENCE, ArtifactId.SWORD_BRIGHTBLADE, ArtifactId.SWORD_BLACK_ICE:
			if Rng.roll_die(2) == 1:
				give_resistance = true
			else:
				give_power = true
		ArtifactId.SWORD_OF_EVERFLAME, ArtifactId.SWORD_FIRETONGUE, ArtifactId.SWORD_DRAGONSLAYER:
			if Rng.roll_die(2) == 1:
				give_resistance = true
			else:
				give_power = true
		ArtifactId.SCIMITAR_SOULSWORD, ArtifactId.BOW_OF_SERPENTS, ArtifactId.CROSSBOW_OF_DEATH:
			if Rng.roll_die(2) == 1:
				give_resistance = true
			else:
				give_power = true
		# Some just get power
		ArtifactId.RING_OF_ELEMENTAL_POWER_ICE, ArtifactId.RING_OF_ELEMENTAL_POWER_STORM, ArtifactId.CROWN_OF_MISERY:
			give_power = true
		ArtifactId.CESTI_OF_COMBAT, ArtifactId.CLOAK_OF_THE_SWASHBUCKLER, ArtifactId.TRIDENT_OF_THE_GNORRI:
			give_power = true
		ArtifactId.QUARTERSTAFF_OF_ATAL:
			give_power = true
		# Some get both
		ArtifactId.RING_OF_SET, ArtifactId.CROWN_OF_THE_SUN, ArtifactId.HAMMER_MJOLNIR:
			give_power = true
			give_resistance = true
	# Give power if needed
	if give_power:
		_item.bonus_power_type = RareItemId.SPECIAL_ABILITY
		_item.bonus_power_sub_type = Rng.roll_die(256)
	_legendary_item_bias = 0
	if give_resistance:
		apply_random_resistance(Rng.roll_die(22) + 16)


## Adds semi-random resistances based on the legendary item bias.
## Note that artifacts and legendary items both use these, although they are stored on the legendary flags
func apply_random_resistance(specific: int) -> void:
	if specific == 0:
		# Acid bias gives us resist acid and possibly acid immunity
		if _legendary_item_bias == LegendaryItemBias.ACID:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_ACID):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_ACID)
				if Rng.roll_die(2) == 1:
					return
			if Rng.roll_die(LegendaryItemBias.LUCK) == 1 and _item.legendary_flags_2.is_clear(ItemFlags2.IM_ACID):
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_ACID)
				if Rng.roll_die(2) == 1:
					return
		# Electricity bias gives us resist electric and possibly electricity shield or electricity immunity
		elif _legendary_item_bias == LegendaryItemBias.ELECTRICITY:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_ELEC):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_ELEC)
				if Rng.roll_die(2) == 1:
					return
			if (_item.category >= ItemCategory.CLOAK and
				_item.category <= ItemCategory.HARD_ARMOUR and
				_item.legendary_flags_3.is_clear(ItemFlags3.SH_ELEC) ):
				_item.legendary_flags_3.set_bits(ItemFlags3.SH_ELEC)
				if Rng.roll_die(2) == 1:
					return
			if Rng.roll_die(LegendaryItemBias.LUCK) == 1 and _item.legendary_flags_2.is_clear(ItemFlags2.IM_ELEC):
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_ELEC)
				if Rng.roll_die(2) == 1:
					return
		# Fire bias gives us fire resistance and possibly fire shield or fire immunity
		elif _legendary_item_bias == LegendaryItemBias.FIRE:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_FIRE):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_FIRE)
				if Rng.roll_die(2) == 1:
					return
			if (_item.category >= ItemCategory.CLOAK and
				_item.category <= ItemCategory.HARD_ARMOUR and
				_item.legendary_flags_3.is_clear(ItemFlags3.SH_FIRE) ):
				_item.legendary_flags_3.set_bits(ItemFlags3.SH_FIRE)
				if Rng.roll_die(2) == 1:
					return
			if Rng.roll_die(LegendaryItemBias.LUCK) == 1 and _item.legendary_flags_2.is_clear(ItemFlags2.IM_FIRE):
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_FIRE)
				if Rng.roll_die(2) == 1:
					return
		# Cold bias gives us cold resistance and possibly cold immunity
		elif _legendary_item_bias == LegendaryItemBias.COLD:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_COLD):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_COLD)
				if Rng.roll_die(2) == 1:
					return
			if Rng.roll_die(LegendaryItemBias.LUCK) == 1 and _item.legendary_flags_2.is_clear(ItemFlags2.IM_COLD):
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_COLD)
				if Rng.roll_die(2) == 1:
					return
		# Poison bias gives us resist poison
		elif _legendary_item_bias == LegendaryItemBias.POISON:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_POIS):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
				if Rng.roll_die(2) == 1:
					return
		# Warrior bias gives resist fear and possibly anti-magic
		elif _legendary_item_bias == LegendaryItemBias.WARRIOR:
			if Rng.roll_die(3) != 1 and _item.legendary_flags_2.is_clear(ItemFlags2.RES_FEAR):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
				if Rng.roll_die(2) == 1:
					return
			if Rng.roll_die(3) == 1 and _item.legendary_flags_3.is_clear(ItemFlags3.NO_MAGIC):
				_item.legendary_flags_3.set_bits(ItemFlags3.NO_MAGIC)
				if Rng.roll_die(2) == 1:
					return
		# Necromantic bias gives us resist nether and possibly resist poison or resist dark
		elif _legendary_item_bias == LegendaryItemBias.NECROMANTIC:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_NETHER):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_NETHER)
				if Rng.roll_die(2) == 1:
					return
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_POIS):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
				if Rng.roll_die(2) == 1:
					return
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_DARK):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_DARK)
				if Rng.roll_die(2) == 1:
					return
		# Chaos bias gives us resist chaos and possibly resist confusion or resist disenchantment
		elif _legendary_item_bias == LegendaryItemBias.CHAOS:
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_CHAOS):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_CHAOS)
				if Rng.roll_die(2) == 1:
					return
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_CONF):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_CONF)
				if Rng.roll_die(2) == 1:
					return
			if _item.legendary_flags_2.is_clear(ItemFlags2.RES_DISEN):
				_item.legendary_flags_2.set_bits(ItemFlags2.RES_DISEN)
				if Rng.roll_die(2) == 1:
					return
	# If we didn't randomly return after getting something specific, add something semi-random, possibly recursively
	match specific if specific != 0 else Rng.roll_die(41):
		1: # or Electricity bias
			if Rng.roll_die(WEIRD_LUCK) != 1:
				apply_random_resistance(0)
			else:
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_ACID)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.ACID
		2: # or Poison bias
			if Rng.roll_die(WEIRD_LUCK) != 1:
				apply_random_resistance(0)
			else:
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_ELEC)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.ELECTRICITY
		3: # or Fire bias
			if Rng.roll_die(WEIRD_LUCK) != 1:
				apply_random_resistance(0)
			else:
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_COLD)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.COLD
		4: # or Cold bias
			if Rng.roll_die(WEIRD_LUCK) != 1:
				apply_random_resistance(0)
			else:
				_item.legendary_flags_2.set_bits(ItemFlags2.IM_FIRE)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.FIRE
		5, 6, 13: # or Acid, Strength, or Priestly bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_ACID)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.ACID
		7, 8, 14: # or Intelligence, Wisdom, or Necromantic bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_ELEC)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.ELECTRICITY
		9, 10, 15: # or Dexterity, Constitution, or Law bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_FIRE)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.FIRE
		11, 12, 16: # or Charisma, Chaos, or Rogue bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_COLD)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.COLD
		17, 18: # or Mage or Warrior bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
			if _legendary_item_bias == 0 and Rng.roll_die(4) != 1:
				_legendary_item_bias = LegendaryItemBias.POISON
			elif _legendary_item_bias == 0 and Rng.roll_die(2) == 1:
				_legendary_item_bias = LegendaryItemBias.NECROMANTIC
			elif _legendary_item_bias == 0 and Rng.roll_die(2) == 1:
				_legendary_item_bias = LegendaryItemBias.ROGUE
		19, 20: # or Ranger bias
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
			if _legendary_item_bias == 0 and Rng.roll_die(3) == 1:
				_legendary_item_bias = LegendaryItemBias.WARRIOR
		21:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_LIGHT)
		22:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_DARK)
		23, 24:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_BLIND)
		25, 26:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_CONF)
			if _legendary_item_bias == 0 and Rng.roll_die(6) == 1:
				_legendary_item_bias = LegendaryItemBias.CHAOS
		27, 28:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_SOUND)
		29, 30:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_SHARDS)
		31, 32:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_NETHER)
			if _legendary_item_bias == 0 and Rng.roll_die(3) == 1:
				_legendary_item_bias = LegendaryItemBias.NECROMANTIC
		33, 34:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_NEXUS)
		35, 36:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_CHAOS)
			if _legendary_item_bias == 0 and Rng.roll_die(2) == 1:
				_legendary_item_bias = LegendaryItemBias.CHAOS
		37, 38:
			_item.legendary_flags_2.set_bits(ItemFlags2.RES_DISEN)
		39:
			if (_item.category >= ItemCategory.CLOAK and
				_item.category <= ItemCategory.HARD_ARMOUR ):
				_item.legendary_flags_3.set_bits(ItemFlags3.SH_ELEC)
			else:
				apply_random_resistance(0)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.ELECTRICITY
		40:
			if (_item.category >= ItemCategory.CLOAK and
				_item.category <= ItemCategory.HARD_ARMOUR ):
				_item.legendary_flags_3.set_bits(ItemFlags3.SH_FIRE)
			else:
				apply_random_resistance(0)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.FIRE
		41:
			if (_item.category == ItemCategory.SHIELD or
				_item.category == ItemCategory.CLOAK or
				_item.category == ItemCategory.HELM or
				_item.category == ItemCategory.HARD_ARMOUR ):
				_item.legendary_flags_2.set_bits(ItemFlags2.REFLECT)
			else:
				apply_random_resistance(0)


## Apply level-appropriate magic properties to a weapon
func apply_magic_to_weapon(level: int, power: int) -> void:
	var to_hit_1: int = Rng.roll_die(5) + get_bonus_value(5, level)
	var to_dam_1: int = Rng.roll_die(5) + get_bonus_value(5, level)
	var to_hit_2: int = get_bonus_value(10, level)
	var to_dam_2: int = get_bonus_value(10, level)
	_legendary_item_bias = 0
	# Add some bonus to hit and damage based on the item's power
	if power > 0:
		_item.bonus_to_hit += to_hit_1
		_item.bonus_damage += to_dam_1
		if power > 1:
			_item.bonus_to_hit += to_hit_2
			_item.bonus_damage += to_dam_2
	elif power < 0:
		_item.bonus_to_hit -= to_hit_1
		_item.bonus_damage -= to_dam_1
		if power < -1:
			_item.bonus_to_hit -= to_hit_2
			_item.bonus_damage -= to_dam_2
		if _item.bonus_to_hit + _item.bonus_damage < 0:
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)
	# Choose a random rare item type based on the item's category
	match _item.category:
		ItemCategory.DIGGING:
			if power > 1:
				_item.rare_index = RareItemId.WEAPON_OF_DIGGING
			elif power < -1:
				_item.type_specific_value = 0 - (5 + Rng.roll_die(5))
			elif power < 0:
				_item.type_specific_value = 0 - _item.type_specific_value
		ItemCategory.HAFTED, ItemCategory.POLEARM, ItemCategory.SWORD:
			if power > 1:
				match Rng.roll_die(40 if _item.category == ItemCategory.POLEARM else 42):
					1:
						_item.rare_index = RareItemId.WEAPON_ELDER_SIGN
						if Rng.roll_die(4) == 1:
							_item.legendary_flags_1.set_bits(ItemFlags1.BLOWS)
							if _item.type_specific_value > 2:
								_item.type_specific_value -= Rng.roll_die(2)
					2:
						_item.rare_index = RareItemId.WEAPON_DEFENDER
						if Rng.roll_die(3) == 1:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
						apply_random_resistance(Rng.roll_die(22) + 16)
					3:
						_item.rare_index = RareItemId.WEAPON_OF_VITRIOL
					4:
						_item.rare_index = RareItemId.AMMO_OF_SHOCKING
					5:
						_item.rare_index = RareItemId.WEAPON_OF_BURNING
					6:
						_item.rare_index = RareItemId.WEAPON_OF_FREEZING
					7, 8:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_ANIMAL
						if Rng.roll_die(100) <= 20:
							_item.rare_index = RareItemId.WEAPON_OF_ANIMAL_BANE
					9, 10:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_DRAGON
						apply_random_resistance(Rng.roll_die(12) + 4)
						if Rng.roll_die(100) <= 20:
							if Rng.roll_die(3) == 1:
								_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
							apply_random_resistance(Rng.roll_die(14) + 4)
							_item.rare_index = RareItemId.WEAPON_OF_DRAGON_BANE
					11, 12:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_EVIL
						if Rng.roll_die(100) <= 20:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
							_item.legendary_flags_3.set_bits(ItemFlags3.BLESSED)
							_item.rare_index = RareItemId.WEAPON_OF_EVIL_BANE
					13, 14:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_UNDEAD
						_item.legendary_flags_2.set_bits(ItemFlags2.HOLD_LIFE)
						if Rng.roll_die(100) <= 20:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_NETHER)
							_item.rare_index = RareItemId.WEAPON_OF_UNDEAD_BANE
					15, 16, 17:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_ORC
					18, 19, 20:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_TROLL
					21, 22, 23:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_GIANT
					24, 25, 26:
						_item.rare_index = RareItemId.WEAPON_OF_SLAY_DEMON
					27:
						_item.rare_index = RareItemId.WEAPON_OF_KADATH
						if Rng.roll_die(3) == 1:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
					28:
						_item.rare_index = RareItemId.WEAPON_BLESSED
					29, 30:
						_item.rare_index = RareItemId.WEAPON_OF_EXTRA_ATTACKS
					31, 32:
						_item.rare_index = RareItemId.WEAPON_VAMPIRIC
					33:
						_item.rare_index = RareItemId.WEAPON_OF_POISONING
					34:
						_item.rare_index = RareItemId.WEAPON_CHAOTIC
						apply_random_resistance(Rng.roll_die(34) + 4)
					35:
						create_legendary(false)
					36, 37:
						_item.rare_index = RareItemId.WEAPON_OF_SLAYING
						if Rng.roll_die(3) == 1:
							_item.damage_dice *= 2
						else:
							_item.damage_dice += 1
							_item.damage_dice_sides += 1
							while Rng.roll_die(_item.damage_dice) == 1:
								_item.damage_dice += 1
							while Rng.roll_die(_item.damage_dice_sides) == 1:
								_item.damage_dice_sides += 1
						if Rng.roll_die(5) == 1:
							_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_POISON)
						if _item.category == ItemCategory.SWORD and Rng.roll_die(3) == 1:
							_item.legendary_flags_1.set_bits(ItemFlags1.VORPAL)
					38, 39:
						_item.rare_index = RareItemId.WEAPON_PLANAR_WEAPON
						apply_random_resistance(Rng.roll_die(22) + 16)
						if Rng.roll_die(5) == 1:
							_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_DEMON)
					40:
						_item.rare_index = RareItemId.WEAPON_OF_LAW
						if Rng.roll_die(3) == 1:
							_item.legendary_flags_2.set_bits(ItemFlags2.HOLD_LIFE)
						if Rng.roll_die(3) == 1:
							_item.legendary_flags_1.set_bits(ItemFlags1.DEX)
						if Rng.roll_die(5) == 1:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
						apply_random_resistance(Rng.roll_die(22) + 16)
					_:
						if _item.category == ItemCategory.SWORD:
							_item.rare_index = RareItemId.WEAPON_OF_SHARPNESS
							_item.type_specific_value = get_bonus_value(5, level) + 1
						else:
							_item.rare_index = RareItemId.WEAPON_OF_EARTHQUAKES
							if Rng.roll_die(3) == 1:
								_item.legendary_flags_1.set_bits(ItemFlags1.BLOWS)
							_item.type_specific_value = get_bonus_value(3, level)

				while Rng.roll_die(10 * _item.damage_dice * _item.damage_dice_sides) == 1:
					_item.damage_dice += 1
				if _item.damage_dice > 9:
					_item.damage_dice = 9
			elif power < -1:
				if Rng.roll_die(DungeonLevel.MAX_DEPTH) <= level:
					_item.rare_index = RareItemId.WEAPON_OF_LENG
					if Rng.roll_die(6) == 1:
						_item.legendary_flags_3.set_bits(ItemFlags3.DREAD_CURSE)
		ItemCategory.BOW:
			if power > 1:
				match Rng.roll_die(21):
					1, 11:
						_item.rare_index = RareItemId.BOW_OF_EXTRA_MIGHT
						apply_random_resistance(Rng.roll_die(34) + 4)
					2, 12:
						_item.rare_index = RareItemId.BOW_OF_EXTRA_SHOTS
					3, 4, 5, 6, 13, 14, 15, 16:
						_item.rare_index = RareItemId.BOW_OF_VELOCITY
					7, 8, 9, 10, 17, 18, 19, 20:
						_item.rare_index = RareItemId.BOW_OF_ACCURACY
					_:
						create_legendary(false)
		ItemCategory.BOLT, ItemCategory.ARROW, ItemCategory.SHOT:
			if power > 1:
				match Rng.roll_die(12):
					1, 2, 3:
						_item.rare_index = RareItemId.AMMO_OF_WOUNDING
					4:
						_item.rare_index = RareItemId.AMMO_OF_FLAME
					5:
						_item.rare_index = RareItemId.AMMO_OF_FROST
					6, 7:
						_item.rare_index = RareItemId.AMMO_OF_HURT_ANIMAL
					8, 9:
						_item.rare_index = RareItemId.AMMO_OF_HURT_EVIL
					10:
						_item.rare_index = RareItemId.AMMO_OF_HURT_DRAGON
					11:
						_item.rare_index = RareItemId.AMMO_OF_SHOCKING
					12:
						_item.rare_index = RareItemId.AMMO_OF_SLAYING
						_item.damage_dice += 1
				while Rng.roll_die(10 * _item.damage_dice * _item.damage_dice_sides) == 1:
					_item.damage_dice += 1
				if _item.damage_dice > 9:
					_item.damage_dice = 9
			elif power < -1:
				if Rng.roll_die(DungeonLevel.MAX_DEPTH) <= level:
					_item.rare_index = RareItemId.AMMO_OF_BACKBITING


func get_bonus_value(maximum: int, level: int) -> int:
	if level > DungeonLevel.MAX_DEPTH - 1:
		level = DungeonLevel.MAX_DEPTH - 1
	var bonus: int = maximum * level / DungeonLevel.MAX_DEPTH
	var extra: int = maximum * level % DungeonLevel.MAX_DEPTH
	if Rng.roll_die(DungeonLevel.MAX_DEPTH) <= extra:
		bonus += 1
	var stand: int = maximum / 4
	extra = maximum % 4
	if Rng.roll_die(4) <= extra:
		stand += 1
	var value: int = Rng.random_normal(bonus, stand)
	return clampi(value, 0, maximum)


func apply_magic_to_armour(level: int, power: int) -> void:
	var to_ac_1: int = Rng.roll_die(5) + get_bonus_value(5, level)
	var to_ac_2: int = get_bonus_value(10, level)
	_legendary_item_bias = 0
	if power > 0:
		_item.bonus_armour_class += to_ac_1
		if power > 1:
			_item.bonus_armour_class += to_ac_2
	elif power < 0:
		_item.bonus_armour_class -= to_ac_1
		if power < -1:
			_item.bonus_armour_class -= to_ac_2
		if _item.bonus_armour_class < 0:
			_item.identify_flags.set_bits(IdentifyFlags.CURSED)
	match _item.category:
		ItemCategory.DRAGON_ARMOUR:
			if SaveGame.instance.dungeon_level != null:
				SaveGame.instance.dungeon_level.treasure_rating += 30
		ItemCategory.HARD_ARMOUR, ItemCategory.SOFT_ARMOUR:
			if power > 1:
				if (_item.category == ItemCategory.SOFT_ARMOUR and
					_item.sub_category == SoftArmourType.ROBE and Rng.roll_die(100) <= 10 ):
					_item.rare_index = RareItemId.ARMOUR_OF_PERMANENCE
				match Rng.roll_die(21):
					1, 2, 3, 4:
						_item.rare_index = RareItemId.ARMOUR_OF_RESIST_ACID
					5, 6, 7, 8:
						_item.rare_index = RareItemId.ARMOUR_OF_RESIST_LIGHTNING
					9, 10, 11, 12:
						_item.rare_index = RareItemId.ARMOUR_OF_RESIST_FIRE
					13, 14, 15, 16:
						_item.rare_index = RareItemId.ARMOUR_OF_RESIST_COLD
					17, 18:
						_item.rare_index = RareItemId.ARMOUR_OF_RESISTANCE
						if Rng.roll_die(4) == 1:
							_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
						apply_random_resistance(Rng.roll_die(22) + 16)
					20, 21:
						_item.rare_index = RareItemId.ARMOUR_OF_YITH
					_:
						create_legendary(false)
		ItemCategory.SHIELD:
			if _item.sub_category == ShieldType.DRAGON_SHIELD:
				if SaveGame.instance.dungeon_level != null:
					SaveGame.instance.dungeon_level.treasure_rating += 5
				apply_dragonscale_resistance()
			else:
				if power > 1:
					match Rng.roll_die(23):
						1, 11:
							_item.rare_index = RareItemId.SHIELD_OF_RESIST_ACID
						2, 3, 4, 12, 13, 14:
							_item.rare_index = RareItemId.SHIELD_OF_RESIST_LIGHTNING
						5, 6, 15, 16:
							_item.rare_index = RareItemId.SHIELD_OF_RESIST_FIRE
						7, 8, 9, 17, 18, 19:
							_item.rare_index = RareItemId.SHIELD_OF_RESIST_COLD
						10, 20:
							apply_random_resistance(Rng.roll_die(34) + 4)
							if Rng.roll_die(4) == 1:
								_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
							_item.rare_index = RareItemId.SHIELD_OF_RESISTANCE
						21, 22:
							_item.rare_index = RareItemId.SHIELD_OF_REFLECTION
						_:
							create_legendary(false)
		ItemCategory.GLOVES:
			if power > 1:
				if Rng.roll_die(20) == 1:
					create_legendary(false)
				else:
					match Rng.roll_die(10):
						1, 2, 3, 4:
							_item.rare_index = RareItemId.GLOVES_OF_FREE_ACTION
						5, 6, 7:
							_item.rare_index = RareItemId.GLOVES_OF_SLAYING
						8, 9:
							_item.rare_index = RareItemId.GLOVES_OF_AGILITY
						10:
							_item.rare_index = RareItemId.GLOVES_OF_POWER
							apply_random_resistance(Rng.roll_die(22) + 16)
			elif power < -1:
				match Rng.roll_die(2):
					1:
						_item.rare_index = RareItemId.GLOVES_OF_CLUMSINESS
					_:
						_item.rare_index = RareItemId.GLOVES_OF_WEAKNESS
		ItemCategory.BOOTS:
			if power > 1:
				if Rng.roll_die(20) == 1:
					create_legendary(false)
				else:
					match Rng.roll_die(24):
						1:
							_item.rare_index = RareItemId.BOOTS_OF_SPEED
						2, 3, 4, 5:
							_item.rare_index = RareItemId.BOOTS_OF_FREE_ACTION
						6, 7, 8, 9, 10, 11, 12, 13:
							_item.rare_index = RareItemId.BOOTS_OF_STEALTH
						_:
							_item.rare_index = RareItemId.BOOTS_WINGED
							if Rng.roll_die(2) == 1:
								apply_random_resistance(Rng.roll_die(22) + 16)
			elif power < -1:
				match Rng.roll_die(3):
					1:
						_item.rare_index = RareItemId.BOOTS_OF_NOISE
					2:
						_item.rare_index = RareItemId.BOOTS_OF_SLOWNESS
					3:
						_item.rare_index = RareItemId.BOOTS_OF_ANNOYANCE
		ItemCategory.CROWN:
			if power > 1:
				if Rng.roll_die(20) == 1:
					create_legendary(false)
				else:
					match Rng.roll_die(8):
						1:
							_item.rare_index = RareItemId.HAT_OF_THE_MAGI
							apply_random_resistance(Rng.roll_die(22) + 16)
						2:
							_item.rare_index = RareItemId.HAT_OF_MIGHT
							apply_random_resistance(Rng.roll_die(22) + 16)
						3:
							_item.rare_index = RareItemId.HAT_OF_TELEPATHY
						4:
							_item.rare_index = RareItemId.HAT_OF_REGENERATION
						5, 6:
							_item.rare_index = RareItemId.HAT_OF_LORDLINESS
							apply_random_resistance(Rng.roll_die(22) + 16)
						_:
							_item.rare_index = RareItemId.HAT_OF_SEEING
							if Rng.roll_die(3) == 1:
								_item.legendary_flags_3.set_bits(ItemFlags3.TELEPATHY)
			elif power < -1:
				match Rng.roll_die(7):
					1, 2:
						_item.rare_index = RareItemId.HAT_OF_STUPIDITY
					3, 4:
						_item.rare_index = RareItemId.HAT_OF_NAIVETY
					5:
						_item.rare_index = RareItemId.HAT_OF_UGLINESS
					6:
						_item.rare_index = RareItemId.HAT_OF_SICKLINESS
					7:
						_item.rare_index = RareItemId.HAT_OF_TELEPORTATION
		ItemCategory.HELM:
			if _item.sub_category == HelmType.DRAGON_HELM:
				if SaveGame.instance.dungeon_level != null:
					SaveGame.instance.dungeon_level.treasure_rating += 5
				apply_dragonscale_resistance()
			else:
				if power > 1:
					if Rng.roll_die(20) == 1:
						create_legendary(false)
					else:
						match Rng.roll_die(14):
							1, 2:
								_item.rare_index = RareItemId.HAT_OF_INTELLIGENCE
							3, 4:
								_item.rare_index = RareItemId.HAT_OF_WISDOM
							5, 6:
								_item.rare_index = RareItemId.HAT_OF_BEAUTY
							7, 8:
								_item.rare_index = RareItemId.HAT_OF_SEEING
								if Rng.roll_die(7) == 1:
									_item.legendary_flags_3.set_bits(ItemFlags3.TELEPATHY)
							9, 10:
								_item.rare_index = RareItemId.HAT_OF_LIGHT
							_:
								_item.rare_index = RareItemId.HAT_OF_INFRAVISION
				elif power < -1:
					match Rng.roll_die(7):
						1, 2:
							_item.rare_index = RareItemId.HAT_OF_STUPIDITY
						3, 4:
							_item.rare_index = RareItemId.HAT_OF_NAIVETY
						5:
							_item.rare_index = RareItemId.HAT_OF_UGLINESS
						6:
							_item.rare_index = RareItemId.HAT_OF_SICKLINESS
						7:
							_item.rare_index = RareItemId.HAT_OF_TELEPORTATION
		ItemCategory.CLOAK:
			if _item.sub_category == CloakType.ELVEN_CLOAK:
				_item.type_specific_value = Rng.roll_die(4)
			if power > 1:
				if Rng.roll_die(20) == 1:
					create_legendary(false)
				else:
					match Rng.roll_die(19):
						1, 2, 3, 4, 5, 6, 7, 8:
							_item.rare_index = RareItemId.CLOAK_OF_PROTECTION
						9, 10, 11, 12, 13, 14, 15, 16:
							_item.rare_index = RareItemId.CLOAK_OF_STEALTH
						17:
							_item.rare_index = RareItemId.CLOAK_OF_AMAN
						18:
							_item.rare_index = RareItemId.CLOAK_OF_ELECTRICITY
						_:
							_item.rare_index = RareItemId.CLOAK_OF_IMMOLATION
			elif power < -1:
				match Rng.roll_die(3):
					1:
						_item.rare_index = RareItemId.CLOAK_OF_IRRITATION
					2:
						_item.rare_index = RareItemId.CLOAK_OF_VULNERABILITY
					3:
						_item.rare_index = RareItemId.CLOAK_OF_ENVELOPING


func apply_magic_to_jewellrey(level: int, power: int) -> void:
	_legendary_item_bias = 0
	match _item.category:
		ItemCategory.RING:
			match _item.sub_category:
				RingType.ATTACKS:
					_item.type_specific_value = get_bonus_value(3, level)
					if _item.type_specific_value < 1:
						_item.type_specific_value = 1
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
				RingType.STR, RingType.CON, RingType.DEX, RingType.INT:
					_item.type_specific_value = 1 + get_bonus_value(5, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
				RingType.SPEED:
					_item.type_specific_value = Rng.roll_die(5) + get_bonus_value(5, level)
					while Rng.roll_die(100) <= 50:
						_item.type_specific_value += 1
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
					if SaveGame.instance.dungeon_level != null:
						SaveGame.instance.dungeon_level.treasure_rating += 25
				RingType.LORDLY:
					apply_random_resistance(Rng.roll_die(20) + 18)
					while Rng.roll_die(4) == 1:
						apply_random_resistance(Rng.roll_die(20) + 18)
					_item.bonus_armour_class = 10 + Rng.roll_die(5) + get_bonus_value(10, level)
					if SaveGame.instance.dungeon_level != null:
						SaveGame.instance.dungeon_level.treasure_rating += 5
				RingType.SEARCHING:
					_item.type_specific_value = 1 + get_bonus_value(5, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
				RingType.FLAMES, RingType.ACID, RingType.ICE:
					_item.bonus_armour_class = 5 + Rng.roll_die(5) + get_bonus_value(10, level)
				RingType.WEAKNESS, RingType.STUPIDITY:
					_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
					_item.identify_flags.set_bits(IdentifyFlags.CURSED)
					_item.type_specific_value = 0 - (1 + get_bonus_value(5, level))
				RingType.WOE:
					_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
					_item.identify_flags.set_bits(IdentifyFlags.CURSED)
					_item.bonus_armour_class = 0 - (5 + get_bonus_value(10, level))
					_item.type_specific_value = 0 - (1 + get_bonus_value(5, level))
				RingType.DAMAGE:
					_item.bonus_damage = 5 + Rng.roll_die(8) + get_bonus_value(10, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.bonus_damage = 0 - _item.bonus_damage
				RingType.ACCURACY:
					_item.bonus_to_hit = 5 + Rng.roll_die(8) + get_bonus_value(10, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.bonus_to_hit = 0 - _item.bonus_to_hit
				RingType.PROTECTION:
					_item.bonus_armour_class = 5 + Rng.roll_die(8) + get_bonus_value(10, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.bonus_armour_class = 0 - _item.bonus_armour_class
				RingType.SLAYING:
					_item.bonus_damage = Rng.roll_die(7) + get_bonus_value(10, level)
					_item.bonus_to_hit = Rng.roll_die(7) + get_bonus_value(10, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.bonus_to_hit = 0 - _item.bonus_to_hit
						_item.bonus_damage = 0 - _item.bonus_damage
		ItemCategory.AMULET:
			match _item.sub_category:
				AmuletType.BRILLIANCE, AmuletType.CHARISMA:
					_item.type_specific_value = 1 + get_bonus_value(5, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
				AmuletType.ANTI_MAGIC, AmuletType.ANTI_TELEPORTATION:
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
				AmuletType.RESISTANCE:
					if Rng.roll_die(3) == 1:
						apply_random_resistance(Rng.roll_die(34) + 4)
					if Rng.roll_die(5) == 1:
						_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
				AmuletType.SEARCHING:
					_item.type_specific_value = Rng.roll_die(5) + get_bonus_value(5, level)
					if power < 0:
						_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
						_item.identify_flags.set_bits(IdentifyFlags.CURSED)
						_item.type_specific_value = 0 - _item.type_specific_value
				AmuletType.THE_MAGI:
					_item.type_specific_value = Rng.roll_die(5) + get_bonus_value(5, level)
					_item.bonus_armour_class = Rng.roll_die(5) + get_bonus_value(5, level)
					if Rng.roll_die(3) == 1:
						_item.legendary_flags_3.set_bits(ItemFlags3.SLOW_DIGEST)
					if SaveGame.instance.dungeon_level != null:
						SaveGame.instance.dungeon_level.treasure_rating += 25
				AmuletType.DOOM:
					_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
					_item.identify_flags.set_bits(IdentifyFlags.CURSED)
					_item.type_specific_value = 0 - (Rng.roll_die(5) + get_bonus_value(5, level))
					_item.bonus_armour_class = 0 - (Rng.roll_die(5) + get_bonus_value(5, level))


func apply_magic_to_light_source(power: int) -> void:
	if _item.sub_category == LightType.TORCH:
		if _item.type_specific_value != 0:
			_item.type_specific_value = Rng.roll_die(_item.type_specific_value)
		return
	if _item.sub_category == LightType.LANTERN:
		if _item.type_specific_value != 0:
			_item.type_specific_value = Rng.roll_die(_item.type_specific_value)
		return
	if power < 0:
		match Rng.roll_die(2):
			1:
				_item.rare_index = RareItemId.ORB_OF_IRRITATION
				_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
				_item.identify_flags.set_bits(IdentifyFlags.CURSED)
			2:
				_item.rare_index = RareItemId.ORB_OF_INSTABILITY
				_item.identify_flags.set_bits(IdentifyFlags.BROKEN)
				_item.identify_flags.set_bits(IdentifyFlags.CURSED)
	elif power == 1:
		match Rng.roll_die(29):
			1:
				_item.rare_index = RareItemId.ORB_OF_FLAME
			2:
				_item.rare_index = RareItemId.ORB_OF_FROST
			3:
				_item.rare_index = RareItemId.ORB_OF_ACID
			4:
				_item.rare_index = RareItemId.ORB_OF_LIGHTNING
			5:
				_item.rare_index = RareItemId.ORB_OF_LIGHT
			6:
				_item.rare_index = RareItemId.ORB_OF_DARKNESS
			7:
				_item.rare_index = RareItemId.ORB_OF_LIFE
			8:
				_item.rare_index = RareItemId.ORB_OF_SIGHT
			9:
				_item.rare_index = RareItemId.ORB_OF_COURAGE
			10:
				_item.rare_index = RareItemId.ORB_OF_VENOM
			11:
				_item.rare_index = RareItemId.ORB_OF_CLARITY
			12:
				_item.rare_index = RareItemId.ORB_OF_SOUND
			13:
				_item.rare_index = RareItemId.ORB_OF_CHAOS
			14:
				_item.rare_index = RareItemId.ORB_OF_SHARDS
			15:
				_item.rare_index = RareItemId.ORB_OF_UNLIFE
			16:
				_item.rare_index = RareItemId.ORB_OF_STABILITY
			17:
				_item.rare_index = RareItemId.ORB_OF_MAGIC
			18:
				_item.rare_index = RareItemId.ORB_OF_FREEDOM
			19:
				_item.rare_index = RareItemId.ORB_OF_STRENGTH
			20:
				_item.rare_index = RareItemId.ORB_OF_INTELLIGENCE
			21:
				_item.rare_index = RareItemId.ORB_OF_WISDOM
			22:
				_item.rare_index = RareItemId.ORB_OF_DEXTERITY
			23:
				_item.rare_index = RareItemId.ORB_OF_CONSTITUTION
			24:
				_item.rare_index = RareItemId.ORB_OF_CHARISMA
			25:
				_item.rare_index = RareItemId.ORB_OF_LIGHTNESS
			26:
				_item.rare_index = RareItemId.ORB_OF_INSIGHT
			27:
				_item.rare_index = RareItemId.ORB_OF_THE_MIND
			28:
				_item.rare_index = RareItemId.ORB_OF_SUSTENANCE
			29:
				_item.rare_index = RareItemId.ORB_OF_HEALTH
	elif power == 2:
		_item.rare_index = RareItemId.ORB_OF_POWER
		for i: int in 3:
			match Rng.roll_die(29):
				1:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_DARK)
				2:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_LIGHT)
				3:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_BLIND)
				4:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_FEAR)
				5:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_ACID)
				6:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_ELEC)
				7:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_FIRE)
				8:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_COLD)
				9:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_POIS)
				10:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_CONF)
				11:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_SOUND)
				12:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_SHARDS)
				13:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_NETHER)
				14:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_NEXUS)
				15:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_CHAOS)
				16:
					_item.legendary_flags_2.set_bits(ItemFlags2.RES_DISEN)
				17:
					_item.legendary_flags_2.set_bits(ItemFlags2.FREE_ACT)
				18:
					_item.legendary_flags_2.set_bits(ItemFlags2.HOLD_LIFE)
				19:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_STR)
				20:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_INT)
				21:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_WIS)
				22:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_DEX)
				23:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CON)
				24:
					_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CHA)
				25:
					_item.legendary_flags_3.set_bits(ItemFlags3.FEATHER)
				26:
					_item.legendary_flags_3.set_bits(ItemFlags3.SEE_INVIS)
				27:
					_item.legendary_flags_3.set_bits(ItemFlags3.TELEPATHY)
				28:
					_item.legendary_flags_3.set_bits(ItemFlags3.SLOW_DIGEST)
				29:
					_item.legendary_flags_3.set_bits(ItemFlags3.REGEN)


func apply_magic_to_misc_item() -> void:
	match _item.category:
		ItemCategory.WAND:
			charge_wand()
		ItemCategory.STAFF:
			charge_staff()
		ItemCategory.CHEST:
			if _item.item_type.level <= 0:
				return
			_item.type_specific_value = Rng.roll_die(_item.item_type.level)
			if _item.type_specific_value > 55:
				_item.type_specific_value = 54 + Rng.roll_die(5)


func charge_staff() -> void:
	match _item.sub_category:
		StaffType.DARKNESS:
			_item.type_specific_value = Rng.roll_die(8) + 8
		StaffType.SLOWNESS:
			_item.type_specific_value = Rng.roll_die(8) + 8
		StaffType.HASTE_MONSTERS:
			_item.type_specific_value = Rng.roll_die(8) + 8
		StaffType.SUMMONING:
			_item.type_specific_value = Rng.roll_die(3) + 1
		StaffType.TELEPORTATION:
			_item.type_specific_value = Rng.roll_die(4) + 5
		StaffType.IDENTIFY:
			_item.type_specific_value = Rng.roll_die(15) + 5
		StaffType.REMOVE_CURSE:
			_item.type_specific_value = Rng.roll_die(3) + 4
		StaffType.STARLIGHT:
			_item.type_specific_value = Rng.roll_die(5) + 6
		StaffType.LIGHT:
			_item.type_specific_value = Rng.roll_die(20) + 8
		StaffType.MAPPING:
			_item.type_specific_value = Rng.roll_die(5) + 5
		StaffType.DETECT_GOLD:
			_item.type_specific_value = Rng.roll_die(20) + 8
		StaffType.DETECT_ITEM:
			_item.type_specific_value = Rng.roll_die(15) + 6
		StaffType.DETECT_TRAP:
			_item.type_specific_value = Rng.roll_die(5) + 6
		StaffType.DETECT_DOOR:
			_item.type_specific_value = Rng.roll_die(8) + 6
		StaffType.DETECT_INVIS:
			_item.type_specific_value = Rng.roll_die(15) + 8
		StaffType.DETECT_EVIL:
			_item.type_specific_value = Rng.roll_die(15) + 8
		StaffType.CURE_LIGHT:
			_item.type_specific_value = Rng.roll_die(5) + 6
		StaffType.CURING:
			_item.type_specific_value = Rng.roll_die(3) + 4
		StaffType.HEALING:
			_item.type_specific_value = Rng.roll_die(2) + 1
		StaffType.THE_MAGI:
			_item.type_specific_value = Rng.roll_die(2) + 2
		StaffType.SLEEP_MONSTERS:
			_item.type_specific_value = Rng.roll_die(5) + 6
		StaffType.SLOW_MONSTERS:
			_item.type_specific_value = Rng.roll_die(5) + 6
		StaffType.SPEED:
			_item.type_specific_value = Rng.roll_die(3) + 4
		StaffType.PROBING:
			_item.type_specific_value = Rng.roll_die(6) + 2
		StaffType.DISPEL_EVIL:
			_item.type_specific_value = Rng.roll_die(3) + 4
		StaffType.POWER:
			_item.type_specific_value = Rng.roll_die(3) + 1
		StaffType.HOLINESS:
			_item.type_specific_value = Rng.roll_die(2) + 2
		StaffType.CARNAGE:
			_item.type_specific_value = Rng.roll_die(2) + 1
		StaffType.EARTHQUAKES:
			_item.type_specific_value = Rng.roll_die(5) + 3
		StaffType.DESTRUCTION:
			_item.type_specific_value = Rng.roll_die(3) + 1


func charge_wand() -> void:
	match _item.sub_category:
		WandType.HEAL_MONSTER:
			_item.type_specific_value = Rng.roll_die(20) + 8
		WandType.HASTE_MONSTER:
			_item.type_specific_value = Rng.roll_die(20) + 8
		WandType.CLONE_MONSTER:
			_item.type_specific_value = Rng.roll_die(5) + 3
		WandType.TELEPORT_AWAY:
			_item.type_specific_value = Rng.roll_die(5) + 6
		WandType.DISARMING:
			_item.type_specific_value = Rng.roll_die(5) + 4
		WandType.TRAP_DOOR_DEST:
			_item.type_specific_value = Rng.roll_die(8) + 6
		WandType.STONE_TO_MUD:
			_item.type_specific_value = Rng.roll_die(8) + 3
		WandType.LIGHT:
			_item.type_specific_value = Rng.roll_die(10) + 6
		WandType.SLEEP_MONSTER:
			_item.type_specific_value = Rng.roll_die(15) + 8
		WandType.SLOW_MONSTER:
			_item.type_specific_value = Rng.roll_die(10) + 6
		WandType.CONFUSE_MONSTER:
			_item.type_specific_value = Rng.roll_die(12) + 6
		WandType.FEAR_MONSTER:
			_item.type_specific_value = Rng.roll_die(5) + 3
		WandType.DRAIN_LIFE:
			_item.type_specific_value = Rng.roll_die(3) + 3
		WandType.POLYMORPH:
			_item.type_specific_value = Rng.roll_die(8) + 6
		WandType.STINKING_CLOUD:
			_item.type_specific_value = Rng.roll_die(8) + 6
		WandType.MAGIC_MISSILE:
			_item.type_specific_value = Rng.roll_die(10) + 6
		WandType.ACID_BOLT:
			_item.type_specific_value = Rng.roll_die(8) + 6
		WandType.CHARM_MONSTER:
			_item.type_specific_value = Rng.roll_die(6) + 2
		WandType.FIRE_BOLT:
			_item.type_specific_value = Rng.roll_die(8) + 6
		WandType.COLD_BOLT:
			_item.type_specific_value = Rng.roll_die(5) + 6
		WandType.ACID_BALL:
			_item.type_specific_value = Rng.roll_die(5) + 2
		WandType.ELEC_BALL:
			_item.type_specific_value = Rng.roll_die(8) + 4
		WandType.FIRE_BALL:
			_item.type_specific_value = Rng.roll_die(4) + 2
		WandType.COLD_BALL:
			_item.type_specific_value = Rng.roll_die(6) + 2
		WandType.WONDER:
			_item.type_specific_value = Rng.roll_die(15) + 8
		WandType.ANNIHILATION:
			_item.type_specific_value = Rng.roll_die(2) + 1
		WandType.DRAGON_FIRE:
			_item.type_specific_value = Rng.roll_die(3) + 1
		WandType.DRAGON_COLD:
			_item.type_specific_value = Rng.roll_die(3) + 1
		WandType.DRAGON_BREATH:
			_item.type_specific_value = Rng.roll_die(3) + 1
		WandType.SHARD:
			_item.type_specific_value = Rng.roll_die(2) + 1


func apply_dragonscale_resistance() -> void:
	# Add a rendom resistance
	_legendary_item_bias = 0
	if Rng.roll_die(4) == 1:
		apply_random_resistance(Rng.roll_die(14) + 4)
	else:
		apply_random_resistance(Rng.roll_die(22) + 16)
	# Repeat a random number of extra times
	while Rng.roll_die(2) == 1:
		_legendary_item_bias = 0
		if Rng.roll_die(4) == 1:
			apply_random_resistance(Rng.roll_die(14) + 4)
		else:
			apply_random_resistance(Rng.roll_die(22) + 16)


func create_legendary(from_scroll: bool) -> bool:
	var has_type_specific_value: bool = false
	var powers: int = Rng.roll_die(5) + 1
	var max_type: int = 7 if _item.category < ItemCategory.BOOTS else 5
	var cursed_artifact: bool = false
	var warrior_artifact_bias: int = 0
	_legendary_item_bias = 0
	if from_scroll and Rng.roll_die(4) == 1:
		_legendary_item_bias = SaveGame.instance.player.profession.legendary_item_bias
		warrior_artifact_bias = SaveGame.instance.player.profession.warrior_artifact_bias
	if Rng.roll_die(100) <= warrior_artifact_bias and from_scroll:
		_legendary_item_bias = LegendaryItemBias.WARRIOR
	if not from_scroll and Rng.roll_die(ARTIFACT_CURSE_CHANCE) == 1:
		cursed_artifact = true
	while Rng.roll_die(powers) == 1 or Rng.roll_die(7) == 1 or Rng.roll_die(10) == 1:
		powers += 1
	if not cursed_artifact and Rng.roll_die(WEIRD_LUCK) == 1:
		powers *= 2
	if cursed_artifact:
		powers /= 2
	while powers != 0:
		powers -= 1
		match Rng.roll_die(max_type):
			1, 2:
				apply_random_bonuses()
				has_type_specific_value = true
			3, 4:
				apply_random_resistance(0)
			5:
				apply_random_misc_power()
			6, 7:
				apply_random_slaying()
			_:
				powers += 1
	if has_type_specific_value:
		if _item.legendary_flags_1.is_set(ItemFlags1.BLOWS):
			_item.type_specific_value = Rng.roll_die(2) + 1
		else:
			_item.type_specific_value += 1
			while (_item.type_specific_value < Rng.roll_die(5) or
				Rng.roll_die(_item.type_specific_value) == 1 ):
				_item.type_specific_value += 1
		if _item.type_specific_value > 4 and Rng.roll_die(WEIRD_LUCK) != 1:
			_item.type_specific_value = 4
	if _item.category >= ItemCategory.BOOTS:
		_item.bonus_armour_class += Rng.roll_die(1 if _item.bonus_armour_class > 19 else (20 - _item.bonus_armour_class))
	else:
		_item.bonus_to_hit += Rng.roll_die(1 if _item.bonus_to_hit > 19 else (20 - _item.bonus_to_hit))
		_item.bonus_damage += Rng.roll_die(1 if _item.bonus_damage > 19 else (20 - _item.bonus_damage))
	_item.legendary_flags_3.set_bits(ItemFlags3.IGNORE_ACID | ItemFlags3.IGNORE_ELEC | ItemFlags3.IGNORE_FIRE | ItemFlags3.IGNORE_COLD)
	if cursed_artifact:
		curse_legendary()
	if not cursed_artifact and Rng.roll_die((ACTIVATION_CHANCE * 2) if _item.category >= ItemCategory.BOOTS else ACTIVATION_CHANCE) == 1:
		_item.bonus_power_sub_type = 0
		give_activation_power()
	if from_scroll:
		_item.identify_fully()
		_item.identify_flags.set_bits(IdentifyFlags.STOREBOUGHT | IdentifyFlags.MENTAL)
		_item.become_flavour_aware()
		_item.become_known()
	_item.legendary_name = get_legendary_item_name()
	return true


func get_legendary_item_name() -> String:
	var name_starts: Array[String] = [
		"Wolf",
		"Tooth",
		"Knee",
		"Foe",
		"Enemy",
		"Nose",
		"Brain",
		"Head",
		"Bowel",
		"Evil",
		"Slime",
		"Blood",
		"Beast",
		"Foul",
		"Poison",
		"Venom",
		"Ichor",
		"Skin",
		"Dark",
		"Wicked",
		"Anger",
		"Fury",
		"Hate",
		"Spine",
		"Bone",
		"Rotten",
		"Beast",
		"Bite",
		"Sword",
		"Armour",
		"Shield",
		"Fetid",
		"Rank",
		"Back",
		"Neck",
		"Face",
		"Eye",
		"King",
		"Queen",
		"God",
		"Devil",
		"Angel",
		"Fiend",
		"Friend",
		"Self",
		"Chest",
		"Groin",
		"Table",
		"Rock",
		"Liver",
		"Heart",
		"Spleen",
	]
	var name_ends: Array[String] = [
		"biter",
		"breaker",
		"chopper",
		"masher",
		"bane",
		"kicker",
		"mangler",
		"crusher",
		"howler",
		"sealer",
		"stopper",
		"stomper",
		"halter",
		"burster",
		"snapper",
		"render",
		"buster",
		"puncher",
		"ripper",
		"tearer",
		"finder",
		"howler",
		"screamer",
		"lighter",
		"burster",
		"cracker",
		"shatterer",
		"violator",
		"ruiner",
		"humiliater",
		"scatterer",
		"spiller",
		"wrecker",
		"basher",
		"spoiler",
		"chewer",
		"blighter",
		"fouler",
		"stinger",
		"bringer",
		"kisser",
		"hater",
		"spawn",
		"salve",
		"slice",
	]
	return name_starts[Rng.roll_die(name_starts.size()) - 1] + name_ends[Rng.roll_die(name_ends.size()) - 1]


func give_activation_power() -> void:
	var type: int = 0
	var chance: int = 0
	if _legendary_item_bias != 0:
		if _legendary_item_bias == LegendaryItemBias.ELECTRICITY:
			if Rng.roll_die(3) != 1:
				type = LegendaryPower.BO_ELEC_1
			elif Rng.roll_die(5) != 1:
				type = LegendaryPower.BA_ELEC_2
			else:
				type = LegendaryPower.BA_ELEC_3
			chance = 101
		elif _legendary_item_bias == LegendaryItemBias.POISON:
			type = LegendaryPower.BA_POIS_1
			chance = 101
		elif _legendary_item_bias == LegendaryItemBias.FIRE:
			if Rng.roll_die(3) != 1:
				type = LegendaryPower.BO_FIRE_1
			elif Rng.roll_die(5) != 1:
				type = LegendaryPower.BO_FIRE_1
			else:
				type = LegendaryPower.BA_FIRE_2
			chance = 101
		elif _legendary_item_bias == LegendaryItemBias.COLD:
			chance = 101
			if Rng.roll_die(3) != 1:
				type = LegendaryPower.BO_COLD_1
			elif Rng.roll_die(3) != 1:
				type = LegendaryPower.BA_COLD_1
			elif Rng.roll_die(3) != 1:
				type = LegendaryPower.BA_COLD_2
			else:
				type = LegendaryPower.BA_COLD_3
		elif _legendary_item_bias == LegendaryItemBias.CHAOS:
			chance = 50
			type = LegendaryPower.SUMMON_DEMON if Rng.roll_die(6) == 1 else LegendaryPower.CALL_CHAOS
		elif _legendary_item_bias == LegendaryItemBias.PRIESTLY:
			chance = 101
			if Rng.roll_die(13) == 1:
				type = LegendaryPower.CHARM_UNDEAD
			elif Rng.roll_die(12) == 1:
				type = LegendaryPower.BANISH_EVIL
			elif Rng.roll_die(11) == 1:
				type = LegendaryPower.DISP_EVIL
			elif Rng.roll_die(10) == 1:
				type = LegendaryPower.PROT_EVIL
			elif Rng.roll_die(9) == 1:
				type = LegendaryPower.CURE_1000
			elif Rng.roll_die(8) == 1:
				type = LegendaryPower.CURE_700
			elif Rng.roll_die(7) == 1:
				type = LegendaryPower.REST_ALL
			elif Rng.roll_die(6) == 1:
				type = LegendaryPower.REST_LIFE
			else:
				type = LegendaryPower.CURE_M_W
		elif _legendary_item_bias == LegendaryItemBias.NECROMANTIC:
			chance = 101
			if Rng.roll_die(66) == 1:
				type = LegendaryPower.WRAITH
			elif Rng.roll_die(13) == 1:
				type = LegendaryPower.DISP_GOOD
			elif Rng.roll_die(9) == 1:
				type = LegendaryPower.MASS_GENO
			elif Rng.roll_die(8) == 1:
				type = LegendaryPower.CARNAGE
			elif Rng.roll_die(13) == 1:
				type = LegendaryPower.SUMMON_UNDEAD
			elif Rng.roll_die(9) == 1:
				type = LegendaryPower.VAMPIRE_2
			elif Rng.roll_die(6) == 1:
				type = LegendaryPower.CHARM_UNDEAD
			else:
				type = LegendaryPower.VAMPIRE_1
		elif _legendary_item_bias == LegendaryItemBias.LAW:
			chance = 101
			if Rng.roll_die(8) == 1:
				type = LegendaryPower.BANISH_EVIL
			elif Rng.roll_die(4) == 1:
				type = LegendaryPower.DISP_EVIL
			else:
				type = LegendaryPower.PROT_EVIL
		elif _legendary_item_bias == LegendaryItemBias.ROGUE:
			chance = 101
			if Rng.roll_die(50) == 1:
				type = LegendaryPower.SPEED
			elif Rng.roll_die(4) == 1:
				type = LegendaryPower.SLEEP
			elif Rng.roll_die(3) == 1:
				type = LegendaryPower.DETECT_ALL
			elif Rng.roll_die(8) == 1:
				type = LegendaryPower.ID_FULL
			else:
				type = LegendaryPower.ID_PLAIN
		elif _legendary_item_bias == LegendaryItemBias.MAGE:
			chance = 66
			if Rng.roll_die(20) == 1:
				type = LegendaryPower.SUMMON_ELEMENTAL
			elif Rng.roll_die(10) == 1:
				type = LegendaryPower.SUMMON_PHANTOM
			elif Rng.roll_die(5) == 1:
				type = LegendaryPower.RUNE_EXPLO
			else:
				type = LegendaryPower.ESP
		elif _legendary_item_bias == LegendaryItemBias.WARRIOR:
			chance = 80
			type = LegendaryPower.INVULN if Rng.roll_die(100) == 1 else LegendaryPower.BERSERK
		elif _legendary_item_bias == LegendaryItemBias.RANGER:
			chance = 101
			if Rng.roll_die(20) == 1:
				type = LegendaryPower.CHARM_ANIMALS
			elif Rng.roll_die(7) == 1:
				type = LegendaryPower.SUMMON_ANIMAL
			elif Rng.roll_die(6) == 1:
				type = LegendaryPower.CHARM_ANIMAL
			elif Rng.roll_die(4) == 1:
				type = LegendaryPower.RESIST_ALL
			elif Rng.roll_die(3) == 1:
				type = LegendaryPower.SATIATE
			else:
				type = LegendaryPower.CURE_POISON
	while type == 0 or Rng.roll_die(100) >= chance:
		type = Rng.roll_die(255)
		match type:
			LegendaryPower.SUNLIGHT, LegendaryPower.BO_MISS_1, LegendaryPower.BA_POIS_1, LegendaryPower.BO_ELEC_1:
				chance = 101
			LegendaryPower.BO_ACID_1, LegendaryPower.BO_COLD_1, LegendaryPower.BO_FIRE_1, LegendaryPower.CONFUSE:
				chance = 101
			LegendaryPower.SLEEP, LegendaryPower.QUAKE, LegendaryPower.CURE_L_W, LegendaryPower.CURE_M_W:
				chance = 101
			LegendaryPower.CURE_POISON, LegendaryPower.BERSERK, LegendaryPower.LIGHT, LegendaryPower.MAP_LIGHT:
				chance = 101
			LegendaryPower.DEST_DOOR, LegendaryPower.STONE_MUD, LegendaryPower.TELEPORT:
				chance = 101
			LegendaryPower.BA_COLD_2, LegendaryPower.BA_FIRE_1, LegendaryPower.DRAIN_1, LegendaryPower.TELE_AWAY:
				chance = 85
			LegendaryPower.ESP, LegendaryPower.RESIST_ALL, LegendaryPower.DETECT_ALL, LegendaryPower.RECALL:
				chance = 85
			LegendaryPower.SATIATE, LegendaryPower.RECHARGE:
				chance = 85
			LegendaryPower.TERROR, LegendaryPower.PROT_EVIL, LegendaryPower.ID_PLAIN:
				chance = 75
			LegendaryPower.DRAIN_2, LegendaryPower.VAMPIRE_1, LegendaryPower.BO_MISS_2, LegendaryPower.BA_FIRE_1, LegendaryPower.REST_LIFE:
				chance = 66
			LegendaryPower.BA_COLD_3, LegendaryPower.BA_ELEC_3, LegendaryPower.WHIRLWIND, LegendaryPower.VAMPIRE_2, LegendaryPower.CHARM_ANIMAL:
				chance = 50
			LegendaryPower.SUMMON_ANIMAL:
				chance = 40
			LegendaryPower.DISP_EVIL, LegendaryPower.BA_MISS_3, LegendaryPower.DISP_GOOD, LegendaryPower.BANISH_EVIL:
				chance = 33
			LegendaryPower.CARNAGE, LegendaryPower.MASS_GENO, LegendaryPower.CHARM_UNDEAD, LegendaryPower.CHARM_OTHER:
				chance = 33
			LegendaryPower.SUMMON_PHANTOM, LegendaryPower.RESIST_ALL, LegendaryPower.RUNE_EXPLO:
				chance = 33
			LegendaryPower.CALL_CHAOS, LegendaryPower.SHARD, LegendaryPower.CHARM_ANIMALS, LegendaryPower.CHARM_OTHER, LegendaryPower.SUMMON_ELEMENTAL:
				chance = 25
			LegendaryPower.CURE_700, LegendaryPower.SPEED, LegendaryPower.ID_FULL, LegendaryPower.RUNE_PROT:
				chance = 25
			LegendaryPower.CURE_1000, LegendaryPower.XTRA_SPEED, LegendaryPower.DETECT_XTRA, LegendaryPower.DIM_DOOR:
				chance = 10
			LegendaryPower.SUMMON_UNDEAD, LegendaryPower.SUMMON_DEMON, LegendaryPower.WRAITH, LegendaryPower.INVULN, LegendaryPower.ALCHEMY:
				chance = 5
			_:
				chance = 0
	_item.bonus_power_sub_type = type
	_item.legendary_flags_3.set_bits(ItemFlags3.ACTIVATE)
	_item.recharge_time_left = 0


func apply_random_bonuses() -> void:
	var this_type: int = 23 if _item.category < ItemCategory.BOOTS else 19
	if _legendary_item_bias == LegendaryItemBias.WARRIOR:
		if _item.legendary_flags_1.is_clear(ItemFlags1.STR):
			_item.legendary_flags_1.set_bits(ItemFlags1.STR)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.CON):
			_item.legendary_flags_1.set_bits(ItemFlags1.CON)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.DEX):
			_item.legendary_flags_1.set_bits(ItemFlags1.DEX)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.MAGE:
		if _item.legendary_flags_1.is_clear(ItemFlags1.INT):
			_item.legendary_flags_1.set_bits(ItemFlags1.INT)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.PRIESTLY:
		if _item.legendary_flags_1.is_clear(ItemFlags1.WIS):
			_item.legendary_flags_1.set_bits(ItemFlags1.WIS)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.RANGER:
		if _item.legendary_flags_1.is_clear(ItemFlags1.CON):
			_item.legendary_flags_1.set_bits(ItemFlags1.CON)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.DEX):
			_item.legendary_flags_1.set_bits(ItemFlags1.DEX)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.STR):
			_item.legendary_flags_1.set_bits(ItemFlags1.STR)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.ROGUE:
		if _item.legendary_flags_1.is_clear(ItemFlags1.STEALTH):
			_item.legendary_flags_1.set_bits(ItemFlags1.STEALTH)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.SEARCH):
			_item.legendary_flags_1.set_bits(ItemFlags1.SEARCH)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.STRENGTH:
		if _item.legendary_flags_1.is_clear(ItemFlags1.STR):
			_item.legendary_flags_1.set_bits(ItemFlags1.STR)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.WISDOM:
		if _item.legendary_flags_1.is_clear(ItemFlags1.WIS):
			_item.legendary_flags_1.set_bits(ItemFlags1.WIS)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.INTELLIGENCE:
		if _item.legendary_flags_1.is_clear(ItemFlags1.INT):
			_item.legendary_flags_1.set_bits(ItemFlags1.INT)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.DEXTERITY:
		if _item.legendary_flags_1.is_clear(ItemFlags1.DEX):
			_item.legendary_flags_1.set_bits(ItemFlags1.DEX)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.CONSTITUTION:
		if _item.legendary_flags_1.is_clear(ItemFlags1.CON):
			_item.legendary_flags_1.set_bits(ItemFlags1.CON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.CHARISMA:
		if _item.legendary_flags_1.is_clear(ItemFlags1.CHA):
			_item.legendary_flags_1.set_bits(ItemFlags1.CHA)
			if Rng.roll_die(2) == 1:
				return
	match Rng.roll_die(this_type):
		1, 2:
			_item.legendary_flags_1.set_bits(ItemFlags1.STR)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.STRENGTH
			elif _legendary_item_bias == 0 and Rng.roll_die(7) == 1:
				_legendary_item_bias = LegendaryItemBias.WARRIOR
		3, 4:
			_item.legendary_flags_1.set_bits(ItemFlags1.INT)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.INTELLIGENCE
			elif _legendary_item_bias == 0 and Rng.roll_die(7) == 1:
				_legendary_item_bias = LegendaryItemBias.MAGE
		5, 6:
			_item.legendary_flags_1.set_bits(ItemFlags1.WIS)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.WISDOM
			elif _legendary_item_bias == 0 and Rng.roll_die(7) == 1:
				_legendary_item_bias = LegendaryItemBias.PRIESTLY
		7, 8:
			_item.legendary_flags_1.set_bits(ItemFlags1.DEX)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.DEXTERITY
			elif _legendary_item_bias == 0 and Rng.roll_die(7) == 1:
				_legendary_item_bias = LegendaryItemBias.ROGUE
		9, 10:
			_item.legendary_flags_1.set_bits(ItemFlags1.CON)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.CONSTITUTION
			elif _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
				_legendary_item_bias = LegendaryItemBias.RANGER
		11, 12:
			_item.legendary_flags_1.set_bits(ItemFlags1.CHA)
			if _legendary_item_bias == 0 and Rng.roll_die(13) != 1:
				_legendary_item_bias = LegendaryItemBias.CHARISMA
		13, 14:
			_item.legendary_flags_1.set_bits(ItemFlags1.STEALTH)
			if _legendary_item_bias == 0 and Rng.roll_die(3) == 1:
				_legendary_item_bias = LegendaryItemBias.ROGUE
		15, 16:
			_item.legendary_flags_1.set_bits(ItemFlags1.SEARCH)
			if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
				_legendary_item_bias = LegendaryItemBias.RANGER
		17, 18:
			_item.legendary_flags_1.set_bits(ItemFlags1.INFRA)
		19:
			_item.legendary_flags_1.set_bits(ItemFlags1.SPEED)
			if _legendary_item_bias == 0 and Rng.roll_die(11) == 1:
				_legendary_item_bias = LegendaryItemBias.ROGUE
		20, 21:
			_item.legendary_flags_1.set_bits(ItemFlags1.TUNNEL)
		22, 23:
			if _item.category == ItemCategory.BOW:
				apply_random_bonuses()
			else:
				_item.legendary_flags_1.set_bits(ItemFlags1.BLOWS)
				if _legendary_item_bias == 0 and Rng.roll_die(11) == 1:
					_legendary_item_bias = LegendaryItemBias.WARRIOR


func apply_random_misc_power() -> void:
	if _legendary_item_bias == LegendaryItemBias.RANGER:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_CON):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.STRENGTH:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_STR):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_STR)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.WISDOM:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_WIS):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_WIS)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.INTELLIGENCE:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_INT):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_INT)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.DEXTERITY:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_DEX):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_DEX)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.CONSTITUTION:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_CON):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.CHARISMA:
		if _item.legendary_flags_2.is_clear(ItemFlags2.SUST_CHA):
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CHA)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.CHAOS:
		if _item.legendary_flags_3.is_clear(ItemFlags3.TELEPORT):
			_item.legendary_flags_3.set_bits(ItemFlags3.TELEPORT)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.FIRE:
		if _item.legendary_flags_3.is_clear(ItemFlags3.LIGHT_SOURCE):
			_item.legendary_flags_3.set_bits(ItemFlags3.LIGHT_SOURCE)
	match Rng.roll_die(31):
		1:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_STR)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.STRENGTH
		2:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_INT)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.INTELLIGENCE
		3:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_WIS)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.WISDOM
		4:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_DEX)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.DEXTERITY
		5:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CON)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.CONSTITUTION
		6:
			_item.legendary_flags_2.set_bits(ItemFlags2.SUST_CHA)
			if _legendary_item_bias == 0:
				_legendary_item_bias = LegendaryItemBias.CHARISMA
		7, 8, 14:
			_item.legendary_flags_2.set_bits(ItemFlags2.FREE_ACT)
		9:
			_item.legendary_flags_2.set_bits(ItemFlags2.HOLD_LIFE)
			if _legendary_item_bias == 0 and Rng.roll_die(5) == 1:
				_legendary_item_bias = LegendaryItemBias.PRIESTLY
			elif _legendary_item_bias == 0 and Rng.roll_die(6) == 1:
				_legendary_item_bias = LegendaryItemBias.NECROMANTIC
		10, 11:
			_item.legendary_flags_3.set_bits(ItemFlags3.LIGHT_SOURCE)
		12, 13:
			_item.legendary_flags_3.set_bits(ItemFlags3.FEATHER)
		15, 16, 17:
			_item.legendary_flags_3.set_bits(ItemFlags3.SEE_INVIS)
		18:
			_item.legendary_flags_3.set_bits(ItemFlags3.TELEPATHY)
			if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
				_legendary_item_bias = LegendaryItemBias.MAGE
		19, 20:
			_item.legendary_flags_3.set_bits(ItemFlags3.SLOW_DIGEST)
		22:
			_item.legendary_flags_3.set_bits(ItemFlags3.REGEN)
		23:
			_item.legendary_flags_3.set_bits(ItemFlags3.TELEPORT)
		24, 25, 26:
			if _item.category >= ItemCategory.BOOTS:
				apply_random_misc_power()
			else:
				_item.legendary_flags_3.set_bits(ItemFlags3.SHOW_MODS)
				_item.bonus_armour_class = 4 + Rng.roll_die(11)
		27, 28, 29:
			_item.legendary_flags_3.set_bits(ItemFlags3.SHOW_MODS)
			_item.bonus_to_hit += 4 + Rng.roll_die(11)
			_item.bonus_damage += 4 + Rng.roll_die(11)
		30:
			_item.legendary_flags_3.set_bits(ItemFlags3.NO_MAGIC)
		31:
			_item.legendary_flags_3.set_bits(ItemFlags3.NO_TELE)


func apply_random_slaying() -> void:
	if _legendary_item_bias == LegendaryItemBias.CHAOS and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.CHAOTIC):
			_item.legendary_flags_1.set_bits(ItemFlags1.CHAOTIC)
			if Rng.roll_die(2) == 1:
				return
	elif (_legendary_item_bias == LegendaryItemBias.PRIESTLY and
		(_item.category == ItemCategory.SWORD or
			_item.category == ItemCategory.POLEARM ) and
		_item.legendary_flags_3.is_clear(ItemFlags3.BLESSED) ):
		_item.legendary_flags_3.set_bits(ItemFlags3.BLESSED)
	elif _legendary_item_bias == LegendaryItemBias.NECROMANTIC and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.VAMPIRIC):
			_item.legendary_flags_1.set_bits(ItemFlags1.VAMPIRIC)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_POISON) and Rng.roll_die(2) == 1:
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_POISON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.RANGER and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.SLAY_ANIMAL):
			_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_ANIMAL)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.ROGUE and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_POISON):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_POISON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.POISON and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_POISON):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_POISON)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.FIRE and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_FIRE):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_FIRE)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.COLD and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_COLD):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_COLD)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.ELECTRICITY and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_ELECTRIC):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_ELECTRIC)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.ACID and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.BRAND_ACID):
			_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_ACID)
			if Rng.roll_die(2) == 1:
				return
	elif _legendary_item_bias == LegendaryItemBias.LAW and _item.category != ItemCategory.BOW:
		if _item.legendary_flags_1.is_clear(ItemFlags1.SLAY_EVIL):
			_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_EVIL)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.SLAY_UNDEAD):
			_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_UNDEAD)
			if Rng.roll_die(2) == 1:
				return
		if _item.legendary_flags_1.is_clear(ItemFlags1.SLAY_DEMON):
			_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_DEMON)
			if Rng.roll_die(2) == 1:
				return
	if _item.category != ItemCategory.BOW:
		match Rng.roll_die(34):
			1, 2:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_ANIMAL)
			3, 4:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_EVIL)
				if _legendary_item_bias == 0 and Rng.roll_die(2) == 1:
					_legendary_item_bias = LegendaryItemBias.LAW
				elif _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
					_legendary_item_bias = LegendaryItemBias.PRIESTLY
			5, 6:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_UNDEAD)
				if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
					_legendary_item_bias = LegendaryItemBias.PRIESTLY
			7, 8:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_DEMON)
				if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
					_legendary_item_bias = LegendaryItemBias.PRIESTLY
			9, 10:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_ORC)
			11, 12:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_TROLL)
			13, 14:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_GIANT)
			15, 16:
				_item.legendary_flags_1.set_bits(ItemFlags1.SLAY_DRAGON)
			17:
				_item.legendary_flags_1.set_bits(ItemFlags1.KILL_DRAGON)
			18, 19:
				if _item.category == ItemCategory.SWORD:
					_item.legendary_flags_1.set_bits(ItemFlags1.VORPAL)
					if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
						_legendary_item_bias = LegendaryItemBias.WARRIOR
				else:
					apply_random_slaying()
			20:
				_item.legendary_flags_1.set_bits(ItemFlags1.IMPACT)
			21, 22:
				_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_FIRE)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.FIRE
			23, 24:
				_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_COLD)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.COLD
			25, 26:
				_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_ELECTRIC)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.ELECTRICITY
			27, 28:
				_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_ACID)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.ACID
			29, 30:
				_item.legendary_flags_1.set_bits(ItemFlags1.BRAND_POISON)
				if _legendary_item_bias == 0 and Rng.roll_die(3) != 1:
					_legendary_item_bias = LegendaryItemBias.POISON
				elif _legendary_item_bias == 0 and Rng.roll_die(6) == 1:
					_legendary_item_bias = LegendaryItemBias.NECROMANTIC
				elif _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.ROGUE
			31, 32:
				_item.legendary_flags_1.set_bits(ItemFlags1.VAMPIRIC)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.NECROMANTIC
			_:
				_item.legendary_flags_1.set_bits(ItemFlags1.CHAOTIC)
				if _legendary_item_bias == 0:
					_legendary_item_bias = LegendaryItemBias.CHAOS
	else:
		match Rng.roll_die(6):
			1, 2, 3:
				_item.legendary_flags_3.set_bits(ItemFlags3.XTRA_MIGHT)
				if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
					_legendary_item_bias = LegendaryItemBias.RANGER
			_:
				_item.legendary_flags_3.set_bits(ItemFlags3.XTRA_SHOTS)
				if _legendary_item_bias == 0 and Rng.roll_die(9) == 1:
					_legendary_item_bias = LegendaryItemBias.RANGER


func curse_legendary() -> void:
	if _item.type_specific_value != 0:
		_item.type_specific_value = 0 - (_item.type_specific_value + Rng.roll_die(4))
	if _item.bonus_armour_class != 0:
		_item.bonus_armour_class = 0 - (_item.bonus_armour_class + Rng.roll_die(4))
	if _item.bonus_to_hit != 0:
		_item.bonus_to_hit = 0 - (_item.bonus_to_hit + Rng.roll_die(4))
	if _item.bonus_damage != 0:
		_item.bonus_damage = 0 - (_item.bonus_damage + Rng.roll_die(4))
	_item.legendary_flags_3.set_bits(ItemFlags3.HEAVY_CURSE | ItemFlags3.CURSED)
	if Rng.roll_die(4) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.PERMA_CURSE)
	if Rng.roll_die(3) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.DREAD_CURSE)
	if Rng.roll_die(2) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.AGGRAVATE)
	if Rng.roll_die(3) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.DRAIN_EXP)
	if Rng.roll_die(2) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.TELEPORT)
	elif Rng.roll_die(3) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.NO_TELE)
	if SaveGame.instance.player.profession.legendary_item_bias != LegendaryItemBias.WARRIOR and Rng.roll_die(3) == 1:
		_item.legendary_flags_3.set_bits(ItemFlags3.NO_MAGIC)
	_item.identify_flags.set_bits(IdentifyFlags.CURSED)


func make_item(good: bool, great: bool) -> bool:
	var prob: int = 10 if good else 1000
	var base_level: int = SaveGame.instance.dungeon_level.item_level + 10 if good else SaveGame.instance.dungeon_level.item_level
	if Rng.roll_die(prob) != 1 or not make_artifact():
		if good:
			ItemType.item_validation_function = ItemType.type_is_good
			ItemType.prepare_random_item_table()
		var type: ItemType = ItemType.random_item_type(base_level)
		if good:
			ItemType.item_validation_function = Callable()
			ItemType.prepare_random_item_table()
		if type == null:
			return false
		_item.assign_item_type(type)
	apply_magic(SaveGame.instance.dungeon_level.item_level, true, good, great)
	match _item.category:
		ItemCategory.SPIKE, ItemCategory.SHOT, ItemCategory.ARROW, ItemCategory.BOLT:
			_item.count = Rng.roll_dice(6, 7)
	if not _item.is_cursed() and not _item.is_broken() and _item.item_type.level > SaveGame.instance.total_difficulty():
		if SaveGame.instance.dungeon_level != null:
			SaveGame.instance.dungeon_level.treasure_rating += _item.item_type.level - SaveGame.instance.total_difficulty()
	return true


func make_artifact() -> bool:
	for index: int in SaveGame.instance.artifacts:
		var artifact:Artifact = SaveGame.instance.artifacts[index]
		if not artifact.has_own_type:
			continue
		if artifact.current_number != 0:
			continue
		if artifact.level > SaveGame.instance.total_difficulty():
			var d: int = (artifact.level - SaveGame.instance.total_difficulty()) * 2
			if Rng.roll_die(d) != 1:
				continue
		if Rng.roll_die(artifact.rarity) != 1:
			return false
		var type: ItemType = SaveGame.instance.item_types.look_up_type(artifact.item_category, artifact.item_sub_category)
		if type.level > SaveGame.instance.dungeon_level.item_level:
			var d: int = (type.level - SaveGame.instance.dungeon_level.item_level) * 5
			if Rng.roll_die(d) != 1:
				continue
		_item.assign_item_type(type)
		_item.artifact_index = index
		return true
	return false
