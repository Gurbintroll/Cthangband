# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Cthangpedia
extends Window

var _file_history: Array[String] = []
var _location_in_history: int = 0
var _page_list: Array[String] = [
	"Ability Scores",
	"Channeller",
	"Chaos Spells",
	"Charisma",
	"Chosen One",
	"Combat",
	"Commands",
	"Constitution",
	"Corporeal Spells",
	"Cultist",
	"Death Spells",
	"Dexterity",
	"Druid",
	"Elements",
	"Fanatic",
	"Favour",
	"Feelings",
	"Folk Spells",
	"Great Ones",
	"High-Mage",
	"History",
	"Intelligence",
	"Introduction",
	"Items",
	"License",
	"Life Spells",
	"Mage",
	"Magic Basics",
	"Martial Arts",
	"Mentalism Talents",
	"Mindcrafter",
	"Monk",
	"Mutations",
	"Mystic",
	"Nature Spells",
	"Paladin",
	"Patrons",
	"Pets",
	"Priest",
	"Ranger",
	"Rogue",
	"Score",
	"Sorcery Spells",
	"Strength",
	"Summoner",
	"Tarot Spells",
	"Towns and Shops",
	"Turns, Time, and Speed",
	"Warrior",
	"Warrior-Mage",
	"What's New",
	"Wisdom",
]

@onready var _text_area: RichTextLabel = $Panel/HBoxContainer/MarginContainer2/MarginContainer/TextArea
@onready var _file_list: ItemList = $Panel/HBoxContainer/MarginContainer/VBoxContainer/MarginContainer2/FileList
@onready var _back_button: AnimatedButton = $Panel/HBoxContainer/MarginContainer/VBoxContainer/MarginContainer/GridContainer/MarginContainer2/BackButton
@onready var _next_button: AnimatedButton = $Panel/HBoxContainer/MarginContainer/VBoxContainer/MarginContainer/GridContainer/MarginContainer3/NextButton


func _ready() -> void:
	for page: String in _page_list:
		_file_list.add_item(page)
	display_file("Introduction")


func display_file(file_name: String) -> void:
	var file_path: String = "res://manual/%s.txt" % file_name
	_text_area.text = FileAccess.get_file_as_string(file_path)
	for i: int in _page_list.size():
		if _page_list[i] == file_name:
			_file_list.select(i)
			_file_list.center_on_current()
			break
	_text_area.scroll_to_line(0)
	if _file_history.size() == 0:
		_file_history.append(file_name)
		_location_in_history = 0
	elif _file_history[_location_in_history] != file_name:
		_file_history.resize(_location_in_history + 2)
		_file_history[_location_in_history + 1] = file_name
		_location_in_history += 1
	if _location_in_history == _file_history.size() - 1:
		_next_button.disabled = true
	else:
		_next_button.disabled = false
	if _location_in_history == 0:
		_back_button.disabled = true
	else:
		_back_button.disabled = false


func _on_close_requested() -> void:
	visible = false


func _on_file_list_item_activated(index: int) -> void:
	display_file(_page_list[index])


func _on_text_area_meta_clicked(meta: Variant) -> void:
	var file_name: String = meta as String
	display_file(file_name)


func _on_file_list_item_selected(index: int) -> void:
	display_file(_page_list[index])


func _on_home_button_pressed() -> void:
	display_file("Introduction")



func _on_back_button_pressed() -> void:
	if _location_in_history > 0:
		_location_in_history -= 1
		display_file(_file_history[_location_in_history])

func _on_next_button_pressed() -> void:
	if _file_history.size() > _location_in_history:
		_location_in_history += 1
		display_file(_file_history[_location_in_history])
