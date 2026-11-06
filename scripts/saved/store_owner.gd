# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name StoreOwner
extends Resource

@export var max_cost: int
@export var min_markup: int
@export var name: String
@export var ancestry: String


static func new_store_owner(owner_name: String, owner_cost: int, owner_markup: int, owner_ancestry: String) -> StoreOwner:
	var owner: StoreOwner = StoreOwner.new()
	owner.name = owner_name
	owner.max_cost = owner_cost
	owner.min_markup = owner_markup
	owner.ancestry = owner_ancestry
	return owner
