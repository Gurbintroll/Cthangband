# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name OptionsScreen
extends Object
## An options screen for changing the console settings

## The currently selected item
var _selected_item: int = 0


## Show the options screen
func show() -> void:
	Console.save_screen()
	Console.clear_screen()
	Console.set_background(BackgroundImage.OPTIONS)
	Console.full_screen_overlay = true
	_print_options_screen()
	while true:
		_print_menu()
		var key: String = User.get_key()
		match key:
			"Up":
				Console.play_sound("MenuSelect")
				# Shift the selected item up, and reprint the menu
				_selected_item -= 1
				if _selected_item < 0:
					_selected_item = 5
			"Down":
				Console.play_sound("MenuSelect")
				# Shift the selected item down, and reprint the menu
				_selected_item += 1
				if _selected_item > 5:
					_selected_item = 0
			"Right":
				# Adjust the selected menu item
				match _selected_item:
					0:
						# Switch font weight, apply the change, and reprint the menu
						if Settings.instance.font_weight < 2:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.font_weight = ((Settings.instance.font_weight as int) + 1) as Settings.FontWeight
							Console.requires_settings_update = true
							User.mutex.unlock()
					1:
						# Switch font size, apply the change, and reprint the menu
						if Settings.instance.font_size < 34:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.font_size += 1
							Console.requires_settings_update = true
							User.mutex.unlock()
					2:
						# Switch fullscreen off, apply the change, and reprint the menu
						if Settings.instance.fullscreen:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.fullscreen = false
							Console.requires_settings_update = true
							User.mutex.unlock()
					3:
						# Switch the window size, apply the change, and reprint the menu
						if Settings.instance.window_size < 7:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.window_size = ((Settings.instance.window_size as int) + 1) as Settings.Resolution
							Console.requires_settings_update = true
							User.mutex.unlock()
					4:
						# Increase the music volume if possible
						if Settings.instance.music_volume < 100:
							Console.play_sound("MenuSelect")
							Settings.instance.music_volume += 5
							Console.set_music_volume(Settings.instance.music_volume)
					5:
						# Increase the sound volume if possible
						# Note that we play the sound after adjusting the volume
						if Settings.instance.sound_volume < 100:
							Settings.instance.sound_volume += 5
							Console.set_sound_volume(Settings.instance.sound_volume)
							Console.play_sound("MenuSelect")
			"Left":
				match _selected_item:
					0:
						# Switch font weight, apply the change, and reprint the menu
						if Settings.instance.font_weight > 0:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.font_weight = ((Settings.instance.font_weight as int) - 1) as Settings.FontWeight
							Console.requires_settings_update = true
							User.mutex.unlock()
					1:
						# Switch font size, apply the change, and reprint the menu
						if Settings.instance.font_size > 14:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.font_size -= 1
							Console.requires_settings_update = true
							User.mutex.unlock()
					2:
						# Switch fullscreen on, apply the change, and reprint the menu
						if Settings.instance.fullscreen == false:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.fullscreen = true
							Console.requires_settings_update = true
							User.mutex.unlock()
					3:
						# Switch the window size, apply the change, and reprint the menu
						if Settings.instance.window_size > 0:
							Console.play_sound("MenuSelect")
							User.mutex.lock()
							Settings.instance.window_size = ((Settings.instance.window_size as int) - 1) as Settings.Resolution
							Console.requires_settings_update = true
							User.mutex.unlock()
					4:
						# Decrease the music volume if possible
						if Settings.instance.music_volume > 0:
							Console.play_sound("MenuSelect")
							Settings.instance.music_volume -= 5
							Console.set_music_volume(Settings.instance.music_volume)
					5:
						# Decrease the music volume if possible
						if Settings.instance.sound_volume > 0:
							Settings.instance.sound_volume -= 5
							Console.set_sound_volume(Settings.instance.sound_volume)
							Console.play_sound("MenuSelect")
			"Escape":
				Console.play_sound("MenuAccept")
				# Save the new settings values, and return to the main menu
				Settings.save()
				break
	Console.restore_screen()
	Console.full_screen_overlay = false


func _print_options_screen() -> void:
	# First print our basic options that we can change
	Console.print_at(1, 10, "Options", Ink.GREEN)
	Console.print_at(2, 10, "=======", Ink.GREEN)
	Console.print_at(4, 1, "   Text style:", Ink.BLUE)
	Console.print_at(5, 1, "    Text size:", Ink.BLUE)
	Console.print_at(6, 1, "Display style:", Ink.BLUE)
	Console.print_at(7, 1, "  Window size:", Ink.BLUE)
	Console.print_at(8, 1, " Music volume:", Ink.BLUE)
	Console.print_at(9, 1, " Sound volume:", Ink.BLUE)
	# Print some sample text in all the different colours
	Console.print_at(12, 8, "Sample Text", Ink.GREEN)
	Console.print_at(13, 8, "===========", Ink.GREEN)
	Console.print_at(15, 1, "Red", Ink.RED)
	Console.print_at(15, 15, "Bright Red", Ink.BRIGHT_RED)
	Console.print_at(16, 1, "Orange", Ink.ORANGE)
	Console.print_at(16, 15, "Bright Orange", Ink.BRIGHT_ORANGE)
	Console.print_at(17, 1, "Yellow", Ink.YELLOW)
	Console.print_at(17, 15, "Bright Yellow", Ink.BRIGHT_YELLOW)
	Console.print_at(18, 1, "Chartreuse", Ink.CHARTREUSE)
	Console.print_at(18, 15, "Bright Chartreuse", Ink.BRIGHT_CHARTREUSE)
	Console.print_at(19, 1, "Green", Ink.GREEN)
	Console.print_at(19, 15, "Bright Green", Ink.BRIGHT_GREEN)
	Console.print_at(20, 1, "Turquoise", Ink.TURQUOISE)
	Console.print_at(20, 15, "Bright Turquoise", Ink.BRIGHT_TURQUOISE)
	Console.print_at(21, 1, "Blue", Ink.BLUE)
	Console.print_at(21, 15, "Bright Blue", Ink.BRIGHT_BLUE)
	Console.print_at(22, 1, "Purple", Ink.PURPLE)
	Console.print_at(22, 15, "Bright Purple", Ink.BRIGHT_PURPLE)
	Console.print_at(23, 1, "Pink", Ink.PINK)
	Console.print_at(23, 15, "Bright Pink", Ink.BRIGHT_PINK)
	Console.print_at(25, 1, "Brown", Ink.BROWN)
	Console.print_at(25, 15, "Bright Brown", Ink.BRIGHT_BROWN)
	Console.print_at(26, 1, "Beige", Ink.BEIGE)
	Console.print_at(26, 15, "Bright Beige", Ink.BRIGHT_BEIGE)
	Console.print_at(28, 1, "Black", Ink.BLACK)
	Console.print_at(29, 1, "Grey", Ink.GREY)
	Console.print_at(30, 1, "Bright Grey", Ink.BRIGHT_GREY)
	Console.print_at(31, 1, "White", Ink.WHITE)
	Console.print_at(32, 1, "Bright White", Ink.BRIGHT_WHITE)
	Console.print_at(28, 15, "Copper", Ink.COPPER)
	Console.print_at(29, 15, "Silver", Ink.SILVER)
	Console.print_at(30, 15, "Gold", Ink.GOLD)
	Console.print_at(31, 15, "Diamond", Ink.DIAMOND)
	Console.print_at(35, 1, "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG", Ink.BLACK)
	Console.print_at(36, 1, "the quick brown fox jumps over the lazy dog", Ink.BLACK)
	Console.print_at(37, 1, "1234567890 !\"$%^&·*()_+-={}[]<>.,/?:@~;'#\\|", Ink.BLACK)
	# Print a sample layout so we can see what it will look like
	Console.print_at(1, 58, "Sample Layout", Ink.GREEN)
	Console.print_at(2, 58, "=============", Ink.GREEN)
	Console.print_at(4, 50, "                   # #       ", Ink.GREY)
	Console.print_at(5, 50, "            ########+########", Ink.GREY)
	Console.print_at(6, 50, "            #      + +       ", Ink.GREY)
	Console.print_at(7, 50, "            # ###############", Ink.GREY)
	Console.print_at(8, 50, "            # #              ", Ink.GREY)
	Console.print_at(9, 50, "      #######·########       ", Ink.GREY)
	Console.print_at(10, 50, "      #··············#       ", Ink.GREY)
	Console.print_at(11, 50, "      #·!·······@····#       ", Ink.GREY)
	Console.print_at(12, 50, "#######···o··········#       ", Ink.GREY)
	Console.print_at(13, 50, "··^·k·'··············#       ", Ink.GREY)
	Console.print_at(14, 50, "#######·oo······$····#       ", Ink.GREY)
	Console.print_at(15, 50, "      #··o···········#       ", Ink.GREY)
	Console.print_at(16, 50, "      #######c########       ", Ink.GREY)
	Console.print_at(17, 50, "            #·#              ", Ink.GREY)
	Console.print_at(18, 50, "            #·#######        ", Ink.GREY)
	Console.print_at(19, 50, "            #·······#        ", Ink.GREY)
	Console.print_at(20, 50, "            #######·#        ", Ink.GREY)
	Console.print_at(21, 50, "                  #'#########", Ink.GREY)
	Console.print_at(22, 50, "                  #·'········", Ink.GREY)
	Console.print_at(23, 50, "                  #'#########", Ink.GREY)
	Console.print_at(24, 50, "                  #·#        ", Ink.GREY)
	Console.print_at(25, 50, "      #############·#        ", Ink.GREY)
	Console.print_at(26, 50, "      #·············#        ", Ink.GREY)
	Console.print_at(27, 50, "      *·#############        ", Ink.GREY)
	Console.print_at(28, 50, "      #·# ################   ", Ink.GREY)
	Console.print_at(29, 50, "      #·###·····>········#   ", Ink.GREY)
	Console.print_at(30, 50, "      #···'··········^···#   ", Ink.GREY)
	Console.print_at(31, 50, "      #####·······|······#   ", Ink.GREY)
	Console.print_at(32, 50, "          #·~············#   ", Ink.GREY)
	Console.print_at(33, 50, "          #··············#   ", Ink.GREY)
	Console.print_at(34, 50, "          ######'############", Ink.GREY)
	Console.print_at(35, 50, "               #·##··········", Ink.GREY)
	Console.print_at(36, 50, "               #····#########", Ink.GREY)
	Console.print_at(37, 50, "               ######        ", Ink.GREY)
	# Overprint elements of the sample display with coloured elements
	Console.print_at(5, 70, "+", Ink.BROWN)
	Console.print_at(6, 69, "+ +", Ink.BROWN)
	Console.print_at(9, 56, "#######·########       ", Ink.WHITE)
	Console.print_at(10, 56, "#··············#       ", Ink.WHITE)
	Console.print_at(11, 56, "#·!·······@····#       ", Ink.WHITE)
	Console.print_at(12, 56, "#···o··········#       ", Ink.WHITE)
	Console.print_at(13, 52, "^·k·'··············#       ", Ink.WHITE)
	Console.print_at(14, 50, "#######·oo······$····#       ", Ink.WHITE)
	Console.print_at(15, 50, "      #··o···········#       ", Ink.WHITE)
	Console.print_at(16, 50, "      #######c########       ", Ink.WHITE)
	Console.print_at(10, 65, "···", Ink.BRIGHT_YELLOW)
	Console.print_at(11, 65, "·@·", Ink.BRIGHT_YELLOW)
	Console.print_at(12, 65, "···", Ink.BRIGHT_YELLOW)
	Console.print_at(11, 66, "@", Ink.BRIGHT_WHITE)
	Console.print_at(11, 58, "!", Ink.ORANGE)
	Console.print_at(12, 60, "o", Ink.BRIGHT_GREEN)
	Console.print_at(13, 52, "^", Ink.RED)
	Console.print_at(13, 54, "k", Ink.GREEN)
	Console.print_at(13, 56, "'", Ink.BRIGHT_BROWN)
	Console.print_at(14, 58, "oo", Ink.BLACK)
	Console.print_at(14, 66, "$", Ink.COPPER)
	Console.print_at(15, 59, "o", Ink.BLACK)
	Console.print_at(16, 63, "c", Ink.BRIGHT_BLUE)
	Console.print_at(17, 62, "#", Ink.WHITE)
	Console.print_at(17, 63, "·", Ink.BLACK)
	Console.print_at(18, 63, "·", Ink.BLACK)
	Console.print_at(19, 63, "·······", Ink.BLACK)
	Console.print_at(20, 69, "·", Ink.BLACK)
	Console.print_at(21, 69, "'", Ink.BROWN)
	Console.print_at(22, 69, "·'········", Ink.BLACK)
	Console.print_at(22, 70, "'", Ink.BROWN)
	Console.print_at(23, 69, "'", Ink.BROWN)
	Console.print_at(24, 69, "·", Ink.BLACK)
	Console.print_at(25, 69, "·", Ink.BLACK)
	Console.print_at(26, 57, "·············", Ink.BLACK)
	Console.print_at(27, 56, "*", Ink.RED)
	Console.print_at(27, 57, "·", Ink.BLACK)
	Console.print_at(28, 57, "·", Ink.BLACK)
	Console.print_at(29, 57, "·", Ink.BLACK)
	Console.print_at(29, 66, ">", Ink.BROWN)
	Console.print_at(30, 57, "···", Ink.BLACK)
	Console.print_at(30, 60, "'", Ink.BROWN)
	Console.print_at(30, 71, "^", Ink.PURPLE)
	Console.print_at(31, 68, "|", Ink.BRIGHT_WHITE)
	Console.print_at(32, 62, "~", Ink.BEIGE)
	Console.print_at(34, 66, "'", Ink.BROWN)
	Console.print_at(35, 66, "·", Ink.BLACK)
	Console.print_at(35, 69, "··········", Ink.BLACK)
	Console.print_at(36, 66, "····", Ink.BLACK)
	Console.print_centred (43, "[Use arrows to set options, Escape to save and exit]", Ink.ORANGE)


## Print the menu, showing the currently selected option
func _print_menu() -> void:
	# First clear the old menu options
	for row: int in range(4, 10):
		Console.print_at(row, 16, "                                  ", Ink.WHITE)
	# Now print the new ones, with colour depending on whether or not they're currently selected
	Console.print_at(4, 16, Settings.instance.font_weight_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 0 else Ink.GREY)
	Console.print_at(5, 16, Settings.instance.font_size_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 1 else Ink.GREY)
	Console.print_at(6, 16, Settings.instance.fullscreen_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 2 else Ink.GREY)
	Console.print_at(7, 16, Settings.instance.window_size_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 3 else Ink.GREY)
	Console.print_at(8, 16, Settings.instance.music_volume_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 4 else Ink.GREY)
	Console.print_at(9, 16, Settings.instance.sound_volume_as_string(), Ink.BRIGHT_PURPLE if _selected_item == 5 else Ink.GREY)
