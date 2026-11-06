# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationDazzle
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(7, 15, Ability.CHARISMA, 8):
		return
	savegame.spell_effects_handler.stun_monsters(player.level * 4)
	savegame.spell_effects_handler.confuse_monsters(player.level * 4)
	savegame.spell_effects_handler.turn_monsters(player.level * 4)


func activation_summary(level: int) -> String:
	return "dazzle           (unusable until level 7)" if level < 7 else "dazzle           (cost 15, CHA based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You gain the ability to emit dazzling lights."
	have_message = "You can emit confusing, blinding radiation."
	lose_message = "You lose the ability to emit dazzling lights."
