# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SavedDisplayCell
extends Object
## A simple holder to remember the state of a display cell so we can restore it later

## The character that the display cell holds
var character: String = " "
## The colour of the display cell
var colour: Color = Color.LIGHT_GRAY
