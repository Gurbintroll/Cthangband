# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ChaosSpellSummonDemon
extends Spell


func _init() -> void:
	default_base_failure = 90
	default_level = 46
	default_power_cost = 100
	first_cast_experience = 1250
	name = "Summon Demon"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3) == 1:
		if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level * 3 / 2, SummonType.SUMMON_DEMON):
			Profile.instance.message_print("The area fills with a stench of sulphur and brimstone.")
			Profile.instance.message_print("'NON SERVIAM! Wretch! I shall feast on thy mortal soul!'")
		else:
			Profile.instance.message_print("No-one ever turns up.")
	else:
		if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level * 3 / 2, SummonType.SUMMON_DEMON, player.level == 50):
			Profile.instance.message_print("The area fills with a stench of sulphur and brimstone.")
			Profile.instance.message_print("'What is thy bidding... Master?'")
		else:
			Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return ""
