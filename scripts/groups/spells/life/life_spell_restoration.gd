# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name LifeSpellRestoration
extends Spell


func _init() -> void:
	default_base_failure = 82
	default_level = 42
	default_power_cost = 100
	first_cast_experience = 225
	name = "Restoration"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.try_restoring_ability_score(Ability.STRENGTH)
	player.try_restoring_ability_score(Ability.INTELLIGENCE)
	player.try_restoring_ability_score(Ability.WISDOM)
	player.try_restoring_ability_score(Ability.DEXTERITY)
	player.try_restoring_ability_score(Ability.CONSTITUTION)
	player.try_restoring_ability_score(Ability.CHARISMA)
	player.restore_level()


func comment(player: Player) -> String:
	return ""
