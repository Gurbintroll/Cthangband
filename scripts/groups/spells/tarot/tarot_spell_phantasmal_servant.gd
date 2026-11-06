# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellPhantasmalServant
extends Spell


func _init() -> void:
	default_base_failure = 60
	default_level = 28
	default_power_cost = 24
	first_cast_experience = 8
	name = "Phantasmal Servant"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level * 3 / 2, SummonType.SUMMON_PHANTOM, false):
		var honorific: String = ""
		match player.gender_index:
			GenderType.MALE:
				honorific = "master"
			GenderType.FEMALE:
				honorific = "mistress"
			GenderType.OTHER:
				honorific = "boss"
		Profile.instance.message_print("Your wish, %s?" % [honorific])
	else:
		Profile.instance.message_print("No-one ever turns up.")


func comment(player: Player) -> String:
	return "100% control"
