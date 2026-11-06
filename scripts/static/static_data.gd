# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name StaticData extends Object

# Some data is read-only and is used directly from here
static var animations: Dictionary[String, AsciiAnimation]
static var projectiles: Dictionary[String, Flavour]
static var tile_types: Dictionary[String, FloorTileType]
static var vault_types: Array[VaultType]
static var rare_item_types: Dictionary[int, RareItemType]

# Artifacts and items are copied to the savegame and modified there
static var artifact_types: Array[Artifact]
static var item_types: Array[ItemType]

# Item flavours are copied into the save-game and randomised there
static var amulet_flavours: Dictionary[String, Flavour]
static var mushroom_flavours: Dictionary[String, Flavour]
static var potion_flavours: Dictionary[String, Flavour]
static var ring_flavours: Dictionary[String, Flavour]
static var rod_flavours: Dictionary[String, Flavour]
static var scroll_flavours: Dictionary[String, Flavour]
static var staff_flavours: Dictionary[String, Flavour]
static var wand_flavours: Dictionary[String, Flavour]

# Monsters are copied into the profile for meta-progression modification
static var monster_types: Array[MonsterType]


static func _static_init() -> void:
	amulet_flavours = _load_flavours("amulets")
	_load_animations()
	_load_artifact_types()
	_load_item_types()
	_load_monster_types()
	mushroom_flavours = _load_flavours("mushrooms")
	potion_flavours = _load_flavours("potions")
	projectiles = _load_flavours("projectiles")
	_load_rare_item_types()
	ring_flavours = _load_flavours("rings")
	rod_flavours = _load_flavours("rods")
	scroll_flavours = _load_flavours("scrolls")
	staff_flavours = _load_flavours("staffs")
	_load_tile_types()
	_load_vault_types()
	wand_flavours = _load_flavours("wands")


static func _load_flavours(folder_name: String)->Dictionary[String, Flavour]:
	var dir: DirAccess = DirAccess.open("res://data/" + folder_name)
	var dictionary: Dictionary[String, Flavour] = {}
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var flavour: Flavour = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		dictionary.get_or_add(flavour.name, flavour)
	return dictionary


static func _load_animations() -> void:
	var dir: DirAccess = DirAccess.open("res://data/animations")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var animation: AsciiAnimation = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		animations.get_or_add(animation.name, animation)


static func _load_artifact_types() -> void:
	var dir: DirAccess = DirAccess.open("res://data/artifacts")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var artifact: Artifact = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		artifact_types.append(artifact)


static func _load_item_types() -> void:
	var dir: DirAccess = DirAccess.open("res://data/items")
	dir.list_dir_begin()
	var dict: Dictionary[int, ItemType]
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var item_type: ItemType = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		dict.get_or_add(item_type.item_category * 1000 + item_type.sub_category, item_type)
	var keys: Array[int] = dict.keys()
	keys.sort()
	for key: int in keys:
		item_types.append(dict[key])

static func _load_monster_types()->void:
	var dir: DirAccess = DirAccess.open("res://data/monsters")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var monster_type: MonsterType= ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		monster_types.append(monster_type)


static func _load_rare_item_types() -> void:
	var dir: DirAccess = DirAccess.open("res://data/rares")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var rare_item_type: RareItemType = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		rare_item_types.get_or_add(rare_item_type.id, rare_item_type)


static func _load_tile_types() -> void:
	var dir: DirAccess = DirAccess.open("res://data/tiles")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var tile_type: FloorTileType = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		tile_types.get_or_add(tile_type.name, tile_type)


static func _load_vault_types() -> void:
	var dir: DirAccess = DirAccess.open("res://data/vaults")
	dir.list_dir_begin()
	for file_name: String in dir.get_files():
		if '.tres.remap' in file_name:
				file_name = file_name.trim_suffix('.remap')
		var vault_type: VaultType = ResourceLoader.load(dir.get_current_dir() + "/" + file_name)
		vault_types.append(vault_type)
