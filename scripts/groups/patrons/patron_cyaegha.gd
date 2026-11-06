# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronCyaegha
extends Patron


func _init() -> void:
	short_name = "Cyaegha"
	long_name = "Cyaegha, the Destroying Eye"
	preferred_ability = Ability.STRENGTH
	rewards = [
		Reward.WRATH,
		Reward.HURT_LOT,
		Reward.PISS_OFF,
		Reward.LOSE_ABL,
		Reward.LOSE_EXP,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.DISPEL_C,
		Reward.DO_HAVOC,
		Reward.DO_HAVOC,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.GAIN_EXP,
		Reward.GAIN_ABL,
		Reward.GAIN_ABL,
		Reward.SER_MONS,
		Reward.GOOD_OBJ,
		Reward.CHAOS_WP,
		Reward.GREA_OBJ,
		Reward.GOOD_OBS,
	]
