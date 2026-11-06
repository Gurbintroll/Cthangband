# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Religion
extends Resource

## How much favour decays towards resting favour per game day (10 = 1/10 of gained favour)
const DECAY_RATE: int = 10
## Resting favour for your patron deity
const PATRON_RESTING_FAVOUR: int = 30

## The gods, by GodName
@export var gods: Dictionary[int, God] = { }
## The GodName of your patron deity
@export var patron: int


## Set up the dictionary of gods
func _init() -> void:
	gods.get_or_add(GodName.LOBON, God.new())
	gods.get_or_add(GodName.TAMASH, God.new())
	gods.get_or_add(GodName.HAGARG_RYONIS, God.new())
	gods.get_or_add(GodName.NATH_HORTHATH, God.new())
	gods.get_or_add(GodName.ZO_KALAR, God.new())
	gods[GodName.LOBON].set_god_name(GodName.LOBON)
	gods[GodName.TAMASH].set_god_name(GodName.TAMASH)
	gods[GodName.HAGARG_RYONIS].set_god_name(GodName.HAGARG_RYONIS)
	gods[GodName.NATH_HORTHATH].set_god_name(GodName.NATH_HORTHATH)
	gods[GodName.ZO_KALAR].set_god_name(GodName.ZO_KALAR)


## Set up the character with a patron deity
func initialise_worship(worshipped: int) -> void:
	patron = worshipped
	# if no patron then all gods start with zero worship
	if patron == GodName.NONE:
		for name: int in gods:
			gods[name].is_patron = false
			gods[name].resting_favour = 0
			gods[name].favour = 0
	else:
		# If we have a patron then that god starts with four times the resting favour and all other start with negative favour
		# Note that favour is a zero-sum game, so the starting favours all add up to zero
		for name: int in gods:
			if name == patron:
				gods[name].is_patron = true
				gods[name].resting_favour = PATRON_RESTING_FAVOUR * 4
				gods[name].favour = PATRON_RESTING_FAVOUR * 4
			else:
				gods[name].is_patron = false
				gods[name].resting_favour = -PATRON_RESTING_FAVOUR
				gods[name].favour = -PATRON_RESTING_FAVOUR


## Take favour from each of the other gods and give it to the specified god
func add_favour(to: int, amount: int) -> void:
	# Favour is a zero-sum game, so we make sure to add and subtract the same amount in total
	for god_name: int in gods:
		if god_name == to:
			gods[god_name].favour = gods[god_name].favour + (4 * amount)
		else:
			gods[god_name].favour = gods[god_name].favour - amount


## Move favour of all gods back towards its resting state
func decay_favour() -> void:
	var max_diff: int = 0
	var is_max: int = GodName.NONE
	# Work out which god has the most favour above its resting state
	for god_name: int in gods:
		if gods[god_name].favour - gods[god_name].resting_favour > max_diff:
			max_diff = gods[god_name].favour - gods[god_name].resting_favour
			is_max = god_name
	# If at least one god had more favour, remove a portion of it to give back to the other gods
	if is_max != GodName.NONE:
		var decrement: int = max(max_diff / DECAY_RATE, 1)
		add_favour(is_max, decrement)


## Get the player's patron deity
func get_patron() -> God:
	if patron == GodName.NONE:
		return null
	else:
		return gods[patron]
