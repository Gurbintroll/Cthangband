# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CharacterViewer
extends RefCounted

var _player: Player


static func print_equippy(player: Player, row: int, col: int) -> void:
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		var item: Item = player.inventory.items[i]
		var ink: int = Ink.WHITE
		var character: String = " "
		if item.item_type != null:
			ink = item.item_type.colour
			character = item.item_type.character
		Console.print_at(row, col + i - InventorySlot.MELEE_WEAPON, character, ink)


func _init(player: Player) -> void:
	_player = player


func display_player() -> void:
	Console.clear_screen()
	_display_player_top()
	_display_player_history()
	_display_player_ability_scores_with_effects()
	_display_player_ability_scores_with_modifiers()
	_display_player_essentials()
	_display_player_skills()


func _display_player_top() -> void:
	var realm_text: String = ""
	Console.print_at(2, 1, "Name        :", Ink.BLUE)
	Console.print_at(3, 1, "Gender      :", Ink.BLUE)
	Console.print_at(4, 1, "Ancestry    :", Ink.BLUE)
	Console.print_at(5, 1, "Profession  :", Ink.BLUE)
	if _player.realm_1 != Realm.NONE or _player.realm_2 != Realm.NONE:
		Console.print_at(6, 1, "Magic       :", Ink.BLUE)
	Console.print_at(2, 15, _player.name, Ink.BROWN)
	Console.print_at(3, 15, _player.gender.title, Ink.BROWN)
	Console.print_at(4, 15, _player.ancestry.title, Ink.BROWN)
	Console.print_at(5, 15, _player.profession.get_profession_sub_name(_player.realm_1), Ink.BROWN)
	if _player.realm_1 != Realm.NONE:
		if _player.realm_2 != Realm.NONE:
			realm_text = Spellcasting.realm_name(_player.realm_1) + "/" + Spellcasting.realm_name(
				_player.realm_2
			)
		else:
			realm_text = Spellcasting.realm_name(_player.realm_1)
		Console.print_at(6, 15, realm_text, Ink.BROWN)
	# Some characters get a patron
	if _player.profession.has_goo_patron:
		Console.print_at(7, 1, "Patron      :", Ink.BLUE)
		Console.print_at(7, 15, _player.goo_patron.long_name, Ink.BROWN)
	# Some characters get a deity
	if _player.profession.has_deity:
		Console.print_at(7, 1, "Deity       :", Ink.BLUE)
		Console.print_at(7, 15, _player.religion.get_patron().long_name, Ink.BROWN)
	Console.print_at(2, 32, "Birthday", Ink.BLUE)
	Console.print_at(2, 46, "%8s" % [_player.game_time.get_birthday_text()], Ink.BROWN)
	_print_small(3, 32, "Age          ", _player.age, Ink.BROWN)
	_print_small(4, 32, "Height       ", _player.height, Ink.BROWN)
	_print_small(5, 32, "Weight       ", _player.weight, Ink.BROWN)
	_print_small(6, 32, "Social Class ", _player.social_class, Ink.BROWN)
	# Simple ability scores but no detail
	for i: int in 6:
		var score: AbilityScore = _player.ability_scores[i]
		if score.innate < score.innate_max:
			Console.print_at(2 + i, 61, Tables.ABILITY_NAMES_REDUCED[i], Ink.BLUE)
			Console.print_at(2 + i, 66, Util.stat_to_string(score.adjusted), Ink.RED)
			Console.print_at(2 + i, 73, Util.stat_to_string(score.adjusted_max), Ink.GREEN)
		else:
			Console.print_at(2 + i, 61, Tables.ABILITY_NAMES[i], Ink.BLUE)
			Console.print_at(2 + i, 66, Util.stat_to_string(score.adjusted), Ink.GREEN)


func _display_player_history() -> void:
	for i: int in 4:
		Console.print_at(i + 9, 10, _player.history[i], Ink.BROWN)


func _display_player_ability_scores_with_effects() -> void:
	for i: int in 6:
		var score: AbilityScore = _player.ability_scores[i]
		if score.innate < score.innate_max:
			Console.print_at(14 + i, 1, Tables.ABILITY_NAMES_REDUCED[i], Ink.BLUE)
			Console.print_at(14 + i, 6, Util.stat_to_string(score.adjusted), Ink.RED)
			Console.print_at(14 + i, 13, _ability_summary(i), Ink.RED)
		else:
			Console.print_at(14 + i, 1, Tables.ABILITY_NAMES[i], Ink.BLUE)
			Console.print_at(14 + i, 6, Util.stat_to_string(score.adjusted), Ink.GREEN)
			Console.print_at(14 + i, 13, _ability_summary(i), Ink.GREEN)


func _ability_summary(ability_index: int) -> String:
	var bonus_1: String
	var bonus_2: String
	var bonus_3: String
	var bonus_4: String
	var bonus_5: String
	var ability: AbilityScore = _player.ability_scores[ability_index]
	match ability_index:
		Ability.STRENGTH:
			bonus_1 = "%+d to hit" % [ability.str_attack_bonus()]
			bonus_2 = ", %+d damage" % [ability.str_damage_bonus()]
			bonus_3 = ", carry %dlb" % [ability.str_carrying_capacity() * 5]
			bonus_4 = ", wield %dlb" % [ability.str_max_weapon_weight()]
			bonus_5 = ", %d%% digging" % [ability.str_digging_bonus()]
		Ability.INTELLIGENCE:
			bonus_1 = "%+d device" % [ability.int_use_device_bonus()]
			bonus_2 = ", %+d%% disarm" % [ability.int_disarm_bonus()]
		Ability.WISDOM:
			bonus_1 = "%+d save" % [ability.wis_saving_throw_bonus()]
		Ability.DEXTERITY:
			bonus_1 = "%+d to hit" % [ability.dex_attack_bonus()]
			bonus_2 = ", %+d%% disarm" % [ability.dex_disarm_bonus()]
			bonus_3 = ", %+d AC" % [ability.dex_armour_class_bonus()]
			bonus_4 = ", %+d%% anti-theft" % [ability.dex_theft_avoidance()]
		Ability.CONSTITUTION:
			var hits: int = ability.con_health_bonus()
			if hits == -1:
				bonus_1 = "-0.5 HP/lvl"
			else:
				if hits % 2 == 0:
					bonus_1 = "%+d HP/lvl" % [hits / 2]
				else:
					bonus_1 = "%+d.5 HP/lvl" % [hits / 2]
			bonus_2 = ", x%d recovery" % [ability.con_recovery_speed() + 1]
		Ability.CHARISMA:
			bonus_1 = "%d%% prices" % [ability.cha_price_adjustment()]
	if _player.spellcasting.spell_stat == ability_index and ability_index != Ability.STRENGTH:
		# Casting abilities only have one or two inherent bonuses, so it's safe to start at 3
		var power: int = ability.power_bonus()
		if power % 2 == 0:
			bonus_3 = ", %+d PP/lvl" % [power / 2]
		else:
			bonus_3 = ", %+d.5 PP/lvl" % [power / 2]
		# Not all casting classes have actual spells
		if _player.profession.has_spells:
			var spells: int = ability.half_spells_per_level()
			if spells == 2:
				bonus_4 = ", 1 spell/lvl"
			elif spells % 2 == 0:
				bonus_4 = ", %d spells/lvl" % [spells / 2]
			else:
				bonus_4 = ", %d.5 spells/lvl" % [spells / 2]
		if _player.profession.has_spell_failure_chance:
			bonus_5 = ", %d%% min fail" % [ability.spell_min_fail_chance()]
	return "%s%s%s%s%s" % [bonus_1, bonus_2, bonus_3, bonus_4, bonus_5]


func _display_player_ability_scores_with_modifiers() -> void:
	var f1: FlagSet = FlagSet.new()
	var f2: FlagSet = FlagSet.new()
	var f3: FlagSet = FlagSet.new()
	# Headers
	Console.print_at(21, 6, "Initial", Ink.PURPLE)
	Console.print_at(21, 14, "Anc. Prof. Mods", Ink.BROWN)
	Console.print_at(21, 30, "Actual", Ink.GREEN)
	Console.print_at(21, 37, "Reduced", Ink.RED)
	# Loop through our scores
	for i: int in 6:
		var score: AbilityScore = _player.ability_scores[i]
		# Reverse engineer our equipment bonuses from our score
		var equipment_bonus: int = 0
		# If both are above 18, each "point" is actually ten points
		if score.innate_max > 18 and score.adjusted_max > 18:
			equipment_bonus = (score.adjusted_max - score.innate_max) / 10
		# If both are 18 or less, it's simple
		elif score.innate_max <= 18 and score.adjusted_max <= 18:
			equipment_bonus = score.adjusted_max - score.innate_max
		# Our equipment bonus might have taken us above 18
		elif score.innate_max <= 18 and score.adjusted_max > 18:
			equipment_bonus = ((score.adjusted_max - 18) / 10) - score.innate_max + 18
		# We have equipment penalties taking us below 18
		else:
			equipment_bonus = score.adjusted_max - ((score.innate_max - 18) / 10) - 19
		# Take out the bonuses we got for our ancestry and profession
		equipment_bonus -= _player.ancestry.ability_bonus[i]
		equipment_bonus -= _player.profession.ability_bonus[i]
		# Now print each of the scores and bonuses
		Console.print_at(22 + i, 1, Tables.ABILITY_NAMES[i], Ink.BLUE)
		Console.print_at(22 + i, 5, Util.stat_to_string(score.innate_max), Ink.PURPLE)
		Console.print_at(22 + i, 14, "%+3d" % [_player.ancestry.ability_bonus[i]], Ink.BROWN)
		Console.print_at(22 + i, 20, "%+3d" % [_player.profession.ability_bonus[i]], Ink.BROWN)
		Console.print_at(22 + i, 25, "%+3d" % [equipment_bonus], Ink.BROWN)
		Console.print_at(22 + i, 28, Util.stat_to_string(score.adjusted_max), Ink.GREEN)
		if score.adjusted < score.adjusted_max:
			Console.print_at(22 + i, 36, Util.stat_to_string(score.adjusted), Ink.RED)
		# Print the bonuses for each score and each item we have
		Console.print_at(21, 45, "abcdefghijklm@", Ink.BLUE)
		Console.print_at(28, 45, "Modifications", Ink.BLUE)
		for item_index: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
			var item: Item = _player.inventory.items[item_index]
			item.get_known_flags(f1, f2, f3)
			var ink: int = Ink.GREY
			var ch: String = "."
			for stat: int in 6:
				if f1.is_set(1 << stat):
					ch = "*"
					if item.type_specific_value > 0:
						ink = Ink.GREEN
						if item.type_specific_value < 10:
							ch = "%d" % [item.type_specific_value]
					elif item.type_specific_value < 0:
						ink = Ink.RED
						if item.type_specific_value > -10:
							ch = "%d" % [-item.type_specific_value]
				if f2.is_set(1 << stat):
					ink = Ink.GREEN
					ch = "s"
				Console.print_at(22 + stat, 18 + item_index, ch, ink)
		# Fake a set of item flags for our character to show along with those of the real items
		_player.get_abilities_as_item_flags(f1, f2, f3)
		for stat: int in 6:
			var ink: int = Ink.GREY
			var ch: String = "."
			var extra_modifier: int = 0
			match stat:
				Ability.STRENGTH:
					extra_modifier += _player.genome.str_bonus
				Ability.INTELLIGENCE:
					extra_modifier += _player.genome.int_bonus
				Ability.WISDOM:
					extra_modifier += _player.genome.wis_bonus
				Ability.DEXTERITY:
					extra_modifier += _player.genome.dex_bonus
				Ability.CONSTITUTION:
					extra_modifier += _player.genome.con_bonus
				Ability.CHARISMA:
					extra_modifier += _player.genome.cha_bonus
					if _player.genome.cha_override:
						extra_modifier = 0
			if extra_modifier != 0:
				ch = "*"
				if extra_modifier > 0:
					ink = Ink.GREEN
					if extra_modifier < 10:
						ch = "%d" % [extra_modifier]
				elif extra_modifier < 0:
					ink = Ink.RED
					if extra_modifier > -10:
						ch = "%d" % [-extra_modifier]
			if f2.is_set(1 << stat):
				ink = Ink.GREEN
				ch = "s"
			Console.print_at(22 + stat, 58, ch, ink)


func _display_player_essentials() -> void:
	var show_to_hit: int = _player.displayed_attack_bonus
	var show_to_dam: int = _player.displayed_damage_bonus
	var item: Item = _player.inventory.items[InventorySlot.MELEE_WEAPON]
	# Only show bonuses if we know them
	if item.is_known():
		show_to_hit += item.bonus_to_hit
		show_to_dam += item.bonus_damage
	# Print some basics
	_print_bonus(30, 1, "+ To Hit    ", show_to_hit, Ink.BROWN)
	_print_bonus(31, 1, "+ To Damage ", show_to_dam, Ink.BROWN)
	_print_bonus(32, 1, "+ To AC     ", _player.displayed_armour_class_bonus, Ink.BROWN)
	_print_small(33, 1, "  Base AC   ", _player.displayed_base_armour_class, Ink.BROWN)
	_print_small(30, 28, "Level      ", _player.level, Ink.GREEN)
	_print_large(
		31,
		28,
		"Experience ",
		_player.experience_points,
		Ink.GREEN if _player.max_experience_gained <= _player.experience_points else Ink.RED,
	)
	_print_large(32, 28, "Max Exp    ", _player.max_experience_gained, Ink.GREEN)
	# If we're max level we don't have any experience to advance
	if _player.level >= Constants.MAX_EXPERIENCE_LEVEL:
		Console.print_at(33, 28, "Exp to Adv.", Ink.BLUE)
		Console.print_at(33, 39, "    *****", Ink.GREEN)
	else:
		var required: int = (Tables.BASE_LEVEL_THRESHOLDS[_player.level - 1]
		* _player.experience_multiplier) / 100
		_print_large(33, 28, "Exp to Adv.", required, Ink.GREEN)
	_print_large(34, 28, "Exp Factor ", _player.experience_multiplier, Ink.GREEN)
	_print_small(30, 52, "Max Hit Points   ", _player.max_health, Ink.GREEN)
	if _player.health >= _player.max_health:
		_print_small(31, 52, "Cur Hit Points   ", _player.health, Ink.GREEN)
	elif _player.health > _player.max_health * Constants.HIT_POINT_WARNING / 10:
		_print_small(31, 52, "Cur Hit Points   ", _player.health, Ink.BRIGHT_YELLOW)
	else:
		_print_small(31, 52, "Cur Hit Points   ", _player.health, Ink.BRIGHT_RED)
	_print_small(32, 52, "Max Power Points ", _player.max_power, Ink.GREEN)
	if _player.power >= _player.max_power:
		_print_small(33, 52, "Cur Power Points ", _player.power, Ink.GREEN)
	elif _player.power > _player.max_power * Constants.HIT_POINT_WARNING / 10:
		_print_small(33, 52, "Cur Power Points ", _player.power, Ink.BRIGHT_YELLOW)
	else:
		_print_small(33, 52, "Cur Power Points ", _player.power, Ink.BRIGHT_RED)
	Console.print_at(34 ,52, "Pets", Ink.BLUE)
	var max_pets: int = 1 + (_player.level / _player.profession.pet_upkeep_divider)
	var pets: int = SaveGame.instance.total_friends
	var pet_string: String = "%d/%d" % [pets, max_pets]
	if pets > max_pets:
		Console.print_at(34,78-pet_string.length(), pet_string, Ink.BRIGHT_RED)
	elif pets == max_pets:
		Console.print_at(34,78-pet_string.length(), pet_string, Ink.BRIGHT_YELLOW)
	else:
		Console.print_at(34,78-pet_string.length(), pet_string, Ink.GREEN)

func _display_player_skills() -> void:
	var item: Item = _player.inventory.items[InventorySlot.RANGED_WEAPON]
	var temp: int = _player.attack_bonus + item.bonus_to_hit
	var shooting: int = _player.skill_ranged + (temp * Constants.ITEM_TO_HIT_MULTIPLIER)
	item = _player.inventory.items[InventorySlot.MELEE_WEAPON]
	temp = _player.attack_bonus + item.bonus_to_hit
	var fighting: int = _player.skill_melee + (temp * Constants.ITEM_TO_HIT_MULTIPLIER)
	var dam_bonus: int = _player.displayed_damage_bonus
	if item.is_known():
		dam_bonus += item.bonus_damage
	var dam_dice: int = item.damage_dice
	var dam_sides: int = item.damage_dice_sides
	var dam_dice_text: String
	if dam_dice == 0 or dam_sides == 0:
		dam_dice_text = "nil!"
	else:
		if dam_bonus == 0:
			dam_dice_text = "%dd%d" % [dam_dice * _player.melee_attacks_per_round, dam_sides]
		else:
			dam_dice_text = "%dd%d+%d" % [
				dam_dice * _player.melee_attacks_per_round,
				dam_sides,
				dam_bonus * _player.melee_attacks_per_round,
			]
	Console.print_at(36, 1, "Fighting    :", Ink.BLUE)
	Console.print_at(37, 1, "Shooting    :", Ink.BLUE)
	Console.print_at(38, 1, "Saving Throw:", Ink.BLUE)
	Console.print_at(39, 1, "Stealth     :", Ink.BLUE)
	Console.print_at(36, 28, "Perception  :", Ink.BLUE)
	Console.print_at(37, 28, "Searching   :", Ink.BLUE)
	Console.print_at(38, 28, "Disarming   :", Ink.BLUE)
	Console.print_at(39, 28, "Magic Device:", Ink.BLUE)
	Console.print_at(36, 55, "Blows/Action:", Ink.BLUE)
	Console.print_at(37, 55, "Tot.Dmg./Act:", Ink.BLUE)
	Console.print_at(38, 55, "Shots/Action:", Ink.BLUE)
	Console.print_at(39, 55, "Infra-Vision:", Ink.BLUE)
	_print_scale(36, 15, fighting, 12)
	_print_scale(37, 15, shooting, 12)
	_print_scale(38, 15, _player.skill_saving_throw, 6)
	_print_scale(39, 15, _player.skill_stealth, 1)
	_print_scale(36, 42, _player.skill_search_frequency, 6)
	_print_scale(37, 42, _player.skill_searching, 6)
	_print_scale(38, 42, _player.skill_disarm_traps, 8)
	_print_scale(39, 42, _player.skill_use_device, 6)
	Console.print_at(36, 69, "%d" % [_player.melee_attacks_per_round], Ink.GREEN)
	Console.print_at(37, 69, dam_dice_text, Ink.RED if dam_dice_text == "nil!" else Ink.GREEN)
	Console.print_at(38, 69, "%d" % [_player.missile_attacks_per_round], Ink.GREEN)
	Console.print_at(39, 69, "%d feet" % [_player.infravision_range * 10], Ink.GREEN)


func _print_small(row: int, col: int, header: String, value: int, ink: int) -> void:
	var l: int = header.length()
	Console.print_at(row, col, header, Ink.BLUE)
	Console.print_at(row, col + l, "   ", Ink.BLUE)
	Console.print_at(row, col + l + 3, "%6d" % [value], ink)


func _print_bonus(row: int, col: int, header: String, value: int, ink: int) -> void:
	var l: int = header.length()
	Console.print_at(row, col, header, Ink.BLUE)
	Console.print_at(row, col + l, "   ", Ink.BLUE)
	Console.print_at(row, col + l + 3, "%+6d" % [value], ink)


func _print_large(row: int, col: int, header: String, value: int, ink: int) -> void:
	var l: int = header.length()
	Console.print_at(row, col, header, Ink.BLUE)
	Console.print_at(row, col + l, "%9d" % [value], ink)


func _print_scale(row: int, col: int, value: int, scale: int) -> void:
	if scale <= 0:
		scale = 1
	var colour: int
	var text: String
	var value_string: String = "%3d%%" % [value]
	if value < 0:
		colour = Ink.BLACK
		text = "Awful   " + value_string
	else:
		match (value / scale):
			0, 1:
				colour = Ink.RED
				text = "Bad     " + value_string
			2:
				colour = Ink.BRIGHT_RED
				text = "Poor    " + value_string
			3, 4:
				colour = Ink.PINK
				text = "Medium  " + value_string
			5:
				colour = Ink.ORANGE
				text = "Fair    " + value_string
			6:
				colour = Ink.BRIGHT_YELLOW
				text = "Good    " + value_string
			7, 8:
				colour = Ink.GREEN
				text = "Great   " + value_string
			9, 10, 11, 12, 13:
				colour = Ink.BRIGHT_GREEN
				text = "Superb  " + value_string
			14, 15, 16, 17:
				colour = Ink.BLUE
				text = "Amazing " + value_string
			_:
				colour = Ink.PURPLE
				text = "Stellar " + value_string
	Console.print_at(row, col, text, colour)
