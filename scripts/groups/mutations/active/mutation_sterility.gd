# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSterility
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(20, 40, Ability.CHARISMA, 18):
		return
	Profile.instance.message_print("You suddenly have a headache!")
	player.take_hit(Rng.roll_die(30) + 30, "the strain of forcing abstinence")
	dungeon_level.dungeon_monsters.num_repro += ArtificialIntelligence.MAX_REPRO


func activation_summary(level: int) -> String:
	return "sterilize        (unusable until level 20)" if level < 20 else "sterilize        (cost 40, CHA based)"


func initialise() -> void:
	frequency = 1
	gain_message = "You can give everything around you a headache."
	have_message = "You can cause mass impotence."
	lose_message = "You hear a massed sigh of relief."
