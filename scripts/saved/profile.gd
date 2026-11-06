# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Profile
extends Resource
## A saved profile that includes a save game and information passed between games

## The singleton instance of the currently loaded profile, will be null between games
static var instance: Profile

## The previous player character, if there has been one
@export var ex_player: ExPlayer
## The save game that this profile contains
@export var savegame: SaveGame
## The index of the save slot that this profile was loaded from
@export var save_slot_index: int
## The list of monster types
@export var monster_types: MonsterTypeArray

@export var message_should_continue: bool
@export var _messages: Array[String]
@export var _message_counts: Array[int]
@export var _message_start_col: int


## Save the profile to its file
static func save() -> void:
	ResourceSaver.save(instance, Constants.SAVE_FILE_PATH)


## Load the profile from the specified save slot, or create a new one if that slot is empty
static func load_or_create() -> void:
	# First attempt to load the profile
	var loaded: Profile
	if FileAccess.file_exists(Constants.SAVE_FILE_PATH):
		loaded = ResourceLoader.load(Constants.SAVE_FILE_PATH)
	# If the load failed, create a new one instead
	if loaded == null:
		instance = Profile.new()
		instance._initialise_new_profile()
	else:
		instance = loaded


static func delete_file() -> void:
	# File name is based on our slot index
	var _file_name: String = Constants.SAVE_FILE_FILENAME
	var dir: DirAccess = DirAccess.open(Constants.SAVE_FILE_LOCATION)
	dir.remove(_file_name)


## Unload the currently loaded profile instance because we're returning to the main menu
static func unload() -> void:
	instance = null


func run() -> void:
	message_should_continue = false
	message_print()
	message_should_continue = false
	if savegame == null:
		savegame = SaveGame.new()
		savegame.extract_resources()
	SaveGame.instance = savegame
	savegame.play()
	SaveGame.instance = null


func get_message_count() -> int:
	return _messages.size()


func get_message(index: int) -> String:
	if index >= _messages.size():
		return ""
	var message: String = _messages[-(index + 1)]
	var count: int = _message_counts[-(index + 1)]
	if count > 1:
		message += " (x%d)" % [count]
	return message


func message_print(text: String = "") -> void:
	if not message_should_continue:
		_message_start_col = 0
	var l: int = text.length()
	# If we have a partial message waiting and this message wants to
	# clear the buffer or will be too long, wait for a keypress
	if _message_start_col != 0 and (text == "" or _message_start_col + l > 72):
		_more(_message_start_col)
		message_should_continue = false
		_message_start_col = 0
	# If we have no message after that, there's nothing else to do
	if text == "":
		return
	# Refuse to print a message that's more than 1,000 characters
	if l > 1000:
		return
	# Make sure the messagge starts with a capital
	if l > 2:
		text = text.left(1).to_upper() + text.substr(1)
	# Add the message to our log
	if savegame.player != null:
		add_message_to_log(text)
	# If we have more than one line to print, print a line at a time
	while l > 72:
		var split: int = 72
		# Find the last space as a suitable place to break
		for check: int in range(40, 72):
			if text[check] == " ":
				split = check
		Console.print_at(0, 0, text.left(split), Ink.WHITE)
		_more(split + 1)
		text = text.substr(split)
		l -= split
	# Print the remainder of the message, and assume the next will continue on the same line
	Console.print_at(0, _message_start_col, text, Ink.WHITE)
	message_should_continue = true
	_message_start_col += l + 1


func add_message_to_log(text: String) -> void:
	# If this is our first message, then just add it
	if _messages.size() == 0:
		_messages.append(text)
		_message_counts.append(1)
		return
	# If the message was the same as the last message, simply increase the count
	if text != "":
		if _messages[-1] == text:
			_message_counts[-1] += 1
			return
	# We had no special cases, so just add the new message
	_messages.append(text)
	_message_counts.append(1)
	# Don't let the log get too big
	if _messages.size() > 2048:
		_messages.remove_at(0)
		_message_counts.remove_at(0)


## Initialise a new profile with the information it needs
func _initialise_new_profile() -> void:
	# Extract the resource data from the csv files
	_extract_resources()


# Extract the resources from our embedded csv files
func _extract_resources() -> void:
	monster_types = MonsterTypeArray.new()
	for monster_type: MonsterType in StaticData.monster_types:
		monster_types.types.append(monster_type.get_copy_for_new_profile())
	monster_types.sort_by_level()


func _more(col: int) -> void:
	Console.print_at(0, col, "-more-", Ink.BRIGHT_BLUE)
	User.get_key()
	Console.clear_row(0)
