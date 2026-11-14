# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name HighScoreTable
extends Resource

@export var scores: Array[HighScore]

var profession_filter: String
var ancestry_filter: String


static func load_or_create() -> HighScoreTable:
	if FileAccess.file_exists(Constants.HISCORE_FILE_PATH):
		var high_score_table: HighScoreTable = ResourceLoader.load(Constants.HISCORE_FILE_PATH)
		high_score_table.clear_score_flags()
		return high_score_table
	# If the load failed, create a new one instead
	return HighScoreTable.new()


func clear_score_flags() -> void:
	for score: HighScore in scores:
		score.living = false
		score.highlight = false
		score.index = 0


func count() -> void:
	return scores.size()


func display_top_scores() -> void:
	clear_score_flags()
	display_scores(-1, null)


func display_scores_around_score(centred_score_index: int) -> void:
	clear_score_flags()
	display_scores(centred_score_index, null)


func display_scores_around_living(living: HighScore) -> void:
	clear_score_flags()
	display_scores(-1, living)


func insert_new_score(score: HighScore) -> void:
	insert_score(scores, score)
	clear_score_flags()
	save()


func display_scores(centred_score: int, living: HighScore) -> void:
	var collected_scores: Array[HighScore] = []
	# Five above ours, ours, and five below
	if centred_score >= 0:
		collected_scores = filter_by_score(centred_score)
	# Five above ours, ours, and five below
	elif living != null:
		collected_scores = filter_by_high_score(living)
	else:
		# All high scores, including the current save
		collected_scores = get_all_high_scores()
	if collected_scores.size() == 0:
		Console.clear_screen()
		Console.set_background(BackgroundImage.DEFAULT)
		Console.set_mask(-1,-1,-1,-1)
		Console.print_at(1, 34, "High Scores", Ink.YELLOW)
		Console.print_at(2, 34, "===========", Ink.YELLOW)
		Console.any_key(43)
		return
	# collected_scores now holds all the scores we're interested in - probably only a single
	# page of them
	var line: int = 0
	while collected_scores.size() > 0:
		if line == 0:
			Console.clear_screen()
			Console.set_mask(-1,-1,-1,-1)
			Console.set_background(BackgroundImage.DEFAULT)
			Console.print_at(1, 34, "High Scores", Ink.YELLOW)
			Console.print_at(2, 34, "===========", Ink.YELLOW)
		show_score(collected_scores[0], line)
		line += 1
		collected_scores.remove_at(0)
		if line > 9 or collected_scores.size() == 0:
			line = 0
			Console.any_key(43)


func filter_by_high_score(living: HighScore) -> Array[HighScore]:
	var index: int = 0
	var use_scores: Array[HighScore]
	for score: HighScore in scores:
		use_scores.append(score)
	var list: Array[HighScore] = []
	# Gather all the scores above ours
	for score: HighScore in use_scores:
		if score.points >= living.points:
			list.append(score)
			score.index = index + 1
			index += 1
		else:
			break
	strip_unwanted(list)
	# Add ours
	list.append(living)
	living.living = true
	living.index = index + 1
	# Remove the excess
	while list.size() > 6:
		list.remove_at(0)
	var below: int = 10 - list.size()
	# Add some more below (shifting their index by one)
	for i: int in below:
		if index < use_scores.size():
			list.append(use_scores[index])
			use_scores[index].index = index + 2
			index += 1
	strip_unwanted(list)
	return list


func filter_by_score(centred_score: int) -> Array[HighScore]:
	var index: int = 0
	var use_scores: Array[HighScore]
	for score: HighScore in scores:
		use_scores.append(score)
	var list: Array[HighScore] = []
	# Gather all the scores above and including ours
	for score: HighScore in use_scores:
		if score.points >= centred_score:
			list.append(score)
			score.index = index + 1
			index += 1
		else:
			break
	strip_unwanted(list)
	# Remove the excess
	while list.size() > 6:
		list.remove_at(0)
	# Hilight the chosen one
	list[list.size() - 1].highlight = true
	var below: int = 10 - list.size()
	# Add some more below (shifting their index by one)
	for i: int in below:
		if index < use_scores.size():
			list.append(use_scores[index])
			use_scores[index].index = index + 2
			index += 1
	strip_unwanted(list)
	return list


func get_all_high_scores() -> Array[HighScore]:
	var use_scores: Array[HighScore] = insert_from_save()
	var list: Array[HighScore] = []
	var index: int = 0
	for score: HighScore in use_scores:
		list.append(score)
		score.index = index + 1
		index += 1
	strip_unwanted(list)
	return list


func insert_from_save() -> Array[HighScore]:
	var list: Array[HighScore] = []
	for score: HighScore in scores:
		list.append(score)
	var score_from_save: HighScore = get_high_score_from_save()
	if score_from_save != null:
		insert_score(list, score_from_save)
	return list


func insert_score(list: Array[HighScore], score: HighScore) -> void:
	for i: int in list.size():
		if list[i].points < score.points:
			list.insert(i, score)
			if list.size() > 99:
				list.remove_at(99)
			return
	# We fell out of the loop so either we have no scores or they're all better than us
	if list.size() <= 99:
		list.append(score)


func save() -> void:
	clear_score_flags()
	ResourceSaver.save(self, Constants.HISCORE_FILE_PATH)


func show_score(score: HighScore, line: int) -> void:
	var color: int = Ink.WHITE
	if line % 2 == 1:
		color = Ink.GREY
	if score.highlight:
		color = Ink.BRIGHT_RED
	if score.living:
		color = Ink.BRIGHT_GREEN
		Console.print_at((line * 3) + 5, 1, "%2d) %5d %s" % [score.index, score.points, score.who], color)
		Console.print_at((line * 3) + 6, 11, "killed by %s" % [score.how], color)
		Console.print_at((line * 3) + 7, 11, score.where, color)
	else:
		Console.print_at((line * 3) + 5, 1, "%2d) %5d %s" % [score.index, score.points, score.who], color)
		Console.print_at((line * 3) + 6, 11, "killed, %s, by %s" % [score.when, score.how], color)
		Console.print_at((line * 3) + 7, 11, score.where, color)


func strip_unwanted(list: Array[HighScore]) -> void:
	var index: int = 0
	if ancestry_filter == "" and profession_filter == "":
		return
	if list.size() == 0:
		return
	while index < list.size():
		if ancestry_filter != "":
			if list[index].current_ancestry != ancestry_filter:
				list.remove_at(index)
			else:
				index += 1
		elif profession_filter != "":
			if list[index].current_profession != profession_filter:
				list.remove_at(index)
			else:
				index += 1


func get_high_score_from_save() -> HighScore:
	var high_score: HighScore
	var file_name: String = Constants.SAVE_FILE_PATH
	# First attempt to load the profile
	var loaded: Profile
	if FileAccess.file_exists(file_name):
		loaded = ResourceLoader.load(file_name)
	# If the load failed, create a new one instead
	if loaded != null:
		if loaded.savegame != null:
			if loaded.savegame.player != null:
				high_score = HighScore.new_highscore(loaded.savegame.player, loaded.savegame)
	return high_score
