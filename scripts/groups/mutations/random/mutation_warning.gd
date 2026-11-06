# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationWarning
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You suddenly feel paranoid."
	have_message = "You receive warnings about your foes."
	lose_message = "You no longer feel paranoid."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(1000) != 1:
		return
	var danger: int = 0
	for monster: Monster in dungeon_level.dungeon_monsters.monsters:
		var monster_type: MonsterType = monster.type
		if monster_type != null:
			if monster_type.level > player.level:
				danger += (monster_type.level - player.level)
	if danger > 100:
		Profile.instance.message_print("You feel utterly terrified!")
	elif danger > 50:
		Profile.instance.message_print("You feel terrified!")
	elif danger > 20:
		Profile.instance.message_print("You feel very worried!")
	elif danger > 10:
		Profile.instance.message_print("You feel paranoid!")
	elif danger > 5:
		Profile.instance.message_print("You feel almost safe.")
	else:
		Profile.instance.message_print("You feel lonely.")
