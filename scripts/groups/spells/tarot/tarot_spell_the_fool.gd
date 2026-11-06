# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellTheFool
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 33
	default_power_cost = 28
	first_cast_experience = 12
	name = "The Fool"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var type: int = 0
	Profile.instance.message_print("You concentrate on the Fool card...")
	match Rng.roll_die(4):
		1:
			type = SummonType.SUMMON_BIZARRE_1
		2:
			type = SummonType.SUMMON_BIZARRE_2
		3:
			type = SummonType.SUMMON_BIZARRE_4
		4:
			type = SummonType.SUMMON_BIZARRE_5
	if Rng.roll_die(6) > 3:
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, type, false):
			Profile.instance.message_print("No-one ever turns up.")
	elif dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level, type):
		Profile.instance.message_print("The summoned creature gets angry!")
	else:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "50% control"
