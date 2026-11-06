# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name UpdateFlags
extends Object

const UPDATE_BONUSES: int = 0x00000001
const UPDATE_TORCH_RADIUS: int = 0x00000002
const UPDATE_HEALTH: int = 0x00000010
const UPDATE_POWER: int = 0x00000020
const UPDATE_SPELLS: int = 0x00000040
const UPDATE_REMOVE_VIEW: int = 0x00010000
const UPDATE_REMOVE_LIGHT: int = 0x00020000
const UPDATE_VIEW: int = 0x00100000
const UPDATE_LIGHT: int = 0x00200000
const UPDATE_MONSTERS: int = 0x01000000
const UPDATE_DISTANCES: int = 0x02000000
const UPDATE_SCENT: int = 0x10000000
