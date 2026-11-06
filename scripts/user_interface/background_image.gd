# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name BackgroundImage
extends TextureRect
## The background image that sits behind the console

## The possible backgrounds that can be displayed
const CROWN: int = 0 # Displayed when the player wins the game
const DEFAULT: int = 1 # The plain background displayed in front of selection screens
const IN_GAME: int = 2 # Plain background with a black area for the in-game display
const MAP: int = 3 # Parchment background with a large black area for the dungeon map
const MENU: int = 4 # Default background, but with the Cthangband title
const OPTIONS: int = 5 # Parchment background, with a black area for a sample display
const PAPER: int = 6 # Full parchment background
const SUNSET: int = 7 # Displayed when the player retires after winning the game
const TOMB: int = 8 # Displayed when the player dies
const WILD_MAP: int = 9 # Parchment background, with a small black area for the wilderness map

var background: int = -1
var changed: bool
# The various images, all pre-loaded as resources
var _crown: CompressedTexture2D = preload("res://images/Crown.png")
var _default: CompressedTexture2D = preload("res://images/Default.png")
var _in_game: CompressedTexture2D = preload("res://images/InGame.png")
var _map: CompressedTexture2D = preload("res://images/Map.png")
var _menu: CompressedTexture2D = preload("res://images/Menu.png")
var _options: CompressedTexture2D = preload("res://images/Options.png")
var _paper: CompressedTexture2D = preload("res://images/Paper.png")
var _sunset: CompressedTexture2D = preload("res://images/Sunset.png")
var _tomb: CompressedTexture2D = preload("res://images/Tomb.png")
var _wild_map: CompressedTexture2D = preload("res://images/WildMap.png")
## The set of background images keyed by the enumeration entries
var _backgrounds: Dictionary[int, CompressedTexture2D]


## Initialise the background
func _ready() -> void:
	_backgrounds.get_or_add(CROWN, _crown)
	_backgrounds.get_or_add(DEFAULT, _default)
	_backgrounds.get_or_add(IN_GAME, _in_game)
	_backgrounds.get_or_add(MAP, _map)
	_backgrounds.get_or_add(MENU, _menu)
	_backgrounds.get_or_add(OPTIONS, _options)
	_backgrounds.get_or_add(PAPER, _paper)
	_backgrounds.get_or_add(SUNSET, _sunset)
	_backgrounds.get_or_add(TOMB, _tomb)
	_backgrounds.get_or_add(WILD_MAP, _wild_map)
	self.texture = _menu


## Set the background to a specific image
func set_background(new_background: int) -> void:
	if new_background != background:
		background = new_background
		changed = true


func refresh() -> void:
	if changed:
		texture = _backgrounds[background]
		changed = false
