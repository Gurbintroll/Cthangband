# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationLaserEyes
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(7, 10, Ability.WISDOM, 9):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_beam(ProjectileLight.new(savegame.spell_effects_handler), dir, 2 * player.level)


func activation_summary(level: int) -> String:
	return "laser eyes        (unusable until level 7)" if level < 7 else "laser eyes        (cost 10, WIS based)"


func initialise() -> void:
	frequency = 3
	gain_message = "Your eyes burn for a moment."
	have_message = "Your eyes can fire laser beams."
	lose_message = "Your eyes burn for a moment, then feel soothed."
