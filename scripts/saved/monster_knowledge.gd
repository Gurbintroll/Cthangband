# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterKnowledge
extends Resource

const PRONOUN: Array[String] = ["it", "he", "she"]
const PRONOUN_CAPITAL: Array[String] = ["It", "He", "She"]
const PRONOUN_POSSESSIVE: Array[String] = ["its", "his", "her"]

@export var r_blows: Array[int] = [0, 0, 0, 0]
@export var r_cast_innate: int
@export var r_cast_spell: int
@export var r_deaths: int
@export var r_drop_gold: int
@export var r_drop_item: int
@export var r_flags_1: FlagSet
@export var r_flags_2: FlagSet
@export var r_flags_3: FlagSet
@export var r_flags_4: FlagSet
@export var r_flags_5: FlagSet
@export var r_flags_6: FlagSet
@export var r_ignore: int
@export var r_kills: int
@export var r_probed: bool
@export var r_sights: int
@export var r_total_kills: int
@export var r_wake: int


func _init() -> void:
	r_flags_1 = FlagSet.new()
	r_flags_2 = FlagSet.new()
	r_flags_3 = FlagSet.new()
	r_flags_4 = FlagSet.new()
	r_flags_5 = FlagSet.new()
	r_flags_6 = FlagSet.new()


func display(monster_type: MonsterType) -> void:
	# Refresh the reference to the monster type, since it may have been broken
	# if we saved and re-loaded the game
	Profile.instance.message_print("")
	Console.clear_row(0)
	display_body(monster_type, 1, 0, Ink.WHITE)
	display_header(monster_type)


func display_body(monster_type: MonsterType, row:int, col:int, ink: int) -> void:
	var gender: int = 0
	var vp: Array[String] = []
	var knowledge: MonsterKnowledge = self
	var description: String = ""
	if SaveGame.instance.player.is_wizard:
		knowledge = MonsterKnowledge.new()
		for m: int in 4:
			if monster_type.attack[m].effect != 0 or monster_type.attack[m].method != 0:
				knowledge.r_blows[m] = 255
		knowledge.r_probed = true
		knowledge.r_wake = 255
		knowledge.r_ignore = 255
		knowledge.r_drop_item = (8 if monster_type.flags_1.is_set(MonsterFlags1.DROP_4D2) else 0) + \
				(6 if monster_type.flags_1.is_set(MonsterFlags1.DROP_3D2) else 0) + \
				(4 if monster_type.flags_1.is_set(MonsterFlags1.DROP_2D2) else 0) + \
				(2 if monster_type.flags_1.is_set(MonsterFlags1.DROP_1D2) else 0) + \
				(1 if monster_type.flags_1.is_set(MonsterFlags1.DROP_90) else 0) + \
				(1 if monster_type.flags_1.is_set(MonsterFlags1.DROP_60) else 0)
		knowledge.r_drop_gold = knowledge.r_drop_item
		if monster_type.flags_1.is_set(MonsterFlags1.ONLY_DROP_GOLD):
			knowledge.r_drop_item = 0
		if monster_type.flags_1.is_set(MonsterFlags1.ONLY_DROP_ITEM):
			knowledge.r_drop_gold = 0
		knowledge.r_cast_innate = 255
		knowledge.r_cast_spell = 255
		knowledge.r_flags_1 = monster_type.flags_1
		knowledge.r_flags_2 = monster_type.flags_2
		knowledge.r_flags_3 = monster_type.flags_3
		knowledge.r_flags_4 = monster_type.flags_4
		knowledge.r_flags_5 = monster_type.flags_5
		knowledge.r_flags_6 = monster_type.flags_6
	if monster_type.flags_1.is_set(MonsterFlags1.FEMALE):
		gender = 2
	elif monster_type.flags_1.is_set(MonsterFlags1.MALE):
		gender = 1
	var flags1: int = monster_type.flags_1.value & knowledge.r_flags_1.value
	var flags2: int = monster_type.flags_2.value & knowledge.r_flags_2.value
	var flags3: int = monster_type.flags_3.value & knowledge.r_flags_3.value
	var flags4: int = monster_type.flags_4.value & knowledge.r_flags_4.value
	var flags5: int = monster_type.flags_5.value & knowledge.r_flags_5.value
	var flags6: int = monster_type.flags_6.value & knowledge.r_flags_6.value
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		flags1 |= MonsterFlags1.UNIQUE
	if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		flags1 |= MonsterFlags1.GUARDIAN
	if monster_type.flags_1.is_set(MonsterFlags1.MALE):
		flags1 |= MonsterFlags1.MALE
	if monster_type.flags_1.is_set(MonsterFlags1.FEMALE):
		flags1 |= MonsterFlags1.FEMALE
	if monster_type.flags_1.is_set(MonsterFlags1.FRIENDS):
		flags1 |= MonsterFlags1.FRIENDS
	if monster_type.flags_1.is_set(MonsterFlags1.ESCORTED):
		flags1 |= MonsterFlags1.ESCORTED
	if monster_type.flags_1.is_set(MonsterFlags1.ESCORTS_GROUP):
		flags1 |= MonsterFlags1.ESCORTS_GROUP
	if knowledge.r_total_kills != 0 or knowledge.r_probed:
		if monster_type.flags_3.is_set(MonsterFlags3.ORC):
			flags3 |= MonsterFlags3.ORC
		if monster_type.flags_3.is_set(MonsterFlags3.TROLL):
			flags3 |= MonsterFlags3.TROLL
		if monster_type.flags_3.is_set(MonsterFlags3.GIANT):
			flags3 |= MonsterFlags3.GIANT
		if monster_type.flags_3.is_set(MonsterFlags3.DRAGON):
			flags3 |= MonsterFlags3.DRAGON
		if monster_type.flags_3.is_set(MonsterFlags3.DEMON):
			flags3 |= MonsterFlags3.DEMON
		if monster_type.flags_3.is_set(MonsterFlags3.CTHULOID):
			flags3 |= MonsterFlags3.CTHULOID
		if monster_type.flags_3.is_set(MonsterFlags3.UNDEAD):
			flags3 |= MonsterFlags3.UNDEAD
		if monster_type.flags_3.is_set(MonsterFlags3.EVIL):
			flags3 |= MonsterFlags3.EVIL
		if monster_type.flags_3.is_set(MonsterFlags3.GOOD):
			flags3 |= MonsterFlags3.GOOD
		if monster_type.flags_3.is_set(MonsterFlags3.ANIMAL):
			flags3 |= MonsterFlags3.ANIMAL
		if monster_type.flags_3.is_set(MonsterFlags3.GREAT_OLD_DONE):
			flags3 |= MonsterFlags3.GREAT_OLD_DONE
		if monster_type.flags_1.is_set(MonsterFlags1.FORCE_MAX_HP):
			flags1 |= MonsterFlags1.FORCE_MAX_HP
	var buf: String = monster_type.description
	description += buf
	description += " "
	var old: bool = false
	if monster_type.level == 0:
		description += PRONOUN_CAPITAL[gender]
		description += " lives in the town"
		old = true
	elif knowledge.r_total_kills != 0 or knowledge.r_probed:
		description += PRONOUN_CAPITAL[gender] + " is normally found at level " + str(monster_type.level)
		old = true
	if old:
		description += ", and "
	else:
		description += PRONOUN_CAPITAL[gender] + ' '
		old = true
	description += "moves"
	if (flags1 & MonsterFlags1.RANDOM_MOVE_50) != 0 or (flags1 & MonsterFlags1.RANDOM_MOVE_25) != 0:
		if (flags1 & MonsterFlags1.RANDOM_MOVE_50) != 0 and (flags1 & MonsterFlags1.RANDOM_MOVE_25) != 0:
			description += " extremely"
		elif (flags1 & MonsterFlags1.RANDOM_MOVE_50) != 0:
			description += " somewhat"
		elif (flags1 & MonsterFlags1.RANDOM_MOVE_25) != 0:
			description += " a bit"
		description += " erratically"
		if monster_type.speed != 110:
			description += ", and"
	if monster_type.speed > 110:
		if monster_type.speed > 130:
			description += " incredibly"
		elif monster_type.speed > 120:
			description += " very"
		description += " quickly"
		description += " (" + str(Tables.EXTRACT_ENERGY[monster_type.speed] / 10.0) + " actions per turn)"
	elif monster_type.speed < 110:
		if monster_type.speed < 90:
			description += " incredibly"
		elif monster_type.speed < 100:
			description += " very"
		description += " slowly"
		description += " (" + str(Tables.EXTRACT_ENERGY[monster_type.speed] / 10.0) + " actions per turn)"
	else:
		description += " at normal speed"
	if (flags1 & MonsterFlags1.NEVER_MOVE) != 0:
		if old:
			description += ", but "
		else:
			description += PRONOUN[gender] + ' '
			old = true
		description += "does not deign to chase intruders"
	if old:
		description += ". "
	var q: String
	var p: String
	if knowledge.r_total_kills != 0 or knowledge.r_probed:
		description += "Killing this" if (flags1 & MonsterFlags1.UNIQUE) != 0 else "A kill of this"
		if (flags2 & MonsterFlags2.ELDRITCH_HORROR) != 0:
			description += " sanity-blasting"
		if (flags3 & MonsterFlags3.ANIMAL) != 0:
			description += " natural"
		if (flags3 & MonsterFlags3.EVIL) != 0:
			description += " evil"
		if (flags3 & MonsterFlags3.GOOD) != 0:
			description += " good"
		if (flags3 & MonsterFlags3.UNDEAD) != 0:
			description += " undead"
		if (flags3 & MonsterFlags3.DRAGON) != 0:
			description += " dragon"
		elif (flags3 & MonsterFlags3.DEMON) != 0:
			description += " demon"
		elif (flags3 & MonsterFlags3.GIANT) != 0:
			description += " giant"
		elif (flags3 & MonsterFlags3.TROLL) != 0:
			description += " troll"
		elif (flags3 & MonsterFlags3.ORC) != 0:
			description += " orc"
		elif (flags3 & MonsterFlags3.GREAT_OLD_DONE) != 0:
			description += " Great Old One"
		else:
			description += " creature"
		var i: int = monster_type.experience * monster_type.level / SaveGame.instance.player.level
		var j: int = ((monster_type.experience * monster_type.level % SaveGame.instance.player.level * 1000 / SaveGame.instance.player.level) + 5) / 10
		if i > 0:
			description += " is worth " + "%0d" % [i] + "xp"
		elif j > 0:
			description += " is worth negligible xp"
		else:
			description += " is worth no xp"
		p = "th"
		i = SaveGame.instance.player.level % 10
		if SaveGame.instance.player.level / 10 == 1:
			pass
		elif i == 1:
			p = "st"
		elif i == 2:
			p = "nd"
		elif i == 3:
			p = "rd"
		q = ""
		i = SaveGame.instance.player.level
		if i == 8 or i == 11 or i == 18:
			q = "n"
		description += " for a" + q + ' ' + str(i) + p + " level character. "
	if (flags2 & MonsterFlags2.FIRE_AURA) != 0 and (flags2 & MonsterFlags2.LIGHTNING_AURA) != 0:
		description += PRONOUN_CAPITAL[gender] + " is surrounded by flames and electricity. "
	elif (flags2 & MonsterFlags2.FIRE_AURA) != 0:
		description += PRONOUN_CAPITAL[gender] + " is surrounded by flames. "
	elif (flags2 & MonsterFlags2.LIGHTNING_AURA) != 0:
		description += PRONOUN_CAPITAL[gender] + " is surrounded by electricity. "
	if (flags2 & MonsterFlags2.REFLECTING) != 0:
		description += PRONOUN_CAPITAL[gender] + " reflects bolt spells. "
	if (flags1 & MonsterFlags1.ESCORTED) != 0 or (flags1 & MonsterFlags1.ESCORTS_GROUP) != 0:
		description += PRONOUN_CAPITAL[gender] + " usually appears with escorts. "
	elif (flags1 & MonsterFlags1.FRIENDS) != 0:
		description += PRONOUN_CAPITAL[gender] + " usually appears in groups. "
	vp = []
	if (flags4 & MonsterFlags4.SHRIEK) != 0:
		vp.append("shriek for help")
	if (flags4 & MonsterFlags4.SHARD_BALL) != 0:
		vp.append("produce shard balls")
	if (flags4 & MonsterFlags4.ARROW_1D6) != 0:
		vp.append("fire an arrow")
	if (flags4 & MonsterFlags4.ARROW_3D6) != 0:
		vp.append("fire arrows")
	if (flags4 & MonsterFlags4.ARROW_5D6) != 0:
		vp.append("fire a missile")
	if (flags4 & MonsterFlags4.ARROW_7D6) != 0:
		vp.append("fire missiles")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " may "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " or "
			description += vp[n]
		description += ". "
	vp = []
	if (flags4 & MonsterFlags4.BREATHE_ACID) != 0:
		vp.append("acid")
	if (flags4 & MonsterFlags4.BREATHE_LIGHTNING) != 0:
		vp.append("lightning")
	if (flags4 & MonsterFlags4.BREATHE_FIRE) != 0:
		vp.append("fire")
	if (flags4 & MonsterFlags4.BREATHE_COLD) != 0:
		vp.append("frost")
	if (flags4 & MonsterFlags4.BREATHE_POISON) != 0:
		vp.append("poison")
	if (flags4 & MonsterFlags4.BREATHE_NETHER) != 0:
		vp.append("nether")
	if (flags4 & MonsterFlags4.BREATHE_LIGHT) != 0:
		vp.append("light")
	if (flags4 & MonsterFlags4.BREATHE_DARK) != 0:
		vp.append("darkness")
	if (flags4 & MonsterFlags4.BREATHE_CONFUSION) != 0:
		vp.append("confusion")
	if (flags4 & MonsterFlags4.BREATHE_SOUND) != 0:
		vp.append("sound")
	if (flags4 & MonsterFlags4.BREATHE_CHAOS) != 0:
		vp.append("chaos")
	if (flags4 & MonsterFlags4.BREATHE_DISENCHANT) != 0:
		vp.append("disenchantment")
	if (flags4 & MonsterFlags4.BREATHE_NEXUS) != 0:
		vp.append("nexus")
	if (flags4 & MonsterFlags4.BREATHE_TIME) != 0:
		vp.append("time")
	if (flags4 & MonsterFlags4.BREATHE_INERTIA) != 0:
		vp.append("inertia")
	if (flags4 & MonsterFlags4.BREATHE_GRAVITY) != 0:
		vp.append("gravity")
	if (flags4 & MonsterFlags4.BREATHE_SHARDS) != 0:
		vp.append("shards")
	if (flags4 & MonsterFlags4.BREATHE_PLASMA) != 0:
		vp.append("plasma")
	if (flags4 & MonsterFlags4.BREATHE_FORCE) != 0:
		vp.append("force")
	if (flags4 & MonsterFlags4.BREATHE_POWER) != 0:
		vp.append("power")
	if (flags4 & MonsterFlags4.BREATHE_RADIATION) != 0:
		vp.append("toxic waste")
	if (flags4 & MonsterFlags4.BREATHE_DISINTEGRATION) != 0:
		vp.append("disintegration")
	var breath: bool = false
	if vp.size() != 0:
		breath = true
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " may breathe "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " or "
			description += vp[n]
	vp = []
	if (flags5 & MonsterFlags5.ACID_BALL) != 0:
		vp.append("produce acid balls")
	if (flags5 & MonsterFlags5.LIGHTNING_BALL) != 0:
		vp.append("produce lightning balls")
	if (flags5 & MonsterFlags5.FIRE_BALL) != 0:
		vp.append("produce fire balls")
	if (flags5 & MonsterFlags5.COLD_BALL) != 0:
		vp.append("produce frost balls")
	if (flags5 & MonsterFlags5.POISON_BALL) != 0:
		vp.append("produce poison balls")
	if (flags5 & MonsterFlags5.NETHER_BALL) != 0:
		vp.append("produce nether balls")
	if (flags5 & MonsterFlags5.WATER_BALL) != 0:
		vp.append("produce water balls")
	if (flags4 & MonsterFlags4.RADIATION_BALL) != 0:
		vp.append("produce balls of radiation")
	if (flags5 & MonsterFlags5.VIS_BALL) != 0:
		vp.append("invoke power storms")
	if (flags5 & MonsterFlags5.DARK_BALL) != 0:
		vp.append("invoke darkness storms")
	if (flags4 & MonsterFlags4.CHAOS_BALL) != 0:
		vp.append("invoke raw chaos")
	if (flags6 & MonsterFlags6.DREAD_CURSE) != 0:
		vp.append("invoke the Dread Curse of Azathoth")
	if (flags5 & MonsterFlags5.DRAIN_POWER) != 0:
		vp.append("drain power")
	if (flags5 & MonsterFlags5.MIND_BLAST) != 0:
		vp.append("cause mind blasting")
	if (flags5 & MonsterFlags5.BRAIN_SMASH) != 0:
		vp.append("cause brain smashing")
	if (flags5 & MonsterFlags5.CAUSE_LIGHT_WOUNDS) != 0:
		vp.append("cause light wounds and cursing")
	if (flags5 & MonsterFlags5.CAUSE_SERIOUS_WOUNDS) != 0:
		vp.append("cause serious wounds and cursing")
	if (flags5 & MonsterFlags5.CAUSE_CRITICAL_WOUNDS) != 0:
		vp.append("cause critical wounds and cursing")
	if (flags5 & MonsterFlags5.CAUSE_MORTAL_WOUNDS) != 0:
		vp.append("cause mortal wounds")
	if (flags5 & MonsterFlags5.ACID_BOLT) != 0:
		vp.append("produce acid bolts")
	if (flags5 & MonsterFlags5.LIGHTNING_BOLT) != 0:
		vp.append("produce lightning bolts")
	if (flags5 & MonsterFlags5.FIRE_BOLT) != 0:
		vp.append("produce fire bolts")
	if (flags5 & MonsterFlags5.COLD_BOLT) != 0:
		vp.append("produce frost bolts")
	if (flags5 & MonsterFlags5.POISON_BOLT) != 0:
		vp.append("produce poison bolts")
	if (flags5 & MonsterFlags5.NETHER_BOLT) != 0:
		vp.append("produce nether bolts")
	if (flags5 & MonsterFlags5.WATER_BOLT) != 0:
		vp.append("produce water bolts")
	if (flags5 & MonsterFlags5.VIS_BOLT) != 0:
		vp.append("produce power bolts")
	if (flags5 & MonsterFlags5.PLASMA_BOLT) != 0:
		vp.append("produce plasma bolts")
	if (flags5 & MonsterFlags5.ICE_BOLT) != 0:
		vp.append("produce ice bolts")
	if (flags5 & MonsterFlags5.MAGIC_MISSILE) != 0:
		vp.append("produce magic missiles")
	if (flags5 & MonsterFlags5.SCARE) != 0:
		vp.append("terrify")
	if (flags5 & MonsterFlags5.BLINDNESS) != 0:
		vp.append("blind")
	if (flags5 & MonsterFlags5.CONFUSE) != 0:
		vp.append("confuse")
	if (flags5 & MonsterFlags5.SLOW) != 0:
		vp.append("slow")
	if (flags5 & MonsterFlags5.HOLD) != 0:
		vp.append("paralyze")
	if (flags6 & MonsterFlags6.HASTE) != 0:
		vp.append("haste-self")
	if (flags6 & MonsterFlags6.HEAL) != 0:
		vp.append("heal-self")
	if (flags6 & MonsterFlags6.BLINK) != 0:
		vp.append("blink-self")
	if (flags6 & MonsterFlags6.TELEPORT_SELF) != 0:
		vp.append("teleport-self")
	if (flags6 & MonsterFlags6.TELEPORT_TO) != 0:
		vp.append("teleport to")
	if (flags6 & MonsterFlags6.TELEPORT_AWAY) != 0:
		vp.append("teleport away")
	if (flags6 & MonsterFlags6.TELEPORT_LEVEL) != 0:
		vp.append("teleport level")
	if (flags6 & MonsterFlags6.DARKNESS) != 0:
		vp.append("create darkness")
	if (flags6 & MonsterFlags6.CREATE_TRAPS) != 0:
		vp.append("create traps")
	if (flags6 & MonsterFlags6.FORGET) != 0:
		vp.append("cause amnesia")
	if (flags6 & MonsterFlags6.SUMMON_MONSTER) != 0:
		vp.append("summon a monster")
	if (flags6 & MonsterFlags6.SUMMON_MONSTERS) != 0:
		vp.append("summon monsters")
	if (flags6 & MonsterFlags6.SUMMON_KIN) != 0:
		vp.append("summon aid")
	if (flags6 & MonsterFlags6.SUMMON_ANT) != 0:
		vp.append("summon ants")
	if (flags6 & MonsterFlags6.SUMMON_SPIDER) != 0:
		vp.append("summon spiders")
	if (flags6 & MonsterFlags6.SUMMON_HOUND) != 0:
		vp.append("summon hounds")
	if (flags6 & MonsterFlags6.SUMMON_HYDRA) != 0:
		vp.append("summon hydras")
	if (flags6 & MonsterFlags6.SUMMON_CTHULOID) != 0:
		vp.append("summon a Cthuloid entity")
	if (flags6 & MonsterFlags6.SUMMON_DEMON) != 0:
		vp.append("summon a demon")
	if (flags6 & MonsterFlags6.SUMMON_UNDEAD) != 0:
		vp.append("summon an undead")
	if (flags6 & MonsterFlags6.SUMMON_DRAGON) != 0:
		vp.append("summon a dragon")
	if (flags6 & MonsterFlags6.SUMMON_HI_UNDEAD) != 0:
		vp.append("summon Greater Undead")
	if (flags6 & MonsterFlags6.SUMMON_HI_DRAGON) != 0:
		vp.append("summon Ancient Dragons")
	if (flags6 & MonsterFlags6.SUMMON_REAVER) != 0:
		vp.append("summon Black Reavers")
	if (flags6 & MonsterFlags6.SUMMON_GREAT_OLD_ONE) != 0:
		vp.append("summon Great Old Ones")
	if (flags6 & MonsterFlags6.SUMMON_UNIQUE) != 0:
		vp.append("summon Unique Monsters")
	var magic: bool = false
	if vp.size() != 0:
		magic = true
		description += ", and is also" if breath else "%s is" % [PRONOUN_CAPITAL[gender]]
		description += " magical, casting spells"
		if (flags2 & MonsterFlags2.SMART) != 0:
			description += " intelligently"
		for n: int in vp.size():
			if n == 0:
				description += " that "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " or "
			description += vp[n]
	if breath or magic:
		var m: int = knowledge.r_cast_innate + knowledge.r_cast_spell
		var n: int = (monster_type.freq_innate + monster_type.freq_spell) / 2
		if m > 100:
			description += " 1 time in " + str(100 / n)
		elif m != 0:
			n = (n + 9) / 10 * 10
			description += " about 1 time in " + str(100 / n)
		description += ". "
	if know_armour(monster_type, knowledge):
		description += PRONOUN_CAPITAL[gender] + " is AC " + str(monster_type.ac)
		if monster_type.hit_dice == 1 and monster_type.hit_dice_sides == 1:
			description += " and has 1hp. "
		else:
			description += " and has %0dhp. " % [monster_type.hit_dice * monster_type.hit_dice_sides] if (flags1 & MonsterFlags1.FORCE_MAX_HP) != 0 else " and has %0dd%0dhp. " % [monster_type.hit_dice, monster_type.hit_dice_sides]
	vp = []
	if (flags2 & MonsterFlags2.OPEN_DOOR) != 0:
		vp.append("open doors")
	if (flags2 & MonsterFlags2.BASH_DOOR) != 0:
		vp.append("bash down doors")
	if (flags2 & MonsterFlags2.PASSWALL) != 0:
		vp.append("pass through walls")
	if (flags2 & MonsterFlags2.KILL_WALL) != 0:
		vp.append("bore through walls")
	if (flags2 & MonsterFlags2.MOVE_BODY) != 0:
		vp.append("push past weaker monsters")
	if (flags2 & MonsterFlags2.KILL_BODY) != 0:
		vp.append("destroy weaker monsters")
	if (flags2 & MonsterFlags2.TAKE_ITEM) != 0:
		vp.append("pick up objects")
	if (flags2 & MonsterFlags2.KILL_ITEM) != 0:
		vp.append("destroy objects")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " can "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " and "
			description += vp[n]
		description += ". "
	if (flags2 & MonsterFlags2.INVISIBLE) != 0:
		description += PRONOUN_CAPITAL[gender] + " is invisible. "
	if (flags2 & MonsterFlags2.COLD_BLOOD) != 0:
		description += PRONOUN_CAPITAL[gender] + " is cold blooded. "
	if (flags2 & MonsterFlags2.EMPTY_MIND) != 0:
		description += PRONOUN_CAPITAL[gender] + " is not detected by telepathy. "
	if (flags2 & MonsterFlags2.WEIRD_MIND) != 0:
		description += PRONOUN_CAPITAL[gender] + " is rarely detected by telepathy. "
	if (flags2 & MonsterFlags2.MULTIPLY) != 0:
		description += PRONOUN_CAPITAL[gender] + " breeds explosively. "
	if (flags2 & MonsterFlags2.REGENERATE) != 0:
		description += PRONOUN_CAPITAL[gender] + " regenerates quickly. "
	vp = []
	if (flags3 & MonsterFlags3.HURT_BY_ROCK) != 0:
		vp.append("rock remover")
	if (flags3 & MonsterFlags3.HURT_BY_LIGHT) != 0:
		vp.append("bright light")
	if (flags3 & MonsterFlags3.HURT_BY_FIRE) != 0:
		vp.append("fire")
	if (flags3 & MonsterFlags3.HURT_BY_COLD) != 0:
		vp.append("cold")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " is hurt by "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " and "
			description += vp[n]
		description += ". "
	vp = []
	if (flags3 & MonsterFlags3.IMMUNE_ACID) != 0:
		vp.append("acid")
	if (flags3 & MonsterFlags3.IMMUNE_LIGHTNING) != 0:
		vp.append("lightning")
	if (flags3 & MonsterFlags3.IMMUNE_FIRE) != 0:
		vp.append("fire")
	if (flags3 & MonsterFlags3.IMMUNE_COLD) != 0:
		vp.append("cold")
	if (flags3 & MonsterFlags3.IMMUNE_POISON) != 0:
		vp.append("poison")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " resists "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " and "
			description += vp[n]
		description += ". "
	vp = []
	if (flags3 & MonsterFlags3.RESIST_NETHER) != 0:
		vp.append("nether")
	if (flags3 & MonsterFlags3.RESIST_WATER) != 0:
		vp.append("water")
	if (flags3 & MonsterFlags3.RESIST_PLASMA) != 0:
		vp.append("plasma")
	if (flags3 & MonsterFlags3.RESIST_NEXUS) != 0:
		vp.append("nexus")
	if (flags3 & MonsterFlags3.RESIST_DISENCHANT) != 0:
		vp.append("disenchantment")
	if (flags3 & MonsterFlags3.RESIST_TELEPORT) != 0:
		vp.append("teleportation")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " resists "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " and "
			description += vp[n]
		description += ". "
	vp = []
	if (flags3 & MonsterFlags3.IMMUNE_STUN) != 0:
		vp.append("stunned")
	if (flags3 & MonsterFlags3.IMMUNE_FEAR) != 0:
		vp.append("frightened")
	if (flags3 & MonsterFlags3.IMMUNE_CONFUSION) != 0:
		vp.append("confused")
	if (flags3 & MonsterFlags3.IMMUNE_SLEEP) != 0:
		vp.append("slept")
	if vp.size() != 0:
		description += PRONOUN_CAPITAL[gender]
		for n: int in vp.size():
			if n == 0:
				description += " cannot be "
			elif n < vp.size() - 1:
				description += ", "
			else:
				description += " or "
			description += vp[n]
		description += ". "
	if knowledge.r_wake * knowledge.r_wake > monster_type.sleep or knowledge.r_ignore == 255 or (monster_type.sleep == 0 and (knowledge.r_total_kills >= 10 or knowledge.r_probed)):
		var act: String
		if monster_type.sleep > 200:
			act = "prefers to ignore"
		elif monster_type.sleep > 95:
			act = "pays very little attention to"
		elif monster_type.sleep > 75:
			act = "pays little attention to"
		elif monster_type.sleep > 45:
			act = "tends to overlook"
		elif monster_type.sleep > 25:
			act = "takes quite a while to see"
		elif monster_type.sleep > 10:
			act = "takes a while to see"
		elif monster_type.sleep > 5:
			act = "is fairly observant of"
		elif monster_type.sleep > 3:
			act = "is observant of"
		elif monster_type.sleep > 1:
			act = "is very observant of"
		elif monster_type.sleep > 0:
			act = "is vigilant for"
		else:
			act = "is ever vigilant for"
		description += PRONOUN_CAPITAL[gender] + ' ' + act + " intruders, which " + PRONOUN[gender] + " may notice from %0d" % [10 * monster_type.notice_range] + " feet. "
	if knowledge.r_drop_gold != 0 or knowledge.r_drop_item != 0:
		var single: bool = false
		description += PRONOUN_CAPITAL[gender] + " may carry"
		var n: int = maxi(knowledge.r_drop_gold, knowledge.r_drop_item)
		if n == 1:
			description += " a"
			single = true
		elif n == 2:
			description += " one or two"
		else:
			description += " up to " + str(n)
		if (flags1 & MonsterFlags1.DROP_GREAT) != 0:
			p = " exceptional"
		elif (flags1 & MonsterFlags1.DROP_GOOD) != 0:
			p = " good"
			single = false
		else:
			p = ""
		if knowledge.r_drop_item != 0:
			if single:
				description += "n"
			single = false
			if p != "":
				description += p
			description += " object"
			if n != 1:
				description += "s"
			p = " or"
		if knowledge.r_drop_gold != 0:
			if p == "":
				single = false
			if single:
				description += "n"
			if p != "":
				description += p
			description += " treasure"
			if n != 1:
				description += "s"
		description += ". "
	var num_attacks: int = 0
	for m: int in 4:
		if monster_type.attack[m].method == 0:
			continue
		if knowledge.r_blows[m] != 0:
			num_attacks += 1
	var r: int = 0
	for m: int in 4:
		if monster_type.attack[m].method == 0:
			continue
		if knowledge.r_blows[m] == 0:
			continue
		var method: int = monster_type.attack[m].method
		var effect: int = monster_type.attack[m].effect
		var d1: int = monster_type.attack[m].damage_dice
		var d2: int = monster_type.attack[m].damage_dice_sides
		p = ""
		match method:
			AttackType.HIT:
				p = "hit"
			AttackType.TOUCH:
				p = "touch"
			AttackType.PUNCH:
				p = "punch"
			AttackType.KICK:
				p = "kick"
			AttackType.CLAW:
				p = "claw"
			AttackType.BITE:
				p = "bite"
			AttackType.STING:
				p = "sting"
			AttackType.BUTT:
				p = "butt"
			AttackType.CRUSH:
				p = "crush"
			AttackType.ENGULF:
				p = "engulf"
			AttackType.CHARGE:
				p = "charge"
			AttackType.CRAWL:
				p = "crawl on you"
			AttackType.DROOL:
				p = "drool on you"
			AttackType.SPIT:
				p = "spit"
			AttackType.GAZE:
				p = "gaze"
			AttackType.WAIL:
				p = "wail"
			AttackType.SPORE:
				p = "release spores"
			AttackType.HERO_WORSHIP:
				p = "hero worship"
			AttackType.BEG:
				p = "beg"
			AttackType.INSULT:
				p = "insult"
			AttackType.MOAN:
				p = "moan"
			AttackType.SHOW:
				p = "sing"
		q = ""
		match effect:
			AttackEffect.HURT:
				q = "attack"
			AttackEffect.POISON:
				q = "poison"
			AttackEffect.UN_BONUS:
				q = "disenchant"
			AttackEffect.UN_POWER:
				q = "drain charges"
			AttackEffect.EAT_GOLD:
				q = "steal gold"
			AttackEffect.EAT_ITEM:
				q = "steal items"
			AttackEffect.EAT_FOOD:
				q = "eat your food"
			AttackEffect.EAT_LIGHT:
				q = "absorb light"
			AttackEffect.ACID:
				q = "shoot acid"
			AttackEffect.ELECTRICITY:
				q = "electrocute"
			AttackEffect.FIRE:
				q = "burn"
			AttackEffect.COLD:
				q = "freeze"
			AttackEffect.BLIND:
				q = "blind"
			AttackEffect.CONFUSE:
				q = "confuse"
			AttackEffect.TERRIFY:
				q = "terrify"
			AttackEffect.PARALYZE:
				q = "paralyze"
			AttackEffect.LOSE_STR:
				q = "reduce strength"
			AttackEffect.LOSE_INT:
				q = "reduce intelligence"
			AttackEffect.LOSE_WIS:
				q = "reduce wisdom"
			AttackEffect.LOSE_DEX:
				q = "reduce dexterity"
			AttackEffect.LOSE_CON:
				q = "reduce constitution"
			AttackEffect.LOSE_CHA:
				q = "reduce charisma"
			AttackEffect.LOSE_ALL:
				q = "reduce all stats"
			AttackEffect.SHATTER:
				q = "shatter"
			AttackEffect.EXP_10:
				q = "lower experience (by 10d6+)"
			AttackEffect.EXP_20:
				q = "lower experience (by 20d6+)"
			AttackEffect.EXP_40:
				q = "lower experience (by 40d6+)"
			AttackEffect.EXP_80:
				q = "lower experience (by 80d6+)"
		if r == 0:
			description += PRONOUN_CAPITAL[gender] + " can "
		elif r < num_attacks - 1:
			description += ", "
		else:
			description += ", and "
		if p == "":
			p = "do something weird"
		description += p
		if q != "":
			description += " to "
			description += q
			if d1 != 0 and d2 != 0 and know_damage(monster_type, knowledge, m):
				description += " for " + str(d1) + 'd' + str(d2) + " damage"
		r += 1
	if r != 0:
		description += ". "
	elif (flags1 & MonsterFlags1.NEVER_ATTACK) != 0:
		description += PRONOUN_CAPITAL[gender] + " has no physical attacks. "
	else:
		description += "Nothing is known about " + PRONOUN_POSSESSIVE[gender] + " attack. "
	if (flags1 & MonsterFlags1.UNIQUE) != 0:
		var dead: bool = monster_type.max_num == 0
		if knowledge.r_deaths != 0:
			description += PRONOUN[gender] + " has slain %0d" % [knowledge.r_deaths] + " of your ancestors"
			if dead:
				description += ", but you have avenged them! "
			else:
				var remain: String = "remains" if knowledge.r_deaths == 1 else "remain"
				description += ", who " + remain + " unavenged. "
		elif dead:
			description += "You have slain this foe. "
	elif knowledge.r_deaths != 0:
		var has: String = "has" if knowledge.r_deaths == 1 else "have"
		description += str(knowledge.r_deaths) + " of your ancestors " + has + " been killed by this creature, "
		if knowledge.r_kills != 0:
			description += "and you have exterminated at least %0d of the creatures. " % [knowledge.r_kills]
		elif knowledge.r_total_kills != 0:
			description += "and your ancestors have exterminated at least %d of the creatures. " % [knowledge.r_total_kills]
		else:
			description += "and " + PRONOUN[gender] + " is not ever known to have been defeated. "
	else:
		if knowledge.r_kills != 0:
			description += "You have killed at least %d of these creatures. " % [knowledge.r_kills]
		elif knowledge.r_total_kills != 0:
			description += "Your ancestors have killed at least %d of these creatures. " % [knowledge.r_total_kills]
		else:
			description += "No battles to the death are recalled. "
	if (flags1 & MonsterFlags1.GUARDIAN) != 0:
		description += "You feel an intense desire to kill this monster... "
	Console.print_wrap(row, col, description, ink)


func know_armour(monster_type: MonsterType, knowledge: MonsterKnowledge) -> bool:
	var kills: int = knowledge.r_total_kills
	if (kills > 304 / (4 + monster_type.level)) or knowledge.r_probed:
		return true
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	return kills > 304 / (38 + (5 * monster_type.level / 4))


func know_damage(monster_type: MonsterType, knowledge: MonsterKnowledge, i: int) -> bool:
	var a: int = knowledge.r_blows[i]
	var d1: int = monster_type.attack[i].damage_dice
	var d2: int = monster_type.attack[i].damage_dice_sides
	var d: int = d1 * d2
	if (4 + monster_type.level) * a > 80 * d:
		return true
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	return (4 + monster_type.level) * 2 * a > 80 * d


func display_header(monster_type: MonsterType) -> void:
	var c1: String = monster_type.character
	var a1: int = monster_type.colour
	Console.clear_row(0)
	var name: String = ""
	if monster_type.flags_1.is_clear(MonsterFlags1.UNIQUE):
		name += "The "
	name += monster_type.name
	name += " ('"
	Console.print_at(0, 0, name, Ink.WHITE)
	Console.print_following(c1, a1)
	Console.print_following("')", Ink.WHITE)
