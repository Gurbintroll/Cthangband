# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MonsterFlags6 extends Object


const HASTE:int =                0x00000001 
const DREAD_CURSE:int =          0x00000002 
const HEAL:int =                 0x00000004 
#const UNUSED_1:int =             0x00000008 
const BLINK:int =                0x00000010
const TELEPORT_SELF:int =        0x00000020 
#const UNUSED_2:int =             0x00000040 
#const UNUSED_3:int =             0x00000080 
const TELEPORT_TO:int =          0x00000100 
const TELEPORT_AWAY:int =        0x00000200 
const TELEPORT_LEVEL:int =       0x00000400 
#const UNUSED_4:int =             0x00000800 
const DARKNESS:int =             0x00001000 
const CREATE_TRAPS:int =         0x00002000 
const FORGET:int =               0x00004000 
#const UNUSED_5:int =             0x00008000 
const SUMMON_KIN:int =           0x00010000 
const SUMMON_REAVER:int =        0x00020000 
const SUMMON_MONSTER:int =       0x00040000 
const SUMMON_MONSTERS:int =      0x00080000 
const SUMMON_ANT:int =           0x00100000 
const SUMMON_SPIDER:int =        0x00200000 
const SUMMON_HOUND:int =         0x00400000 
const SUMMON_HYDRA:int =         0x00800000 
const SUMMON_CTHULOID:int =      0x01000000 
const SUMMON_DEMON:int =         0x02000000 
const SUMMON_UNDEAD:int =        0x04000000 
const SUMMON_DRAGON:int =        0x08000000 
const SUMMON_HI_UNDEAD:int =     0x10000000 
const SUMMON_HI_DRAGON:int =     0x20000000 
const SUMMON_GREAT_OLD_ONE:int = 0x40000000 
const SUMMON_UNIQUE:int =        0x80000000 
const BOLT_MASK:int = 0
const INT_MASK:int = BLINK | TELEPORT_SELF | TELEPORT_LEVEL | TELEPORT_AWAY | HEAL | HASTE | CREATE_TRAPS | SUMMON_KIN | SUMMON_REAVER | SUMMON_MONSTER | SUMMON_MONSTERS | SUMMON_ANT | SUMMON_SPIDER | SUMMON_HOUND | SUMMON_HYDRA | SUMMON_CTHULOID | SUMMON_DRAGON | SUMMON_UNDEAD | SUMMON_DEMON | SUMMON_HI_DRAGON | SUMMON_HI_UNDEAD | SUMMON_GREAT_OLD_ONE | SUMMON_UNIQUE
const SUMMON_MASK:int = SUMMON_KIN | SUMMON_REAVER | SUMMON_MONSTER | SUMMON_MONSTERS | SUMMON_ANT | SUMMON_SPIDER | SUMMON_HOUND | SUMMON_HYDRA | SUMMON_CTHULOID | SUMMON_DEMON | SUMMON_UNDEAD | SUMMON_DRAGON | SUMMON_HI_UNDEAD | SUMMON_HI_DRAGON | SUMMON_GREAT_OLD_ONE | SUMMON_UNIQUE 
