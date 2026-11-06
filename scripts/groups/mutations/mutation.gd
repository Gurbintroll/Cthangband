# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Mutation
extends Resource

@export var attack_description: String
@export var damage_dice_number: int
@export var damage_dice_size: int
@export var equivalent_weapon_weight: int
@export var frequency: int
@export var gain_message: String
@export var group: int
@export var have_message: String
@export var lose_message: String
@export var mutation_attack_type: int


func initialise() -> void:
	pass


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	pass


func activation_summary(level: int) -> String:
	return ""


func on_gain(genome: Genome) -> void:
	pass


func on_lose(genome: Genome) -> void:
	pass


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	pass
