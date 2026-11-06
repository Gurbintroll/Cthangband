# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronShubNiggurath
extends Patron


func _init() -> void:
	short_name = "Shub Niggurath"
	long_name = "Shub Niggurath, Black Goat of the Woods"
	preferred_ability = Ability.INTELLIGENCE
	rewards = [
		Reward.WRATH,
		Reward.CURSE_WP,
		Reward.CURSE_AR,
		Reward.RUIN_ABL,
		Reward.LOSE_ABL,
		Reward.LOSE_EXP,
		Reward.IGNORE,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.HEAL_FUL,
		Reward.CHAOS_WP,
		Reward.GREA_OBJ,
		Reward.GAIN_ABL,
		Reward.GAIN_ABL,
		Reward.GAIN_EXP,
		Reward.GAIN_EXP,
		Reward.AUGM_ABL,
	]
