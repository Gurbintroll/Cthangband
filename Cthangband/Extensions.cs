﻿using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Text;

namespace Cthangband
{
    internal static class Extensions
    {
        public static int A2I(this char x)
        {
            return x - 'a';
        }

        public static string FromCsvFriendly(this string s)
        {
            return s.Replace('£', '"').Replace('§', ',');
        }

        public static char I2A(this int x)
        {
            return (char)('a' + (char)x);
        }

        public static char IndexToLabel(this int i)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyzz";
            if (i < InventorySlot.MeleeWeapon)
            {
                return alphabet[i];
            }
            return alphabet[i - InventorySlot.MeleeWeapon];
        }

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

        public static string PluraliseMonsterName(this string name)
        {
            string dummy;
            if (name.Contains(" of "))
            {
                int i = name.IndexOf(" of ", StringComparison.Ordinal);
                dummy = name.Substring(0, i);
                if (dummy.EndsWith("s"))
                {
                    dummy += "es";
                }
                else if (dummy.EndsWith("ng") || dummy.EndsWith("wn"))
                {
                    // Plural matches singular
                }
                else
                {
                    dummy += "s";
                }
                dummy += name.Substring(i);
            }
            else if (name.Contains("coins"))
            {
                dummy = "piles of " + name;
            }
            else if (name == "Manes" || name == "Mi-Go")
            {
                dummy = name;
            }
            else if (name == "Homonculous")
            {
                dummy = "Homonculi";
            }
            else if (name == "Stairway to hell")
            {
                dummy = "Stairways to hell";
            }
            else if (name.EndsWith("y"))
            {
                dummy = name.Substring(0, name.Length - 1) + "ies";
            }
            else if (name.EndsWith("ouse"))
            {
                dummy = name.Substring(0, name.Length - 4) + "ice";
            }
            else if (name.EndsWith("kelman") || name.EndsWith(" man"))
            {
                dummy = name.Substring(0, name.Length - 2) + "en";
            }
            else if (name.EndsWith("hild"))
            {
                dummy = name + "ren";
            }
            else if (name.EndsWith("ex"))
            {
                dummy = name.Substring(0, name.Length - 2) + "ices";
            }
            else if (name.EndsWith("olf") || name.EndsWith("ief"))
            {
                dummy = name.Substring(0, name.Length - 1) + "ves";
            }
            else if (name.EndsWith("ch") || name.EndsWith("s"))
            {
                dummy = name + "es";
            }
            else
            {
                dummy = name + "s";
            }
            return dummy;
        }

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

        public static string StatToString(this int val)
        {
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

        public static Colour ToAttr(this ItemCategory tval)
        {
            switch (tval)
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

        public static string ToCsvFriendly(this string s)
        {
            return s.Replace('"', '£').Replace(',', '§');
        }

        public static int ToInt(this string s)
        {
            if (!int.TryParse(s, out int i))
            {
                i = 0;
            }
            return i;
        }

        public static string ToRoman(this int number, bool forGeneration)
        {
            StringBuilder roman = new StringBuilder();
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