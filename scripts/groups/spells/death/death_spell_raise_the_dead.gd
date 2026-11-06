# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellRaiseTheDead
extends Spell


func _init() -> void:
	default_base_failure = 75
	default_level = 21
	default_power_cost = 20
	first_cast_experience = 50
	name = "Raise the Dead"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3) == 1:
		if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, player.level * 3 / 2, SummonType.SUMMON_HI_UNDEAD if player.level > 47 else SummonType.SUMMON_UNDEAD):
			Profile.instance.message_print("Cold winds begin to swirl around you, carrying with them the stench of decay...")
			Profile.instance.message_print("The dead arise... to punish you for disturbing them!")
		else:
			Profile.instance.message_print("No-one ever turns up.")
	else:
		if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level * 3 / 2, SummonType.SUMMON_HI_UNDEAD_NO_UNIQUES if player.level > 47 else SummonType.SUMMON_UNDEAD, player.level > 24 and Rng.roll_die(3) == 1):
			Profile.instance.message_print("Cold winds begin to swirl around you, carrying with them the stench of decay...")
			Profile.instance.message_print("Ancient, long dead forms arise from the ground to serve you!")
		else:
			Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "control 67%"
