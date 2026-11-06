# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ChaosSpellWonder
extends Spell


func _init() -> void:
	default_base_failure = 25
	default_level = 17
	default_power_cost = 18
	first_cast_experience = 5
	name = "Wonder"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var beam_chance: int = player.profession.spell_beam_chance(player.level)
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	var die: int = Rng.roll_die(100) + (player.level / 5)
	if die > 100:
		Profile.instance.message_print("You feel a surge of power!")
	if die < 8:
		savegame.spell_effects_handler.clone_monster(dir)
	elif die < 14:
		savegame.spell_effects_handler.speed_monster(dir)
	elif die < 26:
		savegame.spell_effects_handler.heal_monster(dir)
	elif die < 31:
		savegame.spell_effects_handler.poly_monster(dir)
	elif die < 36:
		savegame.spell_effects_handler.fire_bolt_or_beam(beam_chance - 10, ProjectileMissile.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + ((player.level - 1) / 5), 4))
	elif die < 41:
		savegame.spell_effects_handler.confuse_monster(dir, player.level)
	elif die < 46:
		savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 20 + (player.level / 2), 3)
	elif die < 51:
		savegame.spell_effects_handler.light_line(dir)
	elif die < 56:
		savegame.spell_effects_handler.fire_bolt_or_beam(beam_chance - 10, ProjectileElectricity.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3 + ((player.level - 5) / 4), 8))
	elif die < 61:
		savegame.spell_effects_handler.fire_bolt_or_beam(beam_chance - 10, ProjectileCold.new(savegame.spell_effects_handler), dir, Rng.roll_dice(5 + ((player.level - 5) / 4), 8))
	elif die < 66:
		savegame.spell_effects_handler.fire_bolt_or_beam(beam_chance, ProjectileAcid.new(savegame.spell_effects_handler), dir, Rng.roll_dice(6 + ((player.level - 5) / 4), 8))
	elif die < 71:
		savegame.spell_effects_handler.fire_bolt_or_beam(beam_chance, ProjectileFire.new(savegame.spell_effects_handler), dir, Rng.roll_dice(8 + ((player.level - 5) / 4), 8))
	elif die < 76:
		savegame.spell_effects_handler.drain_life(dir, 75)
	elif die < 81:
		savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 30 + (player.level / 2), 2)
	elif die < 86:
		savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 40 + player.level, 2)
	elif die < 91:
		savegame.spell_effects_handler.fire_ball(ProjectileIce.new(savegame.spell_effects_handler), dir, 70 + player.level, 3)
	elif die < 96:
		savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 80 + player.level, 3)
	elif die < 101:
		savegame.spell_effects_handler.drain_life(dir, 100 + player.level)
	elif die < 104:
		savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 12)
	elif die < 106:
		savegame.spell_effects_handler.destroy_area(player.map_row, player.map_col, 15)
	elif die < 108:
		savegame.spell_effects_handler.carnage(true)
	elif die < 110:
		savegame.spell_effects_handler.dispel_monsters(120)
	else:
		savegame.spell_effects_handler.dispel_monsters(150)
		savegame.spell_effects_handler.slow_monsters()
		savegame.spell_effects_handler.sleep_monsters()
		player.restore_health(300)


func comment(player: Player) -> String:
	return ""
