# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationVampirism
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(13, player.level, Ability.CONSTITUTION, 14):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	if savegame.spell_effects_handler.drain_life(dir, player.level * 2):
		player.restore_health(player.level + Rng.roll_die(player.level))


func activation_summary(level: int) -> String:
	return "vampiric drain   (unusable until level 13)" if level < 13 else "vampiric drain   (cost %d, CON based)" % [level]


func initialise() -> void:
	frequency = 2
	gain_message = "You become vampiric."
	have_message = "You can drain life from a foe like a vampire."
	lose_message = "You are no longer vampiric."
