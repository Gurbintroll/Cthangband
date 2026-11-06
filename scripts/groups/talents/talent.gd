# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Talent
extends Resource

@export var base_failure: int
@export var level: int
@export var name: String
@export var power_cost: int


func failure_chance(player: Player) -> int:
	# Start with the base failure chance of the talent
	var chance: int = base_failure
	# Chance is improved by 3% per level we are above the talent's level (can be negative)
	chance -= 3 * (player.level - level)
	# Chance is improved for having a high casting stat
	chance -= 3 * (player.ability_scores[player.spellcasting.spell_stat].spell_failure_reduction() - 1)
	# Chance is made worse if we're overcasting
	if (power_cost > player.power):
		chance += 5 * (power_cost - player.power)
	# Get the minimum failure rate from having a high casting stat
	var min_fail: int = player.ability_scores[player.spellcasting.spell_stat].spell_min_fail_chance()
	# Make sure the failure chance is not less than the minimum
	if chance < min_fail:
		chance = min_fail
	# Being stunned increases chance even after minimum
	if player.timed_stun > 50:
		chance += 25
	elif player.timed_stun != 0:
		chance += 15
	# Always have a chance of success
	if chance > 95:
		chance = 95
	return chance


func initialise(profession: Profession) -> void:
	pass


func comment(player: Player) -> String:
	return ""


func summary_line(player: Player) -> String:
	return "%-30s%2d %4d %4d%% %s" % [name, level, power_cost, failure_chance(player), comment(player)]


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	pass
