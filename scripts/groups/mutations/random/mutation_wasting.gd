# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationWasting
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You suddenly contract a horrible wasting disease."
	have_message = "You have a horrible wasting disease."
	lose_message = "You are cured of the horrible wasting disease!"


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) != 1:
		return
	var which_ability: int = Rng.roll_die(6) - 1
	var sustained: bool = false
	match which_ability:
		Ability.STRENGTH:
			sustained = player.has_sustain_strength
		Ability.INTELLIGENCE:
			sustained = player.has_sustain_intelligence
		Ability.WISDOM:
			sustained = player.has_sustain_wisdom
		Ability.DEXTERITY:
			sustained = player.has_sustain_dexterity
		Ability.CONSTITUTION:
			sustained = player.has_sustain_constitution
		Ability.CHARISMA:
			sustained = player.has_sustain_charisma
	if sustained:
		return
	savegame.disturb(false)
	if Rng.roll_die(10) <= player.religion.gods[GodName.LOBON].adjusted_favour():
		Profile.instance.message_print("Lobon's favour protects you from wasting away!")
		Profile.instance.message_print("")
		return
	Profile.instance.message_print("You can feel yourself wasting away!")
	Profile.instance.message_print("")
	player.decrease_ability_score(which_ability, Rng.roll_die(6) + 6, Rng.roll_die(3) == 1)
