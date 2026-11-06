extends Node
## A wrapper for the random number generator

const RAND_NOR_NUM: int = 256
const RAND_NOR_STD: int = 64

## Whether we're using the fixed seed for repeatable results or the general seed
var _use_fixed_seed: bool = false
var _main_sequence: RandomNumberGenerator = RandomNumberGenerator.new()
var _fixed: RandomNumberGenerator = RandomNumberGenerator.new()
var _rand_nor_table: Array[int] = [
	206,
	613,
	1022,
	1430,
	1838,
	2245,
	2652,
	3058,
	3463,
	3867,
	4271,
	4673,
	5075,
	5475,
	5874,
	6271,
	6667,
	7061,
	7454,
	7845,
	8234,
	8621,
	9006,
	9389,
	9770,
	10148,
	10524,
	10898,
	11269,
	11638,
	12004,
	12367,
	12727,
	13085,
	13440,
	13792,
	14140,
	14486,
	14828,
	15168,
	15504,
	15836,
	16166,
	16492,
	16814,
	17133,
	17449,
	17761,
	18069,
	18374,
	18675,
	18972,
	19266,
	19556,
	19842,
	20124,
	20403,
	20678,
	20949,
	21216,
	21479,
	21738,
	21994,
	22245,
	22493,
	22737,
	22977,
	23213,
	23446,
	23674,
	23899,
	24120,
	24336,
	24550,
	24759,
	24965,
	25166,
	25365,
	25559,
	25750,
	25937,
	26120,
	26300,
	26476,
	26649,
	26818,
	26983,
	27146,
	27304,
	27460,
	27612,
	27760,
	27906,
	28048,
	28187,
	28323,
	28455,
	28585,
	28711,
	28835,
	28955,
	29073,
	29188,
	29299,
	29409,
	29515,
	29619,
	29720,
	29818,
	29914,
	30007,
	30098,
	30186,
	30272,
	30356,
	30437,
	30516,
	30593,
	30668,
	30740,
	30810,
	30879,
	30945,
	31010,
	31072,
	31133,
	31192,
	31249,
	31304,
	31358,
	31410,
	31460,
	31509,
	31556,
	31601,
	31646,
	31688,
	31730,
	31770,
	31808,
	31846,
	31882,
	31917,
	31950,
	31983,
	32014,
	32044,
	32074,
	32102,
	32129,
	32155,
	32180,
	32205,
	32228,
	32251,
	32273,
	32294,
	32314,
	32333,
	32352,
	32370,
	32387,
	32404,
	32420,
	32435,
	32450,
	32464,
	32477,
	32490,
	32503,
	32515,
	32526,
	32537,
	32548,
	32558,
	32568,
	32577,
	32586,
	32595,
	32603,
	32611,
	32618,
	32625,
	32632,
	32639,
	32645,
	32651,
	32657,
	32662,
	32667,
	32672,
	32677,
	32682,
	32686,
	32690,
	32694,
	32698,
	32702,
	32705,
	32708,
	32711,
	32714,
	32717,
	32720,
	32722,
	32725,
	32727,
	32729,
	32731,
	32733,
	32735,
	32737,
	32739,
	32740,
	32742,
	32743,
	32745,
	32746,
	32747,
	32748,
	32749,
	32750,
	32751,
	32752,
	32753,
	32754,
	32755,
	32756,
	32757,
	32757,
	32758,
	32758,
	32759,
	32760,
	32760,
	32761,
	32761,
	32761,
	32762,
	32762,
	32763,
	32763,
	32763,
	32764,
	32764,
	32764,
	32764,
	32765,
	32765,
	32765,
	32765,
	32766,
	32766,
	32766,
	32766,
	32767,
]


## Randomize on start
func _init() -> void:
	_main_sequence.randomize()


## Tell the rng to use a fixed seed
func use_fixed_seed(new_seed: int) -> void:
	_fixed.seed = new_seed
	_use_fixed_seed = true


## Tell the rng to stop using the fixed seed
func use_main_sequence() -> void:
	_use_fixed_seed = false


func big_number() -> int:
	var rng: RandomNumberGenerator = _fixed if _use_fixed_seed else _main_sequence
	return rng.randi()


## Roll multiple dice
func roll_dice(number: int, sides: int) -> int:
	var sum: int = 0
	for i: int in range(number):
		sum += roll_die(sides)
	return sum


## Roll multiple dice forcing a maximum result
func roll_dice_max(number: int, sides: int) -> int:
	return number * sides


## Roll a single die
func roll_die(sides: int) -> int:
	return _next(sides) + 1


## Roll a d100 and see if you get less than or equal to the chance
func roll_chance(chance: int) -> bool:
	return (_next(100) < chance)


## Roll a flat distribution around a number
func random_around(centre: int, range_around: int) -> int:
	return centre + _next(1 + range_around + range_around) - range_around


## Find a number between two others, inclusive
func random_between(minimum: int, maximum: int) -> int:
	return minimum + _next(1 + maximum - minimum)


## Return a number within a normal distribution
func random_normal(mean: int, sd: int) -> int:
	var low: int = 0
	var high: int = RAND_NOR_NUM
	if sd < 1:
		return mean
	var tmp: int = _next(32768)
	while low < high:
		var mid: int = (low + high) >> 1
		if (_rand_nor_table[mid] < tmp):
			low = mid + 1
		else:
			high = mid
	var offset: int = sd * low / RAND_NOR_STD
	if roll_chance(50):
		return mean - offset
	else:
		return mean + offset


## return the next number in the pseudo-random sequence
func _next(maximum: int) -> int:
	if (maximum <= 1):
		return 0
	var rng: RandomNumberGenerator = _fixed if _use_fixed_seed else _main_sequence
	return rng.randi() % maximum
