# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterType
extends Resource

@export var ac: int
@export var attack: Array[MonsterAttack]
@export var character: String
@export var colour: int
@export var cur_num: int
@export var description: String
@export var flags_1: FlagSet
@export var flags_2: FlagSet
@export var flags_3: FlagSet
@export var flags_4: FlagSet
@export var flags_5: FlagSet
@export var flags_6: FlagSet
@export var freq_innate: int
@export var freq_spell: int
@export var hit_dice: int
@export var hit_dice_sides: int
@export var index: int
@export var knowledge: MonsterKnowledge
@export var level: int
@export var max_num: int
@export var experience: int
@export var name: String
@export var notice_range: int
@export var rarity: int
@export var sleep: int
@export var speed: int
@export var split_name_1: String
@export var split_name_2: String
@export var split_name_3: String


func get_copy_for_new_profile() -> MonsterType:
	var new_type: MonsterType = MonsterType.new()
	new_type.ac = ac
	new_type.attack.append_array(attack)
	new_type.character = character
	new_type.colour = colour
	new_type.cur_num = 0
	new_type.description = description
	new_type.experience = experience
	new_type.flags_1 = FlagSet.new()
	new_type.flags_1.copy(flags_1)
	new_type.flags_2 = FlagSet.new()
	new_type.flags_2.copy(flags_2)
	new_type.flags_3 = FlagSet.new()
	new_type.flags_3.copy(flags_3)
	new_type.flags_4 = FlagSet.new()
	new_type.flags_4.copy(flags_4)
	new_type.flags_5 = FlagSet.new()
	new_type.flags_5.copy(flags_5)
	new_type.flags_6 = FlagSet.new()
	new_type.flags_6.copy(flags_6)
	new_type.freq_innate = freq_innate
	new_type.freq_spell = freq_spell
	new_type.hit_dice = hit_dice
	new_type.hit_dice_sides = hit_dice_sides
	new_type.index = index
	new_type.knowledge = MonsterKnowledge.new()
	new_type.level = level
	new_type.max_num = max_num
	new_type.name = name
	new_type.notice_range = notice_range
	new_type.rarity = rarity
	new_type.sleep = sleep
	new_type.speed = speed
	new_type.split_name_1 = split_name_1
	new_type.split_name_2 = split_name_2
	new_type.split_name_3 = split_name_3
	return new_type


func get_coin_type() -> int:
	if character == "$":
		if name.contains(" copper "):
			return 2
		if name.contains(" silver "):
			return 5
		if name.contains(" gold "):
			return 10
		if name.contains(" mithril "):
			return 16
		if name.contains(" adamantite "):
			return 17
		if name.begins_with("Copper "):
			return 2
		if name.begins_with("Silver "):
			return 5
		if name.begins_with("Gold "):
			return 10
		if name.begins_with("Mithril "):
			return 16
		if name.begins_with("Adamantite "):
			return 17
	return 0
