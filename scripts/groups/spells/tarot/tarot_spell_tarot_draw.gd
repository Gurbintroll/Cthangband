# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TarotSpellTarotDraw
extends Spell


func _init() -> void:
	default_base_failure = 75
	default_level = 5
	default_power_cost = 5
	first_cast_experience = 8
	name = "Tarot Draw"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var none_came: bool = false
	var die: int = Rng.roll_die(110) + player.level / 5
	Profile.instance.message_print("You shuffle your tarot deck and draw a card...")
	if die < 7:
		Profile.instance.message_print("Oh no! It's the Blasted Tower!")
		for i: int in Rng.roll_die(3):
			savegame.instance.spell_effects_handler.activate_hi_summons()
	elif die < 14:
		Profile.instance.message_print("Oh no! It's the Devil!")
		if not dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DEMON):
			none_came = true
	elif die < 18:
		Profile.instance.message_print("Oh no! It's the Hanged Man!")
		savegame.activate_dread_curse()
	elif die < 22:
		Profile.instance.message_print("It's the Swords of Discord.")
		savegame.spell_effects_handler.aggravate_monsters(1)
	elif die < 26:
		Profile.instance.message_print("It's the Fool.")
		player.try_decreasing_ability_score(Ability.INTELLIGENCE)
		player.try_decreasing_ability_score(Ability.WISDOM)
	elif die < 30:
		Profile.instance.message_print("It's a picture of a strange monster.")
		if not dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), 32 + Rng.roll_die(6)):
			none_came = true
	elif die < 33:
		Profile.instance.message_print("It's the Moon.")
		savegame.spell_effects_handler.unlight_area(10, 3)
	elif die < 38:
		Profile.instance.message_print("It's the Wheel of Fortune")
		wild_magic(Rng.roll_die(32) - 1, player, dungeon_level, savegame)
	elif die < 40:
		Profile.instance.message_print("It's a teleport card.")
		savegame.spell_effects_handler.teleport_player(10)
	elif die < 42:
		Profile.instance.message_print("It's the Star")
		player.set_timed_blessing(player.timed_blessing + player.level)
	elif die < 47:
		Profile.instance.message_print("It's a teleport card.")
		savegame.spell_effects_handler.teleport_player(100)
	elif die < 52:
		Profile.instance.message_print("It's a teleport card.")
		savegame.spell_effects_handler.teleport_player(200)
	elif die < 60:
		Profile.instance.message_print("It's the Tower.")
		savegame.spell_effects_handler.wall_breaker()
	elif die < 72:
		Profile.instance.message_print("It's Temperance.")
		savegame.instance.spell_effects_handler.sleep_monsters_touch()
	elif die < 80:
		Profile.instance.message_print("It's the Tower.")
		savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 5)
	elif die < 82:
		Profile.instance.message_print("It's a picture of a friendly monster.")
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty() * 3 / 2, SummonType.SUMMON_BIZARRE_1, false):
			none_came = true
	elif die < 84:
		Profile.instance.message_print("It's a picture of a friendly monster.")
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty() * 3 / 2, SummonType.SUMMON_BIZARRE_2, false):
			none_came = true
	elif die < 86:
		Profile.instance.message_print("It's a picture of a friendly monster.")
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty() * 3 / 2, SummonType.SUMMON_BIZARRE_4, false):
			none_came = true
	elif die < 88:
		Profile.instance.message_print("It's a picture of a friendly monster.")
		if not dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty() * 3 / 2, SummonType.SUMMON_BIZARRE_5, false):
			none_came = true
	elif die < 96:
		Profile.instance.message_print("It's the Lovers.")
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		savegame.spell_effects_handler.charm_monster(dir, mini(player.level, 20))
	elif die < 101:
		Profile.instance.message_print("It's the Hermit.")
		savegame.spell_effects_handler.wall_stone()
	elif die < 111:
		Profile.instance.message_print("It's Judgement.")
		player.reroll_hitpoints()
		if player.genome.has_mutations():
			Profile.instance.message_print("You are cured of all mutations.")
			player.genome.lose_all_mutations()
			player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
			savegame.notice_update_redraw()
	elif die < 120:
		Profile.instance.message_print("It's the Sun.")
		dungeon_level.wiz_light()
	else:
		Profile.instance.message_print("It's the World.")
		if player.experience_points < Constants.MAX_EXPERIENCE:
			var old_exp: int = player.experience_points
			var add: int = min(old_exp / 25 + 1, 5000)
			Profile.instance.message_print("You feel more experienced.")
			player.gain_experience(add)
	if none_came:
		Profile.instance.message_print("No-ne ever turns up.")


func comment(player: Player) -> String:
	return ""


func wild_magic(spell: int, player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	match Rng.roll_die(spell) + Rng.roll_die(8) + 1:
		1, 2, 3:
			savegame.spell_effects_handler.teleport_player(10)
		4, 5, 6:
			savegame.spell_effects_handler.teleport_player(100)
		7, 8:
			savegame.spell_effects_handler.teleport_player(200)
		9, 10, 11:
			savegame.spell_effects_handler.unlight_area(10, 3)
		12, 13, 14:
			savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 3), 2)
		15:
			savegame.spell_effects_handler.destroy_doors_touch()
		16, 17:
			savegame.spell_effects_handler.wall_breaker()
		18:
			savegame.spell_effects_handler.sleep_monsters_touch()
		19, 20:
			savegame.spell_effects_handler.trap_creation()
		21, 22:
			savegame.spell_effects_handler.door_creation()
		23, 24, 25:
			savegame.spell_effects_handler.aggravate_monsters(1)
		26:
			savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 5)
		27, 28:
			player.genome.gain_mutation()
		29, 30:
			savegame.spell_effects_handler.apply_disenchantment()
		31:
			savegame.spell_effects_handler.lose_all_info()
		32:
			savegame.spell_effects_handler.fire_ball(ProjectileChaos.new(savegame.spell_effects_handler), 0, spell + 5, 1 + (spell / 10))
		33:
			savegame.spell_effects_handler.wall_stone()
		34, 35:
			var type: int = SummonType.SUMMON_BIZARRE_1 - 1 + Rng.roll_die(6)
			for i: int in 8:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty() * 3 / 2, type)
		36, 37:
			savegame.spell_effects_handler.activate_hi_summons()
		38:
			savegame.spell_effects_handler.summon_reaver()
		_:
			savegame.activate_dread_curse()
