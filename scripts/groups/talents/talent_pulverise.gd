# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentPulverise
extends Talent


func initialise(profession: Profession) -> void:
	name = "Pulverise"
	level = 11
	power_cost = 7
	base_failure = 30


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_ball(ProjectileSound.new(savegame.spell_effects_handler), dir, Rng.roll_dice(8 + ((player.level - 5) / 4), 8), (((player.level - 20) / 8) + 1) if player.level > 20 else 0)


func comment(player: Player) -> String:
	return "dam %dd8" % [8 + ((player.level - 5) / 4)]
