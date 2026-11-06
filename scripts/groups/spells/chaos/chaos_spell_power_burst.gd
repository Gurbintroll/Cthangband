# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ChaosSpellPowerBurst
extends Spell


func _init() -> void:
	default_base_failure = 50
	default_level = 9
	default_power_cost = 6
	first_cast_experience = 1
	name = "Power Burst"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_ball(ProjectileMissile.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3, 5) + player.level + (player.level / player.profession.spell_ball_size_factor), 2 if player.level < 30 else 3)


func comment(player: Player) -> String:
	return "dam 3d5+%d" % [player.level + (player.level / player.profession.spell_ball_size_factor)]
