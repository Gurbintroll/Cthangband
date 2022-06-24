// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Text;

namespace Cthangband
{
    /// <summary>
    /// Extension methods for primitive types
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Undoes the changes the ToCsvFriendly has done to recreate the original string
        /// </summary>
        /// <param name="s"> The csv-friendly string </param>
        /// <returns> The original string </returns>
        public static string FromCsvFriendly(this string s)
        {
            return s.Replace('£', '"').Replace('§', ',');
        }

        /// <summary>
        /// Converts an index (0-37) to a letter (a-z) for an inventory or equipment slot
        /// </summary>
        /// <param name="i"> The index </param>
        /// <returns> The letter </returns>
        public static char IndexToLabel(this int i)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyzz";
            if (i < InventorySlot.MeleeWeapon)
            {
                return alphabet[i];
            }
            return alphabet[i - InventorySlot.MeleeWeapon];
        }

        /// <summary>
        /// Converts an index (0-25) to a lower case letter (a-z)
        /// </summary>
        /// <param name="x"> The index </param>
        /// <returns> The letter </returns>
        public static char IndexToLetter(this int x)
        {
            return (char)('a' + (char)x);
        }

        /// <summary>
        /// Returns whether or not a character is a vowel
        /// </summary>
        /// <param name="ch"> The character </param>
        /// <returns> Whether or not the character is a vowel </returns>
        public static bool IsVowel(this char ch)
        {
            switch (ch)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                case 'A':
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Converts a character from a-z into a numeric index 0-25
        /// </summary>
        /// <param name="x"> The character to convert </param>
        /// <returns> The index of the character </returns>
        public static int LetterToNumber(this char x)
        {
            return x - 'a';
        }

        /// <summary>
        /// Pads a string in both directions to center the original
        /// </summary>
        /// <param name="source"> The original string </param>
        /// <param name="totalWidth"> The total width of the padded string </param>
        /// <param name="paddingChar"> The character with which to pad the string </param>
        /// <returns> The padded string </returns>
        public static string PadCenter(this string source, int totalWidth, char paddingChar = ' ')
        {
            int spaces = totalWidth - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
        }

        /// <summary>
        /// Pluralises a monster name, with various special cases for unusual names
        /// </summary>
        /// <param name="name"> The name to pluralise </param>
        /// <returns> The plural form of the name </returns>
        public static string PluraliseMonsterName(this string name)
        {
            string plural;
            // "X of Y" -> "Xs of Y"
            if (name.Contains(" of "))
            {
                int i = name.IndexOf(" of ", StringComparison.Ordinal);
                plural = name.Substring(0, i);
                // "XS of Y -> XSes of Y"
                if (plural.EndsWith("s"))
                {
                    plural += "es";
                }
                // "Young of Y" or "Spawn of Y" are unchanged
                else if (plural.EndsWith("ng") || plural.EndsWith("wn"))
                {
                    // Plural matches singular
                }
                else
                {
                    plural += "s";
                }
                plural += name.Substring(i);
            }
            // "Pile of X coins" -> "Piles of X coins"
            else if (name.Contains("coins"))
            {
                plural = "piles of " + name;
            }
            // "Manes" and "Mi-Go" are their own plurals
            else if (name == "Manes" || name == "Mi-Go")
            {
                plural = name;
            }
            // "Homonculous" -> "Homonculi"
            else if (name == "Homonculous")
            {
                plural = "Homonculi";
            }
            // "Stairway to hell" -> "Stairways to hell"
            else if (name == "Stairway to hell")
            {
                plural = "Stairways to hell";
            }
            // "Harpy" or similar -> "Harpies" or similar
            else if (name.EndsWith("y"))
            {
                plural = name.Substring(0, name.Length - 1) + "ies";
            }
            // "Mouse" or similar -> "Mice" or similar
            else if (name.EndsWith("ouse"))
            {
                plural = name.Substring(0, name.Length - 4) + "ice";
            }
            // "Pukelman" -> "Pukelmen"
            else if (name.EndsWith("kelman") || name.EndsWith(" man"))
            {
                plural = name.Substring(0, name.Length - 2) + "en";
            }
            // "X child" -> "X children"
            else if (name.EndsWith("hild"))
            {
                plural = name + "ren";
            }
            // "X vortex" -> "X vortices"
            else if (name.EndsWith("ex"))
            {
                plural = name.Substring(0, name.Length - 2) + "ices";
            }
            // "X wolf" and "X thief" -> "X wolves" and "X thieves"
            else if (name.EndsWith("olf") || name.EndsWith("ief"))
            {
                plural = name.Substring(0, name.Length - 1) + "ves";
            }
            // "Leech" (or similar) -> "Leeches"
            else if (name.EndsWith("ch") || name.EndsWith("s"))
            {
                plural = name + "es";
            }
            // If all else fails, just add an 's'.
            else
            {
                plural = name + "s";
            }
            return plural;
        }

        /// <summary>
        /// Convert a spell book category to its realm
        /// </summary>
        /// <param name="category"> The spell book item category </param>
        /// <returns> The realm of magic </returns>
        public static Realm SpellBookToToRealm(this ItemCategory category)
        {
            switch (category)
            {
                case ItemCategory.LifeBook:
                    return Realm.Life;

                case ItemCategory.SorceryBook:
                    return Realm.Sorcery;

                case ItemCategory.NatureBook:
                    return Realm.Nature;

                case ItemCategory.ChaosBook:
                    return Realm.Chaos;

                case ItemCategory.DeathBook:
                    return Realm.Death;

                case ItemCategory.TarotBook:
                    return Realm.Tarot;

                case ItemCategory.FolkBook:
                    return Realm.Folk;

                case ItemCategory.CorporealBook:
                    return Realm.Corporeal;

                default:
                    return Realm.None;
            }
        }

        /// <summary>
        /// Converts a numeric ability score (3-118) to a string (3-40+)
        /// </summary>
        /// <param name="val"> The value of the ability score </param>
        /// <returns> The display value </returns>
        public static string StatToString(this int val)
        {
            // Above 18, scores are measured in tenths of a point
            if (val > 18)
            {
                int bonus = val - 18;
                if (bonus > 220)
                {
                    return "   40+";
                }
                bonus = (bonus - 1) / 10;
                return (bonus + 19).ToString().PadLeft(6);
            }
            return val.ToString().PadLeft(6);
        }

        /// <summary>
        /// Converts an item category to a default colour for its description
        /// </summary>
        /// <param name="itemCategory"> The item category </param>
        /// <returns> The colour for the item's description </returns>
        public static Colour ToAttr(this ItemCategory itemCategory)
        {
            switch (itemCategory)
            {
                case ItemCategory.Skeleton:
                    {
                        return Colour.Beige;
                    }
                case ItemCategory.Bottle:
                case ItemCategory.Junk:
                    {
                        return Colour.White;
                    }
                case ItemCategory.Chest:
                    {
                        return Colour.Grey;
                    }
                case ItemCategory.Shot:
                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                    {
                        return Colour.BrightBrown;
                    }
                case ItemCategory.Light:
                    {
                        return Colour.BrightYellow;
                    }
                case ItemCategory.Spike:
                    {
                        return Colour.Grey;
                    }
                case ItemCategory.Bow:
                    {
                        return Colour.Brown;
                    }
                case ItemCategory.Digging:
                    {
                        return Colour.Grey;
                    }
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                    {
                        return Colour.BrightWhite;
                    }
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Crown:
                case ItemCategory.Helm:
                case ItemCategory.Shield:
                case ItemCategory.Cloak:
                    {
                        return Colour.BrightBrown;
                    }
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                    {
                        return Colour.Grey;
                    }
                case ItemCategory.Amulet:
                    {
                        return Colour.Orange;
                    }
                case ItemCategory.Ring:
                    {
                        return Colour.Gold;
                    }
                case ItemCategory.Staff:
                    {
                        return Colour.Purple;
                    }
                case ItemCategory.Wand:
                    {
                        return Colour.Chartreuse;
                    }
                case ItemCategory.Rod:
                    {
                        return Colour.Turquoise;
                    }
                case ItemCategory.Scroll:
                    {
                        return Colour.BrightBeige;
                    }
                case ItemCategory.Potion:
                    {
                        return Colour.Blue;
                    }
                case ItemCategory.Flask:
                    {
                        return Colour.Yellow;
                    }
                case ItemCategory.Food:
                    {
                        return Colour.Green;
                    }
                case ItemCategory.LifeBook:
                    {
                        return Colour.BrightWhite;
                    }
                case ItemCategory.SorceryBook:
                    {
                        return Colour.BrightBlue;
                    }
                case ItemCategory.NatureBook:
                    {
                        return Colour.BrightGreen;
                    }
                case ItemCategory.ChaosBook:
                    {
                        return Colour.BrightRed;
                    }
                case ItemCategory.DeathBook:
                    {
                        return Colour.Black;
                    }
                case ItemCategory.TarotBook:
                    {
                        return Colour.Pink;
                    }
                case ItemCategory.FolkBook:
                    {
                        return Colour.BrightPurple;
                    }
                case ItemCategory.CorporealBook:
                    {
                        return Colour.BrightYellow;
                    }
            }
            return Colour.White;
        }

        /// <summary>
        /// Strips characters from a string that would clash with viewing the string as
        /// comma-separated text
        /// </summary>
        /// <param name="s"> The original string </param>
        /// <returns> The csv-file friendly version of the string </returns>
        public static string ToCsvFriendly(this string s)
        {
            return s.Replace('"', '£').Replace(',', '§');
        }

        /// <summary>
        /// Try to parse the string to an integer, returning 0 rather than an error if it can't be parsed
        /// </summary>
        /// <param name="s"> The string to parse </param>
        /// <returns> The int value of the string, or 0 if it couldn't be parsed </returns>
        public static int ToIntSafely(this string s)
        {
            if (!int.TryParse(s.Trim(), out int i))
            {
                i = 0;
            }
            return i;
        }

        /// <summary>
        /// Converts an integer to a Roman numeral
        /// </summary>
        /// <param name="number"> The number to convert </param>
        /// <param name="forGeneration"> True if the number is the generation of a character </param>
        /// <returns> The number as a Roman numeral </returns>
        public static string ToRoman(this int number, bool forGeneration)
        {
            StringBuilder roman = new StringBuilder();
            // If we're for a generation, we want to skip 'I' (you're simply "John", not "John I")
            // and prefix with a space if not 'I')
            if (forGeneration)
            {
                if (number == 1)
                {
                    return string.Empty;
                }
                else
                {
                    roman.Append(' ');
                }
            }
            // Roman numerals are not positional, so simply start with the highest number and keep
            // appending and subtracting until we can't
            foreach (System.Collections.Generic.KeyValuePair<int, string> item in GlobalData.NumberRomanDictionary)
            {
                while (number >= item.Key)
                {
                    roman.Append(item.Value);
                    number -= item.Key;
                }
            }

            return roman.ToString();
        }

        /// <summary>
        /// Converts a realm to a spell book item category
        /// </summary>
        /// <param name="realm"> The realm of magic </param>
        /// <returns> The spell book item category </returns>
        public static ItemCategory ToSpellBookItemCategory(this Realm realm)
        {
            switch (realm)
            {
                case Realm.Life:
                    return ItemCategory.LifeBook;

                case Realm.Sorcery:
                    return ItemCategory.SorceryBook;

                case Realm.Nature:
                    return ItemCategory.NatureBook;

                case Realm.Chaos:
                    return ItemCategory.ChaosBook;

                case Realm.Death:
                    return ItemCategory.DeathBook;

                case Realm.Tarot:
                    return ItemCategory.TarotBook;

                case Realm.Folk:
                    return ItemCategory.FolkBook;

                case Realm.Corporeal:
                    return ItemCategory.CorporealBook;

                default:
                    return ItemCategory.None;
            }
        }
    }
}