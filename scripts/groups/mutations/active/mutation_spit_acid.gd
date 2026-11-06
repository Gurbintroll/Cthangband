# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSpitAcid
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(9, 9, Ability.DEXTERITY, 15):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	Profile.instance.message_print("You spit acid...")
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, player.level, 1 + (player.level / 30))


func activation_summary(level: int) -> String:
	return "spit acid        (unusable until level 9)" if level < 9 else "spit acid        (cost 9, dam %d, DEX based)" % [level]


func initialise() -> void:
	frequency = 4
	gain_message = "You gain the ability to spit acid."
	have_message = "You can spit acid (dam lvl)."
	lose_message = "You lose the ability to spit acid."
