# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellHerbalHealing
extends Spell


func _init() -> void:
	default_base_failure = 95
	default_level = 40
	default_power_cost = 100
	first_cast_experience = 50
	name = "Herbal Healing"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.restore_health(1000)
	player.set_timed_stun(0)
	player.set_timed_bleeding(0)
	player.set_timed_poison(0)


func comment(player: Player) -> String:
	return "heal 1000"
