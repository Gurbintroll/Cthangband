# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronAbhoth
extends Patron


func _init() -> void:
	short_name = "Abhoth"
	long_name = "Abhoth, the Unclean"
	preferred_ability = Ability.STRENGTH
	rewards = [
		Reward.WRATH,
		Reward.HURT_LOT,
		Reward.HURT_LOT,
		Reward.H_SUMMON,
		Reward.H_SUMMON,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.SER_MONS,
		Reward.SER_DEMO,
		Reward.POLY_SLF,
		Reward.POLY_WND,
		Reward.HEAL_FUL,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBJ,
		Reward.CHAOS_WP,
		Reward.GOOD_OBS,
		Reward.GOOD_OBS,
		Reward.GREA_OBJ,
		Reward.GREA_OBS,
	]
