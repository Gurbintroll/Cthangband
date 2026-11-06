# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DeathSpellAnnihilation
extends Spell


func _init() -> void:
	default_base_failure = 95
	default_level = 44
	default_power_cost = 100
	first_cast_experience = 250
	name = "Annihilation"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	player.power -= 100
	for i: int in dungeon_level.monster_count_max:
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster_type == null:
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
			continue
		dungeon_level.dungeon_monsters.delete_monster_by_index(i, true)
		player.take_hit(Rng.roll_die(4), "the strain of casting Annihilation")
		player.power += 1
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
		savegame.notice_update_redraw()
		User.wait_for_frame()
		player.power += 100


func comment(player: Player) -> String:
	return ""
