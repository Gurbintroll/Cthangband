# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Settings
extends Resource
## Console settings for the game

## The possible screen resolutions, all in 16x9 aspect ratio
enum Resolution {
	R_640_360,
	R_800_450,
	R_960_540,
	R_1120_630,
	R_1280_720,
	R_1440_810,
	R_1600_900,
	R_1760_990,
}
## The weight of font to use for text display
enum FontWeight {
	LIGHT,
	MEDIUM,
	HEAVY,
}

## A singleton instance
static var instance: Settings

## Whether or not thr game is in fullscreen mode
@export var fullscreen: bool = true
## The size of the window when not in fullscreen mode
@export var window_size: Resolution = Resolution.R_1600_900
## The weight of the display font
@export var font_weight: FontWeight = FontWeight.MEDIUM
## The size of the display font
@export var font_size: int = 24
## Sound volume as a percentage
@export var sound_volume: int = 50
## Music volume as a percentage
@export var music_volume: int = 50


## Load a new settings file, or create one if there isn't a saved file to load
static func load_or_create() -> Settings:
	var loaded: Settings
	if FileAccess.file_exists(Constants.SETTINGS_FILE_PATH):
		loaded = ResourceLoader.load(Constants.SETTINGS_FILE_PATH)
	if loaded == null:
		# We couldn't load one, so create a new one
		instance = Settings.new()
	else:
		# Set our instance to the one we just loaded
		instance = loaded
	return instance


## Save the settings file to its default location
static func save() -> void:
	ResourceSaver.save(instance, Constants.SETTINGS_FILE_PATH)


## The size of the window, converted from an enum to an actual vector
func get_window_size() -> Vector2i:
	match window_size:
		Resolution.R_640_360:
			return Vector2i(640, 360)
		Resolution.R_800_450:
			return Vector2i(800, 450)
		Resolution.R_960_540:
			return Vector2i(960, 540)
		Resolution.R_1120_630:
			return Vector2i(1120, 630)
		Resolution.R_1280_720:
			return Vector2i(1280, 720)
		Resolution.R_1440_810:
			return Vector2i(1440, 810)
		Resolution.R_1600_900:
			return Vector2i(1600, 900)
		Resolution.R_1760_990:
			return Vector2i(1760, 990)
		_:
			push_error("Invalid window size specified.")
	return Vector2i(1600, 900)


## The size of the font, expressed as a string in the range -10 to +10
func font_size_as_string() -> String:
	return ("%+d" % (font_size - 24))


## The weight of the font, expressed as a string
func font_weight_as_string() -> String:
	match font_weight:
		FontWeight.LIGHT:
			return ("Light")
		FontWeight.MEDIUM:
			return ("Medium")
		FontWeight.HEAVY:
			return ("Heavy")
		_:
			push_error("Invalid font weight.")
	return ("Unknown")


## Whether or not we are fullscreen, expressed as a string
func fullscreen_as_string() -> String:
	if fullscreen:
		return "Fullscreen"
	else:
		return "Windowed"


## Returns the current font as a font object
func get_font() -> Font:
	match font_weight:
		FontWeight.LIGHT:
			return load("res://fonts/AtkinsonHyperlegibleMono-Light.otf")
		FontWeight.MEDIUM:
			return load("res://fonts/AtkinsonHyperlegibleMono-Medium.otf")
		FontWeight.HEAVY:
			return load("res://fonts/AtkinsonHyperlegibleMono-Bold.otf")
		_:
			push_error("Invalid font weight.")
			return load("res://fonts/AtkinsonHyperlegibleMono-Bold.otf")


## Returns the current window size as a user-friendly string (e.g. "1600 x 900")
func window_size_as_string() -> String:
	match window_size:
		Resolution.R_640_360:
			return "640 x 360"
		Resolution.R_800_450:
			return "800 x 450"
		Resolution.R_960_540:
			return "960 x 540"
		Resolution.R_1120_630:
			return "1120 x 630"
		Resolution.R_1280_720:
			return "1280 x 720"
		Resolution.R_1440_810:
			return "1440 x 810"
		Resolution.R_1600_900:
			return "1600 x 900"
		Resolution.R_1760_990:
			return "1760 x 990"
		_:
			push_error("Invalid window size specified.")
			return "1600 x 900"


func music_volume_as_string() -> String:
	return "%d%%" % [music_volume]


func sound_volume_as_string() -> String:
	return "%d%%" % [sound_volume]
