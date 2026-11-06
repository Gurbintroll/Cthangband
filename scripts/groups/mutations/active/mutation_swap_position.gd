# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSwapPosition
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(15, 12, Ability.DEXTERITY, 16):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.teleport_swap(dir)


func activation_summary(level: int) -> String:
	return "swap position    (unusable until level 15)" if level < 15 else "swap position    (cost 12, DEX based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You feel like walking a mile in someone else's shoes."
	have_message = "You can switch locations with another being."
	lose_message = "You feel like staying in your own shoes."
