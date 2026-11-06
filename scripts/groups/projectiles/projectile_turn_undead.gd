# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProjectileTurnUndead
extends Projectile


func _init(effects: SpellEffectsHandler) -> void:
	super(effects)
	bolt_graphic = ""
	impact_graphic = ""
	effect_animation = "WhiteControl"


func affect_monster(who: int, r: int, row: int, col: int, dam: int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	var monster_type: MonsterType = monster.monster_type
	var seen: bool = monster.is_visible
	var obvious: bool = false
	var skipped: bool = false
	var do_fear: int = 0
	var note: String = ""
	var note_dies: String = " dies."
	if tile.monster_index == 0:
		return false
	if who != 0 and tile.monster_index == who:
		return false
	dam = (dam + r) / (r + 1)
	var monster_name: String = monster.describe(0)
	if (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD)
		or monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_2.is_set(MonsterFlags2.STUPID)
		or monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
		note_dies = "is destroyed."
	if who == 0 and monster.mind.is_set(MonsterMind.FRIENDLY):
		var gets_angry: bool = monster_type.flags_3.is_set(MonsterFlags3.UNDEAD)
		if gets_angry:
			Profile.instance.message_print("%s gets angry!" % [monster_name])
			monster.mind.clear_bits(MonsterMind.FRIENDLY)
	if monster_type.flags_3.is_set(MonsterFlags3.UNDEAD):
		if seen:
			monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.UNDEAD)
			obvious = true
		do_fear = Rng.roll_dice(3, dam / 2) + 1
		if (monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.flags_3.is_set(MonsterFlags3.IMMUNE_FEAR)
			or monster_type.level > Rng.roll_die(1 if dam - 10 < 1 else dam - 10) + 10 ):
			note = " is unaffected."
			obvious = false
			do_fear = 0
	else:
		skipped = true
	dam = 0
	if skipped:
		return false
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who != 0 and dam > monster.health:
			dam = monster.health
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who > 0 and dam > monster.health:
			dam = monster.health
	if dam > monster.health:
		note = note_dies
	if do_fear != 0:
		var temp: int = monster.fear_level + do_fear
		monster.fear_level = mini(temp, 200)
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
			if (CommandEngine.fear or do_fear) and monster.is_visible:
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
