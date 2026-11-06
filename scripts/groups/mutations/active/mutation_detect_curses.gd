# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationDetectCurses
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(7, 14, Ability.WISDOM, 14):
		return
	for item: Item in player.inventory.items:
		if item.item_type == null:
			continue
		if not item.is_cursed():
			continue
		item.inscription = "cursed"


func activation_summary(level: int) -> String:
	return "detect curses    (unusable until level 7)" if level < 7 else "detect curses    (cost 14, WIS based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You can feel evil magics."
	have_message = "You can feel the danger of evil magic."
	lose_message = "You can no longer feel evil magics."
