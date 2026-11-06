# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationEatMagic
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(17, 1, Ability.WISDOM, 15):
		return
	savegame.item_filter_function = savegame.item_filter_rechargable
	var item_index: int = savegame.get_item("Drain which item?", false, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have no items that you can drain.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[-item_index]
	var item_level: int = item.item_type.level
	if item.category == ItemCategory.ROD:
		if item.type_specific_value > 0:
			Profile.instance.message_print("You can't absorb energy from a discharged rod.")
		else:
			player.power += 2 * item_level
			item.type_specific_value = 500
	else:
		if item.type_specific_value > 0:
			player.power += item.type_specific_value * item_level
			item.type_specific_value = 0
		else:
			Profile.instance.message_print("There is no power left to absorb!")
	player.power = mini(player.power, player.max_power)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)


func activation_summary(level: int) -> String:
	return "eat magic        (unusable until level 17)" if level < 17 else "eat magic        (cost 1, WIS based)"


func initialise() -> void:
	frequency = 1
	gain_message = "Your magic items look delicious."
	have_message = "You can consume magic energy for your own use."
	lose_message = "Your magic items no longer look delicious."
