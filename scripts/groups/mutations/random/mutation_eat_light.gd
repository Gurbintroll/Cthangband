# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationEatLight
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You feel a strange kinship with Nyogtha."
	have_message = "You sometimes feed off of the light around you."
	lose_message = "You feel the world's a brighter place."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) != 1:
		return
	Profile.instance.message_print("A shadow passes over you.")
	Profile.instance.message_print("")
	if dungeon_level.get_grid(player.map_row, player.map_col).tile_flags.is_set(GridTile.SELF_LIT):
		player.restore_health(10)
	var item: Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	if (item.sub_category == LightType.TORCH or item.sub_category == LightType.LANTERN) and item.type_specific_value > 0:
		player.restore_health(item.type_specific_value / 20)
		item.type_specific_value /= 2
		Profile.instance.message_print("You absorb energy from your light!")
		if player.timed_blindness != 0:
			if item.type_specific_value == 0:
				item.type_specific_value = 1
		elif item.type_specific_value == 0:
			Profile.instance.message_print("Your light has gone out!")
		elif item.type_specific_value < 100 and item.type_specific_value % 10 == 0:
			Profile.instance.message_print("Your light is growing faint.")
	savegame.spell_effects_handler.unlight_area(50, 10)
