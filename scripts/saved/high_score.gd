# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name HighScore
extends Resource

@export var current_profession: String
@export var current_ancestry: String
@export var how: String
@export var when: String
@export var points: int
@export var where: String
@export var who: String
@export var highlight: bool
@export var index: int
@export var living: bool


static func new_highscore(player: Player, savegame: SaveGame) -> HighScore:
	var highscore: HighScore = HighScore.new()
	highscore.points = player.get_score(savegame)
	highscore.current_ancestry = player.current_ancestry
	highscore.current_profession = player.current_profession
	if player.is_dead:
		highscore.when = Time.get_date_string_from_system()
		highscore.how = savegame.died_from
	else:
		highscore.living = true
		highscore.when = "Today"
		highscore.how = "nobody (yet!)"
	highscore.who = "%s%s the level %d %s %s" % [player.name.strip_edges(), Util.to_roman(player.generation, true), player.level, player.ancestry.title, player.profession.get_profession_sub_name(player.realm_1)]
	if savegame.current_depth > 0:
		highscore.where = "on level %d of %s" % [savegame.current_depth, savegame.current_dungeon.name]
	else:
		highscore.where = "in the wilderness"
		for town: Town in savegame.towns:
			if player.wilderness_col == town.wild_col and player.wilderness_row == town.wild_row:
				highscore.where = "in %s" % [town.name]
	return highscore
