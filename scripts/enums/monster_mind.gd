# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterMind
extends Object

const SEEN_ACID_RES: int = 0x00000001
const SEEN_ELECTRIC_RES: int = 0x00000002
const SEEN_FIRE_RES: int = 0x00000004
const SEEN_COLD_RES: int = 0x00000008
const SEEN_POISON_RES: int = 0x00000010
const SEEN_NETHER_RES: int = 0x00000020
const SEEN_LIGHT_RES: int = 0x00000040
const SEEN_DARK_RES: int = 0x00000080
const SEEN_FEAR_RES: int = 0x00000100
const SEEN_CONF_RES: int = 0x00000200
const SEEN_CHAOS_RES: int = 0x00000400
const SEEN_DISEN_RES: int = 0x00000800
const SEEN_BLIND_RES: int = 0x00001000
const SEEN_NEXUS_RES: int = 0x00002000
const SEEN_SOUND_RES: int = 0x00004000
const SEEN_SHARD_RES: int = 0x00008000
const SEEN_TEMP_ACID_RES: int = 0x00010000
const SEEN_TEMP_ELECTRIC_RES: int = 0x00020000
const SEEN_TEMP_FIRE_RES: int = 0x00040000
const SEEN_TEMP_COLD_RES: int = 0x00080000
const SEEN_TEMP_POISON_RES: int = 0x00100000
#const UNIUSED_2:int =             0x00200000
const CLONED: int = 0x00400000
const FRIENDLY: int = 0x00800000
const SEEN_ACID_IMMUNITY: int = 0x01000000
const SEEN_ELECTRIC_IMMUNITY: int = 0x02000000
const SEEN_FIRE_IMMUNITY: int = 0x04000000
const SEEN_COLD_IMMUNITY: int = 0x08000000
#const UNUSED_1:int =              0x10000000
const SEEN_REFLECTION: int = 0x20000000
const SEEN_FREE_ACTION: int = 0x40000000
const SEEN_NO_POWER: int = 0x80000000
