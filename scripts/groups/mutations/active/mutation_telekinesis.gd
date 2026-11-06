# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationTelekinesis
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(9, 9, Ability.WISDOM, 14):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.command_engine.summon_item(dir, player.level * 10, true)


func activation_summary(level: int) -> String:
	return "telekinesis      (unusable until level 9)" if level < 9 else "telekinesis      (cost 9, WIS based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You gain the ability to move objects telekinetically."
	have_message = "You are telekinetic."
	lose_message = "You lose the ability to move objects telekinetically."
