# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PopupTextMenu
extends RefCounted

var _items: Array[String]
var _padded_items: Array[String]
var _text: Array[String]
var _width: int


func _init(items: Array[String], texts: Array[String] = []) -> void:
	_items = items
	for item: String in items:
		if item.length() > _width:
			_width = item.length()
	for text: String in texts:
		if text.length() > _width:
			_width = text.length()
	_width += 2
	for item: String in items:
		var padded: String = Util.pad_center(item, _width)
		_padded_items.append(padded)
	for text: String in texts:
		var padded: String = Util.pad_center(text, _width)
		_text.append(padded)


func show() -> String:
	Console.full_screen_overlay = true
	Console.save_screen()
	var result: String = display_menu()
	Console.restore_screen()
	Console.full_screen_overlay = false
	return result


func display_menu() -> String:
	var top: int = Console.CONSOLE_HEIGHT / 2 - (_items.size() + _text.size()) / 2
	var left: int = Console.CONSOLE_WIDTH / 2 - _width / 2
	var top_bottom: String = "+" + "".rpad(_width, "-") + "+"
	var left_right: String = "|" + "".rpad(_width, " ") + "|"
	var chosen_item: int = 0
	Console.print_at(top, left, top_bottom, Ink.WHITE)
	for i: int in _text.size() + _items.size():
		Console.print_at(top + i + 1, left, left_right, Ink.WHITE)
	Console.print_at(top + _text.size() + _items.size() + 1, left, top_bottom, Ink.WHITE)
	for i: int in _text.size():
		Console.print_at(top + i + 1, left + 1, _text[i], Ink.WHITE)
	while true:
		for i: int in _items.size():
			if i == chosen_item:
				Console.print_at(top + _text.size() + i + 1, left + 1, _padded_items[i], Ink.BRIGHT_PURPLE)
			else:
				Console.print_at(top + _text.size() + i + 1, left + 1, _padded_items[i], Ink.WHITE)
		var key: String = User.get_key()
		match key:
			"Escape":
				return ""
			" ", "Enter":
				return _items[chosen_item]
			"Down":
				chosen_item += 1
				if chosen_item == _items.size():
					chosen_item = 0
				Console.play_sound("MenuSelect")
			"Up":
				chosen_item -= 1
				if chosen_item == -1:
					chosen_item = _items.size() - 1
				Console.play_sound("MenuSelect")
	return ""
