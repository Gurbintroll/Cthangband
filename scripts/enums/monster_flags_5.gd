# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterFlags5
extends Object

const ACID_BALL: int = 0x00000001
const LIGHTNING_BALL: int = 0x00000002
const COLD_BALL: int = 0x00000008
const FIRE_BALL: int = 0x00000004
const POISON_BALL: int = 0x00000010
const NETHER_BALL: int = 0x00000020
const WATER_BALL: int = 0x00000040
const VIS_BALL: int = 0x00000080
const DARK_BALL: int = 0x00000100
const DRAIN_POWER: int = 0x00000200
const MIND_BLAST: int = 0x00000400
const BRAIN_SMASH: int = 0x00000800
const CAUSE_LIGHT_WOUNDS: int = 0x00001000
const CAUSE_SERIOUS_WOUNDS: int = 0x00002000
const CAUSE_CRITICAL_WOUNDS: int = 0x00004000
const CAUSE_MORTAL_WOUNDS: int = 0x00008000
const ACID_BOLT: int = 0x00010000
const LIGHTNING_BOLT: int = 0x00020000
const FIRE_BOLT: int = 0x00040000
const POISON_BOLT: int = 0x00100000
const COLD_BOLT: int = 0x00080000
const NETHER_BOLT: int = 0x00200000
const WATER_BOLT: int = 0x00400000
const VIS_BOLT: int = 0x00800000
const PLASMA_BOLT: int = 0x01000000
const ICE_BOLT: int = 0x02000000
const MAGIC_MISSILE: int = 0x04000000
const SCARE: int = 0x08000000
const BLINDNESS: int = 0x10000000
const CONFUSE: int = 0x20000000
const SLOW: int = 0x40000000
const HOLD: int = 0x80000000
const BOLT_MASK: int = ACID_BOLT | LIGHTNING_BOLT | FIRE_BOLT | COLD_BOLT | POISON_BOLT | NETHER_BOLT | WATER_BOLT | VIS_BOLT | PLASMA_BOLT | ICE_BOLT | MAGIC_MISSILE
const INT_MASK: int = HOLD | SLOW | CONFUSE | BLINDNESS | SCARE
const SUMMON_MASK: int = 0
