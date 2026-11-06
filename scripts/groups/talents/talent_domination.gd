# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentDomination
extends Talent


func initialise(profession: Profession) -> void:
	name = "Domination"
	level = 9
	power_cost = 7
	base_failure = 50


func use(player: Player, dungeon_level: DungeonLevel, savegame: SaveGame) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	if player.level < 30:
		var direction: int = target_engine.get_direction_with_aim()
		if direction == -1:
			return
		savegame.spell_effects_handler.fire_ball(ProjectileDomination.new(savegame.spell_effects_handler), direction, player.level, 0)
	else:
		savegame.spell_effects_handler.charm_monsters(player.level * 2)


func comment(player: Player) -> String:
	return ""
