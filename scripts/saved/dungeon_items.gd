# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DungeonItems
extends Resource

const MAX_DUNGEON_ITEMS: int = 256

@export var items: Array[Item]

var dungeon_level: DungeonLevel:
	set = _set_dungeon_level, get = _get_dungeon_level
var _dungeon_level: DungeonLevel


static func new_dungeon_items(new_dungeon_level: DungeonLevel) -> DungeonItems:
	var dungeon_items: DungeonItems = DungeonItems.new()
	dungeon_items._dungeon_level = new_dungeon_level
	return dungeon_items


func _init() -> void:
	items = []
	for i: int in MAX_DUNGEON_ITEMS:
		items.append(Item.new())


func wipe() -> void:
	for i: int in range(1, MAX_DUNGEON_ITEMS):
		var item: Item = items[i]
		if item.item_type == null:
			continue
		if item.holding_monster_index != 0:
			var monster: Monster = dungeon_level.dungeon_monsters.monsters[item.holding_monster_index]
			monster.first_held_item_index = 0
		else:
			dungeon_level.get_grid(item.map_row, item.map_col).item_index = 0
		items[i] = Item.new()
	dungeon_level.item_count_max = 1
	dungeon_level.item_count = 0


func get_free_item_slot() -> int:
	if dungeon_level.item_count_max < MAX_DUNGEON_ITEMS:
		var i: int = dungeon_level.item_count_max
		dungeon_level.item_count_max += 1
		dungeon_level.item_count += 1
		return i
	for i: int in dungeon_level.item_count_max:
		var item: Item = items[i]
		if item.item_type != null:
			continue
		dungeon_level.item_count += 1
		return i
	return 0


func drop_near(item: Item, loss_chance: int, row: int, col: int) -> void:
	var tile: GridTile
	var flag: bool = false
	var done: bool = false
	var plural: bool = item.count != 1
	var item_name: String = item.description(false, 0)
	if not (item.is_legendary() or item.is_artifact()) and Rng.roll_die(100) <= loss_chance:
		var p: String = "" if plural else "s"
		Profile.instance.message_print("The %s disappear%s." % [item_name, p])
		return
	var b_s: int = -1
	var b_n: int = 0
	var row_base: int = row
	var col_base: int = col
	var row_try: int
	var col_try: int
	var item_index: int
	for d_row: int in range(-3, 4):
		for d_col: int in range(-3, 4):
			var combine: bool = false
			var dist_squared: int = (d_row * d_row) + (d_col * d_col)
			if dist_squared > 10:
				continue
			row_try = row + d_row
			col_try = col + d_col
			if not dungeon_level.in_inner_bounds(row_try, col_try):
				continue
			if not dungeon_level.line_of_sight(row, col, row_try, col_try):
				continue
			tile = dungeon_level.get_grid(row_try, col_try)
			if not tile.feature_type.is_open_floor:
				continue
			var k: int = 0
			item_index = tile.item_index
			while item_index != 0:
				var floor_item: Item = items[item_index]
				item_index = floor_item.next_in_stack
				if floor_item.can_absorb(item):
					combine = true
				k += 1
			if not combine:
				k += 1
			if k > 99:
				continue
			var s: int = 1000 - (dist_squared + (k * 5))
			if s < b_s:
				continue
			if s > b_s:
				b_n = 0
			b_n += 1
			if b_n >= 2 and Rng.roll_die(b_n) != 1:
				continue
			b_s = s
			row_base = row_try
			col_base = col_try
			flag = true
	if not flag and not (item.is_legendary() or item.is_artifact()):
		var p: String = "" if plural else "s"
		Profile.instance.message_print("The %s disappear%s." % [item_name, p])
		return
	var i: int = 0
	while not flag:
		if i < 1000:
			row_try = Rng.random_around(row_base, 1)
			col_try = Rng.random_around(col_base, 1)
		else:
			row_try = Rng.roll_die(dungeon_level.cur_hgt) - 1
			col_try = Rng.roll_die(dungeon_level.cur_row) - 1
		tile = dungeon_level.get_grid(row_try, col_try)
		if not tile.feature_type.is_open_floor:
			continue
		row_base = row_try
		col_base = col_try
		if not dungeon_level.grid_open_no_item(row_base, col_base):
			continue
		flag = true
	tile = dungeon_level.get_grid(row_base, col_base)
	item_index = tile.item_index
	while item_index != 0:
		var floor_item: Item = items[item_index]
		item_index = floor_item.next_in_stack
		if floor_item.can_absorb(item):
			floor_item.absorb(item)
			done = true
			break
	var free_slot: int = get_free_item_slot()
	if not done and free_slot == 0:
		var p: String = "" if plural else "s"
		Profile.instance.message_print("The %s disappear%s." % [item_name, p])
		if item.artifact_index != 0:
			Profile.instance.artifacts[item.artifact_index].current_number = 0
		return
	if not done:
		items[free_slot] = item.get_copy()
		item = items[free_slot]
		item.map_row = row_base
		item.map_col = col_base
		item.holding_monster_index = 0
		item.next_in_stack = tile.item_index
		tile.item_index = free_slot
	dungeon_level.note_spot(row_base, col_base)
	dungeon_level.redraw_single_location(row_base, col_base)
	if loss_chance != 0 and row_base == SaveGame.instance.player.map_row and col_base == SaveGame.instance.player.map_col:
		Profile.instance.message_print("You feel something roll beneath your feet.")


func _set_dungeon_level(value: DungeonLevel) -> void:
	_dungeon_level = value


func _get_dungeon_level() -> DungeonLevel:
	if _dungeon_level == null:
		_dungeon_level = SaveGame.instance.dungeon_level
	return _dungeon_level
