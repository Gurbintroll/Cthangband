# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PlayerFactory
extends RefCounted
## A class factory for creating a player class

## A set of spaces the correct width to blank out a field
const SPACES: String = "                 "

var _player: Player
var _previous_profession: String
var _previous_generation: int
var _previous_name: String
var _previous_ancestry: String
var _previous_realm_1: int
var _previous_realm_2: int
var _previous_sex: int
var _sex_info: Array[Gender] = []
var _ancestry_menu: Array[MenuItem] = []
var _profession_menu: Array[MenuItem] = []
var _menu_length: int
var _menu_items: Array[String]


func _init() -> void:
	# Initialise our ancestry and profession menus
	_ancestry_menu = []
	var list: Array[String] = Ancestry.sorted_ancestry_names()
	for i: int in list.size():
		_ancestry_menu.append(MenuItem.new(list[i], i))
	_profession_menu = []
	list = Profession.sorted_profession_names()
	for i: int in list.size():
		_profession_menu.append(MenuItem.new(list[i], i))
	# Initialise genders
	var gender: Gender = Gender.new()
	gender.title = "Female"
	gender.winner = "Queen"
	_sex_info.append(gender)
	gender = Gender.new()
	gender.title = "Male"
	gender.winner = "King"
	_sex_info.append(gender)
	gender = Gender.new()
	gender.title = "Other"
	gender.winner = "Monarch"
	_sex_info.append(gender)
	# Set up the menu item array
	for i: int in 50:
		_menu_items.append("")


func generate_character(ex: ExPlayer) -> Player:
	Console.set_background(BackgroundImage.PAPER)
	Console.change_music("Chargen")
	_player = Player.new_player()
	return _player if create_player(ex) else null


func create_player(ex_player: ExPlayer) -> bool:
	# Set up the defaults for the player from the previous one
	if ex_player == null:
		_previous_ancestry = "Human"
		_previous_profession = "Warrior"
		_previous_sex = GenderType.FEMALE
		_previous_realm_1 = Realm.NONE
		_previous_realm_2 = Realm.NONE
		_previous_generation = 0
		_previous_name = "Xena"
	else:
		_previous_ancestry = ex_player.birth_ancestry
		_previous_profession = ex_player.current_profession
		_previous_sex = ex_player.gender
		_previous_realm_1 = ex_player.realm_1
		_previous_realm_2 = ex_player.realm_2
		_previous_generation = ex_player.generation
		_previous_name = ex_player.name
	if not make_chargen_choices():
		return false
	_player.birth_ancestry = _player.current_ancestry
	Profile.instance.add_message_to_log(" ")
	Profile.instance.add_message_to_log("  ")
	Profile.instance.add_message_to_log("====================")
	Profile.instance.add_message_to_log("  ")
	Profile.instance.add_message_to_log(" ")
	_player.is_dead = false
	_player_outfit()
	for type: MonsterType in Profile.instance.monster_types.types:
		type.cur_num = 0
		type.max_num = 1 if type.flags_1.is_set(MonsterFlags1.UNIQUE) else 100
		type.knowledge.r_kills = 0
	return true


func make_chargen_choices() -> bool:
	var stage: int = BirthStage.INTRODUCTION
	var menu: Array[int] = [0, 0, 0, 0, 0, 0, 0, 0, 0]
	var auto_chose: Array[bool] = [false, false, false, false, false, false, false, false, false]
	# Set our default ancestry/profession to human warrior
	menu[BirthStage.CLASS_SELECTION] = 15
	menu[BirthStage.RACE_SELECTION] = 16
	Console.clear_screen()
	while true:
		match stage:
			BirthStage.INTRODUCTION:
				# Start with no patron deity
				_player.religion.patron = GodName.NONE
				# Reset which options have been auto-selected
				auto_chose = [false, false, false, false, false, false, false, false]
				# Simple first menu
				_menu_items[0] = "Choose"
				_menu_items[1] = "Random"
				_menu_items[2] = "Re-use"
				_menu_length = 3
				_display_partial_character(stage)
				_display_menu(menu[stage])
				match menu[stage]:
					0:
						Console.print_at(35, 20, "Choose your character's ancestry, gender, and profession;", Ink.PURPLE)
						Console.print_at(36, 20, " and select which realms of magic your character will use.", Ink.PURPLE)
					1:
						Console.print_at(35, 20, "Let the game generate a character for you randomly.", Ink.PURPLE)
					2:
						Console.print_at(35, 20, "Re-play with a character similar to the one you played", Ink.PURPLE)
						Console.print_at(36, 20, "last time.", Ink.PURPLE)
				Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
				while true:
					var key: String = User.get_key()
					if key == "Up":
						if menu[stage] > 0:
							Console.play_sound("MenuSelect")
							menu[stage] -= 1
							break
					if key == "Down":
						if menu[stage] < _menu_length - 1:
							Console.play_sound("MenuSelect")
							menu[stage] += 1
							break
					if key == "Left":
						Console.play_sound("MenuAccept")
						return false
					if key == "Right":
						Console.play_sound("MenuAccept")
						stage += 1
						break
					if key == "h":
						Console.play_sound("MenuAccept")
						Console.show_cthangpedia()
			BirthStage.CLASS_SELECTION:
				_player.religion.patron = GodName.NONE
				if menu[0] == BirthStyle.RE_PLAY:
					auto_chose[stage] = true
					_player.current_profession = _previous_profession
					_player.profession = Profession.named_profession(_previous_profession)
					stage += 1
				elif menu[0] == BirthStyle.RANDOM:
					auto_chose[stage] = true
					var list: Array[String] = Profession.sorted_profession_names()
					var choice: int = Rng.roll_die(list.size()) - 1
					_player.current_profession = list[choice]
					_player.profession = Profession.named_profession(_player.current_profession)
					stage += 1
				else:
					auto_chose[stage] = false
					_menu_length = _profession_menu.size()
					for i: int in _menu_length:
						_menu_items[i] = _profession_menu[i].text
					_display_partial_character(stage)
					menu[stage] = clampi(menu[stage], 0, _menu_length - 1)
					_display_menu(menu[stage])
					_display_class_info(_profession_menu[menu[stage]].text)
					Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
					while true:
						var key: String = User.get_key()
						if key == "Up":
							if menu[stage] > 0:
								Console.play_sound("MenuSelect")
								menu[stage] -= 1
								break
						if key == "Down":
							if menu[stage] < _menu_length - 1:
								Console.play_sound("MenuSelect")
								menu[stage] += 1
								break
						if key == "Left":
							Console.play_sound("MenuAccept")
							stage -= 1
							while auto_chose[stage]:
								stage -= 1
							break
						if key == "Right":
							Console.play_sound("MenuAccept")
							stage += 1
							break
						if key == "h":
							Console.play_sound("MenuAccept")
							Console.show_cthangpedia()
					if stage > BirthStage.CLASS_SELECTION:
						_player.current_profession = _profession_menu[menu[BirthStage.CLASS_SELECTION]].text
						_player.profession = Profession.named_profession(_player.current_profession)
			BirthStage.RACE_SELECTION:
				if menu[0] == BirthStyle.RE_PLAY:
					auto_chose[stage] = true
					_player.current_ancestry = _previous_ancestry
					_player.ancestry = Ancestry.named_ancestry(_previous_ancestry)
					stage += 1
				elif menu[0] == BirthStyle.RANDOM:
					auto_chose[stage] = true
					var list: Array[String] = Ancestry.sorted_ancestry_names()
					var choice: int = Rng.roll_die(list.size()) - 1
					_player.current_ancestry = list[choice]
					_player.ancestry = Ancestry.named_ancestry(_player.current_ancestry)
					stage += 1
				else:
					auto_chose[stage] = false
					_menu_length = _ancestry_menu.size()
					for i: int in _menu_length:
						_menu_items[i] = _ancestry_menu[i].text
					_display_partial_character(stage)
					menu[stage] = clampi(menu[stage], 0, _menu_length - 1)
					_display_menu(menu[stage])
					_display_ancestry_info(_ancestry_menu[menu[stage]].text)
					Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
					while true:
						var key: String = User.get_key()
						if key == "Up":
							if menu[stage] > 0:
								Console.play_sound("MenuSelect")
								menu[stage] -= 1
								break
						if key == "Down":
							if menu[stage] < _menu_length - 1:
								Console.play_sound("MenuSelect")
								menu[stage] += 1
								break
						if key == "Left":
							Console.play_sound("MenuAccept")
							stage -= 1
							while auto_chose[stage]:
								stage -= 1
							break
						if key == "Right":
							Console.play_sound("MenuAccept")
							stage += 1
							break
						if key == "h":
							Console.play_sound("MenuAccept")
							Console.show_cthangpedia()
					if stage > BirthStage.RACE_SELECTION:
						_player.current_ancestry = _ancestry_menu[menu[BirthStage.RACE_SELECTION]].text
						_player.ancestry = Ancestry.named_ancestry(_player.current_ancestry)
						_player.get_first_level_mutation = _player.ancestry.mutates
			BirthStage.REALM_SELECTION_1:
				if menu[0] == BirthStyle.RE_PLAY:
					auto_chose[stage] = true
					_player.realm_1 = _previous_realm_1
					stage += 1
				elif menu[0] == BirthStyle.RANDOM:
					auto_chose[stage] = true
					_get_realms_randomly()
					stage += 1
				else:
					_player.realm_1 = Realm.NONE
					var realm_list: Array[int] = _get_realm_list(_player.profession.first_realm_choice)
					_menu_length = realm_list.size()
					if _menu_length == 1:
						auto_chose[stage] = true
						_player.realm_1 = realm_list[0]
						stage += 1
					else:
						auto_chose[stage] = false
						for i: int in _menu_length:
							_menu_items[i] = Spellcasting.realm_name(realm_list[i])
						_display_partial_character(stage)
						menu[stage] = clampi(menu[stage], 0, _menu_length - 1)
						_display_menu(menu[stage])
						_display_realm_info(realm_list[menu[stage]])
						Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
						while true:
							var key: String = User.get_key()
							if key == "Up":
								if menu[stage] > 0:
									Console.play_sound("MenuSelect")
									menu[stage] -= 1
									break
							if key == "Down":
								if menu[stage] < _menu_length - 1:
									Console.play_sound("MenuSelect")
									menu[stage] += 1
									break
							if key == "Left":
								Console.play_sound("MenuAccept")
								stage -= 1
								while auto_chose[stage]:
									stage -= 1
								break
							if key == "Right":
								Console.play_sound("MenuAccept")
								stage += 1
								break
							if key == "h":
								Console.play_sound("MenuAccept")
								Console.show_cthangpedia()
						if stage > BirthStage.REALM_SELECTION_1:
							_player.realm_1 = realm_list[menu[BirthStage.REALM_SELECTION_1]]
			BirthStage.REALM_SELECTION_2:
				if menu[0] == BirthStyle.RE_PLAY:
					auto_chose[stage] = true
					_player.realm_2 = _previous_realm_2
					if _player.profession.has_deity:
						match _player.realm_2:
							Realm.NATURE:
								_player.religion.patron = GodName.HAGARG_RYONIS
							Realm.FOLK:
								_player.religion.patron = GodName.ZO_KALAR
							Realm.CHAOS:
								_player.religion.patron = GodName.NATH_HORTHATH
							Realm.CORPOREAL:
								_player.religion.patron = GodName.LOBON
							Realm.TAROT:
								_player.religion.patron = GodName.TAMASH
							_:
								_player.religion.patron = GodName.NONE
					else:
						_player.religion.patron = GodName.NONE
					stage += 1
				elif menu[0] == BirthStyle.RANDOM:
					auto_chose[stage] = true
					stage += 1
				else:
					_player.realm_2 = Realm.NONE
					var realm_list: Array[int] = _get_realm_list(_player.profession.second_realm_choice)
					_menu_length = realm_list.size()
					if _menu_length == 1:
						auto_chose[stage] = true
						_player.realm_2 = realm_list[0]
						stage += 1
					else:
						auto_chose[stage] = false
						for i: int in _menu_length:
							_menu_items[i] = Spellcasting.realm_name(realm_list[i])
						_display_partial_character(stage)
						menu[stage] = clampi(menu[stage], 0, _menu_length - 1)
						_display_menu(menu[stage])
						_display_realm_info(realm_list[menu[stage]])
						Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
						while true:
							var key: String = User.get_key()
							if key == "Up":
								if menu[stage] > 0:
									Console.play_sound("MenuSelect")
									menu[stage] -= 1
									break
							if key == "Down":
								if menu[stage] < _menu_length - 1:
									Console.play_sound("MenuSelect")
									menu[stage] += 1
									break
							if key == "Left":
								Console.play_sound("MenuAccept")
								stage -= 1
								while auto_chose[stage]:
									stage -= 1
								break
							if key == "Right":
								Console.play_sound("MenuAccept")
								stage += 1
								break
							if key == "h":
								Console.play_sound("MenuAccept")
								Console.show_cthangpedia()
						if stage > BirthStage.REALM_SELECTION_2:
							_player.realm_2 = realm_list[menu[BirthStage.REALM_SELECTION_2]]
							if _player.profession.has_deity:
								match _player.realm_2:
									Realm.NATURE:
										_player.religion.patron = GodName.HAGARG_RYONIS
									Realm.FOLK:
										_player.religion.patron = GodName.ZO_KALAR
									Realm.CHAOS:
										_player.religion.patron = GodName.NATH_HORTHATH
									Realm.CORPOREAL:
										_player.religion.patron = GodName.LOBON
									Realm.TAROT:
										_player.religion.patron = GodName.TAMASH
									_:
										_player.religion.patron = GodName.NONE
							else:
								_player.religion.patron = GodName.NONE
			BirthStage.GENDER_SELECTION:
				if menu[0] == BirthStyle.RE_PLAY:
					auto_chose[stage] = true
					_player.gender_index = _previous_sex
					_player.gender = _sex_info[_player.gender_index]
					stage += 1
				elif menu[0] == BirthStyle.RANDOM:
					auto_chose[stage] = true
					if _player.ancestry.defaults_to_nonbinary():
						_player.gender_index = 2
						if Rng.roll_die(30) == 1:
							_player.gender_index = Rng.roll_die(2) - 1
					else:
						_player.gender_index = Rng.roll_die(2) - 1
						if Rng.roll_die(30) == 1:
							_player.gender_index = Rng.roll_die(2) - 1
					_player.gender = _sex_info[_player.gender_index]
					stage += 1
				else:
					_menu_length = _sex_info.size()
					auto_chose[stage] = false
					for i: int in _menu_length:
						_menu_items[i] = _sex_info[i].title
					_display_partial_character(stage)
					menu[stage] = clampi(menu[stage], 0, _menu_length - 1)
					_display_menu(menu[stage])
					Console.print_at(35, 21, "Your gender has no effect on gameplay", Ink.PURPLE)
					Console.print_centred(43, "[Use up and down to select an option, right to confirm, or left to go back.]", Ink.ORANGE)
					while true:
						var key: String = User.get_key()
						if key == "Up":
							if menu[stage] > 0:
								Console.play_sound("MenuSelect")
								menu[stage] -= 1
								break
						if key == "Down":
							if menu[stage] < _menu_length - 1:
								Console.play_sound("MenuSelect")
								menu[stage] += 1
								break
						if key == "Left":
							Console.play_sound("MenuAccept")
							stage -= 1
							while auto_chose[stage]:
								stage -= 1
							break
						if key == "Right":
							Console.play_sound("MenuAccept")
							stage += 1
							break
						if key == "h":
							Console.play_sound("MenuAccept")
							Console.show_cthangpedia()
					if stage > BirthStage.GENDER_SELECTION:
						_player.gender_index = menu[BirthStage.GENDER_SELECTION]
						_player.gender = _sex_info[_player.gender_index]
			BirthStage.CONFIRMATION:
				if menu[0] != BirthStyle.RE_PLAY:
					_player.name = _player.ancestry.create_random_name()
					_player.generation = 1
				else:
					if _previous_name == "":
						_player.name = _player.ancestry.create_random_name()
					else:
						_player.name = _previous_name
					_player.generation = _previous_generation + 1
				_get_stats()
				_get_extra()
				_get_ahw()
				_player.ancestry.get_history(_player)
				_get_money()
				_player.spellcasting = Spellcasting.new()
				_player.spellcasting.init_spellcasting(_player)
				var patrons: Array[String] = Patron.sorted_patron_names()
				var patron: String = patrons[Rng.roll_die(patrons.size()) - 1]
				_player.goo_patron = Patron.named_patron(patron)
				_player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH)
				SaveGame.instance.player = _player
				SaveGame.instance.update_stuff()
				SaveGame.instance.player = null
				_player.health = _player.max_health
				_player.power = _player.max_power
				_player.energy = 150
				var character_viewer: CharacterViewer = CharacterViewer.new(_player)
				while true:
					character_viewer.display_player()
					Console.print_centred(43, "[Use return to confirm, or left to go back]", Ink.ORANGE)
					var key: String = User.get_key()
					if key == "Left":
						Console.play_sound("MenuAccept")
						stage -= 1
						while auto_chose[stage]:
							stage -= 1
						break
					if key == "Enter":
						Console.play_sound("MenuAccept")
						stage += 1
						break
					if key == "h":
						Console.play_sound("MenuAccept")
						Console.show_cthangpedia()
			BirthStage.NAMING:
				_player.input_player_name()
				return true
	# Should never get here
	return false


func _display_partial_character(stage: int) -> void:
	var buffer: String = ""
	# Basics - name and gender
	Console.clear_row(0)
	Console.print_at(2, 1, "Name        :", Ink.BLUE)
	Console.print_at(2, 15, _previous_name if stage == 0 else SPACES, Ink.BROWN)
	Console.print_at(3, 1, "Gender      :", Ink.BLUE)
	if stage == BirthStage.INTRODUCTION:
		_player.gender = _sex_info[_previous_sex]
		buffer = _player.gender.title
	elif stage < BirthStage.CONFIRMATION:
		buffer = SPACES
	else:
		_player.gender = _sex_info[_player.gender_index]
		buffer = _player.gender.title
	Console.print_at(3, 15, buffer, Ink.BROWN)
	# Ancestry
	Console.print_at(4, 1, "Ancestry    :", Ink.BLUE)
	if stage == BirthStage.INTRODUCTION:
		_player.ancestry = Ancestry.named_ancestry(_previous_ancestry)
		buffer = _player.ancestry.title
	elif stage < BirthStage.REALM_SELECTION_1:
		buffer = SPACES
	else:
		_player.ancestry = Ancestry.named_ancestry(_player.current_ancestry)
		buffer = _player.ancestry.title
	Console.print_at(4, 15, buffer, Ink.BROWN)
	# Profession
	Console.print_at(5, 1, "Profession  :", Ink.BLUE)
	if stage == BirthStage.INTRODUCTION:
		_player.profession = Profession.named_profession(_previous_profession)
		buffer = _player.profession.title
	elif stage < BirthStage.RACE_SELECTION:
		buffer = SPACES
	else:
		_player.profession = Profession.named_profession(_player.current_profession)
		buffer = _player.profession.title
	Console.print_at(5, 15, buffer, Ink.BROWN)
	# Realms
	if stage == BirthStage.INTRODUCTION:
		if _previous_realm_1 != Realm.NONE:
			if _previous_realm_2 != Realm.NONE:
				buffer = Spellcasting.realm_name(_previous_realm_1) + "/" + Spellcasting.realm_name(_previous_realm_2)
			else:
				buffer = Spellcasting.realm_name(_previous_realm_1)
		if _previous_realm_1 != Realm.NONE or _previous_realm_2 != Realm.NONE:
			Console.print_at(6, 1, "Magic       :", Ink.BLUE)
		if _previous_realm_1 != Realm.NONE:
			Console.print_at(6, 15, buffer, Ink.BROWN)
	elif stage < BirthStage.REALM_SELECTION_2:
		buffer = SPACES
		Console.print_at(6, 0, buffer, Ink.BLUE)
		Console.print_at(6, 15, buffer.rpad(SPACES.length()), Ink.BROWN)
	else:
		if _player.realm_1 != Realm.NONE:
			if _player.realm_2 != Realm.NONE:
				buffer = Spellcasting.realm_name(_player.realm_1) + "/" + Spellcasting.realm_name(_player.realm_2)
			else:
				buffer = Spellcasting.realm_name(_player.realm_1)
		if _player.realm_1 != Realm.NONE or _player.realm_2 != Realm.NONE:
			Console.print_at(6, 1, "Magic       :", Ink.BLUE)
		if _player.realm_1 != Realm.NONE:
			Console.print_at(6, 15, buffer.rpad(SPACES.length()), Ink.BROWN)
	# Print template for remainder of character sheet
	Console.print_at(2, 32, "Birthday", Ink.BLUE)
	Console.print_at(3, 32, "Age          ", Ink.BLUE)
	Console.print_at(4, 32, "Height       ", Ink.BLUE)
	Console.print_at(5, 32, "Weight       ", Ink.BLUE)
	Console.print_at(6, 32, "Social Class ", Ink.BLUE)
	Console.print_at(2 + Ability.STRENGTH, 61, "STR:", Ink.BLUE)
	Console.print_at(2 + Ability.INTELLIGENCE, 61, "INT:", Ink.BLUE)
	Console.print_at(2 + Ability.WISDOM, 61, "WIS:", Ink.BLUE)
	Console.print_at(2 + Ability.DEXTERITY, 61, "DEX:", Ink.BLUE)
	Console.print_at(2 + Ability.CONSTITUTION, 61, "CON:", Ink.BLUE)
	Console.print_at(2 + Ability.CHARISMA, 61, "CHA:", Ink.BLUE)
	Console.print_at(14 + Ability.STRENGTH, 1, "STR:", Ink.BLUE)
	Console.print_at(14 + Ability.INTELLIGENCE, 1, "INT:", Ink.BLUE)
	Console.print_at(14 + Ability.WISDOM, 1, "WIS:", Ink.BLUE)
	Console.print_at(14 + Ability.DEXTERITY, 1, "DEX:", Ink.BLUE)
	Console.print_at(14 + Ability.CONSTITUTION, 1, "CON:", Ink.BLUE)
	Console.print_at(14 + Ability.CHARISMA, 1, "CHA:", Ink.BLUE)
	Console.print_at(22 + Ability.STRENGTH, 1, "STR:", Ink.BLUE)
	Console.print_at(22 + Ability.INTELLIGENCE, 1, "INT:", Ink.BLUE)
	Console.print_at(22 + Ability.WISDOM, 1, "WIS:", Ink.BLUE)
	Console.print_at(22 + Ability.DEXTERITY, 1, "DEX:", Ink.BLUE)
	Console.print_at(22 + Ability.CONSTITUTION, 1, "CON:", Ink.BLUE)
	Console.print_at(22 + Ability.CHARISMA, 1, "CHA:", Ink.BLUE)
	Console.print_at(21, 6, "Initial", Ink.PURPLE)
	Console.print_at(21, 14, "Anc. Prof. Mods", Ink.BROWN)
	Console.print_at(21, 30, "Actual", Ink.GREEN)
	Console.print_at(21, 37, "Reduced", Ink.RED)
	Console.print_at(21, 45, "abcdefghijklm@", Ink.BLUE)
	Console.print_at(22, 45, "..............", Ink.GREY)
	Console.print_at(23, 45, "..............", Ink.GREY)
	Console.print_at(24, 45, "..............", Ink.GREY)
	Console.print_at(25, 45, "..............", Ink.GREY)
	Console.print_at(26, 45, "..............", Ink.GREY)
	Console.print_at(27, 45, "..............", Ink.GREY)
	Console.print_at(28, 45, "Modifications", Ink.BLUE)
	# Print profession ability bonuses
	if stage < BirthStage.RACE_SELECTION:
		for i: int in 6:
			Console.print_at(22 + i, 20, "   ", Ink.BROWN)
	else:
		for i: int in 6:
			Console.print_at(22 + i, 20, "%+3d" % [_player.profession.ability_bonus[i]], Ink.BROWN)
	# Print ancestral ability bonuses
	if stage < BirthStage.REALM_SELECTION_1:
		for i: int in 6:
			Console.print_at(22 + i, 14, "   ", Ink.BROWN)
	else:
		for i: int in 6:
			Console.print_at(22 + i, 14, "%+3d" % [_player.ancestry.ability_bonus[i]], Ink.BROWN)


func _display_menu(current: int) -> void:
	Console.clear_from(30)
	Console.print_at(35, 0, "=>", Ink.BRIGHT_PURPLE)
	for i: int in _menu_length:
		var row: int = 35 + i - current
		if row >= 30 and row <= 40:
			Console.print_at(row, 2, _menu_items[i], Ink.BRIGHT_PURPLE if i == current else Ink.PURPLE)


func _get_realms_randomly() -> void:
	var profession: Profession = _player.profession
	_player.realm_1 = Realm.NONE
	_player.realm_2 = Realm.NONE
	_player.realm_1 = profession.choose_realm_randomly(profession.first_realm_choice, _player)
	_player.realm_2 = profession.choose_realm_randomly(profession.second_realm_choice, _player)
	_select_deity(profession)


func _select_deity(profession: Profession) -> void:
	if profession.has_deity:
		match _player.realm_2:
			Realm.NATURE:
				_player.religion.patron = GodName.HAGARG_RYONIS
			Realm.FOLK:
				_player.religion.patron = GodName.ZO_KALAR
			Realm.CHAOS:
				_player.religion.patron = GodName.NATH_HORTHATH
			Realm.CORPOREAL:
				_player.religion.patron = GodName.LOBON
			Realm.TAROT:
				_player.religion.patron = GodName.TAMASH
			_:
				_player.religion.patron = GodName.NONE
	else:
		_player.religion.patron = GodName.NONE


func _get_realm_list(realms: int) -> Array[int]:
	if realms == Realm.NONE:
		return [Realm.NONE]
	var list: Array[int] = []
	if (realms & Realm.CHAOS) != 0 and (_player.realm_1 != Realm.CHAOS):
		list.append(Realm.CHAOS)
	if (realms & Realm.CORPOREAL) != 0 and (_player.realm_1 != Realm.CORPOREAL):
		list.append(Realm.CORPOREAL)
	if (realms & Realm.DEATH) != 0 and (_player.realm_1 != Realm.DEATH):
		list.append(Realm.DEATH)
	if (realms & Realm.FOLK) != 0 and (_player.realm_1 != Realm.FOLK):
		list.append(Realm.FOLK)
	if (realms & Realm.LIFE) != 0 and (_player.realm_1 != Realm.LIFE):
		list.append(Realm.LIFE)
	if (realms & Realm.NATURE) != 0 and (_player.realm_1 != Realm.NATURE):
		list.append(Realm.NATURE)
	if (realms & Realm.SORCERY) != 0 and (_player.realm_1 != Realm.SORCERY):
		list.append(Realm.SORCERY)
	if (realms & Realm.TAROT) != 0 and (_player.realm_1 != Realm.TAROT):
		list.append(Realm.TAROT)
	return list


func _display_ancestry_info(current: String) -> void:
	var ancestry: Ancestry = Ancestry.named_ancestry(current)
	Console.print_at(36, 21, "STR:", Ink.PURPLE)
	Console.print_at(37, 21, "INT:", Ink.PURPLE)
	Console.print_at(38, 21, "WIS:", Ink.PURPLE)
	Console.print_at(39, 21, "DEX:", Ink.PURPLE)
	Console.print_at(40, 21, "CON:", Ink.PURPLE)
	Console.print_at(41, 21, "CHA:", Ink.PURPLE)
	for i: int in 6:
		var bonus: int = ancestry.ability_bonus[i] + _player.profession.ability_bonus[i]
		_display_stat_bonus(36 + i, 26, bonus)
	Console.print_at(36, 53, "Disarming   :", Ink.PURPLE)
	Console.print_at(37, 53, "Magic Device:", Ink.PURPLE)
	Console.print_at(38, 53, "Saving Throw:", Ink.PURPLE)
	Console.print_at(39, 53, "Stealth     :", Ink.PURPLE)
	Console.print_at(40, 53, "Fighting    :", Ink.PURPLE)
	Console.print_at(41, 53, "Shooting    :", Ink.PURPLE)
	Console.print_at(36, 31, "Experience  :", Ink.PURPLE)
	Console.print_at(37, 31, "Hit Dice    :", Ink.PURPLE)
	Console.print_at(38, 31, "Infravision :", Ink.PURPLE)
	Console.print_at(39, 31, "Searching   :", Ink.PURPLE)
	Console.print_at(40, 31, "Perception  :", Ink.PURPLE)
	_display_a_plus_b(36, 67, ancestry.base_disarm_bonus + _player.profession.base_disarm_bonus, _player.profession.disarm_bonus_per_level)
	_display_a_plus_b(37, 67, ancestry.base_device_bonus + _player.profession.base_device_bonus, _player.profession.device_bonus_per_level)
	_display_a_plus_b(38, 67, ancestry.base_save_bonus + _player.profession.base_save_bonus, _player.profession.save_bonus_per_level)
	_display_a_plus_b(39, 67, (ancestry.base_stealth_bonus + _player.profession.base_stealth_bonus) * 4, _player.profession.stealth_bonus_per_level * 4)
	_display_a_plus_b(40, 67, ancestry.base_melee_attack_bonus + _player.profession.base_melee_attack_bonus, _player.profession.melee_attack_bonus_per_level)
	_display_a_plus_b(41, 67, ancestry.base_ranged_attack_bonus + _player.profession.base_ranged_attack_bonus, _player.profession.ranged_attack_bonus_per_level)
	var buffer: String = "+%d%%" % [ancestry.experience_factor + _player.profession.experience_factor]
	Console.print_at(36, 45, buffer, Ink.BLACK)
	buffer = "1d%d" % [ancestry.hit_die_bonus + _player.profession.hit_die_bonus]
	Console.print_at(37, 45, buffer, Ink.BLACK)
	Console.print_at(38, 45, "-", Ink.BLACK)
	buffer = "%0d%%" % [ancestry.base_search_bonus + _player.profession.base_search_bonus]
	Console.print_at(39, 45, buffer, Ink.BLACK)
	buffer = "%00d%%" % [ancestry.base_search_frequency + _player.profession.base_search_frequency]
	Console.print_at(40, 45, buffer, Ink.BLACK)
	Console.print_line_at(29, 20, ancestry.description_line_1, Ink.PURPLE)
	Console.print_line_at(30, 20, ancestry.description_line_2, Ink.PURPLE)
	Console.print_line_at(31, 20, ancestry.description_line_3, Ink.PURPLE)
	Console.print_line_at(32, 20, ancestry.description_line_4, Ink.PURPLE)
	Console.print_line_at(33, 20, ancestry.description_line_5, Ink.PURPLE)
	Console.print_line_at(34, 20, ancestry.description_line_6, Ink.PURPLE)


func _display_class_info(current: String) -> void:
	var profession: Profession = Profession.named_profession(current)
	Console.print_at(36, 21, "STR:", Ink.PURPLE)
	Console.print_at(37, 21, "INT:", Ink.PURPLE)
	Console.print_at(38, 21, "WIS:", Ink.PURPLE)
	Console.print_at(39, 21, "DEX:", Ink.PURPLE)
	Console.print_at(40, 21, "CON:", Ink.PURPLE)
	Console.print_at(41, 21, "CHA:", Ink.PURPLE)
	for i: int in 6:
		var bonus: int = profession.ability_bonus[i]
		_display_stat_bonus(36 + i, 26, bonus)
	Console.print_at(36, 53, "Disarming   :", Ink.PURPLE)
	Console.print_at(37, 53, "Magic Device:", Ink.PURPLE)
	Console.print_at(38, 53, "Saving Throw:", Ink.PURPLE)
	Console.print_at(39, 53, "Stealth     :", Ink.PURPLE)
	Console.print_at(40, 53, "Fighting    :", Ink.PURPLE)
	Console.print_at(41, 53, "Shooting    :", Ink.PURPLE)
	Console.print_at(36, 31, "Experience  :", Ink.PURPLE)
	Console.print_at(37, 31, "Hit Dice    :", Ink.PURPLE)
	Console.print_at(38, 31, "Infravision :", Ink.PURPLE)
	Console.print_at(39, 31, "Searching   :", Ink.PURPLE)
	Console.print_at(40, 31, "Perception  :", Ink.PURPLE)
	_display_a_plus_b(36, 67, profession.base_disarm_bonus, profession.disarm_bonus_per_level)
	_display_a_plus_b(37, 67, profession.base_device_bonus, profession.device_bonus_per_level)
	_display_a_plus_b(38, 67, profession.base_save_bonus, profession.save_bonus_per_level)
	_display_a_plus_b(39, 67, profession.base_stealth_bonus * 4, profession.stealth_bonus_per_level * 4)
	_display_a_plus_b(40, 67, profession.base_melee_attack_bonus, profession.melee_attack_bonus_per_level)
	_display_a_plus_b(41, 67, profession.base_ranged_attack_bonus, profession.ranged_attack_bonus_per_level)
	var buffer: String = "+%d%%" % profession.experience_factor
	Console.print_at(36, 45, buffer, Ink.BLACK)
	buffer = "1d%d" % profession.hit_die_bonus
	Console.print_at(37, 45, buffer, Ink.BLACK)
	Console.print_at(38, 45, "-", Ink.BLACK)
	buffer = "%0d%%" % profession.base_search_bonus
	Console.print_at(39, 45, buffer, Ink.BLACK)
	buffer = "%00d%%" % profession.base_search_frequency
	Console.print_at(40, 45, buffer, Ink.BLACK)
	Console.print_line_at(29, 20, profession.description_line_1, Ink.PURPLE)
	Console.print_line_at(30, 20, profession.description_line_2, Ink.PURPLE)
	Console.print_line_at(31, 20, profession.description_line_3, Ink.PURPLE)
	Console.print_line_at(32, 20, profession.description_line_4, Ink.PURPLE)
	Console.print_line_at(33, 20, profession.description_line_5, Ink.PURPLE)
	Console.print_line_at(34, 20, profession.description_line_6, Ink.PURPLE)


func _display_a_plus_b(row: int, col: int, a: int, b: int) -> void:
	var buffer: String = "%02d%% + %d.%d%%/lv" % [a, b / 10, b % 10]
	Console.print_at(row, col, buffer, Ink.BLACK)


func _display_stat_bonus(row: int, col: int, bonus: int) -> void:
	var buffer: String
	if bonus == 0:
		Console.print_at(row, col, "+0", Ink.BLACK)
	elif bonus < 0:
		buffer = "%d" % bonus
		Console.print_at(row, col, buffer, Ink.BRIGHT_RED)
	else:
		buffer = "+%d" % bonus
		Console.print_at(row, col, buffer, Ink.GREEN)


func _display_realm_info(realm: int) -> void:
	match realm:
		Realm.CHAOS:
			Console.print_at(30, 20, "The Chaos realm is the most destructive realm. It focuses", Ink.PURPLE)
			Console.print_at(31, 20, "on combat spells. It is a very good choice for anyone who", Ink.PURPLE)
			Console.print_at(32, 20, "wants to be able to damage their foes directly, but is", Ink.PURPLE)
			Console.print_at(33, 20, "somewhat lacking in non-combat spells.", Ink.PURPLE)
		Realm.CORPOREAL:
			Console.print_at(30, 20, "The Corporeal realm contains spells that exclusively affect", Ink.PURPLE)
			Console.print_at(31, 20, "the caster's body, although some spells also indirectly", Ink.PURPLE)
			Console.print_at(32, 20, "affect other creatures or objects. The corporeal realm is", Ink.PURPLE)
			Console.print_at(33, 20, "particularly good at sensing spells.", Ink.PURPLE)
		Realm.DEATH:
			Console.print_at(30, 20, "The Death realm has a combination of life-draining spells,", Ink.PURPLE)
			Console.print_at(31, 20, "curses, and undead summoning. Like chaos, it is a very", Ink.PURPLE)
			Console.print_at(32, 20, "offensive realm.", Ink.PURPLE)
		Realm.FOLK:
			Console.print_at(30, 20, "The Folk realm is the least specialised of all the realms.", Ink.PURPLE)
			Console.print_at(31, 20, "Folk magic is capable of doing any effect that is possible", Ink.PURPLE)
			Console.print_at(32, 20, "in other realms - but usually less effectively than the", Ink.PURPLE)
			Console.print_at(33, 20, "specialist realms.", Ink.PURPLE)
		Realm.LIFE:
			Console.print_at(30, 20, "The Life realm is devoted to healing and buffing, with some", Ink.PURPLE)
			Console.print_at(31, 20, "offensive capability against undead and demons. It is the", Ink.PURPLE)
			Console.print_at(32, 20, "most defensive of the realms.", Ink.PURPLE)
		Realm.NATURE:
			Console.print_at(30, 20, "The Nature realm has a large number of summoning spells and", Ink.PURPLE)
			Console.print_at(31, 20, "miscellaneous spells, but little in the way of offensive", Ink.PURPLE)
			Console.print_at(32, 20, "and defensive capabilities.", Ink.PURPLE)
		Realm.SORCERY:
			Console.print_at(30, 20, "The Sorcery realm contains spells dealing with raw magic", Ink.PURPLE)
			Console.print_at(31, 20, "itself, for example spells dealing with magical items.", Ink.PURPLE)
			Console.print_at(32, 20, "It is the premier source of miscellaneous non-combat", Ink.PURPLE)
			Console.print_at(33, 20, "utility spells.", Ink.PURPLE)
		Realm.TAROT:
			Console.print_at(30, 20, "The Tarot realm is one of the most specialised realms of", Ink.PURPLE)
			Console.print_at(31, 20, "all, almost exclusively containing summoning and transport", Ink.PURPLE)
			Console.print_at(32, 20, "spells.", Ink.PURPLE)


func _get_stats() -> void:
	while true:
		var dice: Array[int] = [17, 16, 14, 12, 11, 10]
		for i: int in 6:
			var swap_index: int = Rng.roll_die(dice.size()) - 1
			var j: int = dice[swap_index]
			dice.remove_at(swap_index)
			var score: AbilityScore = _player.ability_scores[i]
			score.innate_max = j
			var bonus: int = _player.ancestry.ability_bonus[i] + _player.profession.ability_bonus[i]
			score.innate = score.innate_max
			score.adjusted = score.modify_stat_value(score.innate_max, bonus)
		if _player.ability_scores[_player.profession.prime_ability_score].innate_max > 13:
			return


func _get_extra() -> void:
	_player.max_level_gained = 1
	_player.level = 1
	_player.experience_multiplier = _player.ancestry.experience_factor + _player.profession.experience_factor
	_player.hit_die = _player.ancestry.hit_die_bonus + _player.profession.hit_die_bonus
	# First give the player hit points cycling down through the hit die to make them even
	_player.player_hp = []
	_player.player_hp.append(_player.hit_die)
	var last_roll: int = _player.hit_die
	for i: int in range(1, Constants.MAX_EXPERIENCE_LEVEL):
		_player.player_hp.append(last_roll)
		last_roll -= 1
		if last_roll < 1:
			last_roll = _player.hit_die
	# Then shuffle the hit points at each level (above first)
	for i: int in range(1, Constants.MAX_EXPERIENCE_LEVEL):
		var swap_index: int = Rng.roll_die(Constants.MAX_EXPERIENCE_LEVEL - 1)
		last_roll = _player.player_hp[i]
		_player.player_hp[i] = _player.player_hp[swap_index]
		_player.player_hp[swap_index] = last_roll
	# Make the hit points cumulative
	for i: int in range(1, Constants.MAX_EXPERIENCE_LEVEL):
		_player.player_hp[i] = _player.player_hp[i - 1] + _player.player_hp[i]


func _get_ahw() -> void:
	_player.age = _player.ancestry.base_age + Rng.roll_die(_player.ancestry.age_range)
	_player.game_time = GameTime.new()
	_player.game_time.set_initial_time(Rng.roll_die(365), _player.ancestry.is_nocturnal)
	if _player.gender_index == GenderType.FEMALE:
		_player.height = Rng.random_normal(_player.ancestry.female_base_height, _player.ancestry.female_height_range)
		_player.weight = Rng.random_normal(_player.ancestry.female_base_weight, _player.ancestry.female_weight_range)
	elif _player.gender_index == GenderType.MALE:
		_player.height = Rng.random_normal(_player.ancestry.male_base_height, _player.ancestry.male_height_range)
		_player.weight = Rng.random_normal(_player.ancestry.male_base_weight, _player.ancestry.male_weight_range)
	else:
		if Rng.roll_die(2) == 1:
			_player.height = Rng.random_normal(_player.ancestry.female_base_height, _player.ancestry.female_height_range)
			_player.weight = Rng.random_normal(_player.ancestry.female_base_weight, _player.ancestry.female_weight_range)
		else:
			_player.height = Rng.random_normal(_player.ancestry.male_base_height, _player.ancestry.male_height_range)
			_player.weight = Rng.random_normal(_player.ancestry.male_base_weight, _player.ancestry.male_weight_range)


func _get_money() -> void:
	var gold: int = (_player.social_class * 6) + Rng.roll_die(100) + 300
	for i: int in 6:
		if _player.ability_scores[i].adjusted >= 18 + 50:
			gold -= 300
		elif _player.ability_scores[i].adjusted >= 18 + 20:
			gold -= 200
		elif _player.ability_scores[i].adjusted >= 18:
			gold -= 150
		else:
			gold -= ((_player.ability_scores[i].adjusted - 8) * 10)
	if gold < 100:
		gold = 100
	_player.gold = gold


func _player_outfit() -> void:
	var item: Item = Item.new()
	if _player.ancestry.doesnt_eat:
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(ItemCategory.SCROLL, ScrollType.SATISFY_HUNGER))
		item.count = Rng.roll_die(4) + 1
		item.become_flavour_aware()
		item.become_known()
		item.identify_flags.set_bits(IdentifyFlags.STOREBOUGHT)
		_player.inventory.inven_carry(item)
		item = Item.new()
	else:
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(ItemCategory.FOOD, FoodType.RATION))
		item.count = Rng.roll_die(5) + 2
		item.become_flavour_aware()
		item.become_known()
		_player.inventory.inven_carry(item)
		item = Item.new()
	if _player.ancestry.glows or _player.profession.glows:
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(ItemCategory.SCROLL, ScrollType.LIGHT))
		item.count = Rng.roll_die(5) + 2
		item.become_flavour_aware()
		item.become_known()
		item.identify_flags.set_bits(IdentifyFlags.STOREBOUGHT)
		_player.inventory.inven_carry(item)
	else:
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(ItemCategory.LIGHT, LightType.TORCH))
		item.count = Rng.roll_die(5) + 2
		item.type_specific_value = (Rng.roll_die(5) + 2) * 500
		item.become_flavour_aware()
		item.become_known()
		_player.inventory.inven_carry(item)
		var carried: Item = item.get_copy()
		carried.count = 1
		_player.inventory.items[InventorySlot.LIGHT_SOURCE] = carried
		_player.weight_carried += carried.weight
	var identifiers: Array[ItemIdentifier] = _player.profession.get_starting_items()
	for identifier: ItemIdentifier in identifiers:
		var category: int = identifier.item_category
		var sub_category: int = identifier.item_type
		if category == ItemCategory.SORCERY_BOOK:
			category = Util.realm_to_spellbook(_player.realm_1)
		elif category == ItemCategory.DEATH_BOOK:
			category = Util.realm_to_spellbook(_player.realm_2)
		if category == ItemCategory.RING and sub_category == RingType.RES_FEAR and _player.ancestry.resists_fear:
			sub_category = RingType.SUSTAIN_STR
		item = Item.new()
		item.assign_item_type(SaveGame.instance.item_types.look_up_type(category, sub_category))
		if category == ItemCategory.WAND:
			item.type_specific_value = 1
		item.identify_flags.set_bits(IdentifyFlags.STOREBOUGHT)
		item.become_flavour_aware()
		item.become_known()
		var slot: int = _player.inventory.wield_slot(item)
		if slot == -1:
			_player.inventory.inven_carry(item)
		else:
			_player.inventory.items[slot] = item
			_player.weight_carried += item.weight
