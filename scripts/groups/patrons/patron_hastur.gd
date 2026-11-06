# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronHastur
extends Patron


func _init() -> void:
	short_name = "Hastur"
	long_name = "Hastur, the Unspeakable"
	preferred_ability = -1
	rewards = [
		Reward.WRATH,
		Reward.SER_DEMO,
		Reward.CHAOS_WP,
		Reward.CURSE_AR,
		Reward.LOSE_EXP,
		Reward.GAIN_ABL,
		Reward.LOSE_ABL,
		Reward.POLY_WND,
		Reward.POLY_SLF,
		Reward.IGNORE,
		Reward.DESTRUCT,
		Reward.MASS_GEN,
		Reward.CHAOS_WP,
		Reward.GREA_OBJ,
		Reward.HURT_LOT,
		Reward.AUGM_ABL,
		Reward.RUIN_ABL,
		Reward.H_SUMMON,
		Reward.GREA_OBS,
		Reward.AUGM_ABL,
	]
