extends Node

var game_thread: Thread = Thread.new()
var mutex: Mutex = Mutex.new()
var key_semaphore: Semaphore = Semaphore.new()
var frame_semaphore: Semaphore = Semaphore.new()
var animation_semaphore: Semaphore = Semaphore.new()
var current_command: String
var queued_command: String
var command_repeat: int
var command_argument: int
var command_direction: int
var _buffer: Array[String]


func _input(event: InputEvent) -> void:
	var key: String
	# We only care about key presses
	if event is InputEventKey:
		var key_event: InputEventKey = event
		if key_event.pressed:
			# Catch special keys first
			var keycode: String = event.as_text_keycode()
			var prefix: String = ""
			if keycode.left(6) == "Shift+":
				prefix = "Shift+"
				keycode = keycode.lstrip("Shift+")
			match (keycode):
				"Delete":
					key = "."
				"Insert", "Kp 0":
					key = "0"
				"Left", "Kp 4":
					key = prefix + "Left"
				"Right", "Kp 6":
					key = prefix + "Right"
				"Up", "Kp 8":
					key = prefix + "Up"
				"Down", "Kp 2":
					key = prefix + "Down"
				"Home", "Kp 7":
					key = prefix + "UpLeft"
				"PageUp", "Kp 9":
					key = prefix + "UpRight"
				"End", "Kp 1":
					key = prefix + "DownLeft"
				"PageDown", "Kp 3":
					key = prefix + "DownRight"
				"Clear", "Kp 5":
					key = "Middle"
				"Enter", "Kp Enter":
					key = "Enter"
				"Backspace":
					key = "Backspace"
				"Escape":
					key = "Escape"
				_:
					var unicode: int = event.unicode
					if unicode != 0:
						key = char(unicode)
	# If we caught a key, so add it to the buffer
	if key != "":
		mutex.lock()
		_buffer.append(key)
		mutex.unlock()
		# If the key is the only one in the buffer, signal that a key has been pressed
		if _buffer.size() == 1:
			key_semaphore.post(1)


## Prompts the user for a simple choice. Returns true if the user makes a choice or false it the user presses escape
func get_simple_choice(prompt: String) -> String:
	Profile.instance.message_print()
	if prompt.length() > 1:
		prompt = prompt.left(1).to_upper() + prompt.substr(1)
	Console.print_line_at(0, 0, prompt, Ink.WHITE)
	var choice: String = get_key()
	Console.clear_row(0)
	return "" if choice == "Escape" else choice


# Gets a command, possibly including repeats
func get_command(shopping: bool) -> void:
	current_command = ""
	command_argument = 0
	command_direction = 0
	# Repeatedly try to get a command until we have one
	while true:
		var cmd: String
		# If we have a queued command, promote it to being the current command
		if queued_command != "":
			Profile.instance.message_print()
			cmd = queued_command
			queued_command = ""
		else:
			Profile.instance.message_should_continue = false
			cmd = get_key()
		Console.clear_row(0)
		# The special '0' command means ask for a number and then repeat the following command that many times
		if cmd == "0":
			var old_command_argument: int = command_argument
			command_argument = 0
			Console.print_line_at(0, 0, "Count: ", Ink.WHITE)
			# Build the number of repeats we want from numeric keypresses
			while true:
				cmd = get_key(true)
				if cmd == "Backspace":
					command_argument /= 10
					Console.print_line_at(0, 0, "Count: %d" % [command_argument], Ink.WHITE)
				elif ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"].has(cmd):
					if command_argument >= 1000:
						command_argument = 9999
					else:
						command_argument = (command_argument * 10) + (cmd as int)
					Console.print_line_at(0, 0, "Count: %d" % [command_argument], Ink.WHITE)
				else:
					break
			if command_argument == 0:
				command_argument = 99
				Console.print_line_at(0, 0, "Count: %d" % [command_argument], Ink.WHITE)
			if old_command_argument != 0:
				command_argument = old_command_argument
				Console.print_line_at(0, 0, "Count: %d" % [command_argument], Ink.WHITE)
			if cmd == " " or cmd == "Enter":
				cmd = get_simple_choice("Command: ")
				if cmd == "":
					command_argument = 0
					continue
		cmd = _handle_moves(cmd)
		if cmd == "":
			continue
		current_command = cmd
		break
	# Certain commans automatically get repeated 99 times
	if command_argument <= 0 and not shopping:
		if "TBDocs+".contains(current_command):
			command_argument = 99
	# If we're in the shop, map certain commands to others
	if shopping:
		match current_command:
			"p":
				current_command = "g"
			"m":
				current_command = "g"
			"s":
				current_command = "d"
	Console.clear_row(0)


func get_quantity(prompt: String, max_num: int, all_by_default: bool) -> int:
	var quantity: int
	if command_argument != 0:
		quantity = command_argument
		command_argument = 0
		return mini(quantity, max_num)
	if prompt == "":
		prompt = "Quantity (1-%d): " % [max_num]
	quantity = max_num if all_by_default else 1
	var default: String = str(quantity)
	var start_col: int = prompt.length()
	Console.print_line_at(0, 0, prompt, Ink.BRIGHT_PURPLE)
	var result: Dictionary[bool, String] = get_input(0, start_col, default, 4)
	Console.clear_row(0)
	if not result.has(true):
		return 0
	var text: String = result[true]
	quantity = int(text)
	return clampi(quantity, 0, max_num)


## Get a simple yes/no from the player, Escape = no, Enter = yes
func get_check(prompt: String) -> bool:
	Profile.instance.message_print("")
	var text: String = "%s[Y/n]" % [prompt]
	Console.print_line_at(0, 0, text, Ink.BRIGHT_PURPLE)
	while true:
		match get_key():
			"Escape", "N", "n":
				Console.clear_row(0)
				return false
			"Enter", "Y", "y":
				Console.clear_row(0)
				return true
	return false


func get_prompted_input(prompt: String, default: String, max_length: int) -> Dictionary[bool, String]:
	Console.print_line_at(0, 0, prompt, Ink.WHITE)
	var col: int = prompt.length()
	var input: Dictionary[bool, String] = get_input(0, col, default, max_length)
	Console.clear_row(0)
	return input


## Input a string, returning an abort flag (normally false) and a result
func get_input(row: int, col: int, default: String, max_length: int) -> Dictionary[bool, String]:
	var result: Dictionary[bool, String]
	var input_string: String = ""
	while true:
		if input_string == "":
			Console.print_at(row, col, default.rpad(max_length), Ink.BRIGHT_PURPLE)
		else:
			Console.print_at(row, col, input_string.rpad(max_length), Ink.BLACK)
		Console.show_cursor(row, col + input_string.length())
		var key: String = get_key(true)
		if key.length() == 1 and input_string.length() < max_length:
			input_string += key
		if key == "Enter":
			break
		if key == "Escape":
			result.get_or_add(false, default)
			Console.hide_cursor()
			return result
		if key == "Backspace" and input_string.length() > 0:
			input_string = input_string.left(input_string.length() - 1)
	if input_string == "":
		input_string = default
	result.get_or_add(true, input_string)
	Console.hide_cursor()
	return result


func wait_for_frame() -> void:
	mutex.lock()
	Console.requires_refresh = true
	mutex.unlock()
	frame_semaphore.wait()


func wait_for_animation() -> void:
	mutex.lock()
	Console.requires_refresh = true
	Console.start_animation_timer = true
	mutex.unlock()
	animation_semaphore.wait()


## Get the pressed key, without waiting for one if there isn't one
func get_key_no_wait() -> String:
	var key: String
	# If there's already something in the buffer, return it
	mutex.lock()
	var size: int = _buffer.size()
	# If we now have something, grab it and break out of the loop
	if size != 0:
		key = _buffer.pop_front()
	mutex.unlock()
	return key


## Get the next keypress, waiting if necessary
func get_key(translate_direction: bool = false) -> String:
	var size: int
	var key: String
	mutex.lock()
	Console.requires_refresh = true
	mutex.unlock()
	while true:
		# Check if we have anything already in the buffer
		mutex.lock()
		size = _buffer.size()
		mutex.unlock()
		# If not, wait for a semaphore that we have
		if size == 0:
			key_semaphore.wait()
		# Check again (keyboard-mashing might have confused things
		mutex.lock()
		size = _buffer.size()
		# If we now have something, grab it and break out of the loop
		if size != 0:
			key = _buffer.pop_front()
			mutex.unlock()
			break
		mutex.unlock()
	# Convert directions into numbers (e.g., "Down" ==> "2") if necessary
	if translate_direction:
		key = strip_movement_keys(key)
	return key


## Return the keypad direction represented by a keypress, or 0 if no direction
func key_to_keypad_direction(key: String) -> int:
	key = strip_movement_keys(key)
	match key:
		"1":
			return 1
		"2":
			return 2
		"3":
			return 3
		"4":
			return 4
		"5":
			return 5
		"6":
			return 6
		"7":
			return 7
		"8":
			return 8
		"9":
			return 9
		_:
			return 0


func _handle_moves(cmd: String) -> String:
	match cmd:
		"DownLeft":
			command_direction = 1
			return ";"
		"Down":
			command_direction = 2
			return ";"
		"DownRight":
			command_direction = 3
			return ";"
		"Left":
			command_direction = 4
			return ";"
		"Middle":
			return ","
		"Right":
			command_direction = 6
			return ";"
		"UpLeft":
			command_direction = 7
			return ";"
		"Up":
			command_direction = 8
			return ";"
		"UpRight":
			command_direction = 9
			return ";"
		"Shift+DownLeft":
			command_direction = 1
			return "."
		"Shift+Down":
			command_direction = 2
			return "."
		"Shift+DownRight":
			command_direction = 3
			return "."
		"Shift+Left":
			command_direction = 4
			return "."
		"Shift+Right":
			command_direction = 6
			return "."
		"Shift+UpLeft":
			command_direction = 7
			return "."
		"Shift+Up":
			command_direction = 8
			return "."
		"Shift+UpRight":
			command_direction = 9
			return "."
		_:
			return cmd


## Get rid of movement keys if we're somewhere where we should have simple numbers instead
func strip_movement_keys(key: String) -> String:
	if key.length() > 1:
		match key:
			"DownLeft", "Shift+DownLeft":
				key = "1"
			"Down", "Shift+Down":
				key = "2"
			"DownRight", "Shift+DownRight":
				key = "3"
			"Left", "Shift+Left":
				key = "4"
			"Middle":
				key = "5"
			"Right", "Shift+Right":
				key = "6"
			"UpLeft", "Shift+UpLeft":
				key = "7"
			"Up", "Shift+Up":
				key = "8"
			"UpRight", "Shift+UpRight":
				key = "9"
	return key
