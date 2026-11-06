@abstract # Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Spell
extends Resource

@export var base_failure: int
@export var level: int
@export var power_cost: int
@export var worked: bool
@export var name: String
@export var forgotten: bool
@export var learned: bool
@export var first_cast_experience: int
@export var default_base_failure: int
@export var default_level: int
@export var default_power_cost: int


@abstract func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void


func failure_chance(player: Player) -> int:
	# If we can't cast spells then chance is 100%
	if player.spellcasting.casting_type == CastingType.NONE:
		return 100
	# Start with the base failure chance of the spell
	var chance: int = base_failure
	# Chance is improved by 3% per level we are above the spell's level (can be negative)
	chance -= 3 * (player.level - level)
	# Chance is improved for having a high casting stat
	chance -= 3 * (player.ability_scores[player.spellcasting.spell_stat].spell_failure_reduction() - 1)
	# Chance is made worse if we're overcasting
	if (power_cost > player.power):
		chance += 5 * (power_cost - player.power)
	# Extra 25% chance of failure from priest having an edged weapon or chaos mage having a non-chaotic weapoin
	if player.profession.blunt_weapons_only and player.has_unpriestly_weapon:
		chance += 25
	if player.profession.chaos_weapons_only and player.has_unpriestly_weapon:
		chance += 25
	# Get the minimum failure rate from having a high casting stat
	var min_fail: int = player.ability_scores[player.spellcasting.spell_stat].spell_min_fail_chance()
	# Some professions simply can't get to 0% failure
	if player.profession.has_minimum_spell_failure:
		if min_fail < 5:
			min_fail = 5
	# Make sure the failure chance is not less than the minimum
	if chance < min_fail:
		chance = min_fail
	# Being stunned increases chance even after minimum
	if player.timed_stun > 50:
		chance += 25
	# Always have a chance of success
	if chance > 95:
		chance = 95
	return chance


@abstract func comment(player: Player) -> String


func get_comment(player: Player) -> String:
	if forgotten:
		return "forgotten"
	if not learned:
		return "unknown"
	return comment(player) if worked else "untried"


func initialise(profession: Profession, realm: int) -> void:
	level = ceil(default_level * profession.spell_level_multiplier(realm))
	power_cost = ceil(default_power_cost * profession.spell_power_cost_multiplier(realm))
	base_failure = ceil(default_base_failure * profession.spell_base_failure_multiplier(realm))
	if level < 1:
		level = 1
	if power_cost < 1:
		power_cost = 1
	if level > 50:
		level = 99
		power_cost = 0
		base_failure = 0


func summary_line(player: Player) -> String:
	if level >= 99:
		return "(illegible)"
	else:
		return "%-30s %3d %4d %3d%% %s" % [name, level, power_cost, failure_chance(player), get_comment(player)] if level < 99 else "{illegible}"
