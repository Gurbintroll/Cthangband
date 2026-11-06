# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationProducePower
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You start producing magical energy uncontrollably."
	have_message = "You are producing magical energy uncontrollably."
	lose_message = "You stop producing magical energy uncontrollably."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(9000) != 1:
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	savegame.disturb(false)
	Profile.instance.message_print("Magical energy dlows through you! You must release it!")
	Profile.instance.message_print("")
	var dir: int = target_engine.get_direction_no_aim()
	savegame.spell_effects_handler.fire_ball(ProjectilePower.new(savegame.spell_effects_handler), dir, player.level * 2, 3)
