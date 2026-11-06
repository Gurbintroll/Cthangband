# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellCallSunlight
extends Spell


func _init() -> void:
	default_base_failure = 90
	default_level = 37
	default_power_cost = 35
	first_cast_experience = 100
	name = "Call Sunlight"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	savegame.spell_effects_handler.fire_ball(ProjectileLight.new(savegame.spell_effects_handler), 0, 150, 8)
	dungeon_level.wiz_light()
	if not player.ancestry.is_sunlight_sensitive or player.has_light_resistance:
		return
	Profile.instance.message_print("The sunlight scorches your flesh!")
	player.take_hit(50, "sunlight")


func comment(player: Player) -> String:
	return "dam 150"
