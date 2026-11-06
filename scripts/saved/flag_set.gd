# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name FlagSet
extends Resource

@export var value: int = 0


func copy(original: FlagSet) -> void:
	value = original.value
	return


func low_order_bits()->int:
	return value & 0x0000FFFF


func high_order_bits()->int:
	return (value & 0xFFFF0000) >> 16


func keep_bits(bits: int) -> void:
	value &= bits


func clear_bits(bits: int) -> void:
	value &= ~bits


func clear_flags(flags: FlagSet) -> void:
	value &= ~flags.value


func clear_all() -> void:
	value = 0


func is_clear(flags: int) -> bool:
	return (value & flags) == 0


func is_set(flags: int) -> bool:
	return (value & flags) != 0


func is_any_set() -> bool:
	return (value != 0)


func is_all_clear() -> bool:
	return (value == 0)


func bits_match(other: FlagSet, bits: int) -> bool:
	return (value & bits) == (other.value & bits)


func matches(other: FlagSet) -> bool:
	return (value == other.value)


func set_bits(bits: int) -> void:
	value |= bits


func set_flags(flags: FlagSet) -> void:
	value |= flags.value


func set_all() -> void:
	value = 0xFFFFFFFF
