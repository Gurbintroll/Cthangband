# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronYogSothoth
extends Patron


func _init() -> void:
	short_name = "Yog Sothoth"
	long_name = "Yog Sothoth, the Gate and the Key"
	preferred_ability = Ability.STRENGTH
	rewards = [
		Reward.WRATH,
		Reward.WRATH,
		Reward.HURT_LOT,
		Reward.PISS_OFF,
		Reward.H_SUMMON,
		Reward.SUMMON_M,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.DESTRUCT,
		Reward.SER_UNDE,
		Reward.CARNAGE,
		Reward.MASS_GEN,
		Reward.MASS_GEN,
		Reward.DISPEL_C,
		Reward.GOOD_OBJ,
		Reward.CHAOS_WP,
		Reward.GOOD_OBS,
		Reward.GOOD_OBJ,
		Reward.AUGM_ABL,
		Reward.AUGM_ABL,
	]
