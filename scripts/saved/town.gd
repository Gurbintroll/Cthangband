# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Town
extends Resource

@export var character: String
@export var house_price: int
@export var index: int
@export var name: String
@export var random_seed: int
@export var stores: Array[Store]
@export var visited: bool
@export var wild_row: int
@export var wild_col: int


static func _new_town(new_stores: Array[Store], new_house_price: int, new_name: String, new_character: String) -> Town:
	var town: Town = Town.new()
	town.character = new_character
	town.house_price = new_house_price
	town.index = 0
	town.name = new_name
	town.random_seed = 0
	town.stores = new_stores
	town.visited = false
	town.wild_row = 0
	town.wild_col = 0
	return town


static func new_town_list() -> Array[Town]:
	var array: Array[Town] = [
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.TEMPLE),
				Store.new_store(StoreType.TEMPLE),
				Store.new_store(StoreType.ALCHEMIST),
				Store.new_store(StoreType.MAGIC_STORE),
				Store.new_store(StoreType.HOME),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.HALL_OF_RECORDS),
				Store.new_store(StoreType.PAWNBROKERS),
			],
			50000,
			"the beautiful city of Celephais",
			"C",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.HOME),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.HALL_OF_RECORDS),
				Store.new_store(StoreType.PAWNBROKERS),
			],
			25000,
			"the unwholesome city of Dylath-Leen",
			"D",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.ALCHEMIST),
				Store.new_store(StoreType.MAGIC_STORE),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.HOME),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.HALL_OF_RECORDS),
			],
			45000,
			"the market town of Hlanith",
			"H",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.TEMPLE),
				Store.new_store(StoreType.ALCHEMIST),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.MAGIC_STORE),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.PAWNBROKERS),
			],
			0,
			"the industrious town of Inganok",
			"I",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
			],
			0,
			"Kadath, home of the Gods",
			"K",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.PAWNBROKERS),
			],
			50000,
			"the hamlet of Nir",
			"N",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.TEMPLE),
				Store.new_store(StoreType.ALCHEMIST),
				Store.new_store(StoreType.MAGIC_STORE),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.HOME),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.HALL_OF_RECORDS),
				Store.new_store(StoreType.PAWNBROKERS),
			],
			45000,
			"the picturesque town of Ulthar",
			"U",
		),
		Town._new_town(
			[
				Store.new_store(StoreType.GENERAL_STORE),
				Store.new_store(StoreType.ARMOURY),
				Store.new_store(StoreType.WEAPONSMITH),
				Store.new_store(StoreType.TEMPLE),
				Store.new_store(StoreType.ALCHEMIST),
				Store.new_store(StoreType.MAGIC_STORE),
				Store.new_store(StoreType.BLACK_MARKET),
				Store.new_store(StoreType.HOME),
				Store.new_store(StoreType.BOOKSTORE),
				Store.new_store(StoreType.EMPTY_LOT),
				Store.new_store(StoreType.INN),
				Store.new_store(StoreType.HALL_OF_RECORDS),
			],
			60000,
			"the city of Ilek-Vad",
			"V",
		),
	]
	for i: int in array.size():
		array[i].index = i
	return array
