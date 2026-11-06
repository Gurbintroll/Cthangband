# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationGrowMould
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(1, 6, Ability.CONSTITUTION, 14):
		return
	for i: int in 8:
		dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_BIZARRE_1, false)


func activation_summary(level: int) -> String:
	return "grow mold        (unusable until level 1)" if level < 1 else "grow mold        (cost 6, CON based)"


func initialise() -> void:
	frequency = 1
	gain_message = "You feel a sudden affinity for mould."
	have_message = "You can cause mold to grow near you."
	lose_message = "You feel a sudden dislike for mold."
