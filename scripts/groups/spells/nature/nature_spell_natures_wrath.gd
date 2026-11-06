# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellNaturesWrath
extends Spell


func _init() -> void:
	default_base_failure = 95
	default_level = 45
	default_power_cost = 90
	first_cast_experience = 200
	name = "Nature's Wrath"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	savegame.spell_effects_handler.dispel_monsters(player.level * 4)
	savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 100 + player.level)
	savegame.spell_effects_handler.project(0, player.level / 12 + 1, player.map_row, player.map_col, 100 + player.level, ProjectileDisintegrate.new(savegame.spell_effects_handler), ProjectionFlag.KILL | ProjectionFlag.ITEM)


func comment(player: Player) -> String:
	return "dam %d+%d" % [player.level * 4, 100 + player.level]
