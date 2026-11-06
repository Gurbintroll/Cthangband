# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name RedrawFlags
extends Object

const REDRAW_MISC: int = 0x00000001
const REDRAW_TITLE: int = 0x00000002
const REDRAW_LEV: int = 0x00000004
const REDRAW_EXP: int = 0x00000008
const REDRAW_STATS: int = 0x00000010
const REDRAW_ARMOR: int = 0x00000020
const REDRAW_HP: int = 0x00000040
const REDRAW_POWER: int = 0x00000080
const REDRAW_GOLD: int = 0x00000100
const REDRAW_DEPTH: int = 0x00000200
const REDRAW_EQUIPPY: int = 0x00000400
const REDRAW_HEALTH: int = 0x00000800
const REDRAW_CUT: int = 0x00001000
const REDRAW_STUN: int = 0x00002000
const REDRAW_HUNGER: int = 0x00004000
const REDRAW_DTRAP: int = 0x00008000
const REDRAW_BLIND: int = 0x00010000
const REDRAW_CONFUSED: int = 0x00020000
const REDRAW_POISONED: int = 0x00080000
const REDRAW_AFRAID: int = 0x00040000
const REDRAW_STATE: int = 0x00100000
const REDRAW_SPEED: int = 0x00200000
const REDRAW_STUDY: int = 0x00400000
const REDRAW_EXTRA: int = 0x01000000
const REDRAW_BASIC: int = 0x02000000
const REDRAW_MAP: int = 0x04000000
const REDRAW_WIPE: int = 0x08000000
