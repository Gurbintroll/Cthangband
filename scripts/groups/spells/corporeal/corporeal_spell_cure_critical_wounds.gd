# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CorporealSpellCureCriticalWounds
extends Spell


func _init() -> void:
	default_base_failure = 75
	default_level = 11
	default_power_cost = 7
	first_cast_experience = 7
	name = "Cure Critical Wounds"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.restore_health(Rng.roll_dice(8, 10))
	player.set_timed_stun(0)
	player.set_timed_bleeding(0)


func comment(player: Player) -> String:
	return "heal 8d10"
