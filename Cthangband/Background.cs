// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;

namespace Cthangband
{
    /// <summary>
    /// A static class with functions to create a character's background
    /// </summary>
    internal static class Background
    {
        /// <summary>
        /// Beginnings for 'angelic' names used by imps
        /// </summary>
        private static readonly string[] _angelSyllable1 =
            {"Sa", "A", "U", "Mi", "Ra", "Ana", "Pa", "Lu", "She", "Ga", "Da", "O", "Pe", "Lau", "Za", "Ze", "E"};

        /// <summary>
        /// Middles for 'angelic' names used by imps
        /// </summary>
        private static readonly string[] _angelSyllable2 =
            {"br", "m", "l", "z", "zr", "mm", "mr", "r", "ral", "ch", "zaz", "tr", "n", "lar"};

        /// <summary>
        /// Endings for 'angelic' names used by imps
        /// </summary>
        private static readonly string[] _angelSyllable3 = { "iel", "ial", "ael", "ubim", "aphon", "iel", "ael" };

        /// <summary>
        /// List of backstory fragments joined together on character generation
        /// </summary>
        private static readonly PlayerHistory[] _backgroundTable =
        {
            // Group 1: Human start /Half-Elf legitimacy 1->2->3->50->51->52->53->End
            new PlayerHistory("You are the illegitimate and unacknowledged child ", 10, 1, 2, 25),
            new PlayerHistory("You are the illegitimate but acknowledged child ", 20, 1, 2, 35),
            new PlayerHistory("You are one of several children ", 95, 1, 2, 45),
            new PlayerHistory("You are the first child ", 100, 1, 2, 50),
            // Group 2: Human/Half-Elf/Half-Orc/Half-Ogre/Half-Giant/Half-Titan parent 2->3->50->51->52->53->End
            new PlayerHistory("of a Serf. ", 40, 2, 3, 65),
            new PlayerHistory("of a Yeoman. ", 65, 2, 3, 80),
            new PlayerHistory("of a Townsman. ", 80, 2, 3, 90),
            new PlayerHistory("of a Guildsman. ", 90, 2, 3, 105),
            new PlayerHistory("of a Landed Knight. ", 96, 2, 3, 120),
            new PlayerHistory("of a Noble Family. ", 99, 2, 3, 130),
            new PlayerHistory("of the Royal Blood Line. ", 100, 2, 3, 140),
            // Group 3: Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan
            // childhood 3->50->51->52->53->End
            new PlayerHistory("You are the black sheep of the family. ", 20, 3, 50, 20),
            new PlayerHistory("You are a credit to the family. ", 80, 3, 50, 55),
            new PlayerHistory("You are a well liked child. ", 100, 3, 50, 60),
            // Group 4: Half-Elf start 4->1->2->3->50->51->52->53->End
            new PlayerHistory("Your mother was of the Teleri. ", 40, 4, 1, 50),
            new PlayerHistory("Your father was of the Teleri. ", 75, 4, 1, 55),
            new PlayerHistory("Your mother was of the Noldor. ", 90, 4, 1, 55),
            new PlayerHistory("Your father was of the Noldor. ", 95, 4, 1, 60),
            new PlayerHistory("Your mother was of the Vanyar. ", 98, 4, 1, 65),
            new PlayerHistory("Your father was of the Vanyar. ", 100, 4, 1, 70),
            // Group 7: Elf/High-Elf start 7->8->9->54->55->56->End
            new PlayerHistory("You are one of several children ", 60, 7, 8, 50),
            new PlayerHistory("You are the only child ", 100, 7, 8, 55),
            // Group 8: Elf/High-Elf ancestry 8->9->54->55->56->End
            new PlayerHistory("of a Teleri ", 75, 8, 9, 50),
            new PlayerHistory("of a Noldor ", 95, 8, 9, 55),
            new PlayerHistory("of a Vanyar ", 100, 8, 9, 60),
            // Group 9: Elf/High-Elf parent 9->54->55->56->End
            new PlayerHistory("Ranger. ", 40, 9, 54, 80),
            new PlayerHistory("Archer. ", 70, 9, 54, 90),
            new PlayerHistory("Warrior. ", 87, 9, 54, 110),
            new PlayerHistory("Mage. ", 95, 9, 54, 125),
            new PlayerHistory("Prince. ", 99, 9, 54, 140),
            new PlayerHistory("King. ", 100, 9, 54, 145),
            // Group 10: Hobbit start 10->11->3->50->51->52->53->End
            new PlayerHistory("You are one of several children of a Hobbit ", 85, 10, 11, 45),
            new PlayerHistory("You are the only child of a Hobbit ", 100, 10, 11, 55),
            // Group 11: Hobbit parent 11->3->50->51->52->53->End
            new PlayerHistory("Bum. ", 20, 11, 3, 55),
            new PlayerHistory("Tavern Owner. ", 30, 11, 3, 80),
            new PlayerHistory("Miller. ", 40, 11, 3, 90),
            new PlayerHistory("Home Owner. ", 50, 11, 3, 100),
            new PlayerHistory("Burglar. ", 80, 11, 3, 110),
            new PlayerHistory("Warrior. ", 95, 11, 3, 115),
            new PlayerHistory("Mage. ", 99, 11, 3, 125),
            new PlayerHistory("Clan Elder. ", 100, 11, 3, 140),
            // Group 13: Gnome start 13->14->3->50->51->52->53->End
            new PlayerHistory("You are one of several children of a Gnome ", 85, 13, 14, 45),
            new PlayerHistory("You are the only child of a Gnome ", 100, 13, 14, 55),
            // Group 14: Gnome parent 14->3->50->51->52->53->End
            new PlayerHistory("Beggar. ", 20, 14, 3, 55),
            new PlayerHistory("Braggart. ", 50, 14, 3, 70),
            new PlayerHistory("Prankster. ", 75, 14, 3, 85),
            new PlayerHistory("Warrior. ", 95, 14, 3, 100),
            new PlayerHistory("Mage. ", 100, 14, 3, 125),
            // Group 16: Dwarf start 16->17->18->57->58->59->60->61->End
            new PlayerHistory("You are one of two children of a Dwarven ", 25, 16, 17, 40),
            new PlayerHistory("You are the only child of a Dwarven ", 100, 16, 17, 50),
            // Group 17: Dwarf parent 17->18->57->58->59->60->61->End
            new PlayerHistory("Thief. ", 10, 17, 18, 60),
            new PlayerHistory("Prison Guard. ", 25, 17, 18, 75),
            new PlayerHistory("Miner. ", 75, 17, 18, 90),
            new PlayerHistory("Warrior. ", 90, 17, 18, 110),
            new PlayerHistory("Priest. ", 99, 17, 18, 130),
            new PlayerHistory("King. ", 100, 17, 18, 150),
            // Group 18: Dwarf/Nibelung childhood 18->57->58->59->60->61->End
            new PlayerHistory("You are the black sheep of the family. ", 15, 18, 57, 10),
            new PlayerHistory("You are a credit to the family. ", 85, 18, 57, 50),
            new PlayerHistory("You are a well liked child. ", 100, 18, 57, 55),
            // Group 19: Half-Orc start 19->20->2->3->50->51->52->53->End
            new PlayerHistory("Your mother was an Orc, but it is unacknowledged. ", 25, 19, 20, 25),
            new PlayerHistory("Your mother was an Orc. ", 50, 19, 20, 25),
            new PlayerHistory("Your father was an Orc. ", 75, 19, 20, 25),
            new PlayerHistory("Your father was an Orc, but it is unacknowledged. ", 100, 19, 20, 25),
            // Group 20: Half-Orc/Half-Ogre/Half-Giant/Half-Titan adoption 20->2->3->50->51->52->53->End
            new PlayerHistory("You are the child ", 80, 20, 2, 50),
            new PlayerHistory("You are the adopted child ", 100, 20, 2, 50),
            // Group 22: Half-Troll start 22->23->62->63->64->65->66->End
            new PlayerHistory("Your mother was a Cave-Troll ", 30, 22, 23, 20),
            new PlayerHistory("Your father was a Cave-Troll ", 60, 22, 23, 25),
            new PlayerHistory("Your mother was a Hill-Troll ", 75, 22, 23, 30),
            new PlayerHistory("Your father was a Hill-Troll ", 90, 22, 23, 35),
            new PlayerHistory("Your mother was a Water-Troll ", 95, 22, 23, 40),
            new PlayerHistory("Your father was a Water-Troll ", 100, 22, 23, 45),
            // Group 23: Half-Troll parent 23->62->63->64->65->66->End
            new PlayerHistory("Cook. ", 5, 23, 62, 60),
            new PlayerHistory("Warrior. ", 95, 23, 62, 55),
            new PlayerHistory("Priest. ", 99, 23, 62, 65),
            new PlayerHistory("Clan Chief. ", 100, 23, 62, 80),
            // Group 24: Kobold scales 24->25->26->End
            new PlayerHistory("You have green scales, ", 25, 24, 25, 50),
            new PlayerHistory("You have dark green scales, ", 50, 24, 25, 50),
            new PlayerHistory("You have yellow scales, ", 75, 24, 25, 50),
            new PlayerHistory("You have green scales, a yellow belly, ", 100, 24, 25, 50),
            // Group 25: Kobold eyes 25->26->End
            new PlayerHistory("bright eyes, ", 25, 25, 26, 50),
            new PlayerHistory("yellow eyes, ", 50, 25, 26, 50),
            new PlayerHistory("red eyes, ", 75, 25, 26, 50),
            new PlayerHistory("snake-like eyes, ", 100, 25, 26, 50),
            // Group 26: Kobold tail 26->End
            new PlayerHistory("and a long sinuous tail.", 20, 26, 0, 50),
            new PlayerHistory("and a short tail.", 40, 26, 0, 50),
            new PlayerHistory("and a muscular tail.", 60, 26, 0, 50),
            new PlayerHistory("and a long tail.", 80, 26, 0, 50),
            new PlayerHistory("and a sinuous tail.", 100, 26, 0, 50),
            // Group 50: Great
            // One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan eyes 50->51->52->53->End
            new PlayerHistory("You have dark brown eyes, ", 20, 50, 51, 50),
            new PlayerHistory("You have brown eyes, ", 60, 50, 51, 50),
            new PlayerHistory("You have hazel eyes, ", 70, 50, 51, 50),
            new PlayerHistory("You have green eyes, ", 80, 50, 51, 50),
            new PlayerHistory("You have blue eyes, ", 90, 50, 51, 50),
            new PlayerHistory("You have blue-gray eyes, ", 100, 50, 51, 50),
            // Group 51: Great
            // One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan hairstyle 51->52->53->End
            new PlayerHistory("straight ", 70, 51, 52, 50),
            new PlayerHistory("wavy ", 90, 51, 52, 50),
            new PlayerHistory("curly ", 100, 51, 52, 50),
            // Group 52: Great
            // One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan hair colour 52->53->End
            new PlayerHistory("black hair, ", 30, 52, 53, 50),
            new PlayerHistory("brown hair, ", 70, 52, 53, 50),
            new PlayerHistory("auburn hair, ", 80, 52, 53, 50),
            new PlayerHistory("red hair, ", 90, 52, 53, 50),
            new PlayerHistory("blond hair, ", 100, 52, 53, 50),
            // Group 53: Great
            // One/Human/Half-Elf/Hobbit/Gnome/Half-Orc/Half-Ogre/Half-Giant/Half-Titan complexion 53->End
            new PlayerHistory("and a very dark complexion.", 10, 53, 0, 50),
            new PlayerHistory("and a dark complexion.", 30, 53, 0, 50),
            new PlayerHistory("and an olive complexion.", 80, 53, 0, 50),
            new PlayerHistory("and a pale complexion.", 90, 53, 0, 50),
            new PlayerHistory("and a very pale complexion.", 100, 53, 0, 50),
            // Group 54: Elf/High-Elf eyes 54->55->56->End
            new PlayerHistory("You have light grey eyes, ", 85, 54, 55, 50),
            new PlayerHistory("You have light blue eyes, ", 95, 54, 55, 50),
            new PlayerHistory("You have light green eyes, ", 100, 54, 55, 50),
            // Group 55: Elf/High-Elf hairstyle 55->56->End
            new PlayerHistory("straight ", 75, 55, 56, 50),
            new PlayerHistory("wavy ", 100, 55, 56, 50),
            // Group 56: Elf/High-Elf colour 56->End
            new PlayerHistory("black hair, and a pale complexion.", 75, 56, 0, 50),
            new PlayerHistory("brown hair, and a pale complexion.", 85, 56, 0, 50),
            new PlayerHistory("blond hair, and a pale complexion.", 95, 56, 0, 50),
            new PlayerHistory("silver hair, and a pale complexion.", 100, 56, 0, 50),
            // Group 57: Dwarf/Nibelung eyes 57->58->59->60->61->End
            new PlayerHistory("You have dark brown eyes, ", 99, 57, 58, 50),
            new PlayerHistory("You have glowing red eyes, ", 100, 57, 58, 60),
            // Group 58: Dwarf/Nibelung hairstyle 58->59->60->61->End
            new PlayerHistory("straight ", 90, 58, 59, 50),
            new PlayerHistory("wavy ", 100, 58, 59, 50),
            // Group 59: Dwarf/Nibelung hair colour 59->60->61->End
            new PlayerHistory("black hair, ", 75, 59, 60, 50),
            new PlayerHistory("brown hair, ", 100, 59, 60, 50),
            // Group 60: Dwarf/Nibelung beard 60->61->End
            new PlayerHistory("a one foot beard, ", 25, 60, 61, 50),
            new PlayerHistory("a two foot beard, ", 60, 60, 61, 51),
            new PlayerHistory("a three foot beard, ", 90, 60, 61, 53),
            new PlayerHistory("a four foot beard, ", 100, 60, 61, 55),
            // Group 61: Dwarf/Nibelung complexion 61->End
            new PlayerHistory("and a dark complexion.", 100, 61, 0, 50),
            // Group 62: Half-Troll/Zombie eyes 62->63->64->65->66->End
            new PlayerHistory("You have slime green eyes, ", 60, 62, 63, 50),
            new PlayerHistory("You have puke yellow eyes, ", 85, 62, 63, 50),
            new PlayerHistory("You have blue-bloodshot eyes, ", 99, 62, 63, 50),
            new PlayerHistory("You have glowing red eyes, ", 100, 62, 63, 55),
            // Group 63: Half-Troll/Zombie hairstyle 63->64->65->66->End
            new PlayerHistory("dirty ", 33, 63, 64, 50),
            new PlayerHistory("mangy ", 66, 63, 64, 50),
            new PlayerHistory("oily ", 100, 63, 64, 50),
            // Group 64: Half-Troll/Zombie hair 64->65->66->End
            new PlayerHistory("sea-weed green hair, ", 33, 64, 65, 50),
            new PlayerHistory("bright red hair, ", 66, 64, 65, 50),
            new PlayerHistory("dark purple hair, ", 100, 64, 65, 50),
            // Group 65: Half-Troll/Zombie skin colour 65->66->End
            new PlayerHistory("and green ", 25, 65, 66, 50),
            new PlayerHistory("and blue ", 50, 65, 66, 50),
            new PlayerHistory("and white ", 75, 65, 66, 50),
            new PlayerHistory("and black ", 100, 65, 66, 50),
            // Group 66: Half-Troll/Zombie skin texture 66->End
            new PlayerHistory("ulcerous skin.", 33, 66, 0, 50),
            new PlayerHistory("scabby skin.", 66, 66, 0, 50),
            new PlayerHistory("leprous skin.", 100, 66, 0, 50),
            // Group 67: Great One start 67->68->50->51->52->53->End
            new PlayerHistory("You are an unacknowledged child of ", 50, 67, 68, 45),
            new PlayerHistory("You are a rebel child of ", 80, 67, 68, 65),
            new PlayerHistory("You are a long lost child of ", 100, 67, 68, 55),
            // Group 68: Great One parent 68->50->51->52->53->End
            new PlayerHistory("someone with Great One blood. ", 50, 68, 50, 80),
            new PlayerHistory("an unknown child of a Great One. ", 65, 68, 50, 90),
            new PlayerHistory("an unknown Great One. ", 79, 68, 50, 100),
            new PlayerHistory("Karakal. ", 80, 68, 50, 130),
            new PlayerHistory("Hagarg Ryonis. ", 83, 68, 50, 105),
            new PlayerHistory("Lobon. ", 84, 68, 50, 105),
            new PlayerHistory("Nath-Horthath. ", 85, 68, 50, 90),
            new PlayerHistory("Tamash. ", 87, 68, 50, 100),
            new PlayerHistory("Zo-Kalar. ", 88, 68, 50, 125),
            new PlayerHistory("Karakal. ", 89, 68, 50, 120),
            new PlayerHistory("Hagarg Ryonis. ", 90, 68, 50, 140),
            new PlayerHistory("Lobon. ", 91, 68, 50, 115),
            new PlayerHistory("Nath-Horthath. ", 92, 68, 50, 110),
            new PlayerHistory("Tamash. ", 93, 68, 50, 105),
            new PlayerHistory("Zo-Kalar. ", 94, 68, 50, 95),
            new PlayerHistory("Karakal. ", 95, 68, 50, 115),
            new PlayerHistory("Hagarg Ryonis. ", 96, 68, 50, 110),
            new PlayerHistory("Lobon. ", 97, 68, 50, 135),
            new PlayerHistory("Nath-Horthath. ", 98, 68, 50, 90),
            new PlayerHistory("Tamash. ", 99, 68, 50, 105),
            new PlayerHistory("Zo-Kalar. ", 100, 68, 50, 80),
            // Group 69: Dark-Elf start 68->70->71->72->73->End
            new PlayerHistory("You are one of several children of a Dark Elven ", 85, 69, 70, 45),
            new PlayerHistory("You are the only child of a Dark Elven ", 100, 69, 70, 55),
            // Group 70: Dark-Elf parent 70->71->72->73->End
            new PlayerHistory("Warrior. ", 50, 70, 71, 60),
            new PlayerHistory("Warlock. ", 80, 70, 71, 75),
            new PlayerHistory("Noble. ", 100, 70, 71, 95),
            // Group 71: Dark-Elf eyes 71->72->73->End
            new PlayerHistory("You have black eyes, ", 100, 71, 72, 50),
            // Group 72: Dark-Elf hair style 72->73->End
            new PlayerHistory("straight ", 70, 72, 73, 50),
            new PlayerHistory("wavy ", 90, 72, 73, 50),
            new PlayerHistory("curly ", 100, 72, 73, 50),
            // Group 73: Dark-Elf complexion 73->End
            new PlayerHistory("black hair and a very dark complexion.", 100, 73, 0, 50),
            // Group 74: Half-Ogre start 74->20->2->3->50->51->52->53->End
            new PlayerHistory("Your mother was an Ogre, but it is unacknowledged. ", 25, 74, 20, 25),
            new PlayerHistory("Your father was an Ogre. ", 50, 74, 20, 25),
            new PlayerHistory("Your mother was an Ogre. ", 75, 74, 20, 25),
            new PlayerHistory("Your father was an Ogre, but it is unacknowledged. ", 100, 74, 20, 25),
            // Group 75: Half-Giant start 75->20->2->3->50->51->52->53->End
            new PlayerHistory("Your mother was a Hill Giant. ", 10, 75, 20, 50),
            new PlayerHistory("Your mother was a Fire Giant. ", 12, 75, 20, 55),
            new PlayerHistory("Your mother was a Frost Giant. ", 20, 75, 20, 60),
            new PlayerHistory("Your mother was a Cloud Giant. ", 23, 75, 20, 65),
            new PlayerHistory("Your mother was a Storm Giant. ", 25, 75, 20, 70),
            new PlayerHistory("Your father was a Hill Giant. ", 60, 75, 20, 50),
            new PlayerHistory("Your father was a Fire Giant. ", 70, 75, 20, 55),
            new PlayerHistory("Your father was a Frost Giant. ", 80, 75, 20, 60),
            new PlayerHistory("Your father was a Cloud Giant. ", 90, 75, 20, 65),
            new PlayerHistory("Your father was a Storm Giant. ", 100, 75, 20, 70),
            // Group 76: Half-Titan start 75->20->2->3->50->51->52->53->End
            new PlayerHistory("Your father was an unknown Titan. ", 75, 76, 20, 50),
            new PlayerHistory("Your mother was Themis. ", 80, 76, 20, 100),
            new PlayerHistory("Your mother was Mnemosyne. ", 85, 76, 20, 100),
            new PlayerHistory("Your father was Okeanoas. ", 90, 76, 20, 100),
            new PlayerHistory("Your father was Crius. ", 95, 76, 20, 100),
            new PlayerHistory("Your father was Hyperion. ", 98, 76, 20, 125),
            new PlayerHistory("Your father was Kronos. ", 100, 76, 20, 150),
            // Group 77: Cyclops start 77->109->110->111->112->End
            new PlayerHistory("You are the offspring of an unknown Cyclops. ", 90, 77, 109, 50),
            new PlayerHistory("You are Polyphemos's child. ", 98, 77, 109, 80),
            new PlayerHistory("You are Uranos's child. ", 100, 77, 109, 135),
            // Group 78: Yeek start 78->79->80->81->135->136->137->End
            new PlayerHistory("You are the runt of ", 20, 78, 79, 40),
            new PlayerHistory("You come from ", 80, 78, 79, 50),
            new PlayerHistory("You are the largest of ", 100, 78, 79, 55),
            // Group 79: Yeek litter size 79->80->81->135->136->137->End
            new PlayerHistory("a litter of 3 pups. ", 15, 79, 80, 55),
            new PlayerHistory("a litter of 4 pups. ", 40, 79, 80, 55),
            new PlayerHistory("a litter of 5 pups. ", 70, 79, 80, 50),
            new PlayerHistory("a litter of 6 pups. ", 85, 79, 80, 50),
            new PlayerHistory("a litter of 7 pups. ", 95, 79, 80, 45),
            new PlayerHistory("a litter of 8 pups. ", 100, 79, 80, 45),
            // Group 80: Yeek parent 80->81->135->136->137->End
            new PlayerHistory("Your mother was a scavenger. ", 25, 80, 81, 40),
            new PlayerHistory("Your mother was a sneak. ", 50, 80, 81, 45),
            new PlayerHistory("Your mother was a warrior. ", 75, 80, 81, 50),
            new PlayerHistory("Your mother was a master yeek. ", 95, 80, 81, 55),
            new PlayerHistory("Your father was a yeek king. ", 100, 80, 81, 60),
            // Group 81: Yeek fur style 81->135->136->137->End
            new PlayerHistory("You have mangy ", 33, 81, 135, 50),
            new PlayerHistory("You have short ", 66, 81, 135, 50),
            new PlayerHistory("You have long ", 100, 81, 135, 50),
            // Group 82: Kobold start 82->83->24->25->26->End
            new PlayerHistory("You are one of several children of ", 100, 82, 83, 50),
            // Group 83: Kobold parent 83->24->25->26->End
            new PlayerHistory("a Small Kobold. ", 40, 83, 24, 50),
            new PlayerHistory("a Kobold. ", 75, 83, 24, 55),
            new PlayerHistory("a Large Kobold. ", 95, 83, 24, 65),
            new PlayerHistory("Vort, the Kobold Queen. ", 100, 83, 24, 100),
            // Group 84: Klackon start 84->85->86->End
            new PlayerHistory("You are one of several children of a Klackon hive queen. ", 100, 84, 85, 50),
            // Group 85: Klackon skin 85->86->End
            new PlayerHistory("You have red chitin, ", 40, 85, 86, 50),
            new PlayerHistory("You have black chitin, ", 90, 85, 86, 50),
            new PlayerHistory("You have yellow chitin, ", 100, 85, 86, 50),
            // Group 86: Klackon antennae 86->End
            new PlayerHistory("long antennae, and black eyes.", 50, 86, 0, 50),
            new PlayerHistory("short antennae, and black eyes.", 80, 86, 0, 50),
            new PlayerHistory("feathered antennae, and black eyes.", 100, 86, 0, 50),
            // Group 87: Nibelung start 87->88->18->57->58->59->60->61->End
            new PlayerHistory("You are one of several children of ", 100, 87, 88, 89),
            // Group 88: Nibelung parent 88->18->57->58->59->60->61->End
            new PlayerHistory("a Nibelung Slave. ", 30, 88, 18, 20),
            new PlayerHistory("a Nibelung Thief. ", 50, 88, 18, 40),
            new PlayerHistory("a Nibelung Smith. ", 70, 88, 18, 60),
            new PlayerHistory("a Nibelung Miner. ", 90, 88, 18, 75),
            new PlayerHistory("a Nibelung Priest. ", 95, 88, 18, 100),
            new PlayerHistory("Mime, the Nibelung. ", 100, 88, 18, 100),
            // Group 89: Draconian start 89->90->91->End
            new PlayerHistory("You are one of several children of a Draconian ", 85, 89, 90, 50),
            new PlayerHistory("You are the only child of a Draconian ", 100, 89, 90, 55),
            // Group 90: Draconian parent 90->91->End
            new PlayerHistory("Warrior. ", 50, 90, 91, 50),
            new PlayerHistory("Priest. ", 65, 90, 91, 65),
            new PlayerHistory("Mage. ", 85, 90, 91, 70),
            new PlayerHistory("Noble. ", 100, 90, 91, 100),
            // Group 91: Draconian colour 91->End
            new PlayerHistory("You have green wings, green skin and yellow belly.", 30, 91, 0, 50),
            new PlayerHistory("You have green wings, and green skin.", 55, 91, 0, 50),
            new PlayerHistory("You have red wings, and red skin.", 80, 91, 0, 50),
            new PlayerHistory("You have black wings, and black skin.", 90, 91, 0, 50),
            new PlayerHistory("You have metallic skin, and shining wings.", 100, 91, 0, 50),
            // Group 92: Mind-Flayer start 93->93->End
            new PlayerHistory("You have slimy skin, empty glowing eyes, and ", 100, 92, 93, 80),
            // Group 93: Mind-Flayer tentacles 93->End
            new PlayerHistory("three tentacles around your mouth.", 20, 93, 0, 45),
            new PlayerHistory("four tentacles around your mouth.", 80, 93, 0, 50),
            new PlayerHistory("five tentacles around your mouth.", 100, 93, 0, 55),
            // Group 94: Imp start 94->95->96->97->End
            new PlayerHistory("You ancestor was ", 100, 94, 95, 50),
            // Group 95: Imp ancestor 95->96->97->End
            new PlayerHistory("a mindless demonic spawn. ", 30, 95, 96, 20),
            new PlayerHistory("a minor demon. ", 60, 95, 96, 50),
            new PlayerHistory("a major demon. ", 90, 95, 96, 75),
            new PlayerHistory("a demon lord. ", 100, 95, 96, 99),
            // Group 96: Imp skin 96->97->End
            new PlayerHistory("You have red skin, ", 50, 96, 97, 50),
            new PlayerHistory("You have brown skin, ", 100, 96, 97, 50),
            // Group 97: Imp eyes 97->End
            new PlayerHistory("claws, fangs, spikes, and glowing red eyes.", 40, 97, 0, 50),
            new PlayerHistory("claws, fangs, and glowing red eyes.", 70, 97, 0, 50),
            new PlayerHistory("claws, and glowing red eyes.", 100, 97, 0, 50),
            // Group 98: Golem start 98->99->100->101->End
            new PlayerHistory("You were shaped from ", 100, 98, 99, 50),
            // Group 99: Golem material 99->100->101->End
            new PlayerHistory("clay ", 40, 99, 100, 50),
            new PlayerHistory("stone ", 80, 99, 100, 50),
            new PlayerHistory("wood ", 85, 99, 100, 40),
            new PlayerHistory("iron ", 99, 99, 100, 50),
            new PlayerHistory("pure gold ", 100, 99, 100, 100),
            // Group 100: Golem creator 100->101->End
            new PlayerHistory("by a Kabbalist", 40, 100, 101, 50),
            new PlayerHistory("by a Wizard", 65, 100, 101, 50),
            new PlayerHistory("by an Alchemist", 90, 100, 101, 50),
            new PlayerHistory("by a Priest", 100, 100, 101, 60),
            // Group 101: Golem purpose 101->End
            new PlayerHistory(" to fight evil.", 10, 101, 0, 65),
            new PlayerHistory(".", 100, 101, 0, 50),
            // Group 102: Skeleton start 102->103->104->105->106->End
            new PlayerHistory("You were created by ", 100, 102, 103, 50),
            // Group 103: Skeleton creator 103->104->105->106->End
            new PlayerHistory("a Necromancer. ", 30, 103, 104, 50),
            new PlayerHistory("a magical experiment. ", 50, 103, 104, 50),
            new PlayerHistory("an Evil Priest. ", 70, 103, 104, 50),
            new PlayerHistory("a pact with the demons. ", 75, 103, 104, 50),
            new PlayerHistory("a restless spirit. ", 85, 103, 104, 50),
            new PlayerHistory("a curse. ", 95, 103, 104, 30),
            new PlayerHistory("an oath. ", 100, 103, 104, 50),
            // Group 104: Skeleton join 104->105->106->End
            new PlayerHistory("You have ", 100, 104, 105, 50),
            // Group 105: Skeleton bones 105->106->End
            new PlayerHistory("dirty, dry bones, ", 40, 105, 106, 50),
            new PlayerHistory("rotten black bones, ", 60, 105, 106, 50),
            new PlayerHistory("filthy, brown bones, ", 80, 105, 106, 50),
            new PlayerHistory("shining white bones, ", 100, 105, 106, 50),
            // Group 106: Skeleton eyes 106->End
            new PlayerHistory("and glowing eyes.", 30, 106, 0, 50),
            new PlayerHistory("and eyes which burn with hellfire.", 50, 106, 0, 50),
            new PlayerHistory("and empty eyesockets.", 100, 106, 0, 50),
            // Group 107: Zombie start 107->108->62->63->64->65->66->End
            new PlayerHistory("You were created by ", 100, 107, 108, 50),
            // Group 108: Zombie creator 108->62->63->64->65->66->End
            new PlayerHistory("a Necromancer. ", 30, 108, 62, 50),
            new PlayerHistory("a Wizard. ", 50, 108, 62, 50),
            new PlayerHistory("a restless spirit. ", 60, 108, 62, 50),
            new PlayerHistory("an Evil Priest. ", 70, 108, 62, 50),
            new PlayerHistory("a pact with the demons. ", 80, 108, 62, 50),
            new PlayerHistory("a curse. ", 95, 108, 62, 30),
            new PlayerHistory("an oath. ", 100, 108, 62, 50),
            // Group 109: Cyclops eye 109->110->111->112->End
            new PlayerHistory("You have a dark brown eye, ", 20, 109, 110, 50),
            new PlayerHistory("You have a brown eye, ", 60, 109, 110, 50),
            new PlayerHistory("You have a hazel eye, ", 70, 109, 110, 50),
            new PlayerHistory("You have a green eye, ", 80, 109, 110, 50),
            new PlayerHistory("You have a blue eye, ", 90, 109, 110, 50),
            new PlayerHistory("You have a blue-gray eye, ", 100, 109, 110, 50),
            // Group 110: Cyclops hair style 110->111->112->End
            new PlayerHistory("straight ", 70, 110, 111, 50),
            new PlayerHistory("wavy ", 90, 110, 111, 50),
            new PlayerHistory("curly ", 100, 110, 111, 50),
            // Group 111: Cyclops hair colour 111->112->End
            new PlayerHistory("black hair, ", 30, 111, 112, 50),
            new PlayerHistory("brown hair, ", 70, 111, 112, 50),
            new PlayerHistory("auburn hair, ", 80, 111, 112, 50),
            new PlayerHistory("red hair, ", 90, 111, 112, 50),
            new PlayerHistory("blond hair, ", 100, 111, 112, 50),
            // Group 112: Cyclops complexion 112->End
            new PlayerHistory("and a very dark complexion.", 10, 112, 0, 50),
            new PlayerHistory("and a dark complexion.", 30, 112, 0, 50),
            new PlayerHistory("and an olive complexion.", 80, 112, 0, 50),
            new PlayerHistory("and a pale complexion.", 90, 112, 0, 50),
            new PlayerHistory("and a very pale complexion.", 100, 112, 0, 50),
            // Group 113: Vampire start 113->114->115->116->117->End
            new PlayerHistory("You arose from an unmarked grave. ", 20, 113, 114, 50),
            new PlayerHistory("In life you were a simple peasant, the victim of a powerful Vampire Lord. ", 40, 113, 114, 50),
            new PlayerHistory("In life you were a Vampire Hunter, but they got you. ", 60, 113, 114, 50),
            new PlayerHistory("In life you were a Necromancer. ", 80, 113, 114, 50),
            new PlayerHistory("In life you were a powerful noble. ", 95, 113, 114, 50),
            new PlayerHistory("In life you were a powerful and cruel tyrant. ", 100, 113, 114, 50),
            // Group 114: Vampire join 114->115->116->117->End
            new PlayerHistory("You have ", 100, 114, 115, 50),
            // Group 115: Vampire hair 115->116->117->End
            new PlayerHistory("jet-black hair, ", 25, 115, 116, 50),
            new PlayerHistory("matted brown hair, ", 50, 115, 116, 50),
            new PlayerHistory("white hair, ", 75, 115, 116, 50),
            new PlayerHistory("a hairless head, ", 100, 115, 116, 50),
            // Group 116: Vampire eyes 116->117->End
            new PlayerHistory("eyes like red coals, ", 25, 116, 117, 50),
            new PlayerHistory("blank white eyes, ", 50, 116, 117, 50),
            new PlayerHistory("feral yellow eyes, ", 75, 116, 117, 50),
            new PlayerHistory("bloodshot red eyes, ", 100, 116, 117, 50),
            // Group 117: Vampire complexion 117->End
            new PlayerHistory("and a deathly pale complexion.", 100, 117, 0, 50),
            // Group 118: Spectre start 118->119->134->120->121->122->123->End
            new PlayerHistory("You were created by ", 100, 118, 119, 50),
            // Group 119: Spectre origin 119->134->120->121->122->123->End
            new PlayerHistory("a Necromancer. ", 30, 119, 134, 50),
            new PlayerHistory("a magical experiment. ", 50, 119, 134, 50),
            new PlayerHistory("an Evil Priest. ", 70, 119, 134, 50),
            new PlayerHistory("a pact with the demons. ", 75, 119, 134, 50),
            new PlayerHistory("a restless spirit. ", 85, 119, 134, 50),
            new PlayerHistory("a curse. ", 95, 119, 134, 30),
            new PlayerHistory("an oath. ", 100, 119, 134, 50),
            // Group 120: Spectre hair 120->121->122->123->End
            new PlayerHistory("jet-black hair, ", 25, 120, 121, 50),
            new PlayerHistory("matted brown hair, ", 50, 120, 121, 50),
            new PlayerHistory("white hair, ", 75, 120, 121, 50),
            new PlayerHistory("a hairless head, ", 100, 120, 121, 50),
            // Group 121: Spectre eyes 121->122->123->End
            new PlayerHistory("eyes like red coals, ", 25, 121, 122, 50),
            new PlayerHistory("blank white eyes, ", 50, 121, 122, 50),
            new PlayerHistory("feral yellow eyes, ", 75, 121, 122, 50),
            new PlayerHistory("bloodshot red eyes, ", 100, 121, 122, 50),
            // Group 122: Spectre complexion 122->123->End
            new PlayerHistory(" and a deathly gray complexion. ", 100, 122, 123, 50),
            // Group 123: Spectre aura 123->End
            new PlayerHistory("An eerie green aura surrounds you.", 100, 123, 0, 50),
            // Group 124: Sprite start 124->125->126->127->128->End
            new PlayerHistory("Your parents were ", 100, 124, 125, 50),
            // Group 125: Sprite parents 125->126->127->128->End
            new PlayerHistory("pixies. ", 20, 125, 126, 35),
            new PlayerHistory("nixies. ", 30, 125, 126, 25),
            new PlayerHistory("wood sprites. ", 75, 125, 126, 50),
            new PlayerHistory("wood spirits. ", 90, 125, 126, 75),
            new PlayerHistory("noble faerie folk. ", 100, 125, 126, 85),
            // Group 126: Sprite wings 126->127->128->End
            new PlayerHistory("You have dragonfly wings attached to your back, ", 45, 126, 127, 50),
            new PlayerHistory("You have butterfly wings attached to your back, ", 90, 126, 127, 50),
            new PlayerHistory("You have beetle wings attached to your back, ", 100, 126, 127, 50),
            // Group 127: Sprite hair 127->128->End
            new PlayerHistory("straight blond hair, ", 80, 127, 128, 50),
            new PlayerHistory("wavy blond hair, ", 100, 127, 128, 50),
            // Group 128: Sprite eyes 128->End
            new PlayerHistory("blue eyes, and skin the colour of pine.", 25, 128, 0, 50),
            new PlayerHistory("blue eyes, and skin the colour of ash.", 50, 128, 0, 50),
            new PlayerHistory("blue eyes, and skin the colour of oak.", 75, 128, 0, 50),
            new PlayerHistory("blue eyes, and skin the colour of mahogany.", 100, 128, 0, 50),
            // Group 129: Miri-Nigri start 129->130->131->132->133->End
            new PlayerHistory("You were summoned by a cult. ", 30, 129, 130, 40),
            new PlayerHistory("You crawled out from a fissure in the ground. ", 50, 129, 130, 50),
            new PlayerHistory("You were summoned by a lone wizard. ", 60, 129, 130, 60),
            new PlayerHistory("You squeezed into the world through a crack between dimensions. ", 75, 129, 130, 50),
            new PlayerHistory("You are the blasphemous crossbreed of unspeakable creatures of chaos. ", 100, 129, 130, 30),
            // Group 130: Miri-Nigri eyes 130->131->132->133->End
            new PlayerHistory("You have green reptilian eyes, ", 60, 130, 131, 50),
            new PlayerHistory("You have unblinking eyes, ", 85, 130, 131, 50),
            new PlayerHistory("You have fathomless black eyes, ", 99, 130, 131, 50),
            new PlayerHistory("You have altogether too many eyes, ", 100, 130, 131, 55),
            // Group 131: Miri-Nigri skin texture 131->132->133->End
            new PlayerHistory("slimy ", 10, 131, 132, 50),
            new PlayerHistory("smooth ", 33, 131, 132, 50),
            new PlayerHistory("slippery ", 66, 131, 132, 50),
            new PlayerHistory("oily ", 100, 131, 132, 50),
            // Group 132: Miri-Nigri skin colour 132->133->End
            new PlayerHistory("green skin, ", 33, 132, 133, 50),
            new PlayerHistory("black skin, ", 66, 132, 133, 50),
            new PlayerHistory("pale skin, ", 100, 132, 133, 50),
            // Group 133: Miri-Nigri mutation 133->End
            new PlayerHistory("and tentacles.", 50, 133, 0, 50),
            new PlayerHistory("and webbed feet.", 75, 133, 0, 50),
            new PlayerHistory("and suckers on your fingers.", 85, 133, 0, 50),
            new PlayerHistory("and no toes.", 90, 133, 0, 50),
            new PlayerHistory("and no nose.", 95, 133, 0, 50),
            new PlayerHistory("and no teeth.", 97, 133, 0, 50),
            new PlayerHistory("and no ears.", 100, 133, 0, 50),
            // Group 134: Spectre join 134->120->121->122->123->End
            new PlayerHistory("You have ", 100, 134, 120, 50),
            // Group 135: Yeek fur colour 135->136->137->End
            new PlayerHistory("blue fur, ", 40, 135, 136, 50),
            new PlayerHistory("brown fur, ", 60, 135, 136, 50),
            new PlayerHistory("striped fur, ", 95, 135, 136, 50),
            new PlayerHistory("spotted fur, ", 100, 135, 136, 50),
            // Group 136: Yeek ears 136->137->End
            new PlayerHistory("rounded ears, ", 10, 136, 137, 50),
            new PlayerHistory("pointed ears, ", 90, 136, 137, 50),
            new PlayerHistory("lop ears, ", 100, 136, 137, 50),
            // Group 137: Yeek eyes 137->End
            new PlayerHistory("and glowing yellow eyes.", 10, 137, 0, 50),
            new PlayerHistory("and glowing red eyes.", 90, 137, 0, 50),
            new PlayerHistory("and glowing orange eyes.", 100, 137, 0, 50),
            // Group 138: Tcho-Tcho start 138->139->140->141->142->End
            new PlayerHistory("You are the only child of ", 10, 138, 139, 50),
            new PlayerHistory("You are one of the children of ", 90, 138, 139, 50),
            new PlayerHistory("You don't know who your parents were. ", 100, 138, 140, 10),
            // Group 139: Tcho-Tcho parent 139->140->141->142->End
            new PlayerHistory("a hunter. ", 40, 139, 140, 20),
            new PlayerHistory("a warrior. ", 75, 139, 140, 30),
            new PlayerHistory("a cultist. ", 95, 139, 140, 50),
            new PlayerHistory("a warlock. ", 100, 139, 140, 60),
            new PlayerHistory("a high priest. ", 10, 139, 140, 80),
            // Group 140: Tcho-Tcho eyes 140->141->142->End
            new PlayerHistory("You have deep-set eyes, ", 30, 140, 141, 50),
            new PlayerHistory("You have a missing eye, ", 40, 140, 141, 50),
            new PlayerHistory("You have dark eyes, ", 90, 140, 141, 50),
            new PlayerHistory("You have bloodshot eyes, ", 100, 140, 141, 50),
            // Group 141: Tcho-Tcho body 141->142->End
            new PlayerHistory("many scars, and ", 30, 141, 142, 50),
            new PlayerHistory("obscene tattoos, and ", 50, 141, 142, 50),
            new PlayerHistory("ritual scarification, and ", 70, 141, 142, 50),
            new PlayerHistory("an extra toe, and ", 82, 141, 142, 50),
            new PlayerHistory("a vestigial tail, and ", 87, 141, 142, 50),
            new PlayerHistory("unmistakable signs of inbreeding, and ", 90, 141, 142, 50),
            new PlayerHistory("a missing nose, and ", 100, 141, 142, 50),
            // Group 142: Tcho-Tcho teeth 142->End
            new PlayerHistory("missing teeth.", 30, 142, 0, 50),
            new PlayerHistory("sharpened teeth.", 50, 142, 0, 50),
            new PlayerHistory("rotten teeth.", 70, 142, 0, 50),
            new PlayerHistory("filed-down teeth.", 90, 142, 0, 50),
            new PlayerHistory("no teeth.", 100, 142, 0, 50)
        };

        /// <summary>
        /// Beginnings of 'Cthulhoid' names used by mind flayers, miri nigri, and tcho tchos
        /// </summary>
        private static readonly string[] _cthuloidSyllable1 =
            {"Cth", "Az", "Fth", "Ts", "Xo", "Q'N", "R'L", "Ghata", "L", "Zz", "Fl", "Cl", "S", "Y"};

        /// <summary>
        /// Middles of 'Cthulhoid' names used by mind flayers, miri nigri, and tcho tchos
        /// </summary>
        private static readonly string[] _cthuloidSyllable2 =
            {"nar", "loi", "ul", "lu", "noth", "thon", "ath", "'N", "rhy", "oth", "aza", "agn", "oa", "og"};

        /// <summary>
        /// Endings of 'Cthulhoid' names used by mind flayers, miri nigri, and tcho tchos
        /// </summary>
        private static readonly string[] _cthuloidSyllable3 =
            {"l", "a", "u", "oa", "oggua", "oth", "ath", "aggua", "lu", "lo", "loth", "lotha", "agn", "axl"};

        /// <summary>
        /// Beginnings of 'Dwarven' names used by dwarves, cyclopes, half giants, golems, and nibelungen
        /// </summary>
        private static readonly string[] _dwarfSyllable1 =
            {"B", "D", "F", "G", "Gl", "H", "K", "L", "M", "N", "R", "S", "T", "Th", "V"};

        /// <summary>
        /// Middles of 'Dwarven' names used by dwarves, cyclopes, half giants, golems, and nibelungen
        /// </summary>
        private static readonly string[] _dwarfSyllable2 = { "a", "e", "i", "o", "oi", "u" };

        /// <summary>
        /// Endings of 'Dwarven' names used by dwarves, cyclopes, half giants, golems, and nibelungen
        /// </summary>
        private static readonly string[] _dwarfSyllable3 =
        {
            "bur", "fur", "gan", "gnus", "gnar", "li", "lin", "lir", "mli", "nar", "nus", "rin", "ran", "sin", "sil",
            "sur"
        };

        /// <summary>
        /// Beginnings of 'Elvish' names used by elves, dark elves, high elves, half-elves, and sprites
        /// </summary>
        private static readonly string[] _elfSyllable1 =
        {
            "Al", "An", "Bal", "Bel", "Cal", "Cel", "El", "Elr", "Elv", "Eow", "Ear", "F", "Fal", "Fel", "Fin", "G",
            "Gal", "Gel", "Gl", "Is", "Lan", "Leg", "Lom", "N", "Nal", "Nel", "S", "Sal", "Sel", "T", "Tal", "Tel",
            "Thr", "Tin"
        };

        /// <summary>
        /// Middles of 'Elvish' names used by elves, dark elves, high elves, half-elves, and sprites
        /// </summary>
        private static readonly string[] _elfSyllable2 =
        {
            "a", "adrie", "ara", "e", "ebri", "ele", "ere", "i", "io", "ithra", "ilma", "il-Ga", "ili", "o", "orfi",
            "u", "y"
        };

        /// <summary>
        /// Endings of 'Elvish' names used by elves, dark elves, high elves, half-elves, and sprites
        /// </summary>
        private static readonly string[] _elfSyllable3 =
        {
            "l", "las", "lad", "ldor", "ldur", "linde", "lith", "mir", "n", "nd", "ndel", "ndil", "ndir", "nduil", "ng",
            "mbor", "r", "rith", "ril", "riand", "rion", "s", "thien", "viel", "wen", "wyn"
        };

        /// <summary>
        /// Beginnings of 'Gnomish' names used by gnomes and draconians
        /// </summary>
        private static readonly string[] _gnomeSyllable1 =
        {
            "Aar", "An", "Ar", "As", "C", "H", "Han", "Har", "Hel", "Iir", "J", "Jan", "Jar", "K", "L", "M", "Mar", "N",
            "Nik", "Os", "Ol", "P", "R", "S", "Sam", "San", "T", "Ter", "Tom", "Ul", "V", "W", "Y"
        };

        /// <summary>
        /// Middles of 'Gnomish' names used by gnomes and draconians
        /// </summary>
        private static readonly string[] _gnomeSyllable2 = { "a", "aa", "ai", "e", "ei", "i", "o", "uo", "u", "uu" };

        /// <summary>
        /// Endings of 'Gnomish' names used by gnomes and draconians
        /// </summary>
        private static readonly string[] _gnomeSyllable3 =
        {
            "ron", "re", "la", "ki", "kseli", "ksi", "ku", "ja", "ta", "na", "namari", "neli", "nika", "nikki", "nu",
            "nukka", "ka", "ko", "li", "kki", "rik", "po", "to", "pekka", "rjaana", "rjatta", "rjukka", "la", "lla",
            "lli", "mo", "nni"
        };

        /// <summary>
        /// Beginnings of 'Hobbit' names used by hobbits and kobolds
        /// </summary>
        private static readonly string[] _hobbitSyllable1 =
        {
            "B", "Ber", "Br", "D", "Der", "Dr", "F", "Fr", "G", "H", "L", "Ler", "M", "Mer", "N", "P", "Pr", "Per", "R",
            "S", "T", "W"
        };

        /// <summary>
        /// Middles of 'Hobbit' names used by hobbits and kobolds
        /// </summary>
        private static readonly string[] _hobbitSyllable2 = { "a", "e", "i", "ia", "o", "oi", "u" };

        /// <summary>
        /// Endings of 'Hobbit' names used by hobbits and kobolds
        /// </summary>
        private static readonly string[] _hobbitSyllable3 =
        {
            "bo", "ck", "decan", "degar", "do", "doc", "go", "grin", "lba", "lbo", "lda", "ldo", "lla", "ll", "lo", "m",
            "mwise", "nac", "noc", "nwise", "p", "ppin", "pper", "tho", "to"
        };

        /// <summary>
        /// Beginnings of 'Human' names used by humans, great ones, skeletons, spectres, vampires,
        /// zombies, and half titans
        /// </summary>
        private static readonly string[] _humanSyllable1 =
        {
            "Ab", "Ac", "Ad", "Af", "Agr", "Ast", "As", "Al", "Adw", "Adr", "Ar", "B", "Br", "C", "Cr", "Ch", "Cad",
            "D", "Dr", "Dw", "Ed", "Eth", "Et", "Er", "El", "Eow", "F", "Fr", "G", "Gr", "Gw", "Gal", "Gl", "H", "Ha",
            "Ib", "Jer", "K", "Ka", "Ked", "L", "Loth", "Lar", "Leg", "M", "Mir", "N", "Nyd", "Ol", "Oc", "On", "P",
            "Pr", "R", "Rh", "S", "Sev", "T", "Tr", "Th", "V", "Y", "Z", "W", "Wic"
        };

        /// <summary>
        /// Middles of 'Human' names used by humans, great ones, skeletons, spectres, vampires,
        /// zombies, and half titans
        /// </summary>
        private static readonly string[] _humanSyllable2 =
        {
            "a", "ae", "au", "ao", "are", "ale", "ali", "ay", "ardo", "e", "ei", "ea", "eri", "era", "ela", "eli",
            "enda", "erra", "i", "ia", "ie", "ire", "ira", "ila", "ili", "ira", "igo", "o", "oa", "oi", "oe", "ore",
            "u", "y"
        };

        /// <summary>
        /// Endings of 'Human' names used by humans, great ones, skeletons, spectres, vampires,
        /// zombies, and half titans
        /// </summary>
        private static readonly string[] _humanSyllable3 =
        {
            "a", "and", "b", "bwyn", "baen", "bard", "c", "ctred", "cred", "ch", "can", "d", "dan", "don", "der",
            "dric", "dfrid", "dus", "f", "g", "gord", "gan", "l", "li", "lgrin", "lin", "lith", "lath", "loth", "ld",
            "ldric", "ldan", "m", "mas", "mos", "mar", "mond", "n", "nydd", "nidd", "nnon", "nwan", "nyth", "nad", "nn",
            "nnor", "nd", "p", "r", "ron", "rd", "s", "sh", "seth", "sean", "t", "th", "tha", "tlan", "trem", "tram",
            "v", "vudd", "w", "wan", "win", "wyn", "wyr", "wyr", "wyth"
        };

        /// <summary>
        /// Beginnings of 'Klackon' names used by klackons
        /// </summary>
        private static readonly string[] _klackonSyllable1 =
            {"K'", "K", "Kri", "Kir", "Kiri", "Iriki", "Irik", "Karik", "Iri", "Akri"};

        /// <summary>
        /// Middles of 'Klackon' names used by klackons
        /// </summary>
        private static readonly string[] _klackonSyllable2 =
            {"arak", "i", "iri", "ikki", "ki", "kiri", "ikir", "irak", "arik", "k'", "r"};

        /// <summary>
        /// Endings of 'Klackon' names used by klackons
        /// </summary>
        private static readonly string[] _klackonSyllable3 =
            {"akkak", "ak", "ik", "ikkik", "irik", "arik", "kidik", "kii", "k", "ki", "riki", "irk"};

        /// <summary>
        /// Beginnings of 'Orcish' names used by half orcs, half ogres, and half trolls
        /// </summary>
        private static readonly string[] _orcSyllable1 =
            {"B", "Er", "G", "Gr", "H", "P", "Pr", "R", "V", "Vr", "T", "Tr", "M", "Dr"};

        /// <summary>
        /// Middles of 'Orcish' names used by half orcs, half ogres, and half trolls
        /// </summary>
        private static readonly string[] _orcSyllable2 = { "a", "i", "o", "oo", "u", "ui" };

        /// <summary>
        /// Endings of 'Orcish' names used by half orcs, half ogres, and half trolls
        /// </summary>
        private static readonly string[] _orcSyllable3 =
        {
            "dash", "dish", "dush", "gar", "gor", "gdush", "lo", "gdish", "k", "lg", "nak", "rag", "rbag", "rg", "rk",
            "ng", "nk", "rt", "ol", "urk", "shnak", "mog", "mak", "rak"
        };

        /// <summary>
        /// Beginnings of 'Yeekish' names used by yeeks
        /// </summary>
        private static readonly string[] _yeekSyllable1 = { "Y", "Ye", "Yee", "Y" };

        /// <summary>
        /// Middles of 'Yeekish' names used by yeeks
        /// </summary>
        private static readonly string[] _yeekSyllable2 =
            {"ee", "eee", "ee", "ee-ee", "ee'ee", "'ee", "eee", "ee", "ee"};

        /// <summary>
        /// Endings of 'Yeekish' names used by yeeks
        /// </summary>
        private static readonly string[] _yeekSyllable3 = { "k", "k", "k", "ek", "eek", "ek" };

        /// <summary>
        /// Create a random name for a character based on their race.
        /// </summary>
        /// <param name="raceIndex"> The race for which to generate a name </param>
        /// <returns> The random name </returns>
        public static string CreateRandomName(int raceIndex)
        {
            string name = "";
            do
            {
                switch (raceIndex)
                {
                    case RaceId.Cyclops:
                    case RaceId.Dwarf:
                    case RaceId.HalfGiant:
                    case RaceId.Golem:
                    case RaceId.Nibelung:
                        name = _dwarfSyllable1[Program.Rng.RandomLessThan(_dwarfSyllable1.Length)];
                        name += _dwarfSyllable2[Program.Rng.RandomLessThan(_dwarfSyllable2.Length)];
                        name += _dwarfSyllable3[Program.Rng.RandomLessThan(_dwarfSyllable3.Length)];
                        break;

                    case RaceId.DarkElf:
                    case RaceId.Elf:
                    case RaceId.HalfElf:
                    case RaceId.HighElf:
                    case RaceId.Sprite:
                        name = _elfSyllable1[Program.Rng.RandomLessThan(_elfSyllable1.Length)];
                        name += _elfSyllable2[Program.Rng.RandomLessThan(_elfSyllable2.Length)];
                        name += _elfSyllable3[Program.Rng.RandomLessThan(_elfSyllable3.Length)];
                        break;

                    case RaceId.Draconian:
                    case RaceId.Gnome:
                        name = _gnomeSyllable1[Program.Rng.RandomLessThan(_gnomeSyllable1.Length)];
                        name += _gnomeSyllable2[Program.Rng.RandomLessThan(_gnomeSyllable2.Length)];
                        name += _gnomeSyllable3[Program.Rng.RandomLessThan(_gnomeSyllable3.Length)];
                        break;

                    case RaceId.Hobbit:
                    case RaceId.Kobold:
                        name = _hobbitSyllable1[Program.Rng.RandomLessThan(_hobbitSyllable1.Length)];
                        name += _hobbitSyllable2[Program.Rng.RandomLessThan(_hobbitSyllable2.Length)];
                        name += _hobbitSyllable3[Program.Rng.RandomLessThan(_hobbitSyllable3.Length)];
                        break;

                    case RaceId.Yeek:
                        name = _yeekSyllable1[Program.Rng.RandomLessThan(_yeekSyllable1.Length)];
                        name += _yeekSyllable2[Program.Rng.RandomLessThan(_yeekSyllable2.Length)];
                        name += _yeekSyllable3[Program.Rng.RandomLessThan(_yeekSyllable3.Length)];
                        break;

                    case RaceId.Great:
                    case RaceId.HalfTitan:
                    case RaceId.Human:
                    case RaceId.Skeleton:
                    case RaceId.Spectre:
                    case RaceId.Vampire:
                    case RaceId.Zombie:
                        name = _humanSyllable1[Program.Rng.RandomLessThan(_humanSyllable1.Length)];
                        name += _humanSyllable2[Program.Rng.RandomLessThan(_humanSyllable2.Length)];
                        name += _humanSyllable3[Program.Rng.RandomLessThan(_humanSyllable3.Length)];
                        break;

                    case RaceId.HalfOgre:
                    case RaceId.HalfOrc:
                    case RaceId.HalfTroll:
                        name = _orcSyllable1[Program.Rng.RandomLessThan(_orcSyllable1.Length)];
                        name += _orcSyllable2[Program.Rng.RandomLessThan(_orcSyllable2.Length)];
                        name += _orcSyllable3[Program.Rng.RandomLessThan(_orcSyllable3.Length)];
                        break;

                    case RaceId.Klackon:
                        name = _klackonSyllable1[Program.Rng.RandomLessThan(_klackonSyllable1.Length)];
                        name += _klackonSyllable2[Program.Rng.RandomLessThan(_klackonSyllable2.Length)];
                        name += _klackonSyllable3[Program.Rng.RandomLessThan(_klackonSyllable3.Length)];
                        break;

                    case RaceId.MiriNigri:
                    case RaceId.MindFlayer:
                    case RaceId.TchoTcho:
                        name = _cthuloidSyllable1[Program.Rng.RandomLessThan(_cthuloidSyllable1.Length)];
                        name += _cthuloidSyllable2[Program.Rng.RandomLessThan(_cthuloidSyllable2.Length)];
                        name += _cthuloidSyllable3[Program.Rng.RandomLessThan(_cthuloidSyllable3.Length)];
                        break;

                    case RaceId.Imp:
                        name = _angelSyllable1[Program.Rng.RandomLessThan(_angelSyllable1.Length)];
                        name += _angelSyllable2[Program.Rng.RandomLessThan(_angelSyllable2.Length)];
                        name += _angelSyllable3[Program.Rng.RandomLessThan(_angelSyllable3.Length)];
                        break;
                }
            } while (name.Length > 12);
            return name;
        }

        /// <summary>
        /// Create a background for a character by stringing together randomised text fragments
        /// based on their race
        /// </summary>
        /// <param name="player"> The player that needs a background </param>
        public static void GetHistory(Player player)
        {
            int i;
            int chart;
            for (i = 0; i < 4; i++)
            {
                player.History[i] = string.Empty;
            }
            string fullHistory = string.Empty;
            int socialClass = Program.Rng.DieRoll(4);
            // Start on a chart based on the character's race
            switch (player.RaceIndex)
            {
                case RaceId.Great:
                    {
                        // Great One 67->68->50->51->52->53->End
                        chart = 67;
                        break;
                    }
                case RaceId.Human:
                    {
                        // Human 1->2->3->50->51->52->53->End
                        chart = 1;
                        break;
                    }
                case RaceId.TchoTcho:
                    {
                        // Tcho-Tcho 138->139->140->141->142->End
                        chart = 138;
                        break;
                    }
                case RaceId.HalfElf:
                    {
                        // Half-Elf 4->1->2->3->50->51->52->53->End
                        chart = 4;
                        break;
                    }
                case RaceId.Elf:
                case RaceId.HighElf:
                    {
                        // Elf/High-Elf 7->8->9->54->55->56->End
                        chart = 7;
                        break;
                    }
                case RaceId.Hobbit:
                    {
                        // Hobbit 10->11->3->50->51->52->53->End
                        chart = 10;
                        break;
                    }
                case RaceId.Gnome:
                    {
                        // Gnome 13->14->3->50->51->52->53->End
                        chart = 13;
                        break;
                    }
                case RaceId.Dwarf:
                    {
                        // Dwarf 16->17->18->57->58->59->60->61->End
                        chart = 16;
                        break;
                    }
                case RaceId.HalfOrc:
                    {
                        // Half-Orc 19->20->2->3->50->51->52->53->End
                        chart = 19;
                        break;
                    }
                case RaceId.HalfTroll:
                    {
                        // Half-Troll 22->23->62->63->64->65->66->End
                        chart = 22;
                        break;
                    }
                case RaceId.DarkElf:
                    {
                        // Dark-Elf 68->70->71->72->73->End
                        chart = 69;
                        break;
                    }
                case RaceId.HalfOgre:
                    {
                        // Half-Ogre 74->20->2->3->50->51->52->53->End
                        chart = 74;
                        break;
                    }
                case RaceId.HalfGiant:
                    {
                        // Half-Giant 75->20->2->3->50->51->52->53->End
                        chart = 75;
                        break;
                    }
                case RaceId.HalfTitan:
                    {
                        // Half-Titan 75->20->2->3->50->51->52->53->End
                        chart = 76;
                        break;
                    }
                case RaceId.Cyclops:
                    {
                        // Cyclops 77->109->110->111->112->End
                        chart = 77;
                        break;
                    }
                case RaceId.Yeek:
                    {
                        // Yeek 78->79->80->81->135->136->137->End
                        chart = 78;
                        break;
                    }
                case RaceId.Kobold:
                    {
                        // Kobold 82->83->24->25->26->End
                        chart = 82;
                        break;
                    }
                case RaceId.Klackon:
                    {
                        // Klackon 84->85->86->End
                        chart = 84;
                        break;
                    }
                case RaceId.Nibelung:
                    {
                        // Nibelung 87->88->18->57->58->59->60->61->End
                        chart = 87;
                        break;
                    }
                case RaceId.Draconian:
                    {
                        // Draconian 89->90->91->End
                        chart = 89;
                        break;
                    }
                case RaceId.MindFlayer:
                    {
                        // Mind-Flayer 93->93->End
                        chart = 92;
                        break;
                    }
                case RaceId.Imp:
                    {
                        // Imp 94->95->96->97->End
                        chart = 94;
                        break;
                    }
                case RaceId.Golem:
                    {
                        // Golem 98->99->100->101->End
                        chart = 98;
                        break;
                    }
                case RaceId.Skeleton:
                    {
                        // Skeleton 102->103->104->105->106->End
                        chart = 102;
                        break;
                    }
                case RaceId.Zombie:
                    {
                        // Zombie 107->108->62->63->64->65->66->End
                        chart = 107;
                        break;
                    }
                case RaceId.Vampire:
                    {
                        // Vampire 113->114->115->116->117->End
                        chart = 113;
                        break;
                    }
                case RaceId.Spectre:
                    {
                        // Spectre 118->119->134->120->121->122->123->End
                        chart = 118;
                        break;
                    }
                case RaceId.Sprite:
                    {
                        // Sprite 124->125->126->127->128->End
                        chart = 124;
                        break;
                    }
                case RaceId.MiriNigri:
                    {
                        // Miri-Nigri 129->130->131->132->133->End
                        chart = 129;
                        break;
                    }
                default:
                    {
                        // Unrecognised race gets no history
                        chart = 0;
                        break;
                    }
            }
            // Keep going till we get to an end
            while (chart != 0)
            {
                i = 0;
                // Roll percentile for which background to use within each chart
                int roll = Program.Rng.DieRoll(100);
                // Find the correct chart and background
                while (chart != _backgroundTable[i].Chart || roll > _backgroundTable[i].Roll)
                {
                    i++;
                }
                // Add the text to our buffer
                fullHistory += _backgroundTable[i].Info;
                // Adjust our social class by the bonus or penalty for that fragment
                socialClass += _backgroundTable[i].Bonus - 50;
                chart = _backgroundTable[i].Next;
            }
            // Make sure our social class didn't go out of bounds
            if (socialClass > 100)
            {
                socialClass = 100;
            }
            else if (socialClass < 1)
            {
                socialClass = 1;
            }
            player.SocialClass = socialClass;
            // Split the buffer into four strings to fit on four lines of the screen
            string s = fullHistory.Trim();
            i = 0;
            while (true)
            {
                int n = s.Length;
                if (n < 60)
                {
                    player.History[i] = s;
                    break;
                }
                for (n = 60; n > 0 && s[n - 1] != ' '; n--)
                {
                }
                player.History[i++] = s.Substring(0, n).Trim();
                s = s.Substring(n).Trim();
            }
        }
    }
}