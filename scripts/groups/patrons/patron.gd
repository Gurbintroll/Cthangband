@abstract # Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Patron
extends Resource

static var _patrons: Dictionary[String, Patron]

@export var preferred_ability: int
@export var rewards: Array[int]
@export var short_name: String
@export var long_name: String
@export var multiple_rewards: bool


static func _populate_patrons() -> void:
	_patrons = { }
	_patrons.get_or_add("Abhoth", PatronAbhoth.new())
	_patrons.get_or_add("Azathoth", PatronAzathoth.new())
	_patrons.get_or_add("Cthulhu", PatronCthulhu.new())
	_patrons.get_or_add("Cyaegha", PatronCyaegha.new())
	_patrons.get_or_add("Eihort", PatronEihort.new())
	_patrons.get_or_add("Glaaki", PatronGlaaki.new())
	_patrons.get_or_add("Hastur", PatronHastur.new())
	_patrons.get_or_add("Iod", PatronIod.new())
	_patrons.get_or_add("Nyarlathotep", PatronNyarlathotep.new())
	_patrons.get_or_add("Nyogtha", PatronNyogtha.new())
	_patrons.get_or_add("Rhan-Tegoth", PatronRhanTegoth.new())
	_patrons.get_or_add("Shub Niggurath", PatronShubNiggurath.new())
	_patrons.get_or_add("Tsathoggua", PatronTsathoggua.new())
	_patrons.get_or_add("Ubbo-Sathla", PatronUbboSathla.new())
	_patrons.get_or_add("Yig", PatronYig.new())
	_patrons.get_or_add("Yog Sothoth", PatronYogSothoth.new())


static func named_patron(name: String) -> Patron:
	if _patrons.size() == 0:
		_populate_patrons()
	return _patrons[name]


static func random_patron() -> Patron:
	if _patrons.size() == 0:
		_populate_patrons()
	return _patrons[_patrons.keys()[Rng.roll_die(_patrons.size())-1]]


static func sorted_patron_names() -> Array[String]:
	if _patrons.size() == 0:
		_populate_patrons()
	var keys: Array[String] = _patrons.keys()
	keys.sort()
	return keys


## Give us a reward from our patron Great Old One
func get_reward(save_game: SaveGame, player: Player, level: DungeonLevel) -> void:
	var type: int
	var wrath_reason: String = "the wrath of %s" % [short_name]
	# 1-in-6 chance of getting a mutation instead of a reward
	if Rng.roll_die(6) == 1:
		Profile.instance.message_print("%s rewards you with a mutation!" % [short_name])
		player.genome.gain_mutation()
		return
	# Basic chance of possibly getting a bad reward is 1-in-6
	var nasty_chance: int = 6
	# If we're in the middle of getting a reward, don't get another (e.g., if the reward increases our level
	if multiple_rewards:
		return
	multiple_rewards = true
	# If we're level 13 then we have a 1-in-2 chance of possibly getting a bad reward
	if player.level == 13:
		nasty_chance = 2
	# If we're level 26 or 39, then we have a 1-in-3 chance of possibly getting a bad reward
	elif player.level % 13 == 0:
		nasty_chance = 3
	# If we're level 14, 28 or 42, then we have a 1-in-12 chance of possibly getting a bad reward
	elif player.level % 14 == 0:
		nasty_chance = 12
	# Choose a reward
	if Rng.roll_die(nasty_chance) == 1:
		# Possibly getting a bad reward doesn't guarantee it will be bad - three quarters of them are good
		type = Rng.roll_die(20)
	else:
		# Just give us a good reward, excluding the bad ones from the roll
		type = Rng.roll_die(15) + 5
	# Rewards array is zero-based and die rolls are one-based
	var effect: int = rewards[type - 1]
	match effect:
		# Polymorph the player
		Reward.POLY_SLF:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thou needst a new form, mortal!'")
			player.polymorph_self()
		# Award the player a chunk of experience points
		Reward.GAIN_EXP:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Well done, mortal! Lead on!'")
			if player.experience_points < Constants.MAX_EXPERIENCE:
				var ee: int = player.experience_points / 2 + 10
				if ee > 100000:
					ee = 100000
				Profile.instance.message_print("You feel more experienced.")
				player.gain_experience(ee)
		# Take a chunk of experience points away from the player
		Reward.LOSE_EXP:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thou didst not deserve that, slave.'")
			player.lose_experience(player.experience_points / 6)
		# Give the player a good object
		Reward.GOOD_OBJ:
			Profile.instance.message_print("The voice of %s whispers:" % [short_name])
			Profile.instance.message_print("'Use my gift wisely.'")
			level.acquirement(player.map_row, player.map_col, 1, false)
		# Give the player a great object
		Reward.GREA_OBJ:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Use my gift wisely.'")
			level.acquirement(player.map_row, player.map_col, 1, true)
		# Give the player a chaotic blade
		Reward.CHAOS_WP:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thine deeds hath earned thee a worthy blade.'")
			var item: Item = Item.new()
			var sword_type: int
			match Rng.roll_die(player.level):
				1, 2:
					sword_type = SwordType.DAGGER
				3, 4:
					sword_type = SwordType.MAIN_GAUCHE
				5, 6:
					sword_type = SwordType.RAPIER
				7, 8:
					sword_type = SwordType.SMALL_SWORD
				9, 10:
					sword_type = SwordType.SHORT_SWORD
				11, 12, 13:
					sword_type = SwordType.SABRE
				14, 15, 16:
					sword_type = SwordType.CUTLASS
				17:
					sword_type = SwordType.TULWAR
				18, 19, 20:
					sword_type = SwordType.BROAD_SWORD
				21, 22, 23:
					sword_type = SwordType.LONG_SWORD
				24, 25, 26:
					sword_type = SwordType.SCIMITAR
				27:
					sword_type = SwordType.KATANA
				28, 29:
					sword_type = SwordType.BASTARD_SWORD
				30, 31:
					sword_type = SwordType.TWO_HANDED_SWORD
				32:
					sword_type = SwordType.EXECUTIONERS_SWORD
				_:
					sword_type = SwordType.BLADE_OF_CHAOS
			item.assign_item_type(SaveGame.instance.item_types.look_up_type(ItemCategory.SWORD, sword_type))
			item.bonus_to_hit = 3 + Rng.roll_die(save_game.total_difficulty()) % 10
			item.bonus_damage = 3 + Rng.roll_die(save_game.total_difficulty()) % 10
			item.apply_random_resistance(Rng.roll_die(34) + 4)
			item.rare_index = RareItemId.WEAPON_CHAOTIC
			level.dungeon_items.drop_near(item, -1, player.map_row, player.map_col)
		# Give the player 1d2+1 good objects
		Reward.GOOD_OBS:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thine deeds hath earned thee a worthy reward.'")
			level.acquirement(player.map_row, player.map_col, Rng.roll_die(2) + 1, false)
		# Give the player 1d2+1 great objects
		Reward.GREA_OBS:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Behold, mortal, how generously I reward thine loyalty.'")
			level.acquirement(player.map_row, player.map_col, Rng.roll_die(2) + 1, true)
		Reward.DREAD_CURSE:
			Profile.instance.message_print("The voice of %s thunders:" % [short_name])
			Profile.instance.message_print("'Thou art growing arrogant, mortal!'")
			save_game.activiate_dread_curse()
		Reward.SUMMON_M:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'My pets, destroy the arrogant mortal!'")
			for i: int in Rng.roll_die(5) + 1:
				level.dungeon_monsters.summon_specific(player.map_row, player.map_col, save_game.total_difficulty(), 0)
		Reward.H_SUMMON:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thou needst worthier opponents!'")
			save_game.spell_effects_handler.activate_hi_summons()
		Reward.DO_HAVOC:
			Profile.instance.message_print("The voice of %s whispers:" % [short_name])
			Profile.instance.message_print("'Death and destruction! This pleaseth me!'")
			save_game.spell_effects_handler.call_chaos()
		Reward.GAIN_ABL:
			Profile.instance.message_print("The voice of %s rings out:" % [short_name])
			Profile.instance.message_print("'Stay, mortal, and let me mould thee.'")
			if preferred_ability >= 0 and Rng.roll_die(3) == 1:
				player.try_increasing_ability_score(preferred_ability)
			else:
				player.try_increasing_ability_score(Rng.roll_die(6) - 1)
		Reward.LOSE_ABL:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'I grow tired of thee, mortal.'")
			if preferred_ability >= 0 and Rng.roll_die(3) == 1:
				player.try_decreasing_ability_score(preferred_ability)
			else:
				player.try_decreasing_ability_score(Rng.roll_die(6) - 1)
		Reward.RUIN_ABL:
			Profile.instance.message_print("The voice of %s thunders:" % [short_name])
			Profile.instance.message_print("'Thou needst a lesson in humility, mortal!'")
			Profile.instance.message_print("'You feel less powerful!'")
		Reward.POLY_WND:
			Profile.instance.message_print("You feel the power of %s touch you:" % [short_name])
			player.polymorph_wounds()
		Reward.AUGM_ABL:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Receive this modest gift from me.'")
			for i: int in 6:
				player.try_increasing_ability_score(i)
		Reward.HURT_LOT:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Suffer, pathetic fool!'")
			save_game.spell_effects_handler.fire_ball(ProjectileDisintegrate.new(save_game.spell_effects_handler), 0, player.level * 4, 4)
			player.take_hit(player.level * 4, wrath_reason)
		Reward.HEAL_FUL:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Rise, my servant!'")
			player.restore_level()
			player.set_timed_poison(0)
			player.set_timed_blindness(0)
			player.set_timed_confusion(0)
			player.set_timed_hallucinations(0)
			player.set_timed_stun(0)
			player.set_timed_bleeding(0)
			player.restore_health(5000)
			for i: int in 6:
				player.try_restoring_ability_score(i)
		Reward.CURSE_WP:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thou reliest too much on thine weapon!'")
			save_game.command_engine.curse_weapon()
		Reward.CURSE_AR:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Thou reliest too much on thine armour!'")
			save_game.command_engine.curse_armour()
		Reward.PISS_OFF:
			Profile.instance.message_print("The voice of %s whispers:" % [short_name])
			Profile.instance.message_print("'Now thou shalt pay for annoying me.'")
			match Rng.roll_die(4):
				1:
					save_game.activate_dread_curse()
				2:
					save_game.spell_effects_handler.activate_hi_summons()
				3:
					if Rng.roll_die(2) == 1:
						save_game.command_engine.curse_weapon()
					else:
						save_game.command_engine.curse_armour()
				4:
					for i: int in 6:
						player.decrease_ability_score(i, 10 + Rng.roll_die(15), true)
		Reward.WRATH:
			Profile.instance.message_print("The voice of %s thunders:" % [short_name])
			Profile.instance.message_print("'Die, mortal!'")
			for i: int in 6:
				player.decrease_ability_score(i, 10 + Rng.roll_die(15), true)
			save_game.spell_effects_handler.activate_hi_summons()
			save_game.activate_dread_curse()
			if Rng.roll_die(2) == 1:
				save_game.command_engine.curse_weapon()
			if Rng.roll_die(2) == 1:
				save_game.command_engine.curse_armour()
		Reward.DESTRUCT:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Death and destruction! This pleaseth me!'")
			save_game.spell_effects_handler.destroy_area(player.map_row, player.map_col, 25)
		Reward.CARNAGE:
			Profile.instance.message_print("The voice of %s booms out:" % [short_name])
			Profile.instance.message_print("'Let me relieve thee of thine oppressors!'")
			save_game.spell_effects_handler.carnage(false)
		Reward.MASS_GEN:
			Profile.instance.message_print("The voice of %s rings out:" % [short_name])
			Profile.instance.message_print("'Let me relieve thee of thine oppressors!'")
			save_game.spell_effects_handler.mass_carnage(false)
		Reward.DISPEL_C:
			Profile.instance.message_print("You can feel the power of %s assault your enemies!" % [short_name])
			save_game.spell_effects_handler.dispel_monster(player.level * 4)
		Reward.IGNORE:
			Profile.instance.message_print("%s ignores you." % [short_name])
		Reward.SER_DEMO:
			Profile.instance.message_print("%s rewards you with a demonic servant!" % [short_name])
			if not level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, save_game.total_difficulty(), SummonType.SUMMON_DEMON, false):
				Profile.instance.message_print("Nobody ever turns up...")
		Reward.SER_MONS:
			Profile.instance.message_print("%s rewards you with a servant!" % [short_name])
			if not level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, save_game.total_difficulty(), SummonType.SUMMON_NO_UNIQUES, false):
				Profile.instance.message_print("Nobody ever turns up...")
		Reward.SER_UNDE:
			Profile.instance.message_print("%s rewards you with a servant!" % [short_name])
			if not level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, save_game.total_difficulty(), SummonType.SUMMON_UNDEAD, false):
				Profile.instance.message_print("Nobody ever turns up...")
		_:
			Profile.instance.message_print("The voice of %s stammers:" % [short_name])
			Profile.instance.message_print("The answer is %d/%d. What's the question?" % [type, effect])
