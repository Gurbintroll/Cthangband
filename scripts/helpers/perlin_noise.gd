# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PerlinNoise
extends Resource

const GRADIENT_TABLE_SIZE: int = 256
const BIT_MASK: int = 255
const _perm: Array[int] = [
	225,
	155,
	210,
	108,
	175,
	199,
	221,
	144,
	203,
	116,
	70,
	213,
	69,
	158,
	33,
	252,
	5,
	82,
	173,
	133,
	222,
	139,
	174,
	27,
	9,
	71,
	90,
	246,
	75,
	130,
	91,
	191,
	169,
	138,
	2,
	151,
	194,
	235,
	81,
	7,
	25,
	113,
	228,
	159,
	205,
	253,
	134,
	142,
	248,
	65,
	224,
	217,
	22,
	121,
	229,
	63,
	89,
	103,
	96,
	104,
	156,
	17,
	201,
	129,
	36,
	8,
	165,
	110,
	237,
	117,
	231,
	56,
	132,
	211,
	152,
	20,
	181,
	111,
	239,
	218,
	170,
	163,
	51,
	172,
	157,
	47,
	80,
	212,
	176,
	250,
	87,
	49,
	99,
	242,
	136,
	189,
	162,
	115,
	44,
	43,
	124,
	94,
	150,
	16,
	141,
	247,
	32,
	10,
	198,
	223,
	255,
	72,
	53,
	131,
	84,
	57,
	220,
	197,
	58,
	50,
	208,
	11,
	241,
	28,
	3,
	192,
	62,
	202,
	18,
	215,
	153,
	24,
	76,
	41,
	15,
	179,
	39,
	46,
	55,
	6,
	128,
	167,
	23,
	188,
	106,
	34,
	187,
	140,
	164,
	73,
	112,
	182,
	244,
	195,
	227,
	13,
	35,
	77,
	196,
	185,
	26,
	200,
	226,
	119,
	31,
	123,
	168,
	125,
	249,
	68,
	183,
	230,
	177,
	135,
	160,
	180,
	12,
	1,
	243,
	148,
	102,
	166,
	38,
	238,
	251,
	37,
	240,
	126,
	64,
	74,
	161,
	40,
	184,
	149,
	171,
	178,
	101,
	66,
	29,
	59,
	146,
	61,
	254,
	107,
	42,
	86,
	154,
	4,
	236,
	232,
	120,
	21,
	233,
	209,
	45,
	98,
	193,
	114,
	78,
	19,
	206,
	14,
	118,
	127,
	48,
	79,
	147,
	85,
	30,
	207,
	219,
	54,
	88,
	234,
	190,
	122,
	95,
	67,
	143,
	109,
	137,
	214,
	145,
	93,
	92,
	100,
	245,
	0,
	216,
	186,
	60,
	83,
	105,
	97,
	204,
	52,
]

@export var _gradients: Array[float]


func randomise(new_seed: int) -> void:
	var random: RandomNumberGenerator = RandomNumberGenerator.new()
	random.seed = new_seed
	_gradients = []
	for i: int in GRADIENT_TABLE_SIZE:
		var z: float = 1.0 - (2.0 * random.randf())
		var r: float = sqrt(1.0 - (z * z))
		var theta: float = 2.0 * PI * random.randf()
		_gradients.append(r * cos(theta))
		_gradients.append(r * sin(theta))
		_gradients.append(z)


func noise(x: float, y: float, z: float) -> float:
	var ix: int = floori(x)
	var fx0: float = x - ix
	var fx1: float = fx0 - 1
	var wx: float = _smooth(fx0)
	var iy: int = floori(y)
	var fy0: float = y - iy
	var fy1: float = fy0 - 1
	var wy: float = _smooth(fy0)
	var iz: int = floori(z)
	var fz0: float = z - iz
	var fz1: float = fz0 - 1
	var wz: float = _smooth(fz0)
	var vx0: float = _lattice(ix, iy, iz, fx0, fy0, fz0)
	var vx1: float = _lattice(ix + 1, iy, iz, fx1, fy0, fz0)
	var vy0: float = _lerp(wx, vx0, vx1)
	vx0 = _lattice(ix, iy + 1, iz, fx0, fy1, fz0)
	vx1 = _lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0)
	var vy1: float = _lerp(wx, vx0, vx1)
	var vz0: float = _lerp(wy, vy0, vy1)
	vx0 = _lattice(ix, iy, iz + 1, fx0, fy0, fz1)
	vx1 = _lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1)
	vy0 = _lerp(wx, vx0, vx1)
	vx0 = _lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1)
	vx1 = _lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1)
	vy1 = _lerp(wx, vx0, vx1)
	var vz1: float = _lerp(wy, vy0, vy1)
	return _lerp(wz, vz0, vz1)


func _index(ix: int, iy: int, iz: int) -> int:
	return _permutate(ix + _permutate(iy + _permutate(iz)))


func _permutate(x: int) -> int:
	return _perm[x & BIT_MASK]


func _smooth(x: float) -> float:
	return x * x * (3.0 - (2.0 * x))


func _lerp(t: float, value_0: float, value_1: float) -> float:
	return value_0 + (t * (value_1 - value_0))


func _lattice(ix: int, iy: int, iz: int, fx: float, fy: float, fz: float) -> float:
	var index: int = _index(ix, iy, iz)
	var g: int = index * 3
	return (_gradients[g] * fx) + (_gradients[g + 1] * fy) + (_gradients[g + 2] * fz)
