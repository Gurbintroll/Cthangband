# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronTsathoggua
extends Patron


func _init() -> void:
	short_name = "Tsathoggua"
	long_name = "Tsathoggua, the Sleeper of N'Kai"
	preferred_ability = Ability.INTELLIGENCE
	rewards = [
		Reward.WRATH,
		Reward.PISS_OFF,
		Reward.RUIN_ABL,
		Reward.LOSE_EXP,
		Reward.H_SUMMON,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.MASS_GEN,
		Reward.SER_DEMO,
		Reward.HEAL_FUL,
		Reward.CHAOS_WP,
		Reward.CHAOS_WP,
		Reward.GOOD_OBJ,
		Reward.GAIN_EXP,
		Reward.GREA_OBJ,
		Reward.AUGM_ABL,
	]
