# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name LevelBuildInfo
extends RefCounted

var cent: Array[Location]
var door: Array[Location]
var room_map: Array
var tunn: Array[Location]
var wall: Array[Location]
var cent_num: int
var col_rooms: int
var crowded: bool
var door_num: int
var row_rooms: int
var tunn_num: int
var wall_num: int


func _init() -> void:
	for i: int in LevelFactory.CENT_MAX:
		cent.append(Location.new())
	for i: int in LevelFactory.DOOR_MAX:
		door.append(Location.new())
	for i: int in LevelFactory.TUNN_MAX:
		tunn.append(Location.new())
	for i: int in LevelFactory.WALL_MAX:
		wall.append(Location.new())
	for row: int in LevelFactory.MAX_ROOMS_ROW:
		room_map.append([])
		for col: int in LevelFactory.MAX_ROOMS_COL:
			room_map[row].append(false)
