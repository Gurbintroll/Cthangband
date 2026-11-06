# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellSummonHounds
extends Spell


func _init() -> void:
	default_base_failure = 70
	default_level = 29
	default_power_cost = 30
	first_cast_experience = 35
	name = "Summon Hounds"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	Profile.instance.message_print("You concentrate on the image of a pack of hounds...")
	if Rng.roll_die(5) > 2:
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_HOUND, true):
			Profile.instance.message_print("No-one ever turns up.")
	elif dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level, SummonType.SUMMON_HOUND):
		Profile.instance.message_print("The summoned hounds get angry!")
	else:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "60% control"
