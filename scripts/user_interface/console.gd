# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Console
extends Node2D

const CONSOLE_WIDTH: int = 80
const CONSOLE_HEIGHT: int = 45

## The global instance that can be used by other classes
static var instance: Console
## Set this to quit the game
static var exit_to_desktop: bool
## Flag to say if we have a full screen overlay, and therefore the display of various things shoudl be suppressed
static var full_screen_overlay: bool
static var requires_settings_update: bool
static var requires_refresh: bool
static var start_animation_timer: bool
static var high_score_table: HighScoreTable = HighScoreTable.load_or_create()

## The save slot that's currently active
var active_save_slot: int
## The scene to use for cells
var _cell_scene: PackedScene = preload("res://scenes/display_cell.tscn")
## The array of cells to use
var _cells: Array[DisplayCell] = []
## The array of cells in the saved screen
var _saved_cells: Array[SavedDisplayCell] = []
## The row of the cursor
var _cursor_row: int = 0
## The column of the cursor
var _cursor_col: int = 0
## The default ink colour to print in
var _ink: Color = Color.WHITE
## The mapping of ink colours to actual colours
var _colours: Dictionary[int, Color] = {
	Ink.BACKGROUND: Color(0x000000ff),
	Ink.BLACK: Color(0x2c4a4aff),
	Ink.GREY: Color(0x696969ff),
	Ink.BRIGHT_GREY: Color(0xa9a9a9ff),
	Ink.WHITE: Color(0xd3d3d3ff),
	Ink.BRIGHT_WHITE: Color(0xffffffff),
	Ink.RED: Color(0x8b0000ff),
	Ink.BRIGHT_RED: Color(0xff0000ff),
	Ink.ORANGE: Color(0xff6f00ff),
	Ink.BRIGHT_ORANGE: Color(0xffa500ff),
	Ink.YELLOW: Color(0xbdbd00ff),
	Ink.BRIGHT_YELLOW: Color(0xffff00ff),
	Ink.CHARTREUSE: Color(0x759725ff),
	Ink.BRIGHT_CHARTREUSE: Color(0x7fff00ff),
	Ink.GREEN: Color(0x006400ff),
	Ink.BRIGHT_GREEN: Color(0x31cc34ff),
	Ink.TURQUOISE: Color(0x00968aff),
	Ink.BRIGHT_TURQUOISE: Color(0x00ffffff),
	Ink.BLUE: Color(0x001da6ff),
	Ink.BRIGHT_BLUE: Color(0x0671ffff),
	Ink.PURPLE: Color(0x800080ff),
	Ink.BRIGHT_PURPLE: Color(0xff00ffff),
	Ink.PINK: Color(0xff5ea9ff),
	Ink.BRIGHT_PINK: Color(0xffa3d5ff),
	Ink.BROWN: Color(0x8b4513ff),
	Ink.BRIGHT_BROWN: Color(0xd47720ff),
	Ink.BEIGE: Color(0xafae80ff),
	Ink.BRIGHT_BEIGE: Color(0xffffe0ff),
	Ink.COPPER: Color(0xff8c00ff),
	Ink.SILVER: Color(0xc0c0c0ff),
	Ink.GOLD: Color(0xffd700ff),
	Ink.DIAMOND: Color(0xf0f8ffff),
}
## Our sounds
static var _sounds: Dictionary[String, Sound] = {
	"AttackHit": preload("res://sfx/resources/attack_hit.tres"),
	"AttackMiss": preload("res://sfx/resources/attack_miss.tres"),
	"CloseDoor": preload("res://sfx/resources/close_door.tres"),
	"EnterShop": preload("res://sfx/resources/enter_shop.tres"),
	"HealthWarning": preload("res://sfx/resources/health_warning.tres"),
	"LevelUp": preload("res://sfx/resources/level_up.tres"),
	"LockedDoor": preload("res://sfx/resources/locked_door.tres"),
	"MenuAccept": preload("res://sfx/resources/menu_accept.tres"),
	"MenuSelect": preload("res://sfx/resources/menu_select.tres"),
	"MonsterDeath": preload("res://sfx/resources/mon_death.tres"),
	"OpenDoor": preload("res://sfx/resources/open_door.tres"),
	"PlayerDeath": preload("res://sfx/resources/player_death.tres"),
	"ShopTransaction": preload("res://sfx/resources/shop_transaction.tres"),
}

static var _sound_players: Dictionary[String, AudioStreamPlayer]

## Our background picture
@onready var _background_image: BackgroundImage = $BackgroundImage
## Our cursor
@onready var _cursor: ColorRect = $Cursor
## The help window
@onready var _cthangpedia: Cthangpedia = $Cthangpedia
## The music player
@onready var _music_player: AudioStreamPlayer = $MusicPlayer
## The sound player
@onready var _master_sound_player: AudioStreamPlayer = $SoundPlayer
## The animation timer
@onready var _animation_timer: Timer = $AnimationTimer
## The title
@onready var _main_title: MainTitle = $MainTitle
## The CRT effect overlay
@onready var _shader_overlay: CanvasLayer = $ShaderOverlay
## The mask for showing maps and the like
@onready var _mask: ColorRect = $Mask

## Move the cursor to a specific location, then print a string there in a custom colour, advancing the cursor
static func print_at(row: int, col: int, text: String, colour: int) -> void:
	instance._go_to(row, col)
	instance._ink = instance._colours[colour]
	for c: String in text:
		instance._ink_character(c)


static func print_line_at(row: int, col: int, text: String, colour: int) -> void:
	clear_to_row_end(row, col)
	print_at(row, col, text, colour)


static func print_following(text: String, colour: int) -> void:
	instance._ink = instance._colours[colour]
	for c: String in text:
		instance._ink_character(c)


static func print_wrap(row: int, col: int, text: String, colour: int) -> void:
	instance._print_wrap(row, col, text, colour)


## Move the cursor to the desired row, and then print text centered on that row in a custom colour, advancing the cursor
static func print_centred(row: int, text: String, colour: int) -> void:
	var spaces_needed: int = CONSOLE_WIDTH - text.length()
	var left_pad: int = spaces_needed / 2 + text.length()
	text = text.lpad(left_pad)
	text = text.rpad(CONSOLE_WIDTH)
	print_at(row, 0, text, colour)


static func print_visual_at(row: int, col: int, visual: Visual) -> void:
	print_at(row, col, visual.character, visual.ink)


## Clear the screen
static func clear_screen() -> void:
	for cell: DisplayCell in instance._cells:
		cell.set_both("", Ink.WHITE)
	instance._go_to(0, 0)


## Clear a single line
static func clear_row(row: int) -> void:
	for col: int in CONSOLE_WIDTH:
		instance._cells[row * CONSOLE_WIDTH + col].set_both("", Ink.WHITE)


## Clear a partial line
static func clear_to_row_end(row: int, col: int) -> void:
	for clear_col: int in range(col, CONSOLE_WIDTH):
		instance._cells[row * CONSOLE_WIDTH + clear_col].set_both("", Ink.WHITE)


## Clear a partial screen
static func clear_from(from_row: int) -> void:
	for row: int in range(from_row, CONSOLE_HEIGHT):
		clear_row(row)


## Save the screen
static func save_screen() -> void:
	for i: int in CONSOLE_WIDTH * CONSOLE_HEIGHT:
		instance._saved_cells[i].character = instance._cells[i].character
		instance._saved_cells[i].colour = instance._cells[i].colour


## Restore the screen
static func restore_screen() -> void:
	for i: int in CONSOLE_WIDTH * CONSOLE_HEIGHT:
		instance._cells[i].set_both(
			instance._saved_cells[i].character,
			instance._saved_cells[i].colour,
		)


## Set the background
static func set_background(background: int) -> void:
	instance._background_image.set_background(background)
	if background == BackgroundImage.MENU:
		instance._main_title.call_deferred("show")
	else:
		instance._main_title.call_deferred("hide")


static func any_key(row: int) -> void:
	print_centred(row, "[Press any key to continue]", Ink.ORANGE)
	User.get_key()
	clear_row(row)


## Show the cursor in a given location
static func show_cursor(row: int, col: int) -> void:
	instance._cursor.set_deferred("position", Vector2i(col * 24, row * 24))
	instance._cursor.set_deferred("visible", true)


## Hide the cursor
static func hide_cursor() -> void:
	instance._cursor.set_deferred("visible", false)


## Show the help window
static func show_cthangpedia() -> void:
	instance._cthangpedia.set_deferred("visible", true)


static func change_music(track_name: String) -> void:
	var player: AudioStreamPlaybackInteractive = instance._music_player.get_stream_playback()
	var stream: AudioStreamInteractive = instance._music_player.stream as AudioStreamInteractive
	var current: String = stream.get_clip_name(player.get_current_clip_index())
	if track_name != current:
		player.switch_to_clip_by_name(track_name)


static func play_sound(track_name: String) -> void:
	if instance._master_sound_player.volume_linear == 0:
		return
	var player: AudioStreamPlayer = _sound_players[track_name]
	if player.playing:
		player.stop()
	var sound: Sound = _sounds[track_name]
	if sound.randomise:
		player.pitch_scale = 1.0 + ((Rng.roll_die(9)-2)/15.0)
	else:
		player.pitch_scale = 1.0
	player.volume_db = instance._master_sound_player.volume_db
	player.call_deferred("set_stream", sound.stream)
	player.call_deferred("play")

static func set_music_volume(volume: int) -> void:
	instance._music_player.volume_linear = volume * Constants.MUSIC_VOLUME_SCALE


static func set_sound_volume(volume: int) -> void:
	pass
	instance._master_sound_player.volume_linear = volume * Constants.SFX_VOLUME_SCALE


## Initialise the console when ready
func _ready() -> void:
	# Hide the mouse cursor
	Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN)
	# Stop the window being closed
	get_tree().set_auto_accept_quit(false)
	instance = self
	# Set up the display cells
	for i: int in CONSOLE_WIDTH * CONSOLE_HEIGHT:
		var cell: DisplayCell = _cell_scene.instantiate()
		self.add_child(cell)
		_cells.append(cell)
		cell.position = (Vector2i(i % CONSOLE_WIDTH, i / CONSOLE_WIDTH) * 24) - (Vector2i(6, 6))
		cell.modulate = Color.WHITE
		cell.text = ""
	# Set up the fake cells for the saved display
	for i: int in CONSOLE_WIDTH * CONSOLE_HEIGHT:
		var cell: SavedDisplayCell = SavedDisplayCell.new()
		_saved_cells.append(cell)
	var settings: Settings = Settings.load_or_create()
	apply_settings(settings)
	Profile.load_or_create()
	# Set up the audio players
	for sound_name:String in _sounds:
		var player: AudioStreamPlayer = _sound_players.get_or_add(sound_name, AudioStreamPlayer.new())
		player.bus = "Sfx"
		player.stream = _sounds[sound_name].stream
		add_child(player)
	# Start the game on a new thread
	User.game_thread.start(show_main_menu)


func _process(_delta: float) -> void:
	# Lock the mutex so we can check variables without clashing with the game thread
	User.mutex.lock()
	# If we should exit to the desktop, proces that
	if exit_to_desktop:
		User.mutex.unlock()
		# Wait for the game thread to finish, just in case it hasn't yet
		User.game_thread.wait_to_finish()
		# Shut down the display thread
		get_tree().quit()
	elif not User.game_thread.is_alive():
		# If we're not exiting, the game thread should be alive
		print ("Game thread unexpectedly died. Emergency quit...")
		breakpoint
		# Quit to avoid hanging, since there's no thread catching user input
		get_tree().quit()
	# The settings might need to be updated this frame
	if requires_settings_update:
		apply_settings(Settings.instance)
		requires_settings_update = false
	# The screen might need refreshing this frame
	if requires_refresh:
		for cell: DisplayCell in _cells:
			cell.refresh()
		_background_image.refresh()
		if not User.frame_semaphore.try_wait():
			User.frame_semaphore.post(1)
		requires_refresh = false
	# We might need to start a timer for animation frames
	if start_animation_timer:
		_animation_timer.start()
		start_animation_timer = false
	User.mutex.unlock()


static func set_mask(start_row: int, start_col: int, height: int, width: int) -> void:
	if start_row < 0 or start_col < 0:
		instance._mask.call_deferred("hide")
	else:
		instance._mask.call_deferred("show")
		instance._mask.set_deferred("position", Vector2i(start_col, start_row) * 24)
		instance._mask.set_deferred("size", Vector2(width*24, height*24))


func show_main_menu() -> void:
	while true:
		set_background(BackgroundImage.MENU)
		set_mask(-1,-1,-1,-1)
		change_music("Menu")
		clear_screen()
		hide_cursor()
		print_centred(41, "© 1997-2026 Dean Anderson", Ink.BRIGHT_RED)
		print_centred(42, "See Cthangpedia for license and credits.", Ink.BRIGHT_RED)
		print_centred(43, Constants.VERSION_STAMP, Ink.BRIGHT_RED)
		var can_continue: bool = (
			Profile.instance.savegame != null and Profile.instance.savegame.player != null
		)
		var options: Array[String] = [
			"New Game",
			"High Scores",
			"Cthangpedia",
			"Options",
			"Quit to Desktop",
		]
		if can_continue:
			options.push_front("Continue")
		else:
			options.insert(3, "Reset Knowledge")
		var menu: PopupTextMenu = PopupTextMenu.new(options)
		var choice: String = menu.show()
		match choice:
			"Continue":
				play_sound("MenuAccept")
				Profile.instance.run()
				if exit_to_desktop:
					return
			"New Game":
				play_sound("MenuAccept")
				if can_continue:
					var text: Array[String] = [
						"You already have a game in process.",
						"Starting a new game will overwrite",
						"your existing character without saving",
						"their progress to the high score table.",
						"However, it will not overwrite your",
						"meta-progression such as the knowledge",
						"you have about monsters.",
					]
					var overwrite: Array[String] = ["New Game", "Cancel"]
					var popup: PopupTextMenu = PopupTextMenu.new(overwrite, text)
					var result: String = popup.show()
					match result:
						"", "Cancel":
							play_sound("MenuAccept")
							continue
						"New Game":
							play_sound("MenuAccept")
							pass
				Profile.instance.savegame = null
				Profile.instance.run()
				if exit_to_desktop:
					return
			"Reset Knowledge":
				play_sound("MenuAccept")
				var text: Array[String] = [
					"Resetting knowledge will reset your.",
					"meta-progression. You will lose all of",
					"the monster knowledge that you have",
					"acquired in previous games.",
				]
				var overwrite: Array[String] = ["Reset Knowledge", "Cancel"]
				var popup: PopupTextMenu = PopupTextMenu.new(overwrite, text)
				var result: String = popup.show()
				match result:
					"", "Cancel":
						play_sound("MenuAccept")
						continue
					"Reset Knowledge":
						play_sound("MenuAccept")
						Profile.delete_file()
						Profile.load_or_create()
						continue
			"High Scores":
				play_sound("MenuAccept")
				high_score_table.display_top_scores()
			"Cthangpedia":
				play_sound("MenuAccept")
				_cthangpedia.set_deferred("visible", true)
			"Options":
				play_sound("MenuAccept")
				var options_screen: OptionsScreen = OptionsScreen.new()
				options_screen.show()
			"Quit to Desktop", "":
				exit_to_desktop = true
				return


## Apply the console settings to the console
func apply_settings(settings: Settings) -> void:
	# If fullscreen, set that, otherwise set the window size and center it in the screen
	if settings.fullscreen:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
		get_window().borderless = false
		get_window().size = settings.get_window_size()
		get_window().move_to_center()
	# Apply the CRT shader or not
	_shader_overlay.visible = settings.crt_effect
	# Get the font typeface and size
	var font: Font = settings.get_font()
	var size: int = settings.font_size
	# Loop through all our cells setting their font typeface and size to the ones we got from the settings
	for cell: DisplayCell in _cells:
		cell.add_theme_font_override("font", font)
		cell.add_theme_font_size_override("font_size", size)
	# Set the sound and music volume
	_music_player.volume_linear = settings.music_volume * Constants.MUSIC_VOLUME_SCALE
	_master_sound_player.volume_linear = settings.sound_volume * Constants.SFX_VOLUME_SCALE


## Move the cursor to a specific location
func _go_to(row: int, col: int) -> void:
	_cursor_row = row
	_cursor_col = col


## Advance the cursor, usually after printing a character
func _advance_cursor() -> void:
	_cursor_col += 1
	if _cursor_col >= CONSOLE_WIDTH:
		_cursor_row += 1
		_cursor_col = 0
	if _cursor_row >= CONSOLE_HEIGHT:
		_cursor_row = CONSOLE_HEIGHT - 1


## Print a single character in the default ink colour at the cursor location, advancing the cursor
func _ink_character(character: String) -> void:
	_cells[_cursor_row * CONSOLE_WIDTH + _cursor_col].set_both(character, _ink)
	_advance_cursor()


func _print_wrap(row: int, col: int, text: String, colour: int) -> void:
	_ink = _colours[colour]
	_cursor_row = row
	_cursor_col = col
	clear_row(_cursor_row)
	var split: PackedStringArray = text.split(" ")
	for i: int in split.size():
		var s: String = split[i]
		if i > 0:
			s = " " + s
		if _cursor_col + s.length() >= CONSOLE_WIDTH:
			_cursor_col = 0
			_cursor_row += 1
			if _cursor_row > 42:
				return
			clear_row(_cursor_row)
		for c: String in s:
			if c == " " and _cursor_col == 0:
				pass
			elif c == "\n":
				_cursor_col = 0
				_cursor_row += 1
				if row > 42:
					return
				clear_row(_cursor_col)
			else:
				_ink_character(c)


func _on_animation_timer_timeout() -> void:
	User.animation_semaphore.post(1)
