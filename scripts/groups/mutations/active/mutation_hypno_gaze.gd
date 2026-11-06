# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHypnoGaze
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(12, 12, Ability.CHARISMA, 18):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	Profile.instance.message_print("Your eyes look mesmerising...")
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.charm_monster(dir, player.level)


func activation_summary(level: int) -> String:
	return "hypnotic gaze    (unusable until level 12)" if level < 12 else "hypnotic gaze    (cost 12, CHA based)"


func initialise() -> void:
	frequency = 2
	gain_message = "Your eyes look mesmerizing..."
	have_message = "Your gaze is hypnotic."
	lose_message = "Your eyes look uninteresting."
