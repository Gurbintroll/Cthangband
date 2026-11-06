# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationShriek
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(4, 4, Ability.CONSTITUTION, 6):
		return
	savegame.spell_effects_handler.fire_ball(ProjectileSound.new(savegame.spell_effects_handler), 0, 4 * player.level, 8)
	savegame.spell_effects_handler.aggravate_monsters(0)


func activation_summary(level: int) -> String:
	return "shriek           (unusable until level 4)" if level < 4 else "shriek           (cost 4, CON based)"


func initialise() -> void:
	frequency = 3
	gain_message = "Your vocal cords get much tougher."
	have_message = "You can emit a horrible shriek."
	lose_message = "Your vocal cords get much weaker."
