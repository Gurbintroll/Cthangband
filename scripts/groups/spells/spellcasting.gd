# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Spellcasting
extends Resource

@export var spell_first: int
@export var spell_order: Array[int]
@export var spells: Array[SpellList]
@export var spell_stat: int
@export var spell_weight: int
@export var talents: TalentList
@export var casting_type: int


static func realm_name(realm: int) -> String:
	match realm:
		Realm.NONE:
			return "None"
		Realm.CHAOS:
			return "Chaos"
		Realm.CORPOREAL:
			return "Corporeal"
		Realm.DEATH:
			return "Death"
		Realm.FOLK:
			return "Folk"
		Realm.LIFE:
			return "Life"
		Realm.NATURE:
			return "Nature"
		Realm.SORCERY:
			return "Sorcery"
		Realm.TAROT:
			return "Tarot"
	return "Unknown Realm"


func init_spellcasting(player: Player) -> void:
	# Initialise the talent list
	talents = TalentList.new()
	talents.init_talent_list(player.profession)
	# Initialise our spell list from the player's chosen realms
	spells.append(SpellList.new())
	spells.append(SpellList.new())
	spells[0].init_spell_list(player.realm_1, player.profession)
	spells[1].init_spell_list(player.realm_2, player.profession)
	# Grab other variables from the player's profession
	casting_type = player.profession.casting_type
	spell_stat = player.profession.prime_ability_score
	spell_weight = player.profession.spell_weight_limit
	# Detect when we get our first spell
	spell_first = 100
	for bookset: SpellList in spells:
		for spell: Spell in bookset.spells:
			if spell.level < spell_first:
				spell_first = spell.level
	# Set the order in which we've learned spells to show we haven't learned any yet
	spell_order.resize(64)
	spell_order.fill(99)
