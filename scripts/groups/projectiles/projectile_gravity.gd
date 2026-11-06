# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProjectileGravity
extends Projectile


func _init(effects: SpellEffectsHandler) -> void:
	super(effects)
	bolt_graphic = "TurquoiseBolt"
	impact_graphic = "TurquoiseSplat"
	effect_animation = ""


func affect_monster(who: int, r: int, row: int, col: int, dam: int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	var monster_type: MonsterType = monster.monster_type
	var seen: bool = monster.is_visible
	var obvious: bool = false
	var do_dist: int = 0
	var do_stun: int = 0
	var note: String = ""
	var note_dies: String = " dies."
	if tile.monster_index == 0:
		return false
	if who != 0 and tile.monster_index == who:
		return false
	dam = (dam + r) / (r + 1)
	var monster_name: String = monster.describe(0)
	if (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or
		monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_3.is_set(MonsterFlags2.STUPID) or
		monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
		note_dies = " is destroyed."
	if who == 0 and monster.mind.is_set(MonsterMind.FRIENDLY):
		Profile.instance.message_print("%s gets angry!" % [monster_name])
		monster.mind.clear_bits(MonsterMind.FRIENDLY)
	var resists_tele: bool = false
	if seen:
		obvious = true
	if monster_type.flags_3.is_set(MonsterFlags3.RESIST_TELEPORT):
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			if seen:
				monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.RESIST_TELEPORT)
			note = " is unaffected!"
			resists_tele = true
		elif monster_type.level > Rng.roll_die(100):
			if seen:
				monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.RESIST_TELEPORT)
			note = " resists!"
			resists_tele = true
	do_dist = 0 if resists_tele else 10
	if monster_type.flags_4.is_set(MonsterFlags4.BREATHE_GRAVITY):
		note = " resists."
		dam *= 3
		dam /= Rng.roll_die(6) + 6
		do_dist = 0
	else:
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.level > Rng.roll_die(1 if dam - 10 < 1 else dam - 10) + 10:
			obvious = false
		else:
			if monster.speed > 60:
				monster.speed -= 10
			note = " starts moving slower."
		do_stun = Rng.roll_dice(player.level / 10 + 3, dam) + 1
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.level > Rng.roll_die(1 if dam - 10 < 1 else dam - 10) + 10:
			do_stun = 0
			note = " is unaffected!"
			obvious = false
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who != 0 and dam > monster.health:
			dam = monster.health
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who > 0 and dam > monster.health:
			dam = monster.health
	if dam > monster.health:
		note = note_dies
	elif do_dist != 0:
		if seen:
			obvious = true
		note = " disappears!"
		spell_effects.teleport_away(tile.monster_index, do_dist)
		row = monster.map_row
		col = monster.map_col
		tile = dungeon_level.get_grid(row, col)
	elif do_stun != 0 and monster_type.flags_4.is_clear(MonsterFlags4.BREATHE_SOUND) and monster_type.flags_4.is_clear(MonsterFlags4.BREATHE_FORCE):
		var temp: int
		if monster.stun_level != 0:
			note = " is more dazed."
			temp = monster.stun_level + (do_stun / 2)
		else:
			note = " is dazed."
			temp = do_stun
		monster.stun_level = mini(temp, 200)
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
	var fuzzy: bool = false
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
	if dam > 1600:
		dam = 1600
	dam = (dam + r) / (r + 1)
	if blind:
		fuzzy = true
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[who]
	var killer: String = monster.describe(0x88)
	if fuzzy:
		Profile.instance.message_print("You are hit by something heavy!")
	Profile.instance.message_print("Gravity warps around you.")
	spell_effects.teleport_player(5)
	if not player.has_feather_fall:
		player.set_timed_slow(player.timed_slow + Rng.roll_die(4) + 3)
	if not (player.has_sound_resistance or player.has_feather_fall):
		var stun_time: int = Rng.roll_die(35 if dam > 90 else (dam / 3) + 5)
		player.set_timed_stun(player.timed_stun + stun_time)
	if not player.has_sound_resistance:
		player.set_timed_stun(player.timed_stun + Rng.roll_die(20))
	if player.has_feather_fall:
		dam = dam * 2 / 3
	if not player.has_feather_fall or Rng.roll_die(13) == 1:
		player.inventory.inven_damage(spell_effects.set_cold_destroy, 2)
	spell_effects.take_hit(dam, killer)
	savegame.disturb(true)
	return true
