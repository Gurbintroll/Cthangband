# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name LevelFactory
extends RefCounted

const TOWN_MONSTERS_DAY: int = 4
const TOWN_MONSTERS_NIGHT: int = 8
const DUNGEON_MONSTERS: int = 14
const ALLOC_SET_BOTH: int = 3
const ALLOC_SET_CORR: int = 1
const ALLOC_SET_ROOM: int = 2
const ALLOC_TYP_GOLD: int = 4
const ALLOC_TYP_OBJECT: int = 5
const ALLOC_TYP_RUBBLE: int = 1
const ALLOC_TYP_TRAP: int = 3
const BLOCK_HGT: int = 21
const BLOCK_WID: int = 11
const DUN_AMT_GOLD: int = 3
const DUN_AMT_ITEM: int = 3
const DUN_AMT_ROOM: int = 9
const DUN_STR_DEN: int = 5
const DUN_STR_MAG: int = 3
const DUN_STR_MC: int = 90
const DUN_STR_QC: int = 40
const DUN_STR_QUA: int = 2
const DUN_STR_RNG: int = 2
const DUN_TUN_CHG: int = 30
const DUN_TUN_CON: int = 15
const DUN_TUN_JCT: int = 90
const DUN_TUN_PEN: int = 25
const DUN_TUN_RND: int = 10
const CENT_MAX: int = 100
const DOOR_MAX: int = 200
const MAX_ROOMS_COL: int = DungeonLevel.MAX_WID / BLOCK_WID
const MAX_ROOMS_ROW: int = DungeonLevel.MAX_HGT / BLOCK_HGT
const SAFE_MAX_ATTEMPTS: int = 5000
const TUNN_MAX: int = 900
const WALL_MAX: int = 500
const DARK_EMPTY: int = 5
const DUN_DEST: int = 18
const DUN_ROOMS: int = 50
const DUN_UNUSUAL: int = 194
const EMPTY_LEVEL: int = 15
const SMALL_LEVEL: int = 3

var _level: DungeonLevel
var _savegame: SaveGame
var _player: Player
var _build_info: LevelBuildInfo
var _rooms: Array[Room] = [
	Room.new(0, 0, 0, 0, 0),
	Room.new(0, 0, -1, 1, 1),
	Room.new(0, 0, -1, 1, 1),
	Room.new(0, 0, -1, 1, 3),
	Room.new(0, 0, -1, 1, 3),
	Room.new(0, 0, -1, 1, 5),
	Room.new(0, 0, -1, 1, 5),
	Room.new(0, 1, -1, 1, 5),
	Room.new(-1, 2, -2, 3, 10),
	Room.new(0, 1, -1, 1, 1),
]


func _init(level: DungeonLevel, savegame: SaveGame) -> void:
	_level = level
	_savegame = savegame
	_player = _savegame.player


func generate_new_level() -> void:
	var region: WildernessRegion = _savegame.island.get_region(_player.wilderness_row, _player.wilderness_col)
	# Count how many tries we've had creating an interesting level - we'll give up after 100
	var tries: int = 1
	while true:
		# Assume the level is okay
		var okay: bool = true
		_level.item_count_max = 1
		_level.monster_count_max = 1
		if _savegame.current_depth == 0:
			_level.reset_grid("Grass")
		elif region.dungeon.is_tower:
			_level.reset_grid("TowerFloor")
		else:
			_level.reset_grid("DungeonFloor")
		_level.panel_row_min = 0
		_level.panel_col_min = 0
		_level.panel_row_max = 0
		_level.panel_col_max = 0
		# Set up the difficulty and bias for monsters
		if _savegame.current_depth == 0:
			if region.town != null:
				# Surface and town = no difficulty or bias
				_savegame.current_town = region.town
				_savegame.dungeon_difficulty = 0
				_level.dungeon_monsters.dungeon_bias = 0
				if region.town.character == "k":
					# Unless our town is Kadath
					_savegame.dungeon_difficulty = 35
					_level.dungeon_monsters.dungeon_bias = SummonType.SUMMON_CTHULOID
			elif region.dungeon != null:
				# Surface and dungeon = half dungeon's difficulty
				_savegame.dungeon_difficulty = maxi(region.dungeon.offset / 2, 4)
				_level.dungeon_monsters.dungeon_bias = region.dungeon.bias
			else:
				# Surface wilderness = 2
				_savegame.dungeon_difficulty = 2
				_level.dungeon_monsters.dungeon_bias = SummonType.SUMMON_ANIMAL
		else:
			# Not on surface means use dungeon's
			_savegame.dungeon_difficulty = _savegame.current_dungeon.offset
			_level.dungeon_monsters.dungeon_bias = _savegame.current_dungeon.bias
		_level.monster_level = _savegame.total_difficulty()
		_level.item_level = _savegame.total_difficulty()
		_level.special_treasure = false
		_level.special_danger = false
		_level.treasure_rating = 0
		_level.danger_rating = 0
		if _savegame.current_depth == 0:
			# On surface, so use a one-screen size
			_level.cur_hgt = Constants.SCREEN_HEIGHT
			_level.cur_wid = Constants.SCREEN_WIDTH
			_level.max_panel_rows = (_level.cur_hgt / Constants.SCREEN_HEIGHT * 2) - 2
			_level.max_panel_cols = (_level.cur_wid / Constants.SCREEN_WIDTH * 2) - 2
			_level.panel_col = _level.max_panel_cols
			_level.panel_row = _level.max_panel_rows
			# Surface can be either town or wilderness
			if region.town != null:
				town_gen()
			else:
				wilderness_gen()
			# Feelings for surface are always 0
			_level.danger_feeling = 0
			_level.treasure_feeling = 0
		else:
			# We're not on the surface, so get the size for a dungeon level
			if _savegame.current_dungeon.is_tower:
				# Towers are always one screen large
				_level.cur_hgt = Constants.SCREEN_HEIGHT
				_level.cur_wid = Constants.SCREEN_WIDTH
				_level.max_panel_rows = 0
				_level.max_panel_cols = 0
				_level.panel_col = 0
				_level.panel_row = 0
			else:
				if Rng.roll_die(SMALL_LEVEL) == 1:
					# Small level
					var temp_wid: int = Rng.roll_die(_level.MAX_WID / Constants.SCREEN_WIDTH)
					var temp_hgt: int = Rng.roll_die(_level.MAX_HGT / Constants.SCREEN_HEIGHT)
					_level.cur_hgt = temp_hgt * Constants.SCREEN_HEIGHT
					_level.cur_wid = temp_wid * Constants.SCREEN_WIDTH
					_level.max_panel_rows = (temp_hgt * 2) - 2
					_level.max_panel_cols = (temp_wid * 2) - 2
					_level.panel_col = _level.max_panel_cols
					_level.panel_row = _level.max_panel_rows
				else:
					# Full size level
					_level.cur_hgt = DungeonLevel.MAX_HGT
					_level.cur_wid = DungeonLevel.MAX_WID
					_level.max_panel_rows = (DungeonLevel.MAX_HGT / Constants.SCREEN_HEIGHT * 2) - 2
					_level.max_panel_cols = (DungeonLevel.MAX_WID / Constants.SCREEN_WIDTH * 2) - 2
					_level.panel_col = _level.max_panel_cols
					_level.panel_row = _level.max_panel_rows
			# Try making a dungeon level
			if not underground_gen():
				# If the level was rejected, try again
				okay = false
			# Make the feelings based on the level's contents
			if _level.special_treasure:
				_level.treasure_feeling = TreasureRating.SPECIAL
			else:
				_level.treasure_feeling = TreasureRating.rating_to_feeling(_level.treasure_rating)
			if _level.special_danger:
				_level.danger_feeling = DangerRating.SPECIAL
			else:
				_level.danger_feeling = DangerRating.rating_to_feeling(_level.danger_rating)
		# Check for reasons to reject the level
		if _level.item_count >= _level.item_count_max:
			okay = false
		if _level.monster_count >= _level.monster_count_max:
			okay = false
		# If it's less than our 100th try, reject boring levels (based on depth)
		if tries < 100:
			var total_feeling: int = _level.treasure_feeling + _level.danger_feeling
			if total_feeling > 18:
				okay = false
			if _savegame.total_difficulty() >= 5 and total_feeling > 16:
				okay = false
			if _savegame.total_difficulty() >= 10 and total_feeling > 14:
				okay = false
			if _savegame.total_difficulty() >= 20 and total_feeling > 12:
				okay = false
			if _savegame.total_difficulty() >= 40 and total_feeling > 10:
				okay = false
		# If the level was okay, exit the loop
		if okay:
			break
		# The level was not okay, so wipe monsters and items before retrying
		_level.dungeon_monsters.wipe()
		_level.dungeon_items.wipe()
		tries += 1
	# We fell out of the loop, so we're finished
	# Mark the player's time of entry for feeling purposes
	_player.game_time.mark_level_entry()


func town_gen() -> void:
	for row: int in _level.cur_hgt:
		for col: int in _level.cur_wid:
			var grid: GridTile = _level.get_grid(row, col)
			grid.revert_to_background()
	make_town_walls()
	make_corner_towers(_player.wilderness_row, _player.wilderness_col)
	make_town_contents()
	resolve_paths()
	if _player.game_time.is_light():
		# Light up the whole town in day
		for row: int in _level.cur_hgt:
			for col: int in _level.cur_wid:
				_level.get_grid(row, col).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		# Daytime has fewer monsters
		for i: int in TOWN_MONSTERS_DAY:
			_level.dungeon_monsters.spawn_monster(3, true)
	else:
		# Night has more monsters
		for i: int in TOWN_MONSTERS_NIGHT:
			_level.dungeon_monsters.spawn_monster(3, true)


func make_town_walls() -> void:
	for col: int in _level.cur_wid:
		var grid: GridTile = _level.get_grid(0, col)
		grid.set_feature("TownWall")
		grid.tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		grid = _level.get_grid(_level.cur_hgt - 1, col)
		grid.set_feature("TownWall")
		grid.tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	for row: int in _level.cur_hgt:
		var grid: GridTile = _level.get_grid(row, 0)
		grid.set_feature("TownWall")
		grid.tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		grid = _level.get_grid(row, _level.cur_wid - 1)
		grid.set_feature("TownWall")
		grid.tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) - 2).set_background_feature("InsideGatehouse")
	_level.get_grid(0, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) - 1).set_background_feature("InsideGatehouse")
	_level.get_grid(0, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) + 1).set_background_feature("InsideGatehouse")
	_level.get_grid(0, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) + 2).set_background_feature("InsideGatehouse")
	_level.get_grid(0, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) - 2).set_background_feature("InsideGatehouse")
	_level.get_grid(1, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) - 1).set_background_feature("InsideGatehouse")
	_level.get_grid(1, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) + 1).set_background_feature("InsideGatehouse")
	_level.get_grid(1, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) + 2).set_background_feature("InsideGatehouse")
	_level.get_grid(1, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) - 2).set_feature("TownWall")
	_level.get_grid(0, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) - 1).set_feature("TownWall")
	_level.get_grid(0, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, _level.cur_wid / 2).set_feature("PathBorderNS")
	_level.get_grid(0, _level.cur_wid / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) + 1).set_feature("TownWall")
	_level.get_grid(0, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(0, (_level.cur_wid / 2) + 2).set_feature("TownWall")
	_level.get_grid(0, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) - 2).set_feature("TownWall")
	_level.get_grid(1, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) - 1).set_feature("TownWall")
	_level.get_grid(1, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, _level.cur_wid / 2).set_feature("PathBase")
	_level.get_grid(1, _level.cur_wid / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) + 1).set_feature("TownWall")
	_level.get_grid(1, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(1, (_level.cur_wid / 2) + 2).set_feature("TownWall")
	_level.get_grid(1, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 2).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 1).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 1).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 2).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 2).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 1).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 1).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 2).set_background_feature("InsideGatehouse")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 2).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 1).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, _level.cur_wid / 2).set_feature("PathBorderNS")
	_level.get_grid(_level.cur_hgt - 1, _level.cur_wid / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 1).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 2).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 1, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 2).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 1).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, _level.cur_wid / 2).set_feature("PathBase")
	_level.get_grid(_level.cur_hgt - 2, _level.cur_wid / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 1).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 2).set_feature("TownWall")
	_level.get_grid(_level.cur_hgt - 2, (_level.cur_wid / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, 0).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, 0).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, 0).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, 0).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, 0).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, 0).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt / 2, 0).set_feature("PathBorderEW")
	_level.get_grid(_level.cur_hgt / 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, 0).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, 0).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt / 2, 1).set_feature("PathBase")
	_level.get_grid(_level.cur_hgt / 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 1).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 2).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 2).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 2).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 2).set_background_feature("InsideGatehouse")
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt / 2, _level.cur_wid - 1).set_feature("PathBorderEW")
	_level.get_grid(_level.cur_hgt / 2, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 1).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 2).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 2, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 2).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) - 1, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid(_level.cur_hgt / 2, _level.cur_wid - 2).set_feature("PathBase")
	_level.get_grid(_level.cur_hgt / 2, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 2).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 1, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 2).set_feature("TownWall")
	_level.get_grid((_level.cur_hgt / 2) + 2, _level.cur_wid - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)


func make_corner_towers(wild_row: int, wild_col: int) -> void:
	var island: Island = _savegame.island
	var height: int = _level.cur_hgt
	var width: int = _level.cur_wid
	if island.get_region(wild_row, wild_col).town != null or island.get_region(wild_row - 1, wild_col).town != null or island.get_region(wild_row, wild_col - 1).town != null or island.get_region(wild_row - 1, wild_col - 1).town != null:
		_level.get_grid(0, 0).set_background_feature("InsideGatehouse")
		_level.get_grid(0, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, 0).set_background_feature("InsideGatehouse")
		_level.get_grid(1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, 1).set_background_feature("InsideGatehouse")
		_level.get_grid(1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, 1).set_background_feature("InsideGatehouse")
		_level.get_grid(0, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, 0).revert_to_background()
		_level.get_grid(0, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, 0).set_feature("TownWall")
		_level.get_grid(1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, 1).set_feature("TownWall")
		_level.get_grid(1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, 1).set_feature("TownWall")
		_level.get_grid(0, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row, wild_col).town != null or island.get_region(wild_row - 1, wild_col).town != null or island.get_region(wild_row, wild_col + 1).town != null or island.get_region(wild_row - 1, wild_col + 1).town != null:
		_level.get_grid(0, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(0, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(0, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, width - 1).revert_to_background()
		_level.get_grid(0, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, width - 1).set_feature("TownWall")
		_level.get_grid(1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, width - 2).set_feature("TownWall")
		_level.get_grid(1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, width - 2).set_feature("TownWall")
		_level.get_grid(0, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row, wild_col).town != null or island.get_region(wild_row + 1, wild_col).town != null or island.get_region(wild_row, wild_col + 1).town != null or island.get_region(wild_row + 1, wild_col + 1).town != null:
		_level.get_grid(height - 1, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, width - 1).revert_to_background()
		_level.get_grid(height - 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, width - 1).set_feature("TownWall")
		_level.get_grid(height - 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, width - 2).set_feature("TownWall")
		_level.get_grid(height - 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, width - 2).set_feature("TownWall")
		_level.get_grid(height - 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row, wild_col).town != null or island.get_region(wild_row + 1, wild_col).town != null or island.get_region(wild_row, wild_col - 1).town != null or island.get_region(wild_row + 1, wild_col - 1).town != null:
		_level.get_grid(height - 1, 0).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, 0).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, 0).revert_to_background()
		_level.get_grid(height - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, 0).set_feature("TownWall")
		_level.get_grid(height - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, 1).set_feature("TownWall")
		_level.get_grid(height - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, 1).set_feature("TownWall")
		_level.get_grid(height - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)


func make_town_contents() -> void:
	var store_deck: Array[int] = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
	var grid: GridTile
	var middle_row: int = _level.cur_hgt / 2
	var middle_col: int = _level.cur_wid / 2
	Rng.use_fixed_seed(_savegame.current_town.random_seed)
	# Make a crossroads of paths
	for col: int in range(1, _level.cur_wid - 1):
		_level.get_grid(middle_row, col).set_feature("PathBase")
	for row: int in range(1, _level.cur_hgt - 1):
		_level.get_grid(row, middle_col).set_feature("PathBase")
	# Four by four grid of potential buildings
	for row: int in 4:
		for col: int in 4:
			# Only non-corners have stores
			if row == 1 or row == 2 or col == 1 or col == 2:
				# Pick a random store from the deck and build the town's store of that index
				var store_index: int = Rng.roll_die(store_deck.size()) - 1
				build_store(_savegame.current_town.stores[store_deck[store_index]], row, col)
				store_deck.remove_at(store_index)
			# Corners usually have a field (or a graveyard if in Kadath)
			else:
				match Rng.roll_die(10):
					3, 7, 9:
						pass
					_:
						if _savegame.current_town.character == "K":
							build_graveyard(row, col)
						else:
							build_field(row, col)
	# Add 1d10-6 rocks in random places
	for n: int in Rng.roll_die(10) - 6:
		var row: int = Rng.roll_die(_level.cur_hgt - 2)
		var col: int = Rng.roll_die(_level.cur_wid - 2)
		grid = _level.get_grid(row, col)
		if grid.feature_type.name == grid.background_feature.name:
			grid.set_feature("Rock")
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	# Add 1d5+5 trees in random places
	for n: int in Rng.roll_die(5) + 5:
		var row: int = Rng.roll_die(_level.cur_hgt - 2)
		var col: int = Rng.roll_die(_level.cur_wid - 2)
		grid = _level.get_grid(row, col)
		if grid.feature_type.name == grid.background_feature.name:
			grid.set_feature("Tree")
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	# Add 1d5+5 bushes in random places
	for n: int in Rng.roll_die(5) + 5:
		var row: int = Rng.roll_die(_level.cur_hgt - 2)
		var col: int = Rng.roll_die(_level.cur_wid - 2)
		grid = _level.get_grid(row, col)
		if grid.feature_type.name == grid.background_feature.name:
			grid.set_feature("Bush")
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	# Add a feature (maybe a fountain, maybe a signpost) at the town centre
	make_town_centre()
	# Add path borders at the edges
	grid = _level.get_grid(0, middle_col)
	grid.set_feature("PathBorderNS")
	grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	grid = _level.get_grid(_level.cur_hgt - 1, middle_col)
	grid.set_feature("PathBorderNS")
	grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	grid = _level.get_grid(middle_row, 0)
	grid.set_feature("PathBorderEW")
	grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	grid = _level.get_grid(middle_row, _level.cur_wid - 1)
	grid.set_feature("PathBorderEW")
	grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	# Find a location for the dungeon entrance
	var tries: int = 0
	var stair_row: int
	var stair_col: int
	while tries < SAFE_MAX_ATTEMPTS:
		tries += 1
		stair_row = Rng.random_between(12, 29)
		stair_col = Rng.random_between(17, 46)
		if _level.grid_open_no_item_or_creature(stair_row, stair_col):
			break
	grid = _level.get_grid(stair_row, stair_col)
	grid.set_feature("DownStair")
	grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	# Set up the player's start location
	Rng.use_main_sequence()
	match _savegame.came_from:
		LevelStart.START_RANDOM:
			new_player_spot()
		LevelStart.START_STAIRS:
			_player.map_row = stair_row
			_player.map_col = stair_col
		LevelStart.START_WALK:
			pass
		LevelStart.START_HOUSE:
			for store: Store in _savegame.current_town.stores:
				if store.store_type == StoreType.HOME:
					_player.map_row = store.door_row
					_player.map_col = store.door_col
					break


func new_player_spot() -> bool:
	var row: int = 0
	var col: int = 0
	var tries: int = 5000
	while tries > 0:
		tries -= 1
		row = Rng.random_between(1, _level.cur_hgt - 1)
		col = Rng.random_between(1, _level.cur_wid - 1)
		if not _level.grid_open_no_item_or_creature(row, col):
			continue
		if _level.get_grid(row, col).tile_flags.is_set(GridTile.IN_VAULT):
			continue
		break
	if tries < 1:
		return false
	_player.map_row = row
	_player.map_col = col
	return true


func build_store(store: Store, grid_row: int, grid_col: int) -> void:
	# If we have an empty lot, then don't actually build a store
	if _savegame.current_town.character != "K":
		if store.store_type == StoreType.EMPTY_LOT:
			match Rng.roll_die(10):
				3, 7, 9:
					pass
				_:
					if _savegame.current_town.character == "K":
						build_graveyard(grid_row, grid_col)
					else:
						build_field(grid_row, grid_col)
			return
	# We have an actual store to build, so convert the grid to actual map coordinates
	var row_base: int = grid_row * 9 + 6
	var col_base: int = grid_col * 15 + 10
	var row_min: int = row_base - Rng.roll_die(2)
	var row_max: int = row_base + Rng.roll_die(2) + 1
	var col_min: int = col_base - Rng.roll_die(3) - 2
	var col_max: int = col_base + Rng.roll_die(3) + 2
	# Must have an odd height
	if (row_max - row_min) % 2 == 0:
		row_max += 1
	# Fill in the store area appropriately
	for row: int in range(row_min, row_max + 1):
		for col: int in range(col_min, col_max + 1):
			var grid: GridTile = _level.get_grid(row, col)
			# If we're in Kadath, all buildings are ruins
			if _savegame.current_town.character == "K":
				match Rng.roll_die(6):
					1:
						grid.set_feature("WallInner")
					2, 3, 4:
						grid.set_feature("Rubble")
					_:
						pass
			else:
				if row == row_max:
					grid.set_feature("WallPermBuilding")
				else:
					grid.set_feature("Roof")
	# Place a door in the lower wall
	var door_row: int = row_max
	var door_col: int = Rng.random_between(col_min + 1, col_max - 2)
	# Kadath just has random doors, not actual store doors
	if _savegame.current_town.character == "K":
		if Rng.roll_die(8) == 6:
			place_random_door(door_row, door_col)
	else:
		var door_grid: GridTile = _level.get_grid(door_row, door_col)
		door_grid.set_feature(store.feature_type)
	# Get the store to remember where its door is
	store.door_row = door_row
	store.door_col = door_col
	# Build a path from the store door to the main road
	for row: int in range(door_row + 1, row_base + 7):
		_level.get_grid(row, door_col).set_feature("PathBase")
	var d_x: int = sign((_level.cur_wid / 2) - door_col)
	for col: int in range(door_col + d_x, _level.cur_wid / 2, d_x):
		_level.get_grid(row_base + 6, col).set_feature("PathBase")


func place_random_door(row: int, col: int) -> void:
	var grid: GridTile = _level.get_grid(row, col)
	var roll: int = Rng.roll_die(1000)
	if roll < 300:
		grid.set_feature("OpenDoor")
	elif roll < 400:
		grid.set_feature("BrokenDoor")
	elif roll < 600:
		grid.set_feature("SecretDoor")
	elif roll < 900:
		grid.set_feature("LockedDoor0")
	elif roll < 999:
		grid.set_feature("LockedDoor%d" % [Rng.roll_die(7)])
	else:
		grid.set_feature("JammedDoor%d" % [Rng.roll_die(8) - 1])


func build_graveyard(grid_row: int, grid_col: int) -> void:
	var base_row: int = (grid_row * 9) + 8
	var base_col: int = (grid_col * 15) + 10
	var row_min: int = base_row - Rng.roll_die(2) - 1
	var row_max: int = base_row + Rng.roll_die(2) + 1
	var col_min: int = base_col - Rng.roll_die(3) - 2
	var col_max: int = base_col + Rng.roll_die(3) + 2
	for i: int in Rng.random_between(10, 20):
		var row: int = (Rng.random_between(col_min, col_max) / 2 * 2) + 1
		var col: int = (Rng.random_between(row_min, row_max) / 2 * 2) + 1
		_level.get_grid(row, col).set_feature("Grave")


func build_field(grid_row: int, grid_col: int) -> void:
	var base_row: int = (grid_row * 9) + 8
	var base_col: int = (grid_col * 15) + 10
	var row_min: int = base_row - Rng.roll_die(2) - 1
	var row_max: int = base_row + Rng.roll_die(2) + 1
	var col_min: int = base_col - Rng.roll_die(3) - 2
	var col_max: int = base_col + Rng.roll_die(3) + 2
	for row: int in range(row_min, row_max):
		for col: int in range(col_min, col_max):
			_level.get_grid(row, col).set_feature("Field")
			_level.get_grid(row, col).set_background_feature("Field")
	if Rng.roll_die(5) == 4:
		var row: int = Rng.random_between(row_min, row_max)
		var col: int = Rng.random_between(col_min, col_max)
		_level.get_grid(row, col).set_feature("Scarecrow")


func make_town_centre() -> void:
	var row: int = _level.cur_hgt / 2
	var col: int = _level.cur_wid / 2
	match Rng.roll_die(12):
		1, 3:
			_level.get_grid(row - 1, col).set_feature("PathBase")
			_level.get_grid(row + 1, col).set_feature("PathBase")
			_level.get_grid(row, col - 1).set_feature("PathBase")
			_level.get_grid(row, col + 1).set_feature("PathBase")
			_level.get_grid(row - 1, col - 1).set_feature("PathBase")
			_level.get_grid(row - 1, col + 1).set_feature("PathBase")
			_level.get_grid(row + 1, col - 1).set_feature("PathBase")
			_level.get_grid(row + 1, col + 1).set_feature("PathBase")
			match Rng.roll_die(6):
				1, 4:
					_level.get_grid(row, col).revert_to_background()
				2:
					_level.get_grid(row, col).set_feature("Statue")
				_:
					_level.get_grid(row, col).set_feature("Fountain")
		2, 8, 9, 12:
			row += 1 if Rng.roll_die(2) == 1 else -1
			col += 1 if Rng.roll_die(2) == 1 else -1
			_level.get_grid(row, col).set_feature("Signpost")
		_:
			return


func resolve_paths() -> void:
	for row: int in range(1, _level.cur_hgt - 1):
		for col: int in range(1, _level.cur_wid - 1):
			if _level.get_grid(row, col).feature_type.name != "PathBase":
				continue
			var map: int = 0
			if _level.get_grid(row - 1, col).feature_type.name.begins_with("Path"):
				map += 1
			if _level.get_grid(row, col - 1).feature_type.name.begins_with("Path"):
				map += 2
			if _level.get_grid(row + 1, col).feature_type.name.begins_with("Path"):
				map += 4
			if _level.get_grid(row, col + 1).feature_type.name.begins_with("Path"):
				map += 8
			match map:
				1, 4, 5:
					_level.get_grid(row, col).set_feature("PathNS")
				2, 8, 10:
					_level.get_grid(row, col).set_feature("PathEW")
				_:
					_level.get_grid(row, col).set_feature("PathJunction")


func wilderness_gen() -> void:
	var island: Island = _savegame.island
	Rng.use_fixed_seed(island.get_region(_player.wilderness_row, _player.wilderness_col).random_seed)
	for row: int in _level.cur_hgt:
		for col: int in _level.cur_wid:
			var elevation: int = island.get_elevation(_player.wilderness_row, _player.wilderness_col, row, col)
			var floor_name: String = "Water"
			var feature_name: String = "Water"
			if elevation > 0:
				floor_name = "Grass"
				if Rng.roll_die(10) < elevation:
					if Rng.roll_die(10) < elevation:
						feature_name = "Tree"
					else:
						feature_name = "Bush"
				else:
					feature_name = "Grass"
			_level.get_grid(row, col).set_feature(feature_name)
			_level.get_grid(row, col).set_background_feature(floor_name)
	for col: int in _level.cur_wid:
		var tile: GridTile = _level.get_grid(0, col)
		tile.set_feature("WaterBorder" if tile.background_feature.name.begins_with("Water") else "WildBorder")
		tile = _level.get_grid(_level.cur_hgt - 1, col)
		tile.set_feature("WaterBorder" if tile.background_feature.name.begins_with("Water") else "WildBorder")
	for row: int in _level.cur_hgt:
		var tile: GridTile = _level.get_grid(row, 0)
		tile.set_feature("WaterBorder" if tile.background_feature.name.begins_with("Water") else "WildBorder")
		tile = _level.get_grid(row, _level.cur_wid - 1)
		tile.set_feature("WaterBorder" if tile.background_feature.name.begins_with("Water") else "WildBorder")
	make_wilderness_walls(_player.wilderness_row, _player.wilderness_col)
	make_corner_towers(_player.wilderness_row, _player.wilderness_col)
	make_wilderness_paths(_player.wilderness_row, _player.wilderness_col)
	var stair_location: Location = make_wilderness_features(_player.wilderness_row, _player.wilderness_col)
	var rocks: int = Rng.roll_die(10)
	for i: int in rocks:
		var row: int = Rng.roll_die(_level.cur_hgt - 2)
		var col: int = Rng.roll_die(_level.cur_wid - 2)
		if _level.get_grid(row, col).feature_type.name != "Grass":
			continue
		_level.get_grid(row, col).set_feature("Rock")
	Rng.use_main_sequence()
	if _savegame.came_from == LevelStart.START_RANDOM:
		new_player_spot()
	elif _savegame.came_from == LevelStart.START_STAIRS:
		_player.map_row = stair_location.row
		_player.map_col = stair_location.col
	elif _savegame.came_from == LevelStart.START_WALK:
		var tile: GridTile = _level.get_grid(_player.map_row, _player.map_col)
		if tile.feature_type.category == FloorTileTypeCategory.TREE or tile.feature_type.name == "Water":
			tile.revert_to_background()
	resolve_paths()
	for row: int in _level.cur_hgt:
		for col: int in _level.cur_wid:
			var tile: GridTile = _level.get_grid(row, col)
			if tile.feature_type.is_open_floor:
				tile.tile_flags.set_bits(GridTile.IN_ROOM)
			if _player.game_time.is_light():
				tile.tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	for i: int in DUNGEON_MONSTERS:
		_level.dungeon_monsters.spawn_monster(3, true)


func make_wilderness_paths(wild_row: int, wild_col: int) -> void:
	var mid_col: int = _level.cur_wid / 2
	var mid_row: int = _level.cur_hgt / 2
	if _savegame.island.get_region(wild_row, wild_col).road_map == 0:
		return
	_level.get_grid(mid_row - 1, mid_col - 1).set_feature("Grass")
	_level.get_grid(mid_row - 1, mid_col).set_feature("Grass")
	_level.get_grid(mid_row - 1, mid_col + 1).set_feature("Grass")
	_level.get_grid(mid_row, mid_col - 1).set_feature("Grass")
	_level.get_grid(mid_row, mid_col).set_feature("PathBase")
	_level.get_grid(mid_row, mid_col + 1).set_feature("Grass")
	_level.get_grid(mid_row + 1, mid_col - 1).set_feature("Grass")
	_level.get_grid(mid_row + 1, mid_col).set_feature("Grass")
	_level.get_grid(mid_row + 1, mid_col + 1).set_feature("Grass")
	if (_savegame.island.get_region(wild_row, wild_col).road_map & RoadDirection.ROAD_UP) != 0:
		var col: int = 0
		_level.get_grid(0, mid_col).set_feature("PathBorderNS")
		_level.get_grid(1, mid_col).set_feature("PathBase")
		_level.get_grid(mid_row - 1, mid_col).set_feature("PathBase")
		for row: int in range(2, mid_row - 1):
			col += Rng.random_between(-2, 2) / 2
			if col > mid_row - 1 - row:
				col = mid_row - 1 - row
			if col < -(mid_row - 1 - row):
				col = -(mid_row - 1 - row)
			if not _level.get_grid(row, mid_col - 1 + col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(row, mid_col - 1 + col).set_feature("Grass")
			_level.get_grid(row, mid_col + col).set_feature("WildPathNS")
			if not _level.get_grid(row, mid_col + 1 + col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(row, mid_col + 1 + col).set_feature("Grass")
	if (_savegame.island.get_region(wild_row, wild_col).road_map & RoadDirection.ROAD_DOWN) != 0:
		var col: int = 0
		_level.get_grid(_level.cur_hgt - 1, mid_col).set_feature("PathBorderNS")
		_level.get_grid(_level.cur_hgt - 2, mid_col).set_feature("PathBase")
		_level.get_grid(mid_row + 1, mid_col).set_feature("PathBase")
		for row: int in range(_level.cur_hgt - 3, mid_row + 1, -1):
			col += Rng.random_between(-2, 2) / 2
			if col > row - (mid_row + 1):
				col = row - (mid_row + 1)
			if col < -(row - (mid_row + 1)):
				col = -(row - (mid_row + 1))
			if not _level.get_grid(row, mid_col - 1 + col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(row, mid_col - 1 + col).set_feature("Grass")
			_level.get_grid(row, mid_col + col).set_feature("WildPathNS")
			if not _level.get_grid(row, mid_col + 1 + col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(row, mid_col + 1 + col).set_feature("Grass")
	if (_savegame.island.get_region(wild_row, wild_col).road_map & RoadDirection.ROAD_LEFT) != 0:
		var row: int = 0
		_level.get_grid(mid_row, 0).set_feature("PathBorderEW")
		_level.get_grid(mid_row, 1).set_feature("PathBase")
		_level.get_grid(mid_row, mid_col - 1).set_feature("PathBase")
		for col: int in range(2, mid_col - 1):
			row += Rng.random_between(-2, 2) / 2
			if row > mid_col - 1 - col:
				row = mid_col - 1 - col
			if row < -(mid_col - 1 - col):
				row = -(mid_col - 1 - col)
			if not _level.get_grid(mid_row - 1 + row, col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(mid_row - 1 + row, col).set_feature("Grass")
			_level.get_grid(mid_row + row, col).set_feature("WildPathEW")
			if not _level.get_grid(mid_row + 1 + row, col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(mid_row + 1 + row, col).set_feature("Grass")
	if (_savegame.island.get_region(wild_row, wild_col).road_map & RoadDirection.ROAD_RIGHT) != 0:
		var row: int = 0
		_level.get_grid(mid_row, _level.cur_wid - 1).set_feature("PathBorderEW")
		_level.get_grid(mid_row, _level.cur_wid - 2).set_feature("PathBase")
		_level.get_grid(mid_row, mid_col + 1).set_feature("PathBase")
		for col: int in range(_level.cur_wid - 3, mid_col + 1, -1):
			row += Rng.random_between(-2, 2) / 2
			if row > col - (mid_col + 1):
				row = col - (mid_col + 1)
			if row < -(col - (mid_col + 1)):
				row = -(col - (mid_col + 1))
			if not _level.get_grid(mid_row - 1 + row, col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(mid_row - 1 + row, col).set_feature("Grass")
			_level.get_grid(mid_row + row, col).set_feature("WildPathEW")
			if not _level.get_grid(mid_row + 1 + row, col).feature_type.name.begins_with("WildPath"):
				_level.get_grid(mid_row + 1 + row, col).set_feature("Grass")


func make_wilderness_walls(wild_row: int, wild_col: int) -> void:
	var island: Island = _savegame.island
	var height: int = _level.cur_hgt
	var width: int = _level.cur_wid
	if island.get_region(wild_row - 1, wild_col).town != null:
		for col: int in width:
			_level.get_grid(0, col).set_feature("TownWall")
			_level.get_grid(0, col).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(0, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(0, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) + 1).set_background_feature("InsideGatehouse")
		_level.get_grid(0, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) + 2).set_background_feature("InsideGatehouse")
		_level.get_grid(0, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(1, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(1, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) + 1).set_background_feature("InsideGatehouse")
		_level.get_grid(1, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) + 2).set_background_feature("InsideGatehouse")
		_level.get_grid(1, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) - 2).set_feature("TownWall")
		_level.get_grid(0, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) - 1).set_feature("TownWall")
		_level.get_grid(0, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, width / 2).set_feature("PathBorderNS")
		_level.get_grid(0, width / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) + 1).set_feature("TownWall")
		_level.get_grid(0, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(0, (width / 2) + 2).set_feature("TownWall")
		_level.get_grid(0, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) - 2).set_feature("TownWall")
		_level.get_grid(1, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) - 1).set_feature("TownWall")
		_level.get_grid(1, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, width / 2).set_feature("PathBase")
		_level.get_grid(1, width / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) + 1).set_feature("TownWall")
		_level.get_grid(1, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(1, (width / 2) + 2).set_feature("TownWall")
		_level.get_grid(1, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row + 1, wild_col).town != null:
		for col: int in width:
			_level.get_grid(height - 1, col).set_feature("TownWall")
			_level.get_grid(height - 1, col).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) + 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) + 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 1, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) - 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) - 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) + 1).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) + 2).set_background_feature("InsideGatehouse")
		_level.get_grid(height - 2, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) - 2).set_feature("TownWall")
		_level.get_grid(height - 1, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) - 1).set_feature("TownWall")
		_level.get_grid(height - 1, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, width / 2).set_feature("PathBorderNS")
		_level.get_grid(height - 1, width / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) + 1).set_feature("TownWall")
		_level.get_grid(height - 1, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 1, (width / 2) + 2).set_feature("TownWall")
		_level.get_grid(height - 1, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) - 2).set_feature("TownWall")
		_level.get_grid(height - 2, (width / 2) - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) - 1).set_feature("TownWall")
		_level.get_grid(height - 2, (width / 2) - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, width / 2).set_feature("PathBase")
		_level.get_grid(height - 2, width / 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) + 1).set_feature("TownWall")
		_level.get_grid(height - 2, (width / 2) + 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height - 2, (width / 2) + 2).set_feature("TownWall")
		_level.get_grid(height - 2, (width / 2) + 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row, wild_col - 1).town != null:
		for row: int in height:
			_level.get_grid(row, 0).set_feature("TownWall")
			_level.get_grid(row, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, 0).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, 0).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, 0).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, 0).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, 0).set_feature("TownWall")
		_level.get_grid((height / 2) - 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, 0).set_feature("TownWall")
		_level.get_grid((height / 2) - 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height / 2, 0).set_feature("PathBorderEW")
		_level.get_grid(height / 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, 0).set_feature("TownWall")
		_level.get_grid((height / 2) + 1, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, 0).set_feature("TownWall")
		_level.get_grid((height / 2) + 2, 0).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, 1).set_feature("TownWall")
		_level.get_grid((height / 2) - 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, 1).set_feature("TownWall")
		_level.get_grid((height / 2) - 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height / 2, 1).set_feature("PathBase")
		_level.get_grid(height / 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, 1).set_feature("TownWall")
		_level.get_grid((height / 2) + 1, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, 1).set_feature("TownWall")
		_level.get_grid((height / 2) + 2, 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
	if island.get_region(wild_row, wild_col + 1).town != null:
		for row: int in height:
			_level.get_grid(row, width - 1).set_feature("TownWall")
			_level.get_grid(row, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, width - 1).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) - 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, width - 2).set_background_feature("InsideGatehouse")
		_level.get_grid((height / 2) + 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, width - 1).set_feature("TownWall")
		_level.get_grid((height / 2) - 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, width - 1).set_feature("TownWall")
		_level.get_grid((height / 2) - 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height / 2, width - 1).set_feature("PathBorderEW")
		_level.get_grid(height / 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, width - 1).set_feature("TownWall")
		_level.get_grid((height / 2) + 1, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, width - 1).set_feature("TownWall")
		_level.get_grid((height / 2) + 2, width - 1).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 2, width - 2).set_feature("TownWall")
		_level.get_grid((height / 2) - 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) - 1, width - 2).set_feature("TownWall")
		_level.get_grid((height / 2) - 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid(height / 2, width - 2).set_feature("PathBase")
		_level.get_grid(height / 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 1, width - 2).set_feature("TownWall")
		_level.get_grid((height / 2) + 1, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
		_level.get_grid((height / 2) + 2, width - 2).set_feature("TownWall")
		_level.get_grid((height / 2) + 2, width - 2).tile_flags.set_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)


func make_wilderness_features(wild_row: int, wild_col: int) -> Location:
	var stair_location: Location = Location.new()
	stair_location.row = _level.cur_hgt / 2
	stair_location.col = _level.cur_wid / 2
	if wild_col == 1 or wild_col == 10 or wild_row == 1 or wild_row == 10:
		return stair_location
	var dungeon_row: int = 0
	var dungeon_col: int = 0
	match Rng.roll_die(4):
		1:
			dungeon_row = 0
			dungeon_col = 0
		2:
			dungeon_row = 0
			dungeon_col = _level.cur_wid / 2
		3:
			dungeon_row = _level.cur_hgt / 2
			dungeon_col = 0
		4:
			dungeon_row = _level.cur_hgt / 2
			dungeon_col = _level.cur_wid / 2
	for offset_col: int in range(0, _level.cur_wid - 1, _level.cur_wid / 2):
		for offset_row: int in range(0, _level.cur_hgt - 1, _level.cur_hgt / 2):
			if offset_col == dungeon_col and offset_row == dungeon_row:
				if _savegame.island.get_region(wild_row, wild_col).dungeon != null:
					if _savegame.island.get_region(wild_row, wild_col).dungeon.is_tower:
						stair_location = make_tower(offset_row + 4, offset_col + 4, (_level.cur_hgt / 2) - 8, (_level.cur_wid / 2) - 8)
					else:
						stair_location = make_dungeon_entrance(offset_row + 4, offset_col + 4, (_level.cur_hgt / 2) - 8, (_level.cur_wid / 2) - 8)
			else:
				match Rng.roll_die(30):
					7, 22:
						make_lake(offset_row + 4, offset_col + 4, (_level.cur_hgt / 2) - 8, (_level.cur_wid / 2) - 8)
					15:
						make_henge(offset_row + 4, offset_col + 4, (_level.cur_hgt / 2) - 8, (_level.cur_wid / 2) - 8)
	return stair_location


func make_dungeon_entrance(top: int, left: int, height: int, width: int) -> Location:
	var i: int = 0
	var col: int
	var row: int
	var location: Location = Location.new()
	while i < SAFE_MAX_ATTEMPTS:
		i += 1
		row = Rng.random_between(top, top + height)
		col = Rng.random_between(left, left + width)
		if _level.grid_open_no_item_or_creature(row, col):
			break
	_level.get_grid(row - 2, col).revert_to_background()
	_level.get_grid(row - 1, col - 1).revert_to_background()
	_level.get_grid(row - 1, col).revert_to_background()
	_level.get_grid(row - 1, col + 1).revert_to_background()
	_level.get_grid(row, col - 2).revert_to_background()
	_level.get_grid(row, col - 1).revert_to_background()
	_level.get_grid(row, col).set_feature("DownStair")
	location.col = col
	location.row = row
	_level.get_grid(row, col + 1).revert_to_background()
	_level.get_grid(row, col + 2).revert_to_background()
	_level.get_grid(row + 1, col - 1).revert_to_background()
	_level.get_grid(row + 1, col).revert_to_background()
	_level.get_grid(row + 1, col + 1).revert_to_background()
	_level.get_grid(row + 2, col).revert_to_background()
	return location


func make_henge(top: int, left: int, height: int, width: int) -> void:
	var mid_col: int = left + (width / 2)
	var mid_row: int = top + (height / 2)
	for row: int in range(mid_row - 3, mid_row + 3):
		_level.get_grid(row, mid_col - 7).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 7).set_feature("Grass")
	for row: int in range(mid_row - 5, mid_row + 5):
		_level.get_grid(row, mid_col - 6).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 6).set_feature("Grass")
	for row: int in range(mid_row - 6, mid_row + 6):
		_level.get_grid(row, mid_col - 5).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 5).set_feature("Grass")
	for row: int in range(mid_row - 6, mid_row + 6):
		_level.get_grid(row, mid_col - 4).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 4).set_feature("Grass")
	for row: int in range(mid_row - 7, mid_row + 6):
		_level.get_grid(row, mid_col - 3).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 3).set_feature("Grass")
	for row: int in range(mid_row - 7, mid_row + 6):
		_level.get_grid(row, mid_col - 2).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 2).set_feature("Grass")
	for row: int in range(mid_row - 6, mid_row + 6):
		_level.get_grid(row, mid_col - 1).set_background_feature("Grass")
		_level.get_grid(row, mid_col - 1).set_feature("Grass")
	for row: int in range(mid_row - 7, mid_row + 6):
		_level.get_grid(row, mid_col).set_background_feature("Grass")
		_level.get_grid(row, mid_col).set_feature("Grass")
	for row: int in range(mid_row - 7, mid_row + 6):
		_level.get_grid(row, mid_col + 1).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 1).set_feature("Grass")
	for row: int in range(mid_row - 6, mid_row + 6):
		_level.get_grid(row, mid_col + 2).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 2).set_feature("Grass")
	for row: int in range(mid_row - 7, mid_row + 6):
		_level.get_grid(row, mid_col + 3).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 3).set_feature("Grass")
	for row: int in range(mid_row - 6, mid_row + 6):
		_level.get_grid(row, mid_col + 4).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 4).set_feature("Grass")
	for row: int in range(mid_row - 5, mid_row + 5):
		_level.get_grid(row, mid_col + 5).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 5).set_feature("Grass")
	for row: int in range(mid_row - 3, mid_row + 3):
		_level.get_grid(row, mid_col + 6).set_background_feature("Grass")
		_level.get_grid(row, mid_col + 6).set_feature("Grass")
	_level.get_grid(mid_row - 6, mid_col).set_feature("Rock")
	_level.get_grid(mid_row - 6, mid_col - 1).set_feature("Rock")
	_level.get_grid(mid_row - 5, mid_col - 4).set_feature("Rock")
	_level.get_grid(mid_row - 4, mid_col - 5).set_feature("Rock")
	_level.get_grid(mid_row - 1, mid_col - 6).set_feature("Rock")
	_level.get_grid(mid_row, mid_col - 6).set_feature("Rock")
	_level.get_grid(mid_row + 3, mid_col - 5).set_feature("Rock")
	_level.get_grid(mid_row + 4, mid_col - 4).set_feature("Rock")
	_level.get_grid(mid_row + 5, mid_col - 1).set_feature("Rock")
	_level.get_grid(mid_row + 5, mid_col).set_feature("Rock")
	_level.get_grid(mid_row + 4, mid_col + 3).set_feature("Rock")
	_level.get_grid(mid_row + 3, mid_col + 4).set_feature("Rock")
	_level.get_grid(mid_row, mid_col + 5).set_feature("Rock")
	_level.get_grid(mid_row - 1, mid_col + 5).set_feature("Rock")
	_level.get_grid(mid_row - 4, mid_col + 4).set_feature("Rock")
	_level.get_grid(mid_row - 5, mid_col + 3).set_feature("Rock")


func make_lake(min_row: int, min_col: int, height: int, width: int) -> void:
	var perlin_noise: PerlinNoise = PerlinNoise.new()
	perlin_noise.randomise(Rng.big_number())
	var width_divisor: float = 1.0 / (width as float)
	var height_divisor: float = 1.0 / (height as float)
	for row: int in height:
		for col: int in width:
			var tile: GridTile = _level.get_grid(min_row + row, min_col + col)
			var v: float = perlin_noise.noise(10 * col * width_divisor, 10 * row * height_divisor, -0.5)
			v = (v + 1.0) / 2.0
			var dcol: float = abs(col - (width / 2.0)) * width_divisor
			var drow: float = abs(row - (height / 2.0)) * height_divisor
			var d: float = max(dcol, drow)
			const elevation: float = 0.05
			const steepness: float = 6.0
			const dropoff: float = 50.0
			v += elevation - (dropoff * pow(d, steepness))
			v = min(1.0, max(0.0, v))
			var rounded: int = (v * 10) as int
			if rounded > 3:
				tile.set_background_feature("Water")
				tile.set_feature("Water")
			elif rounded == 3:
				tile.set_background_feature("Grass")
				tile.set_feature("Grass")


func make_tower(top: int, left: int, height: int, width: int) -> Location:
	var row: int = top + height
	var col: int = left + (width / 2)
	var location: Location = Location.new()
	location.row = row
	location.col = col
	for i: int in range(-2, 3):
		_level.get_grid(row, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-4, 5):
		_level.get_grid(row - 1, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 1, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-5, 6):
		_level.get_grid(row - 2, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 2, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-6, 7):
		_level.get_grid(row - 3, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 3, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-6, 7):
		_level.get_grid(row - 4, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 4, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-7, 8):
		_level.get_grid(row - 5, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 5, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-7, 8):
		_level.get_grid(row - 6, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 6, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-7, 8):
		_level.get_grid(row - 7, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 7, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-7, 8):
		_level.get_grid(row - 8, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 8, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-7, 8):
		_level.get_grid(row - 9, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 9, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-6, 7):
		_level.get_grid(row - 10, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 10, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-6, 7):
		_level.get_grid(row - 11, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 11, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-5, 6):
		_level.get_grid(row - 12, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 12, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-4, 5):
		_level.get_grid(row - 13, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 13, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-2, 4):
		_level.get_grid(row - 14, col + i).set_feature("WallPermBuilding")
		_level.get_grid(row - 14, col + i).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	_level.get_grid(row, col).set_feature("UpStair")
	_level.get_grid(row, col).tile_flags.set_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
	for i: int in range(-3, 4):
		if _level.get_grid(row + 1, col + i).feature_type.category == FloorTileTypeCategory.TREE:
			_level.get_grid(row + 1, col + i).revert_to_background()
	for i: int in range(-2, 3):
		if _level.get_grid(row + 2, col + i).feature_type.category == FloorTileTypeCategory.TREE:
			_level.get_grid(row + 2, col + i).revert_to_background()
	return location


func underground_gen() -> bool:
	Profile.instance.monster_types.reset_unique_only_guardian_status()
	if SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth):
		Profile.instance.monster_types.types[SaveGame.instance.quests.get_quest_monster()].flags_1.set_bits(MonsterFlags1.GUARDIAN)
	if Rng.roll_die(100) <= 4 and not SaveGame.instance.current_dungeon.is_tower:
		make_cavern_level()
	else:
		make_dungeon_level()
	alloc_stairs("DownStair", Rng.random_between(3, 4), 3)
	alloc_stairs("UpStair", Rng.random_between(1, 2), 3)
	if not new_player_spot():
		return false
	var k: int = SaveGame.instance.total_difficulty() / 3
	if k > 10:
		k = 10
	if k < 2:
		k = 2
	if SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth):
		var monster_index: int = SaveGame.instance.quests.get_quest_monster()
		var quest_index: int = SaveGame.instance.quests.get_quest_number()
		while Profile.instance.monster_types.types[monster_index].cur_num < SaveGame.instance.quests.quests[quest_index].to_kill - SaveGame.instance.quests.quests[quest_index].killed:
			_level.put_quest_monster(monster_index)
	var i: int = DUNGEON_MONSTERS
	if _level.cur_hgt < DungeonLevel.MAX_HGT or _level.cur_wid < DungeonLevel.MAX_WID:
		var small_tester: int = i
		i = i * _level.cur_hgt / DungeonLevel.MAX_HGT
		i = i * _level.cur_wid / DungeonLevel.MAX_WID
		i += 1
		if i > small_tester:
			i = small_tester
	i += Rng.roll_die(8)
	for j: int in i + k:
		_level.dungeon_monsters.spawn_monster(0, true)
	alloc_object(ALLOC_SET_BOTH, ALLOC_TYP_TRAP, Rng.roll_die(k))
	alloc_object(ALLOC_SET_CORR, ALLOC_TYP_RUBBLE, Rng.roll_die(k))
	alloc_object(ALLOC_SET_ROOM, ALLOC_TYP_OBJECT, Rng.random_normal(DUN_AMT_ROOM, 3))
	alloc_object(ALLOC_SET_BOTH, ALLOC_TYP_OBJECT, Rng.random_normal(DUN_AMT_ITEM, 3))
	alloc_object(ALLOC_SET_BOTH, ALLOC_TYP_GOLD, Rng.random_normal(DUN_AMT_GOLD, 3))
	return true


func make_cavern_level() -> void:
	var perlin_noise: PerlinNoise = PerlinNoise.new()
	perlin_noise.randomise(Rng.big_number())
	var width_divisor: float = 1.0 / (_level.cur_wid as float)
	var height_divisor: float = 1.0 / (_level.cur_hgt as float)
	for row: int in _level.cur_hgt:
		for col: int in _level.cur_wid:
			var tile: GridTile = _level.get_grid(row, col)
			var v: float = perlin_noise.noise(10 * col * width_divisor, 10 * row * height_divisor, -0.5)
			v = (v + 1) / 2
			var dX: float = abs(col - (_level.cur_wid / 2)) * width_divisor
			var dY: float = abs(row - (_level.cur_hgt / 2)) * height_divisor
			var d: float = max(dX, dY)
			const elevation: float = 0.05
			const steepness: float = 6.0
			const dropoff: float = 50.0
			v += elevation - (dropoff * pow(d, steepness))
			v = min(1, max(0, v))
			var rounded: int = (v * 10) as int
			if rounded < 2 || rounded > 5:
				tile.set_feature("WallBasic")
			else:
				tile.set_feature("DungeonFloor")
	for i: int in DUN_STR_MAG:
		build_streamer("Magma", DUN_STR_MC)
	for i: int in DUN_STR_QUA:
		build_streamer("Quartz", DUN_STR_QC)
	for col: int in _level.cur_wid:
		_level.get_grid(0, col).set_feature("WallPermSolid")
		_level.get_grid(_level.cur_hgt - 1, col).set_feature("WallPermSolid")
	for row: int in _level.cur_hgt:
		_level.get_grid(row, 0).set_feature("WallPermSolid")
		_level.get_grid(row, _level.cur_wid - 1).set_feature("WallPermSolid")
	if Rng.roll_die(DARK_EMPTY) != 1 or Rng.roll_die(100) > SaveGame.instance.total_difficulty():
		_level.wiz_light()


func alloc_stairs(feat: String, num: int, walls: int) -> void:
	for i: int in num:
		var done: bool = false
		while not done:
			for j: int in 3000:
				var row: int = Rng.roll_die(_level.cur_hgt) - 1
				var col: int = Rng.roll_die(_level.cur_wid) - 1
				if not _level.grid_open_no_item_or_creature(row, col):
					continue
				if next_to_walls(row, col) < walls:
					continue
				var tile: GridTile = _level.get_grid(row, col)
				if SaveGame.instance.current_depth <= 0:
					tile.set_feature("DownStair")
				elif (SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth) or
					SaveGame.instance.current_depth == SaveGame.instance.current_dungeon.max_level ):
					tile.set_feature("DownStair" if SaveGame.instance.current_dungeon.is_tower else "UpStair")
				else:
					tile.set_feature(feat)
				done = true
				break
			if walls != 0:
				walls -= 1


func next_to_walls(row: int, col: int) -> int:
	var k: int = 0
	if _level.get_grid(row + 1, col).feature_type.is_wall:
		k += 1
	if _level.get_grid(row - 1, col).feature_type.is_wall:
		k += 1
	if _level.get_grid(row, col + 1).feature_type.is_wall:
		k += 1
	if _level.get_grid(row, col - 1).feature_type.is_wall:
		k += 1
	return k


func alloc_object(alloc_set: int, typ: int, num: int) -> void:
	var dummy: int = 0
	var row: int
	var col: int
	for k: int in num:
		while dummy < SAFE_MAX_ATTEMPTS:
			dummy += 1
			row = Rng.roll_die(_level.cur_hgt) - 1
			col = Rng.roll_die(_level.cur_wid) - 1
			if not _level.grid_open_no_item_or_creature(row, col):
				continue
			var is_room: bool = _level.get_grid(row, col).tile_flags.is_set(GridTile.IN_ROOM)
			if alloc_set == ALLOC_SET_CORR and is_room:
				continue
			if alloc_set == ALLOC_SET_ROOM and not is_room:
				continue
			break
		if dummy >= SAFE_MAX_ATTEMPTS:
			return
		match typ:
			ALLOC_TYP_RUBBLE:
				place_rubble(row, col)
			ALLOC_TYP_TRAP:
				_level.place_trap(row, col)
			ALLOC_TYP_GOLD:
				_level.place_gold(row, col)
			ALLOC_TYP_OBJECT:
				_level.place_item(row, col, false, false)


func place_rubble(row: int, col: int) -> void:
	_level.get_grid(row, col).set_feature("Rubble")


func make_dungeon_level() -> void:
	var max_vault_ok: int = 2
	var destroyed: bool = false
	var empty_level: bool = false
	_build_info = LevelBuildInfo.new()
	if _level.max_panel_rows == 0:
		max_vault_ok -= 1
	if _level.max_panel_cols == 0:
		max_vault_ok -= 1
	if Rng.roll_die(EMPTY_LEVEL) == 1:
		empty_level = true
	for row: int in _level.cur_hgt:
		for col: int in _level.cur_wid:
			var tile: GridTile = _level.get_grid(row, col)
			if empty_level:
				tile.revert_to_background()
			else:
				tile.set_feature("WallBasic")
	if SaveGame.instance.total_difficulty() > 10 and Rng.roll_die(DUN_DEST) == 1:
		destroyed = true
	if SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth):
		destroyed = false
	_build_info.row_rooms = _level.cur_hgt / BLOCK_HGT
	_build_info.col_rooms = _level.cur_wid / BLOCK_WID
	for row: int in _build_info.row_rooms:
		for col: int in _build_info.col_rooms:
			_build_info.room_map[row][col] = false
	_build_info.crowded = false
	_build_info.cent_num = 0
	for i: int in DUN_ROOMS:
		var row: int = Rng.roll_die(_build_info.row_rooms) - 1
		var col: int = Rng.roll_die(_build_info.col_rooms) - 1
		if col % 3 == 0:
			col += 1
		if col % 3 == 2:
			col -= 1
		if destroyed:
			room_build(row, col, 1)
			continue
		if Rng.roll_die(DUN_UNUSUAL) <= SaveGame.instance.total_difficulty():
			var k: int = Rng.roll_die(100) - 1
			if Rng.roll_die(DUN_UNUSUAL) < SaveGame.instance.total_difficulty():
				if k < 10:
					if max_vault_ok > 1:
						if room_build(row, col, 8):
							continue
				if k < 25:
					if max_vault_ok > 0:
						if room_build(row, col, 7):
							continue
				if k < 40 and room_build(row, col, 5):
					continue
				if k < 55 and room_build(row, col, 6):
					continue
			if k < 25 and room_build(row, col, 4):
				continue
			if k < 50 and room_build(row, col, 3):
				continue
			if k < 100 and room_build(row, col, 2):
				continue
		room_build(row, col, 1)
	for col: int in _level.cur_wid:
		_level.get_grid(0, col).set_feature("WallPermSolid")
		_level.get_grid(_level.cur_hgt - 1, col).set_feature("WallPermSolid")
	for row: int in _level.cur_hgt:
		_level.get_grid(row, 0).set_feature("WallPermSolid")
		_level.get_grid(row, _level.cur_wid - 1).set_feature("WallPermSolid")
	for i: int in _build_info.cent_num:
		var pick_1: int = Rng.roll_die(_build_info.cent_num) - 1
		var pick_2: int = Rng.roll_die(_build_info.cent_num) - 1
		var row_1: int = _build_info.cent[pick_1].row
		var col_1: int = _build_info.cent[pick_1].col
		_build_info.cent[pick_1].row = _build_info.cent[pick_2].row
		_build_info.cent[pick_1].col = _build_info.cent[pick_2].col
		_build_info.cent[pick_2].row = row_1
		_build_info.cent[pick_2].col = col_1
	_build_info.door_num = 0
	var end_row: int = _build_info.cent[_build_info.cent_num - 1].row
	var end_col: int = _build_info.cent[_build_info.cent_num - 1].col
	for i: int in _build_info.cent_num:
		build_tunnel(_build_info.cent[i].row, _build_info.cent[i].col, end_row, end_col)
		end_row = _build_info.cent[i].row
		end_col = _build_info.cent[i].col
	for i: int in _build_info.door_num:
		var row: int = _build_info.door[i].row
		var col: int = _build_info.door[i].col
		try_door(row, col - 1)
		try_door(row, col + 1)
		try_door(row - 1, col)
		try_door(row + 1, col)
	for i: int in DUN_STR_MAG:
		build_streamer("Magma", DUN_STR_MC)
	for i: int in DUN_STR_QUA:
		build_streamer("Quartz", DUN_STR_QC)
	if destroyed:
		destroy_level()
	if empty_level and (Rng.roll_die(DARK_EMPTY) != 1 or Rng.roll_die(100) > SaveGame.instance.total_difficulty()):
		_level.wiz_light()


func room_build(build_row: int, build_col: int, type: int) -> bool:
	var vaultFactory: VaultFactory = VaultFactory.new(_level)
	if SaveGame.instance.total_difficulty() < _rooms[type].min_level:
		return false
	if _build_info.crowded and (type == 5 or type == 6):
		return false
	var row_start: int = build_row + _rooms[type].row_start
	var row_end: int = build_row + _rooms[type].row_end
	var col_start: int = build_col + _rooms[type].col_start
	var col_end: int = build_col + _rooms[type].col_end
	if row_start < 0 or row_end >= _build_info.row_rooms:
		return false
	if col_start < 0 or col_end >= _build_info.col_rooms:
		return false
	for check_row: int in range(row_start, row_end + 1):
		for check_col: int in range(col_start, col_end + 1):
			if _build_info.room_map[check_row][check_col]:
				return false
	var row: int = (row_start + row_end + 1) * BLOCK_HGT / 2
	var col: int = (col_start + col_end + 1) * BLOCK_WID / 2
	match type:
		8:
			vaultFactory.build_type_8(row, col)
		7:
			vaultFactory.build_type_7(row, col)
		6:
			vaultFactory.build_type_6(row, col)
		5:
			vaultFactory.build_type_5(row, col)
		4:
			vaultFactory.build_type_4(row, col)
		3:
			vaultFactory.build_type_3(row, col)
		2:
			vaultFactory.build_type_2(row, col)
		1:
			vaultFactory.build_type_1(row, col)
		_:
			return false
	if _build_info.cent_num < CENT_MAX:
		_build_info.cent[_build_info.cent_num].row = row
		_build_info.cent[_build_info.cent_num].col = col
		_build_info.cent_num += 1
	for check_row: int in range(row_start, row_end + 1):
		for check_col: int in range(col_start, col_end + 1):
			_build_info.room_map[check_row][check_col] = true
	if type == 5 or type == 6:
		_build_info.crowded = true
	return true


func build_streamer(feat: String, chance: int) -> void:
	var tries: int = 0
	var row: int = Rng.random_around(_level.cur_hgt / 2, 10)
	var col: int = Rng.random_around(_level.cur_wid / 2, 15)
	var dir: int = _level.ordered_direction[Rng.roll_die(8) - 1]
	while tries < SAFE_MAX_ATTEMPTS:
		tries += 1
		for i: int in DUN_STR_DEN:
			var to_row: int
			var to_col: int
			while true:
				to_row = Rng.random_around(row, DUN_STR_RNG)
				to_col = Rng.random_around(col, DUN_STR_RNG)
				if not _level.in_inner_bounds(to_row, to_col):
					continue
				break
			var tile: GridTile = _level.get_grid(to_row, to_col)
			if not tile.feature_type.is_basic_wall:
				continue
			tile.set_feature(feat)
			if Rng.roll_die(chance) == 1:
				tile.set_feature(feat + "VisTreas")
		if tries >= SAFE_MAX_ATTEMPTS:
			return
		row += _level.keypad_direction_row_offset[dir]
		col += _level.keypad_direction_col_offset[dir]
		if not _level.in_outer_bounds(row, col):
			break


func build_tunnel(row_1: int, col_1: int, row_2: int, col_2: int) -> void:
	var main_loop_count: int = 0
	var door_flag: bool = false
	_build_info.tunn_num = 0
	_build_info.wall_num = 0
	var start_row: int = row_1
	var start_col: int = col_1
	var dir_as_location: Location = correct_dir(row_1, col_1, row_2, col_2)
	var row_dir: int = dir_as_location.row
	var col_dir: int = dir_as_location.col
	while row_1 != row_2 or col_1 != col_2:
		main_loop_count += 1
		if main_loop_count > 2000:
			break
		if Rng.roll_die(100) <= DUN_TUN_CHG:
			dir_as_location = correct_dir(row_1, col_1, row_2, col_2)
			row_dir = dir_as_location.row
			col_dir = dir_as_location.col
			if Rng.roll_die(100) < DUN_TUN_RND:
				dir_as_location = random_dir()
				row_dir = dir_as_location.row
				col_dir = dir_as_location.col
		var temp_row: int = row_1 + row_dir
		var temp_col: int = col_1 + col_dir
		while not _level.in_inner_bounds(temp_row, temp_col):
			dir_as_location = correct_dir(row_1, col_1, row_2, col_2)
			row_dir = dir_as_location.row
			col_dir = dir_as_location.col
			if Rng.roll_die(100) < DUN_TUN_RND:
				dir_as_location = random_dir()
				row_dir = dir_as_location.row
				col_dir = dir_as_location.col
			temp_row = row_1 + row_dir
			temp_col = col_1 + col_dir
		var tile: GridTile = _level.get_grid(temp_row, temp_col)
		if tile.feature_type.name == "WallPermSolid":
			continue
		if tile.feature_type.name == "WallPermOuter":
			continue
		if tile.feature_type.name == "WallSolid":
			continue
		if tile.feature_type.name == "WallOuter":
			var test_row: int = temp_row + row_dir
			var test_col: int = temp_col + col_dir
			if _level.get_grid(test_row, test_col).feature_type.name == "WallPermSolid":
				continue
			if _level.get_grid(test_row, test_col).feature_type.name == "WallPermOuter":
				continue
			if _level.get_grid(test_row, test_col).feature_type.name == "WallOuter":
				continue
			if _level.get_grid(test_row, test_col).feature_type.name == "WallSolid":
				continue
			row_1 = temp_row
			col_1 = temp_col
			if _build_info.wall_num < WALL_MAX:
				_build_info.wall[_build_info.wall_num].row = row_1
				_build_info.wall[_build_info.wall_num].col = col_1
				_build_info.wall_num += 1
			for row: int in range(row_1 - 1, row_1 + 2):
				for col: int in range(col_1 - 1, col_1 + 2):
					if _level.get_grid(row, col).feature_type.name == "WallOuter":
						_level.get_grid(row, col).set_feature("WallSolid")
		elif tile.tile_flags.is_set(GridTile.IN_ROOM):
			row_1 = temp_row
			col_1 = temp_col
		elif tile.feature_type.is_wall:
			row_1 = temp_row
			col_1 = temp_col
			if _build_info.tunn_num < TUNN_MAX:
				_build_info.tunn[_build_info.tunn_num].row = row_1
				_build_info.tunn[_build_info.tunn_num].col = col_1
				_build_info.tunn_num += 1
			door_flag = false
		else:
			row_1 = temp_row
			col_1 = temp_col
			if not door_flag:
				if _build_info.door_num < DOOR_MAX:
					_build_info.door[_build_info.door_num].row = row_1
					_build_info.door[_build_info.door_num].col = col_1
					_build_info.door_num += 1
				door_flag = true
			if Rng.roll_die(100) > DUN_TUN_CON:
				temp_row = row_1 - start_row
				if temp_row < 0:
					temp_row = -temp_row
				temp_col = col_1 - start_col
				if temp_col < 0:
					temp_col = -temp_col
				if temp_row > 10 or temp_col > 10:
					break
	for i: int in _build_info.tunn_num:
		var row: int = _build_info.tunn[i].row
		var col: int = _build_info.tunn[i].col
		var tile: GridTile = _level.get_grid(row, col)
		tile.revert_to_background()
	for i: int in _build_info.wall_num:
		var row: int = _build_info.wall[i].row
		var col: int = _build_info.wall[i].col
		var tile: GridTile = _level.get_grid(row, col)
		tile.revert_to_background()
		if Rng.roll_die(100) <= DUN_TUN_PEN:
			place_random_door(row, col)


func try_door(row: int, col: int) -> void:
	if not _level.in_inner_bounds(row, col):
		return
	if _level.get_grid(row, col).feature_type.is_wall:
		return
	if _level.get_grid(row, col).tile_flags.is_set(GridTile.IN_ROOM):
		return
	if Rng.roll_die(100) < DUN_TUN_JCT and possible_doorway(row, col):
		place_random_door(row, col)


func destroy_level() -> void:
	var row_1: int = Rng.random_between(5, _level.cur_hgt - 1 - 5)
	var col_1: int = Rng.random_between(5, _level.cur_wid - 1 - 5)
	for row: int in range(row_1 - 15, row_1 + 16):
		for col: int in range(col_1 - 15, col_1 + 16):
			if not _level.in_inner_bounds(row, col):
				continue
			var k: int = _level.distance(row_1, col_1, row, col)
			if k >= 16:
				continue
			_level.delete_monster_at(row, col)
			if _level.cave_valid_bold(row, col):
				_level.delete_item_at(row, col)
				var tile: GridTile = _level.get_grid(row, col)
				var t: int = Rng.roll_die(20)
				if t <= 2:
					tile.set_feature("WallBasic")
				elif t <= 7:
					tile.set_feature("Quartz")
				elif t <= 10:
					tile.set_feature("Magma")
				else:
					tile.revert_to_background()
				tile.tile_flags.clear_bits(GridTile.IN_ROOM | GridTile.IN_VAULT)
				tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)


func correct_dir(row_1: int, col_1: int, row_2: int, col_2: int) -> Location:
	var location: Location = Location.new()
	location.row = signi(row_2 - row_1)
	location.col = signi(col_2 - col_1)
	if location.row != 0 and location.col != 0:
		if Rng.roll_die(100) <= 50:
			location.row = 0
		else:
			location.col = 0
	return location


func random_dir() -> Location:
	var roll: int = Rng.roll_die(4) - 1
	var location: Location = Location.new()
	location.row = _level.ordered_direction_row_offset[roll]
	location.col = _level.ordered_direction_col_offset[roll]
	return location


func possible_doorway(row: int, col: int) -> bool:
	if next_to_corridor(row, col) >= 2:
		if _level.get_grid(row - 1, col).feature_type.is_wall and _level.get_grid(row + 1, col).feature_type.is_wall:
			return true
		if _level.get_grid(row, col - 1).feature_type.is_wall and _level.get_grid(row, col + 1).feature_type.is_wall:
			return true
	return false


func next_to_corridor(row: int, col: int) -> int:
	var k: int = 0
	for i: int in 4:
		var test_row: int = row + _level.ordered_direction_row_offset[i]
		var test_col: int = col + _level.ordered_direction_col_offset[i]
		if not _level.grid_passable(test_row, test_col):
			continue
		var tile: GridTile = _level.get_grid(test_row, test_col)
		if tile.feature_type.is_open_floor:
			continue
		if tile.tile_flags.is_set(GridTile.IN_ROOM):
			continue
		k += 1
	return k
