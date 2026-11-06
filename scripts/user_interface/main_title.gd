# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MainTitle
extends Label


func _ready() -> void:
	var rotation_tween: Tween = create_tween()
	rotation_tween.set_loops()
	rotation_tween.tween_property(self, "rotation", 0.005, 1.1).set_ease(Tween.EASE_OUT)
	rotation_tween.tween_property(self, "rotation", -0.005, 2.2).set_ease(Tween.EASE_IN_OUT)
	rotation_tween.tween_property(self, "rotation", 0.0, 1.1).set_ease(Tween.EASE_IN)
	var size_tween: Tween = create_tween()
	size_tween.set_loops()
	size_tween.tween_property(self, "scale", Vector2(1.01, 1.01), 2.6).set_ease(Tween.EASE_OUT)
	size_tween.tween_property(self, "scale", Vector2(0.99, 0.99), 5.2).set_ease(Tween.EASE_IN_OUT)
	size_tween.tween_property(self, "scale", Vector2(1.0, 1.0), 2.6).set_ease(Tween.EASE_IN)
