# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SorcerySpellDimensionDoor
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 12
	default_power_cost = 12
	first_cast_experience = 40
	name = "Dimension Door"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	Profile.instance.message_print("You open a dimensional gate. Choose a destination.")
	var location: Location = Location.new()
	if not target_engine.target_point(location):
		return
	player.energy -= 60 - player.level
	if (not dungeon_level.grid_passable_no_creature(location.row, location.col) or
		dungeon_level.get_grid(location.row, location.col).tile_flags.is_set(GridTile.IN_VAULT) or
		dungeon_level.distance(location.row, location.col, player.map_row, player.map_col) > player.level + 2 or
		Rng.roll_die(player.level * player.level / 2) == 1 ):
		Profile.instance.message_print("You fail to exit the astral plane correctly!")
		player.energy -= 100
		savegame.spell_effects_handler.teleport_player(10)
	else:
		savegame.spell_effects_handler.teleport_player_to(location.row, location.col)


func comment(player: Player) -> String:
	return "range %d" % [player.level + 2]
