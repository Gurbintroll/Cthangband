# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellResetRecall
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 6
	default_power_cost = 6
	first_cast_experience = 8
	name = "Reset Recall"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var prompt: String = "Reset to which level (1-%d): " % [player.max_dlv[savegame.current_dungeon.index]]
	var default: String = "%d" % [maxi(savegame.instance.current_depth, 1)]
	var result: Dictionary[bool, String] = User.get_prompted_input(prompt, default, 10)
	if result.has(true) == false:
		return
	var i: int = int(result[true])
	i = clampi(i, 1, player.max_dlv[savegame.current_dungeon.index])
	player.max_dlv[savegame.current_dungeon.index] = i
	Profile.instance.message_print("Recall depth set to level %d" % [i])


func comment(player: Player) -> String:
	return ""
