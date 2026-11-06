# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronGlaaki
extends Patron


func _init() -> void:
	short_name = "Glaaki"
	long_name = "Glaaki, Lord of Dead Dreams"
	preferred_ability = Ability.CONSTITUTION
	rewards = [
		Reward.WRATH,
		Reward.CURSE_WP,
		Reward.CURSE_AR,
		Reward.H_SUMMON,
		Reward.SUMMON_M,
		Reward.SUMMON_M,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_WND,
		Reward.POLY_WND,
		Reward.POLY_SLF,
		Reward.HEAL_FUL,
		Reward.HEAL_FUL,
		Reward.GAIN_ABL,
		Reward.SER_UNDE,
		Reward.CHAOS_WP,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBJ,
		Reward.GOOD_OBS,
		Reward.GOOD_OBS,
	]
