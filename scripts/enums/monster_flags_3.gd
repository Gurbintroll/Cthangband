# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterFlags3
extends Node

const ORC: int = 0x00000001
const TROLL: int = 0x00000002
const GIANT: int = 0x00000004
const DRAGON: int = 0x00000008
const DEMON: int = 0x00000010
const UNDEAD: int = 0x00000020
const EVIL: int = 0x00000040
const ANIMAL: int = 0x00000080
const GREAT_OLD_DONE: int = 0x00000100
const GOOD: int = 0x00000200
const UNUSED_1: int = 0x00000400
const NON_LIVING: int = 0x00000800
const HURT_BY_LIGHT: int = 0x00001000
const HURT_BY_ROCK: int = 0x00002000
const HURT_BY_FIRE: int = 0x00004000
const HURT_BY_COLD: int = 0x00008000
const IMMUNE_ACID: int = 0x00010000
const IMMUNE_LIGHTNING: int = 0x00020000
const IMMUNE_FIRE: int = 0x00040000
const IMMUNE_COLD: int = 0x00080000
const IMMUNE_POISON: int = 0x00100000
const RESIST_TELEPORT: int = 0x00200000
const RESIST_NETHER: int = 0x00400000
const RESIST_WATER: int = 0x00800000
const RESIST_PLASMA: int = 0x01000000
const RESIST_NEXUS: int = 0x02000000
const RESIST_DISENCHANT: int = 0x04000000
const CTHULOID: int = 0x08000000
const IMMUNE_FEAR: int = 0x10000000
const IMMUNE_STUN: int = 0x20000000
const IMMUNE_CONFUSION: int = 0x40000000
const IMMUNE_SLEEP: int = 0x80000000
