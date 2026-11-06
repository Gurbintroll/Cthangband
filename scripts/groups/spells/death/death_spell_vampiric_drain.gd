# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellVampiricDrain
extends Spell


func _init() -> void:
	default_base_failure = 60
	default_level = 23
	default_power_cost = 20
	first_cast_experience = 16
	name = "Vampiric Drain"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	var dummy: int = player.level + (Rng.roll_die(player.level) * maxi(1, player.level / 10))
	if not savegame.spell_effects_handler.drain_life(dir, dummy):
		return
	player.restore_health(dummy)
	dummy = player.food + mini(5000, 100 * dummy)
	if player.food < Constants.FOOD_GORGED:
		player.set_food(mini(Constants.FOOD_GORGED, dummy))


func comment(player: Player) -> String:
	return "dam %dd%d" % [maxi(1, player.level / 10), player.level * 2]
