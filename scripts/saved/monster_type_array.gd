# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterTypeArray extends Resource

@export var types:Array[MonsterType]


func sort_by_level()->void:
	var sorted_types:Array[MonsterType] = []
	var index:int = 0
	for level:int in range(-1,128):
		for monster_type:MonsterType in types:
			if monster_type.level == level:
				monster_type.index = index
				sorted_types.append(monster_type)
				index += 1
	types = sorted_types


func reset_unique_only_guardian_status()->void:
	for type:MonsterType in types:
		type.flags_1.clear_bits(MonsterFlags1.ONLY_GUARDIAN)


func index_from_name(name:String)->int:
	for type:MonsterType in types:
		if type.name == name:
			return type.index
	return 0
	
