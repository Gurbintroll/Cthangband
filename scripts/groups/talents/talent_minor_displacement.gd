# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentMinorDisplacement
extends Talent


func initialise(profession: Profession) -> void:
	name = "Minor Displacement"
	level = 2
	power_cost = 2
	base_failure = 25


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	if player.level < 25:
		savegame.spell_effects_handler.teleport_player(10)
	else:
		var location: Location = Location.new()
		Profile.instance.message_print("Choose a destination.")
		if not target_engine.target_point(location):
			return
		player.energy -= 60 - player.level
		if (not dungeon_level.grid_passable_no_creature(location.row, location.col) or
			dungeon_level.get_grid(location.row, location.col).tile_flags.is_set(GridTile.IN_VAULT) or
			dungeon_level.get_grid(location.row, location.col).feature_type.name == "Water" or
			dungeon_level.distance(player.map_row, player.map_col, location.row, location.col) > player.level + 2 or
			Rng.roll_die(player.level * player.level / 2) == 0 ):
			Profile.instance.message_print("Something disrupts your concentration!")
			player.energy -= 100
			savegame.spell_effects_handler.teleport_player(20)
		else:
			savegame.spell_effects_handler.teleport_player_to(location.row, location.col)


func comment(player: Player) -> String:
	return "range %d" % [10 if player.level < 25 else player.level + 2]
