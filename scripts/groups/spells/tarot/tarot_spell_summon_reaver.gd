# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellSummonReaver
extends Spell


func _init() -> void:
	default_base_failure = 90
	default_level = 45
	default_power_cost = 100
	first_cast_experience = 200
	name = "Summon Reaver"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	Profile.instance.message_print("You concentrate on the image of a black reaver...")
	if Rng.roll_die(10) > 3:
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, 75, SummonType.SUMMON_REAVER, false):
			Profile.instance.message_print("No-one ever turns up.")
	elif dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, 75, SummonType.SUMMON_REAVER):
		Profile.instance.message_print("The summoned black reaver gets angry!")
	else:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "70% control"
