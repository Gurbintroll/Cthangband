# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellVampirismTrue
extends Spell


func _init() -> void:
	default_base_failure = 60
	default_level = 32
	default_power_cost = 35
	first_cast_experience = 125
	name = "Vampirism True"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	for dummy: int in 3:
		if savegame.spell_effects_handler.drain_life(dir, 100):
			player.restore_health(100)


func comment(player: Player) -> String:
	return "dam 3*100"
