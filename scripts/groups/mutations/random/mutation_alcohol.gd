# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationAlcohol
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "Your body starts producing alcohol!"
	have_message = "Your body produces alcohol."
	lose_message = "Your body stops producing alcohol!"


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(6400) != 321:
		return
	if player.has_chaos_resistance and player.has_confusion_resistance:
		return
	savegame.disturb(false)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EXTRA)
	Profile.instance.message_print("You feel a SSSCHtupor cOmINg over yOu... *HIC*!")
	if Rng.roll_die(20) == 1:
		Profile.instance.message_print("")
		if Rng.roll_die(3) == 1:
			savegame.spell_effects_handler.lose_all_info()
		else:
			dungeon_level.wiz_dark()
		savegame.spell_effects_handler.teleport_player(100)
		Profile.instance.message_print("You wake up somewhere with a sore head...")
		Profile.instance.message_print("You can't remember a thing, or how you got here!")
	else:
		if not player.has_confusion_resistance:
			player.set_timed_confusion(player.timed_confusion + Rng.roll_die(20) + 14)
		if not player.has_chaos_resistance and Rng.roll_die(3) == 1:
			Profile.instance.message_print("Thishcischs GooDSChtuff!")
			player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(150) + 149)
