# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronIod
extends Patron


func _init() -> void:
	short_name = "Iod"
	long_name = "Iod, the Shining Hunter"
	preferred_ability = Ability.CHARISMA
	rewards = [
		Reward.WRATH,
		Reward.CURSE_AR,
		Reward.CURSE_WP,
		Reward.CURSE_WP,
		Reward.CURSE_AR,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.HEAL_FUL,
		Reward.HEAL_FUL,
		Reward.GAIN_EXP,
		Reward.AUGM_ABL,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBJ,
		Reward.CHAOS_WP,
		Reward.GREA_OBJ,
		Reward.GREA_OBS,
	]
