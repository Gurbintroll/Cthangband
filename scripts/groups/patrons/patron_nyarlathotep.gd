# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronNyarlathotep
extends Patron


func _init() -> void:
	short_name = "Nyarlathotep"
	long_name = "Nyarlathotep, the Crawling Chaos"
	preferred_ability = Ability.STRENGTH
	rewards = [
		Reward.DREAD_CURSE,
		Reward.HURT_LOT,
		Reward.CURSE_WP,
		Reward.CURSE_AR,
		Reward.RUIN_ABL,
		Reward.SUMMON_M,
		Reward.LOSE_EXP,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.SER_UNDE,
		Reward.HEAL_FUL,
		Reward.HEAL_FUL,
		Reward.GAIN_EXP,
		Reward.GAIN_EXP,
		Reward.CHAOS_WP,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBS,
		Reward.GREA_OBS,
		Reward.AUGM_ABL,
	]
