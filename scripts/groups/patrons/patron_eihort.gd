# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronEihort
extends Patron


func _init() -> void:
	short_name = "Eihort"
	long_name = "Eihort, God of the Labyrinth"
	preferred_ability = Ability.CONSTITUTION
	rewards = [
		Reward.WRATH,
		Reward.CURSE_WP,
		Reward.CURSE_AR,
		Reward.RUIN_ABL,
		Reward.LOSE_ABL,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_WND,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.GAIN_ABL,
		Reward.GAIN_ABL,
		Reward.GAIN_EXP,
		Reward.GOOD_OBJ,
		Reward.CHAOS_WP,
		Reward.GREA_OBJ,
		Reward.AUGM_ABL,
		Reward.AUGM_ABL,
	]
