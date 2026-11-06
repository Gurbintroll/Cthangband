# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Artifact
extends Resource

@export var flags_1: FlagSet
@export var flags_2: FlagSet
@export var flags_3: FlagSet
@export var ac: int
@export var cost: int
@export var current_number: int
@export var damage_dice: int
@export var damage_dice_sides: int
@export var has_own_type: bool
@export var item_category: int
@export var item_sub_category: int
@export var level: int
@export var name: String
@export var type_specific_value: int
@export var rarity: int
@export var to_ac: int
@export var to_hit: int
@export var to_damage: int
@export var weight: int
@export var id: int


func get_copy_for_new_savegame() -> Artifact:
	var new_artifact: Artifact = Artifact.new()
	new_artifact.flags_1 = FlagSet.new()
	new_artifact.flags_1.copy(flags_1)
	new_artifact.flags_2 = FlagSet.new()
	new_artifact.flags_2.copy(flags_2)
	new_artifact.flags_3 = FlagSet.new()
	new_artifact.flags_3.copy(flags_3)
	new_artifact.ac = ac
	new_artifact.cost = cost
	new_artifact.current_number = 0
	new_artifact.damage_dice = damage_dice
	new_artifact.damage_dice_sides = damage_dice_sides
	new_artifact.has_own_type = has_own_type
	new_artifact.item_category = item_category
	new_artifact.item_sub_category = item_sub_category
	new_artifact.level = level
	new_artifact.name = name
	new_artifact.type_specific_value = type_specific_value
	new_artifact.rarity = rarity
	new_artifact.to_ac = to_ac
	new_artifact.to_damage = to_damage
	new_artifact.to_hit = to_hit
	new_artifact.weight = weight
	new_artifact.id = id
	return new_artifact
