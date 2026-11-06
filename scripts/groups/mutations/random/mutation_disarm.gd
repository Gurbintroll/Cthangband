# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationDisarm
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "Your feet grow to four times their former size."
	have_message = "You occasionally stumble and drop things."
	lose_message = "Your feet shrink to their former size."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(10000) != 1:
		return
	savegame.disturb(false)
	Profile.instance.message_print("You trip over your own feet!")
	Profile.instance.message_print("")
	var item: Item = player.inventory.items[InventorySlot.MELEE_WEAPON]
	if item.item_type == null:
		return
	Profile.instance.message_print("You drop your weapon!")
	player.inventory.inven_drop(InventorySlot.MELEE_WEAPON, 1)
