# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SorcerySpellTeleport
extends Spell


func _init() -> void:
	default_base_failure = 35
	default_level = 5
	default_power_cost = 5
	first_cast_experience = 5
	name = "Teleport"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	savegame.spell_effects_handler.teleport_player(player.level * 5)


func comment(player: Player) -> String:
	return "range %d" % [player.level * 5]
