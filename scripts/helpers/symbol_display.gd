# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SymbolDisplay
extends Node

const SYMBOL_IDENTIFICATION: Array[String] = [
	"a : Ant",
	"A : Abomination",
	"b : Bat",
	"B : Bird",
	"c : Centipede",
	"C : Canine",
	"d : Dragon",
	"D : Ancient Dragon/Wyrm",
	"e : Floating Eye",
	"E : Elemental",
	"f : Feline",
	"F : Dragon Fly",
	"g : Golem",
	"G : Ghost",
	"h : Hobbit/Elf/Dwarf",
	"H : Hybrid",
	"i : Icky Thing",
	"I : Insect",
	"j : Jelly",
	"J : Snake",
	"k : Kobold",
	"K : Killer Beetle",
	"l : Louse",
	"L : Lich",
	"m : Mold",
	"M : Multi-Headed Reptile",
	"n : Naga",
	"N : (unused)",
	"o : Orc",
	"O : Ogre",
	"p : Person/Human",
	"P : Giant Humanoid",
	"q : Quadruped",
	"Q : Quylthulg (Pulsing Flesh Mound)",
	"r : Rodent",
	"R : Reptile/Amphibian",
	"s : Skeleton",
	"S : Spider/Scorpion/Tick",
	"t : Townsperson",
	"T : Troll",
	"u : Minor Demon",
	"U : Major Demon",
	"v : Vortex",
	"V : Vampire",
	"w : Worm/Worm-Mass",
	"W : Wight/Wraith/etc",
	"x : Xorn/Xaren/etc",
	"X : Extradimensional Entity",
	"y : Yeek",
	"Y : Yeti",
	"z : Zombie/Mummy",
	"Z : Zephyr Hound",
	"0 : Entrance to Pawnbrokers",
	"1 : Entrance to General Store",
	"2 : Entrance to Armoury",
	"3 : Entrance to Weaponsmith",
	"4 : Entrance to Temple",
	"5 : Entrance to Alchemy shop",
	"6 : Entrance to Magic Stores",
	"7 : Entrance to Black Market",
	"8 : Entrance to Hall of Records",
	"9 : Entrance to Bookstore",
	"& : Entrance to Inn",
	"@ : Entrance to your home (and you)",
	"  : An unexplored area",
	"· : Floor",
	"# : A wall (or secret door)",
	"% : A vein (magma or quartz)",
	"+ : A closed door",
	"' : An open door",
	"^ : A trap",
	": : Rubble",
	"< : An up staircase",
	"> : A down staircase",
	", : Food (or mushroom patch)",
	"| : An edged weapon (sword/dagger/etc)",
	"/ : A polearm (Axe/Pike/etc)",
	"\\ : A hafted weapon (mace/whip/etc)",
	"{ : A missile (arrow/bolt/shot)",
	"} : A launcher (bow/crossbow/sling)",
	"( : Soft armour",
	"[ : Hard armour",
	"] : Misc. armour",
	") : A shield",
	"* : Gems",
	"$ : Treasure",
	"! : A potion (or oil)",
	"? : A scroll",
	"- : A wand (or rod)",
	"_ : A staff",
	"= : A ring",
	"\" : An amulet (or necklace)",
	"~ : A tool (or miscellaneous item)",
	"; : An Elder Sign / Yellow Sign",
]


func display(from_carnage: bool) -> void:
	Console.clear_screen()
	var row: int = 2
	var col: int = 1
	var monster_chars: Array[String] = []
	if from_carnage:
		var monster_dict: Dictionary[String, bool]
		for monster_type: MonsterType in Profile.instance.monster_types.types:
			monster_dict.get_or_add(monster_type.character, true)
		monster_chars = monster_dict.keys()
		monster_chars.erase("@")
	for entry: String in SYMBOL_IDENTIFICATION:
		if from_carnage:
			if entry.left(1) != "N" and not monster_chars.has(entry.left(1)):
				continue
		Console.print_at(row, col, entry, Ink.WHITE)
		col += 41
		if col > 80:
			col = 1
			row += 1
		if row > 38 and not from_carnage:
			Console.any_key(44)
			Console.clear_screen()
			row = 1
			col = 1
