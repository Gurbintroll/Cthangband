# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBanishAll
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You feel a terrifying power lurking behind you."
	have_message = "You sometimes cause nearby creatures to vanish."
	lose_message = "You no longer feel a terrifying power lurking behind you."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(9000) != 1:
		return
	savegame.disturb(false)
	Profile.instance.message_print("You suddenly feel almost lonely.")
	savegame.spell_effects_handler.banish_monsters(100)
	Profile.instance.message_print("")
