# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentPrecognition
extends Talent


func initialise(profession: Profession) -> void:
	name = "Precognition"
	level = 1
	power_cost = 1
	base_failure = 15


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	if player.level > 44:
		dungeon_level.wiz_light()
	elif player.level > 19:
		dungeon_level.map_area()
	var found_something: bool = false
	if player.level < 30:
		found_something = savegame.spell_effects_handler.detect_monsters_normal()
		if player.level > 14:
			found_something = found_something or savegame.spell_effects_handler.detect_monsters_invis()
		if player.level > 4:
			found_something = found_something or savegame.spell_effects_handler.detect_traps()
	else:
		found_something = savegame.spell_effects_handler.detect_all()
	if player.level > 24 and player.level < 40:
		player.set_timed_telepathy(player.timed_telepathy + player.level)
	if not found_something:
		Profile.instance.message_print("You feel safe.")


func comment(player: Player) -> String:
	return ""
