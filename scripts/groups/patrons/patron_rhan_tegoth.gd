# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PatronRhanTegoth
extends Patron


func _init() -> void:
	short_name = "Rhan-Tegoth"
	long_name = "Rhan-Tegoth, He of the Ivory Throne"
	preferred_ability = Ability.INTELLIGENCE
	rewards = [
		Reward.WRATH,
		Reward.DREAD_CURSE,
		Reward.PISS_OFF,
		Reward.H_SUMMON,
		Reward.H_SUMMON,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.IGNORE,
		Reward.POLY_WND,
		Reward.POLY_SLF,
		Reward.POLY_SLF,
		Reward.SER_DEMO,
		Reward.HEAL_FUL,
		Reward.GAIN_ABL,
		Reward.GAIN_ABL,
		Reward.CHAOS_WP,
		Reward.DO_HAVOC,
		Reward.GOOD_OBJ,
		Reward.GREA_OBJ,
		Reward.GREA_OBS,
	]
