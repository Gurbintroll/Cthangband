# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProjectilePsiDrain
extends Projectile


func _init(effects: SpellEffectsHandler) -> void:
	super(effects)
	bolt_graphic = "BeigeBolt"
	impact_graphic = ""
	effect_animation = "BeigeContract"


func affect_monster(who: int, r: int, row: int, col: int, dam: int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	var monster_type: MonsterType = monster.monster_type
	var seen: bool = monster.is_visible
	var obvious: bool = false
	var note: String = ""
	var note_dies: String = " dies."
	if tile.monster_index == 0:
		return false
	if who != 0 and tile.monster_index == who:
		return false
	dam = (dam + r) / (r + 1)
	var monster_name: String = monster.describe(0)
	if who == 0 and monster.mind.is_set(MonsterMind.FRIENDLY):
		var get_angry: bool = monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_CONFUSION)
		if get_angry and who == 0:
			Profile.instance.message_print("%s gets angry" % [monster_name])
			monster.mind.clear_bits(MonsterMind.FRIENDLY)
	if seen:
		obvious = true
	if monster_type.flags_2.is_set(MonsterFlags2.EMPTY_MIND):
		dam = 0
		note = " is immune!"
	elif (monster_type.flags_2.is_set(MonsterFlags2.STUPID) or monster_type.flags_2.is_set(MonsterFlags2.WEIRD_MIND)
		or monster_type.flags_3.is_set(MonsterFlags3.ANIMAL) or monster_type.level > Rng.roll_die(3 * dam) ):
		dam /= 3
		note = " resists"
		if ((monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or monster_type.flags_3.is_set(MonsterFlags3.DEMON))
			and monster_type.level > player.level / 2 and Rng.roll_die(2) == 1 ):
			var s: String = "'s" if seen else "s"
			note = ""
			Profile.instance.message_print("%s%s corrupted mind backlashes your attack!" % [monster_name, s])
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				var killer: String = monster.describe(0x88)
				Profile.instance.message_print("Your psychic energy is drained!")
				player.power = maxi(0, player.power - Rng.roll_dice(5, dam) / 2)
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
				player.take_hit(dam, killer)
			dam = 0
	elif dam > 0:
		var b: int = Rng.roll_dice(5, dam) / 4
		var s: String = "'s" if seen else "s"
		Profile.instance.message_print("You convert %s%s pain into psychic energy!" % [monster_name, s])
		player.power = mini(player.max_power, player.power + b)
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
	note_dies = " collapses, a mindless husk."
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who != 0 and dam > monster.health:
			dam = monster.health
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who > 0 and dam > monster.health:
			dam = monster.health
	if dam > monster.health:
		note = note_dies
	if who != 0:
		if savegame.tracked_monster_index == tile.monster_index:
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
		monster.sleep_level = 0
		monster.health -= dam
		if monster.health < 0:
			var sad: bool = monster.mind.is_set(MonsterMind.FRIENDLY) and not monster.is_visible
			savegame.monster_death(tile.monster_index)
			dungeon_level.dungeon_monsters.delete_monster_by_index(tile.monster_index, true)
			if note != "":
				Profile.instance.message_print("%s%s" % [monster_name, note])
			if sad:
				Profile.instance.message_print("You feel sad for a moment.")
		else:
			if note != "" and seen:
				Profile.instance.message_print("%s%s" % [monster_name, note])
			elif dam > 0:
				dungeon_level.dungeon_monsters.message_pain(tile.monster_index, dam)
	else:
		if dungeon_level.dungeon_monsters.damage_monster(tile.monster_index, dam, note_dies):
			pass
		else:
			if note != "" and seen:
				Profile.instance.message_print("%s%s" % [monster_name, note])
			elif dam > 0:
				dungeon_level.dungeon_monsters.message_pain(tile.monster_index, dam)
			if CommandEngine.fear and monster.is_visible:
				Profile.instance.message_print("%s flees in terror!" % [monster_name])
	dungeon_level.dungeon_monsters.update_monster_visibility(tile.monster_index, false)
	dungeon_level.redraw_single_location(row, col)
	projectile_mon_num += 1
	projectile_mon_row = row
	projectile_mon_col = col
	return obvious


func affect_player(who: int, r: int, row: int, col: int, dam: int, rad_a: int) -> bool:
	var blind: bool = player.timed_blindness != 0
	if col != player.map_col or row != player.map_row:
		return false
	if who == 0:
		return false
	if player.has_reflection and rad_a == 0 and Rng.roll_die(10) != 1:
		var row_t: int = dungeon_level.dungeon_monsters.monsters[who].map_row - 1 + Rng.roll_die(3)
		var col_t: int = dungeon_level.dungeon_monsters.monsters[who].map_col - 1 + Rng.roll_die(3)
		var max_attempts: int = 10
		Profile.instance.message_print("Something bounces!" if blind else "The attack bounces!")
		while max_attempts > 0 and dungeon_level.in_outer_bounds(row_t, col_t) and not dungeon_level.player_has_los_bold(row_t, col_t):
			row_t = dungeon_level.dungeon_monsters.monsters[who].map_row - 1 + Rng.roll_die(3)
			col_t = dungeon_level.dungeon_monsters.monsters[who].map_col - 1 + Rng.roll_die(3)
			max_attempts -= 1
		if max_attempts < 1:
			row_t = dungeon_level.dungeon_monsters.monsters[who].map_row
			col_t = dungeon_level.dungeon_monsters.monsters[who].map_col
		fire(0, 0, row_t, col_t, dam, ProjectionFlag.STOP | ProjectionFlag.KILL)
		savegame.disturb(true)
		return true
	savegame.disturb(true)
	return true
