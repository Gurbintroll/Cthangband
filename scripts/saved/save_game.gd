# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SaveGame
extends Resource
## A Cthangband game, somewhere from character generation to death/retirement

## The singleton instance of the currently loaded save game, will be null between games
static var instance: SaveGame

## The player
@export var player: Player
## The current dungeon level
@export var dungeon_level: DungeonLevel
@export var quests: QuestList
@export var random_item_table: Array[FindChance]
@export var random_item_table_size: int
@export var random_monster_table: Array[FindChance]
@export var random_monster_table_size: int
## All dungeons
@export var dungeons: Array[Dungeon]
## The wilderness map
@export var island: Island
## The towns
@export var towns: Array[Town]
## The RNG Seed for flavours
@export var seed_flavour: int
## Our current depth
@export var current_depth: int
## Our current town
@export var current_town: Town
## Our current dungeon
@export var current_dungeon: Dungeon
## The dungeon we will recall back to
@export var recall_dungeon: int
## The difficulty of the current dungeon
@export var dungeon_difficulty: int
## Where the player came from
@export var came_from: int
# Who we are targeting
@export var target_who: int
@export var target_row: int
@export var target_col: int
# Whose health we are tracking
@export var tracked_monster_index: int
@export var total_friends: int
@export var total_friend_levels: int
# Items for this game
@export var item_types: ItemTypeArray
@export var artifacts: Dictionary[int, Artifact]
# Flavours for this game
@export var amulet_flavours: Array[Flavour]
@export var mushroom_flavours: Array[Flavour]
@export var potion_flavours: Array[Flavour]
@export var ring_flavours: Array[Flavour]
@export var rod_flavours: Array[Flavour]
@export var scroll_flavours: Array[Flavour]
@export var staff_flavours: Array[Flavour]
@export var wand_flavours: Array[Flavour]
@export var playing: bool
@export var new_level_flag: bool
@export var san_checks_allowed: bool
@export var create_up_stair: bool
@export var create_down_stair: bool
@export var resting: int
@export var running: int
@export var died_from: String
@export var energy_use: int
@export var viewing_item_list: bool
@export var viewing_equipment: bool
@export var item_filter_all: bool
@export var item_display_column: int = 50
@export var is_auto_save: bool
## A flag to say that we're loading the game and messages should be suppressed
@export var in_post_load_update: bool

var item_filter_function: Callable = Callable()
var command_handler: CommandHandler = CommandHandler.new()
var command_engine: CommandEngine = CommandEngine.new()
var combat_engine: CombatEngine = CombatEngine.new()
var spell_effects_handler: SpellEffectsHandler = SpellEffectsHandler.new()


func total_difficulty() -> int:
	return dungeon_difficulty + current_depth


func dungeon_loop() -> void:
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, self)
	new_level_flag = false
	san_checks_allowed = false
	User.current_command = ""
	User.queued_command = ""
	User.command_repeat = 0
	User.command_argument = 0
	User.command_direction = 0
	target_who = 0
	health_track(0)
	dungeon_level.dungeon_monsters.shimmer_monsters = true
	dungeon_level.dungeon_monsters.repair_monsters = true
	disturb(true)
	if player.max_level_gained < player.level:
		player.max_level_gained = player.level
	if player.max_dlv[current_dungeon.index] < current_depth:
		player.max_dlv[current_dungeon.index] = current_depth
	# On a quest level, don't no stair creation (yet)
	if quests.is_quest_level(current_depth):
		if current_dungeon.is_tower:
			create_up_stair = false
		else:
			create_down_stair = false
	# No stair creation on the surface
	if current_depth == 0:
		create_up_stair = false
		create_down_stair = false
	# If we are allowed to create a stair, create one where the player arrived
	if create_down_stair or create_up_stair:
		if dungeon_level.cave_valid_bold(player.map_row, player.map_col):
			dungeon_level.delete_item_at(player.map_row, player.map_col)
			dungeon_level.cave_set_feat(player.map_row, player.map_col, "DownStair" if create_down_stair else "UpStair")
		create_down_stair = false
		create_up_stair = false
	target_engine.recenter_screen_around_player()
	target_engine.panel_bounds_center()
	in_post_load_update = true
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_WIPE | RedrawFlags.REDRAW_BASIC | RedrawFlags.REDRAW_EXTRA)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EQUIPPY | RedrawFlags.REDRAW_MAP)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_SPELLS | UpdateFlags.UPDATE_TORCH_RADIUS)
	notice_update_redraw()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_DISTANCES)
	notice_update_redraw()
	in_post_load_update = false
	player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	notice_update_redraw()
	if not playing or player.is_dead or new_level_flag:
		return
	if quests.is_quest_level(current_depth):
		quests.discover_quest()
	dungeon_level.monster_level = total_difficulty()
	dungeon_level.item_level = total_difficulty()
	san_checks_allowed = true
	if came_from == LevelStart.START_HOUSE:
		update_handler_objects()
		command_handler.command_store()
		came_from = LevelStart.START_RANDOM
	if current_depth == 0:
		if total_difficulty() == 0:
			Console.change_music("Town")
		else:
			Console.change_music("Wilderness")
	else:
		if quests.is_quest_level(current_depth):
			Console.change_music("Quest")
		else:
			Console.change_music("Dungeon")
	while true:
		if dungeon_level.monster_count + 32 > DungeonMonsters.MAX_DUNGEON_MONSTERS:
			dungeon_level.dungeon_monsters.compact_monsters(64)
		if dungeon_level.monster_count + 32 < dungeon_level.monster_count_max:
			dungeon_level.dungeon_monsters.compact_monsters(0)
		if dungeon_level.item_count + 32 > DungeonItems.MAX_DUNGEON_ITEMS:
			dungeon_level.dungeon_items.compact_items(64)
		if dungeon_level.item_count + 32 < dungeon_level.item_count_max:
			dungeon_level.compact_items(0)
		process_player()
		notice_update_redraw()
		if not playing or player.is_dead or new_level_flag:
			break
		total_friends = 0
		total_friend_levels = 0
		var ai: ArtificialIntelligence = ArtificialIntelligence.new(player, dungeon_level)
		ai.process_all_monsters()
		notice_update_redraw()
		if not playing or player.is_dead or new_level_flag:
			break
		process_world()
		notice_update_redraw()
		if not playing or player.is_dead or new_level_flag:
			break
		player.game_time.tick()
		if not player.game_time.is_feel_time:
			continue
		if current_depth > 0:
			command_handler.command_feeling(true)


func notice_update_redraw() -> void:
	if player == null:
		return
	if player.notice_flags.is_any_set():
		notice_stuff()
	if player.updates_needed.is_any_set():
		update_stuff()
	if player.redraw_needed.is_any_set():
		redraw_stuff()


func disturb(stop_search: bool) -> void:
	if User.command_repeat != 0:
		User.command_repeat = 0
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
	if resting != 0:
		resting = 0
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
	if running != 0:
		running = 0
		player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
	if stop_search and player.is_searching:
		player.is_searching = false
		player.updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)


## We want to play the game - either because we've just started it or just loaded it
func play() -> void:
	# Set up our handlers because we might have just been loaded
	item_filter_function = Callable()
	command_handler = CommandHandler.new()
	command_engine = CommandEngine.new()
	combat_engine = CombatEngine.new()
	spell_effects_handler = SpellEffectsHandler.new()
	update_handler_objects()
	# We start with a full screen overlay
	Console.full_screen_overlay = true
	Console.set_background(BackgroundImage.DEFAULT)
	Rng.use_main_sequence()
	# If we don't have a player then we must be a new save
	if player == null:
		var player_factory: PlayerFactory = PlayerFactory.new()
		var new_player: Player = player_factory.generate_character(Profile.instance.ex_player)
		if new_player == null:
			return
		player = new_player
		# Initialising the savegame is SLOW, so we do it after character generation
		var save_factory: SaveGameFactory = SaveGameFactory.new(self)
		save_factory.initialise_savegame()
	Profile.instance.message_should_continue = false
	Profile.instance.message_print()
	# If we don't have a level (because we're a new game) then we need one
	if dungeon_level == null:
		dungeon_level = DungeonLevel.new_dungeon_level()
		var factory: LevelFactory = LevelFactory.new(dungeon_level, self)
		factory.generate_new_level()
	# We're ready to start
	Console.full_screen_overlay = false
	Console.set_background(BackgroundImage.IN_GAME)
	playing = true
	# A quick sanity check
	if player.health < 0:
		player.is_dead = true
	# Loop till we need to stop
	while true:
		# Play the dungeon level
		dungeon_loop()
		# We have left the level for some reason (dead? quitting? stairs? recall?)
		# Firstly, clean up the level
		notice_update_redraw()
		target_who = 0
		health_track(0)
		dungeon_level.forget_light()
		dungeon_level.forget_view()
		# Now check the status - if we're not playing then exit the loop
		if not playing and not player.is_dead:
			break
		# We're still playing, so clean up the level
		dungeon_level.dungeon_items.wipe()
		var pet_list: Array[Monster] = dungeon_level.dungeon_monsters.get_pets()
		dungeon_level.dungeon_monsters.wipe()
		Profile.instance.message_print()
		# If the playter is dead, end the game
		if player.is_dead:
			Profile.instance.ex_player = ExPlayer.new()
			Profile.instance.ex_player.copy_player(player)
			break
		# We're still playing and not dead, so we must have moved to a new level somehow
		var factory: LevelFactory = LevelFactory.new(dungeon_level, self)
		factory.generate_new_level()
		dungeon_level.replace_pets(player.map_row, player.map_col, pet_list)
	close_game()


func close_game() -> void:
	notice_update_redraw()
	Profile.instance.message_print()
	Console.full_screen_overlay = true
	if player.is_dead:
		if player.is_winner:
			show_victory_screen()
		var corpse: Player = player
		var score: HighScore = HighScore.new_highscore(player, self)
		player = null
		save_player()
		print_tomb(corpse)
		if corpse.is_wizard:
			return
		Console.high_score_table.insert_new_score(score)
		Console.high_score_table.display_scores_around_score(score.points)
	else:
		is_auto_save = false
		command_save_game()
		if not Console.exit_to_desktop:
			Console.change_music("Menu")
			Console.high_score_table.display_scores_around_living(HighScore.new_highscore(player, self))


func show_victory_screen() -> void:
	current_depth = 0
	died_from = "Ripe Old Age"
	player.experience_points = player.max_experience_gained
	player.level = player.max_level_gained
	player.gold += 10000000
	Console.set_background(BackgroundImage.CROWN)
	Console.clear_screen()
	Console.any_key(44)


func update_stuff() -> void:
	var player_status: PlayerStatus = PlayerStatus.new(player, dungeon_level)
	if player.updates_needed.is_set(UpdateFlags.UPDATE_BONUSES):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_BONUSES)
		player_status.calc_bonuses()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_TORCH_RADIUS):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
		player_status.calc_torch()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_HEALTH):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_HEALTH)
		player_status.calc_hit_points()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_POWER):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_POWER)
		player_status.calc_vis()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_SPELLS):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_SPELLS)
		player_status.calc_spells()
	if player == null:
		return
	if Console.full_screen_overlay:
		return
	if player.updates_needed.is_set(UpdateFlags.UPDATE_REMOVE_LIGHT):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_REMOVE_LIGHT)
		dungeon_level.forget_light()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_REMOVE_VIEW):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_REMOVE_VIEW)
		dungeon_level.forget_view()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_VIEW):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_VIEW)
		dungeon_level.update_view()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_LIGHT):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_LIGHT)
		dungeon_level.update_light()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_SCENT):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_SCENT)
		dungeon_level.update_scent_trail()
	if player.updates_needed.is_set(UpdateFlags.UPDATE_DISTANCES):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_DISTANCES)
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_MONSTERS)
		dungeon_level.update_monsters(true)
	if player.updates_needed.is_set(UpdateFlags.UPDATE_MONSTERS):
		player.updates_needed.clear_bits(UpdateFlags.UPDATE_MONSTERS)
		dungeon_level.update_monsters(false)


func redraw_stuff() -> void:
	if player == null:
		return
	if not player.redraw_needed.is_any_set():
		return
	if Console.full_screen_overlay:
		return
	var player_status: PlayerStatus = PlayerStatus.new(player, dungeon_level)
	var redraw: FlagSet = player.redraw_needed
	# Wipe the screen if necessary
	if redraw.is_set(RedrawFlags.REDRAW_WIPE):
		redraw.clear_bits(RedrawFlags.REDRAW_WIPE)
		Console.clear_screen()
	# Print the map if necessary
	if redraw.is_set(RedrawFlags.REDRAW_MAP):
		redraw.clear_bits(RedrawFlags.REDRAW_MAP)
		dungeon_level.print_map()
	if redraw.is_set(RedrawFlags.REDRAW_BASIC):
		redraw.clear_bits(RedrawFlags.REDRAW_BASIC)
		redraw.clear_bits(RedrawFlags.REDRAW_MISC | RedrawFlags.REDRAW_TITLE | RedrawFlags.REDRAW_STATS)
		redraw.clear_bits(RedrawFlags.REDRAW_LEV | RedrawFlags.REDRAW_EXP | RedrawFlags.REDRAW_GOLD)
		redraw.clear_bits(RedrawFlags.REDRAW_ARMOR | RedrawFlags.REDRAW_HP | RedrawFlags.REDRAW_POWER)
		redraw.clear_bits(RedrawFlags.REDRAW_DEPTH | RedrawFlags.REDRAW_HEALTH)
		player_status.print_frame_basic()
	if redraw.is_set(RedrawFlags.REDRAW_EQUIPPY):
		redraw.clear_bits(RedrawFlags.REDRAW_EQUIPPY)
		player_status.print_equippy()
	if redraw.is_set(RedrawFlags.REDRAW_MISC):
		redraw.clear_bits(RedrawFlags.REDRAW_MISC)
		player_status.print_field(player.ancestry.title, ScreenLocation.ROW_RACE, ScreenLocation.COL_RACE)
		player_status.print_field(player.profession.get_profession_sub_name(player.realm_1), ScreenLocation.ROW_CLASS, ScreenLocation.COL_CLASS)
	if redraw.is_set(RedrawFlags.REDRAW_TITLE):
		redraw.clear_bits(RedrawFlags.REDRAW_TITLE)
		player_status.print_title()
	if redraw.is_set(RedrawFlags.REDRAW_LEV):
		redraw.clear_bits(RedrawFlags.REDRAW_LEV)
		player_status.print_level()
	if redraw.is_set(RedrawFlags.REDRAW_EXP):
		redraw.clear_bits(RedrawFlags.REDRAW_EXP)
		player_status.print_experience()
	if redraw.is_set(RedrawFlags.REDRAW_STATS):
		redraw.clear_bits(RedrawFlags.REDRAW_STATS)
		player_status.print_stat(Ability.STRENGTH)
		player_status.print_stat(Ability.INTELLIGENCE)
		player_status.print_stat(Ability.WISDOM)
		player_status.print_stat(Ability.DEXTERITY)
		player_status.print_stat(Ability.CONSTITUTION)
		player_status.print_stat(Ability.CHARISMA)
	if redraw.is_set(RedrawFlags.REDRAW_ARMOR):
		redraw.clear_bits(RedrawFlags.REDRAW_ARMOR)
		player_status.print_armour_class()
	if redraw.is_set(RedrawFlags.REDRAW_HP):
		redraw.clear_bits(RedrawFlags.REDRAW_HP)
		player_status.print_hit_points()
	if redraw.is_set(RedrawFlags.REDRAW_POWER):
		redraw.clear_bits(RedrawFlags.REDRAW_POWER)
		player_status.print_power_points()
	if redraw.is_set(RedrawFlags.REDRAW_GOLD):
		redraw.clear_bits(RedrawFlags.REDRAW_GOLD)
		player_status.print_gold()
	if redraw.is_set(RedrawFlags.REDRAW_DEPTH):
		redraw.clear_bits(RedrawFlags.REDRAW_DEPTH)
		player_status.print_depth()
	if redraw.is_set(RedrawFlags.REDRAW_HEALTH):
		redraw.clear_bits(RedrawFlags.REDRAW_HEALTH)
		player_status.print_health_tracker()
	if redraw.is_set(RedrawFlags.REDRAW_EXTRA):
		redraw.clear_bits(RedrawFlags.NOTIFICATION_EXTENSION_RELOADED)
		redraw.clear_bits(RedrawFlags.REDRAW_CUT | RedrawFlags.REDRAW_STUN | RedrawFlags.REDRAW_HUNGER)
		redraw.clear_bits(RedrawFlags.REDRAW_DTRAP | RedrawFlags.REDRAW_BLIND | RedrawFlags.REDRAW_CONFUSED)
		redraw.clear_bits(RedrawFlags.REDRAW_AFRAID | RedrawFlags.REDRAW_POISONED | RedrawFlags.REDRAW_STATE)
		redraw.clear_bits(RedrawFlags.REDRAW_SPEED | RedrawFlags.REDRAW_STUDY)
		player_status.print_frame_extra()
	if redraw.is_set(RedrawFlags.REDRAW_CUT):
		redraw.clear_bits(RedrawFlags.REDRAW_CUT)
		player_status.print_cuts()
	if redraw.is_set(RedrawFlags.REDRAW_STUN):
		redraw.clear_bits(RedrawFlags.REDRAW_STUN)
		player_status.print_stun()
	if redraw.is_set(RedrawFlags.REDRAW_HUNGER):
		redraw.clear_bits(RedrawFlags.REDRAW_HUNGER)
		player_status.print_hunger()
	if redraw.is_set(RedrawFlags.REDRAW_DTRAP):
		redraw.clear_bits(RedrawFlags.REDRAW_DTRAP)
		player_status.print_trap_detection()
	if redraw.is_set(RedrawFlags.REDRAW_BLIND):
		redraw.clear_bits(RedrawFlags.REDRAW_BLIND)
		player_status.print_blindness()
	if redraw.is_set(RedrawFlags.REDRAW_CONFUSED):
		redraw.clear_bits(RedrawFlags.REDRAW_CONFUSED)
		player_status.print_confused()
	if redraw.is_set(RedrawFlags.REDRAW_AFRAID):
		redraw.clear_bits(RedrawFlags.REDRAW_AFRAID)
		player_status.print_afraid()
	if redraw.is_set(RedrawFlags.REDRAW_POISONED):
		redraw.clear_bits(RedrawFlags.REDRAW_POISONED)
		player_status.print_poisoned()
	if redraw.is_set(RedrawFlags.REDRAW_STATE):
		redraw.clear_bits(RedrawFlags.REDRAW_STATE)
		player_status.print_state()
	if redraw.is_set(RedrawFlags.REDRAW_SPEED):
		redraw.clear_bits(RedrawFlags.REDRAW_SPEED)
		player_status.print_speed()
	if redraw.is_set(RedrawFlags.REDRAW_STUDY):
		redraw.clear_bits(RedrawFlags.REDRAW_STUDY)
		player_status.print_study()
	player_status.print_time()


func notice_stuff() -> void:
	if player.notice_flags.is_any_set() == false:
		return
	if player.notice_flags.is_set(NoticeFlags.NOTICE_COMBINE):
		player.notice_flags.clear_bits(NoticeFlags.NOTICE_COMBINE)
		player.inventory.combine_pack()
	if player.notice_flags.is_set(NoticeFlags.NOTICE_REORDER):
		player.notice_flags.clear_bits(NoticeFlags.NOTICE_REORDER)
		player.inventory.reorder_pack()


func health_track(monster_index: int) -> void:
	tracked_monster_index = monster_index
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)


func process_player() -> void:
	# Special, if the player shoud start with a mutation then give them one here
	if player.get_first_level_mutation:
		Profile.instance.message_print("You feel different!")
		player.genome.gain_mutation()
		player.get_first_level_mutation = false
	# Give us some energy for the turn
	player.energy += Tables.EXTRACT_ENERGY[player.speed]
	# If we don't have enough to make a move, then return early
	if player.energy < 100:
		return
	User.wait_for_frame()
	# If we're resting for a special duration, check if we should stop
	if resting < 0:
		# -1 = rest until healed
		if resting == -1:
			if player.health >= player.max_health and player.power >= player.max_power:
				disturb(false)
		# -2 = rest until all durations have run out
		elif resting == -2:
			if (player.health >= player.max_health and player.power >= player.max_power and
				player.timed_blindness == 0 and player.timed_confusion == 0 and player.timed_poison == 0 and
				player.timed_fear == 0 and player.timed_stun == 0 and player.timed_bleeding == 0 and
				player.timed_slow == 0 and player.timed_paralysis == 0 and player.timed_hallucinations == 0 and
				player.word_of_recall_delay == 0 ):
				disturb(false)
	# If we're running or resting, allow us to cancel with a keypress
	if running != 0 or User.command_repeat != 0 or resting != 0:
		if User.get_key_no_wait() != "":
			disturb(false)
			Profile.instance.message_print("Cancelled.")
	# Keep trying to get a command until we've use up our energy
	while player.energy >= 100:
		# We may need to redraw our detect traps status
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_DTRAP)
		notice_update_redraw()
		# Overflow our pack if necessary
		if player.inventory.items[InventorySlot.PACK].item_type != null:
			var item: Item = player.inventory.items[InventorySlot.PACK]
			disturb(false)
			Profile.instance.message_print("Your pack overflows!")
			var item_name: String = item.get_description(true, 3)
			Profile.instance.message_print("You drop %s (%s)." % [item_name, Util.index_to_label(InventorySlot.PACK)])
			dungeon_level.dungeon_items.drop_near(item, 0, player.map_row, player.map_col)
			player.inventory.inven_item_increase(InventorySlot.PACK, -255)
			player.inventory.inven_item_describe(InventorySlot.PACK)
			player.inventory.inven_item_optimise(InventorySlot.PACK)
			notice_update_redraw()
		# If we have no queued command, we're not viewing the item list
		if User.queued_command == "":
			viewing_item_list = false
		# Assume our command will use no energy unless told otherwise
		energy_use = 0
		# If we're paralysed or stunned, we do nothing but use energy
		if player.timed_paralysis != 0 or player.timed_stun >= 100:
			energy_use = 100
		# If we're resting, then we do nothing but use energy
		elif resting != 0:
			if resting > 0:
				resting -= 1
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
			energy_use = 100
		# If we're running then we move one step
		elif running != 0:
			command_engine.run_one_step(0)
		# If we're repeating a previous command then we do it again
		elif User.command_repeat != 0:
			User.command_repeat -= 1
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
			redraw_stuff()
			Profile.instance.message_should_continue = false
			Console.clear_row(0)
			update_handler_objects()
			command_handler.process_command()
		# If all else fails, get a new command and do it
		else:
			User.get_command(false)
			update_handler_objects()
			command_handler.process_command()
		# If our command used energy, deal with that
		if energy_use != 0:
			# Remove the energy
			player.energy -= energy_use
			# Shimmer multi-hued monsters if we need to
			if dungeon_level.dungeon_monsters.shimmer_monsters:
				# Assume there are none left and we won't need to do this again
				dungeon_level.dungeon_monsters.shimmer_monsters = false
				for i: int in dungeon_level.monster_count_max:
					var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
					if monster.monster_type == null:
						continue
					var type: MonsterType = monster.monster_type
					if not type.flags_1.is_set(MonsterFlags1.ATTR_MULTI):
						continue
					# We found a multi-hued monster, so we will need to continue to do this
					dungeon_level.dungeon_monsters.shimmer_monsters = true
					# Re-draw the multi-hued monster
					dungeon_level.redraw_single_location(monster.map_row, monster.map_col)
			# If we need to repair the monster list then do so
			if dungeon_level.dungeon_monsters.repair_monsters:
				# Assume we won't need to do this again
				dungeon_level.dungeon_monsters.repair_monsters = false
				for i: int in dungeon_level.monster_count_max:
					var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
					if monster.monster_type == null:
						continue
					# Any monsters marked as nice are no longer nice
					if monster.individual_monster_flags.is_set(IndividualMonsterFlags.NICE):
						monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.NICE)
					# If a monster has MARK and SHOW, then remove the SHOW and note that we'll have to repair again
					if monster.individual_monster_flags.is_set(IndividualMonsterFlags.MARK):
						if monster.individual_monster_flags.is_set(IndividualMonsterFlags.SHOW):
							monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.SHOW)
							dungeon_level.dungeon_monsters.repair_monsters = true
						else:
							# The monster had MARK but not SHOW, so hide it again and lose the MARK
							monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.MARK)
							monster.is_visible = false
							dungeon_level.dungeon_monsters.update_monster_visibility(i, false)
							dungeon_level.redraw_single_location(monster.map_row, monster.map_col)
		# If we are finished playing the level then break out of our endless loop
		if not playing or player.is_dead or new_level_flag:
			break


func update_handler_objects() -> void:
	command_handler.player = player
	command_handler.dungeon_level = dungeon_level
	command_handler.savegame = self
	command_engine.player = player
	command_engine.dungeon_level = dungeon_level
	command_engine.savegame = self
	combat_engine.player = player
	combat_engine.dungeon_level = dungeon_level
	combat_engine.savegame = self
	spell_effects_handler.savegame = self
	spell_effects_handler.dungeon_level = dungeon_level
	spell_effects_handler.player = player


func command_save_game() -> void:
	if not is_auto_save:
		disturb(true)
	Profile.instance.message_print()
	notice_update_redraw()
	died_from = "(saved)"
	save_player()
	died_from = "(alive and well)"


func save_player() -> void:
	Profile.save()


func display_wild_map() -> Location:
	var location: Location = Location.new()
	location.row = player.wilderness_row + 2
	location.col = player.wilderness_col + 2
	var dungeon_guardians: Array[int]
	for i: int in dungeons.size():
		dungeon_guardians.append(0)
	for i: int in quests.quests.size():
		if quests.quests[i].is_active:
			dungeon_guardians[quests.quests[i].dungeon] += 1
	for row: int in 12:
		for col: int in 12:
			var wild_map_symbol: String = "^"
			var wild_map_ink: int = Ink.GREEN
			var region: WildernessRegion = island.get_region(row, col)
			if region.dungeon != null:
				var dungeon: Dungeon = region.dungeon
				wild_map_symbol = dungeon.map_symbol if dungeon.visited else "?"
				wild_map_ink = Ink.GREY if region.town != null else Ink.BROWN
				if dungeon_guardians[region.dungeon.index] != 0:
					wild_map_ink = Ink.BRIGHT_RED
			if row == 0 or col == 0 or row == 11 or col == 11:
				wild_map_symbol = "~"
				wild_map_ink = Ink.BLUE
			Console.print_at(row + 2, col + 2, wild_map_symbol, wild_map_ink)
	Console.print_at(1, 1, "+------------+", Ink.PURPLE)
	for row: int in 12:
		Console.print_at(row + 2, 1, "|", Ink.PURPLE)
		Console.print_at(row + 2, 14, "|", Ink.PURPLE)
	Console.print_at(14, 1, "+------------+", Ink.PURPLE)
	for i: int in dungeons.size():
		var depth: String = ("%d" % [dungeons[i].max_level]) if dungeons[i].known_depth else "?"
		var difficulty: String = ("%d" % [dungeons[i].offset]) if dungeons[i].known_offset else "?"
		var text: String
		if dungeons[i].visited:
			if i < towns.size():
				text = "%s = %s (L:%s, D:%s,Q:%d)" % [dungeons[i].map_symbol, towns[i].name, depth, difficulty, dungeon_guardians[i]]
			else:
				text = "%s = %s (L:%s, D:%s,Q:%d)" % [dungeons[i].map_symbol, dungeons[i].name, depth, difficulty, dungeon_guardians[i]]
		else:
			if i < towns.size():
				text = "? = %s (L:%s, D:%s,Q:%d)" % [towns[i].name, depth, difficulty, dungeon_guardians[i]]
			else:
				text = "? = %s (L:%s, D:%s,Q:%d)" % [dungeons[i].name, depth, difficulty, dungeon_guardians[i]]
		var key_ink: int = Ink.BROWN
		if i < towns.size():
			key_ink = Ink.GREY
		if dungeon_guardians[i] != 0:
			key_ink = Ink.BRIGHT_RED
		Console.print_at(i + 1, 19, text, key_ink)
	Console.print_at(16, 1, "L:Levels", Ink.PURPLE)
	Console.print_at(17, 1, "D:Difficulty", Ink.PURPLE)
	Console.print_at(18, 1, "Q:Quests", Ink.PURPLE)
	Console.print_at(dungeons.size() + 2, 19, "(Your position is marked with the cursor)", Ink.PURPLE)
	return location


## Process the world after each turn
func process_world() -> void:
	var flags_1: FlagSet = FlagSet.new()
	var flags_2: FlagSet = FlagSet.new()
	var flags_3: FlagSet = FlagSet.new()
	# We start with things that can happen any turn, rather than just at the end of a "player" turn
	# Check for some "easter egg" special times
	if player.game_time.is_birthday:
		Profile.instance.message_print("Happy Birthday!")
		dungeon_level.acquirement(player.map_row, player.map_col, Rng.roll_die(2) + 1, true)
		player.age += 1
	if player.game_time.is_new_year:
		Profile.instance.message_print("Happy New Year!")
		dungeon_level.acquirement(player.map_row, player.map_col, Rng.roll_die(2) + 1, true)
	if player.game_time.is_halloween:
		Profile.instance.message_print("All Hallows Eve and the ghouls come out to play...")
		dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, total_difficulty(), SummonType.SUMMON_UNDEAD)
	# Check for daybreak, and if so make the level light
	if current_depth <= 0:
		if player.game_time.is_dawn:
			Profile.instance.message_print("The sun has risen.")
			for row: int in dungeon_level.cur_hgt:
				for col: int in dungeon_level.cur_wid:
					var tile: GridTile = dungeon_level.get_grid(row, col)
					tile.tile_flags.set_bits(GridTile.SELF_LIT)
					tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
					dungeon_level.note_spot(row, col)
			player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
		# Check for nightfall, and if so make the level dark
		elif player.game_time.is_dusk:
			Profile.instance.message_print("The sun has fallen.")
			for row: int in dungeon_level.cur_hgt:
				for col: int in dungeon_level.cur_wid:
					var tile: GridTile = dungeon_level.get_grid(row, col)
					if tile.feature_type.IsOpenFloor:
						tile.tile_flags.clear(GridTile.SELF_LIT)
						dungeon_level.note_spot(row, col)
			player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
			player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	# Check for midnight, and if so do daily updates
	if player.game_time.is_midnight:
		# Religious favour decays each day
		player.religion.decay_favour()
		player.updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH | UpdateFlags.UPDATE_POWER)
		# Shops reset their stock at midnight
		for town: Town in towns:
			for store: Store in town.stores:
				store.store_maint()
		# Chance of a new owner for a random store
		if Rng.roll_die(Store.STORE_SHUFFLE) == 1:
			var town: int = Rng.roll_die(towns.size()) - 1
			var store: int = Rng.roll_die(12) - 1
			towns[town].stores[store].store_shuffle()
	# If we're not on a tenth turn (i.e., a player turn) we're done
	if not player.game_time.is_turn_ten:
		return
	# From here on are things that only happen after a player turn
	# Small chance of spawning a random monster outside the player's sight radius
	if Rng.roll_die(DungeonMonsters.RANDOM_ENCOUNTER_CHANCE) == 1:
		dungeon_level.dungeon_monsters.spawn_monster(dungeon_level.MAX_SIGHT_RADIUS + 5, false)
	# Every hundred turns (ten player turns), monsters regenerate
	if player.game_time.is_turn_hundred:
		regen_monsters()
	# If we're poisoned, lose a hit point
	if player.timed_poison != 0 and player.timed_invulnerability == 0:
		player.take_hit(1, "poison")
	# Assume we are able to regenerate unless told otherwise
	var cant_regenerate: bool = false
	# If we're sunlight sensitive and we're outside or are using a star essence, we get scorched and can't regenerate
	if player.ancestry.is_sunlight_sensitive:
		if current_depth <= 0 and not player.has_light_resistance and player.timed_invulnerability == 0 and player.game_time.is_light():
			if dungeon_level.get_grid(player.map_row, player.map_col).tile_flags.is_set(GridTile.SELF_LIT):
				Profile.instance.message_print("The sun's rays scorch your flesh!")
				player.take_hit(1, "sunlight")
				cant_regenerate = true
		if (player.inventory.items[InventorySlot.LIGHT_SOURCE].category != 0 and
			player.inventory.items[InventorySlot.LIGHT_SOURCE].sub_category >= LightType.POLARIS and
			not player.has_light_resistance ):
			var item: Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
			var item_name: String = item.description(false, 0)
			Profile.instance.message_print("The %s scorches your flesh!" % [item_name])
			cant_regenerate = true
			item_name = item.description(true, 0)
			var ouch: String = "wielding %s" % [item_name]
			if player.timed_invulnerability == 0:
				player.take_hit(1, ouch)
	# If we're walking through walls we can't regenerate and might get squished
	if not dungeon_level.grid_passable(player.map_row, player.map_col):
		cant_regenerate = true
		if (player.timed_invulnerability == 0 and player.timed_etherealness == 0 and
			(player.health > player.level / 5 or not player.ancestry.is_incorporeal) ):
			var ouch: String
			if player.ancestry.is_incorporeal:
				Profile.instance.message_print("Your body feels disrupted!")
				ouch = "density"
			else:
				Profile.instance.message_print("You are being crushed!")
				ouch = "solid rock"
			player.take_hit(1 + (player.level / 5), ouch)
	# If we're bleeding we take a variable amount of damage (but this doesn't stop regen)
	if player.timed_bleeding != 0 and player.timed_invulnerability == 0:
		var bleed: int
		if player.timed_bleeding > 200:
			bleed = 3
		elif player.timed_bleeding > 100:
			bleed = 2
		else:
			bleed = 1
		player.take_hit(bleed, "a fatal wound")
	# We're using up food
	if player.food < Constants.FOOD_GORGED:
		if player.game_time.is_turn_hundred:
			# Base food use is dependent on speed
			var food_used: int = Tables.EXTRACT_ENERGY[player.speed] * 2
			# Regeneration makes us hungry
			if player.has_regeneration:
				food_used += 30
			# Slow digestion helps
			if player.has_slow_digestion:
				food_used -= 10
			if food_used < 1:
				food_used = 1
			player.set_food(player.food - food_used)
	else:
		# Recovering form being gorged is quicker
		player.set_food(player.food - 100)
	# If we're starving it hurts
	if player.food < Constants.FOOD_STARVE:
		var ouch: int = (Constants.FOOD_STARVE - player.food) / 10
		if player.timed_invulnerability == 0:
			player.take_hit(ouch, "starvation")
	# Work out how much we regenerate
	var regen_amount: int = Constants.PLAYER_REGEN_NORMAL
	# We regenerate less if hungry
	if player.food < Constants.FOOD_WEAK:
		if player.food < Constants.FOOD_STARVE:
			regen_amount = 0
		elif player.food < Constants.FOOD_FAINT:
			regen_amount = Constants.PLAYER_REGEN_FAINT
		else:
			regen_amount = Constants.PLAYER_REGEN_WEAK
		# We have a chance of fainting if we're extremely hungry
		if player.food < Constants.FOOD_FAINT:
			if player.timed_paralysis == 0 and Rng.roll_die(100) <= 10:
				Profile.instance.message_print("You faint from the lack of food.")
				disturb(true)
				player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(5))
	# Having regeneration means we regenerate twice as quickly
	if player.has_regeneration:
		regen_amount *= 2
	# Searching or resting means we regenerate twice as quickly
	if player.is_searching or resting != 0:
		regen_amount *= 2
	# Power regeneration might be slowed by having pets
	var upkeep_factor: int = 0
	if total_friends != 0:
		var upkeep_divider: int = player.profession.pet_upkeep_divider
		if total_friends > 1 + (player.level / upkeep_divider):
			upkeep_factor = total_friend_levels
			upkeep_factor = clampi(upkeep_factor, 10, 90)
	if player.power < player.max_power:
		if upkeep_factor != 0:
			var upkeep_regen: int = (100 - upkeep_factor) * regen_amount / 100
			player.regenerate_power(upkeep_regen)
		else:
			player.regenerate_power(regen_amount)
	# If we're poisoned, we don't regenerate
	if player.timed_poison != 0:
		regen_amount = 0
	# If we're bleeding, we don't regenerate
	if player.timed_bleeding != 0:
		regen_amount = 0
	# If we were flagged for such earlier, we don't regenerate
	if cant_regenerate:
		regen_amount = 0
	if player.health < player.max_health and not cant_regenerate:
		player.regenerate_health(regen_amount)
	# If our patron was prevented from giving us multiple rewards in the same turn, that no longer applies
	if player.goo_patron.multiple_rewards:
		player.goo_patron.multiple_rewards = false
	# Reduce our various timed conditions
	if player.timed_hallucinations != 0:
		player.set_timed_hallucinations(player.timed_hallucinations - 1)
	if player.timed_blindness != 0:
		player.set_timed_blindness(player.timed_blindness - 1)
	if player.timed_see_invisibility != 0:
		player.set_timed_see_invisibility(player.timed_see_invisibility - 1)
	if player.timed_telepathy != 0:
		player.set_timed_telepathy(player.timed_telepathy - 1)
	if player.timed_infravision != 0:
		player.set_timed_infravision(player.timed_infravision - 1)
	if player.timed_paralysis != 0:
		player.set_timed_paralysis(player.timed_paralysis - 1)
	if player.timed_confusion != 0:
		player.set_timed_confusion(player.timed_confusion - 1)
	if player.timed_fear != 0:
		player.set_timed_fear(player.timed_fear - 1)
	if player.timed_haste != 0:
		player.set_timed_haste(player.timed_haste - 1)
	if player.timed_slow != 0:
		player.set_timed_slow(player.timed_slow - 1)
	if player.timed_protection_from_evil != 0:
		player.set_timed_protection_from_evil(player.timed_protection_from_evil - 1)
	if player.timed_invulnerability != 0:
		player.set_timed_invulnerability(player.timed_invulnerability - 1)
	if player.timed_etherealness != 0:
		player.set_timed_etherealness(player.timed_etherealness - 1)
	if player.timed_heroism != 0:
		player.set_timed_heroism(player.timed_heroism - 1)
	if player.timed_super_heroism != 0:
		player.set_timed_superheroism(player.timed_super_heroism - 1)
	if player.timed_blessing != 0:
		player.set_timed_blessing(player.timed_blessing - 1)
	if player.timed_stoneskin != 0:
		player.set_timed_stoneskin(player.timed_stoneskin - 1)
	if player.timed_acid_resistance != 0:
		player.set_timed_acid_resistance(player.timed_acid_resistance - 1)
	if player.timed_lightning_resistance != 0:
		player.set_timed_lightning_resistance(player.timed_lightning_resistance - 1)
	if player.timed_fire_resistance != 0:
		player.set_timed_fire_resistance(player.timed_fire_resistance - 1)
	if player.timed_cold_resistance != 0:
		player.set_timed_cold_resistance(player.timed_cold_resistance - 1)
	if player.timed_poison_resistance != 0:
		player.set_timed_poison_resistance(player.timed_poison_resistance - 1)
	if player.timed_poison != 0:
		var adjust: int = player.ability_scores[Ability.CONSTITUTION].con_recovery_speed() + 1
		player.set_timed_poison(player.timed_poison - adjust)
	if player.timed_stun != 0:
		var adjust: int = player.ability_scores[Ability.CONSTITUTION].con_recovery_speed() + 1
		player.set_timed_stun(player.timed_stun - adjust)
	if player.timed_bleeding != 0:
		var adjust: int = player.ability_scores[Ability.CONSTITUTION].con_recovery_speed() + 1
		if player.timed_bleeding > 1000:
			adjust = 0
		player.set_timed_bleeding(player.timed_bleeding - adjust)
	# Dim the player's light source
	var light: Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	if light.category == ItemCategory.LIGHT:
		if (light.sub_category == LightType.TORCH or light.sub_category == LightType.LANTERN) and light.type_specific_value > 0:
			light.type_specific_value -= 1
			# If we're blind, keep the light source going until we can notice it going out
			if player.timed_blindness != 0:
				if light.type_specific_value == 0:
					light.type_specific_value += 1
			elif light.type_specific_value == 0:
				disturb(true)
				Profile.instance.message_print("Your light has gone out!")
			elif light.type_specific_value < 100 and light.type_specific_value % 10 == 0 and player.timed_blindness == 0:
				Profile.instance.message_print("Your light is growing faint.")
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
	# Handle experience drain
	if player.has_experience_drain:
		if Rng.roll_die(100) <= 10 and player.experience_points > 0:
			player.experience_points -= 1
			player.max_experience_gained -= 1
			player.check_experience()
	# Handle Shining Trapezohedron occasionally draining life
	if Rng.roll_die(999) == 1 and not player.has_anti_magic:
		if (player.inventory.items[InventorySlot.LIGHT_SOURCE].category != 0 and player.timed_invulnerability == 0 and
			player.inventory.items[InventorySlot.LIGHT_SOURCE].sub_category == LightType.TRAPEZOHEDRON ):
			Profile.instance.message_print("The Shining Trapezohedron drains life from you!")
			player.take_hit(min(player.level, 50), "the Shining Trapezohedron")
	# Check for any of the player's random mutations going off
	player.genome.on_process_world(self, player, dungeon_level)
	# Check for any wielded items that have the dread curse or a teleport curse
	for i: int in range(InventorySlot.MELEE_WEAPON, InventorySlot.TOTAL):
		var item: Item = player.inventory.items[i]
		item.get_merged_flags(flags_1, flags_2, flags_3)
		if flags_3.is_set(ItemFlags3.DREAD_CURSE) and Rng.roll_die(100) == 1:
			activate_dread_curse()
		if flags_3.is_set(ItemFlags3.TELEPORT) and Rng.roll_die(100) == 1:
			if item.identify_flags.is_set(IdentifyFlags.CURSED) and not player.has_anti_teleport:
				disturb(true)
				spell_effects_handler.teleport_player(40)
			else:
				if User.get_check("Teleport? "):
					disturb(false)
					spell_effects_handler.teleport_player(50)
		# Also check for artifacts and legendary items recharging while we're here
		# Note that unlike rods, these only recharge while you're wielding them
		if item.item_type == null:
			continue
		if item.recharge_time_left > 0:
			item.recharge_time_left -= 1
	# Check for rods in your pack recharging
	var possible_item_combine: bool = false
	for i: int in InventorySlot.PACK:
		var item: Item = player.inventory.items[i]
		if item.item_type == null:
			continue
		if item.category == ItemCategory.ROD and item.type_specific_value != 0:
			item.type_specific_value -= 1
			if item.type_specific_value == 0:
				possible_item_combine = true
	# If a rod became recharged, it might combine with another recharged rod of the same type
	if possible_item_combine:
		player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE)
	# We might notice something about one of our items
	player.sense_inventory()
	# Rods that have been dropped on the floor should still recharge
	for i: int in range(1, dungeon_level.item_count_max):
		var item: Item = dungeon_level.dungeon_items.items[i]
		if item.item_type == null:
			continue
		if item.category == ItemCategory.ROD and item.type_specific_value != 0:
			item.type_specific_value -= 1
	# If the player is waiting for a Word of Recall, decrease the wait
	if player.word_of_recall_delay != 0:
		player.word_of_recall_delay -= 1
		# If our WoR wait reaches zero, yoink us out of (or into) the dungeon
		if player.word_of_recall_delay == 0:
			disturb(false)
			if current_depth != 0:
				Profile.instance.message_print("You feel yourself yanked downwards!" if current_dungeon.is_tower else "You feel yourself yanked upwards!")
				is_auto_save = true
				command_save_game()
				is_auto_save = false
				recall_dungeon = current_dungeon.index
				current_depth = 0
				# If we have a house, Word of Recall always takes us there
				if player.town_with_house > -1:
					current_town = towns[player.town_with_house]
					player.wilderness_row = current_town.wild_row
					player.wilderness_col = current_town.wild_col
					new_level_flag = true
					came_from = LevelStart.START_HOUSE
				else:
					player.wilderness_row = current_town.wild_row
					player.wilderness_col = current_town.wild_col
					new_level_flag = true
					came_from = LevelStart.START_RANDOM
			else:
				Profile.instance.message_print("You feel yourself yanked upwards!" if dungeons[recall_dungeon].is_tower else "You feel yourself yanked downwards!")
				is_auto_save = true
				command_save_game()
				is_auto_save = false
				current_dungeon = dungeons[recall_dungeon]
				player.wilderness_row = current_dungeon.wild_row
				player.wilderness_col = current_dungeon.wild_col
				current_depth = player.max_dlv[current_dungeon.index]
				if current_depth < 1:
					current_depth = 1
				new_level_flag = true


func regen_monsters() -> void:
	for i: int in dungeon_level.monster_count_max:
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[i]
		var type: MonsterType = monster.monster_type
		if type == null:
			continue
		if monster.health < monster.max_health:
			var frac: int = monster.max_health / 100
			if frac == 0:
				frac = 1
			if type.flags_2.is_set(MonsterFlags2.REGENERATE):
				frac *= 2
			monster.health += frac
			if monster.health > monster.max_health:
				monster.health = monster.max_health
			if tracked_monster_index == i:
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)


func activate_dread_curse() -> void:
	var do_it_skywalker: bool = true
	while do_it_skywalker:
		match Rng.roll_die(27):
			1, 2, 3, 16, 17:
				spell_effects_handler.aggravate_monsters(1)
			4, 5, 6:
				spell_effects_handler.activate_hi_summon()
			7, 8, 9, 18:
				dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, total_difficulty(), 0)
			10, 11, 12:
				Profile.instance.message_print("You feel your life draining away...")
				player.lose_experience(player.experience_points / 16)
			13, 14, 15, 19, 20:
				if not player.has_free_action or Rng.roll_die(100) >= player.skill_saving_throw:
					Profile.instance.message_print("You feel like a statue!")
					if player.has_free_action:
						player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(3))
					else:
						player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(13))
			21, 22, 23:
				player.try_decreasing_ability_score(Rng.roll_die(6) - 1)
			24:
				Profile.instance.message_print("Huh? Who am I? What am I doing here?")
				spell_effects_handler.lose_all_info()
			25:
				spell_effects_handler.summon_reaver()
			_:
				for i: int in 6:
					while Rng.roll_die(2) == 1:
						player.try_decreasing_ability_score(i)
		do_it_skywalker = (Rng.roll_die(3) == 1)


func get_item(prompt: String, can_choose_equip: bool, can_choose_inven: bool, can_choose_floor: bool) -> int:
	var tile: GridTile = dungeon_level.get_grid(player.map_row, player.map_col)
	var inventory: Inventory = player.inventory
	var current_item_index: int
	var allow_floor: bool = false
	Profile.instance.message_print()
	var done: bool = false
	var item_index: int = -1
	var i_1: int = 0
	var i_2: int = InventorySlot.PACK - 1
	if not can_choose_inven:
		i_2 = -1
	while i_1 <= i_2 and !inventory.get_item_okay(i_1):
		i_1 += 1
	while i_1 <= i_2 and !inventory.get_item_okay(i_2):
		i_2 -= 1
	var e_1: int= InventorySlot.MELEE_WEAPON
	var e_2: int = InventorySlot.TOTAL - 1
	if not can_choose_equip:
		e_2 = -1
	while e_1 <= e_2 and !inventory.get_item_okay(e_1):
		e_1 += 1
	while e_1 <= e_2 and !inventory.get_item_okay(e_2):
		e_2 -= 1
	if can_choose_floor:
		current_item_index = tile.item_index
		while current_item_index != 0:
			var item: Item = dungeon_level.dungeon_items.items[current_item_index]
			current_item_index = item.next_in_stack
			if inventory.item_matches_filter(item):
				allow_floor = true
	if not allow_floor and i_1 > i_2 and e_1 > e_2:
		viewing_item_list = false
		item_index = -2
		done = true
	else:
		if not viewing_item_list:
			item_display_column = 50
		if viewing_item_list and viewing_equipment and can_choose_equip:
			viewing_equipment = true
		elif can_choose_inven:
			viewing_equipment = false
		elif can_choose_equip:
			viewing_equipment = true
		else:
			viewing_equipment = false
	if viewing_item_list:
		Console.save_screen()
	while not done:
		if not viewing_equipment:
			if viewing_item_list:
				player.inventory.show_inventory()
		else:
			if viewing_item_list:
				player.inventory.show_equipment()
		var partial_text: String
		var text: String
		if not viewing_equipment:
			text = "Inven:"
			if i_1 <= i_2:
				partial_text = " %s-%s," % [Util.index_to_label(i_1), Util.index_to_label(i_2)]
				text += partial_text
			if not viewing_item_list:
				text += " * to see,"
			if can_choose_equip:
				text += " / for Equip,"
		else:
			text = "Equip:"
			if e_1 <= e_2:
				partial_text = " %s-%s" % [Util.index_to_label(e_1), Util.index_to_label(e_2)]
				text += partial_text
			if not viewing_item_list:
				text += " * to see,"
			if can_choose_inven:
				text += " / for Inven,"
		if allow_floor:
			text += " - for floor,"
		text += " ESC"
		partial_text = "(%s) %s" % [text, prompt]
		Console.print_line_at(0, 0, partial_text, Ink.WHITE)
		var which: String = User.get_key()
		match which:
			"Escape":
				item_display_column = 50
				item_index = 99
				done = true
			"*", "?", " ":
				if not viewing_item_list:
					Console.save_screen()
					viewing_item_list = true
				else:
					Console.restore_screen()
					viewing_item_list = false
			"/":
				if can_choose_inven and can_choose_equip:
					if viewing_item_list:
						Console.restore_screen()
						Console.save_screen()
					viewing_equipment = not viewing_equipment
			"-":
				if allow_floor:
					current_item_index = tile.item_index
					while current_item_index != 0:
						var item: Item = dungeon_level.dungeon_items.items[current_item_index]
						current_item_index = item.next_in_stack
						if not inventory.item_matches_filter(item):
							continue
						item_index = 0 - current_item_index
						done = true
						break
			"Enter":
				var k: int
				if not viewing_equipment:
					k = i_1 if i_1 == i_2 else -1
				else:
					k = e_1 if e_1 == e_2 else -1
				if inventory.get_item_okay(k):
					item_index = k
					done = true
			_:
				var k: int = player.inventory.label_to_inven(which) if not viewing_equipment else player.inventory.label_to_equip(which)
				if inventory.get_item_okay(k):
					item_index = k
					done = true
	if viewing_item_list:
		Console.restore_screen()
	viewing_item_list = false
	Inventory.item_filter_category = 0
	item_filter_function = Callable()
	Console.clear_row(0)
	return item_index


func print_tomb(corpse: Player) -> void:
	var ct: String = GameTime.get_current_datetime()
	if corpse.is_winner:
		Console.set_background(BackgroundImage.SUNSET)
		Console.change_music("Victory")
	else:
		Console.set_background(BackgroundImage.TOMB)
		Console.change_music("Death")
	Console.clear_screen()
	var text: String = corpse.name + Util.to_roman(corpse.generation, true)
	if corpse.is_winner or corpse.level > Constants.MAX_EXPERIENCE_LEVEL:
		text += " the Magnificent"
	Console.print_at(39, 1, text, Ink.WHITE)
	text = "Level %d %s" % [corpse.level, corpse.profession.get_profession_sub_name(corpse.realm_1)]
	Console.print_at(40, 1, text, Ink.WHITE)
	text = ("Killed on Level %d" % [current_depth]).lpad(45)
	Console.print_at(39, 34, text, Ink.WHITE)
	text = ("by %s" % [died_from]).lpad(45)
	Console.print_at(40, 34, text, Ink.WHITE)
	text = ("on %s" % [ct]).lpad(45)
	Console.print_at(41, 34, text, Ink.WHITE)
	Console.any_key(44)


func monster_death(monster_index: int) -> void:
	var dropped_item: int = 0
	var dropped_gold: int = 0
	var number: int = 0
	var quest_index: int = 0
	var quest: bool = false
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	if monster_type == null:
		return
	var visible: bool = monster.is_visible or monster_type.flags_1.is_set(MonsterFlags1.UNIQUE)
	var good: bool = monster_type.flags_1.is_set(MonsterFlags1.DROP_GOOD)
	var great: bool = monster_type.flags_1.is_set(MonsterFlags1.DROP_GREAT)
	var do_gold: bool = monster_type.flags_1.is_clear(MonsterFlags1.ONLY_DROP_ITEM)
	var do_item: bool = monster_type.flags_1.is_clear(MonsterFlags1.ONLY_DROP_GOLD)
	var cloned: bool = false
	var forced_coin_type: int = monster_type.get_coin_type()
	var item: Item
	var row: int = monster.map_row
	var col: int = monster.map_col
	if monster.mind.is_set(MonsterMind.CLONED):
		cloned = true
	var item_index: int = monster.first_held_item_index
	while item_index != 0:
		var held_item: Item = dungeon_level.dungeon_items.items[item_index]
		held_item.holding_monster_index = 0
		item = held_item.get_copy()
		dungeon_level.delete_item_by_index(item_index)
		dungeon_level.dungeon_items.drop_near(item, -1, row, col)
		item_index = item.next_in_stack
	if monster.stolen_gold > 0:
		var gold_item: Item = Item.new()
		Item.forced_coin_type = 10
		gold_item.make_gold()
		Item.forced_coin_type = 0
		gold_item.type_specific_value = monster.stolen_gold
		dungeon_level.dungeon_items.drop_near(gold_item, -1, row, col)
	monster.first_held_item_index = 0
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_60) and Rng.roll_die(100) <= 60:
		number += 1
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_90) and Rng.roll_die(100) <= 90:
		number += 1
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_1D2):
		number += Rng.roll_dice(1, 2)
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_2D2):
		number += Rng.roll_dice(2, 2)
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_3D2):
		number += Rng.roll_dice(3, 2)
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_4D2):
		number += Rng.roll_dice(4, 2)
	if cloned:
		number = 0
	if quests.is_quest_level(current_depth) and monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		quest_index = quests.get_quest_number()
		quests.quests[quest_index].killed += 1
		if quests.quests[quest_index].killed == quests.quests[quest_index].to_kill:
			great = true
			good = true
			do_gold = false
			number += 2
			quest = true
			quests.quest[quest_index].level = 0
	Item.forced_coin_type = forced_coin_type
	dungeon_level.item_level = (total_difficulty() + monster_type.level) / 2
	for j: int in number:
		item = Item.new()
		if do_gold and (not do_item or Rng.roll_die(100) <= 50):
			if not item.make_gold():
				continue
			dropped_gold += 1
		else:
			if not quest or j > 1:
				if not item.make_item(good, great):
					continue
			else:
				if not item.make_item(true, true):
					continue
			dropped_item += 1
		dungeon_level.dungeon_items.drop_near(item, -1, row, col)
	dungeon_level.item_level = total_difficulty()
	Item.forced_coin_type = 0
	if visible and (dropped_item != 0 or dropped_gold != 0):
		dungeon_level.dungeon_monsters.lore_treasure(monster_index, dropped_item, dropped_gold)
	if monster_type.flags_1.is_clear(MonsterFlags1.GUARDIAN):
		return
	if quests.quests[quest_index].killed != quests.quests[quest_index].to_kill:
		return
	monster_type.flags_1.clear_bits(MonsterFlags1.GUARDIAN)
	if quests.get_active_quest_count() == 0:
		player.is_winner = true
		player.redraw_needed.set_bits(RedrawFlags.REDRAW_TITLE)
		Profile.instance.message_print("*** CONGRATULATIONS ***")
		Profile.instance.message_print("You have won the game!")
		Profile.instance.message_print("You may retire ('Q') when you are ready.")
	if current_depth < current_dungeon.max_level:
		while not dungeon_level.cave_valid_bold(row, col):
			var stair_location: Location = dungeon_level.scatter(row, col, 1)
			row = stair_location.row
			col = stair_location.col
		dungeon_level.delete_object(row, col)
		Profile.instance.message_print("A magical stairway appears...")
		dungeon_level.cave_set_feat(row, col, "UpStair" if current_dungeon.is_tower else "DownStair")
		player.UpdatesNeeded.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT | UpdateFlags.UPDATE_MONSTERS)


func get_which_store() -> Store:
	for store: Store in current_town.stores:
		if player.map_row == store.door_row and player.map_col == store.door_col:
			return store
	return null


func extract_resources()->void:
	quests = QuestList.new()
	item_types = ItemTypeArray.new()
	for item_type: ItemType in StaticData.item_types:
		item_types.types.append(item_type.get_copy_for_new_savegame())
	for artifact: Artifact in StaticData.artifact_types:
		artifacts.get_or_add(artifact.id, artifact.get_copy_for_new_savegame())
	
