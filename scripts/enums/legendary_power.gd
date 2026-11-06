# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name LegendaryPower
extends Object

const SUNLIGHT: int = 1
const BO_MISS_1: int = 2
const BA_POIS_1: int = 3
const BO_ELEC_1: int = 4
const BO_ACID_1: int = 5
const BO_COLD_1: int = 6
const BO_FIRE_1: int = 7
const BA_COLD_1: int = 8
const BA_FIRE_1: int = 9
const DRAIN_1: int = 10
const BA_COLD_2: int = 11
const BA_ELEC_2: int = 12
const DRAIN_2: int = 13
const VAMPIRE_1: int = 14
const BO_MISS_2: int = 15
const BA_FIRE_2: int = 16
const BA_COLD_3: int = 17
const BA_ELEC_3: int = 18
const WHIRLWIND: int = 19
const VAMPIRE_2: int = 20
const CALL_CHAOS: int = 21
const SHARD: int = 22
const DISP_EVIL: int = 23
const BA_MISS_3: int = 24
const DISP_GOOD: int = 25
const CONFUSE: int = 51
const SLEEP: int = 52
const QUAKE: int = 53
const TERROR: int = 54
const TELE_AWAY: int = 55
const BANISH_EVIL: int = 56
const CARNAGE: int = 57
const MASS_GENO: int = 58
const CHARM_ANIMAL: int = 65
const CHARM_UNDEAD: int = 66
const CHARM_OTHER: int = 67
const CHARM_ANIMALS: int = 68
const CHARM_OTHERS: int = 69
const SUMMON_ANIMAL: int = 70
const SUMMON_PHANTOM: int = 71
const SUMMON_ELEMENTAL: int = 72
const SUMMON_DEMON: int = 73
const SUMMON_UNDEAD: int = 74
const CURE_L_W: int = 81
const CURE_M_W: int = 82
const CURE_POISON: int = 83
const REST_LIFE: int = 84
const REST_ALL: int = 85
const CURE_700: int = 86
const CURE_1000: int = 87
const ESP: int = 91
const BERSERK: int = 92
const PROT_EVIL: int = 93
const RESIST_ALL: int = 94
const SPEED: int = 95
const XTRA_SPEED: int = 96
const WRAITH: int = 97
const INVULN: int = 98
const LIGHT: int = 111
const DETECT_ALL: int = 113
const MAP_LIGHT: int = 112
const DETECT_XTRA: int = 114
const ID_FULL: int = 115
const ID_PLAIN: int = 116
const RUNE_EXPLO: int = 117
const RUNE_PROT: int = 118
const SATIATE: int = 119
const DEST_DOOR: int = 120
const STONE_MUD: int = 121
const RECHARGE: int = 122
const ALCHEMY: int = 123
const DIM_DOOR: int = 124
const TELEPORT: int = 125
const RECALL: int = 126
