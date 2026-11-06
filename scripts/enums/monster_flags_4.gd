# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterFlags4
extends Object

const SHRIEK: int = 0x00000001
#const UNUSED_1:int =               0x00000002
#const UNUSED_2:int =               0x00000004
const SHARD_BALL: int = 0x00000008
const ARROW_1D6: int = 0x00000010
const ARROW_3D6: int = 0x00000020
const ARROW_5D6: int = 0x00000040
const ARROW_7D6: int = 0x00000080
const BREATHE_ACID: int = 0x00000100
const BREATHE_LIGHTNING: int = 0x00000200
const BREATHE_FIRE: int = 0x00000400
const BREATHE_COLD: int = 0x00000800
const BREATHE_POISON: int = 0x00001000
const BREATHE_NETHER: int = 0x00002000
const BREATHE_LIGHT: int = 0x00004000
const BREATHE_DARK: int = 0x00008000
const BREATHE_CONFUSION: int = 0x00010000
const BREATHE_SOUND: int = 0x00020000
const BREATHE_CHAOS: int = 0x00040000
const BREATHE_DISENCHANT: int = 0x00080000
const BREATHE_NEXUS: int = 0x00100000
const BREATHE_TIME: int = 0x00200000
const BREATHE_INERTIA: int = 0x00400000
const BREATHE_GRAVITY: int = 0x00800000
const BREATHE_SHARDS: int = 0x01000000
const BREATHE_PLASMA: int = 0x02000000
const BREATHE_FORCE: int = 0x04000000
const BREATHE_POWER: int = 0x08000000
const RADIATION_BALL: int = 0x10000000
const BREATHE_RADIATION: int = 0x20000000
const CHAOS_BALL: int = 0x40000000
const BREATHE_DISINTEGRATION: int = 0x80000000
const BOLT_MASK: int = ARROW_1D6 | ARROW_3D6 | ARROW_5D6 | ARROW_7D6
const INT_MASK: int = 0
const SUMMON_MASK: int = 0
