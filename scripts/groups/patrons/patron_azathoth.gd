# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronAzathoth
extends Patron


func _init() -> void:
	short_name = "Azathoth"
	long_name = "Azathoth, the Daemon Sultan"
	preferred_ability = Ability.STRENGTH
	rewards = [
		Reward.DREAD_CURSE,
		Reward.DREAD_CURSE,
		Reward.PISS_OFF,
		Reward.RUIN_ABL,
		Reward.LOSE_ABL,
		Reward.IGNORE,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.POLY_SLF,
		Reward.CARNAGE,
		Reward.DISPEL_C,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBJ,
		Reward.SER_MONS,
		Reward.GAIN_ABL,
		Reward.CHAOS_WP,
		Reward.GAIN_EXP,
		Reward.AUGM_ABL,
		Reward.GOOD_OBS,
	]
