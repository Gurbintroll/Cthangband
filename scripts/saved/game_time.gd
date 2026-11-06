# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name GameTime
extends Resource

const LEVEL_FEEL_DELAY: int = 2500
# One turn is 0.8 seconds
const TURNS_PER_MINUTE: int = 75
const TURNS_PER_HOUR: int = 4500
const TURNS_PER_DAY: int = 108000
const DAYS_PER_YEAR: int = 365
const SECONDS_PER_DAY: int = 86400

@export var is_birthday: bool
@export var is_dawn: bool
@export var is_dusk: bool
@export var is_feel_time: bool
@export var level_feel: bool
@export var is_halloween: bool
@export var is_midnight: bool
@export var is_new_year: bool
@export var birthday: int
@export var dawn_turn: int
@export var dusk_turn: int
@export var current_turn: int
@export var level_entry_turn: int
@export var current_turn_in_day: int
@export var current_day_of_year: int
@export var game_start_day_in_year: int
@export var game_start_turn_in_day: int
@export var is_turn_ten: bool
@export var is_turn_hundred: bool


static func _day_string(day: int) -> String:
	var suffix: String
	match day:
		1, 21, 31:
			suffix = "st"
		2, 22:
			suffix = "nd"
		3, 23:
			suffix = "rd"
		_:
			suffix = "th"
	return "%d%s" % [day, suffix]


static func get_current_datetime() -> String:
	var months: Array[String] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
	var time:Dictionary = Time.get_datetime_dict_from_system()
	var date_portion: String = "%s %s %d" % [_day_string(time["day"]), months[time["month"] - 1], time["year"]]
	var hour: int = time["hour"]
	var am_pm: String = "AM"
	if hour > 11:
		am_pm = "PM"
		hour -= 12
	if hour == 0:
		hour = 12
	var time_portion: String = "%d:%d %s" % [hour, time["minute"], am_pm]
	return date_portion + " " + time_portion


func set_initial_time(start_date: int, start_at_dusk: bool) -> void:
	current_day_of_year = start_date
	birthday = start_date
	_recalculate_dawn_and_dusk()
	current_turn_in_day = dusk_turn if start_at_dusk else dawn_turn
	game_start_day_in_year = start_date
	game_start_turn_in_day = current_turn_in_day
	tick()


func tick() -> void:
	var old_day: int = current_day_of_year
	current_turn += 1
	current_turn_in_day += 1
	# turns in a day are 0-107,999
	while current_turn_in_day >= TURNS_PER_DAY:
		current_turn_in_day -= TURNS_PER_DAY
		current_day_of_year += 1
		# NOTE: day of year is 1-365, not 0-364
		# NOTE: we assume no leap years, since we don't track a year number anyway
		if current_day_of_year > DAYS_PER_YEAR:
			current_day_of_year -= DAYS_PER_YEAR
	is_turn_ten = false
	is_turn_hundred = false
	is_birthday = false
	is_dawn = false
	is_dusk = false
	is_feel_time = false
	is_halloween = false
	is_midnight = false
	is_new_year = false
	level_feel = false
	if current_turn % 10 == 0:
		is_turn_ten = true
		if current_turn % 100 == 0:
			is_turn_hundred = true
	if current_turn - level_entry_turn == LEVEL_FEEL_DELAY:
		is_feel_time = true
	if current_turn - level_entry_turn >= LEVEL_FEEL_DELAY:
		level_feel = true
	if current_turn_in_day == dawn_turn:
		is_dawn = true
	if current_turn_in_day == dusk_turn:
		is_dusk = true
	if old_day != current_day_of_year:
		is_midnight = true
		_recalculate_dawn_and_dusk()
		if current_day_of_year == 1:
			is_new_year = true
		if current_day_of_year == birthday:
			is_birthday = true
		if current_day_of_year == 305:
			is_halloween = true


func get_birthday_text() -> String:
	var months: Array[String] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
	var unix_time: int = birthday * SECONDS_PER_DAY
	var time:Dictionary = Time.get_datetime_dict_from_unix_time(unix_time)
	return "%s %s" % [months[time["month"] - 1], _day_string(time["day"])]


func date_string() -> String:
	var months: Array[String] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
	var unix_time: int = current_day_of_year * SECONDS_PER_DAY
	var time: Dictionary = Time.get_datetime_dict_from_unix_time(unix_time)
	return ("%s %s" % [months[time["month"] - 1], _day_string(time["day"])]).lpad(8)


func time_string() -> String:
	var hour: int = current_turn_in_day / TURNS_PER_HOUR
	var minute: int = (current_turn_in_day - (TURNS_PER_HOUR * hour)) / TURNS_PER_MINUTE
	var am_pm: String = "AM"
	if hour > 11:
		am_pm = "PM"
		hour -= 12
	if hour == 0:
		hour = 12
	return "%2d:%02d %s" % [hour, minute, am_pm]


func mark_level_entry() -> void:
	level_entry_turn = current_turn


func is_light() -> bool:
	return current_turn_in_day >= dawn_turn and current_turn_in_day <= dusk_turn


func to_next_dawn()->void:
	# Start by making it midnight
	var turns_to_midnight:int = (TURNS_PER_DAY-1) - current_turn_in_day
	current_turn_in_day = 0
	current_turn += turns_to_midnight
	# Advance the day
	current_day_of_year += 1
	if current_day_of_year > DAYS_PER_YEAR:
		current_day_of_year -= DAYS_PER_YEAR
	# Then advance to dawn
	_recalculate_dawn_and_dusk()
	current_turn += (dawn_turn - 1)
	current_turn_in_day += (dawn_turn - 1)
	tick()


func to_next_dusk()->void:
	# Start by making it midnight
	var turns_to_midnight:int = (TURNS_PER_DAY-1) - current_turn_in_day
	current_turn_in_day = 0
	current_turn += turns_to_midnight
	# Advance the day
	current_day_of_year += 1
	if current_day_of_year > DAYS_PER_YEAR:
		current_day_of_year -= DAYS_PER_YEAR
	# Then advance to dawn
	_recalculate_dawn_and_dusk()
	current_turn += dusk_turn
	current_turn_in_day += dusk_turn
	tick()


func _recalculate_dawn_and_dusk() -> void:
	# Declination of sun at the current day of the year, converted to radians
	var solar_declination: float = 23.45 * sin((360.0 / 365.0 * (current_day_of_year as float + 284.0)) * (PI / 180.0)) * (PI / 180.0)
	# Assumed latitude of the island (actually latitude of the author's home town), also converted to radians
	var assumed_latitude: float = 50.838 * (PI / 180.0)
	# Calculate solar hour angle from declination and latitude, and convert back to degrees
	var solar_hour_angle: float = acos(tan(solar_declination) * -tan(assumed_latitude)) * (180.0 / PI)
	# Solar hour angle changes at 15 degress per hour
	var sunrise_hours_before_noon: float = solar_hour_angle / 15.0
	# Convert a time into a number of turns
	var sunrise_turns_before_noon: float = sunrise_hours_before_noon * TURNS_PER_HOUR
	dawn_turn = TURNS_PER_DAY / 2 - (sunrise_turns_before_noon as int)
	dusk_turn = TURNS_PER_DAY - dawn_turn
