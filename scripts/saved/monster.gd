# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Monster
extends Resource

const OBJECTIVE: int = 0x01 # Objective (or Reflexive)
const POSSESSIVE: int = 0x02 # Possessive (or Reflexive)
const INDEFINITE_HIDDEN: int = 0x04 # Use indefinites for hidden monsters ("something")
const INDEFINITE_VISIBLE: int = 0x08 # Use indefinites for visible monsters ("a kobold")
const HIDDEN_PRONOUN: int = 0x10 # Pronominalize hidden monsters
const VISIBLE_PRONOUN: int = 0x20 # Pronominalize visible monsters
const ASSUME_HIDDEN: int = 0x40 # Assume the monster is hidden
const ASSUME_VISIBLE: int = 0x80 # Assume the monster is visible

const FUNNY_COMMENTS: Array[String] = ["Wow, cosmic, man!", "Rad!", "Groovy!", "Cool!", "Far out!"]

const FUNNY_DESC: Array[String]  =["silly", "hilarious", "absurd", "insipid", "ridiculous", "laughable", "ludicrous", "far-out", "groovy",
	"postmodern", "fantastic", "dadaistic", "cubistic", "cosmic", "awesome", "incomprehensible", "fabulous",
	"amazing", "incredible", "chaotic", "wild", "preposterous"]

const HORROR_DESC: Array[String] = ["abominable", "abysmal", "appalling", "baleful", "blasphemous", "disgusting", "dreadful", "filthy",
	"grisly", "hideous", "hellish", "horrible", "infernal", "loathsome", "nightmarish", "repulsive",
	"sacrilegious", "terrible", "unclean", "unspeakable"]


@export var confusion_level: int
@export var distance_from_player: int
@export var energy: int
@export var fear_level: int
@export var first_held_item_index: int
@export var generation: int
@export var health: int
@export var individual_monster_flags: FlagSet
@export var is_visible: bool
@export var map_row: int
@export var map_col: int
@export var max_health: int
@export var mind: FlagSet
@export var monster_type: MonsterType
@export var sleep_level: int
@export var speed: int
@export var stolen_gold: int
@export var stun_level: int


func _init() -> void:
	mind = FlagSet.new()
	individual_monster_flags = FlagSet.new()


# Useful Modes:
# 0x00 --> Full nominative name ("the kobold") or "it"
# 0x04 --> Full nominative name ("the kobold") or "something"
# 0x80 --> Genocide resistance name ("the kobold")
# 0x88 --> Killing name ("a kobold")
# 0x22 --> Possessive, genderized if visable ("his") or "its"
# 0x23 --> Reflexive, genderized if visable ("himself") or "itself"
func describe(mode: int) -> String:
	if monster_type == null:
		return ""
	var desc: String
	var name: String = monster_type.name
	if SaveGame.instance.player.timed_hallucinations != 0:
		var types: MonsterTypeArray = Profile.instance.monster_types
		var hallucinated_type: MonsterType = types.types[Rng.roll_die(types.types.size())]
		while hallucinated_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			hallucinated_type = types.types[Rng.roll_die(types.types.size())]
		var silly_name: String = hallucinated_type.name
		name = silly_name
	var currently_visible: bool = (mode & ASSUME_VISIBLE) != 0 or ((mode & ASSUME_HIDDEN) == 0 and is_visible)
	var use_pronoun: bool = (currently_visible and (mode & VISIBLE_PRONOUN) != 0) or (not currently_visible and (mode & HIDDEN_PRONOUN) != 0)
	if not currently_visible or use_pronoun:
		var kind: int = 0x00
		if monster_type.flags_1.is_set(MonsterFlags1.FEMALE):
			kind = 0x20
		elif monster_type.flags_1.is_set(MonsterFlags1.MALE):
			kind = 0x10
		if not use_pronoun:
			kind = 0x00
		var res: String = "it"
		match kind + (mode & 0x07):
			0x00:
				res = "it"
			0x01:
				res = "it"
			0x02:
				res = "its"
			0x03:
				res = "itself"
			0x04:
				res = "something"
			0x05:
				res = "something"
			0x06:
				res = "something's"
			0x07:
				res = "itself"
			0x10:
				res = "he"
			0x11:
				res = "him"
			0x12:
				res = "his"
			0x13:
				res = "himself"
			0x14:
				res = "someone"
			0x15:
				res = "someone"
			0x16:
				res = "someone's"
			0x17:
				res = "himself"
			0x20:
				res = "she"
			0x21:
				res = "her"
			0x22:
				res = "her"
			0x23:
				res = "herself"
			0x24:
				res = "someone"
			0x25:
				res = "someone"
			0x26:
				res = "someone's"
			0x27:
				res = "herself"
		desc = res
	elif (mode & POSSESSIVE) != 0 and (mode & OBJECTIVE) != 0:
		if monster_type.flags_1.is_set(MonsterFlags1.FEMALE):
			desc = "herself"
		elif monster_type.flags_1.is_set(MonsterFlags1.MALE):
			desc = "himself"
		else:
			desc = "itself"
	else:
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) and SaveGame.instance.player.timed_hallucinations == 0:
			desc = name
		elif (mode & INDEFINITE_VISIBLE) != 0:
			desc = "an " if Util.is_vowel(name[0]) else "a "
			desc += name
		else:
			desc = "your " if mind.is_set(MonsterMind.FRIENDLY) else "the "
			desc += name
		if (mode & POSSESSIVE) != 0:
			desc += "'s"
	return desc


func sanity_blast (from_necronomicon: bool) -> void:
	var player: Player = SaveGame.instance.player
	var happened: bool = false
	var power: int = 100
	if from_necronomicon:
		Profile.instance.message_print("Your sanity is shaken by reading the Necronomicon!")
	else:
		power = monster_type.level + 10
		var monster_name: String = describe(0)
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			power *= 2
		else:
			if monster_type.flags_1.is_set(MonsterFlags1.FRIENDS):
				power /= 2
		if not SaveGame.instance.san_checks_allowed:
			return
		if not is_visible:
			return
		if monster_type.flags_2.is_clear(MonsterFlags2.ELDRITCH_HORROR):
			return
		if mind.is_set(MonsterMind.FRIENDLY) and Rng.roll_die(8) != 1:
			return
		if Rng.roll_die(power) < player.skill_saving_throw:
			return
		if player.timed_hallucinations != 0:
			Profile.instance.message_print("You behold the %s visage of %s!" % [FUNNY_DESC[Rng.roll_die(FUNNY_DESC.size())-1], monster_name])
			if Rng.roll_die(3) == 1:
				Profile.instance.message_print(FUNNY_COMMENTS[Rng.roll_die(FUNNY_COMMENTS.size())-1])
				player.timed_hallucinations += Rng.roll_die(monster_type.level)
			return
		Profile.instance.message_print("You behold the %s visage of %s!" % [HORROR_DESC[Rng.roll_die(HORROR_DESC.size())-1], monster_name])
		monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.ELDRITCH_HORROR)
		if player.ancestry.sanity_immune:
			Profile.instance.message_print("You've seen worse.")
			return
		if player.ancestry.sanity_resistant:
			if Rng.roll_die(100) < 25 + player.level:
				Profile.instance.message_print("You've seen worse.")
				return
	if Rng.roll_die(power) < player.skill_saving_throw:
		if not player.has_confusion_resistance:
			player.set_timed_confusion(player.timed_confusion + Rng.roll_die(4) + 3)
		if not player.has_chaos_resistance and Rng.roll_die(3) == 1:
			player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(250) + 149)
		return
	if Rng.roll_die(power) < player.skill_saving_throw:
		player.try_decreasing_ability_score(Ability.INTELLIGENCE)
		player.try_decreasing_ability_score(Ability.WISDOM)
		return
	if Rng.roll_die(power) < player.skill_saving_throw:
		if not player.has_confusion_resistance:
			player.set_timed_confusion(player.timed_confusion + Rng.roll_die(4) + 3)
		if not player.has_free_action:
			player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(4) + 3)
		while Rng.roll_die(100) > player.skill_saving_throw:
			player.try_decreasing_ability_score(Ability.INTELLIGENCE)
		while Rng.roll_die(100) > player.skill_saving_throw:
			player.try_decreasing_ability_score(Ability.WISDOM)
		if not player.has_chaos_resistance:
			player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(250) + 149)
		return
	if Rng.roll_die(power) < player.skill_saving_throw:
		if player.decrease_ability_score(Ability.INTELLIGENCE, 10, true):
			happened = true
		if player.decrease_ability_score(Ability.WISDOM, 10, true):
			happened = true
		if happened:
			Profile.instance.message_print("You feel much less sane than before.")
		return
	if Rng.roll_die(power) < player.skill_saving_throw:
		if SaveGame.instance.spell_effects_handler.lose_all_info():
			Profile.instance.message_print("You forget everything in your utmost terror!")
		return
	if Rng.roll_die(power) < player.skill_saving_throw:
		if not player.has_chaos_resistance:
			Profile.instance.message_print("The exposure to eldritch forces warps you.")
			player.genome.gain_mutation()
			player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
			SaveGame.instance.notice_update_redraw()
