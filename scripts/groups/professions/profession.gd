@abstract # Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Profession
extends Resource
## The base class from which all professions inherit

static var _professions: Dictionary[String, Profession]

@export var ability_bonus: Array[int]
@export var attacks_per_turn_max: int
@export var attacks_per_turn_min_weapon_weight: int
@export var attacks_per_turn_weight_multiplier: int
@export var base_device_bonus: int
@export var base_disarm_bonus: int
@export var base_melee_attack_bonus: int
@export var base_ranged_attack_bonus: int
@export var base_save_bonus: int
@export var base_search_bonus: int
@export var base_search_frequency: int
@export var base_stealth_bonus: int
@export var blunt_weapons_only: bool
@export var casting_type: int
@export var chaos_weapons_only: bool
@export var description_line_1: String
@export var description_line_2: String
@export var description_line_3: String
@export var description_line_4: String
@export var description_line_5: String
@export var description_line_6: String
@export var device_bonus_per_level: int
@export var disarm_bonus_per_level: int
@export var experience_factor: int
@export var first_realm_choice: int
@export var glows: bool
@export var has_backstab: bool
@export var has_deity: bool
@export var has_detailed_sense_inventory: bool
@export var has_minimum_spell_failure: bool
@export var has_goo_patron: bool
@export var has_spell_failure_chance: bool
@export var has_spells: bool
@export var hit_die_bonus: int
@export var is_martial_artist: bool
@export var legendary_item_bias: int
@export var melee_attack_bonus_per_level: int
@export var pet_upkeep_divider: int
@export var prime_ability_score: int
@export var ranged_attack_bonus_per_level: int
@export var save_bonus_per_level: int
@export var search_bonus_per_level: int
@export var search_frequency_per_level: int
@export var second_realm_choice: int
@export var spell_ball_size_factor: int
@export var spell_weight_limit: int
@export var stealth_bonus_per_level: int
@export var title: String
@export var warrior_artifact_bias: int


static func _populate_professions() -> void:
	_professions = { }
	_professions.get_or_add("Channeller", ProfessionChanneller.new())
	_professions.get_or_add("Chosen One", ProfessionChosenOne.new())
	_professions.get_or_add("Cultist", ProfessionCultist.new())
	_professions.get_or_add("Druid", ProfessionDruid.new())
	_professions.get_or_add("Fanatic", ProfessionFanatic.new())
	_professions.get_or_add("High-Mage", ProfessionHighMage.new())
	_professions.get_or_add("Mage", ProfessionMage.new())
	_professions.get_or_add("Mindcrafter", ProfessionMindcrafter.new())
	_professions.get_or_add("Monk", ProfessionMonk.new())
	_professions.get_or_add("Mystic", ProfessionMystic.new())
	_professions.get_or_add("Paladin", ProfessionPaladin.new())
	_professions.get_or_add("Priest", ProfessionPriest.new())
	_professions.get_or_add("Ranger", ProfessionRanger.new())
	_professions.get_or_add("Rogue", ProfessionRogue.new())
	_professions.get_or_add("Summoner", ProfessionSummoner.new())
	_professions.get_or_add("Warrior", ProfessionWarrior.new())
	_professions.get_or_add("Warrior-Mage", ProfessionWarriorMage.new())
	_professions.get_or_add("Zealot", ProfessionZealot.new())


static func named_profession(name: String) -> Profession:
	if _professions.size() == 0:
		_populate_professions()
	return _professions[name]


static func sorted_profession_names() -> Array[String]:
	if _professions.size() == 0:
		_populate_professions()
	var keys: Array[String] = _professions.keys()
	keys.sort()
	return keys


func get_starting_items() -> Array[ItemIdentifier]:
	return []


func apply_attack_and_damage_bonus(player: Player) -> void:
	pass


func apply_bonus_melee_attacks(player: Player) -> void:
	pass


func apply_bonus_missile_attacks(player: Player) -> void:
	pass


func apply_statuses_from_profession(player: Player) -> void:
	pass


func apply_martial_artist_armour_class(player: Player) -> void:
	pass


func apply_power_bonus(max_power: int) -> int:
	return max_power


func choose_realm_randomly(choices: int, player: Player) -> int:
	# Simple case - there were no choices
	if choices == Realm.NONE:
		return Realm.NONE
	# More complex case - one or more realms specified
	var picks: Array[int] = []
	var realms: Array[int] = [
		Realm.LIFE,
		Realm.SORCERY,
		Realm.NATURE,
		Realm.CHAOS,
		Realm.DEATH,
		Realm.TAROT,
		Realm.FOLK,
		Realm.CORPOREAL,
	]
	# Add any realms that match the choices to our array
	for realm: int in realms:
		if choices & realm != 0:
			picks.append(realm)
	# Pick one from the array (remembering that arrays are zero-based)
	var pick: int = Rng.roll_die(picks.size())
	return picks[pick - 1]


func get_profession_sub_name(realm: int) -> String:
	return title


func gets_experience_for_destroying(player: Player, category: int) -> bool:
	return false


func get_abilities_as_item_flags(player: Player, f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	pass


func has_speed_bonus(player: Player) -> bool:
	return false


func martial_artist_heavy_armour(player: Player) -> bool:
	return false


func spell_base_failure_multiplier(realm: int) -> float:
	return 0.0


func spell_beam_chance(player_level: int) -> int:
	return player_level / 2


func spell_level_multiplier(realm: int) -> float:
	return 99.0


func spell_power_cost_multiplier(realm: int) -> float:
	return 0.0


func try_sense_inventory(player_level: int) -> bool:
	return false
