# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name QuestList
extends Resource

const MAX_QUESTS: int = 50

@export var quests: Array[Quest]

var find_quest: Array[String] = [
	"You find the following inscription in the floor",
	"You see a message inscribed in the wall",
	"There is a sign saying",
	"Something is writen on the staircase",
	"You find a scroll with the following message",
]


static func create_new_quest_list() -> QuestList:
	var quest_list: QuestList = QuestList.new()
	for i: int in MAX_QUESTS:
		quest_list.quests.append(Quest.new())
	return quest_list


func get_active_quest_count() -> int:
	var count: int = 0
	for quest: Quest in quests:
		if quest.is_active():
			count += 1
	return count


func get_random_quest_monster(quest_index: int) -> int:
	var monster_index: int
	# Pick a random monster type, skipping low level monsters and skewing slightly to high level monsters
	match (Rng.roll_die(10)):
		1:
			monster_index = Rng.random_between(141, 210)
		2:
			monster_index = Rng.random_between(211, 260)
		3:
			monster_index = Rng.random_between(261, 300)
		4:
			monster_index = Rng.random_between(301, 340)
		5:
			monster_index = Rng.random_between(341, 380)
		6:
			monster_index = Rng.random_between(381, 420)
		7:
			monster_index = Rng.random_between(421, 460)
		8:
			monster_index = Rng.random_between(461, 500)
		9:
			monster_index = Rng.random_between(501, 300)
		10:
			monster_index = Rng.random_between(531, 560)
	# Don't allow multiplying monsters
	if Profile.instance.monster_types.types[monster_index].flags_2.is_set(MonsterFlags2.MULTIPLY):
		return 0
	# Disallow monsters already chosen for a quest
	for i: int in range(1, quest_index):
		if quests[i].monster_type_index == monster_index:
			return 0
	# We passed all the tests, so return the monster
	return monster_index


func initialise_quests() -> void:
	var monster_types: MonsterTypeArray = Profile.instance.monster_types
	monster_types.reset_unique_only_guardian_status()
	var index: int = 0
	# Initialise one or two quests per dungeon - always with a named unique
	for dungeon: Dungeon in SaveGame.instance.dungeons:
		if dungeon.first_guardian != "":
			quests[index].depth = dungeon.first_guardian_level
			var monster_type: int = monster_types.index_from_name(dungeon.first_guardian)
			quests[index].monster_type_index = monster_type
			monster_types.types[monster_type].flags_1.set_bits(MonsterFlags1.ONLY_GUARDIAN)
			quests[index].dungeon = dungeon.index
			quests[index].to_kill = 1
			quests[index].killed = 0
			index += 1
		if dungeon.second_guardian != "":
			quests[index].depth = dungeon.second_guardian_level
			var monster_type: int = monster_types.index_from_name(dungeon.second_guardian)
			quests[index].monster_type_index = monster_type
			monster_types.types[monster_type].flags_1.set_bits(MonsterFlags1.ONLY_GUARDIAN)
			quests[index].dungeon = dungeon.index
			quests[index].to_kill = 1
			quests[index].killed = 0
			index += 1
	# Fill the rest of the array with random quests
	while index < MAX_QUESTS:
		# assume the worst, so the loops will run at least once each
		var same_level: bool = true
		var monster_index: int = 0
		# Loop until we have a monster that is not the same level as a previous one
		while same_level:
			# Loop until we get a suitable monster selection
			monster_index = 0
			while monster_index == 0:
				monster_index = get_random_quest_monster(index)
			quests[index].monster_type_index = monster_index
			quests[index].depth = monster_types.types[monster_index].level
			quests[index].depth -= Rng.random_between(2, 3 + quests[index].depth / 6)
			# Disallow two monsters that ended up on the same level
			same_level = false
			for i: int in range(index):
				if quests[index].depth == quests[i].depth:
					same_level = true
					break
		# If our chosen quest monster is unique, then prevent it from spawning outside the quest level
		if monster_types.types[monster_index].flags_1.is_set(MonsterFlags1.UNIQUE):
			monster_types.types[monster_index].flags_1.set_bits(MonsterFlags1.ONLY_GUARDIAN)
		# Find a dungeon to put the quest in
		var found_dungeon: bool = false
		var dungeon_index: int
		var dungeon: Dungeon
		while found_dungeon == false:
			# Note that we never put quests in dungeon 0 (Celephais) even if they would fit
			dungeon_index = Rng.roll_die(SaveGame.instance.dungeons.size() - 1)
			dungeon = SaveGame.instance.dungeons[dungeon_index]
			# Try again if the quest would be outside the dungeon's level range
			if quests[index].depth <= dungeon.offset or quests[index].depth > dungeon.offset + dungeon.max_level:
				continue
			# Try again if the quest would be on a guardian level
			if quests[index].depth == dungeon.offset + dungeon.first_guardian_level or quests[index].depth == dungeon.offset + dungeon.second_guardian_level:
				continue
			# We didn't quit, so we have our dungeon
			found_dungeon = true
		# Put the quest in the dungeon
		quests[index].dungeon = dungeon_index
		# Take into account the dungeon's offset for the quest
		quests[index].depth -= dungeon.offset
		# Only one to kill if it's a unique
		if monster_types.types[monster_index].flags_1.is_set(MonsterFlags1.UNIQUE):
			quests[index].to_kill = 1
		else:
			# Get a number to kill based on the quest level (taking into account the dungeon's offset)
			quests[index].to_kill = 10 + Rng.roll_die((quests[index].depth / 3) + 5)
			# If the monster comes with friends, then we need to kill more of them
			if monster_types.types[monster_index].flags_1.is_set(MonsterFlags1.FRIENDS):
				quests[index].to_kill += 5
		# We haven't killed any, yet
		quests[index].killed = 0
		index += 1


func get_quest_number() -> int:
	for i: int in quests.size():
		if quests[i].depth == SaveGame.instance.current_depth and quests[i].dungeon == SaveGame.instance.current_dungeon.index:
			return i
	return -1


func is_quest_level(level: int) -> bool:
	if level == 0:
		return false
	for quest: Quest in quests:
		if quest.depth == level and quest.dungeon == SaveGame.instance.current_dungeon.index:
			return true
	return false


func get_quest_monster() -> int:
	for quest: Quest in quests:
		if quest.depth == SaveGame.instance.current_depth and quest.dungeon == SaveGame.instance.current_dungeon.index:
			return quest.monster_type_index
	return 0


func discover_quest() -> void:
	var index: int = get_quest_number()
	var monster: MonsterType = Profile.instance.monster_types.types[quests[index].monster_type_index]
	var name: String = monster.name
	var num: int = quests[index].to_kill
	Profile.instance.message_print(find_quest[Rng.roll_die(5) - 1])
	Profile.instance.message_print()
	if num == 1:
		Profile.instance.message_print("Beware, this level is protected by %s!" % [name])
	else:
		var plural: String = Util.pluralise_monster_name(name)
		Profile.instance.message_print("Be warned, this level is guarded by %d %s!" % [num, plural])
	quests[index].discovered = true


func describe_quest (index:int) -> String:
	var buf:String
	var quest:Quest = quests[index]
	var monster_type:MonsterType = Profile.instance.monster_types.types[quest.monster_type_index]
	var name:String = monster_type.name
	var to_kill:int = quest.to_kill
	var dungeon_name:String = SaveGame.instance.dungeons[quest.dungeon].name
	var lev:int = quest.depth
	if lev == 0:
		if to_kill == 1:
			buf = "You have defeated %s in %s" % [name, dungeon_name]
		else:
			var plural:String = Util.pluralise_monster_name(name)
			buf = "You have defeated %d %s in %s" % [to_kill, plural, dungeon_name]
	else:
		if quest.discovered:
			if to_kill == 1:
				buf = "You must defeat %s at lvl %d of %s" % [name, lev, dungeon_name]
			else:
				if quest.to_kill - quest.killed > 1:
					var plural:String = Util.pluralise_monster_name(name)
					buf = "You must defeat %d %s at lvl %d of %s" % [to_kill, plural, lev, dungeon_name]
				else:
					buf = "You must defeat 1 %s at lvl %d of %s" % [name, lev, dungeon_name]
		else:
			buf = "You must defeat something at lvl %d of %s" % [lev, dungeon_name]
	return buf


func print_quest_message() -> void:
	var index: int = get_quest_number()
	var monster: MonsterType = Profile.instance.monster_types.types[quests[index].monster_type_index]
	var name: String = monster.name
	var num_left: int = quests[index].to_kill - quests[index].killed
	if quests[index].to_kill == 1:
		Profile.instance.message_print("You still have to kill %s." % [name])
	elif num_left > 1:
		var plural: String = Util.pluralise_monster_name(name)
		Profile.instance.message_print("You still have to kill %d %s." % [num_left, plural])
	else:
		Profile.instance.message_print("You still have to kill 1 %s." % [name])
