# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name PlayerHistory
extends RefCounted

static var _background_table: Array[PlayerHistory] = [
	# Group 1: Human start /Half-Elf legitimacy 1->2->3->50->51->52->53->End
	PlayerHistory.new("You are the illegitimate and unacknowledged child ", 10, 1, 2, 25),
	PlayerHistory.new("You are the illegitimate but acknowledged child ", 20, 1, 2, 35),
	PlayerHistory.new("You are one of several children ", 95, 1, 2, 45),
	PlayerHistory.new("You are the first child ", 100, 1, 2, 50),
	# Group 2: Human/Half-Elf/Half-Orc/Half-Ogre/Half-Giant/Half-Titan parent 2->3->50->51->52->53->End
	PlayerHistory.new("of a Serf. ", 40, 2, 3, 65),
	PlayerHistory.new("of a Yeoman. ", 65, 2, 3, 80),
	PlayerHistory.new("of a Townsman. ", 80, 2, 3, 90),
	PlayerHistory.new("of a Guildsman. ", 90, 2, 3, 105),
	PlayerHistory.new("of a Landed Knight. ", 96, 2, 3, 120),
	PlayerHistory.new("of a Noble Family. ", 99, 2, 3, 130),
	PlayerHistory.new("of the Royal Blood Line. ", 100, 2, 3, 140),
	# Group 3: Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan childhood 3->50->51->52->53->End
	PlayerHistory.new("You are the black sheep of the family. ", 20, 3, 50, 20),
	PlayerHistory.new("You are a credit to the family. ", 80, 3, 50, 55),
	PlayerHistory.new("You are a well liked child. ", 100, 3, 50, 60),
	# Group 4: Half-Elf start 4->1->2->3->50->51->52->53->End
	PlayerHistory.new("Your mother was of the Teleri. ", 40, 4, 1, 50),
	PlayerHistory.new("Your father was of the Teleri. ", 75, 4, 1, 55),
	PlayerHistory.new("Your mother was of the Noldor. ", 90, 4, 1, 55),
	PlayerHistory.new("Your father was of the Noldor. ", 95, 4, 1, 60),
	PlayerHistory.new("Your mother was of the Vanyar. ", 98, 4, 1, 65),
	PlayerHistory.new("Your father was of the Vanyar. ", 100, 4, 1, 70),
	# Group 7: Elf/High-Elf start 7->8->9->54->55->56->End
	PlayerHistory.new("You are one of several children ", 60, 7, 8, 50),
	PlayerHistory.new("You are the only child ", 100, 7, 8, 55),
	# Group 8: Elf/High-Elf ancestry 8->9->54->55->56->End
	PlayerHistory.new("of a Teleri ", 75, 8, 9, 50),
	PlayerHistory.new("of a Noldor ", 95, 8, 9, 55),
	PlayerHistory.new("of a Vanyar ", 100, 8, 9, 60),
	# Group 9: Elf/High-Elf parent 9->54->55->56->End
	PlayerHistory.new("Ranger. ", 40, 9, 54, 80),
	PlayerHistory.new("Archer. ", 70, 9, 54, 90),
	PlayerHistory.new("Warrior. ", 87, 9, 54, 110),
	PlayerHistory.new("Mage. ", 95, 9, 54, 125),
	PlayerHistory.new("Prince. ", 99, 9, 54, 140),
	PlayerHistory.new("King. ", 100, 9, 54, 145),
	# Group 10: Hobbit start 10->11->3->50->51->52->53->End
	PlayerHistory.new("You are one of several children of a Hobbit ", 85, 10, 11, 45),
	PlayerHistory.new("You are the only child of a Hobbit ", 100, 10, 11, 55),
	# Group 11: Hobbit parent 11->3->50->51->52->53->End
	PlayerHistory.new("Bum. ", 20, 11, 3, 55),
	PlayerHistory.new("Tavern Owner. ", 30, 11, 3, 80),
	PlayerHistory.new("Miller. ", 40, 11, 3, 90),
	PlayerHistory.new("Home Owner. ", 50, 11, 3, 100),
	PlayerHistory.new("Burglar. ", 80, 11, 3, 110),
	PlayerHistory.new("Warrior. ", 95, 11, 3, 115),
	PlayerHistory.new("Mage. ", 99, 11, 3, 125),
	PlayerHistory.new("Clan Elder. ", 100, 11, 3, 140),
	# Group 13: Gnome start 13->14->3->50->51->52->53->End
	PlayerHistory.new("You are one of several children of a Gnome ", 85, 13, 14, 45),
	PlayerHistory.new("You are the only child of a Gnome ", 100, 13, 14, 55),
	# Group 14: Gnome parent 14->3->50->51->52->53->End
	PlayerHistory.new("Beggar. ", 20, 14, 3, 55),
	PlayerHistory.new("Braggart. ", 50, 14, 3, 70),
	PlayerHistory.new("Prankster. ", 75, 14, 3, 85),
	PlayerHistory.new("Warrior. ", 95, 14, 3, 100),
	PlayerHistory.new("Mage. ", 100, 14, 3, 125),
	# Group 16: Dwarf start 16->17->18->57->58->59->60->61->End
	PlayerHistory.new("You are one of two children of a Dwarven ", 25, 16, 17, 40),
	PlayerHistory.new("You are the only child of a Dwarven ", 100, 16, 17, 50),
	# Group 17: Dwarf parent 17->18->57->58->59->60->61->End
	PlayerHistory.new("Thief. ", 10, 17, 18, 60),
	PlayerHistory.new("Prison Guard. ", 25, 17, 18, 75),
	PlayerHistory.new("Miner. ", 75, 17, 18, 90),
	PlayerHistory.new("Warrior. ", 90, 17, 18, 110),
	PlayerHistory.new("Priest. ", 99, 17, 18, 130),
	PlayerHistory.new("King. ", 100, 17, 18, 150),
	# Group 18: Dwarf/Nibelung childhood 18->57->58->59->60->61->End
	PlayerHistory.new("You are the black sheep of the family. ", 15, 18, 57, 10),
	PlayerHistory.new("You are a credit to the family. ", 85, 18, 57, 50),
	PlayerHistory.new("You are a well liked child. ", 100, 18, 57, 55),
	# Group 19: Half-Orc start 19->20->2->3->50->51->52->53->End
	PlayerHistory.new("Your mother was an Orc, but it is unacknowledged. ", 25, 19, 20, 25),
	PlayerHistory.new("Your mother was an Orc. ", 50, 19, 20, 25),
	PlayerHistory.new("Your father was an Orc. ", 75, 19, 20, 25),
	PlayerHistory.new("Your father was an Orc, but it is unacknowledged. ", 100, 19, 20, 25),
	# Group 20: Half-Orc/Half-Ogre/Half-Giant/Half-Titan adoption 20->2->3->50->51->52->53->End
	PlayerHistory.new("You are the child ", 80, 20, 2, 50),
	PlayerHistory.new("You are the adopted child ", 100, 20, 2, 50),
	# Group 22: Half-Troll start 22->23->62->63->64->65->66->End
	PlayerHistory.new("Your mother was a Cave-Troll ", 30, 22, 23, 20),
	PlayerHistory.new("Your father was a Cave-Troll ", 60, 22, 23, 25),
	PlayerHistory.new("Your mother was a Hill-Troll ", 75, 22, 23, 30),
	PlayerHistory.new("Your father was a Hill-Troll ", 90, 22, 23, 35),
	PlayerHistory.new("Your mother was a Water-Troll ", 95, 22, 23, 40),
	PlayerHistory.new("Your father was a Water-Troll ", 100, 22, 23, 45),
	# Group 23: Half-Troll parent 23->62->63->64->65->66->End
	PlayerHistory.new("Cook. ", 5, 23, 62, 60),
	PlayerHistory.new("Warrior. ", 95, 23, 62, 55),
	PlayerHistory.new("Priest. ", 99, 23, 62, 65),
	PlayerHistory.new("Clan Chief. ", 100, 23, 62, 80),
	# Group 24: Kobold scales 24->25->26->End
	PlayerHistory.new("You have green scales, ", 25, 24, 25, 50),
	PlayerHistory.new("You have dark green scales, ", 50, 24, 25, 50),
	PlayerHistory.new("You have yellow scales, ", 75, 24, 25, 50),
	PlayerHistory.new("You have green scales, a yellow belly, ", 100, 24, 25, 50),
	# Group 25: Kobold eyes 25->26->End
	PlayerHistory.new("bright eyes, ", 25, 25, 26, 50),
	PlayerHistory.new("yellow eyes, ", 50, 25, 26, 50),
	PlayerHistory.new("red eyes, ", 75, 25, 26, 50),
	PlayerHistory.new("snake-like eyes, ", 100, 25, 26, 50),
	# Group 26: Kobold tail 26->End
	PlayerHistory.new("and a long sinuous tail.", 20, 26, 0, 50),
	PlayerHistory.new("and a short tail.", 40, 26, 0, 50),
	PlayerHistory.new("and a muscular tail.", 60, 26, 0, 50),
	PlayerHistory.new("and a long tail.", 80, 26, 0, 50),
	PlayerHistory.new("and a sinuous tail.", 100, 26, 0, 50),
	# Group 50: Great One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan eyes 50->51->52->53->End
	PlayerHistory.new("You have dark brown eyes, ", 20, 50, 51, 50),
	PlayerHistory.new("You have brown eyes, ", 60, 50, 51, 50),
	PlayerHistory.new("You have hazel eyes, ", 70, 50, 51, 50),
	PlayerHistory.new("You have green eyes, ", 80, 50, 51, 50),
	PlayerHistory.new("You have blue eyes, ", 90, 50, 51, 50),
	PlayerHistory.new("You have blue-gray eyes, ", 100, 50, 51, 50),
	# Group 51: Great One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan hairstyle 51->52->53->End
	PlayerHistory.new("straight ", 70, 51, 52, 50),
	PlayerHistory.new("wavy ", 90, 51, 52, 50),
	PlayerHistory.new("curly ", 100, 51, 52, 50),
	# Group 52: Great One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan hair colour 52->53->End
	PlayerHistory.new("black hair, ", 30, 52, 53, 50),
	PlayerHistory.new("brown hair, ", 70, 52, 53, 50),
	PlayerHistory.new("auburn hair, ", 80, 52, 53, 50),
	PlayerHistory.new("red hair, ", 90, 52, 53, 50),
	PlayerHistory.new("blond hair, ", 100, 52, 53, 50),
	# Group 53: Great One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan complexion 53->End
	PlayerHistory.new("and a very dark complexion.", 10, 53, 0, 50),
	PlayerHistory.new("and a dark complexion.", 30, 53, 0, 50),
	PlayerHistory.new("and an olive complexion.", 80, 53, 0, 50),
	PlayerHistory.new("and a pale complexion.", 90, 53, 0, 50),
	PlayerHistory.new("and a very pale complexion.", 100, 53, 0, 50),
	# Group 54: Elf/High-Elf eyes 54->55->56->End
	PlayerHistory.new("You have light grey eyes, ", 85, 54, 55, 50),
	PlayerHistory.new("You have light blue eyes, ", 95, 54, 55, 50),
	PlayerHistory.new("You have light green eyes, ", 100, 54, 55, 50),
	# Group 55: Elf/High-Elf hairstyle 55->56->End
	PlayerHistory.new("straight ", 75, 55, 56, 50),
	PlayerHistory.new("wavy ", 100, 55, 56, 50),
	# Group 56: Elf/High-Elf colour 56->End
	PlayerHistory.new("black hair, and a pale complexion.", 75, 56, 0, 50),
	PlayerHistory.new("brown hair, and a pale complexion.", 85, 56, 0, 50),
	PlayerHistory.new("blond hair, and a pale complexion.", 95, 56, 0, 50),
	PlayerHistory.new("silver hair, and a pale complexion.", 100, 56, 0, 50),
	# Group 57: Dwarf/Nibelung eyes 57->58->59->60->61->End
	PlayerHistory.new("You have dark brown eyes, ", 99, 57, 58, 50),
	PlayerHistory.new("You have glowing red eyes, ", 100, 57, 58, 60),
	# Group 58: Dwarf/Nibelung hairstyle 58->59->60->61->End
	PlayerHistory.new("straight ", 90, 58, 59, 50),
	PlayerHistory.new("wavy ", 100, 58, 59, 50),
	# Group 59: Dwarf/Nibelung hair colour 59->60->61->End
	PlayerHistory.new("black hair, ", 75, 59, 60, 50),
	PlayerHistory.new("brown hair, ", 100, 59, 60, 50),
	# Group 60: Dwarf/Nibelung beard 60->61->End
	PlayerHistory.new("a one foot beard, ", 25, 60, 61, 50),
	PlayerHistory.new("a two foot beard, ", 60, 60, 61, 51),
	PlayerHistory.new("a three foot beard, ", 90, 60, 61, 53),
	PlayerHistory.new("a four foot beard, ", 100, 60, 61, 55),
	# Group 61: Dwarf/Nibelung complexion 61->End
	PlayerHistory.new("and a dark complexion.", 100, 61, 0, 50),
	# Group 62: Half-Troll/Zombie eyes 62->63->64->65->66->End
	PlayerHistory.new("You have slime green eyes, ", 60, 62, 63, 50),
	PlayerHistory.new("You have puke yellow eyes, ", 85, 62, 63, 50),
	PlayerHistory.new("You have blue-bloodshot eyes, ", 99, 62, 63, 50),
	PlayerHistory.new("You have glowing red eyes, ", 100, 62, 63, 55),
	# Group 63: Half-Troll/Zombie hairstyle 63->64->65->66->End
	PlayerHistory.new("dirty ", 33, 63, 64, 50),
	PlayerHistory.new("mangy ", 66, 63, 64, 50),
	PlayerHistory.new("oily ", 100, 63, 64, 50),
	# Group 64: Half-Troll/Zombie hair 64->65->66->End
	PlayerHistory.new("sea-weed green hair, ", 33, 64, 65, 50),
	PlayerHistory.new("bright red hair, ", 66, 64, 65, 50),
	PlayerHistory.new("dark purple hair, ", 100, 64, 65, 50),
	# Group 65: Half-Troll/Zombie skin colour 65->66->End
	PlayerHistory.new("and green ", 25, 65, 66, 50),
	PlayerHistory.new("and blue ", 50, 65, 66, 50),
	PlayerHistory.new("and white ", 75, 65, 66, 50),
	PlayerHistory.new("and black ", 100, 65, 66, 50),
	# Group 66: Half-Troll/Zombie skin texture 66->End
	PlayerHistory.new("ulcerous skin.", 33, 66, 0, 50),
	PlayerHistory.new("scabby skin.", 66, 66, 0, 50),
	PlayerHistory.new("leprous skin.", 100, 66, 0, 50),
	# Group 67: Great One start 67->68->50->51->52->53->End
	PlayerHistory.new("You are an unacknowledged child of ", 50, 67, 68, 45),
	PlayerHistory.new("You are a rebel child of ", 80, 67, 68, 65),
	PlayerHistory.new("You are a long lost child of ", 100, 67, 68, 55),
	# Group 68: Great One parent 68->50->51->52->53->End
	PlayerHistory.new("someone with Great One blood. ", 50, 68, 50, 80),
	PlayerHistory.new("an unknown child of a Great One. ", 65, 68, 50, 90),
	PlayerHistory.new("an unknown Great One. ", 79, 68, 50, 100),
	PlayerHistory.new("Karakal. ", 80, 68, 50, 130),
	PlayerHistory.new("Hagarg Ryonis. ", 83, 68, 50, 105),
	PlayerHistory.new("Lobon. ", 84, 68, 50, 105),
	PlayerHistory.new("Nath-Horthath. ", 85, 68, 50, 90),
	PlayerHistory.new("Tamash. ", 87, 68, 50, 100),
	PlayerHistory.new("Zo-Kalar. ", 88, 68, 50, 125),
	PlayerHistory.new("Karakal. ", 89, 68, 50, 120),
	PlayerHistory.new("Hagarg Ryonis. ", 90, 68, 50, 140),
	PlayerHistory.new("Lobon. ", 91, 68, 50, 115),
	PlayerHistory.new("Nath-Horthath. ", 92, 68, 50, 110),
	PlayerHistory.new("Tamash. ", 93, 68, 50, 105),
	PlayerHistory.new("Zo-Kalar. ", 94, 68, 50, 95),
	PlayerHistory.new("Karakal. ", 95, 68, 50, 115),
	PlayerHistory.new("Hagarg Ryonis. ", 96, 68, 50, 110),
	PlayerHistory.new("Lobon. ", 97, 68, 50, 135),
	PlayerHistory.new("Nath-Horthath. ", 98, 68, 50, 90),
	PlayerHistory.new("Tamash. ", 99, 68, 50, 105),
	PlayerHistory.new("Zo-Kalar. ", 100, 68, 50, 80),
	# Group 69: Dark-Elf start 69->70->71->72->73->End
	PlayerHistory.new("You are one of several children of a Dark Elven ", 85, 69, 70, 45),
	PlayerHistory.new("You are the only child of a Dark Elven ", 100, 69, 70, 55),
	# Group 70: Dark-Elf parent 70->71->72->73->End
	PlayerHistory.new("Warrior. ", 50, 70, 71, 60),
	PlayerHistory.new("Warlock. ", 80, 70, 71, 75),
	PlayerHistory.new("Noble. ", 100, 70, 71, 95),
	# Group 71: Dark-Elf eyes 71->72->73->End
	PlayerHistory.new("You have black eyes, ", 100, 71, 72, 50),
	# Group 72: Dark-Elf hair style 72->73->End
	PlayerHistory.new("straight ", 70, 72, 73, 50),
	PlayerHistory.new("wavy ", 90, 72, 73, 50),
	PlayerHistory.new("curly ", 100, 72, 73, 50),
	# Group 73: Dark-Elf complexion 73->End
	PlayerHistory.new("black hair and a very dark complexion.", 100, 73, 0, 50),
	# Group 74: Half-Ogre start 74->20->2->3->50->51->52->53->End
	PlayerHistory.new("Your mother was an Ogre, but it is unacknowledged. ", 25, 74, 20, 25),
	PlayerHistory.new("Your father was an Ogre. ", 50, 74, 20, 25),
	PlayerHistory.new("Your mother was an Ogre. ", 75, 74, 20, 25),
	PlayerHistory.new("Your father was an Ogre, but it is unacknowledged. ", 100, 74, 20, 25),
	# Group 75: Half-Giant start 75->20->2->3->50->51->52->53->End
	PlayerHistory.new("Your mother was a Hill Giant. ", 10, 75, 20, 50),
	PlayerHistory.new("Your mother was a Fire Giant. ", 12, 75, 20, 55),
	PlayerHistory.new("Your mother was a Frost Giant. ", 20, 75, 20, 60),
	PlayerHistory.new("Your mother was a Cloud Giant. ", 23, 75, 20, 65),
	PlayerHistory.new("Your mother was a Storm Giant. ", 25, 75, 20, 70),
	PlayerHistory.new("Your father was a Hill Giant. ", 60, 75, 20, 50),
	PlayerHistory.new("Your father was a Fire Giant. ", 70, 75, 20, 55),
	PlayerHistory.new("Your father was a Frost Giant. ", 80, 75, 20, 60),
	PlayerHistory.new("Your father was a Cloud Giant. ", 90, 75, 20, 65),
	PlayerHistory.new("Your father was a Storm Giant. ", 100, 75, 20, 70),
	# Group 76: Half-Titan start 75->20->2->3->50->51->52->53->End
	PlayerHistory.new("Your father was an unknown Titan. ", 75, 76, 20, 50),
	PlayerHistory.new("Your mother was Themis. ", 80, 76, 20, 100),
	PlayerHistory.new("Your mother was Mnemosyne. ", 85, 76, 20, 100),
	PlayerHistory.new("Your father was Okeanoas. ", 90, 76, 20, 100),
	PlayerHistory.new("Your father was Crius. ", 95, 76, 20, 100),
	PlayerHistory.new("Your father was Hyperion. ", 98, 76, 20, 125),
	PlayerHistory.new("Your father was Kronos. ", 100, 76, 20, 150),
	# Group 77: Cyclops start 77->109->110->111->112->End
	PlayerHistory.new("You are the offspring of an unknown Cyclops. ", 90, 77, 109, 50),
	PlayerHistory.new("You are Polyphemos's child. ", 98, 77, 109, 80),
	PlayerHistory.new("You are Uranos's child. ", 100, 77, 109, 135),
	# Group 78: Yeek start 78->79->80->81->135->136->137->End
	PlayerHistory.new("You are the runt of ", 20, 78, 79, 40),
	PlayerHistory.new("You come from ", 80, 78, 79, 50),
	PlayerHistory.new("You are the largest of ", 100, 78, 79, 55),
	# Group 79: Yeek litter size 79->80->81->135->136->137->End
	PlayerHistory.new("a litter of 3 pups. ", 15, 79, 80, 55),
	PlayerHistory.new("a litter of 4 pups. ", 40, 79, 80, 55),
	PlayerHistory.new("a litter of 5 pups. ", 70, 79, 80, 50),
	PlayerHistory.new("a litter of 6 pups. ", 85, 79, 80, 50),
	PlayerHistory.new("a litter of 7 pups. ", 95, 79, 80, 45),
	PlayerHistory.new("a litter of 8 pups. ", 100, 79, 80, 45),
	# Group 80: Yeek parent 80->81->135->136->137->End
	PlayerHistory.new("Your mother was a scavenger. ", 25, 80, 81, 40),
	PlayerHistory.new("Your mother was a sneak. ", 50, 80, 81, 45),
	PlayerHistory.new("Your mother was a warrior. ", 75, 80, 81, 50),
	PlayerHistory.new("Your mother was a master yeek. ", 95, 80, 81, 55),
	PlayerHistory.new("Your father was a yeek king. ", 100, 80, 81, 60),
	# Group 81: Yeek fur style 81->135->136->137->End
	PlayerHistory.new("You have mangy ", 33, 81, 135, 50),
	PlayerHistory.new("You have short ", 66, 81, 135, 50),
	PlayerHistory.new("You have long ", 100, 81, 135, 50),
	# Group 82: Kobold start 82->83->24->25->26->End
	PlayerHistory.new("You are one of several children of ", 100, 82, 83, 50),
	# Group 83: Kobold parent 83->24->25->26->End
	PlayerHistory.new("a Small Kobold. ", 40, 83, 24, 50),
	PlayerHistory.new("a Kobold. ", 75, 83, 24, 55),
	PlayerHistory.new("a Large Kobold. ", 95, 83, 24, 65),
	PlayerHistory.new("Vort, the Kobold Queen. ", 100, 83, 24, 100),
	# Group 84: Klackon start 84->85->86->End
	PlayerHistory.new("You are one of several children of a Klackon hive queen. ", 100, 84, 85, 50),
	# Group 85: Klackon skin 85->86->End
	PlayerHistory.new("You have red chitin, ", 40, 85, 86, 50),
	PlayerHistory.new("You have black chitin, ", 90, 85, 86, 50),
	PlayerHistory.new("You have yellow chitin, ", 100, 85, 86, 50),
	# Group 86: Klackon antennae 86->End
	PlayerHistory.new("long antennae, and black eyes.", 50, 86, 0, 50),
	PlayerHistory.new("short antennae, and black eyes.", 80, 86, 0, 50),
	PlayerHistory.new("feathered antennae, and black eyes.", 100, 86, 0, 50),
	# Group 87: Nibelung start 87->88->18->57->58->59->60->61->End
	PlayerHistory.new("You are one of several children of ", 100, 87, 88, 89),
	# Group 88: Nibelung parent 88->18->57->58->59->60->61->End
	PlayerHistory.new("a Nibelung Slave. ", 30, 88, 18, 20),
	PlayerHistory.new("a Nibelung Thief. ", 50, 88, 18, 40),
	PlayerHistory.new("a Nibelung Smith. ", 70, 88, 18, 60),
	PlayerHistory.new("a Nibelung Miner. ", 90, 88, 18, 75),
	PlayerHistory.new("a Nibelung Priest. ", 95, 88, 18, 100),
	PlayerHistory.new("Mime, the Nibelung. ", 100, 88, 18, 100),
	# Group 89: Draconian start 89->90->91->End
	PlayerHistory.new("You are one of several children of a Draconian ", 85, 89, 90, 50),
	PlayerHistory.new("You are the only child of a Draconian ", 100, 89, 90, 55),
	# Group 90: Draconian parent 90->91->End
	PlayerHistory.new("Warrior. ", 50, 90, 91, 50),
	PlayerHistory.new("Priest. ", 65, 90, 91, 65),
	PlayerHistory.new("Mage. ", 85, 90, 91, 70),
	PlayerHistory.new("Noble. ", 100, 90, 91, 100),
	# Group 91: Draconian colour 91->End
	PlayerHistory.new("You have green wings, green skin and yellow belly.", 30, 91, 0, 50),
	PlayerHistory.new("You have green wings, and green skin.", 55, 91, 0, 50),
	PlayerHistory.new("You have red wings, and red skin.", 80, 91, 0, 50),
	PlayerHistory.new("You have black wings, and black skin.", 90, 91, 0, 50),
	PlayerHistory.new("You have metallic skin, and shining wings.", 100, 91, 0, 50),
	# Group 92: Mind-Flayer start 93->93->End
	PlayerHistory.new("You have slimy skin, empty glowing eyes, and ", 100, 92, 93, 80),
	# Group 93: Mind-Flayer tentacles 93->End
	PlayerHistory.new("three tentacles around your mouth.", 20, 93, 0, 45),
	PlayerHistory.new("four tentacles around your mouth.", 80, 93, 0, 50),
	PlayerHistory.new("five tentacles around your mouth.", 100, 93, 0, 55),
	# Group 94: Imp start 94->95->96->97->End
	PlayerHistory.new("You ancestor was ", 100, 94, 95, 50),
	# Group 95: Imp ancestor 95->96->97->End
	PlayerHistory.new("a mindless demonic spawn. ", 30, 95, 96, 20),
	PlayerHistory.new("a minor demon. ", 60, 95, 96, 50),
	PlayerHistory.new("a major demon. ", 90, 95, 96, 75),
	PlayerHistory.new("a demon lord. ", 100, 95, 96, 99),
	# Group 96: Imp skin 96->97->End
	PlayerHistory.new("You have red skin, ", 50, 96, 97, 50),
	PlayerHistory.new("You have brown skin, ", 100, 96, 97, 50),
	# Group 97: Imp eyes 97->End
	PlayerHistory.new("claws, fangs, spikes, and glowing red eyes.", 40, 97, 0, 50),
	PlayerHistory.new("claws, fangs, and glowing red eyes.", 70, 97, 0, 50),
	PlayerHistory.new("claws, and glowing red eyes.", 100, 97, 0, 50),
	# Group 98: Golem start 98->99->100->101->End
	PlayerHistory.new("You were shaped from ", 100, 98, 99, 50),
	# Group 99: Golem material 99->100->101->End
	PlayerHistory.new("clay ", 40, 99, 100, 50),
	PlayerHistory.new("stone ", 80, 99, 100, 50),
	PlayerHistory.new("wood ", 85, 99, 100, 40),
	PlayerHistory.new("iron ", 99, 99, 100, 50),
	PlayerHistory.new("pure gold ", 100, 99, 100, 100),
	# Group 100: Golem creator 100->101->End
	PlayerHistory.new("by a Kabbalist", 40, 100, 101, 50),
	PlayerHistory.new("by a Wizard", 65, 100, 101, 50),
	PlayerHistory.new("by an Alchemist", 90, 100, 101, 50),
	PlayerHistory.new("by a Priest", 100, 100, 101, 60),
	# Group 101: Golem purpose 101->End
	PlayerHistory.new(" to fight evil.", 10, 101, 0, 65),
	PlayerHistory.new(".", 100, 101, 0, 50),
	# Group 102: Skeleton start 102->103->104->105->106->End
	PlayerHistory.new("You were created by ", 100, 102, 103, 50),
	# Group 103: Skeleton creator 103->104->105->106->End
	PlayerHistory.new("a Necromancer. ", 30, 103, 104, 50),
	PlayerHistory.new("a magical experiment. ", 50, 103, 104, 50),
	PlayerHistory.new("an Evil Priest. ", 70, 103, 104, 50),
	PlayerHistory.new("a pact with the demons. ", 75, 103, 104, 50),
	PlayerHistory.new("a restless spirit. ", 85, 103, 104, 50),
	PlayerHistory.new("a curse. ", 95, 103, 104, 30),
	PlayerHistory.new("an oath. ", 100, 103, 104, 50),
	# Group 104: Skeleton join 104->105->106->End
	PlayerHistory.new("You have ", 100, 104, 105, 50),
	# Group 105: Skeleton bones 105->106->End
	PlayerHistory.new("dirty, dry bones, ", 40, 105, 106, 50),
	PlayerHistory.new("rotten black bones, ", 60, 105, 106, 50),
	PlayerHistory.new("filthy, brown bones, ", 80, 105, 106, 50),
	PlayerHistory.new("shining white bones, ", 100, 105, 106, 50),
	# Group 106: Skeleton eyes 106->End
	PlayerHistory.new("and glowing eyes.", 30, 106, 0, 50),
	PlayerHistory.new("and eyes which burn with hellfire.", 50, 106, 0, 50),
	PlayerHistory.new("and empty eyesockets.", 100, 106, 0, 50),
	# Group 107: Zombie start 107->108->62->63->64->65->66->End
	PlayerHistory.new("You were created by ", 100, 107, 108, 50),
	# Group 108: Zombie creator 108->62->63->64->65->66->End
	PlayerHistory.new("a Necromancer. ", 30, 108, 62, 50),
	PlayerHistory.new("a Wizard. ", 50, 108, 62, 50),
	PlayerHistory.new("a restless spirit. ", 60, 108, 62, 50),
	PlayerHistory.new("an Evil Priest. ", 70, 108, 62, 50),
	PlayerHistory.new("a pact with the demons. ", 80, 108, 62, 50),
	PlayerHistory.new("a curse. ", 95, 108, 62, 30),
	PlayerHistory.new("an oath. ", 100, 108, 62, 50),
	# Group 109: Cyclops eye 109->110->111->112->End
	PlayerHistory.new("You have a dark brown eye, ", 20, 109, 110, 50),
	PlayerHistory.new("You have a brown eye, ", 60, 109, 110, 50),
	PlayerHistory.new("You have a hazel eye, ", 70, 109, 110, 50),
	PlayerHistory.new("You have a green eye, ", 80, 109, 110, 50),
	PlayerHistory.new("You have a blue eye, ", 90, 109, 110, 50),
	PlayerHistory.new("You have a blue-gray eye, ", 100, 109, 110, 50),
	# Group 110: Cyclops hair style 110->111->112->End
	PlayerHistory.new("straight ", 70, 110, 111, 50),
	PlayerHistory.new("wavy ", 90, 110, 111, 50),
	PlayerHistory.new("curly ", 100, 110, 111, 50),
	# Group 111: Cyclops hair colour 111->112->End
	PlayerHistory.new("black hair, ", 30, 111, 112, 50),
	PlayerHistory.new("brown hair, ", 70, 111, 112, 50),
	PlayerHistory.new("auburn hair, ", 80, 111, 112, 50),
	PlayerHistory.new("red hair, ", 90, 111, 112, 50),
	PlayerHistory.new("blond hair, ", 100, 111, 112, 50),
	# Group 112: Cyclops complexion 112->End
	PlayerHistory.new("and a very dark complexion.", 10, 112, 0, 50),
	PlayerHistory.new("and a dark complexion.", 30, 112, 0, 50),
	PlayerHistory.new("and an olive complexion.", 80, 112, 0, 50),
	PlayerHistory.new("and a pale complexion.", 90, 112, 0, 50),
	PlayerHistory.new("and a very pale complexion.", 100, 112, 0, 50),
	# Group 113: Vampire start 113->114->115->116->117->End
	PlayerHistory.new("You arose from an unmarked grave. ", 20, 113, 114, 50),
	PlayerHistory.new("In life you were a simple peasant, the victim of a powerful Vampire Lord. ", 40, 113, 114, 50),
	PlayerHistory.new("In life you were a Vampire Hunter, but they got you. ", 60, 113, 114, 50),
	PlayerHistory.new("In life you were a Necromancer. ", 80, 113, 114, 50),
	PlayerHistory.new("In life you were a powerful noble. ", 95, 113, 114, 50),
	PlayerHistory.new("In life you were a powerful and cruel tyrant. ", 100, 113, 114, 50),
	# Group 114: Vampire join 114->115->116->117->End
	PlayerHistory.new("You have ", 100, 114, 115, 50),
	# Group 115: Vampire hair 115->116->117->End
	PlayerHistory.new("jet-black hair, ", 25, 115, 116, 50),
	PlayerHistory.new("matted brown hair, ", 50, 115, 116, 50),
	PlayerHistory.new("white hair, ", 75, 115, 116, 50),
	PlayerHistory.new("a hairless head, ", 100, 115, 116, 50),
	# Group 116: Vampire eyes 116->117->End
	PlayerHistory.new("eyes like red coals, ", 25, 116, 117, 50),
	PlayerHistory.new("blank white eyes, ", 50, 116, 117, 50),
	PlayerHistory.new("feral yellow eyes, ", 75, 116, 117, 50),
	PlayerHistory.new("bloodshot red eyes, ", 100, 116, 117, 50),
	# Group 117: Vampire complexion 117->End
	PlayerHistory.new("and a deathly pale complexion.", 100, 117, 0, 50),
	# Group 118: Spectre start 118->119->134->120->121->122->123->End
	PlayerHistory.new("You were created by ", 100, 118, 119, 50),
	# Group 119: Spectre origin 119->134->120->121->122->123->End
	PlayerHistory.new("a Necromancer. ", 30, 119, 134, 50),
	PlayerHistory.new("a magical experiment. ", 50, 119, 134, 50),
	PlayerHistory.new("an Evil Priest. ", 70, 119, 134, 50),
	PlayerHistory.new("a pact with the demons. ", 75, 119, 134, 50),
	PlayerHistory.new("a restless spirit. ", 85, 119, 134, 50),
	PlayerHistory.new("a curse. ", 95, 119, 134, 30),
	PlayerHistory.new("an oath. ", 100, 119, 134, 50),
	# Group 120: Spectre hair 120->121->122->123->End
	PlayerHistory.new("jet-black hair, ", 25, 120, 121, 50),
	PlayerHistory.new("matted brown hair, ", 50, 120, 121, 50),
	PlayerHistory.new("white hair, ", 75, 120, 121, 50),
	PlayerHistory.new("a hairless head, ", 100, 120, 121, 50),
	# Group 121: Spectre eyes 121->122->123->End
	PlayerHistory.new("eyes like red coals, ", 25, 121, 122, 50),
	PlayerHistory.new("blank white eyes, ", 50, 121, 122, 50),
	PlayerHistory.new("feral yellow eyes, ", 75, 121, 122, 50),
	PlayerHistory.new("bloodshot red eyes, ", 100, 121, 122, 50),
	# Group 122: Spectre complexion 122->123->End
	PlayerHistory.new(" and a deathly gray complexion. ", 100, 122, 123, 50),
	# Group 123: Spectre aura 123->End
	PlayerHistory.new("An eerie green aura surrounds you.", 100, 123, 0, 50),
	# Group 124: Sprite start 124->125->126->127->128->End
	PlayerHistory.new("Your parents were ", 100, 124, 125, 50),
	# Group 125: Sprite parents 125->126->127->128->End
	PlayerHistory.new("pixies. ", 20, 125, 126, 35),
	PlayerHistory.new("nixies. ", 30, 125, 126, 25),
	PlayerHistory.new("wood sprites. ", 75, 125, 126, 50),
	PlayerHistory.new("wood spirits. ", 90, 125, 126, 75),
	PlayerHistory.new("noble faerie folk. ", 100, 125, 126, 85),
	# Group 126: Sprite wings 126->127->128->End
	PlayerHistory.new("You have dragonfly wings attached to your back, ", 45, 126, 127, 50),
	PlayerHistory.new("You have butterfly wings attached to your back, ", 90, 126, 127, 50),
	PlayerHistory.new("You have beetle wings attached to your back, ", 100, 126, 127, 50),
	# Group 127: Sprite hair 127->128->End
	PlayerHistory.new("straight blond hair, ", 80, 127, 128, 50),
	PlayerHistory.new("wavy blond hair, ", 100, 127, 128, 50),
	# Group 128: Sprite eyes 128->End
	PlayerHistory.new("blue eyes, and skin the colour of pine.", 25, 128, 0, 50),
	PlayerHistory.new("blue eyes, and skin the colour of ash.", 50, 128, 0, 50),
	PlayerHistory.new("blue eyes, and skin the colour of oak.", 75, 128, 0, 50),
	PlayerHistory.new("blue eyes, and skin the colour of mahogany.", 100, 128, 0, 50),
	# Group 129: Miri-Nigri start 129->130->131->132->133->End
	PlayerHistory.new("You were summoned by a cult. ", 30, 129, 130, 40),
	PlayerHistory.new("You crawled out from a fissure in the ground. ", 50, 129, 130, 50),
	PlayerHistory.new("You were summoned by a lone wizard. ", 60, 129, 130, 60),
	PlayerHistory.new("You squeezed into the world through a crack between dimensions. ", 75, 129, 130, 50),
	PlayerHistory.new("You are the blasphemous crossbreed of unspeakable creatures of chaos. ", 100, 129, 130, 30),
	# Group 130: Miri-Nigri eyes 130->131->132->133->End
	PlayerHistory.new("You have green reptilian eyes, ", 60, 130, 131, 50),
	PlayerHistory.new("You have unblinking eyes, ", 85, 130, 131, 50),
	PlayerHistory.new("You have fathomless black eyes, ", 99, 130, 131, 50),
	PlayerHistory.new("You have altogether too many eyes, ", 100, 130, 131, 55),
	# Group 131: Miri-Nigri skin texture 131->132->133->End
	PlayerHistory.new("slimy ", 10, 131, 132, 50),
	PlayerHistory.new("smooth ", 33, 131, 132, 50),
	PlayerHistory.new("slippery ", 66, 131, 132, 50),
	PlayerHistory.new("oily ", 100, 131, 132, 50),
	# Group 132: Miri-Nigri skin colour 132->133->End
	PlayerHistory.new("green skin, ", 33, 132, 133, 50),
	PlayerHistory.new("black skin, ", 66, 132, 133, 50),
	PlayerHistory.new("pale skin, ", 100, 132, 133, 50),
	# Group 133: Miri-Nigri mutation 133->End
	PlayerHistory.new("and tentacles.", 50, 133, 0, 50),
	PlayerHistory.new("and webbed feet.", 75, 133, 0, 50),
	PlayerHistory.new("and suckers on your fingers.", 85, 133, 0, 50),
	PlayerHistory.new("and no toes.", 90, 133, 0, 50),
	PlayerHistory.new("and no nose.", 95, 133, 0, 50),
	PlayerHistory.new("and no teeth.", 97, 133, 0, 50),
	PlayerHistory.new("and no ears.", 100, 133, 0, 50),
	# Group 134: Spectre join 134->120->121->122->123->End
	PlayerHistory.new("You have ", 100, 134, 120, 50),
	# Group 135: Yeek fur colour 135->136->137->End
	PlayerHistory.new("blue fur, ", 40, 135, 136, 50),
	PlayerHistory.new("brown fur, ", 60, 135, 136, 50),
	PlayerHistory.new("striped fur, ", 95, 135, 136, 50),
	PlayerHistory.new("spotted fur, ", 100, 135, 136, 50),
	# Group 136: Yeek ears 136->137->End
	PlayerHistory.new("rounded ears, ", 10, 136, 137, 50),
	PlayerHistory.new("pointed ears, ", 90, 136, 137, 50),
	PlayerHistory.new("lop ears, ", 100, 136, 137, 50),
	# Group 137: Yeek eyes 137->End
	PlayerHistory.new("and glowing yellow eyes.", 10, 137, 0, 50),
	PlayerHistory.new("and glowing red eyes.", 90, 137, 0, 50),
	PlayerHistory.new("and glowing orange eyes.", 100, 137, 0, 50),
	# Group 138: Tcho-Tcho start 138->139->140->141->142->End
	PlayerHistory.new("You are the only child of ", 10, 138, 139, 50),
	PlayerHistory.new("You are one of the children of ", 90, 138, 139, 50),
	PlayerHistory.new("You don't know who your parents were. ", 100, 138, 140, 10),
	# Group 139: Tcho-Tcho parent 139->140->141->142->End
	PlayerHistory.new("a hunter. ", 40, 139, 140, 20),
	PlayerHistory.new("a warrior. ", 75, 139, 140, 30),
	PlayerHistory.new("a cultist. ", 95, 139, 140, 50),
	PlayerHistory.new("a warlock. ", 100, 139, 140, 60),
	PlayerHistory.new("a high priest. ", 10, 139, 140, 80),
	# Group 140: Tcho-Tcho eyes 140->141->142->End
	PlayerHistory.new("You have deep-set eyes, ", 30, 140, 141, 50),
	PlayerHistory.new("You have a missing eye, ", 40, 140, 141, 50),
	PlayerHistory.new("You have dark eyes, ", 90, 140, 141, 50),
	PlayerHistory.new("You have bloodshot eyes, ", 100, 140, 141, 50),
	# Group 141: Tcho-Tcho body 141->142->End
	PlayerHistory.new("many scars, and ", 30, 141, 142, 50),
	PlayerHistory.new("obscene tattoos, and ", 50, 141, 142, 50),
	PlayerHistory.new("ritual scarification, and ", 70, 141, 142, 50),
	PlayerHistory.new("an extra toe, and ", 82, 141, 142, 50),
	PlayerHistory.new("a vestigial tail, and ", 87, 141, 142, 50),
	PlayerHistory.new("unmistakable signs of inbreeding, and ", 90, 141, 142, 50),
	PlayerHistory.new("a missing nose, and ", 100, 141, 142, 50),
	# Group 142: Tcho-Tcho teeth 142->End
	PlayerHistory.new("missing teeth.", 30, 142, 0, 50),
	PlayerHistory.new("sharpened teeth.", 50, 142, 0, 50),
	PlayerHistory.new("rotten teeth.", 70, 142, 0, 50),
	PlayerHistory.new("filed-down teeth.", 90, 142, 0, 50),
	PlayerHistory.new("no teeth.", 100, 142, 0, 50),
	# Group 143: Shadowling start 143-144->137->End
	PlayerHistory.new("You snuck into this world through an unseen crack. ", 30, 143, 144, 50),
	PlayerHistory.new("You slipped through a summoning circle while no-one was looking. ", 55, 143, 144, 50),
	PlayerHistory.new("You followed a demon that was being summoned. ", 100, 143, 144, 50),
	# Group 144: Shadowling description 144->137->End
	PlayerHistory.new("You have a shadow-black body ", 20, 144, 137, 50),
	PlayerHistory.new("You have an ebon-black body ", 40, 144, 137, 50),
	PlayerHistory.new("You have a void-black body ", 75, 144, 137, 50),
	PlayerHistory.new("You have a midnight-black body ", 85, 144, 137, 50),
	PlayerHistory.new("You have a deep black body ", 100, 144, 137, 50),
	# Group 145: Sand Dweller start 145->146->147->148->149->150->End
	PlayerHistory.new("You are composed of ", 100, 145, 146, 50),
	# Group 146: Sand Dweller texture 146->147->148->149->150->End
	PlayerHistory.new("fine ", 25, 146, 147, 50),
	PlayerHistory.new("rough ", 35, 146, 147, 50),
	PlayerHistory.new("coarse ", 55, 146, 147, 50),
	PlayerHistory.new("smooth ", 75, 146, 147, 50),
	PlayerHistory.new("mixed ", 100, 146, 147, 50),
	# Group 147: Sand Dweller grains 147->148->149->150->End
	PlayerHistory.new("sand. ", 75, 147, 148, 50),
	PlayerHistory.new("silt. ", 90, 147, 148, 50),
	PlayerHistory.new("grit. ", 100, 147, 148, 50),
	# Group 148: Sand Dweller eyes 148->149->150->End
	PlayerHistory.new("You have round black eyes, ", 40, 148, 149, 50),
	PlayerHistory.new("You have glittering black eyes, ", 70, 148, 149, 50),
	PlayerHistory.new("You have large black eyes, ", 100, 148, 149, 50),
	# Group 149: Sand Dweller ears 149->150->End
	PlayerHistory.new("pointed ears, ", 40, 149, 150, 50),
	PlayerHistory.new("rounded ears, ", 70, 149, 150, 50),
	PlayerHistory.new("no ears, ", 100, 149, 150, 50),
	# Group 150: Sand Dweller snout 150->End
	PlayerHistory.new("and a koala-like snout, ", 60, 150, 0, 50),
	PlayerHistory.new("and a dog-like snout, ", 80, 150, 0, 50),
	PlayerHistory.new("and a bear-like snout, ", 100, 150, 0, 50),
]

var bonus: int
var chart: int
var info: String
var next: int
var roll: int


static func get_history(player: Player, current_chart: int) -> String:
	var i: int = 0
	player.history = ["", "", "", ""]
	var full_history: String = ""
	var social_class: int = Rng.roll_die(4)
	# Start on a chart based on the character's race
	# Keep going till we get to the end
	while current_chart != 0:
		i = 0
		# Roll percentile for which background to use within each chart
		var current_roll: int = Rng.roll_die(100)
		# Find the correct chart and background
		while current_chart != _background_table[i].chart or current_roll > _background_table[i].roll:
			i += 1
		# Add the text to our buffer
		full_history += _background_table[i].info
		# Adjust our social class by the bonus or penalty for that fragment
		social_class += (_background_table[i].bonus - 50)
		# Move onto the next chart
		current_chart = _background_table[i].next
	# Make sure our social class didn't go out of bounds
	if social_class > 100:
		social_class = 100
	if social_class < 1:
		social_class = 1
	player.social_class = social_class
	return full_history


func _init(new_info: String, new_roll: int, new_chart: int, new_next: int, new_bonus: int) -> void:
	bonus = new_bonus
	chart = new_chart
	info = new_info
	next = new_next
	roll = new_roll
