@abstract # Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Ancestry
extends Resource

# 'Angelic' names used by imps and golems
const _angel_syllable_1: Array[String] = ["Sa", "A", "U", "Mi", "Ra", "Ana", "Pa", "Lu", "She", "Ga", "Da", "O", "Pe", "Lau", "Za", "Ze", "E"]
const _angel_syllable_2: Array[String] = ["br", "m", "l", "z", "zr", "mm", "mr", "r", "ral", "ch", "zaz", "tr", "n", "lar"]
const _angel_syllable_3: Array[String] = ["iel", "ial", "ael", "ubim", "aphon", "iel", "ael"]
# 'Cthuloid' names used by mind flayers, miri nigri, and tcho tchos
const _cthuloid_syllable_1: Array[String] = ["Cth", "Az", "Fth", "Ts", "Xo", "Q'N", "R'L", "Ghata", "L", "Zz", "Fl", "Cl", "S", "Y"]
const _cthuloid_syllable_2: Array[String] = ["nar", "loi", "ul", "lu", "noth", "thon", "ath", "'N", "rhy", "oth", "aza", "agn", "oa", "og"]
const _cthuloid_syllable_3: Array[String] = ["l", "a", "u", "oa", "oggua", "oth", "ath", "aggua", "lu", "lo", "loth", "lotha", "agn", "axl"]
# 'Dwarven' names used by dwarves, cyclopses, half giants, and nibelungen
const _dwarf_syllable_1: Array[String] = ["B", "D", "F", "G", "Gl", "H", "K", "L", "M", "N", "R", "S", "T", "Th", "V"]
const _dwarf_syllable_2: Array[String] = ["a", "e", "i", "o", "oi", "u"]
const _dwarf_syllable_3: Array[String] = ["bur", "fur", "gan", "gnus", "gnar", "li", "lin", "lir", "mli", "nar", "nus", "rin", "ran", "sin", "sil", "sur"]
# 'Elvish' names used by elves, dark elves, high elves, half elves, and sprites
const _elf_syllable_1: Array[String] = ["Al", "An", "Bal", "Bel", "Cal", "Cel", "El", "Elr", "Elv", "Eow", "Ear", "F", "Fal", "Fel", "Fin", "G", "Gal", "Gel", "Gl", "Is", "Lan", "Leg", "Lom", "N", "Nal", "Nel", "S", "Sal", "Sel", "T", "Tal", "Tel", "Thr", "Tin"]
const _elf_syllable_2: Array[String] = ["a", "adrie", "ara", "e", "ebri", "ele", "ere", "i", "io", "ithra", "ilma", "il-Ga", "ili", "o", "orfi", "u", "y"]
const _elf_syllable_3: Array[String] = ["l", "las", "lad", "ldor", "ldur", "linde", "lith", "mir", "n", "nd", "ndel", "ndil", "ndir", "nduil", "ng", "mbor", "r", "rith", "ril", "riand", "rion", "s", "thien", "viel", "wen", "wyn"]
# 'Gnomish' names used by gnomes and draconians
const _gnome_syllable_1: Array[String] = ["Aar", "An", "Ar", "As", "C", "H", "Han", "Har", "Hel", "Iir", "J", "Jan", "Jar", "K", "L", "M", "Mar", "N", "Nik", "Os", "Ol", "P", "R", "S", "Sam", "San", "T", "Ter", "Tom", "Ul", "V", "W", "Y"]
const _gnome_syllable_2: Array[String] = ["a", "aa", "ai", "e", "ei", "i", "o", "uo", "u", "uu"]
const _gnome_syllable_3: Array[String] = ["ron", "re", "la", "ki", "kseli", "ksi", "ku", "ja", "ta", "na", "namari", "neli", "nika", "nikki", "nu", "nukka", "ka", "ko", "li", "kki", "rik", "po", "to", "pekka", "rjaana", "rjatta", "rjukka", "la", "lla", "lli", "mo", "nni"]
# 'Hobbitish' names used by hobbits and kobolds
const _hobbit_syllable_1: Array[String] = ["B", "Ber", "Br", "D", "Der", "Dr", "F", "Fr", "G", "H", "L", "Ler", "M", "Mer", "N", "P", "Pr", "Per", "R", "S", "T", "W"]
const _hobbit_syllable_2: Array[String] = ["a", "e", "i", "ia", "o", "oi", "u"]
const _hobbit_syllable_3: Array[String] = ["bo", "ck", "decan", "degar", "do", "doc", "go", "grin", "lba", "lbo", "lda", "ldo", "lla", "ll", "lo", "m", "mwise", "nac", "noc", "nwise", "p", "ppin", "pper", "tho", "to"]
# 'Human' names used by humans, great ones, skeletons, spectres, and vampires, zombies, and half titans
const _human_syllable_1: Array[String] = ["Ab", "Ac", "Ad", "Af", "Agr", "Ast", "As", "Al", "Adw", "Adr", "Ar", "B", "Br", "C", "Cr", "Ch", "Cad", "D", "Dr", "Dw", "Ed", "Eth", "Et", "Er", "El", "Eow", "F", "Fr", "G", "Gr", "Gw", "Gal", "Gl", "H", "Ha", "Ib", "Jer", "K", "Ka", "Ked", "L", "Loth", "Lar", "Leg", "M", "Mir", "N", "Nyd", "Ol", "Oc", "On", "P", "Pr", "R", "Rh", "S", "Sev", "T", "Tr", "Th", "V", "Y", "Z", "W", "Wic"]
const _human_syllable_2: Array[String] = ["a", "ae", "au", "ao", "are", "ale", "ali", "ay", "ardo", "e", "ei", "ea", "eri", "era", "ela", "eli", "enda", "erra", "i", "ia", "ie", "ire", "ira", "ila", "ili", "ira", "igo", "o", "oa", "oi", "oe", "ore", "u", "y"]
const _human_syllable_3: Array[String] = ["a", "and", "b", "bwyn", "baen", "bard", "c", "ctred", "cred", "ch", "can", "d", "dan", "don", "der", "dric", "dfrid", "dus", "f", "g", "gord", "gan", "l", "li", "lgrin", "lin", "lith", "lath", "loth", "ld", "ldric", "ldan", "m", "mas", "mos", "mar", "mond", "n", "nydd", "nidd", "nnon", "nwan", "nyth", "nad", "nn", "nnor", "nd", "p", "r", "ron", "rd", "s", "sh", "seth", "sean", "t", "th", "tha", "tlan", "trem", "tram", "v", "vudd", "w", "wan", "win", "wyn", "wyr", "wyr", "wyth"]
# 'Insectoid' names used by klackons
const _klackon_syllable_1: Array[String] = ["K'", "K", "Kri", "Kir", "Kiri", "Iriki", "Irik", "Karik", "Iri", "Akri"]
const _klackon_syllable_2: Array[String] = ["arak", "i", "iri", "ikki", "ki", "kiri", "ikir", "irak", "arik", "k'", "r"]
const _klackon_syllable_3: Array[String] = ["akkak", "ak", "ik", "ikkik", "irik", "arik", "kidik", "kii", "k", "ki", "riki", "irk"]
# 'Orcish' names used by half orcs, half ogres, and half trolls
const _orc_syllable_1: Array[String] = ["B", "Er", "G", "Gr", "H", "P", "Pr", "R", "V", "Vr", "T", "Tr", "M", "Dr"]
const _orc_syllable_2: Array[String] = ["a", "i", "o", "oo", "u", "ui"]
const _orc_syllable_3: Array[String] = ["dash", "dish", "dush", "gar", "gor", "gdush", "lo", "gdish", "k", "lg", "nak", "rag", "rbag", "rg", "rk", "ng", "nk", "rt", "ol", "urk", "shnak", "mog", "mak", "rak"]
# 'Yeekish' names used by yeeks
const _yeek_syllable_1: Array[String] = ["Y", "Ye", "Yee", "Y"]
const _yeek_syllable_2: Array[String] = ["ee", "eee", "ee", "ee-ee", "ee'ee", "'ee", "eee", "ee", "ee"]
const _yeek_syllable_3: Array[String] = ["k", "k", "k", "ek", "eek", "ek"]

static var ancestries: Dictionary[String, Ancestry]

@export var ability_bonus: Array[int]
@export var article: String
@export var age_range: int
@export var base_age: int
@export var base_device_bonus: int
@export var base_disarm_bonus: int
@export var base_melee_attack_bonus: int
@export var base_ranged_attack_bonus: int
@export var base_save_bonus: int
@export var base_search_bonus: int
@export var base_search_frequency: int
@export var base_stealth_bonus: int
@export var description_line_1: String
@export var description_line_2: String
@export var description_line_3: String
@export var description_line_4: String
@export var description_line_5: String
@export var description_line_6: String
@export var doesnt_eat: bool
@export var experience_factor: int
@export var feeds_on_nether: bool
@export var female_base_height: int
@export var female_base_weight: int
@export var female_height_range: int
@export var female_weight_range: int
@export var glows: bool
@export var has_speed_bonus: bool
@export var hit_die_bonus: int
@export var infravision: int
@export var is_incorporeal: bool
@export var is_nocturnal: bool
@export var is_sunlight_sensitive: bool
@export var male_base_height: int
@export var male_base_weight: int
@export var male_height_range: int
@export var male_weight_range: int
@export var mutates: bool
@export var resists_fear: bool
@export var sanity_immune: bool
@export var sanity_resistant: bool
@export var spills_potions: bool
@export var title: String
@export var background_starting_chart: int


static func _populate_ancestries() -> void:
	ancestries = { }
	ancestries.get_or_add("Cyclops", AncestryCyclops.new())
	ancestries.get_or_add("Dark Elf", AncestryDarkElf.new())
	ancestries.get_or_add("Draconian", AncestryDraconian.new())
	ancestries.get_or_add("Dwarf", AncestryDwarf.new())
	ancestries.get_or_add("Elf", AncestryElf.new())
	ancestries.get_or_add("Gnome", AncestryGnome.new())
	ancestries.get_or_add("Golem", AncestryGolem.new())
	ancestries.get_or_add("Great One", AncestryGreatOne.new())
	ancestries.get_or_add("Half Elf", AncestryHalfElf.new())
	ancestries.get_or_add("Half Giant", AncestryHalfGiant.new())
	ancestries.get_or_add("Half Ogre", AncestryHalfOgre.new())
	ancestries.get_or_add("Half Orc", AncestryHalfOrc.new())
	ancestries.get_or_add("Half Titan", AncestryHalfTitan.new())
	ancestries.get_or_add("Half Troll", AncestryHalfTroll.new())
	ancestries.get_or_add("High Elf", AncestryHighElf.new())
	ancestries.get_or_add("Hobbit", AncestryHobbit.new())
	ancestries.get_or_add("Human", AncestryHuman.new())
	ancestries.get_or_add("Imp", AncestryImp.new())
	ancestries.get_or_add("Klackon", AncestryKlackon.new())
	ancestries.get_or_add("Kobold", AncestryKobold.new())
	ancestries.get_or_add("Mind Flayer", AncestryMindFlayer.new())
	ancestries.get_or_add("Miri Nigri", AncestryMiriNigri.new())
	ancestries.get_or_add("Niebelung", AncestryNiebelung.new())
	ancestries.get_or_add("Sand Dweller", AncestrySandDweller.new())
	ancestries.get_or_add("Shadowling", AncestryShadowling.new())
	ancestries.get_or_add("Skeleton", AncestrySkeleton.new())
	ancestries.get_or_add("Spectre", AncestrySpectre.new())
	ancestries.get_or_add("Sprite", AncestrySprite.new())
	ancestries.get_or_add("Tcho Tcho", AncestryTchoTcho.new())
	ancestries.get_or_add("Vampire", AncestryVampire.new())
	ancestries.get_or_add("Yeek", AncestryYeek.new())
	ancestries.get_or_add("Zombie", AncestryZombie.new())



static func named_ancestry(name: String) -> Ancestry:
	if ancestries.size() == 0:
		_populate_ancestries()
	return ancestries[name]


static func sorted_ancestry_names() -> Array[String]:
	if ancestries.size() == 0:
		_populate_ancestries()
	var keys: Array[String] = ancestries.keys()
	keys.sort()
	return keys


func apply_armour_bonus(player: Player) -> void:
	pass


func apply_ancestral_status(player: Player) -> void:
	pass


func consume_food(player: Player, item: Item) -> void:
	player.set_food(player.food + item.type_specific_value)


func create_random_name() -> String:
	return ""


func doesnt_bleed(player: Player) -> bool:
	return false


func doesnt_stun(player: Player) -> bool:
	return false


func defaults_to_nonbinary() -> bool:
	return false


func get_abilities_as_item_flags(player: Player, f_1: FlagSet, f_2: FlagSet, f_3: FlagSet) -> void:
	pass


func get_history(player: Player) -> void:
	var full_history: String = PlayerHistory.get_history(player, background_starting_chart)
	# Split the buffer into four strings to fit on four lines of the screen
	var remaining_history: String = full_history.strip_edges()
	var i: int = 0
	while true:
		var n: int = remaining_history.length()
		if n < 60:
			player.history[i] = remaining_history
			break
		n = 60
		while n > 0 and remaining_history[n - 1] != ' ':
			n -= 1
		player.history[i] = remaining_history.left(n).strip_edges()
		remaining_history = remaining_history.substr(n).strip_edges()
		i += 1


func get_ancestral_power_text(player: Player) -> String:
	return ""


func self_knowledge(player: Player) -> Array[String]:
	return []


func use_ancestral_power(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	pass
