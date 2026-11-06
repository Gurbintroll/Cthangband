# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CorporealSpellHealing
extends Spell


func _init() -> void:
	default_base_failure = 70
	default_level = 28
	default_power_cost = 20
	first_cast_experience = 15
	name = "Healing"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.restore_health(300)
	player.set_timed_stun(0)
	player.set_timed_bleeding(0)


func comment(player: Player) -> String:
	return "heal 300"
