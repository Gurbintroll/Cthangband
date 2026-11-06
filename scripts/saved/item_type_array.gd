# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemTypeArray extends Resource

@export var types:Array[ItemType]

func reset_stompability()->void:
	for item:ItemType in types:
		if item.has_quality():
			item.stompable[0] = true
		else:
			item.stompable[0] = (item.cost <= 0)
		item.stompable[1] = false
		item.stompable[2] = false
		item.stompable[3] = false

func look_up_type(category:int, sub_category:int)->ItemType:
	for type:ItemType in types:
		if type.item_category == category and type.sub_category == sub_category:
			return type
	return null
