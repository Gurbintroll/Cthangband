# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationAttractAnimals
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You start attracting animals."
	have_message = "You attract animals."
	lose_message = "You stop attracting animals."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(7000) != 1:
		return
	var worked: bool = false
	if Rng.roll_die(3) == 1:
		worked = dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_ANIMAL, true)
	else:
		worked = dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_ANIMAL)
	if not worked:
		return
	Profile.instance.message_print("You have attracted an animal!")
	savegame.disturb(false)
