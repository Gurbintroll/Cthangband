# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ProjectileWizardBolt
extends Projectile


func _init(effects: SpellEffectsHandler) -> void:
	super(effects)
	bolt_graphic = "PinkBolt"
	impact_graphic = "PinkSplat"
	effect_animation = ""


func affect_item(who: int, row: int, col: int) -> bool:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var obvious: bool = false
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	var item_name: String = ""
	var item_index: int = tile.item_index
	while item_index != 0:
		var is_artifact: bool = false
		var plural: bool = false
		var item: Item = dungeon_level.dungeon_items.items[item_index]
		item_index = item.next_in_stack
		item.get_merged_flags(f_1, f_2, f_3)
		if item.count > 1:
			plural = true
		if item.is_artifact() or item.is_legendary():
			is_artifact = true
		var note_kill: String = " are destroyed!" if plural else " is destroyed!"
		if item.marked:
			obvious = true
			item_name = item.description(false, 0)
		if is_artifact:
			if item.marked:
				var s: String = "are" if plural else "is"
				Profile.instance.message_print("The %s %s unaffected!" % [item_name, s])
		else:
			if item.marked and note_kill == "":
				Profile.instance.message_print("The %s%s" % [item_name, note_kill])
			var sub_category: int = item.sub_category
			var is_potion: int = item.item_type.item_category == ItemCategory.POTION
			dungeon_level.delete_item_by_index(item_index)
			if is_potion:
				spell_effects.potion_smass_effect(who, row, col, sub_category)
			dungeon_level.redraw_single_location(row, col)
	return obvious


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
	if monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_2.is_set(MonsterFlags2.STUPID) or monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character):
		note_dies = " is destroyed."
	if who == 0 and monster.mind.is_set(MonsterMind.FRIENDLY):
		if who == 0:
			Profile.instance.message_print("%s gets angry!" % [monster_name])
			monster.mind.clear_bits(MonsterMind.FRIENDLY)
	if seen:
		obvious = true
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		if who != 0 and dam > monster.health:
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
			var fear: bool = CommandEngine.fear
			if note != "" and seen:
				Profile.instance.message_print("%s%s" % [monster_name, note])
			elif dam > 0:
				dungeon_level.dungeon_monsters.message_pain(tile.monster_index, dam)
			if fear and monster.is_visible:
				Profile.instance.message_print("%s flees in terror!" % [monster_name])
	dungeon_level.dungeon_monsters.update_monster_visibility(tile.monster_index, false)
	dungeon_level.redraw_single_location(row, col)
	projectile_mon_num += 1
	projectile_mon_col = col
	projectile_mon_row = row
	return obvious
