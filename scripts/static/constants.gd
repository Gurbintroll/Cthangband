# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Constants
extends Node
## Constants and static constant-derived variables for global use

const MAX_INT: int = 9223372036854775807
## Version
const VERSION_MAJOR: int = 7
const VERSION_MINOR: int = 0
const VERSION_YEAR: String = "2026"
## Screen size
const SCREEN_WIDTH: int = 66
const SCREEN_HEIGHT: int = 42
const MAX_EXPERIENCE: int = 99999999
const MAX_GOLD: int = 999999999
const MAX_EXPERIENCE_LEVEL: int = 50
const ITEM_TO_HIT_MULTIPLIER: int = 3
const HIT_POINT_WARNING: int = 2
const FOOD_STARVE: int = 100
const FOOD_FAINT: int = 500
const FOOD_WEAK: int = 1000
const FOOD_ALERT: int = 2000
const FOOD_FULL: int = 10000
const FOOD_GORGED: int = 15000
const PLAYER_REGEN_NORMAL: int = 197
const PLAYER_REGEN_WEAK: int = 98
const PLAYER_REGEN_FAINT: int = 33
const MAX_SCROLLS: int = 54
const FUEL_LANTERN: int = 15000
const FUEL_TORCH: int = 5000

const SFX_VOLUME_SCALE: float = 0.06
const MUSIC_VOLUME_SCALE: float = 0.02

static var VERSION_STAMP: String = "v%s.%s" % [VERSION_MAJOR, VERSION_MINOR]
static var SAVE_FILE_LOCATION: String = "user://"
static var SAVE_FILE_FILENAME: String = "savefile_%d_%d.res" % [VERSION_MAJOR, VERSION_MINOR]
static var SAVE_FILE_PATH: String = "user://savefile_%d_%d.res" % [VERSION_MAJOR, VERSION_MINOR]
static var SETTINGS_FILE_PATH: String = "user://settings_%d_%d.res" % [VERSION_MAJOR, VERSION_MINOR]
static var HISCORE_FILE_PATH: String = "user://scores_%d_%d.res" % [VERSION_MAJOR, VERSION_MINOR]
