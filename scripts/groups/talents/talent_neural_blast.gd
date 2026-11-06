# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentNeuralBlast
extends Talent


func initialise(profession: Profession) -> void:
	name = "Neural Blast"
	level = 2
	power_cost = 1
	base_failure = 20


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	if Rng.roll_die(100) <= player.level * 2:
		savegame.spell_effects_handler.fire_beam(ProjectilePsi.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + ((player.level - 1) / 4), 3 + (player.level / 15)))
	else:
		savegame.spell_effects_handler.fire_bolt(ProjectilePsi.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + ((player.level - 1) / 4), 3 + (player.level / 15)))


func comment(player: Player) -> String:
	return "dam %dd%d" % [3 + ((player.level - 1) / 4), 3 + (player.level / 15)]
