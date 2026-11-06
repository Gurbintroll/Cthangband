# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemFlags3
extends Object

const SH_FIRE: int = 0x00000001
const SH_ELEC: int = 0x00000002
#const UNUSED_1:int =     0x00000004
const ANTI_THEFT: int = 0x00000008
const NO_TELE: int = 0x00000010
const NO_MAGIC: int = 0x00000020
const WRAITH: int = 0x00000040
const DREAD_CURSE: int = 0x00000080
const EASY_KNOW: int = 0x00000100
const HIDE_TYPE: int = 0x00000200
const SHOW_MODS: int = 0x00000400
const INSTA_ART: int = 0x00000800
const FEATHER: int = 0x00001000
const LIGHT_SOURCE: int = 0x00002000
const SEE_INVIS: int = 0x00004000
const TELEPATHY: int = 0x00008000
const SLOW_DIGEST: int = 0x00010000
const REGEN: int = 0x00020000
const XTRA_MIGHT: int = 0x00040000
const XTRA_SHOTS: int = 0x00080000
const IGNORE_ACID: int = 0x00100000
const IGNORE_ELEC: int = 0x00200000
const IGNORE_FIRE: int = 0x00400000
const IGNORE_COLD: int = 0x00800000
const ACTIVATE: int = 0x01000000
const DRAIN_EXP: int = 0x02000000
const TELEPORT: int = 0x04000000
const AGGRAVATE: int = 0x08000000
const BLESSED: int = 0x10000000
const CURSED: int = 0x20000000
const HEAVY_CURSE: int = 0x40000000
const PERMA_CURSE: int = 0x80000000
