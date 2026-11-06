# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellSummonMonster
extends Spell


func _init() -> void:
	default_base_failure = 70
	default_level = 30
	default_power_cost = 25
	first_cast_experience = 9
	name = "Summon Monster"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	Profile.instance.message_print("You concentrate on the image of a monster...")
	if Rng.roll_die(5) > 2:
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_NO_UNIQUES, false):
			Profile.instance.message_print("No-one ever turns up.")
	elif dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level, SummonType.SUMMON_NO_UNIQUES):
		Profile.instance.message_print("The summoned monster gets angry!")
	else:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "60% control"
