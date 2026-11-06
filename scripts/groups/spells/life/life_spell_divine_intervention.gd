# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name LifeSpellDivineIntervention
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 47
	default_power_cost = 50
	first_cast_experience = 100
	name = "Divine Intervention"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	savegame.spell_effects_handler.project(0, 1, player.map_row, player.map_col, 777, ProjectileHolyFire.new(savegame.spell_effects_handler), ProjectionFlag.KILL)
	savegame.spell_effects_handler.dispel_monsters(player.level * 4)
	savegame.spell_effects_handler.slow_monsters()
	savegame.spell_effects_handler.stun_monsters(player.level * 4)
	savegame.spell_effects_handler.confuse_monsters(player.level * 4)
	savegame.spell_effects_handler.turn_monsters(player.level * 4)
	savegame.spell_effects_handler.stasis_monsters(player.level * 4)
	dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, player.level, SummonType.SUMMON_CTHULOID, true)
	player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(25) + 25)
	player.restore_health(300)
	if player.timed_haste == 0:
		player.set_timed_haste(Rng.roll_die(20 + player.level) + player.level)
	else:
		player.set_timed_haste(player.timed_haste + Rng.roll_die(5))
	player.set_timed_fear(0)


func comment(player: Player) -> String:
	return "heal 300/dam 777" % [player.level * 4]
