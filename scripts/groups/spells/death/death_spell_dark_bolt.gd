# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellDarkBolt
extends Spell


func _init() -> void:
	default_base_failure = 30
	default_level = 11
	default_power_cost = 11
	first_cast_experience = 15
	name = "Dark Bolt"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var beam: int = player.profession.spell_beam_chance(player.level)
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	savegame.spell_effects_handler.fire_bolt_or_beam(beam, ProjectileDark.new(savegame.spell_effects_handler), dir, Rng.roll_dice(4 + (player.level - 5) / 4, 8))


func comment(player: Player) -> String:
	return "dam %dd8" % [4 + (player.level - 5) / 4]
