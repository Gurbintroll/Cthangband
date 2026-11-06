# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentPsychometry
extends Talent


func initialise(profession: Profession) -> void:
	name = "Psychometry"
	level = 15
	power_cost = 12
	base_failure = 60


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	if player.level < 40:
		psychometry(player, dungeon_level, savegame)
	else:
		savegame.spell_effects_handler.identify_item()


func psychometry(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	var item_index: int = savegame.get_item("Meditate on which item_index? ", true, true, true)
	if item_index == 99:
		return
	if (item_index == -2):
		Profile.instance.message_print("You have nothing appropriate.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	if (item.is_known() || item.identify_flags.is_set(IdentifyFlags.SENSED)):
		Profile.instance.message_print("You cannot find out anything more about that.")
		return
	var feel: String = item.get_detailed_feeling()
	var item_name: String = item.description(false, 0)
	if feel == "":
		Profile.instance.message_print("You do not perceive anything unusual about the %s." % [item_name])
		return
	var s: String = "is" if item.count == 1 else "are"
	Profile.instance.message_print("You feel that the %s %s %s..." % [item_name, s, feel])
	item.identify_flags.set_bits(IdentifyFlags.SENSED)
	if item.inscription == "":
		item.inscription = feel
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)


func comment(player: Player) -> String:
	return ""
