# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Island
extends Resource

const ELEVATION: float = 0.05
const STEEPNESS: float = 6.0
const DROPOFF: float = 50.0
## Wilderness map size
const WILDERNESS_WIDTH: int = 66
const WILDERNESS_HEIGHT: int = 42

@export var inner_regions: Array[WildernessRegion]
@export var map_width: int
@export var map_height: int
@export var width_divisor: float
@export var height_divisor: float
@export var noise: PerlinNoise


static func new_island() -> Island:
	var island: Island = Island.new()
	island.initialize()
	return island


func initialize() -> void:
	noise = PerlinNoise.new()
	# Set up our regions
	for row: int in 144:
		inner_regions.append(WildernessRegion.new())
	# set up our elevation data
	var reject: bool = true
	while reject:
		reject = false
		# Make an island large enough for a 10 x 10 island
		map_width = (10 * (WILDERNESS_WIDTH - 1)) + 3
		map_height = (10 * (WILDERNESS_HEIGHT - 1)) + 3
		width_divisor = 1.0 / map_width
		height_divisor = 1.0 / map_height
		noise.randomise(Rng.big_number())
		# Check the map for validity
		for row: int in map_height:
			if _elevation_map(row, 1) != 0:
				reject = true
				break
			if _elevation_map(row, map_width - 2) != 0:
				reject = true
				break
		for col: int in map_width:
			if _elevation_map(1, col) != 0:
				reject = true
				break
			if _elevation_map(map_height - 2, col) != 0:
				reject = true
				break


func get_region(row: int, col: int) -> WildernessRegion:
	return inner_regions[row * 12 + col]


func get_elevation(wild_row: int, wild_col: int, row: int, col: int) -> int:
	return _elevation_map(((wild_row - 1) * (WILDERNESS_HEIGHT - 1)) + row + 1, ((wild_col - 1) * (WILDERNESS_WIDTH - 1)) + col + 1)


func _elevation_map(row: int, col: int) -> int:
	var v: float = noise.noise(10 * col * width_divisor, 10 * row * height_divisor, -0.5)
	v = (v + 1.0) / 2.0
	var dx: float = abs(col - (map_width / 2.0)) * width_divisor
	var dy: float = abs(row - (map_height / 2.0)) * height_divisor
	var d: float = max(dx, dy)
	v += ELEVATION - (DROPOFF * pow(d, STEEPNESS))
	v = clamp(v, 0.0, 1.0)
	return floori(v * 10)
