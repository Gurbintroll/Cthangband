# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Dungeon
extends Resource

@export var base_offset: int
@export var bias: int
@export var first_guardian: String
@export var first_guardian_level: int
@export var map_symbol: String
@export var max_level: int
@export var name: String
@export var second_guardian: String
@export var second_guardian_level: int
@export var short_name: String
@export var is_tower: bool
@export var index: int
@export var known_depth: bool
@export var known_offset: bool
@export var offset: int
@export var visited: bool
@export var wild_row: int
@export var wild_col: int


static func new_dungeon(new_tower: bool, new_offset: int, new_max_level: int, new_bias: int, first_guardian_name: String, second_guardian_name: String, first_level: int, second_level: int, new_name: String, new_short_name: String, symbol: String) -> Dungeon:
	var dungeon: Dungeon = Dungeon.new()
	dungeon.wild_row = 0
	dungeon.wild_col = 0
	dungeon.is_tower = new_tower
	dungeon.offset = new_offset
	dungeon.base_offset = new_offset
	dungeon.max_level = new_max_level
	dungeon.bias = new_bias
	dungeon.first_guardian = first_guardian_name
	dungeon.first_guardian_level = first_level
	dungeon.second_guardian = second_guardian_name
	dungeon.second_guardian_level = second_level
	dungeon.name = new_name
	dungeon.short_name = new_short_name
	dungeon.map_symbol = symbol
	dungeon.visited = false
	dungeon.known_depth = false
	dungeon.known_offset = false
	dungeon._randomise_offset()
	return dungeon


static func new_dungeon_list() -> Array[Dungeon]:
	var dungeons: Array[Dungeon] = [
		# Note that the indices of the first eight dungeons match the towns that they are under
		new_dungeon(false, 0, 3, 0, "", "", 0, 0, "the sewers under Celephais", "Celephais", "C"),
		new_dungeon(false, 1, 9, 0, "", "", 0, 0, "the sewers under Dylath Leen", "Dylath-Leen", "D"),
		new_dungeon(false, 0, 5, 0, "", "", 0, 0, "the sewers under Hlanith", "Hlanith", "H"),
		new_dungeon(false, 0, 5, 0, "", "", 0, 0, "the sewers under Inganok", "Inganok", "I"),
		new_dungeon(false, 50, 75, SummonType.SUMMON_CTHULOID, "Nyarlathotep", "Azathoth, The Daemon Sultan", 49, 50, "the catacombs under Kadath", "Kadath", "K"),
		new_dungeon(false, 0, 7, SummonType.SUMMON_HUMAN, "Robin Hood, the Outlaw", "", 7, 0, "the sewers under Nir", "Nir", "N"),
		new_dungeon(false, 0, 7, SummonType.SUMMON_ANIMAL, "Hobbes the Tiger", "", 7, 0, "the sewers under Ulthar", "Ulthar", "U"),
		new_dungeon(false, 0, 5, 0, "", "", 0, 0, "the sewers under Ilek-Vad", "Ilek-Vad", "V"),
		# The remaining twelve dungeons are in the wilderness
		new_dungeon(false, 30, 20, 0, "The Collector", "", 20, 0, "the Collector's Cave", "Cave", "c"),
		new_dungeon(true, 15, 20, SummonType.SUMMON_DEMON, "The Emissary", "Glaryssa, Succubus Queen", 1, 20, "the Demon Spire", "Spire", "d"),
		new_dungeon(true, 20, 20, SummonType.SUMMON_ELEMENTAL, "Lasha, Mistress of Water", "Grom, Master of Earth", 15, 20, "the Conflux of the Elements", "Conflux", "e"),
		new_dungeon(true, 1, 5, SummonType.SUMMON_KOBOLD, "Vort, the Kobold Queen", "", 5, 0, "the Kobold Fort", "Fort", "f"),
		new_dungeon(true, 40, 20, SummonType.SUMMON_CTHULOID, "Father Dagon", "Tulzscha", 1, 20, "the Tower of Koth", "Koth", "k"),
		new_dungeon(false, 15, 35, SummonType.SUMMON_DRAGON, "Glaurung, Father of the Dragons", "Ancalagon the Black", 34, 35, "the Dragon's Lair", "Dragon Lair", "l"),
		new_dungeon(true, 30, 40, SummonType.SUMMON_HI_UNDEAD, "Fire Phantom", "Vecna, the Emperor Lich", 1, 40, "the Necropolis", "Necropolis", "n"),
		new_dungeon(true, 3, 17, SummonType.SUMMON_ORC, "Bolg, Son of Azog", "Azog, King of the Uruk-Hai", 16, 17, "the Orc Tower", "Orc Tower", "o"),
		new_dungeon(true, 13, 17, SummonType.SUMMON_SPIDER, "Shelob, Spider of Darkness", "", 17, 0, "Shelob's Tower", "Tower", "s"),
		new_dungeon(false, 4, 21, SummonType.SUMMON_UNDEAD, "The disembodied hand", "Khufu the mummified King", 1, 21, "Khufu's Tomb", "Tomb", "t"),
		new_dungeon(false, 10, 30, 0, "The Stormbringer", "", 30, 0, "the Vault of the Sword", "Vault", "v"),
		new_dungeon(false, 2, 8, SummonType.SUMMON_YEEK, "Orfax, Son of Boldor", "Boldor, King of the Yeeks", 7, 8, "the Yeek King's Lair", "Yeek Lair", "y"),
	]
	var player: Player = SaveGame.instance.player
	player.max_dlv = []
	for i: int in dungeons.size():
		dungeons[i].index = i
		player.max_dlv.append(0)
	return dungeons


func _randomise_offset() -> void:
	# Maximum offset is the dundgeon's base offset of 10, whichever is less
	# But since we'll be subtracting one, we add one to the possible range
	var level_range: int = min(base_offset + 1, 11)
	var change: int = Rng.roll_die(level_range) - 1
	# Dungeons are equally likely to be more or less difficult
	if Rng.roll_die(6) >= 4:
		offset = base_offset + change
	else:
		offset = base_offset - change
