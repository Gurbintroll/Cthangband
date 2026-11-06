# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PlayerStatus
extends RefCounted

var _player: Player
var _dungeon_level: DungeonLevel
var _savegame: SaveGame
var _blows_table: Array = [
	[1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3],
	[1, 1, 1, 1, 2, 2, 3, 3, 3, 4, 4, 4],
	[1, 1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 5],
	[1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 5, 5],
	[1, 2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5],
	[2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5, 6],
	[2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5, 6],
	[2, 3, 3, 4, 4, 4, 5, 5, 5, 5, 5, 6],
	[3, 3, 3, 4, 4, 4, 5, 5, 5, 5, 6, 6],
	[3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6],
	[3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6],
	[3, 3, 4, 4, 4, 4, 5, 5, 6, 6, 6, 6],
]


func _init(player: Player, dungeon_level: DungeonLevel) -> void:
	_player = player
	_dungeon_level = dungeon_level
	_savegame = SaveGame.instance


func calc_bonuses() -> void:
	# Store some values for later checks to see if they have changed
	var old_speed: int = _player.speed
	var old_telepathy: bool = _player.has_telepathy
	var old_see_invisible: bool = _player.has_see_invisibility
	var old_displayed_ac: int = _player.displayed_base_armour_class
	var old_displayed_ac_bonus: int = _player.displayed_armour_class_bonus
	var item: Item
	# We'll need to get item flags in more than once place
	var f1: FlagSet = FlagSet.new()
	var f2: FlagSet = FlagSet.new()
	var f3: FlagSet = FlagSet.new()
	# Assume we have no bonuses or abilities to start with
	var extra_blows: int = 0
	var extra_shots: int = 0
	for i: int in 6:
		_player.ability_scores[i].bonus = 0
	_player.displayed_base_armour_class = 0
	_player.base_armour_class = 0
	_player.displayed_attack_bonus = 0
	_player.attack_bonus = 0
	_player.displayed_damage_bonus = 0
	_player.damage_bonus = 0
	_player.displayed_armour_class_bonus = 0
	_player.armour_class_bonus = 0
	_player.has_aggravation = false
	_player.has_random_teleport = false
	_player.has_experience_drain = false
	_player.has_blessed_blade = false
	_player.has_extra_might = false
	_player.has_quake_weapon = false
	_player.has_see_invisibility = false
	_player.has_free_action = false
	_player.has_slow_digestion = false
	_player.has_regeneration = false
	_player.has_feather_fall = false
	_player.has_hold_life = false
	_player.has_telepathy = false
	_player.has_glow = false
	_player.has_sustain_strength = false
	_player.has_sustain_intelligence = false
	_player.has_sustain_wisdom = false
	_player.has_sustain_dexterity = false
	_player.has_sustain_constitution = false
	_player.has_sustain_charisma = false
	_player.has_acid_resistance = false
	_player.has_lightning_resistance = false
	_player.has_fire_resistance = false
	_player.has_cold_resistance = false
	_player.has_poison_resistance = false
	_player.has_confusion_resistance = false
	_player.has_sound_resistance = false
	_player.has_time_resistance = false
	_player.has_light_resistance = false
	_player.has_dark_resistance = false
	_player.has_chaos_resistance = false
	_player.has_disenchant_resistance = false
	_player.has_shard_resistance = false
	_player.has_nexus_resistance = false
	_player.has_blindness_resistance = false
	_player.has_nether_resistance = false
	_player.has_fear_resistance = false
	_player.has_elemental_vulnerability = false
	_player.has_reflection = false
	_player.has_fire_shield = false
	_player.has_lightning_shield = false
	_player.has_anti_magic = false
	_player.has_anti_teleport = false
	_player.has_anti_theft = false
	_player.has_acid_immunity = false
	_player.has_lightning_immunity = false
	_player.has_fire_immunity = false
	_player.has_cold_immunity = false
	_player.infravision_range = _player.ancestry.infravision
	_player.skill_disarm_traps = _player.ancestry.base_disarm_bonus + _player.profession.base_disarm_bonus
	_player.skill_use_device = _player.ancestry.base_device_bonus + _player.profession.base_device_bonus
	_player.skill_saving_throw = _player.ancestry.base_save_bonus + _player.profession.base_save_bonus
	_player.skill_stealth = _player.ancestry.base_stealth_bonus + _player.profession.base_stealth_bonus
	_player.skill_searching = _player.ancestry.base_search_bonus + _player.profession.base_search_bonus
	_player.skill_search_frequency = _player.ancestry.base_search_frequency + _player.profession.base_search_frequency
	_player.skill_melee = _player.ancestry.base_melee_attack_bonus + _player.profession.base_melee_attack_bonus
	_player.skill_ranged = _player.ancestry.base_ranged_attack_bonus + _player.profession.base_ranged_attack_bonus
	_player.skill_throwing = _player.ancestry.base_ranged_attack_bonus + _player.profession.base_ranged_attack_bonus
	_player.skill_digging = 0
	_player.speed = 110
	_player.melee_attacks_per_round = 1
	_player.missile_attacks_per_round = 1
	_player.ammunition_item_category = 0
	# Add the bonuses from our profession and ancestry
	_player.profession.apply_statuses_from_profession(_player)
	_player.ancestry.apply_ancestral_status(_player)
	for i: int in 6:
		_player.ability_scores[i].bonus += _player.ancestry.ability_bonus[i] + _player.profession.ability_bonus[i]
	# Add the bonuses from any mutations
	_player.ability_scores[Ability.STRENGTH].bonus += _player.genome.str_bonus
	_player.ability_scores[Ability.INTELLIGENCE].bonus += _player.genome.int_bonus
	_player.ability_scores[Ability.WISDOM].bonus += _player.genome.wis_bonus
	_player.ability_scores[Ability.DEXTERITY].bonus += _player.genome.dex_bonus
	_player.ability_scores[Ability.CONSTITUTION].bonus += _player.genome.con_bonus
	_player.ability_scores[Ability.CHARISMA].bonus += _player.genome.cha_bonus
	_player.speed += _player.genome.speed_bonus
	_player.has_regeneration = _player.has_regeneration or _player.genome.regen
	_player.skill_search_frequency += _player.genome.search_bonus
	_player.skill_searching += _player.genome.search_bonus
	_player.infravision_range += _player.genome.infravision_bonus
	_player.has_lightning_shield = _player.has_lightning_shield or _player.genome.elec_hit
	_player.has_fire_shield = _player.has_fire_shield or _player.genome.fire_hit
	_player.has_glow = _player.has_glow or _player.genome.fire_hit
	_player.armour_class_bonus += _player.genome.armour_class_bonus
	_player.displayed_armour_class_bonus += _player.genome.armour_class_bonus
	_player.has_feather_fall = _player.has_feather_fall or _player.genome.feather_fall
	_player.has_fear_resistance = _player.has_fear_resistance or _player.genome.res_fear
	_player.has_time_resistance = _player.has_time_resistance or _player.genome.res_time
	_player.has_telepathy = _player.has_telepathy or _player.genome.esp
	_player.skill_stealth += _player.genome.stealth_bonus
	_player.has_free_action = _player.has_free_action or _player.genome.free_action
	_player.has_elemental_vulnerability = _player.has_elemental_vulnerability or _player.genome.vulnerable
	if _player.genome.magic_resistance:
		_player.skill_saving_throw += 15 + (_player.level / 5)
	if _player.genome.suppress_regen:
		_player.has_regeneration = false
	if _player.genome.cha_override:
		_player.ability_scores[Ability.CHARISMA].Bonus = 0
	if _player.genome.sustain_all:
		_player.has_sustain_constitution = true
		if _player.level > 9:
			_player.has_sustain_strength = true
		if _player.level > 19:
			_player.has_sustain_dexterity = true
		if _player.level > 29:
			_player.has_sustain_wisdom = true
		if _player.level > 39:
			_player.has_sustain_intelligence = true
		if _player.level > 49:
			_player.has_sustain_charisma = true
	# Add bonuses from any items the player is wielding
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		item = _player.inventory.items[i]
		# If item slot is empty then skip it
		if item.item_type == null:
			continue
		# Consolidate the item's abilities onto a single set of flags
		item.get_merged_flags(f1, f2, f3)
		# Flagset 1
		if f1.is_set(ItemFlags1.STR):
			_player.ability_scores[Ability.STRENGTH].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.INT):
			_player.ability_scores[Ability.INTELLIGENCE].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.WIS):
			_player.ability_scores[Ability.WISDOM].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.DEX):
			_player.ability_scores[Ability.DEXTERITY].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.CON):
			_player.ability_scores[Ability.CONSTITUTION].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.CHA):
			_player.ability_scores[Ability.CHARISMA].bonus += item.type_specific_value
		if f1.is_set(ItemFlags1.STEALTH):
			_player.skill_stealth += item.type_specific_value
		if f1.is_set(ItemFlags1.SEARCH):
			_player.skill_searching += item.type_specific_value * 5
			_player.skill_search_frequency += item.type_specific_value * 5
		if f1.is_set(ItemFlags1.INFRA):
			_player.infravision_range += item.type_specific_value
		if f1.is_set(ItemFlags1.TUNNEL):
			_player.skill_digging += item.type_specific_value * 20
		if f1.is_set(ItemFlags1.SPEED):
			_player.speed += item.type_specific_value
		if f1.is_set(ItemFlags1.BLOWS):
			extra_blows += item.type_specific_value
		if f1.is_set(ItemFlags1.IMPACT):
			_player.has_quake_weapon = true
		# Flagset 2
		if f2.is_set(ItemFlags2.FREE_ACT):
			_player.has_free_action = true
		if f2.is_set(ItemFlags2.HOLD_LIFE):
			_player.has_hold_life = true
		if f2.is_set(ItemFlags2.IM_FIRE):
			_player.has_fire_immunity = true
		if f2.is_set(ItemFlags2.IM_ACID):
			_player.has_acid_immunity = true
		if f2.is_set(ItemFlags2.IM_COLD):
			_player.has_cold_immunity = true
		if f2.is_set(ItemFlags2.IM_ELEC):
			_player.has_lightning_immunity = true
		if f2.is_set(ItemFlags2.RES_ACID):
			_player.has_acid_resistance = true
		if f2.is_set(ItemFlags2.RES_ELEC):
			_player.has_lightning_resistance = true
		if f2.is_set(ItemFlags2.RES_FIRE):
			_player.has_fire_resistance = true
		if f2.is_set(ItemFlags2.RES_COLD):
			_player.has_cold_resistance = true
		if f2.is_set(ItemFlags2.RES_POIS):
			_player.has_poison_resistance = true
		if f2.is_set(ItemFlags2.RES_FEAR):
			_player.has_fear_resistance = true
		if f2.is_set(ItemFlags2.RES_CONF):
			_player.has_confusion_resistance = true
		if f2.is_set(ItemFlags2.RES_SOUND):
			_player.has_sound_resistance = true
		if f2.is_set(ItemFlags2.RES_LIGHT):
			_player.has_light_resistance = true
		if f2.is_set(ItemFlags2.RES_DARK):
			_player.has_dark_resistance = true
		if f2.is_set(ItemFlags2.RES_CHAOS):
			_player.has_chaos_resistance = true
		if f2.is_set(ItemFlags2.RES_DISEN):
			_player.has_disenchant_resistance = true
		if f2.is_set(ItemFlags2.RES_SHARDS):
			_player.has_shard_resistance = true
		if f2.is_set(ItemFlags2.RES_NEXUS):
			_player.has_nexus_resistance = true
		if f2.is_set(ItemFlags2.RES_BLIND):
			_player.has_blindness_resistance = true
		if f2.is_set(ItemFlags2.RES_NETHER):
			_player.has_nether_resistance = true
		if f2.is_set(ItemFlags2.REFLECT):
			_player.has_reflection = true
		if f2.is_set(ItemFlags2.SUST_STR):
			_player.has_sustain_strength = true
		if f2.is_set(ItemFlags2.SUST_INT):
			_player.has_sustain_intelligence = true
		if f2.is_set(ItemFlags2.SUST_WIS):
			_player.has_sustain_wisdom = true
		if f2.is_set(ItemFlags2.SUST_DEX):
			_player.has_sustain_dexterity = true
		if f2.is_set(ItemFlags2.SUST_CON):
			_player.has_sustain_constitution = true
		if f2.is_set(ItemFlags2.SUST_CHA):
			_player.has_sustain_charisma = true
		# Flagset 3
		if f3.is_set(ItemFlags3.ANTI_THEFT):
			_player.has_anti_theft = true
		if f3.is_set(ItemFlags3.XTRA_SHOTS):
			extra_shots += 1
		if f3.is_set(ItemFlags3.AGGRAVATE):
			_player.has_aggravation = true
		if f3.is_set(ItemFlags3.TELEPORT):
			_player.has_random_teleport = true
		if f3.is_set(ItemFlags3.DRAIN_EXP):
			_player.has_experience_drain = true
		if f3.is_set(ItemFlags3.BLESSED):
			_player.has_blessed_blade = true
		if f3.is_set(ItemFlags3.XTRA_MIGHT):
			_player.has_extra_might = true
		if f3.is_set(ItemFlags3.SLOW_DIGEST):
			_player.has_slow_digestion = true
		if f3.is_set(ItemFlags3.REGEN):
			_player.has_regeneration = true
		if f3.is_set(ItemFlags3.TELEPATHY):
			_player.has_telepathy = true
		if f3.is_set(ItemFlags3.LIGHT_SOURCE):
			_player.has_glow = true
		if f3.is_set(ItemFlags3.SEE_INVIS):
			_player.has_see_invisibility = true
		if f3.is_set(ItemFlags3.FEATHER):
			_player.has_feather_fall = true
		if f3.is_set(ItemFlags3.WRAITH):
			_player.timed_etherealness = max(_player.timed_etherealness, 20)
		if f3.is_set(ItemFlags3.SH_FIRE):
			_player.has_fire_shield = true
		if f3.is_set(ItemFlags3.SH_ELEC):
			_player.has_lightning_shield = true
		if f3.is_set(ItemFlags3.NO_MAGIC):
			_player.has_anti_magic = true
		if f3.is_set(ItemFlags3.NO_TELE):
			_player.has_anti_teleport = true
		# Now the non-flag item properties
		_player.base_armour_class += item.base_armour_class
		_player.displayed_base_armour_class += item.base_armour_class
		_player.armour_class_bonus += item.bonus_armour_class
		if item.is_known():
			_player.displayed_armour_class_bonus += item.bonus_armour_class
		# Weapon bonuses are added separately during the attack
		if i == InventorySlot.MELEE_WEAPON or i == InventorySlot.RANGED_WEAPON:
			continue
		_player.attack_bonus += item.bonus_to_hit
		if item.is_known():
			_player.displayed_attack_bonus += item.bonus_to_hit
		_player.damage_bonus += item.bonus_damage
		if item.is_known():
			_player.displayed_damage_bonus += item.bonus_damage
	# Add additional bonuses to armour class and attack bonus from ancestry and profession
	_player.profession.apply_martial_artist_armour_class(_player)
	_player.ancestry.apply_armour_bonus(_player)
	# Fire shield makes us glow toasty warm
	if _player.has_fire_shield:
		_player.has_glow = true
	# Apply adjustments to player's ability scores
	for i: int in 6:
		# Adjust the score by the bonus, flagging that we need a redaw if necessary
		var index: int
		var score: AbilityScore = _player.ability_scores[i]
		var top: int = score.modify_stat_value(score.innate_max, score.bonus)
		if score.adjusted_max != top:
			score.adjusted_max = top
			_player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATS)
		var use: int = score.modify_stat_value(score.innate, score.bonus)
		if i == Ability.CHARISMA and _player.genome.cha_override:
			if use < 8 + (2 * _player.level):
				use = 8 + (2 * _player.level)
		if score.adjusted != use:
			score.adjusted = use
			_player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATS)
		# Convert the score to an index to be used on the various ability score tables
		if use < 18:
			index = use - 3
		elif use <= 18 + 219:
			index = 15 + ((use - 18) / 10)
		else:
			index = 37
		# Look up the relevant bonuses for the score if it has changed
		if score.table_index != index:
			score.table_index = index
			if i == Ability.CONSTITUTION:
				_player.updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH)
			elif i == Ability.INTELLIGENCE and i == _player.spellcasting.spell_stat:
				_player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS)
			elif i == Ability.WISDOM and i == _player.spellcasting.spell_stat:
				_player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS)
			elif i == Ability.CHARISMA and i == _player.spellcasting.spell_stat:
				_player.updates_needed.set_bits(UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS)
	# Check effects of timed abilities/penalties
	# Stunned
	if _player.timed_stun > 50:
		_player.attack_bonus -= 20
		_player.displayed_attack_bonus -= 20
		_player.damage_bonus -= 20
		_player.displayed_damage_bonus -= 20
	elif _player.timed_stun != 0:
		_player.attack_bonus -= 5
		_player.displayed_attack_bonus -= 5
		_player.damage_bonus -= 5
		_player.displayed_damage_bonus -= 5
	# Invulnerability
	if _player.timed_invulnerability != 0:
		_player.armour_class_bonus += 100
		_player.displayed_armour_class_bonus += 100
	# Etherealness
	if _player.timed_etherealness != 0:
		_player.armour_class_bonus += 100
		_player.displayed_armour_class_bonus += 100
		_player.has_reflection = true
	# Bless
	if _player.timed_blessing != 0:
		_player.armour_class_bonus += 5
		_player.displayed_armour_class_bonus += 5
		_player.attack_bonus += 10
		_player.displayed_attack_bonus += 10
	# Stone SKin
	if _player.timed_stoneskin != 0:
		_player.armour_class_bonus += 50
		_player.displayed_armour_class_bonus += 50
	# Heroism
	if _player.timed_heroism != 0:
		_player.attack_bonus += 12
		_player.displayed_attack_bonus += 12
		_player.has_fear_resistance = true
	# Super heroism
	if _player.timed_super_heroism != 0:
		_player.attack_bonus += 24
		_player.displayed_attack_bonus += 24
		_player.armour_class_bonus -= 10
		_player.displayed_armour_class_bonus -= 10
		_player.has_fear_resistance = true
	# Haste
	if _player.timed_haste != 0:
		_player.speed += 10
	# Slow
	if _player.timed_slow != 0:
		_player.speed -= 10
	# Other Speed
	if _player.ancestry.has_speed_bonus or _player.profession.has_speed_bonus(_player):
		_player.speed += _player.level / 10
	# Telepathy
	if _player.timed_telepathy != 0:
		_player.has_telepathy = true
	# See invisible
	if _player.timed_see_invisibility != 0:
		_player.has_see_invisibility = true
	# Infravision
	if _player.timed_infravision != 0:
		_player.infravision_range += 1
	# Chaos resistance also counts as confusion resistance
	if _player.has_chaos_resistance:
		_player.has_confusion_resistance = true
	# Change in telepathy status requires a monster update
	if _player.has_telepathy != old_telepathy:
		_player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	# Change in see invisible status requires a monster update
	if _player.has_see_invisibility != old_see_invisible:
		_player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	# Miscellaneous other things affect speed
	var weight: int = _player.weight_carried
	var limit: int = _player.ability_scores[Ability.STRENGTH].str_carrying_capacity() * 100
	if weight > limit / 2:
		_player.speed -= (weight - (limit / 2)) / (limit / 10)
	if _player.food > Constants.FOOD_GORGED:
		_player.speed -= 10
	if _player.is_searching:
		_player.speed -= 10
	if _player.speed != old_speed:
		_player.redraw_needed.set_bits(RedrawFlags.REDRAW_SPEED)
	# Apply attribute adjustments
	_player.armour_class_bonus += _player.ability_scores[Ability.DEXTERITY].dex_armour_class_bonus()
	_player.displayed_armour_class_bonus += _player.ability_scores[Ability.DEXTERITY].dex_armour_class_bonus()
	_player.damage_bonus += _player.ability_scores[Ability.STRENGTH].str_damage_bonus()
	_player.displayed_damage_bonus += _player.ability_scores[Ability.STRENGTH].str_damage_bonus()
	_player.attack_bonus += _player.ability_scores[Ability.DEXTERITY].dex_attack_bonus()
	_player.displayed_attack_bonus += _player.ability_scores[Ability.DEXTERITY].dex_attack_bonus()
	_player.attack_bonus += _player.ability_scores[Ability.STRENGTH].str_attack_bonus()
	_player.displayed_attack_bonus += _player.ability_scores[Ability.STRENGTH].str_attack_bonus()
	# Change in displayed AC requires a redraw
	if _player.displayed_armour_class_bonus != old_displayed_ac_bonus or _player.displayed_base_armour_class != old_displayed_ac:
		_player.redraw_needed.set_bits(RedrawFlags.REDRAW_ARMOR)
	# Check weight and ammunition type of your bow
	var max_weapon_weight: int = _player.ability_scores[Ability.STRENGTH].str_max_weapon_weight()
	item = _player.inventory.items[InventorySlot.RANGED_WEAPON]
	_player.has_heavy_bow = false
	if max_weapon_weight < item.weight / 10:
		_player.attack_bonus += 2 * (max_weapon_weight - (item.weight / 10))
		_player.displayed_attack_bonus += 2 * (max_weapon_weight - (item.weight / 10))
		_player.has_heavy_bow = true
	if item.item_type != null and _player.has_heavy_bow == false:
		match item.item_sub_category:
			BowType.SLING:
				_player.ammunition_item_category = ItemCategory.SHOT
			BowType.SHORTBOW, BowType.LONGBOW:
				_player.ammunition_item_category = ItemCategory.ARROW
			BowType.LIGHT_CROSSBOW, BowType.HEAVY_CROSSBOW:
				_player.ammunition_item_category = ItemCategory.BOLT
		_player.profession.apply_bonus_missile_attacks(_player)
		_player.missile_attacks_per_round += extra_shots
		if _player.missile_attacks_per_round < 1:
			_player.missile_attacks_per_round = 1
	# Check weight and type of melee weapon
	item = _player.inventory.items[InventorySlot.MELEE_WEAPON]
	_player.has_heavy_weapon = false
	if max_weapon_weight < item.weight / 10:
		_player.attack_bonus += 2 * (max_weapon_weight - (item.weight / 10))
		_player.displayed_attack_bonus += 2 * (max_weapon_weight - (item.weight / 10))
		_player.has_heavy_weapon = true
	if item.item_type != null and _player.has_heavy_weapon == false:
		var max_attacks: int = _player.profession.attacks_per_turn_max
		var min_weight: int = _player.profession.attacks_per_turn_min_weapon_weight
		var multiplier: int = _player.profession.attacks_per_turn_weight_multiplier
		var divider: int = min_weight if item.weight < min_weight else item.weight
		var str_index: int = _player.ability_scores[Ability.STRENGTH].str_attack_speed_component() * multiplier / divider
		if str_index > 11:
			str_index = 11
		var dex_index: int = _player.ability_scores[Ability.DEXTERITY].dex_attack_speed_component()
		if dex_index > 11:
			dex_index = 11
		_player.melee_attacks_per_round = _blows_table[str_index][dex_index]
		if _player.melee_attacks_per_round > max_attacks:
			_player.melee_attacks_per_round = max_attacks
		_player.melee_attacks_per_round += extra_blows
		_player.profession.apply_bonus_melee_attacks(_player)
		if _player.melee_attacks_per_round < 1:
			_player.melee_attacks_per_round = 1
		_player.skill_digging += item.weight / 10
	# If we have no weapon check for martial arts
	elif _player.profession.is_martial_artist and _martial_artist_empty_hands():
		_player.melee_attacks_per_round = 0
		if _player.level > 9:
			_player.melee_attacks_per_round += 1
		if _player.level > 19:
			_player.melee_attacks_per_round += 1
		if _player.level > 29:
			_player.melee_attacks_per_round += 1
		if _player.level > 34:
			_player.melee_attacks_per_round += 1
		if _player.level > 39:
			_player.melee_attacks_per_round += 1
		if _player.level > 44:
			_player.melee_attacks_per_round += 1
		if _player.level > 49:
			_player.melee_attacks_per_round += 1
		if _player.profession.martial_artist_heavy_armour(_player):
			_player.melee_attacks_per_round /= 2
		_player.melee_attacks_per_round += 1 + extra_blows
		if not _player.profession.martial_artist_heavy_armour(_player):
			_player.attack_bonus += _player.level / 3
			_player.displayed_attack_bonus += _player.level / 3
			_player.damage_bonus += _player.level / 3
			_player.displayed_damage_bonus += _player.level / 3
	# Check for non-blessed weapons
	_player.has_unpriestly_weapon = false
	_player.has_heavy_armour = false
	_player.profession.apply_attack_and_damage_bonus(_player)
	if _player.profession.blunt_weapons_only and (not _player.has_blessed_blade) and (item.category == ItemCategory.SWORD or item.category == ItemCategory.POLEARM):
		_player.attack_bonus -= 2
		_player.displayed_attack_bonus -= 2
		_player.damage_bonus -= 2
		_player.displayed_damage_bonus -= 2
		_player.has_unpriestly_weapon = true
	# Check for non-chaos blade
	if _player.profession.chaos_weapons_only and item.iten_type != null:
		if item.category != ItemCategory.SWORD or item.sub_category != SwordType.BLADE_OF_CHAOS:
			item.get_merged_flags(f1, f2, f3)
			if f1.is_clear(ItemFlags1.CHAOTIC):
				_player.attack_bonus -= 10
				_player.displayed_attack_bonus -= 10
				_player.damage_bonus -= 10
				_player.displayed_damage_bonus -= 10
				_player.has_unpriestly_weapon = true
	# check for heavy armour
	if _player.profession.martial_artist_heavy_armour(_player):
		_player.has_heavy_armour = true
	#Update our skills from our ability scores
	_player.skill_stealth += 1
	_player.skill_disarm_traps += _player.ability_scores[Ability.DEXTERITY].dex_disarm_bonus()
	_player.skill_disarm_traps += _player.ability_scores[Ability.INTELLIGENCE].int_disarm_bonus()
	_player.skill_use_device += _player.ability_scores[Ability.INTELLIGENCE].int_use_device_bonus()
	_player.skill_saving_throw += _player.ability_scores[Ability.WISDOM].wis_saving_throw_bonus()
	_player.skill_digging += _player.ability_scores[Ability.STRENGTH].str_digging_bonus()
	_player.skill_disarm_traps += (_player.profession.disarm_bonus_per_level * _player.level) / 10
	_player.skill_use_device += (_player.profession.device_bonus_per_level * _player.level) / 10
	_player.skill_saving_throw += (_player.profession.save_bonus_per_level * _player.level) / 10
	_player.skill_stealth += (_player.profession.stealth_bonus_per_level * _player.level) / 10
	_player.skill_searching += (_player.profession.search_bonus_per_level * _player.level) / 10
	_player.skill_search_frequency += (_player.profession.search_frequency_per_level * _player.level) / 10
	_player.skill_melee += (_player.profession.melee_attack_bonus_per_level * _player.level) / 10
	_player.skill_ranged += (_player.profession.ranged_attack_bonus_per_level * _player.level) / 10
	_player.skill_throwing += (_player.profession.ranged_attack_bonus_per_level * _player.level) / 10
	if _player.skill_stealth > 30:
		_player.skill_stealth = 30
	if _player.skill_stealth < 0:
		_player.skill_stealth = 0
	if _player.skill_digging < 1:
		_player.skill_digging = 1
	if _player.has_anti_magic and _player.skill_saving_throw < 95:
		_player.skill_saving_throw = 95
	# If we are in the middle of the post-load update, stop here
	if _savegame.in_post_load_update:
		return
	# Inform the player about important changes
	# Too heavy bow
	if _player.has_heavy_bow != _player.old_heavy_bow:
		if _player.has_heavy_bow:
			Profile.instance.message_print("You have trouble wielding such a heavy bow.")
		elif _player.inventory.items[InventorySlot.RANGED_WEAPON].item_type != null:
			Profile.instance.message_print("You have no trouble weilding your bow.")
		else:
			Profile.instance.message_print("You feel relieved to put down your heavy bow.")
		_player.old_heavy_bow = _player.has_heavy_bow
	# Too heavy weapon
	if _player.has_heavy_weapon != _player.old_heavy_weapon:
		if _player.has_heavy_weapon:
			Profile.instance.message_print("You have trouble wielding such a heavy weapon.")
		elif _player.inventory.items[InventorySlot.MELEE_WEAPON].item_type != null:
			Profile.instance.message_print("You have no trouble weilding your weapon.")
		else:
			Profile.instance.message_print("You feel relieved to put down your heavy weapon.")
		_player.old_heavy_weapon = _player.has_heavy_weapon
	# Weapon is against class restrictions
	if _player.has_unpriestly_weapon != _player.old_unpriestly_weapon:
		if _player.has_unpriestly_weapon:
			if _player.profession.chaos_weapons_only:
				Profile.instance.message_print("Your weapon restricts the flow of chaos through you.")
			else:
				Profile.instance.message_print("You do not feel comfortable with your weapon.")
		elif _player.inventory.items[InventorySlot.MELEE_WEAPON].item_type != null:
			Profile.instance.message_print("You feel comfortable with your weapon.")
		else:
			if _player.profession.chaos_weapons_only:
				Profile.instance.message_print("Chaos flows freely through you again.")
			else:
				Profile.instance.message_print("You feel more comfortable after removing your weapon.")
		_player.old_unpriestly_weapon = _player.has_unpriestly_weapon
	# Armour is too heavy for martial arts
	if _player.profession.is_martial_artist and _player.has_heavy_armour != _player.old_heavy_armour:
		if _player.has_heavy_armour:
			Profile.instance.message_print("The weight of your armour disrupts your balance.")
		else:
			Profile.instance.message_print("You regain your balance.")
		_player.old_heavy_armour = _player.has_heavy_armour


func calc_hit_points() -> void:
	var bonus: int = _player.ability_scores[Ability.CONSTITUTION].con_health_bonus()
	var max_hp: int = _player.player_hp[_player.level - 1] + (bonus * _player.level / 2)
	if max_hp < _player.level + 1:
		max_hp = _player.level + 1
	if _player.timed_heroism != 0:
		max_hp += 10
	if _player.timed_super_heroism != 0:
		max_hp += 30
	var multiplier: int = _player.religion.gods[GodName.NATH_HORTHATH].adjusted_favour() + 10
	max_hp *= multiplier
	max_hp /= 10
	if _player.max_health != max_hp:
		if _player.health >= max_hp:
			_player.health = max_hp
			_player.fractional_health = 0
		_player.max_health = max_hp
		_player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP)


func calc_vis() -> void:
	var levels: int
	match _player.spellcasting.casting_type:
		CastingType.NONE:
			_player.power = 0
			_player.max_power = 0
			return
		CastingType.ARCANE, CastingType.DIVINE:
			levels = _player.level - _player.spellcasting.spell_first + 1
		CastingType.MENTALISM, CastingType.CHANNELLING:
			levels = _player.level
	levels = max(levels, 0)
	var max_power: int = (_player.ability_scores[_player.spellcasting.spell_stat].power_bonus() * levels) / 2
	if max_power != 0:
		max_power += 1
	max_power = _player.profession.apply_power_bonus(max_power)
	if _player.spellcasting.casting_type == CastingType.ARCANE:
		var f1: FlagSet = FlagSet.new()
		var f2: FlagSet = FlagSet.new()
		var f3: FlagSet = FlagSet.new()
		_player.has_restricting_gloves = false
		var item: Item = _player.inventory.items[InventorySlot.HANDS]
		item.get_merged_flags(f1, f2, f3)
		if item.item_type != null and f2.is_clear(ItemFlags2.FREE_ACT) and f1.is_clear(ItemFlags1.DEX) and item.type_specific_value > 0:
			_player.has_restricting_gloves = true
			max_power = (3 * max_power) / 4
		_player.has_restricting_armour = false
		var total: int = 0
		total += _player.inventory.items[InventorySlot.BODY].weight
		total += _player.inventory.items[InventorySlot.HEAD].weight
		total += _player.inventory.items[InventorySlot.ARM].weight
		total += _player.inventory.items[InventorySlot.CLOAK].weight
		total += _player.inventory.items[InventorySlot.HANDS].weight
		total += _player.inventory.items[InventorySlot.FEET].weight
		var max_weight: int = _player.spellcasting.spell_weight
		if (total - max_weight) / 10 > 0:
			_player.has_restricting_armour = true
			max_power -= (total - max_weight) / 10
	if max_power < 0:
		max_power = 0
	var multiplier: int = _player.religion.gods[GodName.TAMASH].adjusted_favour() + 10
	max_power *= multiplier
	max_power /= 10
	if _player.max_power != max_power:
		if _player.power >= max_power:
			_player.power = max_power
			_player.fractional_power = 0
		_player.max_power = max_power
		_player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
	if _savegame.in_post_load_update:
		return
	if _player.old_restricting_gloves != _player.has_restricting_gloves:
		if _player.has_restricting_gloves:
			Profile.instance.message_print("Your covered hands feel unsuitable for spellcasting.")
		else:
			Profile.instance.message_print("Your hands feel more suitable for spellcasting.")
		_player.old_restricting_gloves = _player.has_restricting_gloves
	if _player.old_restricting_armour != _player.has_restricting_armour:
		if _player.has_restricting_armour:
			Profile.instance.message_print("The weight of your armour encumbers your movement.")
		else:
			Profile.instance.message_print("You feel able to move more freely.")
		_player.old_restricting_armour = _player.has_restricting_armour


func calc_spells() -> void:
	var spell: Spell
	if _player == null:
		return
	var p: String = "spell" if _player.spellcasting.casting_type == CastingType.ARCANE else "prayer"
	if _player.spellcasting.casting_type == CastingType.NONE:
		return
	if _player.realm_1 == Realm.NONE:
		return
	if _savegame.in_post_load_update:
		return
	var levels: int = _player.level - _player.spellcasting.spell_first + 1
	if levels < 0:
		levels = 0
	var num_allowed: int = _player.ability_scores[_player.spellcasting.spell_stat].half_spells_per_level() * levels / 2
	var num_known: int = 0
	for j: int in 64:
		if _player.spellcasting.spells[j / 32].spells[j % 32].learned:
			num_known += 1
	_player.spare_spell_slots = num_allowed - num_known
	for i: int in range(63, -1, -1):
		if num_known == 0:
			break
		var j: int = _player.spellcasting.spell_order[i]
		if j >= 99:
			continue
		spell = _player.spellcasting.spells[j / 32].spells[j % 32]
		if spell.level <= _player.level:
			continue
		if not spell.learned:
			continue
		spell.forgotten = true
		spell.learned = false
		num_known -= 1
		Profile.instance.message_print("You have forgotten the %s of %s." % [p, spell.name])
		_player.spare_spell_slots += 1
	for i: int in range(63, -1, -1):
		if _player.spare_spell_slots >= 0:
			break
		if num_known == 0:
			break
		var j: int = _player.spellcasting.spell_order[i]
		if j >= 99:
			continue
		spell = _player.spellcasting.spells[j / 32].spells[j % 32]
		if not spell.learned:
			continue
		spell.forgotten = true
		spell.learned = false
		num_known -= 1
		Profile.instance.message_print("You have forgotten the %s of %s." % [p, spell.name])
		_player.spare_spell_slots += 1
	var forgotten_total: int = 0
	for l: int in 64:
		if _player.spellcasting.spells[l / 32].spells[l % 32].forgotten:
			forgotten_total += 1
	for i: int in 64:
		if _player.spare_spell_slots <= 0:
			break
		if forgotten_total == 0:
			break
		var j: int = _player.spellcasting.spell_order[i]
		if j >= 99:
			break
		spell = _player.spellcasting.spells[j / 32].spells[j % 32]
		if spell.level > _player.level:
			continue
		if not spell.forgotten:
			continue
		spell.forgotten = false
		spell.learned = true
		forgotten_total -= 1
		if not Console.full_screen_overlay:
			Profile.instance.message_print("You have remembered the %s of %s." % [p, spell.name])
		_player.spare_spell_slots -= 1
	var k: int = 0
	var limit: int = 32 if _player.realm_2 == Realm.NONE else 64
	for j: int in limit:
		spell = _player.spellcasting.spells[j / 32].spells[j % 32]
		if spell.level > _player.level:
			continue
		if spell.learned:
			continue
		k += 1
	if _player.realm_2 == 0:
		if k > 32:
			k = 32
	else:
		if k > 64:
			k = 64
	if _player.spare_spell_slots > k:
		_player.spare_spell_slots = k
	if _player.old_spare_spell_slots != _player.spare_spell_slots:
		if _player.spare_spell_slots != 0:
			if not Console.full_screen_overlay:
				var suffix: String = "s" if _player.spare_spell_slots != 1 else ""
				Profile.instance.message_print("You can learn %d more %s%s." % [_player.spare_spell_slots, p, suffix])
		_player.old_spare_spell_slots = _player.spare_spell_slots
		_player.redraw_needed.set_bits(RedrawFlags.REDRAW_STUDY)


func calc_torch() -> void:
	var flags_1: FlagSet = FlagSet.new()
	var flags_2: FlagSet = FlagSet.new()
	var flags_3: FlagSet = FlagSet.new()
	_player.light_level = 0
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		var item: Item = _player.inventory.items[i]
		# Lights inherently give a base light radius
		if i == InventorySlot.LIGHT_SOURCE and item.item_type != null and item.category == ItemCategory.LIGHT:
			if item.sub_category == LightType.TORCH and item.type_specific_value > 0:
				_player.light_level += 1
			if item.sub_category == LightType.LANTERN and item.type_specific_value > 0:
				_player.light_level += 2
			if item.sub_category == LightType.ORB:
				_player.light_level += 2
			if item.is_artifact():
				_player.light_level += 3
		else:
			# Non-lights add to light radius if they glow
			if item.item_type == null:
				continue
			item.get_merged_flags(flags_1, flags_2, flags_3)
			if flags_3.is_set(ItemFlags3.LIGHT_SOURCE):
				_player.light_level += 1
		# We can't stack light beyond a radius of 5
		if _player.light_level > 5:
			_player.light_level = 5
		# Glowing gives us a minimum radius of 1
		if _player.has_glow and _player.light_level == 0:
			_player.light_level = 1
		# Notice a change in light level
		if _player.light_level != _player.old_light_level:
			_player.updates_needed.set_bits(UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_MONSTERS)
			_player.old_light_level = _player.light_level


func martial_arts_empty_hands() -> bool:
	if not _player.profession.is_martial_artist:
		return false
	return _player.inventory.items[InventorySlot.MELEE_WEAPON].item_type == null


func print_frame_basic() -> void:
	print_field(_player.name, ScreenLocation.ROW_NAME, ScreenLocation.COL_NAME)
	print_field(_player.ancestry.title, ScreenLocation.ROW_RACE, ScreenLocation.COL_RACE)
	print_field(_player.profession.get_profession_sub_name(_player.realm_1), ScreenLocation.ROW_CLASS, ScreenLocation.COL_CLASS)
	print_title()
	print_level()
	print_experience()
	for i: int in 6:
		print_stat(i)
	print_armour_class()
	print_hit_points()
	print_power_points()
	print_gold()
	print_depth()
	print_health_tracker()


func print_frame_extra() -> void:
	print_cuts()
	print_stun()
	print_hunger()
	print_trap_detection()
	print_blindness()
	print_confused()
	print_afraid()
	print_poisoned()
	print_state()
	print_speed()
	print_study()


func print_field(text: String, row: int, col: int) -> void:
	Console.print_at(row, col, "             ", Ink.WHITE)
	Console.print_at(row, col, text, Ink.BRIGHT_BLUE)


func print_speed() -> void:
	var i: int = _player.speed
	var ink: int = Ink.WHITE
	var text: String
	if _player.is_searching:
		i += 10
	var energy: float = Tables.EXTRACT_ENERGY[i]
	if i > 110:
		ink = Ink.BRIGHT_GREEN
		text = "Fast %.1f" % [energy / 10.0]
	elif i < 110:
		ink = Ink.BRIGHT_BROWN
		text = "Slow %.1f" % [energy / 10.0]
	Console.print_at(ScreenLocation.ROW_SPEED, ScreenLocation.COL_SPEED, text.rpad(10), ink)


func print_state() -> void:
	var ink: int = Ink.WHITE
	var text: String
	if _player.timed_paralysis != 0:
		ink = Ink.RED
		text = "Paralysed!"
	elif _savegame.resting != 0:
		if _savegame.resting > 0:
			text = "Rest %5d" % [_savegame.resting]
		elif _savegame.resting == -1:
			text = "Rest *****"
		elif _savegame.resting == -2:
			text = "Rest &&&&&"
		else:
			text = "Rest ?????"
	#TODO: Mark repeated commands
	elif _player.is_searching:
		text = "Searching "
	else:
		text = "          "
	Console.print_at(ScreenLocation.ROW_STATE, ScreenLocation.COL_STATE, text, ink)


func print_study() -> void:
	if _player.spare_spell_slots != 0:
		Console.print_at(ScreenLocation.ROW_STUDY, ScreenLocation.COL_STUDY, "Study", Ink.WHITE)
	else:
		Console.print_at(ScreenLocation.ROW_STUDY, ScreenLocation.COL_STUDY, "     ", Ink.WHITE)


func print_blindness() -> void:
	if _player.timed_blindness != 0:
		Console.print_at(ScreenLocation.ROW_BLIND, ScreenLocation.COL_BLIND, "Blind", Ink.ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_BLIND, ScreenLocation.COL_BLIND, "     ", Ink.WHITE)


func print_confused() -> void:
	if _player.timed_confusion != 0:
		Console.print_at(ScreenLocation.ROW_CONFUSED, ScreenLocation.COL_CONFUSED, "Confused", Ink.ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_CONFUSED, ScreenLocation.COL_CONFUSED, "        ", Ink.WHITE)


func print_afraid() -> void:
	if _player.timed_fear != 0:
		Console.print_at(ScreenLocation.ROW_AFRAID, ScreenLocation.COL_AFRAID, "Afraid", Ink.ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_AFRAID, ScreenLocation.COL_AFRAID, "      ", Ink.WHITE)


func print_poisoned() -> void:
	if _player.timed_poison != 0:
		Console.print_at(ScreenLocation.ROW_POISONED, ScreenLocation.COL_POISONED, "Poisoned", Ink.ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_POISONED, ScreenLocation.COL_POISONED, "        ", Ink.WHITE)


func print_title() -> void:
	if _player.is_wizard:
		print_field("-=<WIZARD>=-", ScreenLocation.ROW_TITLE, ScreenLocation.COL_TITLE)
	elif _player.is_winner:
		print_field("***WINNER***", ScreenLocation.ROW_TITLE, ScreenLocation.COL_TITLE)


func print_level() -> void:
	var text: String = "%6d" % [_player.level]
	if _player.level >= _player.max_level_gained:
		Console.print_at(ScreenLocation.ROW_LEVEL, 0, "LEVEL ", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_LEVEL, ScreenLocation.COL_LEVEL + 6, text, Ink.BRIGHT_GREEN)
	else:
		Console.print_at(ScreenLocation.ROW_LEVEL, 0, "level ", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_LEVEL, ScreenLocation.COL_LEVEL + 6, text, Ink.YELLOW)


func print_experience() -> void:
	var ink: int = Ink.BRIGHT_GREEN
	if _player.experience_points < _player.max_experience_gained:
		ink = Ink.YELLOW
	Console.print_at(ScreenLocation.ROW_EXP, 0, "NEXT", Ink.WHITE)
	if _player.level >= Constants.MAX_EXPERIENCE_LEVEL:
		Console.print_at(ScreenLocation.ROW_EXP, ScreenLocation.COL_EXP + 4, "   *****", Ink.BRIGHT_GREEN)
	else:
		var needed: int = (Tables.BASE_LEVEL_THRESHOLDS[_player.level - 1] * _player.experience_multiplier / 100) - _player.experience_points
		var text: String = "%8s" % [needed]
		Console.print_at(ScreenLocation.ROW_EXP, ScreenLocation.COL_EXP + 4, text, ink)


func print_stat(index: int) -> void:
	var stat: AbilityScore = _player.ability_scores[index]
	if stat.innate < stat.innate_max:
		Console.print_at(ScreenLocation.ROW_STAT + index, 0, Tables.ABILITY_NAMES_REDUCED[index], Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_STAT + index, ScreenLocation.COL_STAT + 6, Util.stat_to_string(stat.adjusted), Ink.YELLOW)
	else:
		Console.print_at(ScreenLocation.ROW_STAT + index, 0, Tables.ABILITY_NAMES[index], Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_STAT + index, ScreenLocation.COL_STAT + 6, Util.stat_to_string(stat.adjusted), Ink.BRIGHT_GREEN)
	if stat.innate_max == 118:
		Console.print_at(ScreenLocation.ROW_STAT + index, 3, "!", Ink.WHITE)


func print_armour_class() -> void:
	Console.print_at(ScreenLocation.ROW_AC, ScreenLocation.COL_AC, "Cur AC ", Ink.WHITE)
	var text: String = "%5d" % [_player.displayed_base_armour_class + _player.displayed_armour_class_bonus]
	Console.print_at(ScreenLocation.ROW_AC, ScreenLocation.COL_AC + 7, text, Ink.BRIGHT_GREEN)


func print_hit_points() -> void:
	Console.print_at(ScreenLocation.ROW_MAX_HP, ScreenLocation.COL_MAX_HP, "Max HP ", Ink.WHITE)
	Console.print_at(ScreenLocation.ROW_MAX_HP, ScreenLocation.COL_MAX_HP + 7, "%5d" % [_player.max_health], Ink.BRIGHT_GREEN)
	Console.print_at(ScreenLocation.ROW_CUR_HP, ScreenLocation.COL_CUR_HP, "Cur HP ", Ink.WHITE)
	var text: String = "%5d" % [_player.health]
	var ink: int
	if _player.health >= _player.max_health:
		ink = Ink.BRIGHT_GREEN
	elif _player.health > _player.max_health * Constants.HIT_POINT_WARNING / 5:
		ink = Ink.YELLOW
	elif _player.health > _player.max_health * Constants.HIT_POINT_WARNING / 5:
		ink = Ink.ORANGE
	else:
		ink = Ink.RED
	Console.print_at(ScreenLocation.ROW_CUR_HP, ScreenLocation.COL_CUR_HP + 7, text, ink)


func print_power_points() -> void:
	Console.print_at(ScreenLocation.ROW_MAX_SP, ScreenLocation.COL_MAX_SP, "Max PP ", Ink.WHITE)
	Console.print_at(ScreenLocation.ROW_MAX_SP, ScreenLocation.COL_MAX_SP + 7, "%5d" % [_player.max_power], Ink.BRIGHT_GREEN)
	Console.print_at(ScreenLocation.ROW_CUR_SP, ScreenLocation.COL_CUR_SP, "Cur PP ", Ink.WHITE)
	var text: String = "%5d" % [_player.power]
	var ink: int
	if _player.power >= _player.max_power:
		ink = Ink.BRIGHT_GREEN
	elif _player.power > _player.max_power * Constants.HIT_POINT_WARNING / 5:
		ink = Ink.YELLOW
	elif _player.power > _player.max_power * Constants.HIT_POINT_WARNING / 5:
		ink = Ink.ORANGE
	else:
		ink = Ink.RED
	Console.print_at(ScreenLocation.ROW_CUR_SP, ScreenLocation.COL_CUR_SP + 7, text, ink)


func print_gold() -> void:
	Console.print_at(ScreenLocation.ROW_GOLD, ScreenLocation.COL_GOLD, "GP ", Ink.WHITE)
	Console.print_at(ScreenLocation.ROW_GOLD, ScreenLocation.COL_GOLD + 3, "%9d" % [_player.gold], Ink.BRIGHT_GREEN)


func print_depth() -> void:
	var text: String
	var region: WildernessRegion = _savegame.island.get_region(_player.wilderness_row, _player.wilderness_col)
	if _savegame.current_depth == 0:
		if region.dungeon != null:
			text = region.dungeon.short_name
			region.dungeon.visited = true
		else:
			text = "Wild (%d,%d)" % [_player.wilderness_col, _player.wilderness_row]
	else:
		text = "Lvl %d" % [_savegame.current_depth + _savegame.dungeon_difficulty]
		_savegame.current_dungeon.known_offset = true
		if _savegame.current_depth == _savegame.current_dungeon.max_level:
			_savegame.current_dungeon.known_depth = true
	Console.print_at(ScreenLocation.ROW_DEPTH, ScreenLocation.COL_DEPTH, text, Ink.WHITE)


func print_health_tracker() -> void:
	var index: int = _savegame.tracked_monster_index
	if index == 0:
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO, "            ", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO - 1, ScreenLocation.COL_INFO, "            ", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO - 2, ScreenLocation.COL_INFO, "            ", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO - 3, ScreenLocation.COL_INFO, "            ", Ink.WHITE)
	elif not _dungeon_level.dungeon_monsters.monsters[index].is_visible:
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO, "[----------]", Ink.WHITE)
	elif _player.timed_hallucinations != 0:
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO, "[----------]", Ink.WHITE)
	elif _dungeon_level.dungeon_monsters.monsters[index].health < 0:
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO, "[----------]", Ink.WHITE)
	else:
		var monster: Monster = _dungeon_level.dungeon_monsters.monsters[index]
		var ink: int = Ink.RED
		var smb: String = "**********"
		var pct: int = 100 * monster.health / monster.max_health
		if pct >= 10:
			ink = Ink.BRIGHT_RED
		if pct >= 25:
			ink = Ink.ORANGE
		if pct >= 60:
			ink = Ink.YELLOW
		if pct >= 100:
			ink = Ink.BRIGHT_GREEN
		if monster.fear_level > 0:
			ink = Ink.PURPLE
			smb = "AFRAID****"
		if monster.confusion_level > 0:
			ink = Ink.TURQUOISE
			smb = "CONFUSED**"
		if monster.sleep_level > 0:
			ink = Ink.BLUE
			smb = "SLEEPING**"
		if monster.mind.is_set(MonsterMind.FRIENDLY):
			ink = Ink.BRIGHT_BROWN
			smb = "FRIENDLY**"
		var length: int = 1 if pct < 10 else 10 if pct > 90 else pct / 10 + 1
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO, "[----------]", Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO, ScreenLocation.COL_INFO + 1, smb.left(length), ink)
		Console.print_at(ScreenLocation.ROW_INFO - 3, ScreenLocation.COL_INFO, monster.monster_type.split_name_1, Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO - 2, ScreenLocation.COL_INFO, monster.monster_type.split_name_2, Ink.WHITE)
		Console.print_at(ScreenLocation.ROW_INFO - 1, ScreenLocation.COL_INFO, monster.monster_type.split_name_3, Ink.WHITE)


func print_cuts() -> void:
	var c: int = _player.timed_bleeding
	if c > 1000:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Mortal wound", Ink.BRIGHT_RED)
	elif c > 200:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Deep gash   ", Ink.RED)
	elif c > 100:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Severe cut  ", Ink.RED)
	elif c > 50:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Nasty cut   ", Ink.ORANGE)
	elif c > 25:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Bad cut     ", Ink.BRIGHT_ORANGE)
	elif c > 10:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Light cut   ", Ink.YELLOW)
	elif c > 0:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "Graze       ", Ink.BRIGHT_YELLOW)
	else:
		Console.print_at(ScreenLocation.ROW_CUT, ScreenLocation.COL_CUT, "            ", Ink.WHITE)


func print_stun() -> void:
	var c: int = _player.timed_stun
	if c > 100:
		Console.print_at(ScreenLocation.ROW_STUN, ScreenLocation.COL_STUN, "Knocked out ", Ink.RED)
	elif c > 50:
		Console.print_at(ScreenLocation.ROW_STUN, ScreenLocation.COL_STUN, "Heavy stun  ", Ink.ORANGE)
	elif c > 0:
		Console.print_at(ScreenLocation.ROW_STUN, ScreenLocation.COL_STUN, "Stun        ", Ink.BRIGHT_ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_STUN, ScreenLocation.COL_STUN, "            ", Ink.WHITE)


func print_hunger() -> void:
	var c: int = _player.food
	if c >= Constants.FOOD_GORGED:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "Gorged", Ink.GREEN)
	elif c >= Constants.FOOD_FULL:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "Full  ", Ink.BRIGHT_GREEN)
	elif c >= Constants.FOOD_ALERT:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "      ", Ink.WHITE)
	elif c >= Constants.FOOD_WEAK:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "Hungry", Ink.YELLOW)
	elif c >= Constants.FOOD_FAINT:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "Weak  ", Ink.ORANGE)
	else:
		Console.print_at(ScreenLocation.ROW_HUNGRY, ScreenLocation.COL_HUNGRY, "Weak! ", Ink.RED)


func print_trap_detection() -> void:
	var count: int = 0
	if _dungeon_level.get_grid(_player.map_row, _player.map_col).tile_flags.is_clear(GridTile.TRAPS_DETECTED):
		Console.print_at(ScreenLocation.ROW_DTRAP, ScreenLocation.COL_DTRAP, "     ", Ink.WHITE)
		return
	if _dungeon_level.get_grid(_player.map_row + 1, _player.map_col).tile_flags.is_set(GridTile.TRAPS_DETECTED):
		count += 1
	if _dungeon_level.get_grid(_player.map_row - 1, _player.map_col).tile_flags.is_set(GridTile.TRAPS_DETECTED):
		count += 1
	if _dungeon_level.get_grid(_player.map_row, _player.map_col + 1).tile_flags.is_set(GridTile.TRAPS_DETECTED):
		count += 1
	if _dungeon_level.get_grid(_player.map_row, _player.map_col - 1).tile_flags.is_set(GridTile.TRAPS_DETECTED):
		count += 1
	if count == 4:
		Console.print_at(ScreenLocation.ROW_DTRAP, ScreenLocation.COL_DTRAP, "DTrap", Ink.GREEN)
	else:
		Console.print_at(ScreenLocation.ROW_DTRAP, ScreenLocation.COL_DTRAP, "DTrap", Ink.YELLOW)


func print_equippy() -> void:
	CharacterViewer.print_equippy(_player, ScreenLocation.ROW_EQUIPPY, ScreenLocation.COL_EQUIPPY)


func print_time() -> void:
	Console.print_at(ScreenLocation.ROW_TIME, ScreenLocation.COL_TIME, "Time", Ink.WHITE)
	Console.print_at(ScreenLocation.ROW_DATE, ScreenLocation.COL_DATE, "Day", Ink.WHITE)
	Console.print_at(ScreenLocation.ROW_TIME, ScreenLocation.COL_TIME + 4, _player.game_time.time_string(), Ink.BRIGHT_GREEN)
	Console.print_at(ScreenLocation.ROW_DATE, ScreenLocation.COL_DATE + 4, _player.game_time.date_string(), Ink.BRIGHT_GREEN)


func _martial_artist_empty_hands() -> bool:
	if _player.profession.is_martial_artist == false:
		return false
	return _player.inventory.items[InventorySlot.MELEE_WEAPON].item_type == null
