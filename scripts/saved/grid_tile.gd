# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name GridTile
extends Resource

const PLAYER_MEMORISED: int = 0x0001
const SELF_LIT: int = 0x0002
const IN_VAULT: int = 0x0004
const IN_ROOM: int = 0x0008
const PLAYER_LIT: int = 0x0010
const IS_VISIBLE: int = 0x0020
const NEEDS_REDRAW: int = 0x0040
const EASY_VISIBILITY: int = 0x0080
const TRAPS_DETECTED: int = 0x0100

@export var tile_flags: FlagSet
@export var feature_type: FloorTileType
@export var background_feature: FloorTileType
@export var item_index: int
@export var monster_index: int
@export var scent_strength: int
@export var scent_age: int


func _init() -> void:
	tile_flags = FlagSet.new()


func revert_to_background() -> void:
	feature_type = background_feature


func set_background_feature(feature_name: String) -> void:
	background_feature = StaticData.tile_types[feature_name]


func set_feature(feature_name: String) -> void:
	feature_type = StaticData.tile_types[feature_name]
