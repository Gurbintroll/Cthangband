# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name VaultFactory
extends RefCounted

var _level: DungeonLevel
var _template_monster_type: int
var _vault_dragon_mask_4: int


func _init(level: DungeonLevel) -> void:
	_level = level


## A simple rectangular room with the possibility of pillars
func build_type_1(centre_row: int, centre_col: int) -> void:
	var tile: GridTile
	var light: bool = SaveGame.instance.total_difficulty() <= Rng.roll_die(25)
	var row_start: int = centre_row - Rng.roll_die(4)
	var row_end: int = centre_row + Rng.roll_die(3)
	var col_start: int = centre_col - Rng.roll_die(11)
	var col_end: int = centre_col + Rng.roll_die(11)
	for row: int in range(row_start - 1, row_end + 2):
		for col: int in range(col_start - 1, col_end + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallOuter")
	if Rng.roll_die(20) == 1:
		for row: int in range(row_start, row_end + 1, 2):
			for col: int in range(col_start, col_end + 1, 2):
				tile = _level.get_grid(row, col)
				tile.set_feature("Pillar")
	elif Rng.roll_die(50) == 1:
		for row: int in range(row_start + 2, row_end - 1, 2):
			tile = _level.get_grid(row, col_start)
			tile.set_feature("Pillar")
			tile = _level.get_grid(row, col_end)
			tile.set_feature("Pillar")
		for col: int in range(col_start + 2, col_end - 1, 2):
			tile = _level.get_grid(row_start, col)
			tile.set_feature("Pillar")
			tile = _level.get_grid(row_end, col)
			tile.set_feature("Pillar")


## A room made from two overlapping rectangles
func build_type_2(centre_row: int, centre_col: int) -> void:
	var tile: GridTile
	var light: bool = SaveGame.instance.total_difficulty() <= Rng.roll_die(25)
	var row_start_a: int = centre_row - Rng.roll_die(4)
	var row_end_a: int = centre_row + Rng.roll_die(3)
	var col_start_a: int = centre_col - Rng.roll_die(11)
	var col_end_a: int = centre_col + Rng.roll_die(10)
	var row_start_b: int = centre_row - Rng.roll_die(3)
	var row_end_b: int = centre_row + Rng.roll_die(4)
	var col_start_b: int = centre_col - Rng.roll_die(10)
	var col_end_b: int = centre_col + Rng.roll_die(11)
	for row: int in range(row_start_a - 1, row_end_a + 2):
		for col: int in range(col_start_a - 1, col_end_a + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start_b - 1, row_end_b + 2):
		for col: int in range(col_start_b - 1, col_end_b + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start_a - 1, row_end_a + 2):
		tile = _level.get_grid(row, col_start_a - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end_a + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start_a - 1, col_end_a + 2):
		tile = _level.get_grid(row_start_a - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end_a + 1, col)
		tile.set_feature("WallOuter")
	for row: int in range(row_start_b - 1, row_end_b + 2):
		tile = _level.get_grid(row, col_start_b - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end_b + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start_b - 1, col_end_b + 2):
		tile = _level.get_grid(row_start_b - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end_b + 1, col)
		tile.set_feature("WallOuter")
	for row: int in range(row_start_a, row_end_a + 1):
		for col: int in range(col_start_a, col_end_a + 1):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
	for row: int in range(row_start_b, row_end_b + 1):
		for col: int in range(col_start_b, col_end_b + 1):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()


## A cross-shaped room made from two overlapping rectangles with a centrepiece
func build_type_3(centre_row: int, centre_col: int) -> void:
	var row_w: int = 1
	var tile: GridTile
	var light: bool = SaveGame.instance.total_difficulty() <= Rng.roll_die(25)
	var col_w: int = 1
	var d_row: int = Rng.random_between(3, 4)
	var d_col: int = Rng.random_between(3, 11)
	var row_start_a: int = centre_row - d_row
	var row_end_a: int = centre_row + d_row
	var col_start_a: int = centre_col - col_w
	var col_end_a: int = centre_col + col_w
	var row_start_b: int = centre_row - row_w
	var row_end_b: int = centre_row + row_w
	var col_start_b: int = centre_col - d_col
	var col_end_b: int = centre_col + d_col
	for row: int in range(row_start_a - 1, row_end_a + 2):
		for col: int in range(col_start_a - 1, col_end_a + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start_b - 1, row_end_b + 2):
		for col: int in range(col_start_b - 1, col_end_b + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start_a - 1, row_end_a + 2):
		tile = _level.get_grid(row, col_start_a - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end_a + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start_a - 1, col_end_a + 2):
		tile = _level.get_grid(row_start_a - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end_a + 1, col)
		tile.set_feature("WallOuter")
	for row: int in range(row_start_b - 1, row_end_b + 2):
		tile = _level.get_grid(row, col_start_b - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end_b + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start_b - 1, col_end_b + 2):
		tile = _level.get_grid(row_start_b - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end_b + 1, col)
		tile.set_feature("WallOuter")
	for row: int in range(row_start_a, row_end_a + 1):
		for col: int in range(col_start_a, col_end_a + 1):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
	for row: int in range(row_start_b, row_end_b + 1):
		for col: int in range(col_start_b, col_end_b + 1):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
	match Rng.roll_die(4):
		1:
			for row: int in range(row_start_b, row_end_b + 1):
				for col: int in range(col_start_a, col_end_a + 1):
					tile = _level.get_grid(row, col)
					tile.set_feature("WallInner")
		2:
			for row: int in range(row_start_b, row_end_b + 1):
				tile = _level.get_grid(row, col_start_a)
				tile.set_feature("WallInner")
				tile = _level.get_grid(row, col_end_a)
				tile.set_feature("WallInner")
			for col: int in range(col_start_a, col_end_a + 1):
				tile = _level.get_grid(row_start_b, col)
				tile.set_feature("WallInner")
				tile = _level.get_grid(row_end_b, col)
				tile.set_feature("WallInner")
			match Rng.roll_die(4):
				1:
					place_secret_door(row_start_b, centre_col)
				2:
					place_secret_door(row_end_b, centre_col)
				3:
					place_secret_door(centre_row, col_start_a)
				4:
					place_secret_door(centre_row, col_end_a)
			_level.place_item(centre_row, centre_col, false, false)
			vault_monsters(centre_row, centre_col, Rng.roll_die(2) + 2)
			vault_traps(centre_row, centre_col, 4, 4, Rng.roll_die(3) + 2)
		3:
			if Rng.roll_die(3) == 1:
				for row: int in range(row_start_b, row_end_b + 1):
					if row == centre_row:
						continue
					tile = _level.get_grid(row, col_start_a - 1)
					tile.set_feature("WallInner")
					tile = _level.get_grid(row, col_end_a + 1)
					tile.set_feature("WallInner")
				for col: int in range(col_start_a, col_end_a + 1):
					if col == centre_col:
						continue
					tile = _level.get_grid(row_start_b - 1, col)
					tile.set_feature("WallInner")
					tile = _level.get_grid(row_end_b + 1, col)
					tile.set_feature("WallInner")
				if Rng.roll_die(3) == 1:
					place_secret_door(centre_row, col_start_a - 1)
					place_secret_door(centre_row, col_end_a + 1)
					place_secret_door(row_start_b - 1, centre_col)
					place_secret_door(row_end_b + 1, centre_col)
			elif Rng.roll_die(3) == 1:
				tile = _level.get_grid(centre_row, centre_col)
				tile.set_feature("WallInner")
				tile = _level.get_grid(row_start_b, centre_col)
				tile.set_feature("WallInner")
				tile = _level.get_grid(row_end_b, centre_col)
				tile.set_feature("WallInner")
				tile = _level.get_grid(centre_row, col_start_a)
				tile.set_feature("WallInner")
				tile = _level.get_grid(centre_row, col_end_a)
				tile.set_feature("WallInner")
			elif Rng.roll_die(3) == 1:
				tile = _level.get_grid(centre_row, centre_col)
				tile.set_feature("Pillar")


## Room with a corridor ring around it
func build_type_4(centre_row: int, centre_col: int) -> void:
	var tile: GridTile
	var light: bool = SaveGame.instance.total_difficulty() <= Rng.roll_die(25)
	var row_start: int = centre_row - 4
	var row_end: int = centre_row + 4
	var col_start: int = centre_col - 11
	var col_end: int = centre_col + 11
	for row: int in range(row_start - 1, row_end + 2):
		for col: int in range(col_start - 1, col_end + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if light:
				tile.tile_flags.set_bits(GridTile.SELF_LIT)
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallOuter")
	row_start += 2
	row_end -= 2
	col_start += 2
	col_end -= 2
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallInner")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallInner")
	match Rng.roll_die(5):
		1:
			match Rng.roll_die(4):
				1:
					place_secret_door(row_start - 1, centre_col)
				2:
					place_secret_door(row_end + 1, centre_col)
				3:
					place_secret_door(centre_row, col_start - 1)
				4:
					place_secret_door(centre_row, col_end + 1)
			vault_monsters(centre_row, centre_col, 1)
		2:
			match Rng.roll_die(4):
				1:
					place_secret_door(row_start - 1, centre_col)
				2:
					place_secret_door(row_end + 1, centre_col)
				3:
					place_secret_door(centre_row, col_start - 1)
				4:
					place_secret_door(centre_row, col_end + 1)
			for row: int in range(centre_row - 1, centre_row + 2):
				for col: int in range(centre_col - 1, centre_col + 2):
					if col == centre_col and row == centre_row:
						continue
					tile = _level.get_grid(row, col)
					tile.set_feature("WallInner")
			match Rng.roll_die(4):
				1:
					place_locked_door(centre_row - 1, centre_col)
				2:
					place_locked_door(centre_row + 1, centre_col)
				3:
					place_locked_door(centre_row, centre_col - 1)
				4:
					place_locked_door(centre_row, centre_col + 1)
			vault_monsters(centre_row, centre_col, Rng.roll_die(3) + 2)
			if Rng.roll_die(100) <= 80:
				_level.place_item(centre_row, centre_col, false, false)
			else:
				place_random_stairs(centre_row, centre_col)
			vault_traps(centre_row, centre_col, 4, 10, 2 + Rng.roll_die(3))
		3:
			match Rng.roll_die(4):
				1:
					place_secret_door(row_start - 1, centre_col)
				2:
					place_secret_door(row_end + 1, centre_col)
				3:
					place_secret_door(centre_row, col_start - 1)
				4:
					place_secret_door(centre_row, col_end + 1)
			for row: int in range(centre_row - 1, centre_row + 2):
				for col: int in range(centre_col - 1, centre_col + 2):
					tile = _level.get_grid(row, col)
					tile.set_feature("WallInner")
			if Rng.roll_die(2) == 1:
				var tmp: int = Rng.roll_die(2)
				for row: int in range(centre_row - 1, centre_row + 2):
					for col: int in range(centre_col - 5 - tmp, centre_col - 2 - tmp):
						tile = _level.get_grid(row, col)
						tile.set_feature("WallInner")
					for col: int in range(centre_col + 3 + tmp, centre_col + 6 + tmp):
						tile = _level.get_grid(row, col)
						tile.set_feature("WallInner")
			if Rng.roll_die(3) == 1:
				for col: int in range(centre_col - 5, centre_col + 6):
					tile = _level.get_grid(centre_row - 1, col)
					tile.set_feature("WallInner")
					tile = _level.get_grid(centre_row + 1, col)
					tile.set_feature("WallInner")
				tile = _level.get_grid(centre_row, centre_col - 5)
				tile.set_feature("WallInner")
				tile = _level.get_grid(centre_row, centre_col + 5)
				tile.set_feature("WallInner")
				place_secret_door(centre_row - 3 + (Rng.roll_die(2) * 2), centre_col - 3)
				place_secret_door(centre_row - 3 + (Rng.roll_die(2) * 2), centre_col + 3)
				vault_monsters(centre_row, centre_col - 2, Rng.roll_die(2))
				vault_monsters(centre_row, centre_col + 2, Rng.roll_die(2))
				if Rng.roll_die(3) == 1:
					_level.place_item(centre_row, centre_col - 2, false, false)
				if Rng.roll_die(3) == 1:
					_level.place_item(centre_row, centre_col + 2, false, false)
		4:
			match Rng.roll_die(4):
				1:
					place_secret_door(row_start - 1, centre_col)
				2:
					place_secret_door(row_end + 1, centre_col)
				3:
					place_secret_door(centre_row, col_start - 1)
				4:
					place_secret_door(centre_row, col_end + 1)
			for row: int in range(row_start, row_end + 1):
				for col: int in range(col_start, col_end + 1):
					if (1 & (col + row)) != 0:
						tile = _level.get_grid(row, col)
						tile.set_feature("WallInner")
			vault_monsters(centre_row, centre_col - 5, Rng.roll_die(3))
			vault_monsters(centre_row, centre_col + 5, Rng.roll_die(3))
			vault_traps(centre_row, centre_col - 3, 2, 8, Rng.roll_die(3))
			vault_traps(centre_row, centre_col + 3, 2, 8, Rng.roll_die(3))
			vault_objects(centre_row, centre_col, 3)
		5:
			for row: int in range(row_start, row_end + 1):
				tile = _level.get_grid(row, centre_col)
				tile.set_feature("WallInner")
			for col: int in range(col_start, col_end + 1):
				tile = _level.get_grid(centre_row, col)
				tile.set_feature("WallInner")
			if Rng.roll_die(100) <= 50:
				var i: int = Rng.roll_die(10)
				place_secret_door(row_start - 1, centre_col - i)
				place_secret_door(row_start - 1, centre_col + i)
				place_secret_door(row_end + 1, centre_col - i)
				place_secret_door(row_end + 1, centre_col + i)
			else:
				var i: int = Rng.roll_die(3)
				place_secret_door(centre_row + i, col_start - 1)
				place_secret_door(centre_row - i, col_start - 1)
				place_secret_door(centre_row + i, col_end + 1)
				place_secret_door(centre_row - i, col_end + 1)
			vault_objects(centre_row, centre_col, 2 + Rng.roll_die(2))
			vault_monsters(centre_row + 1, centre_col - 4, Rng.roll_die(4))
			vault_monsters(centre_row + 1, centre_col + 4, Rng.roll_die(4))
			vault_monsters(centre_row - 1, centre_col - 4, Rng.roll_die(4))
			vault_monsters(centre_row - 1, centre_col + 4, Rng.roll_die(4))


## Lesser monster pit
func build_type_5(centre_row: int, centre_col: int) -> void:
	var what: Array[int] = [
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
	]
	var tile: GridTile
	var empty: bool = false
	var row_start: int = centre_row - 4
	var row_end: int = centre_row + 4
	var col_start: int = centre_col - 11
	var col_end: int = centre_col + 11
	for row: int in range(row_start - 1, row_end + 2):
		for col: int in range(col_start - 1, col_end + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallOuter")
	row_start += 2
	row_end -= 2
	col_start += 2
	col_end -= 2
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallInner")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallInner")
	match Rng.roll_die(4):
		1:
			place_secret_door(row_start - 1, centre_col)
		2:
			place_secret_door(row_end + 1, centre_col)
		3:
			place_secret_door(centre_row, col_start - 1)
		4:
			place_secret_door(centre_row, col_end + 1)
	var tmp: int = Rng.roll_die(SaveGame.instance.total_difficulty())
	if tmp < 25 and Rng.roll_die(2) != 1:
		_template_monster_type = Rng.roll_die(Profile.instance.monster_types.types.size() - 2)
		while (Profile.instance.monster_types.types[_template_monster_type].flags_1.is_set(MonsterFlags1.UNIQUE) or
			Profile.instance.monster_types.types[_template_monster_type].level + Rng.roll_die(5) > SaveGame.instance.total_difficulty() + Rng.roll_die(5) ):
			_template_monster_type = Rng.roll_die(Profile.instance.monster_types.types.size() - 2)
		if Rng.roll_die(2) != 1 and SaveGame.instance.total_difficulty() >= 25 + Rng.roll_die(15):
			_level.dungeon_monsters.monster_validation_function = validation_function_symbol
		else:
			_level.dungeon_monsters.monster_validation_function = validation_function_clone
	elif tmp < 25:
		_level.dungeon_monsters.monster_validation_function = validation_function_jelly
	elif tmp < 50:
		_level.dungeon_monsters.monster_validation_function = validation_function_treasure
	elif tmp < 65:
		if Rng.roll_die(3) == 1:
			_level.dungeon_monsters.monster_validation_function = validation_function_kennel
		else:
			_level.dungeon_monsters.monster_validation_function = validation_function_animal
	else:
		if Rng.roll_die(3) == 1:
			_level.dungeon_monsters.monster_validation_function = validation_function_chapel
		else:
			_level.dungeon_monsters.monster_validation_function = validation_function_undead
	_level.dungeon_monsters.prepare_random_monster_table()
	for i: int in 64:
		what[i] = _level.dungeon_monsters.get_random_monster(SaveGame.instance.total_difficulty() + 10)
		if what[i] == 0:
			empty = true
	_level.dungeon_monsters.monster_validation_function = Callable()
	_level.dungeon_monsters.prepare_random_monster_table()
	if empty:
		return
	_level.danger_rating += 10
	if SaveGame.instance.total_difficulty() <= 40 and Rng.roll_die((SaveGame.instance.total_difficulty() * SaveGame.instance.total_difficulty()) + 50) < 300:
		_level.special_danger = true
	for row: int in range(centre_row - 2, centre_row + 3):
		for col: int in range(centre_col - 9, centre_col + 10):
			var index: int = what[Rng.roll_die(64) - 1]
			var monster_type: MonsterType = Profile.instance.monster_types.types[index]
			_level.Monsters.place_monster_by_type(row, col, monster_type, false, false, false)


## Greater monster pit
func build_type_6(centre_row: int, centre_col: int) -> void:
	var what: Array[int] = [
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
	]
	var empty: bool = false
	var tile: GridTile
	var row_start: int = centre_row - 4
	var row_end: int = centre_row + 4
	var col_start: int = centre_col - 11
	var col_end: int = centre_col + 11
	for row: int in range(row_start - 1, row_end + 2):
		for col: int in range(col_start - 1, col_end + 2):
			tile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM)
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallOuter")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallOuter")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallOuter")
	row_start += 2
	row_end -= 2
	col_start += 2
	col_end -= 2
	for row: int in range(row_start - 1, row_end + 2):
		tile = _level.get_grid(row, col_start - 1)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row, col_end + 1)
		tile.set_feature("WallInner")
	for col: int in range(col_start - 1, col_end + 2):
		tile = _level.get_grid(row_start - 1, col)
		tile.set_feature("WallInner")
		tile = _level.get_grid(row_end + 1, col)
		tile.set_feature("WallInner")
	match Rng.roll_die(4):
		1:
			place_secret_door(row_start - 1, centre_col)
		2:
			place_secret_door(row_end + 1, centre_col)
		3:
			place_secret_door(centre_row, col_start - 1)
		4:
			place_secret_door(centre_row, col_end + 1)
	var tmp: int = Rng.roll_die(SaveGame.instance.total_difficulty())
	if tmp < 20:
		_level.dungeon_monsters.monster_validation_function = validation_function_orc
	elif tmp < 40:
		_level.dungeon_monsters.monster_validation_function = validation_function_troll
	elif tmp < 55:
		_level.dungeon_monsters.monster_validation_function = validation_function_giant
	elif tmp < 70:
		if Rng.roll_die(4) != 1:
			_template_monster_type = Rng.roll_die(Profile.instance.monster_types.types.size() - 2)
			while (Profile.instance.monster_types.types[_template_monster_type].flags_1.is_set(MonsterFlags1.UNIQUE) or
				Profile.instance.monster_types.types[_template_monster_type].Level + Rng.roll_die(5) > SaveGame.instance.total_difficulty() + Rng.roll_die(5) ):
				_template_monster_type = Rng.roll_die(Profile.instance.monster_types.types.size() - 2)
			_level.dungeon_monsters.monster_validation_function = validation_function_symbol
		else:
			if Rng.roll_die(2) == 1:
				_level.dungeon_monsters.monster_validation_function = validation_function_cult
			else:
				_level.dungeon_monsters.monster_validation_function = validation_function_chapel
	elif tmp < 80:
		match Rng.roll_die(6):
			1:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_ACID
			2:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_LIGHTNING
			3:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_FIRE
			4:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_COLD
			5:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_POISON
			_:
				_vault_dragon_mask_4 = MonsterFlags4.BREATHE_ACID | MonsterFlags4.BREATHE_LIGHTNING | MonsterFlags4.BREATHE_FIRE | MonsterFlags4.BREATHE_COLD | MonsterFlags4.BREATHE_POISON
		_level.dungeon_monsters.monster_validation_function = validation_function_dragon
	else:
		_level.dungeon_monsters.monster_validation_function = validation_function_demon
	_level.dungeon_monsters.prepare_random_monster_table()
	for i: int in 16:
		what[i] = _level.dungeon_monsters.get_random_monster(SaveGame.instance.total_difficulty() + 10)
		if what[i] == 0:
			empty = true
	_level.dungeon_monsters.monster_validation_function = Callable()
	_level.dungeon_monsters.prepare_random_monster_table()
	if empty:
		return
	for i: int in 16:
		for j: int in 15:
			var i_1: int = j
			var i_2: int = j + 1
			var p_1: int = Profile.instance.monster_types.types[what[i_1]].level
			var p_2: int = Profile.instance.monster_types.types[what[i_2]].level
			if p_1 > p_2:
				var temp: int = what[i_1]
				what[i_1] = what[i_2]
				what[i_2] = temp
	for i: int in 8:
		what[i] = what[i * 2]
	_level.danger_rating += 10
	if SaveGame.instance.total_difficulty() <= 40 and Rng.roll_die((SaveGame.instance.total_difficulty() * SaveGame.instance.total_difficulty()) + 50) < 300:
		_level.special_danger = true
	for col: int in range(centre_col - 9, centre_col + 10):
		_level.Monsters.place_monster_by_index(centre_row - 2, col, what[0], false, false, false)
		_level.Monsters.place_monster_by_index(centre_row + 2, col, what[0], false, false, false)
	for row: int in range(centre_row - 1, centre_row + 2):
		_level.Monsters.place_monster_by_index(row, centre_col - 9, what[0], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 9, what[0], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 8, what[1], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 8, what[1], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 7, what[1], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 7, what[1], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 6, what[2], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 6, what[2], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 5, what[2], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 5, what[2], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 4, what[3], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 4, what[3], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 3, what[3], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 3, what[3], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col - 2, what[4], false, false, false)
		_level.Monsters.place_monster_by_index(row, centre_col + 2, what[4], false, false, false)
	for col: int in range(centre_col - 1, centre_col + 2):
		_level.Monsters.place_monster_by_index(centre_row + 1, col, what[5], false, false, false)
		_level.Monsters.place_monster_by_index(centre_row - 1, col, what[5], false, false, false)
	_level.Monsters.place_monster_by_index(centre_row, centre_col + 1, what[6], false, false, false)
	_level.Monsters.place_monster_by_index(centre_row, centre_col - 1, what[6], false, false, false)
	_level.Monsters.place_monster_by_index(centre_row, centre_col, what[7], false, false, false)


# Lesser prefab vault
func build_type_7(centre_row: int, centre_col: int) -> void:
	var vault: VaultType = StaticData.vault_types[0]
	var tries: int = 0
	while tries < LevelFactory.SAFE_MAX_ATTEMPTS:
		tries += 1
		vault = StaticData.vault_types[Rng.roll_die(StaticData.vault_types.size()) - 1]
		if vault.category == 7:
			var col_min: int = centre_col - (vault.Width / 2)
			var col_max: int = centre_col + (vault.Width / 2)
			var row_min: int = centre_row - (vault.Height / 2)
			var row_max: int = centre_row + (vault.Height / 2)
			if col_min >= 1 and row_min >= 1 and col_max < _level.cur_wid - 1 and row_max < _level.cur_hgt - 1:
				break
	if tries >= LevelFactory.SAFE_MAX_ATTEMPTS:
		return
	_level.danger_rating += vault.rating
	if (SaveGame.instance.total_difficulty() <= 50 or
		Rng.roll_die(((SaveGame.instance.total_difficulty() - 40) * (SaveGame.instance.total_difficulty() - 40)) + 50) < 400 ):
		_level.special_danger = true
	_build_vault(centre_row, centre_col, vault.Height, vault.Width, vault.text)


# Greater prefab vault
func build_type_8(centre_row: int, centre_col: int) -> void:
	var vault: VaultType = StaticData.vault_types[0]
	var tries: int = 0
	while tries < LevelFactory.SAFE_MAX_ATTEMPTS:
		tries += 1
		vault = StaticData.vault_types[Rng.roll_die(StaticData.vault_types.size()) - 1]
		if vault.category == 8:
			var col_min: int = centre_col - (vault.Width / 2)
			var col_max: int = centre_col + (vault.Width / 2)
			var row_min: int = centre_row - (vault.Height / 2)
			var row_max: int = centre_row + (vault.Height / 2)
			if col_min >= 1 and row_min >= 1 and col_max < _level.cur_wid - 1 and row_max < _level.cur_hgt - 1:
				break
	if tries >= LevelFactory.SAFE_MAX_ATTEMPTS:
		return
	_level.danger_rating += vault.Rating
	if (SaveGame.instance.total_difficulty() <= 50 or
		Rng.roll_die(((SaveGame.instance.total_difficulty() - 40) * (SaveGame.instance.total_difficulty() - 40)) + 50) < 400 ):
		_level.special_danger = true
	_build_vault(centre_row, centre_col, vault.Height, vault.Width, vault.text)


func place_down_stairs(row: int, col: int) -> void:
	var tile: GridTile = _level.get_grid(row, col)
	tile.set_feature("DownStair")


func place_locked_door(row: int, col: int) -> void:
	var tile: GridTile = _level.get_grid(row, col)
	tile.set_feature("LockedDoor%d" % [Rng.roll_die(7)])


func place_random_stairs(row: int, col: int) -> void:
	if not _level.grid_open_no_item(row, col):
		return
	if SaveGame.instance.current_depth <= 0:
		pass
	if (SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth) or
		SaveGame.instance.current_depth == SaveGame.instance.current_dungeon.max_level ):
		if SaveGame.instance.current_dungeon.is_tower:
			place_down_stairs(row, col)
		else:
			place_up_stairs(row, col)
	elif Rng.roll_die(100) <= 50:
		place_down_stairs(row, col)
	else:
		place_up_stairs(row, col)


func place_secret_door(row: int, col: int) -> void:
	var tile: GridTile = _level.get_grid(row, col)
	tile.set_feature("SecretDoor")


func place_up_stairs(row: int, col: int) -> void:
	var tile: GridTile = _level.get_grid(row, col)
	tile.set_feature("UpStair")


func validation_function_animal(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.flags_3.is_set(MonsterFlags3.ANIMAL):
		return false
	return true


func validation_function_chapel(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if not monster_type.name.contains("haman"):
		return false
	return true


func validation_function_clone(index: int) -> bool:
	return index == _template_monster_type


func validation_function_cult(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if not monster_type.name.contains("Cult"):
		return false
	return true


func validation_function_demon(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.character != "U":
		return false
	return true


func validation_function_dragon(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if not "Dd".contains(monster_type.character):
		return false
	if monster_type.flags_4.is_set(_vault_dragon_mask_4):
		return false
	return true


func validation_function_giant(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.character != "P":
		return false
	return true


func validation_function_jelly(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.flags_3.is_set(MonsterFlags3.EVIL):
		return false
	if not "ijm,".contains(monster_type.character):
		return false
	return true


func validation_function_kennel(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	return monster_type.character == "Z" or monster_type.character == "C"


func validation_function_orc(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.character != "o":
		return false
	return true


func validation_function_symbol(index: int) -> bool:
	return (Profile.instance.monster_types.types[index].character == Profile.instance.monster_types.types[_template_monster_type].character and
		Profile.instance.monster_types.types[index].flags_1.is_set(MonsterFlags1.UNIQUE) )


func validation_function_treasure(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if not monster_type.character == "!" or monster_type.character == "|" or monster_type.character == "$" or monster_type.character == "?" or monster_type.character == "=":
		return false
	return true


func validation_function_troll(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.character != "T":
		return false
	return true


func validation_function_undead(index: int) -> bool:
	var monster_type: MonsterType = Profile.instance.monster_types.types[index]
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	if monster_type.flags_3.is_set(MonsterFlags3.UNDEAD):
		return false
	return true


func vault_monsters(row_start: int, col_start: int, num: int) -> void:
	for k: int in num:
		for i: int in 9:
			var location: Location = _level.scatter(row_start, col_start, 1)
			var row: int = location.row
			var col: int = location.col
			if not _level.grid_passable_no_creature(row, col):
				continue
			_level.monster_level = SaveGame.instance.total_difficulty() + 2
			_level.dungeon_monsters.place_random_monster(row, col, true, true)
			_level.monster_level = SaveGame.instance.total_difficulty()


func vault_objects(row: int, col: int, num: int) -> void:
	var tries: int = 0
	var row_place: int
	var col_place: int
	for n: int in num:
		for i: int in 11:
			while tries < LevelFactory.SAFE_MAX_ATTEMPTS:
				row_place = Rng.random_around(row, 2)
				col_place = Rng.random_around(col, 3)
				tries += 1
				if not _level.in_inner_bounds(row_place, col_place):
					continue
				break
			if not _level.grid_open_no_item(row_place, col_place):
				continue
			if Rng.roll_die(100) <= 75:
				_level.place_item(row_place, col_place, false, false)
			else:
				_level.place_gold(row_place, col_place)
			break


func vault_trap(row: int, col: int, row_d: int, col_d: int) -> void:
	var tries: int = 0
	var row_place: int
	var col_place: int
	for count: int in 6:
		while tries < LevelFactory.SAFE_MAX_ATTEMPTS:
			row_place = Rng.random_around(row, row_d)
			col_place = Rng.random_around(col, col_d)
			tries += 1
			if not _level.in_inner_bounds(row_place, col_place):
				continue
			break
		if not _level.grid_open_no_item_or_creature(row_place, col_place):
			continue
		_level.place_trap(row_place, col_place)
		break


func vault_traps(row: int, col: int, row_d: int, col_d: int, num: int) -> void:
	for i: int in num:
		vault_trap(row, col, row_d, col_d)


func _build_vault(centre_row: int, centre_col: int, row_max: int, col_max: int, data: String) -> void:
	var index: int = 0
	for d_row: int in row_max:
		for d_col: int in col_max:
			var t: String = data[index]
			index += 1
			var col: int = centre_col - (col_max / 2) + d_col
			var row: int = centre_row - (row_max / 2) + d_row
			if t == " ":
				continue
			var tile: GridTile = _level.get_grid(row, col)
			tile.revert_to_background()
			tile.tile_flags.set_bits(GridTile.IN_ROOM | GridTile.IN_VAULT)
			match t:
				"%":
					tile.set_feature("WallOuter")
				"#":
					tile.set_feature("WallInner")
				"X":
					tile.set_feature("WallPermInner")
				"*":
					if Rng.roll_die(100) <= 75:
						_level.place_item(row, col, false, false)
					else:
						_level.place_trap(row, col)
				"+":
					place_secret_door(row, col)
				"^":
					_level.place_trap(row, col)
	index = 0
	for d_row: int in row_max:
		for d_col: int in col_max:
			var t: String = data[index]
			index += 1
			var col: int = centre_col - (col_max / 2) + d_col
			var row: int = centre_row - (row_max / 2) + d_row
			if t == " ":
				continue
			match t:
				"&":
					_level.monster_level = SaveGame.instance.total_difficulty() + 5
					_level.Monsters.place_random_monster(row, col, true, true)
					_level.monster_level = SaveGame.instance.total_difficulty()
				"@":
					_level.monster_level = SaveGame.instance.total_difficulty() + 11
					_level.Monsters.place_random_monster(row, col, true, true)
					_level.monster_level = SaveGame.instance.total_difficulty()
				"9":
					_level.monster_level = SaveGame.instance.total_difficulty() + 9
					_level.Monsters.place_random_monster(row, col, true, true)
					_level.monster_level = SaveGame.instance.total_difficulty()
					_level.item_level = SaveGame.instance.total_difficulty() + 7
					_level.place_item(row, col, true, false)
					_level.item_level = SaveGame.instance.total_difficulty()
				"8":
					_level.monster_level = SaveGame.instance.total_difficulty() + 40
					_level.Monsters.place_random_monster(row, col, true, true)
					_level.monster_level = SaveGame.instance.total_difficulty()
					_level.item_level = SaveGame.instance.total_difficulty() + 20
					_level.place_item(row, col, true, true)
					_level.item_level = SaveGame.instance.total_difficulty()
				",":
					if Rng.roll_die(100) <= 50:
						_level.monster_level = SaveGame.instance.total_difficulty() + 3
						_level.Monsters.place_random_monster(row, col, true, true)
						_level.monster_level = SaveGame.instance.total_difficulty()
					if Rng.roll_die(100) < 50:
						_level.item_level = SaveGame.instance.total_difficulty() + 7
						_level.place_item(row, col, false, false)
						_level.item_level = SaveGame.instance.total_difficulty()
					break
				"A":
					_level.item_level = SaveGame.instance.total_difficulty() + 12
					_level.place_item(row, col, true, false)
					_level.item_level = SaveGame.instance.total_difficulty()
