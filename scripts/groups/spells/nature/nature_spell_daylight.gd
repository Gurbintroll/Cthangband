# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellDaylight
extends Spell


func _init() -> void:
	default_base_failure = 50
	default_level = 2
	default_power_cost = 2
	first_cast_experience = 5
	name = "Daylight"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	savegame.spell_effects_handler.light_area(Rng.roll_dice(2, player.level / 2), player.level / 10 + 1)
	if not player.ancestry.is_sunlight_sensitive or player.has_light_resistance:
		return
	Profile.instance.message_print("The daylight scorches your flesh!")
	player.take_hit(Rng.roll_dice(2, 2), "daylight")


func comment(player: Player) -> String:
	return "dam 2d%d" % [player.level / 2]
