# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ChaosSpellMeteorSwarm
extends Spell


func _init() -> void:
	default_base_failure = 85
	default_level = 34
	default_power_cost = 32
	first_cast_experience = 35
	name = "Meteor Swarm"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var col: int = player.map_col
	var row: int = player.map_row
	var count: int = 0
	var b: int = 10 + Rng.roll_die(10)
	for i: int in b:
		var d: int = 99
		while d > 5 or not dungeon_level.player_has_los_bold(row, col):
			count += 1
			if count > 1000:
				break
			col = player.map_col - 5 + Rng.roll_die(10)
			row = player.map_row - 5 + Rng.roll_die(10)
			var dx: int = player.map_col - col if player.map_col > col else col - player.map_col
			var dy: int = player.map_row - row if player.map_row > row else row - player.map_row
			d = dy + (dx >> 1) if dy > dx else dx + (dy >> 1)
		if count > 1000:
			break
		count = 0
		savegame.spell_effects_handler.project(0, 2, row, col, player.level * 3 / 2, ProjectileMeteor.new(savegame.spell_effects_handler), ProjectionFlag.KILL | ProjectionFlag.JUMP | ProjectionFlag.ITEM)


func comment(player: Player) -> String:
	return "dam %d each" % [3 * player.level / 2]
