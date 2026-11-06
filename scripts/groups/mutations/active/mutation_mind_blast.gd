# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationMindBlast
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(5, 3, Ability.WISDOM, 15):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fir_bolt(ProjectilePsi.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + (player.level - 1) / 5, 3))


func activation_summary(level: int) -> String:
	return "mind blast       (unusable until level 5)" if level < 5 else "mind blast       (cost 3, WIS based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You gain the power of Mind Blast."
	have_message = "You can Mind Blast your enemies."
	lose_message = "You lose the power of Mind Blast."
