# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ItemFlags1
extends Object

const STR: int = 0x00000001
const INT: int = 0x00000002
const WIS: int = 0x00000004
const DEX: int = 0x00000008
const CON: int = 0x00000010
const CHA: int = 0x00000020
#const UNUSED_1:int =        0x00000040
#const UNUSED_2:int =        0x00000080
const STEALTH: int = 0x00000100
const SEARCH: int = 0x00000200
const INFRA: int = 0x00000400
const TUNNEL: int = 0x00000800
const SPEED: int = 0x00001000
const BLOWS: int = 0x00002000
const CHAOTIC: int = 0x00004000
const VAMPIRIC: int = 0x00008000
const SLAY_ANIMAL: int = 0x00010000
const SLAY_EVIL: int = 0x00020000
const SLAY_UNDEAD: int = 0x00040000
const SLAY_DEMON: int = 0x00080000
const SLAY_ORC: int = 0x00100000
const SLAY_TROLL: int = 0x00200000
const SLAY_GIANT: int = 0x00400000
const SLAY_DRAGON: int = 0x00800000
const KILL_DRAGON: int = 0x01000000
const VORPAL: int = 0x02000000
const IMPACT: int = 0x04000000
const BRAND_POISON: int = 0x08000000
const BRAND_ACID: int = 0x10000000
const BRAND_ELECTRIC: int = 0x20000000
const BRAND_FIRE: int = 0x40000000
const BRAND_COLD: int = 0x80000000
const PvalMask: int = STR | INT | WIS | DEX | CON | CHA | STEALTH | SEARCH | INFRA | TUNNEL | SPEED | BLOWS
