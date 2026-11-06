# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellMassSummons
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 43
	default_power_cost = 100
	name = "Mass Summons"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var none_came: bool = true
	Profile.instance.message_print("You concentrate on several images at once...")
	for i: int in 3 + player.level / 10:
		if Rng.roll_die(10) > 3:
			if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_NO_UNIQUES, false):
				none_came = false
		elif dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level, SummonType.SUMMON_NO_UNIQUES):
			Profile.instance.message_print("A summoned creature gets angry!")
			none_came = false
	if none_came:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "70% control"
