# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBreatheFire
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(20, player.level, Ability.CONSTITUTION, 18):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	Profile.instance.message_print("You breathe fire...")
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, player.level * 2, -(1 + player.level / 10))


func activation_summary(level: int) -> String:
	return "fire breath      (unusable until level 20)" if level < 20 else "fire breath      (cost %d, dam %d, CON based)" % [level, level * 2]


func initialise() -> void:
	frequency = 3
	gain_message = "You gain the ability to breathe fire."
	have_message = "You can breathe fire (dam lvl * 2)."
	lose_message = "You lose the ability to breathe fire."
