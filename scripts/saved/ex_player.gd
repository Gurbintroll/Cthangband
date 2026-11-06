# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ExPlayer
extends Resource
## A record of a dead player that can be shown in the Load/New Game screen

## The ancestry in which the character was born
@export var birth_ancestry: String
## The character's current ancestry (they might have been polymorphed or mutated)
@export var current_ancestry: String
## The character's profession
@export var current_profession: String
## The character's gender
@export var gender: int
## The character's generation, to be appended to their name
@export var generation: int
## The level at which the character died
@export var level: int
## The character's name
@export var name: String
## The character's first realm of magic (if any)
@export var realm_1: int
## The character#s second realm of magic (if any)
@export var realm_2: int


func copy_player(player: Player) -> void:
	birth_ancestry = player.birth_ancestry
	current_ancestry = player.current_ancestry
	current_profession = player.current_profession
	gender = player.gender_index
	generation = player.generation
	level = player.level
	name = player.name
	realm_1 = player.realm_1
	realm_2 = player.realm_2
