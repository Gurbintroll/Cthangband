# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronNyogtha
extends Patron


func _init() -> void:
	short_name = "Nyogtha"
	long_name = "Nyogtha, the Thing Which Should Not Be"
	preferred_ability = Ability.CONSTITUTION
	rewards = [
		Reward.WRATH,
		Reward.DREAD_CURSE,
		Reward.PISS_OFF,
		Reward.CURSE_WP,
		Reward.RUIN_ABL,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBJ,
		Reward.SER_MONS,
		Reward.HEAL_FUL,
		Reward.GAIN_EXP,
		Reward.GAIN_ABL,
		Reward.CHAOS_WP,
		Reward.GOOD_OBS,
		Reward.GREA_OBJ,
		Reward.AUGM_ABL,
	]
