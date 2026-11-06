# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name FolkSpellElementalBall
extends Spell


func _init() -> void:
	default_base_failure = 66
	default_level = 41
	default_power_cost = 30
	first_cast_experience = 30
	name = "Elemental Ball"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	var projectile: Projectile
	match Rng.roll_die(4):
		1:
			projectile = ProjectileFire.new(savegame.spell_effects_handler)
		2:
			projectile = ProjectileElectricity.new(savegame.spell_effects_handler)
		3:
			projectile = ProjectileCold.new(savegame.spell_effects_handler)
		4:
			projectile = ProjectileAcid.new(savegame.spell_effects_handler)
	savegame.spell_effects_handler.fire_ball(projectile, dir, 75 + player.level, 2)


func comment(player: Player) -> String:
	return "dam %d" % [player.level + 75]
