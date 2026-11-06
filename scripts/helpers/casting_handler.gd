# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CastingHandler
extends RefCounted

var dungeon_level: DungeonLevel
var player: Player
var savegame: SaveGame


func _init(my_dungeon_level: DungeonLevel, my_player: Player) -> void:
	dungeon_level = my_dungeon_level
	player = my_player
	savegame = SaveGame.instance


func command_cast() -> void:
	if player.has_anti_magic:
		var which_magic_type: String = "magic"
		if player.spellcasting.casting_type == CastingType.MENTALISM:
			which_magic_type = "psychic talents"
		elif player.spellcasting.casting_type == CastingType.DIVINE:
			which_magic_type = "prayer"
		Profile.instance.message_print("An anti-magic shell disrupts your %s!" % [which_magic_type])
		savegame.energy_use = 5
	else:
		if player.spellcasting.casting_type == CastingType.MENTALISM:
			use_mentalism_power()
		else:
			cast_spell()


func get_spell(prompt: String, book_type: int, known: bool, use_realm_2: bool) -> int:
	var num: int = 0
	var spells: Array[int]
	var spell_index: int
	for i: int in 64:
		spells.append(0)
	var realm_to_use: int = player.realm_2 if use_realm_2 else player.realm_1
	var p: String = "prayer" if player.spellcasting.casting_type == CastingType.DIVINE else "spell"
	for spell: int in 32:
		if Tables.BOOK_SPELL_FLAGS[book_type] & (1 << spell) != 0:
			spells[num] = spell
			num += 1
	var okay: bool = false
	for i: int in num:
		if player.spell_okay(spells[i], known, use_realm_2):
			okay = true
	if not okay:
		return -2
	var flag: int = false
	var redraw: int = false
	var full_prompt: String = "(%ss a-%s, *=List, ESC=exit) %s which %s? " % [p, Util.index_to_label(num - 1), prompt, p]
	while not flag:
		var choice: String = User.get_simple_choice(full_prompt)
		if choice == "":
			break
		if choice == ' ' or choice == '*' or choice == '?':
			if not redraw:
				redraw = true
				Console.save_screen()
				player.print_spells(spells, num, 1, 20, realm_to_use)
			else:
				redraw = false
				Console.restore_screen()
			continue
		var i: int = Util.label_to_index(choice.to_lower())
		if i < 0 or i >= num:
			continue
		spell_index = spells[i]
		if not player.spell_okay(spell_index, known, use_realm_2):
			Profile.instance.message_print("You may not %s that %s." % [prompt, p])
			continue
		flag = true
	if redraw:
		Console.restore_screen()
	if not flag:
		return 99
	return spell_index


func cast_spell() -> void:
	var prayer: String = "prayer" if player.spellcasting.casting_type == CastingType.DIVINE else "spell"
	if player.realm_1 == 0:
		Profile.instance.message_print("You cannot cast spells!")
		return
	if player.timed_blindness != 0 or dungeon_level.no_light():
		Profile.instance.message_print("You cannot see!")
		return
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused!")
		return
	Inventory.item_filter_useable_spell_book = true
	var item_index: int = savegame.get_item("Use which book? ", false, true, true)
	if item_index == 99:
		Inventory.item_filter_useable_spell_book = false
		return
	if item_index == -2:
		Profile.instance.message_print("You have no %s books!" % [prayer])
		Inventory.item_filter_useable_spell_book = false
		return
	Inventory.item_filter_useable_spell_book = false
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var book_type: int = item.sub_category
	var use_set_two: bool = item.category == Util.realm_to_spellbook(player.realm_2)
	savegame.notice_update_redraw()
	var prompt: String = "recite" if player.spellcasting.casting_type == CastingType.DIVINE else "cast"
	var spell_index: int = get_spell(prompt, book_type, true, use_set_two)
	if spell_index == 99:
		return
	if spell_index == -2:
		Profile.instance.message_print("You don't know any %ss in that book." % [prayer])
		return
	var spell: Spell = player.spellcasting.spells[1].spells[spell_index] if use_set_two else player.spellcasting.spells[0].spells[spell_index]
	if spell.power_cost > player.power:
		var cast: String = "recite" if player.spellcasting.casting_type == CastingType.DIVINE else "cast"
		Profile.instance.message_print("You do not have enough power to %s this %s." % [cast, prayer])
		if not User.get_check("Attempt it anyway? "):
			return
	var chance: int = spell.failure_chance(player)
	if Rng.roll_die(100) <= chance:
		Profile.instance.message_print("You failed to get the %s off!" % [prayer])
		if item.category == ItemCategory.CHAOS_BOOK and Rng.roll_die(100) < spell_index:
			Profile.instance.message_print("You produce a chaotic effect!")
			wild_magic(spell_index)
		elif item.category == ItemCategory.DEATH_BOOK and Rng.roll_die(100) < spell_index:
			if book_type == 3 and Rng.roll_die(2) == 1:
				dungeon_level.dungeon_monsters.monsters[0].sanity_blast(true)
			else:
				Profile.instance.message_print("It hurts!")
				player.take_hit(Rng.roll_dice(item.sub_category + 1, 6), "a miscast Death spell")
				if spell_index > 15 and Rng.roll_die(6) == 1 and not player.has_hold_life:
					player.lose_experience(spell_index * 250)
	else:
		spell.cast(savegame, player, dungeon_level)
		if not spell.worked:
			var e: int = spell.first_cast_experience
			spell.worked = true
			player.gain_experience(e * spell.level)
	savegame.energy_use = 100
	if spell.power_cost <= player.power:
		player.power -= spell.power_cost
	else:
		var oops: int = spell.power_cost - player.power
		player.power = 0
		player.fractional_power = 0
		Profile.instance.message_print("You faint from the effort!")
		player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die((5 * oops) + 1))
		if Rng.roll_die(100) <= 50:
			var perm: bool = Rng.roll_die(100) <= 25
			Profile.instance.message_print("You have damaged your health!")
			player.decrease_ability_score(Ability.CONSTITUTION, 15 + Rng.roll_die(10), perm)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)


func use_mentalism_power() -> void:
	var plev: int = player.level
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused!")
		return
	var n: int = get_mentalism_talent()
	if n < 0:
		return
	var talent: Talent = player.spellcasting.talents.talents[n]
	if talent.power_cost > player.power:
		Profile.instance.message_print("You do not have enough vis to use this talent.")
		if not User.get_check("Attempt it anyway? "):
			return
	var chance: int = talent.failure_chance(player)
	if Rng.roll_die(100) <= chance:
		Profile.instance.message_print("You failed to concentrate hard enough!")
		if Rng.roll_die(100) < chance / 2:
			var i: int = Rng.roll_die(100)
			if i < 5:
				Profile.instance.message_print("Oh, no! Your mind has gone blank!")
				savegame.spell_effects_handler.lose_all_info()
			elif i < 15:
				Profile.instance.message_print("Weird visions seem to dance before your eyes...")
				player.set_timed_hallucinations(player.timed_hallucinations + 5 + Rng.roll_die(10))
			elif i < 45:
				Profile.instance.message_print("Your brain is addled!")
				player.set_timed_confusion(player.timed_confusion + Rng.roll_die(8))
			elif i < 90:
				player.set_timed_stun(player.timed_stun + Rng.roll_die(8))
			else:
				Profile.instance.message_print("Your mind unleashes its power in an uncontrollable storm!")
				savegame.spell_effects_handler.project(1, 2 + (plev / 10), player.map_row, player.map_col, plev * 2, ProjectilePower.new(savegame.spell_effects_handler), ProjectionFlag.JUMP | ProjectionFlag.KILL | ProjectionFlag.GRID | ProjectionFlag.ITEM)
				player.power = maxi(0, player.power - (plev * maxi(1, plev / 10)))
	else:
		talent.use(player, dungeon_level, savegame)
	savegame.energy_use = 100
	if talent.power_cost <= player.power:
		player.power -= talent.power_cost
	else:
		var oops: int = talent.power_cost - player.power
		player.power = 0
		player.fractional_power = 0
		Profile.instance.message_print("You faint from the effort!")
		player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die((5 * oops) + 1))
		if Rng.roll_die(100) <= 50:
			var perm: bool = Rng.roll_die(100) <= 25
			Profile.instance.message_print("You have damaged your mind!")
			player.decrease_ability_score(Ability.WISDOM, 15 + Rng.roll_die(10), perm)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)


func get_mentalism_talent() -> int:
	var num: int = 0
	var y: int = 1
	var x: int = 20
	var plev: int = player.level
	var p: String = "talent"
	var flag: bool = false
	var redraw: bool = false
	var talent_index: int = -1
	var talents: Array[Talent] = player.spellcasting.talents.talents
	for i: int in talents.size():
		if talents[i].level <= plev:
			num += 1
	var prompt: String = "(%ss a-%s, *=List, ESC=exit) Use which %s? " % [p, Util.index_to_label(num - 1), p]
	while not flag:
		var choice: String = User.get_simple_choice(prompt)
		if choice == "":
			break
		if choice == ' ' or choice == '*' or choice == '?':
			if not redraw:
				redraw = true
				Console.save_screen()
				Console.clear_to_row_end(y, x)
				Console.print_at(y, x + 5, "Name", Ink.WHITE)
				Console.print_at(y, x + 35, "Lv Vis Fail Info", Ink.WHITE)
				var row: int = 0
				for i: int in talents.size():
					var talent: Talent = talents[i]
					if talent.level > plev:
						break
					var psi_desc: String = "  %s) %s" % [Util.index_to_label(i), talent.summary_line(player)]
					row += 1
					Console.print_line_at(y + i + 1, x, psi_desc, Ink.WHITE)
				Console.clear_to_row_end(y + row + 1, x)
			else:
				redraw = false
				Console.restore_screen()
			continue
		var i: int = Util.label_to_index(choice.to_lower())
		if i < 0 or i >= num:
			continue
		talent_index = i
		flag = true
	if redraw:
		Console.restore_screen()
	if not flag:
		return -1
	return talent_index


func wild_magic(spell: int) -> void:
	var counter: int = 0
	var type: int = SummonType.SUMMON_BIZARRE_1 - 1 + Rng.roll_die(6)
	if type < SummonType.SUMMON_BIZARRE_1:
		type = SummonType.SUMMON_BIZARRE_1
	elif type > SummonType.SUMMON_BIZARRE_6:
		type = SummonType.SUMMON_BIZARRE_6
	match Rng.roll_die(spell) + Rng.roll_die(8) + 1:
		1, \
		2, \
		3:
			savegame.spell_effects_handler.teleport_player(10)
		4, \
		5, \
		6:
			savegame.spell_effects_handler.teleport_player(100)
		7, \
		8:
			savegame.spell_effects_handler.teleport_player(200)
		9, \
		10, \
		11:
			savegame.spell_effects_handler.unlight_area(10, 3)
		12, \
		13, \
		14:
			savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 3), 2)
		15:
			savegame.spell_effects_handler.destroy_doors_touch()
		16, \
		17:
			savegame.spell_effects_handler.wall_breaker()
		18:
			savegame.spell_effects_handler.sleep_monsters_touch()
		19, \
		20:
			savegame.spell_effects_handler.trap_creation()
		21, \
		22:
			savegame.spell_effects_handler.door_creation()
		23, \
		24, \
		25:
			savegame.spell_effects_handler.aggravate_monsters(1)
		26:
			savegame.spell_effects_handler.earthquake(player.MapY, player.MapX, 5)
		27, \
		28:
			player.genome.gain_mutation()
		29, \
		30:
			savegame.spell_effects_handler.apply_disenchant()
		31:
			savegame.spell_effects_handler.lose_all_info()
		32:
			savegame.spell_effects_handler.fire_ball(ProjectileChaos.new(savegame.spell_effects_handler), 0, spell + 5, 1 + (spell / 10))
		33:
			savegame.spell_effects_handler.wall_stone()
		34, \
		35:
			while counter < 8:
				counter += 1
				dungeon_level.dungeon_monsters.summon_specific(
					player.map_row,
					player.map_col,
					savegame.total_difficulty() * 3 / 2,
					type,
				)
		36, \
		37:
			savegame.spell_effects_handler.activate_hi_summons()
		38:
			savegame.spell_effects_handler.summon_reaver()
		_:
			savegame.activate_dread_curse()
