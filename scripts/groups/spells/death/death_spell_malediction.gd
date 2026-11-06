# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellMalediction
extends Spell


func _init() -> void:
	default_base_failure = 25
	default_level = 2
	default_power_cost = 2
	first_cast_experience = 4
	name = "Malediction"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_ball(ProjectileHellFire.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + (player.level - 1) / 5, 3), 0)
	if Rng.roll_die(5) != 1:
		return
	var dummy: int = Rng.roll_die(1000)
	if dummy == 666:
		savegame.spell_effects_handler.fire_bolt(ProjectileDeathRay.new(savegame.spell_effects_handler), dir, player.level)
	elif dummy < 500:
		savegame.spell_effects_handler.fire_bolt(ProjectileTurnAll.new(savegame.spell_effects_handler), dir, player.level)
	elif dummy < 800:
		savegame.spell_effects_handler.fire_bolt(ProjectileOldConfusion.new(savegame.spell_effects_handler), dir, player.level)
	else:
		savegame.spell_effects_handler.fire_bolt(ProjectileStun.new(savegame.spell_effects_handler), dir, player.level)


func comment(player: Player) -> String:
	return "dam %dd3" % [3 + (player.level - 1) / 5]
