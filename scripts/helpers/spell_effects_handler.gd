# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SpellEffectsHandler
extends RefCounted

const HURT_CHANCE: int = 16

var savegame: SaveGame
var player: Player
var dungeon_level: DungeonLevel


func acid_dam(damage: int, killed_by: String) -> void:
	var inv: int = 1 if damage < 30 else 2 if damage < 60 else 3
	if player.has_acid_immunity or damage <= 0:
		return
	if player.has_elemental_vulnerability:
		damage *= 2
	if player.has_acid_resistance:
		damage = (damage + 2) / 3
	if player.timed_acid_resistance != 0:
		damage = (damage + 2) / 3
	if not (player.timed_acid_resistance != 0 or player.has_acid_resistance) and Rng.roll_die(HURT_CHANCE) == 1:
		player.try_decreasing_ability_score(Ability.CHARISMA)
	if minus_ac():
		damage = (damage + 1) / 2
	player.take_hit(damage, killed_by)
	if not (player.timed_acid_resistance != 0 and player.has_acid_resistance):
		player.inventory.inven_damage(set_acid_destroy, inv)


func activate_hi_summons() -> void:
	for i: int in Rng.roll_die(9) + (savegame.total_difficulty() / 40):
		match Rng.roll_die(26) + (savegame.total_difficulty() / 20):
			1, 2:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_ANT)
			3, 4:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_SPIDER)
			5, 6:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_HOUND)
			7, 8:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_HYDRA)
			9, 10:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_CTHULOID)
			11, 12:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_UNDEAD)
			13, 14:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DRAGON)
			15, 16:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DEMON)
			17:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_GOO)
			18, 19:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_UNIQUE)
			20, 21:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_HI_UNDEAD)
			22, 23:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_HI_DRAGON)
			24, 25:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, 100, SummonType.SUMMON_REAVER)
			_:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, (savegame.total_difficulty() * 3 / 2) + 5, 0)


func aggravate_monsters(who: int) -> void:
	var sleep: bool = false
	var speed: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		if i == who:
			continue
		if monster.distance_from_player < dungeon_level.MAX_SIGHT_RADIUS * 2:
			if monster.sleep_level != 0:
				monster.sleep_level = 0
				sleep = true
		if dungeon_level.player_has_los_bold(monster.map_row, monster.map_col):
			if monster.speed < monster_type.speed + 10:
				monster.speed = monster_type.speed + 10
				speed = true
			if monster.mind.is_set(MonsterMind.FRIENDLY):
				if Rng.roll_die(2) == 1:
					monster.mind.clear_bits(MonsterMind.FRIENDLY)
	if speed:
		Profile.instance.message_print("You feel a sudden stirring nearby!")
	elif sleep:
		Profile.instance.message_print("You hear a sudden stirring in the distance!")


func alchemy() -> void:
	var amt: int = 1
	var force: bool = User.command_argument > 0
	var item_index: int = savegame.get_item("Turn which item to gold? ", false, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to turn to gold.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	if item.count > 1:
		amt = User.get_quantity("", item.count, true)
		if amt <= 0:
			return
	var old_number: int = item.count
	item.count = amt
	var item_name: String = item.description(true, 3)
	item.count = old_number
	if not force:
		if item.value() >= 1:
			var text: String = "Really turn %s to gold? " % [item_name]
			if not User.get_check(text):
				return
	if item.is_artifact() or item.is_legendary():
		var feel: String = "special"
		Profile.instance.message_print("You fail to turn %s to gold!" % [item_name])
		if item.is_cursed() or item.is_broken():
			feel = "terrible"
		item.inscription = feel
		item.identify_flags.set_bits(IdentifyFlags.SENSED)
		player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE)
		return
	var price: int = item.real_value()
	if price <= 0:
		Profile.instance.message_print("You turn %s to worthless lead." % [item_name])
	else:
		price /= 3
		if amt > 1:
			price *= amt
		if price > 30000:
			price = 30000
		Profile.instance.message_print("You turn %s to %d coins worth of gold." % [item_name, price])
		player.gold += price
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_GOLD)
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -amt)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -amt)
		dungeon_level.floor_item_describe(0 - item_index)
		dungeon_level.floor_item_optimize(0 - item_index)


func alter_reality() -> void:
	Profile.instance.message_print("The world changes!")
	savegame.is_auto_save = true
	savegame.command_save_game()
	savegame.is_auto_save = false
	savegame.new_level_flag = true
	savegame.came_from = LevelStart.START_RANDOM


func apply_disenchant() -> bool:
	var t: int = 0
	match Rng.roll_die(8):
		1:
			t = InventorySlot.MELEE_WEAPON
		2:
			t = InventorySlot.RANGED_WEAPON
		3:
			t = InventorySlot.BODY
		4:
			t = InventorySlot.CLOAK
		5:
			t = InventorySlot.ARM
		6:
			t = InventorySlot.HEAD
		7:
			t = InventorySlot.HANDS
		8:
			t = InventorySlot.FEET
	var item: Item = player.inventory.items[t]
	if item.item_type == null:
		return false
	if item.bonus_to_hit <= 0 and item.bonus_damage <= 0 and item.bonus_armour_class <= 0:
		return false
	var item_name: String = item.description(false, 0)
	var s: String
	if (item.is_artifact() or item.is_legendary()) and Rng.roll_die(100) <= 71:
		s = "" if item.count != 1 else "s"
		Profile.instance.message_print("Your %s (%s) resist%s disenchantment!" % [item_name, Util.index_to_label(t), s])
		return true
	if item.bonus_to_hit > 0:
		item.bonus_to_hit -= 1
	if item.bonus_to_hit > 5 and Rng.roll_die(100) <= 20:
		item.bonus_to_hit -= 1
	if item.bonus_damage > 0:
		item.bonus_damage -= 1
	if item.bonus_damage > 5 and Rng.roll_die(100) <= 20:
		item.bonus_damage -= 1
	if item.bonus_armour_class > 0:
		item.bonus_armour_class -= 1
	if item.bonus_armour_class > 5 and Rng.roll_die(100) <= 20:
		item.bonus_armour_class -= 1
	s = "were" if item.count != 1 else "was"
	Profile.instance.message_print("Your %s (%s) %s disenchanted!" % [item_name, Util.index_to_label(t), s])
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	return true


func apply_nexus(monster: Monster) -> void:
	match Rng.roll_die(7):
		1, 2, 3:
			teleport_player(200)
		4, 5:
			teleport_player_to(monster.map_row, monster.map_col)
		6:
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				teleport_player_level()
		7:
			if Rng.roll_die(100) <= player.skill_saving_throw:
				Profile.instance.message_print("You resist the effects!")
			else:
				Profile.instance.message_print("Your body starts to scramble...")
				player.shuffle_ability_scores()


func artifact_scroll() -> void:
	var okay: bool
	savegame.item_filter_function = item_filter_weapon
	var item_index: int = savegame.get_item("Enchant which item? ", true, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to enchant.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var item_name: String = item.description(false, 0)
	var your: String = "Your" if item_index >= 0 else "The"
	var s: String = "" if item.count > 1 else "s"
	Profile.instance.message_print("%s %s radiate%s a blinding light!" % [your, item_name, s])
	if item.artifact_index != 0 or item.is_legendary():
		var are: String = "are" if item.count > 1 else "is"
		if item.artifact_index != 0:
			s = "artifacts" if item.count > 1 else "an artifact"
		else:
			s = "legendary"
		Profile.instance.message_print("The %s %s already %s!" % [item_name, are, s])
		okay = false
	elif item.rare_index != 0:
		var are: String = "are" if item.count > 1 else "is"
		s = "rare items" if item.count > 1 else "a rare item"
		Profile.instance.message_print("The %s %s already %s!" % [item_name, are, s])
		okay = false
	else:
		if item.count > 1:
			Profile.instance.message_print("Not enough enough energy to enchant more than one object!")
			s = "were" if item.count > 2 else "was"
			Profile.instance.message_print("%d of your item_name %s destroyed!" % [item.count - 1, s])
			item.count = 1
		okay = item.create_legendary(true)
	if not okay:
		Profile.instance.message_print("The enchantment failed.")


func banish_evil(dist: int) -> bool:
	return project_at_all_in_los(ProjectileAwayEvil.new(self), dist)


func banish_monsters(dist: int) -> void:
	project_at_all_in_los(ProjectileAwayAll.new(self), dist)


func bless_weapon() -> void:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	savegame.item_filter_function = item_filter_weapon
	var item_index: int = savegame.get_item("Bless which weapon? ", true, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have no weapon to bless.")
		return
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var item_name: String = item.description(false, 0)
	item.get_merged_flags(f_1, f_2, f_3)
	if item.identify_flags.is_set(IdentifyFlags.CURSED):
		var your: String
		if (f_3.is_set(ItemFlags3.HEAVY_CURSE) and Rng.roll_die(100) < 33) or f_3.is_set(ItemFlags3.PERMA_CURSE):
			your = "your" if item_index >= 0 else "the"
			Profile.instance.message_print("The black aura on %s %s disrupts the blessing!" % [your, item_name])
			return
		your = "your" if item_index >= 0 else "the"
		Profile.instance.message_print("A malignant aura leaves %s %s." % [your, item_name])
		item.identify_flags.clear_bits(IdentifyFlags.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.SENSED)
		item.inscription = "uncursed"
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	if f_3.is_set(ItemFlags3.BLESSED):
		var your: String = "your" if item_index >= 0 else "the"
		var s: String = "were" if item.count > 1 else "was"
		Profile.instance.message_print("%s %s %s blessed already." % [your, item_name, s])
		return
	if not (item.is_legendary() or item.artifact_index != 0) or Rng.roll_die(3) == 1:
		var your: String = "your" if item_index >= 0 else "the"
		var s: String = "" if item.count > 1 else "s"
		Profile.instance.message_print("%s %s shine%s!" % [your, item_name, s])
		item.legendary_flags_3.set_bits(ItemFlags3.BLESSED)
	else:
		var dis_happened: bool = false
		var rarity: String = "legendary item" if item.is_legendary() else "artifact"
		Profile.instance.message_print("The %s resists your blessing!" % [rarity])
		if item.bonus_to_hit > 0:
			item.bonus_to_hit -= 1
			dis_happened = true
		if item.bonus_to_hit > 5 and Rng.roll_die(100) <= 33:
			item.bonus_to_hit -= 1
		if item.bonus_damage > 0:
			item.bonus_damage -= 1
			dis_happened = true
		if item.bonus_damage > 5 and Rng.roll_die(100) <= 33:
			item.bonus_damage -= 1
		if item.bonus_armour_class > 0:
			item.bonus_armour_class -= 1
			dis_happened = true
		if item.bonus_armour_class > 5 and Rng.roll_die(100) <= 33:
			item.bonus_armour_class -= 1
		if dis_happened:
			Profile.instance.message_print("There is a static feeling in the air...")
			var your: String = "your" if item_index >= 0 else "the"
			var s: String = "were" if item.count > 1 else "was"
			Profile.instance.message_print("%s %s %s disenchanted!" % [your, item_name, s])
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)


func call_chaos() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var player_level: int = player.level
	var line_chaos: bool = false
	var hurt_types: Array[Projectile] = [
		ProjectileElectricity.new(self),
		ProjectilePoison.new(self),
		ProjectileAcid.new(self),
		ProjectileCold.new(self),
		ProjectileFire.new(self),
		ProjectileMissile.new(self),
		ProjectileArrow.new(self),
		ProjectilePlasma.new(self),
		ProjectileHolyFire.new(self),
		ProjectileWater.new(self),
		ProjectileLight.new(self),
		ProjectileDark.new(self),
		ProjectileForce.new(self),
		ProjectileInertia.new(self),
		ProjectileMeteor.new(self),
		ProjectilePower.new(self),
		ProjectileIce.new(self),
		ProjectileChaos.new(self),
		ProjectileNether.new(self),
		ProjectileDisenchantment.new(self),
		ProjectileExplode.new(self),
		ProjectileSound.new(self),
		ProjectileNexus.new(self),
		ProjectileConfusion.new(self),
		ProjectileTime.new(self),
		ProjectileGravity.new(self),
		ProjectileShard.new(self),
		ProjectileNuke.new(self),
		ProjectileHellFire.new(self),
		ProjectileDisintegrate.new(self),
	]
	var chaos_type: Projectile = hurt_types[Rng.roll_die(30) - 1]
	if Rng.roll_die(4) == 1:
		line_chaos = true
	if Rng.roll_die(6) == 1:
		for dummy: int in range(1, 10):
			if dummy - 5 != 0:
				if line_chaos:
					fire_beam(chaos_type, dummy, 75)
				else:
					fire_ball(chaos_type, dummy, 75, 2)
	elif Rng.roll_die(3) == 1:
		fire_ball(chaos_type, 0, 300, 8)
	else:
		var dir: int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		if line_chaos:
			fire_beam(chaos_type, dir, 150)
		else:
			fire_ball(chaos_type, dir, 150, 3 + (player_level / 35))


func carnage(player_cast: bool) -> void:
	var needs_screen_restore: bool = false
	var done: bool = false
	var typ: String = ""
	var symbols: SymbolDisplay = SymbolDisplay.new()
	while not done:
		typ = User.get_simple_choice("Choose a monster race (by symbol) to carnage (* for list): ")
		typ = User.strip_movement_keys(typ)
		if typ == ".":
			typ = "·"
		if typ == "*":
			if needs_screen_restore:
				Console.restore_screen()
				Console.set_background(BackgroundImage.IN_GAME)
				needs_screen_restore = false
			else:
				Console.save_screen()
				Console.set_background(BackgroundImage.DEFAULT)
				symbols.display(true)
				needs_screen_restore = true
			continue
		if typ == "":
			if needs_screen_restore:
				Console.restore_screen()
				Console.set_background(BackgroundImage.IN_GAME)
			return
		break
	if needs_screen_restore:
		Console.restore_screen()
		Console.set_background(BackgroundImage.IN_GAME)
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster_type == null:
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			continue
		if monster_type.character != typ:
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
			continue
		dungeon_level.dungeon_monsters.delete_monster_by_index(i, true)
		if player_cast:
			player.take_hit(Rng.roll_die(4), "the strain of casting carnage")
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
		savegame.notice_update_redraw()
		User.wait_for_frame()


func charm_animal(dir: int, player_level: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(ProjectileControlAnimal.new(self), dir, player_level, flag)


func charm_animals(damage: int) -> void:
	project_at_all_in_los(ProjectileControlAnimal.new(self), damage)


func charm_monster(dir: int, player_level: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileCharm.new(self), dir, player_level, flag)


func charm_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectileCharm.new(self), damage)


func clone_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldClone.new(self), dir, 0, flag)


func cold_dam(damage: int, killed_by: String) -> void:
	var inv: int = 1 if damage < 30 else 2 if damage < 60 else 3
	if player.has_cold_immunity or damage <= 0:
		return
	if player.has_elemental_vulnerability:
		damage *= 2
	if player.has_cold_resistance:
		damage = (damage + 2) / 3
	if player.timed_cold_resistance != 0:
		damage = (damage + 2) / 3
	if not (player.timed_cold_resistance != 0 or player.has_cold_resistance) and Rng.roll_die(HURT_CHANCE) == 1:
		player.try_decreasing_ability_score(Ability.STRENGTH)
	player.take_hit(damage, killed_by)
	if not (player.has_cold_resistance and player.timed_cold_resistance != 0):
		player.inventory.inven_damage(set_cold_destroy, inv)


func confuse_monster(dir: int, player_level: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldConfusion.new(self), dir, player_level, flag)


func confuse_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectileOldConfusion.new(self), damage)


func control_one_undead(dir: int, player_level: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(ProjectileControlUndead.new(self), dir, player_level, flag)


func death_ray(dir: int, player_level: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(ProjectileDeathRay.new(self), dir, player_level, flag)


func destroy_area(row_1: int, col_1: int, r: int) -> void:
	var flag: bool = false
	for row: int in range(row_1 - r, row_1 + r + 1):
		for col: int in range(col_1 - r, col_1 + r + 1):
			if not dungeon_level.in_inner_bounds(row, col):
				continue
			var k: int = dungeon_level.distance(row_1, col_1, row, col)
			if k > r:
				continue
			var tile: GridTile = dungeon_level.get_grid(row, col)
			tile.tile_flags.clear_bits(GridTile.IN_ROOM | GridTile.IN_VAULT)
			tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED | GridTile.SELF_LIT)
			if col == player.map_col and row == player.map_row:
				flag = true
				continue
			if row == row_1 and col == col_1:
				continue
			dungeon_level.delete_monster_at(row, col)
			if dungeon_level.cave_valid_bold(row, col):
				dungeon_level.delete_item_at(row, col)
				var t: int = Rng.roll_die(200)
				if t <= 20:
					tile.set_feature("WallBasic")
				elif t <= 70:
					tile.set_feature("Quartz")
				elif t < 100:
					tile.set_feature("Magma")
				else:
					tile.revert_to_background()
	if flag:
		Profile.instance.message_print("There is a searing blast of light!")
		if not player.has_blindness_resistance and not player.has_light_resistance:
			player.set_timed_blindness(player.timed_blindness + 10 + Rng.roll_die(10))
	player.updates_needed.set_bits(UpdateFlags.UPDATE_REMOVE_VIEW | UpdateFlags.UPDATE_REMOVE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func destroy_door(dir: int) -> bool:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.GRID | ProjectionFlag.ITEM
	return targeted_project(ProjectileKillDoor.new(self), dir, 0, flag)


func destroy_doors_touch() -> bool:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.HIDE
	return project(0, 1, player.map_row, player.map_col, 0, ProjectileKillDoor.new(self), flag)


func detect_all() -> bool:
	var detect: bool = detect_traps()
	detect = detect or (detect_doors())
	detect = detect or (detect_stairs())
	detect = detect or (detect_treasure())
	detect = detect or (detect_items_gold())
	detect = detect or (detect_items_normal())
	detect = detect or (detect_monsters_invis())
	detect = detect or (detect_monsters_normal())
	return detect


func detect_doors() -> bool:
	var detect: bool = false
	for row: int in range(dungeon_level.panel_row_min, dungeon_level.panel_row_max + 1):
		for col: int in range(dungeon_level.panel_col_min, dungeon_level.panel_col_max + 1):
			var tile: GridTile = dungeon_level.get_grid(row, col)
			if tile.feature_type.category == FloorTileTypeCategory.SECRET_DOOR:
				dungeon_level.replace_secret_door(row, col)
			if tile.feature_type.is_closed_door or tile.feature_type.category == FloorTileTypeCategory.OPEN_DOORWAY:
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(row, col)
				detect = true
	if detect:
		Profile.instance.message_print("You sense the presence of doors!")
	return detect


func detect_monsters_evil() -> bool:
	var flag: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if monster_type.flags_3.is_set(MonsterFlags3.EVIL):
			monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.EVIL)
			dungeon_level.dungeon_monsters.repair_monsters = true
			monster.individual_monster_flags.set_bits(IndividualMonsterFlags.MARK | IndividualMonsterFlags.SHOW)
			monster.is_visible = true
			dungeon_level.redraw_single_location(row, col)
			flag = true
	if flag:
		Profile.instance.message_print("You sense the presence of evil creatures!")
	return flag


func detect_monsters_invis() -> bool:
	var flag: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if monster_type.flags_2.is_set(MonsterFlags2.INVISIBLE):
			monster_type.knowledge.r_flags_2.set_bits(MonsterFlags2.INVISIBLE)
			dungeon_level.dungeon_monsters.repair_monsters = true
			monster.individual_monster_flags.set_bits(IndividualMonsterFlags.MARK | IndividualMonsterFlags.SHOW)
			monster.is_visible = true
			dungeon_level.redraw_single_location(row, col)
			flag = true
	if flag:
		Profile.instance.message_print("You sense the presence of invisible creatures!")
	return flag


func detect_monsters_non_living() -> void:
	var flag: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_3.is_set(MonsterFlags3.DEMON):
			dungeon_level.dungeon_monsters.repair_monsters = true
			monster.individual_monster_flags.set_bits(IndividualMonsterFlags.MARK | IndividualMonsterFlags.SHOW)
			monster.is_visible = true
			dungeon_level.redraw_single_location(row, col)
			flag = true
	if flag:
		Profile.instance.message_print("You sense the presence of unnatural beings!")


func detect_monsters_normal() -> bool:
	var flag: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if monster_type.flags_2.is_clear(MonsterFlags2.INVISIBLE) or player.has_see_invisibility or player.timed_see_invisibility != 0:
			dungeon_level.dungeon_monsters.repair_monsters = true
			monster.individual_monster_flags.set_bits(IndividualMonsterFlags.MARK | IndividualMonsterFlags.SHOW)
			monster.is_visible = true
			dungeon_level.redraw_single_location(row, col)
			flag = true
	if flag:
		Profile.instance.message_print("You sense the presence of monsters!")
	return flag


func detect_items_gold() -> bool:
	var detect: bool = false
	for i: int in range(1, dungeon_level.item_count_max):
		var item: Item = dungeon_level.dungeon_items.items[i]
		if item.item_type == null:
			continue
		if item.holding_monster_index != 0:
			continue
		var row: int = item.map_row
		var col: int = item.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if item.category == ItemCategory.GOLD:
			item.marked = true
			dungeon_level.redraw_single_location(row, col)
			detect = true
	if (detect):
		Profile.instance.message_print("You sense the presence of treasure!")
	if detect_monsters_string("$*"):
		detect = true
	return detect


func detect_objects_magic() -> void:
	var detect: bool = false
	for i: int in range(1, dungeon_level.item_count_max):
		var item: Item = dungeon_level.dungeon_items.items[i]
		if item.item_type == null:
			continue
		if item.holding_monster_index != 0:
			continue
		var row: int = item.map_row
		var col: int = item.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		var item_category: int = item.category
		if (item.is_artifact() or item.is_rare() or item.is_legendary() or
			item_category == ItemCategory.AMULET or item_category == ItemCategory.RING or item_category == ItemCategory.STAFF or
			item_category == ItemCategory.WAND or item_category == ItemCategory.ROD or item_category == ItemCategory.SCROLL or
			item_category == ItemCategory.POTION or item_category == ItemCategory.LIFE_BOOK or item_category == ItemCategory.SORCERY_BOOK or
			item_category == ItemCategory.NATURE_BOOK or item_category == ItemCategory.CHAOS_BOOK or item_category == ItemCategory.DEATH_BOOK or
			item_category == ItemCategory.CORPOREAL_BOOK or item_category == ItemCategory.TAROT_BOOK or item_category == ItemCategory.FOLK_BOOK or
			item.bonus_armour_class > 0 or item.bonus_to_hit + item.bonus_damage > 0 ):
			item.marked = true
			dungeon_level.redraw_single_location(row, col)
			detect = true
	if detect:
		Profile.instance.message_print("You sense the presence of magic objects!")


func detect_items_normal() -> bool:
	var detect: bool = false
	for i: int in range(1, dungeon_level.item_count_max):
		var item: Item = dungeon_level.dungeon_items.items[i]
		if item.item_type == null:
			continue
		if item.holding_monster_index != 0:
			continue
		var row: int = item.map_row
		var col: int = item.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if item.category != ItemCategory.GOLD:
			item.marked = true
			dungeon_level.redraw_single_location(row, col)
			detect = true
	if detect:
		Profile.instance.message_print("You sense the presence of objects!")
	if detect_monsters_string("!=?|"):
		detect = true
	return detect


func detect_stairs() -> bool:
	var detect: bool = false
	for row: int in range(dungeon_level.panel_row_min, dungeon_level.panel_row_max + 1):
		for col: int in range(dungeon_level.panel_col_min, dungeon_level.panel_col_max + 1):
			var tile: GridTile = dungeon_level.get_grid(row, col)
			if tile.feature_type.category == FloorTileTypeCategory.UP_STAIR or tile.feature_type.category == FloorTileTypeCategory.DOWN_STAIR:
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(row, col)
				detect = true
	if detect:
		Profile.instance.message_print("You sense the presence of stairs!")
	return detect


func detect_traps() -> bool:
	var detect: bool = false
	for row: int in range(dungeon_level.panel_row_min, dungeon_level.panel_row_max + 1):
		for col: int in range(dungeon_level.panel_col_min, dungeon_level.panel_col_max + 1):
			var tile: GridTile = dungeon_level.get_grid(row, col)
			tile.tile_flags.set_bits(GridTile.TRAPS_DETECTED)
			dungeon_level.redraw_single_location(row, col)
			if tile.feature_type.category == FloorTileTypeCategory.UNIDENTIFIED_TRAP:
				dungeon_level.pick_trap(row, col)
			if tile.feature_type.is_trap:
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				detect = true
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_DTRAP)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	if detect:
		Profile.instance.message_print("You sense the presence of traps!")
	return detect


func detect_treasure() -> bool:
	var detect: bool = false
	for row: int in range(dungeon_level.panel_row_min, dungeon_level.panel_row_max + 1):
		for col: int in range(dungeon_level.panel_col_min, dungeon_level.panel_col_max + 1):
			var tile: GridTile = dungeon_level.get_grid(row, col)
			if tile.feature_type.name.contains("HidTreas"):
				tile.set_feature(tile.feature_type.name.replace("Hid", "Power"))
			if tile.feature_type.name.contains("VisTreas"):
				tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				dungeon_level.redraw_single_location(row, col)
				detect = true
	if detect:
		Profile.instance.message_print("You sense the presence of buried treasure!")
	return detect


func disarm_trap(dir: int) -> bool:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.GRID | ProjectionFlag.ITEM
	return targeted_project(ProjectileKillTrap.new(self), dir, 0, flag)


func dispel_demons(damage: int) -> void:
	project_at_all_in_los(ProjectileDispDemon.new(self), damage)


func dispel_evil(damage: int) -> bool:
	return project_at_all_in_los(ProjectileDispEvil.new(self), damage)


func dispel_good(damage: int) -> void:
	project_at_all_in_los(ProjectileDispGood.new(self), damage)


func dispel_living(damage: int) -> void:
	project_at_all_in_los(ProjectileDispLiving.new(self), damage)


func dispel_monsters(damage: int) -> bool:
	return project_at_all_in_los(ProjectileDispAll.new(self), damage)


func dispel_undead(damage: int) -> bool:
	return project_at_all_in_los(ProjectileDispUndead.new(self), damage)


func door_creation() -> void:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.HIDE
	project(0, 1, player.map_row, player.map_col, 0, ProjectileMakeDoor.new(self), flag)


func drain_life(dir: int, damage: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldDrain.new(self), dir, damage, flag)


func earthquake(row_centre: int, col_centre: int, r: int) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var row: int
	var col: int
	var row_1: int
	var col_1: int
	var damage: int = 0
	var sn: int = 0
	var row_s: int = 0
	var col_s: int = 0
	var hurt: bool = false
	var tile: GridTile
	var map: Array
	for j: int in 32:
		var map_row: Array[bool] = []
		for k: int in 32:
			map_row.append(false)
		map.append(map_row)
	if r > 12:
		r = 12
	for row_d: int in range(-r, r + 1):
		for col_d: int in range(-r, r + 1):
			row_1 = row_centre + row_d
			col_1 = col_centre + col_d
			if not dungeon_level.in_inner_bounds(row_1, col_1):
				continue
			if dungeon_level.distance(row_centre, col_centre, row_1, col_1) > r:
				continue
			tile = dungeon_level.get_grid(row_1, col_1)
			tile.tile_flags.clear_bits(GridTile.IN_ROOM | GridTile.IN_VAULT)
			tile.tile_flags.clear_bits(GridTile.SELF_LIT | GridTile.PLAYER_MEMORISED)
			if col_d == 0 and row_d == 0:
				continue
			if Rng.roll_die(100) <= 85:
				continue
			map[16 + row_1 - row_centre][16 + col_1 - col_centre] = true
			if row_1 == player.map_row and col_1 == player.map_col:
				hurt = true
	if hurt:
		for i: int in 8:
			row = player.map_row + dungeon_level.keypad_direction_row_offset[i]
			col = player.map_col + dungeon_level.keypad_direction_col_offset[i]
			if not dungeon_level.grid_passable_no_creature(row, col):
				continue
			if map[16 + row - row_centre][16 + col - col_centre]:
				continue
			sn += 1
			if Rng.roll_die(sn) > 1:
				continue
			row_s = row
			col_s = col
		var dt: String = "tower" if savegame.current_dungeon.is_tower else "dungeon"
		match Rng.roll_die(3):
			1:
				Profile.instance.message_print("The %s ceiling collapses!" % [dt])
			2:
				Profile.instance.message_print("The %s floor twists in an unnatural way!" % [dt])
			_:
				Profile.instance.message_print("The %s quakes!  You are pummeled with debris!" % [dt])
		if sn == 0:
			Profile.instance.message_print("You are severely crushed!")
			damage = 300
		else:
			match Rng.roll_die(3):
				1:
					Profile.instance.message_print("You nimbly dodge the blast!")
					damage = 0
				2:
					Profile.instance.message_print("You are bashed by rubble!")
					damage = Rng.roll_dice(10, 4)
					player.set_timed_stun(player.timed_stun + Rng.roll_die(50))
				3:
					Profile.instance.message_print("You are crushed between the floor and ceiling!")
					damage = Rng.roll_dice(10, 4)
					player.set_timed_stun(player.timed_stun + Rng.roll_die(50))
			var row_orig: int = player.map_row
			var col_orig: int = player.map_col
			player.map_row = row_s
			player.map_col = col_s
			dungeon_level.redraw_single_location(row_orig, col_orig)
			dungeon_level.redraw_single_location(player.map_row, player.map_col)
			target_engine.recenter_screen_around_player()
		map[16 + player.map_row - row_centre][16 + player.map_col - col_centre] = false
		if damage != 0:
			player.take_hit(damage, "an earthquake")
	for row_d: int in range(-r, r + 1):
		for col_d: int in range(-r, r + 1):
			row_1 = row_centre + row_d
			col_1 = col_centre + col_d
			if not map[16 + row_1 - row_centre][16 + col_1 - col_centre]:
				continue
			tile = dungeon_level.get_grid(row_1, col_1)
			if tile.monster_index != 0:
				var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
				var monster_type: MonsterType = monster.monster_type
				if monster_type.flags_2.is_clear(MonsterFlags2.KILL_WALL) and monster_type.flags_2.is_clear(MonsterFlags2.PASSWALL):
					sn = 0
					if monster_type.flags_1.is_clear(MonsterFlags1.NEVER_MOVE):
						for i: int in 8:
							row = row_1 + dungeon_level.keypad_direction_row_offset[i]
							col = col_1 + dungeon_level.keypad_direction_col_offset[i]
							if not dungeon_level.grid_passable_no_creature(row, col):
								continue
							if dungeon_level.get_grid(row, col).feature_type.name == "ElderSign":
								continue
							if dungeon_level.get_grid(row, col).feature_type.name == "YellowSign":
								continue
							if map[16 + row - row_centre][16 + col - col_centre]:
								continue
							sn += 1
							if Rng.roll_die(sn) > 1:
								continue
							row_s = row
							col_s = col
					var monster_name: String = monster.describe(0)
					Profile.instance.message_print("%s wails out in pain!" % [monster_name])
					damage = Rng.roll_dice(4, 8) if sn != 0 else 200
					monster.sleep_level = 0
					monster.health -= damage
					if monster.health < 0:
						Profile.instance.message_print("%s is embedded in the rock!" % [monster_name])
						dungeon_level.delete_monster_at(row_1, col_1)
						sn = 0
					if sn != 0:
						var monster_index: int = dungeon_level.get_grid(row_1, col_1).monster_index
						dungeon_level.get_grid(row_s, col_s).monster_index = monster_index
						dungeon_level.get_grid(row_1, col_1).monster_index = 0
						monster.map_row = row_s
						monster.map_col = col_s
						dungeon_level.dungeon_monsters.update_monster_visibility(monster_index, true)
						dungeon_level.redraw_single_location(row_1, col_1)
						dungeon_level.redraw_single_location(row_s, col_s)
	for row_d: int in range(-r, r + 1):
		for col_d: int in range(-r, r + 1):
			row_1 = row_centre + row_d
			col_1 = col_centre + col_d
			if not map[16 + row_1 - row_centre][16 + col_1 - col_centre]:
				continue
			tile = dungeon_level.get_grid(row_1, col_1)
			if row_1 == player.map_row and col_1 == player.map_col:
				continue
			if dungeon_level.cave_valid_bold(row_1, col_1):
				var floor_passable: bool = dungeon_level.grid_passable(row_1, col_1)
				dungeon_level.delete_item_at(row_1, col_1)
				var t: int = Rng.roll_die(100) if floor_passable else 200
				if t <= 20:
					tile.set_feature("WallBasic")
				elif t <= 70:
					tile.set_feature("Quartz")
				elif t <= 100:
					tile.set_feature("Magma")
				else:
					tile.revert_to_background()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_REMOVE_VIEW | UpdateFlags.UPDATE_REMOVE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func elder_sign() -> void:
	if not dungeon_level.grid_open_no_item(player.map_row, player.map_col):
		Profile.instance.message_print("The object resists the spell.")
		return
	dungeon_level.cave_set_feat(player.map_row, player.map_col, "ElderSign")


func elder_sign_creation() -> void:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.ITEM
	project(0, 1, player.map_row, player.map_col, 0, ProjectileMakeElderSign.new(self), flag)


func elec_dam(damage: int, killed_by: String) -> void:
	var inv: int = 1 if damage < 30 else 2 if damage < 60 else 3
	if player.has_lightning_immunity or damage <= 0:
		return
	if player.has_elemental_vulnerability:
		damage *= 2
	if player.timed_lightning_resistance != 0:
		damage = (damage + 2) / 3
	if player.has_lightning_resistance:
		damage = (damage + 2) / 3
	if not (player.timed_lightning_resistance != 0 or player.has_lightning_resistance) and Rng.roll_die(HURT_CHANCE) == 1:
		player.try_decreasing_ability_score(Ability.DEXTERITY)
	player.take_hit(damage, killed_by)
	if not (player.timed_lightning_resistance != 0 and player.has_lightning_resistance):
		player.inventory.inven_damage(set_elec_destroy, inv)


func enchant(item: Item, n: int, enchant_flag: int) -> bool:
	var res: bool = false
	var a: bool = item.is_artifact() or item.is_legendary()
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	item.get_merged_flags(f_1, f_2, f_3)
	var prob: int = item.count * 100
	if item.category == ItemCategory.BOLT or item.category == ItemCategory.ARROW or item.category == ItemCategory.SHOT:
		prob /= 20
	for i: int in n:
		if Rng.roll_die(prob) > 100:
			continue
		var chance: int
		if enchant_flag & EnchantFlags.TO_HIT != 0:
			if item.bonus_to_hit < 0:
				chance = 0
			elif item.bonus_to_hit > 15:
				chance = 1000
			else:
				chance = Tables.ENCHANT_TABLE[item.bonus_to_hit]
			if Rng.roll_die(1000) > chance and (!a or Rng.roll_die(100) <= 50):
				item.bonus_to_hit += 1
				res = true
				if item.is_cursed() and f_3.is_clear(ItemFlags3.PERMA_CURSE) and item.bonus_to_hit >= 0 and Rng.roll_die(100) <= 25:
					Profile.instance.message_print("The curse is broken!")
					item.identify_flags.clear_bits(IdentifyFlags.CURSED)
					item.identify_flags.set_bits(IdentifyFlags.SENSED)
					if item.legendary_flags_3.is_set(ItemFlags3.CURSED):
						item.legendary_flags_3.clear_bits(ItemFlags3.CURSED)
					if item.legendary_flags_3.is_set(ItemFlags3.HEAVY_CURSE):
						item.legendary_flags_3.clear_bits(ItemFlags3.HEAVY_CURSE)
					item.inscription = "uncursed"
		if enchant_flag & EnchantFlags.TO_DAM != 0:
			if item.bonus_damage < 0:
				chance = 0
			elif item.bonus_damage > 15:
				chance = 1000
			else:
				chance = Tables.ENCHANT_TABLE[item.bonus_damage]
			if Rng.roll_die(1000) > chance and (!a or Rng.roll_die(100) <= 50):
				item.bonus_damage += 1
				res = true
				if item.is_cursed() and f_3.is_clear(ItemFlags3.PERMA_CURSE) and item.bonus_damage >= 0 and Rng.roll_die(100) <= 25:
					Profile.instance.message_print("The curse is broken!")
					item.identify_flags.clear_bits(IdentifyFlags.CURSED)
					item.identify_flags.set_bits(IdentifyFlags.SENSED)
					if item.legendary_flags_3.is_set(ItemFlags3.CURSED):
						item.legendary_flags_3.clear_bits(ItemFlags3.CURSED)
					if item.legendary_flags_3.is_set(ItemFlags3.HEAVY_CURSE):
						item.legendary_flags_3.clear_bits(ItemFlags3.HEAVY_CURSE)
					item.inscription = "uncursed"
		if enchant_flag & EnchantFlags.TO_AC != 0:
			if item.bonus_armour_class < 0:
				chance = 0
			elif item.bonus_armour_class > 15:
				chance = 1000
			else:
				chance = Tables.ENCHANT_TABLE[item.bonus_armour_class]
			if Rng.roll_die(1000) > chance and (!a or Rng.roll_die(100) <= 50):
				item.bonus_armour_class += 1
				res = true
				if item.is_cursed() and f_3.is_clear(ItemFlags3.PERMA_CURSE) and item.bonus_armour_class >= 0 and Rng.roll_die(100) <= 25:
					Profile.instance.message_print("The curse is broken!")
					item.identify_flags.clear_bits(IdentifyFlags.CURSED)
					item.identify_flags.set_bits(IdentifyFlags.SENSED)
					if item.legendary_flags_3.is_set(ItemFlags3.CURSED):
						item.legendary_flags_3.clear_bits(ItemFlags3.CURSED)
					if item.legendary_flags_3.is_set(ItemFlags3.HEAVY_CURSE):
						item.legendary_flags_3.clear_bits(ItemFlags3.HEAVY_CURSE)
					item.inscription = "uncursed"
	if not res:
		return false
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	return true


func enchant_spell(num_to_hit: int, num_to_dam: int, num_to_ac: int) -> bool:
	var okay: bool = false
	savegame.item_filter_function = item_filter_weapon
	if num_to_ac != 0:
		savegame.item_filter_function = item_filter_armour
	var item_index: int = savegame.get_item("Enchant which item? ", true, true, true)
	if item_index == 99:
		return false
	if item_index == -2:
		Profile.instance.message_print("You have nothing to enchant.")
		return false
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var item_name: String = item.description(false, 0)
	var your: String = "Your" if item_index >= 0 else "The"
	var s: String = "" if item.count > 1 else "s"
	Profile.instance.message_print("%s %s glow%s brightly!" % [your, item_name, s])
	if enchant(item, num_to_hit, EnchantFlags.TO_HIT):
		okay = true
	if enchant(item, num_to_dam, EnchantFlags.TO_DAM):
		okay = true
	if enchant(item, num_to_ac, EnchantFlags.TO_AC):
		okay = true
	if not okay:
		Profile.instance.message_print("The enchantment failed.")
	return true


func fear_monster(dir: int, player_level: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileTurnAll.new(self), dir, player_level, flag)


func fire_ball(projectile: Projectile, dir: int, damage: int, rad: int) -> bool:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	var col_target: int = player.map_col + (99 * dungeon_level.keypad_direction_col_offset[dir])
	var row_target: int = player.map_row + (99 * dungeon_level.keypad_direction_row_offset[dir])
	if dir == 5 and target_engine.target_okay():
		flag &= ~ProjectionFlag.STOP
		col_target = savegame.target_col
		row_target = savegame.target_row
	return project(0, rad, row_target, col_target, damage, projectile, flag)


func fire_beam(projectile: Projectile, dir: int, damage: int) -> void:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.KILL
	targeted_project(projectile, dir, damage, flag)


func fire_bolt(projectile: Projectile, dir: int, damage: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(projectile, dir, damage, flag)


func fire_bolt_or_beam(prob: int, projectile: Projectile, dir: int, damage: int) -> void:
	if Rng.roll_die(100) <= prob:
		fire_beam(projectile, dir, damage)
	else:
		fire_bolt(projectile, dir, damage)


func fire_dam(damage: int, killed_by: String) -> void:
	var inv: int = 1 if damage < 30 else 2 if damage < 60 else 3
	if player.has_fire_immunity or damage <= 0:
		return
	if player.has_elemental_vulnerability:
		damage *= 2
	if player.has_fire_resistance:
		damage = (damage + 2) / 3
	if player.timed_fire_resistance != 0:
		damage = (damage + 2) / 3
	if not (player.timed_fire_resistance != 0 or player.has_fire_resistance) and Rng.roll_die(HURT_CHANCE) == 1:
		player.try_decreasing_ability_score(Ability.STRENGTH)
	player.take_hit(damage, killed_by)
	if not (player.has_fire_resistance and player.timed_fire_resistance != 0):
		player.inventory.inven_damage(set_fire_destroy, inv)


func haste_monsters() -> bool:
	return project_at_all_in_los(ProjectileOldSpeed.new(self), player.level)


func heal_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldHeal.new(self), dir, Rng.roll_dice(4, 6), flag)


func identify_fully() -> bool:
	var item_index: int = savegame.get_item("Identify which item? ", true, true, true)
	if item_index == 99:
		return false
	if item_index == -2:
		Profile.instance.message_print("You have nothing to identify.")
		return false
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	item.become_flavour_aware()
	item.become_known()
	item.identify_flags.set_bits(IdentifyFlags.MENTAL)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	savegame.notice_update_redraw()
	var item_name: String = item.description(true, 3)
	if item_index >= InventorySlot.MELEE_WEAPON:
		Profile.instance.message_print("%s: %s (%s)." % [player.describe_wield_location(item_index), item_name, Util.index_to_label(item_index)])
		if item.stompable():
			item_name = item.description(true, 3)
			Profile.instance.message_print("You destroy %s." % [item_name])
			var amount: int = item.count
			player.inventory.inven_item_increase(item_index, -amount)
			player.inventory.inven_item_optimise(item_index)
	elif item_index >= 0:
		Profile.instance.message_print("In your pack: %s (%s)." % [item_name, Util.index_to_label(item_index)])
		if item.stompable():
			Profile.instance.message_print("You destroy %s." % [item_name])
			var amount: int = item.count
			player.inventory.inven_item_increase(item_index, -amount)
			player.inventory.inven_item_optimise(item_index)
	else:
		Profile.instance.message_print("On the ground: %s." % [item_name])
	item.identify_fully()
	return true


func identify_item() -> bool:
	var item_index: int = savegame.get_item("Identify which item? ", true, true, true)
	if item_index == 99:
		return false
	if item_index == -2:
		Profile.instance.message_print("You have nothing to identify.")
		return false
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	item.become_flavour_aware()
	item.become_known()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	var item_name: String = item.description(true, 3)
	if item_index >= InventorySlot.MELEE_WEAPON:
		Profile.instance.message_print("%s: %s (%s)." % [player.describe_wield_location(item_index), item_name, Util.index_to_label(item_index)])
		if item.stompable():
			Profile.instance.message_print("You destroy %s." % [item_name])
			var amount: int = item.count
			player.inventory.inven_item_increase(item_index, -amount)
			player.inventory.inven_item_optimise(item_index)
	elif item_index >= 0:
		Profile.instance.message_print("In your pack: %s (%s)." % [item_name, Util.index_to_label(item_index)])
		if item.stompable():
			Profile.instance.message_print("You destroy %s." % [item_name])
			var amount: int = item.count
			player.inventory.inven_item_increase(item_index, -amount)
			player.inventory.inven_item_optimise(item_index)
	else:
		Profile.instance.message_print("On the ground: %s." % [item_name])
	return true


func identify_pack() -> void:
	var i:int = 0
	while i < InventorySlot.TOTAL:
		var item: Item = player.inventory.items[i]
		if item.item_type == null:
			i += 1
			continue
		item.become_flavour_aware()
		item.become_known()
		if item.stompable():
			var item_name: String = item.description(true, 3)
			Profile.instance.message_print("You destroy %s." % [item_name])
			var amount: int = item.count
			player.inventory.inven_item_increase(i, -amount)
			player.inventory.inven_item_optimise(i)
		else:
			i += 1


func item_filter_armour(item: Item) -> bool:
	match item.category:
		ItemCategory.DRAGON_ARMOUR, \
		ItemCategory.HARD_ARMOUR, \
		ItemCategory.SOFT_ARMOUR, \
		ItemCategory.SHIELD, \
		ItemCategory.CLOAK, \
		ItemCategory.CROWN, \
		ItemCategory.HELM, \
		ItemCategory.BOOTS, \
		ItemCategory.GLOVES:
			return true
	return false


func item_filter_recharge(item: Item) -> bool:
	if item.category == ItemCategory.STAFF:
		return true
	if item.category == ItemCategory.WAND:
		return true
	if item.category == ItemCategory.ROD:
		return true
	return false


func light_area(damage: int, rad: int) -> bool:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.KILL
	if player.timed_blindness == 0:
		Profile.instance.message_print("You are surrounded by a white light.")
	project(0, rad, player.map_row, player.map_col, damage, ProjectileLightWeak.new(self), flag)
	light_room(player.map_row, player.map_col)
	return true


func light_line(dir: int) -> void:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.GRID | ProjectionFlag.KILL
	targeted_project(ProjectileLightWeak.new(self), dir, Rng.roll_dice(6, 8), flag)


func lose_all_info() -> bool:
	for i: int in InventorySlot.TOTAL:
		var item: Item = player.inventory.items[i]
		if item.item_type == null:
			continue
		if item.identify_flags.is_set(IdentifyFlags.MENTAL):
			continue
		if item.inscription != "" and item.identify_flags.is_set(IdentifyFlags.SENSED):
			var q: String = item.inscription
			if q == "cursed" or q == "broken" or q == "good" or q == "average" or q == "excellent" or q == "worthless" or q == "special" or q == "terrible":
				item.inscription = ""
		item.identify_flags.clear_bits(IdentifyFlags.EMPTY)
		item.identify_flags.clear_bits(IdentifyFlags.KNOWN)
		item.identify_flags.clear_bits(IdentifyFlags.SENSED)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	dungeon_level.wiz_dark()
	return true


func mass_carnage(player_cast: bool) -> void:
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			continue
		if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
			continue
		if monster.distance_from_player > dungeon_level.MAX_SIGHT_RADIUS:
			continue
		dungeon_level.dungeon_monsters.delete_monster_by_index(i, true)
		if player_cast:
			player.take_hit(Rng.roll_die(3), "the strain of casting Mass carnage")
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
		savegame.notice_update_redraw()
		User.wait_for_frame()


func mind_blast_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectilePsi.new(self), damage)


func poly_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldPoly.new(self), dir, player.level, flag)


func polymorph_monster(monster_type: MonsterType) -> int:
	var index: int = monster_type.index
	if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) or monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		return monster_type.index
	var lev1: int = monster_type.level - ((Rng.roll_die(20) / Rng.roll_die(9)) + 1)
	var lev2: int = monster_type.level + (Rng.roll_die(20) / Rng.roll_die(9)) + 1
	for i: int in 1000:
		var r: int = dungeon_level.dungeon_monsters.get_mon_num(((savegame.total_difficulty() + monster_type.level) / 2) + 5)
		if r == 0:
			break
		monster_type = Profile.instance.monster_types.types[r]
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			continue
		if monster_type.level < lev1 or monster_type.level > lev2:
			continue
		index = r
		break
	return index


func potion_smash_effect(who: int, row: int, col: int, item_subtype: int) -> bool:
	var radius: int = 2
	var dt: Projectile = null
	var damage: int = 0
	var angry: bool = false
	match item_subtype:
		PotionType.SALT_WATER, \
		PotionType.SLIME_MOLD, \
		PotionType.LOSE_MEMORIES, \
		PotionType.DEC_STR, \
		PotionType.DEC_INT, \
		PotionType.DEC_WIS, \
		PotionType.DEC_DEX, \
		PotionType.DEC_CON, \
		PotionType.DEC_CHA, \
		PotionType.WATER, \
		PotionType.APPLE_JUICE:
			return true
		PotionType.INFRAVISION, \
		PotionType.DETECT_INVIS, \
		PotionType.SLOW_POISON, \
		PotionType.CURE_POISON, \
		PotionType.BOLDNESS, \
		PotionType.RESIST_HEAT, \
		PotionType.RESIST_COLD, \
		PotionType.HEROISM, \
		PotionType.BESERK_STRENGTH, \
		PotionType.RESTORE_EXP, \
		PotionType.RES_STR, \
		PotionType.RES_INT, \
		PotionType.RES_WIS, \
		PotionType.RES_DEX, \
		PotionType.RES_CON, \
		PotionType.RES_CHA, \
		PotionType.INC_STR, \
		PotionType.INC_INT, \
		PotionType.INC_WIS, \
		PotionType.INC_DEX, \
		PotionType.INC_CON, \
		PotionType.INC_CHA, \
		PotionType.AUGMENTATION, \
		PotionType.ENLIGHTENMENT, \
		PotionType.STAR_ENLIGHTENMENT, \
		PotionType.SELF_KNOWLEDGE, \
		PotionType.EXPERIENCE, \
		PotionType.RESISTANCE, \
		PotionType.INVULNERABILITY, \
		PotionType.NEW_LIFE:
			return false
		PotionType.SLOWNESS:
			dt = ProjectileOldSlow.new(self)
			damage = 5
			angry = true
		PotionType.POISON:
			dt = ProjectilePoison.new(self)
			damage = 3
			angry = true
		PotionType.BLINDNESS:
			dt = ProjectileDark.new(self)
			angry = true
		PotionType.CONFUSION:
			dt = ProjectileOldConfusion.new(self)
			angry = true
		PotionType.SLEEP:
			dt = ProjectileOldSleep.new(self)
			angry = true
		PotionType.RUINATION, \
		PotionType.DETONATIONS:
			dt = ProjectileExplode.new(self)
			damage = Rng.roll_dice(25, 25)
			angry = true
		PotionType.DEATH:
			dt = ProjectileDeathRay.new(self)
			angry = true
			radius = 1
		PotionType.SPEED:
			dt = ProjectileOldSpeed.new(self)
		PotionType.CURE_LIGHT:
			dt = ProjectileOldHeal.new(self)
			damage = Rng.roll_dice(2, 3)
		PotionType.CURE_SERIOUS:
			dt = ProjectileOldHeal.new(self)
			damage = Rng.roll_dice(4, 3)
		PotionType.CURE_CRITICAL, \
		PotionType.CURING:
			dt = ProjectileOldHeal.new(self)
			damage = Rng.roll_dice(6, 3)
		PotionType.HEALING:
			dt = ProjectileOldHeal.new(self)
			damage = Rng.roll_dice(10, 10)
		PotionType.STAR_HEALING, \
		PotionType.LIFE:
			dt = ProjectileOldHeal.new(self)
			damage = Rng.roll_dice(50, 50)
			radius = 1
		PotionType.RESTORE_POWER:
			dt = ProjectilePower.new(self)
			damage = Rng.roll_dice(10, 10)
			radius = 1
	project(who, radius, row, col, damage, dt, ProjectionFlag.JUMP | ProjectionFlag.ITEM | ProjectionFlag.KILL)
	return angry


func probing() -> void:
	var probe: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		if monster.monster_type == null:
			continue
		if not dungeon_level.player_has_los_bold(monster.map_row, monster.map_col):
			continue
		if monster.is_visible:
			if not probe:
				Profile.instance.message_print("Probing...")
			var monster_name: String = monster.describe(0x04)
			Profile.instance.message_print("%s has %d hit points." % [monster_name, monster.health])
			dungeon_level.dungeon_monsters.lore_do_probe(i)
			probe = true
	if probe:
		Profile.instance.message_print("That's all.")


func project(who: int, rad: int, row: int, col: int, damage: int, projectile: Projectile, flag: int) -> bool:
	return projectile.fire(who, rad, row, col, damage, flag)


func recharge(num: int) -> bool:
	var i: int
	var t: int
	savegame.item_filter_function = item_filter_recharge
	var item_index: int = savegame.get_item("Recharge which item? ", false, true, true)
	if item_index == 99:
		return false
	if item_index == -2:
		Profile.instance.message_print("You have nothing to recharge.")
		return false
	var item: Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var lev: int = item.item_type.dungeon_level
	if item.category == ItemCategory.ROD:
		i = (100 - lev + num) / 5
		if i < 1:
			i = 1
		if Rng.roll_die(i) == 1:
			Profile.instance.message_print("The recharge backfires, draining the rod further!")
			if item.type_specific_value < 10000:
				item.type_specific_value = (item.type_specific_value + 100) * 2
		else:
			t = num * Rng.roll_dice(2, 4)
			if item.type_specific_value > t:
				item.type_specific_value -= t
			else:
				item.type_specific_value = 0
	else:
		i = (num + 100 - lev - (10 * item.type_specific_value)) / 15
		if i < 1:
			i = 1
		if Rng.roll_die(i) == 1:
			Profile.instance.message_print("There is a bright flash of light.")
			if item_index >= 0:
				player.inventory.inven_item_increase(item_index, -999)
				player.inventory.inven_item_describe(item_index)
				player.inventory.inven_item_optimise(item_index)
			else:
				dungeon_level.floor_item_increase(0 - item_index, -999)
				dungeon_level.floor_item_describe(0 - item_index)
				dungeon_level.floor_item_optimize(0 - item_index)
		else:
			t = (num / (lev + 2)) + 1
			if t > 0:
				item.type_specific_value += 2 + Rng.roll_die(t)
			item.identify_flags.clear_bits(IdentifyFlags.KNOWN)
			item.identify_flags.clear_bits(IdentifyFlags.EMPTY)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	return true


func remove_all_curses() -> void:
	remove_curses(true)


func remove_curse() -> bool:
	return remove_curses(false)


func report_magics() -> void:
	var info_text: Array[String]
	var info_value: Array[int]
	if player.timed_blindness != 0:
		info_value.append(report_individual_magic(player.timed_blindness))
		info_text.append("You cannot see")
	if player.timed_confusion != 0:
		info_value.append(report_individual_magic(player.timed_confusion))
		info_text.append("You are confused")
	if player.timed_fear != 0:
		info_value.append(report_individual_magic(player.timed_fear))
		info_text.append("You are terrified")
	if player.timed_poison != 0:
		info_value.append(report_individual_magic(player.timed_poison))
		info_text.append("You are poisoned")
	if player.timed_hallucinations != 0:
		info_value.append(report_individual_magic(player.timed_hallucinations))
		info_text.append("You are hallucinating")
	if player.timed_blessing != 0:
		info_value.append(report_individual_magic(player.timed_blessing))
		info_text.append("You feel rightous")
	if player.timed_heroism != 0:
		info_value.append(report_individual_magic(player.timed_heroism))
		info_text.append("You feel heroic")
	if player.timed_super_heroism != 0:
		info_value.append(report_individual_magic(player.timed_super_heroism))
		info_text.append("You are in a battle rage")
	if player.timed_protection_from_evil != 0:
		info_value.append(report_individual_magic(player.timed_protection_from_evil))
		info_text.append("You are protected from evil")
	if player.timed_stoneskin != 0:
		info_value.append(report_individual_magic(player.timed_stoneskin))
		info_text.append("You are protected by a mystic shield")
	if player.timed_invulnerability != 0:
		info_value.append(report_individual_magic(player.timed_invulnerability))
		info_text.append("You are invulnerable")
	if player.timed_etherealness != 0:
		info_value.append(report_individual_magic(player.timed_etherealness))
		info_text.append("You are incorporeal")
	if player.has_confusing_touch:
		info_value.append(7)
		info_text.append("Your hands are glowing dull red.")
	if player.word_of_recall_delay != 0:
		info_value.append(report_individual_magic(player.word_of_recall_delay))
		info_text.append("You waiting to be recalled")
	if player.timed_acid_resistance != 0:
		info_value.append(report_individual_magic(player.timed_acid_resistance))
		info_text.append("You are resistant to acid")
	if player.timed_lightning_resistance != 0:
		info_value.append(report_individual_magic(player.timed_lightning_resistance))
		info_text.append("You are resistant to lightning")
	if player.timed_fire_resistance != 0:
		info_value.append(report_individual_magic(player.timed_fire_resistance))
		info_text.append("You are resistant to fire")
	if player.timed_cold_resistance != 0:
		info_value.append(report_individual_magic(player.timed_cold_resistance))
		info_text.append("You are resistant to cold")
	if player.timed_poison_resistance != 0:
		info_value.append(report_individual_magic(player.timed_poison_resistance))
		info_text.append("You are resistant to poison")
	Console.save_screen()
	for k: int in range(1, 24):
		Console.clear_to_row_end(k, 13)
	Console.print_at(1, 15, "     Your Current Magic:", Ink.WHITE)
	var k: int = 2
	for j: int in info_text.size():
		var dummy: String = "%s %s." % [info_text[j], Tables.MAGIC_DURATION[info_value[j]]]
		Console.print_at(k, 15, dummy, Ink.WHITE)
		k += 1
		if k == 22 and j + 1 < info_text.size():
			Console.print_at(k, 15, "--more--", Ink.BRIGHT_BLUE)
			User.get_key()
			for i: int in range(k, 1, -1):
				Console.clear_to_row_end(i, 13)
	Console.print_at(k, 13, "[Press any key to continue]", Ink.ORANGE)
	User.get_key()
	Console.restore_screen()


func self_knowledge() -> void:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	var item: Item
	var info_text: Array[String]
	for k: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		var t_1: FlagSet = FlagSet.new()
		var t_2: FlagSet = FlagSet.new()
		var t_3: FlagSet = FlagSet.new()
		item = player.inventory.items[k]
		if item.item_type != null:
			continue
		item.get_merged_flags(t_1, t_2, t_3)
		f_1.set_flags(t_1)
		f_2.set_flags(t_2)
		f_3.set_flags(t_3)
	var ancestral_powers: Array[String] = player.ancestry.self_knowledge(player)
	for power: String in ancestral_powers:
		info_text.append(power)
	var mutations: Array[String] = player.genome.get_mutation_list()
	for power: String in mutations:
		info_text.append(power)
	if player.timed_blindness != 0:
		info_text.append("You cannot see.")
	if player.timed_confusion != 0:
		info_text.append("You are confused.")
	if player.timed_fear != 0:
		info_text.append("You are terrified.")
	if player.timed_bleeding != 0:
		info_text.append("You are bleeding.")
	if player.timed_stun != 0:
		info_text.append("You are stunned.")
	if player.timed_poison != 0:
		info_text.append("You are poisoned.")
	if player.timed_hallucinations != 0:
		info_text.append("You are hallucinating.")
	if player.has_aggravation:
		info_text.append("You aggravate monsters.")
	if player.has_random_teleport:
		info_text.append("Your position is very uncertain.")
	if player.timed_blessing != 0:
		info_text.append("You feel rightous.")
	if player.timed_heroism != 0:
		info_text.append("You feel heroic.")
	if player.timed_super_heroism != 0:
		info_text.append("You are in a battle rage.")
	if player.timed_protection_from_evil != 0:
		info_text.append("You are protected from evil.")
	if player.timed_stoneskin != 0:
		info_text.append("You are protected by a mystic shield.")
	if player.timed_invulnerability != 0:
		info_text.append("You are temporarily invulnerable.")
	if player.timed_etherealness != 0:
		info_text.append("You are temporarily incorporeal.")
	if player.has_confusing_touch:
		info_text.append("Your hands are glowing dull red.")
	if player.is_searching:
		info_text.append("You are looking around very carefully.")
	if player.spare_spell_slots != 0:
		info_text.append("You can learn some spells/prayers.")
	if player.word_of_recall_delay != 0:
		info_text.append("You will soon be recalled.")
	if player.infravision_range != 0:
		info_text.append("Your eyes are sensitive to infrared light.")
	if player.has_see_invisibility:
		info_text.append("You can see invisible creatures.")
	if player.has_feather_fall:
		info_text.append("You can fly.")
	if player.has_free_action:
		info_text.append("You have free action.")
	if player.has_regeneration:
		info_text.append("You regenerate quickly.")
	if player.has_slow_digestion:
		info_text.append("Your appetite is small.")
	if player.has_telepathy:
		info_text.append("You have ESP.")
	if player.has_hold_life:
		info_text.append("You have a firm hold on your life force.")
	if player.has_reflection:
		info_text.append("You reflect arrows and bolts.")
	if player.has_fire_shield:
		info_text.append("You are surrounded with a fiery aura.")
	if player.has_lightning_shield:
		info_text.append("You are surrounded with electricity.")
	if player.has_anti_magic:
		info_text.append("You are surrounded by an anti-magic shell.")
	if player.has_anti_teleport:
		info_text.append("You cannot teleport.")
	if player.has_glow:
		info_text.append("You are carrying a permanent light.")
	if player.has_acid_immunity:
		info_text.append("You are completely immune to acid.")
	elif player.has_acid_resistance and player.timed_acid_resistance != 0:
		info_text.append("You resist acid exceptionally well.")
	elif player.has_acid_resistance or player.timed_acid_resistance != 0:
		info_text.append("You are resistant to acid.")
	if player.has_lightning_immunity:
		info_text.append("You are completely immune to) lightning.")
	elif player.has_lightning_resistance and player.timed_lightning_resistance != 0:
		info_text.append("You resist lightning exceptionally well.")
	elif player.has_lightning_resistance or player.timed_lightning_resistance != 0:
		info_text.append("You are resistant to lightning.")
	if player.has_fire_immunity:
		info_text.append("You are completely immune to fire.")
	elif player.has_fire_resistance and player.timed_fire_resistance != 0:
		info_text.append("You resist fire exceptionally well.")
	elif player.has_fire_resistance or player.timed_fire_resistance != 0:
		info_text.append("You are resistant to fire.")
	if player.has_cold_immunity:
		info_text.append("You are completely immune to cold.")
	elif player.has_cold_resistance and player.timed_cold_resistance != 0:
		info_text.append("You resist cold exceptionally well.")
	elif player.has_cold_resistance or player.timed_cold_resistance != 0:
		info_text.append("You are resistant to cold.")
	if player.has_poison_resistance and player.timed_poison_resistance != 0:
		info_text.append("You resist poison exceptionally well.")
	elif player.has_poison_resistance or player.timed_poison_resistance != 0:
		info_text.append("You are resistant to poison.")
	if player.has_light_resistance:
		info_text.append("You are resistant to bright light.")
	if player.has_dark_resistance:
		info_text.append("You are resistant to darkness.")
	if player.has_confusion_resistance:
		info_text.append("You are resistant to confusion.")
	if player.has_sound_resistance:
		info_text.append("You are resistant to sonic attacks.")
	if player.has_disenchant_resistance:
		info_text.append("You are resistant to disenchantment.")
	if player.has_chaos_resistance:
		info_text.append("You are resistant to chaos.")
	if player.has_shard_resistance:
		info_text.append("You are resistant to blasts of shards.")
	if player.has_nexus_resistance:
		info_text.append("You are resistant to nexus attacks.")
	if player.has_nether_resistance:
		info_text.append("You are resistant to nether forces.")
	if player.has_fear_resistance:
		info_text.append("You are completely fearless.")
	if player.has_blindness_resistance:
		info_text.append("Your eyes are resistant to blindness.")
	if player.has_sustain_strength:
		info_text.append("Your strength is sustained.")
	if player.has_sustain_intelligence:
		info_text.append("Your intelligence is sustained.")
	if player.has_sustain_wisdom:
		info_text.append("Your wisdom is sustained.")
	if player.has_sustain_constitution:
		info_text.append("Your constitution is sustained.")
	if player.has_sustain_dexterity:
		info_text.append("Your dexterity is sustained.")
	if player.has_sustain_charisma:
		info_text.append("Your charisma is sustained.")
	if f_1.is_set(ItemFlags1.STR):
		info_text.append("Your strength is affected by your equipment.")
	if f_1.is_set(ItemFlags1.INT):
		info_text.append("Your intelligence is affected by your equipment.")
	if f_1.is_set(ItemFlags1.WIS):
		info_text.append("Your wisdom is affected by your equipment.")
	if f_1.is_set(ItemFlags1.DEX):
		info_text.append("Your dexterity is affected by your equipment.")
	if f_1.is_set(ItemFlags1.CON):
		info_text.append("Your constitution is affected by your equipment.")
	if f_1.is_set(ItemFlags1.CHA):
		info_text.append("Your charisma is affected by your equipment.")
	if f_1.is_set(ItemFlags1.STEALTH):
		info_text.append("Your stealth is affected by your equipment.")
	if f_1.is_set(ItemFlags1.SEARCH):
		info_text.append("Your searching ability is affected by your equipment.")
	if f_1.is_set(ItemFlags1.INFRA):
		info_text.append("Your infravision is affected by your equipment.")
	if f_1.is_set(ItemFlags1.TUNNEL):
		info_text.append("Your digging ability is affected by your equipment.")
	if f_1.is_set(ItemFlags1.SPEED):
		info_text.append("Your speed is affected by your equipment.")
	if f_1.is_set(ItemFlags1.BLOWS):
		info_text.append("Your attack speed is affected by your equip)ment.")
	item = player.inventory.items[InventorySlot.MELEE_WEAPON]
	if item.item_type != null:
		if f_3.is_set(ItemFlags3.BLESSED):
			info_text.append("Your weapon has been blessed by the gods.")
		if f_1.is_set(ItemFlags1.CHAOTIC):
			info_text.append("Your weapon is branded with the Yellow Sign.")
		if f_1.is_set(ItemFlags1.IMPACT):
			info_text.append("The impact of your weapon can cause earthquakes.")
		if f_1.is_set(ItemFlags1.VORPAL):
			info_text.append("Your weapon is very sharp.")
		if f_1.is_set(ItemFlags1.VAMPIRIC):
			info_text.append("Your weapon drains life from your foes.")
		if f_1.is_set(ItemFlags1.BRAND_ACID):
			info_text.append("Your weapon melts your foes.")
		if f_1.is_set(ItemFlags1.BRAND_ELECTRIC):
			info_text.append("Your weapon shocks your foes.")
		if f_1.is_set(ItemFlags1.BRAND_FIRE):
			info_text.append("Your weapon burns your foes.")
		if f_1.is_set(ItemFlags1.BRAND_COLD):
			info_text.append("Your weapon freezes your foes.")
		if f_1.is_set(ItemFlags1.BRAND_POISON):
			info_text.append("Your weapon poisons your foes.")
		if f_1.is_set(ItemFlags1.SLAY_ANIMAL):
			info_text.append("Your weapon strikes at animals with extra force.")
		if f_1.is_set(ItemFlags1.SLAY_EVIL):
			info_text.append("Your weapon strikes at evil with extra force.")
		if f_1.is_set(ItemFlags1.SLAY_UNDEAD):
			info_text.append("Your weapon strikes at undead with holy wrath.")
		if f_1.is_set(ItemFlags1.SLAY_DEMON):
			info_text.append("Your weapon strikes at demons with holy wrath.")
		if f_1.is_set(ItemFlags1.SLAY_ORC):
			info_text.append("Your weapon is especially deadly against orcs.")
		if f_1.is_set(ItemFlags1.SLAY_TROLL):
			info_text.append("Your weapon is especially deadly against trolls.")
		if f_1.is_set(ItemFlags1.SLAY_GIANT):
			info_text.append("Your weapon is especially deadly against giants.")
		if f_1.is_set(ItemFlags1.SLAY_DRAGON):
			info_text.append("Your weapon is especially deadly against dragons.")
		if f_1.is_set(ItemFlags1.KILL_DRAGON):
			info_text.append("Your weapon is a great bane of dragons.")
	Console.save_screen()
	for k: int in range(1, 24):
		Console.clear_to_row_end(k, 13)
	Console.print_at(1, 15, "     Your Attributes:", Ink.WHITE)
	var k: int = 2
	for j: int in info_text.size():
		Console.print_at(k, 15, info_text[j], Ink.WHITE)
		k += 1
		if k == 22 and j + 1 < info_text.size():
			Console.print_at(k, 15, "--more--", Ink.BRIGHT_BLUE)
			User.get_key()
			for i: int in range(k, 1, -1):
				Console.clear_to_row_end(i, 13)
	Console.print_at(k, 13, "[Press any key to continue]", Ink.ORANGE)
	User.get_key()
	Console.restore_screen()


func set_acid_destroy(item: Item) -> bool:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if not item.hates_acid():
		return false
	item.get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.IGNORE_ACID):
		return false
	return true


func set_cold_destroy(item: Item) -> bool:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if not item.hates_cold():
		return false
	item.get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.IGNORE_COLD):
		return false
	return true


func set_elec_destroy(item: Item) -> bool:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if not item.hates_elec():
		return false
	item.get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.IGNORE_ELEC):
		return false
	return true


func set_fire_destroy(item: Item) -> bool:
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	if not item.hates_fire():
		return false
	item.get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.IGNORE_FIRE):
		return false
	return true


func sleep_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldSleep.new(self), dir, player.level, flag)


func sleep_monsters() -> bool:
	return project_at_all_in_los(ProjectileOldSleep.new(self), player.level)


func sleep_monsters_touch() -> void:
	var flag: int = ProjectionFlag.KILL | ProjectionFlag.HIDE
	project(0, 1, player.map_row, player.map_col, player.level, ProjectileOldSleep.new(self), flag)


func slow_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldSlow.new(self), dir, player.level, flag)


func slow_monsters() -> bool:
	return project_at_all_in_los(ProjectileOldSlow.new(self), player.level)


func speed_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	return targeted_project(ProjectileOldSpeed.new(self), dir, player.level, flag)


func stair_creation() -> void:
	if not dungeon_level.cave_valid_bold(player.map_row, player.map_col):
		Profile.instance.message_print("The object resists the spell.")
		return
	dungeon_level.delete_item_at(player.map_row, player.map_col)
	if savegame.current_depth <= 0:
		dungeon_level.cave_set_feat(player.map_row, player.map_col, "DownStair")
	elif savegame.quests.is_quest_level(savegame.current_depth) or savegame.current_depth >= savegame.current_dungeon.max_level:
		dungeon_level.cave_set_feat(player.map_row, player.map_col, "DownStair" if savegame.current_dungeon.is_tower else "UpStair")
	elif Rng.roll_die(100) <= 50:
		dungeon_level.cave_set_feat(player.map_row, player.map_col, "DownStair")
	else:
		dungeon_level.cave_set_feat(player.map_row, player.map_col, "UpStair")


func stasis_monster(dir: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(ProjectileStasis.new(self), dir, player.level, flag)


func stasis_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectileStasis.new(self), damage)


func stun_monster(dir: int, player_level: int) -> void:
	var flag: int = ProjectionFlag.STOP | ProjectionFlag.KILL
	targeted_project(ProjectileStun.new(self), dir, player_level, flag)


func stun_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectileStun.new(self), damage)


func summon_reaver() -> void:
	var reaver_count: int = (savegame.total_difficulty() / 50) + Rng.roll_die(6)
	for i: int in reaver_count:
		dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, 100, SummonType.SUMMON_REAVER)


func teleport_away(monster_index: int, dis: int) -> void:
	var row_new: int = 0
	var col_new: int = 0
	var look: bool = true
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	if monster.monster_type == null:
		return
	var row_orig: int = monster.map_row
	var col_orig: int = monster.map_col
	var min_dist: int = dis / 2
	while look:
		if dis > 200:
			dis = 200
		for i: int in 500:
			while true:
				row_new = Rng.random_around(row_orig, dis)
				col_new = Rng.random_around(col_orig, dis)
				var d: int = dungeon_level.distance(row_orig, col_orig, row_new, col_new)
				if d >= min_dist and d <= dis:
					break
			if not dungeon_level.in_inner_bounds(row_new, col_new):
				continue
			if not dungeon_level.grid_passable_no_creature(row_new, col_new):
				continue
			if dungeon_level.get_grid(row_new, col_new).feature_type.name == "ElderSign":
				continue
			if dungeon_level.get_grid(row_new, col_new).feature_type.name == "YellowSign":
				continue
			look = false
			break
		dis *= 2
		min_dist /= 2
	dungeon_level.get_grid(row_new, col_new).monster_index = monster_index
	dungeon_level.get_grid(row_orig, col_orig).monster_index = 0
	monster.map_row = row_new
	monster.map_col = col_new
	dungeon_level.dungeon_monsters.update_monster_visibility(monster_index, true)
	dungeon_level.redraw_single_location(row_orig, col_orig)
	dungeon_level.redraw_single_location(row_new, col_new)


func teleport_monster(dir: int) -> bool:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.KILL
	return targeted_project(ProjectileAwayAll.new(self), dir, dungeon_level.MAX_SIGHT_RADIUS * 5, flag)


func teleport_player(dis: int) -> void:
	var row: int = player.map_row
	var col: int = player.map_col
	var look: bool = true
	if player.has_anti_teleport:
		Profile.instance.message_print("A mysterious force prevents you from teleporting!")
		return
	if dis > 200:
		dis = 200
	var min_dist: int = dis / 2
	while look:
		if dis > 200:
			dis = 200
		for i: int in 500:
			while true:
				row = Rng.random_around(player.map_row, dis)
				col = Rng.random_around(player.map_col, dis)
				var d: int = dungeon_level.distance(player.map_row, player.map_col, row, col)
				if d >= min_dist and d <= dis:
					break
			if not dungeon_level.in_inner_bounds(row, col):
				continue
			if not dungeon_level.grid_open_no_item_or_creature(row, col):
				continue
			if dungeon_level.get_grid(row, col).tile_flags.is_set(GridTile.IN_VAULT):
				continue
			look = false
			break
		dis *= 2
		min_dist /= 2
	var row_orig: int = player.map_row
	var col_orig: int = player.map_col
	player.map_row = row
	player.map_col = col
	dungeon_level.redraw_single_location(row_orig, col_orig)
	for row_1: int in range(row_orig - 1, row_orig + 2):
		for col_1: int in range(col_orig - 1, col_orig + 2):
			if col_1 == col_orig and row_1 == row_orig:
				pass
			else:
				var monster_index: int = dungeon_level.get_grid(row_1, col_1).monster_index
				if monster_index != 0:
					var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
					var monster_type: MonsterType = monster.monster_type
					if monster_type.flags_6.is_set(MonsterFlags6.TELEPORT_SELF) and monster_type.flags_3.is_clear(MonsterFlags3.RESIST_TELEPORT):
						if monster.sleep_level == 0:
							teleport_to_player(monster_index)
	dungeon_level.redraw_single_location(player.map_row, player.map_col)
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	target_engine.recenter_screen_around_player()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
	savegame.notice_update_redraw()


func teleport_player_level() -> void:
	if player.has_anti_teleport:
		Profile.instance.message_print("A mysterious force prevents you from teleporting!")
		return
	var down_desc: String = "You rise up through the ceiling." if savegame.current_dungeon.is_tower else "You sink through the floor."
	var up_desc: String = "You sink through the floor." if savegame.current_dungeon.is_tower else "You rise up through the ceiling."
	savegame.came_from = LevelStart.START_RANDOM
	if savegame.current_depth <= 0:
		Profile.instance.message_print(down_desc)
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
		savegame.current_depth += 1
		savegame.new_level_flag = true
	elif savegame.quests.is_quest_level(savegame.current_depth) or savegame.current_depth >= savegame.current_dungeon.max_level:
		Profile.instance.message_print(up_desc)
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
		savegame.current_depth -= 1
		savegame.new_level_flag = true
	elif Rng.roll_die(100) <= 50:
		Profile.instance.message_print(up_desc)
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
		savegame.current_depth -= 1
		savegame.new_level_flag = true
	else:
		Profile.instance.message_print(down_desc)
		savegame.is_auto_save = true
		savegame.command_save_game()
		savegame.is_auto_save = false
		savegame.current_depth += 1
		savegame.new_level_flag = true
	savegame.is_auto_save = true
	savegame.command_save_game()
	savegame.is_auto_save = false
	savegame.current_depth += 1
	savegame.new_level_flag = true


func teleport_player_to(row_new: int, col_new: int) -> void:
	var row: int
	var col: int
	var dis: int = 0
	var ctr: int = 0
	if player.has_anti_teleport:
		Profile.instance.message_print("A mysterious force prevents you from teleporting!")
		return
	while true:
		while true:
			row = Rng.random_around(row_new, dis)
			col = Rng.random_around(col_new, dis)
			if dungeon_level.in_inner_bounds(row, col):
				break
		if dungeon_level.grid_open_no_item_or_creature(row, col):
			break
		ctr += 1
		if ctr > (4 * dis * dis) + (4 * dis) + 1:
			ctr = 0
			dis += 1
	var row_orig: int = player.map_row
	var col_orig: int = player.map_col
	player.map_row = row
	player.map_col = col
	dungeon_level.redraw_single_location(row_orig, col_orig)
	dungeon_level.redraw_single_location(player.map_row, player.map_col)
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	target_engine.recenter_screen_around_player()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
	savegame.notice_update_redraw()


func teleport_swap(dir: int) -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var col_target: int
	var row_target: int
	if dir == 5 and target_engine.target_okay():
		col_target = savegame.target_col
		row_target = savegame.target_row
	else:
		col_target = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		row_target = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var tile: GridTile = dungeon_level.Grid[row_target][col_target]
	if tile.monster_index == 0:
		Profile.instance.message_print("You can't trade places with that!")
	else:
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
		var monster_type: MonsterType = monster.monster_type
		if monster_type.flags_3.is_set(MonsterFlags3.RESIST_TELEPORT):
			Profile.instance.message_print("Your teleportation is blocked!")
		else:
			dungeon_level.Grid[player.map_row][player.map_col].monster_index = tile.monster_index
			tile.monster_index = 0
			monster.map_row = player.map_row
			monster.map_col = player.map_col
			player.map_col = col_target
			player.map_row = row_target
			col_target = monster.map_col
			row_target = monster.map_row
			dungeon_level.dungeon_monsters.update_monster_visibility(dungeon_level.Grid[row_target][col_target].monster_index, true)
			dungeon_level.redraw_single_location(row_target, col_target)
			dungeon_level.redraw_single_location(player.map_row, player.map_col)
			target_engine.recenter_screen_around_player()
			player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
			player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)
			savegame.notice_update_redraw()


func trap_creation() -> bool:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.HIDE
	return project(0, 1, player.map_row, player.map_col, 0, ProjectileMakeTrap.new(self), flag)


func turn_evil(damage: int) -> void:
	project_at_all_in_los(ProjectileTurnEvil.new(self), damage)


func turn_monsters(damage: int) -> void:
	project_at_all_in_los(ProjectileTurnAll.new(self), damage)


func unlight_area(damage: int, rad: int) -> bool:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.KILL
	if player.timed_blindness == 0:
		Profile.instance.message_print("Darkness surrounds you.")
	project(0, rad, player.map_row, player.map_col, damage, ProjectileDarkWeak.new(self), flag)
	unlight_room(player.map_row, player.map_col)
	return true


func unlight_room(row_1: int, col_1: int) -> void:
	cave_temp_room_aux(row_1, col_1)
	for i: int in dungeon_level.locations_for_redraw_count:
		var col: int = dungeon_level.locations_for_redraw[i].col
		var row: int = dungeon_level.locations_for_redraw[i].row
		if not dungeon_level.grid_passable(row, col):
			continue
		cave_temp_room_aux(row + 1, col)
		cave_temp_room_aux(row - 1, col)
		cave_temp_room_aux(row, col + 1)
		cave_temp_room_aux(row, col - 1)
		cave_temp_room_aux(row + 1, col + 1)
		cave_temp_room_aux(row - 1, col - 1)
		cave_temp_room_aux(row - 1, col + 1)
		cave_temp_room_aux(row + 1, col - 1)
	cave_temp_room_unlight()


func wall_breaker() -> void:
	var dummy: int
	if Rng.roll_die(80 + player.level) < 70:
		dummy = Rng.roll_die(9)
		while dummy == 5 or dummy == 0:
			dummy = Rng.roll_die(9)
		wall_to_mud(dummy)
	elif Rng.roll_die(100) > 30:
		earthquake(player.map_row, player.map_col, 1)
	else:
		for i: int in 10:
			if i != 5:
				wall_to_mud(i)


func wall_stone() -> void:
	var flag: int = ProjectionFlag.GRID | ProjectionFlag.ITEM
	project(0, 1, player.map_row, player.map_col, 0, ProjectileStoneWall.new(self), flag)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func wall_to_mud(dir: int) -> bool:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	return targeted_project(ProjectileKillWall.new(self), dir, 20 + Rng.roll_die(30), flag)


func wizard_lock(dir: int) -> void:
	var flag: int = ProjectionFlag.BEAM | ProjectionFlag.GRID | ProjectionFlag.ITEM | ProjectionFlag.KILL
	targeted_project(ProjectileJamDoor.new(self), dir, 20 + Rng.roll_die(30), flag)


func yellow_sign() -> void:
	if not dungeon_level.grid_open_no_item(player.map_row, player.map_col):
		Profile.instance.message_print("The object resists the spell.")
		return
	dungeon_level.cave_set_feat(player.map_row, player.map_col, "YellowSign")


func cave_temp_room_aux(row: int, col: int) -> void:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if tile.tile_flags.is_set(GridTile.NEEDS_REDRAW):
		return
	if tile.tile_flags.is_clear(GridTile.IN_ROOM):
		return
	if dungeon_level.locations_for_redraw_count == DungeonLevel.VIEW_MAX:
		return
	tile.tile_flags.set_bits(GridTile.NEEDS_REDRAW)
	dungeon_level.locations_for_redraw[dungeon_level.locations_for_redraw_count].row = row
	dungeon_level.locations_for_redraw[dungeon_level.locations_for_redraw_count].col = col
	dungeon_level.locations_for_redraw_count += 1


func cave_temp_room_light() -> void:
	for i: int in dungeon_level.locations_for_redraw_count:
		var row: int = dungeon_level.locations_for_redraw[i].row
		var col: int = dungeon_level.locations_for_redraw[i].col
		var tile: GridTile = dungeon_level.get_grid(row, col)
		tile.tile_flags.clear_bits(GridTile.NEEDS_REDRAW)
		tile.tile_flags.set_bits(GridTile.SELF_LIT)
		if tile.monster_index != 0:
			var chance: int = 25
			var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
			var monster_type: MonsterType = monster.monster_type
			dungeon_level.dungeon_monsters.update_monster_visibility(tile.monster_index, false)
			if monster_type.flags_2.is_set(MonsterFlags2.STUPID):
				chance = 10
			if monster_type.flags_2.is_set(MonsterFlags2.SMART):
				chance = 100
			if monster.sleep_level != 0 and Rng.roll_die(100) <= chance:
				monster.sleep_level = 0
				if monster.is_visible:
					var monster_name: String = monster.describe(0)
					Profile.instance.message_print("%s wakes up." % [monster_name])
		dungeon_level.note_spot(row, col)
		dungeon_level.redraw_single_location(row, col)
	dungeon_level.locations_for_redraw_count = 0


func cave_temp_room_unlight() -> void:
	for i: int in dungeon_level.locations_for_redraw_count:
		var row: int = dungeon_level.locations_for_redraw[i].row
		var col: int = dungeon_level.locations_for_redraw[i].col
		var tile: GridTile = dungeon_level.get_grid(row, col)
		tile.tile_flags.clear_bits(GridTile.NEEDS_REDRAW)
		tile.tile_flags.clear_bits(GridTile.SELF_LIT)
		if tile.feature_type.is_open_floor:
			tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
			dungeon_level.note_spot(row, col)
		if tile.monster_index != 0:
			dungeon_level.dungeon_monsters.update_monster_visibility(tile.monster_index, false)
		dungeon_level.redraw_single_location(row, col)
	dungeon_level.locations_for_redraw_count = 0


func detect_monsters_string(to_match: String) -> bool:
	var flag: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var monster_type: MonsterType = monster.monster_type
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.panel_contains(row, col):
			continue
		if to_match.contains(monster_type.character):
			dungeon_level.dungeon_monsters.repair_monsters = true
			monster.individual_monster_flags.set_bits(IndividualMonsterFlags.MARK | IndividualMonsterFlags.SHOW)
			monster.is_visible = true
			dungeon_level.redraw_single_location(row, col)
			flag = true
	if flag:
		Profile.instance.message_print("You sense the presence of monsters!")
	return flag


func item_filter_weapon(item: Item) -> bool:
	match item.category:
		ItemCategory.SWORD, \
		ItemCategory.HAFTED, \
		ItemCategory.POLEARM, \
		ItemCategory.DIGGING, \
		ItemCategory.BOW, \
		ItemCategory.BOLT, \
		ItemCategory.ARROW, \
		ItemCategory.SHOT:
			return true
	return false


func light_room(row_1: int, col_1: int) -> void:
	cave_temp_room_aux(row_1, col_1)
	for i: int in dungeon_level.locations_for_redraw_count:
		var col: int = dungeon_level.locations_for_redraw[i].col
		var row: int = dungeon_level.locations_for_redraw[i].row
		if not dungeon_level.grid_passable(row, col):
			continue
		cave_temp_room_aux(row + 1, col)
		cave_temp_room_aux(row - 1, col)
		cave_temp_room_aux(row, col + 1)
		cave_temp_room_aux(row, col - 1)
		cave_temp_room_aux(row + 1, col + 1)
		cave_temp_room_aux(row - 1, col - 1)
		cave_temp_room_aux(row - 1, col + 1)
		cave_temp_room_aux(row + 1, col - 1)
	cave_temp_room_light()


func minus_ac() -> bool:
	var item: Item = null
	var f_1: FlagSet = FlagSet.new()
	var f_2: FlagSet = FlagSet.new()
	var f_3: FlagSet = FlagSet.new()
	match Rng.roll_die(6):
		1:
			item = player.inventory.items[InventorySlot.BODY]
		2:
			item = player.inventory.items[InventorySlot.ARM]
		3:
			item = player.inventory.items[InventorySlot.CLOAK]
		4:
			item = player.inventory.items[InventorySlot.HANDS]
		5:
			item = player.inventory.items[InventorySlot.HEAD]
		6:
			item = player.inventory.items[InventorySlot.FEET]
	if item == null:
		return false
	if item.item_type == null:
		return false
	if item.base_armour_class + item.bonus_armour_class <= 0:
		return false
	var item_name: String = item.description(false, 0)
	item.get_merged_flags(f_1, f_2, f_3)
	if f_3.is_set(ItemFlags3.IGNORE_ACID):
		Profile.instance.message_print("Your %s is unaffected!" % [item_name])
		return true
	Profile.instance.message_print("Your %s is damaged!" % [item_name])
	item.bonus_armour_class -= 1
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	return true


func project_at_all_in_los(projectile: Projectile, damage: int) -> bool:
	var flag: int = ProjectionFlag.JUMP | ProjectionFlag.KILL | ProjectionFlag.HIDE
	var obvious: bool = false
	for i: int in range(1, dungeon_level.monster_count_max):
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		if monster.monster_type == null:
			continue
		var row: int = monster.map_row
		var col: int = monster.map_col
		if not dungeon_level.player_has_los_bold(row, col):
			continue
		if project(0, 0, row, col, damage, projectile, flag):
			obvious = true
	return obvious


func remove_curses(all: bool) -> bool:
	var cnt: int = 0
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		var f_1: FlagSet = FlagSet.new()
		var f_2: FlagSet = FlagSet.new()
		var f_3: FlagSet = FlagSet.new()
		var item: Item = player.inventory.items[i]
		if item.item_type == null:
			continue
		if not item.is_cursed():
			continue
		item.get_merged_flags(f_1, f_2, f_3)
		if !all and f_3.is_set(ItemFlags3.HEAVY_CURSE):
			continue
		if f_3.is_set(ItemFlags3.PERMA_CURSE):
			continue
		item.identify_flags.clear_bits(IdentifyFlags.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.SENSED)
		if item.legendary_flags_3.is_set(ItemFlags3.CURSED):
			item.legendary_flags_3.clear_bits(ItemFlags3.CURSED)
		if item.legendary_flags_3.is_set(ItemFlags3.HEAVY_CURSE):
			item.legendary_flags_3.clear_bits(ItemFlags3.HEAVY_CURSE)
		item.inscription = "uncursed"
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		cnt += 1
	return cnt > 0


func report_individual_magic(dur: int) -> int:
	if dur <= 5:
		return 0
	if dur <= 10:
		return 1
	if dur <= 20:
		return 2
	if dur <= 50:
		return 3
	if dur <= 100:
		return 4
	if dur <= 200:
		return 5
	return 6


func targeted_project(projectile: Projectile, dir: int, damage: int, flag: int) -> bool:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	flag |= ProjectionFlag.THRU
	var col_target: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var row_target: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	if dir == 5 and target_engine.target_okay():
		col_target = savegame.target_col
		row_target = savegame.target_row
	return project(0, 0, row_target, col_target, damage, projectile, flag)


func teleport_to_player(monster_index: int) -> void:
	var row_new: int = player.map_row
	var col_new: int = player.map_col
	var dis: int = 2
	var look: bool = true
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var attempts: int = 500
	if monster.monster_type == null:
		return
	if Rng.roll_die(100) > monster.monster_type.level:
		return
	var row_orig: int = monster.map_row
	var col_orig: int = monster.map_col
	var min_dist: int = dis / 2
	while look and attempts != 0:
		attempts -= 1
		if dis > 200:
			dis = 200
		for i: int in 500:
			while true:
				row_new = Rng.random_around(player.map_row, dis)
				col_new = Rng.random_around(player.map_col, dis)
				var d: int = dungeon_level.distance(player.map_row, player.map_col, row_new, col_new)
				if d >= min_dist and d <= dis:
					break
			if not dungeon_level.in_inner_bounds(row_new, col_new):
				continue
			if not dungeon_level.grid_passable_no_creature(row_new, col_new):
				continue
			if dungeon_level.get_grid(row_new, col_new).feature_type.name == "ElderSign":
				continue
			if dungeon_level.get_grid(row_new, col_new).feature_type.name == "YellowSign":
				continue
			look = false
			break
		dis *= 2
		min_dist /= 2
	if attempts < 1:
		return
	dungeon_level.get_grid(row_new, col_new).monster_index = monster_index
	dungeon_level.Grid[row_orig][col_orig].monster_index = 0
	monster.map_row = row_new
	monster.map_col = col_new
	dungeon_level.dungeon_monsters.update_monster_visibility(monster_index, true)
	dungeon_level.redraw_single_location(row_orig, col_orig)
	dungeon_level.redraw_single_location(row_new, col_new)
