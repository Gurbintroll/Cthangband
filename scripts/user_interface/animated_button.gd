# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AnimatedButton
extends TextureButton

@export var hover_scale: Vector2 = Vector2(1.1, 1.1)
@export var hover_animation_length: float = 0.1
@export var un_hover_animation_length: float = 0.1

@export var press_scale: Vector2 = Vector2(0.95, 0.95)
@export var press_animation_length_1: float = 0.1
@export var press_animation_length_2: float = 0.1

var animation_tween: Tween


func _init() -> void:
	pivot_offset_ratio = Vector2.ONE / 2.0


func _on_mouse_entered() -> void:
	if disabled:
		return
	if animation_tween:
		animation_tween.kill()
	animation_tween = create_tween().set_trans(Tween.TRANS_SINE)
	animation_tween.tween_property(self, "scale", hover_scale, hover_animation_length)


func _on_mouse_exited() -> void:
	if animation_tween:
		animation_tween.kill()
	animation_tween = create_tween().set_trans(Tween.TRANS_SINE)
	animation_tween.tween_property(self, "scale", Vector2.ONE, un_hover_animation_length)


func _on_pressed() -> void:
	if disabled:
		return
	if animation_tween:
		animation_tween.kill()
	animation_tween = create_tween().set_trans(Tween.TRANS_SINE)
	animation_tween.tween_property(self, "scale", press_scale, press_animation_length_1)
	animation_tween.tween_property(self, "scale", hover_scale, press_animation_length_2)
