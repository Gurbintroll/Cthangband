// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.IO;

namespace Cthangband
{
    [Serializable]
    internal class Item
    {
        public static int CoinType;
        public readonly FlagSet IdentifyFlags = new FlagSet();
        public readonly FlagSet LegendaryFlags1 = new FlagSet();
        public readonly FlagSet LegendaryFlags2 = new FlagSet();
        public readonly FlagSet LegendaryFlags3 = new FlagSet();
        public ArtifactId ArtifactIndex;
        public int BaseArmourClass;
        public int BonusArmourClass;
        public int BonusDamage;
        public int BonusPowerSubType;
        public Enumerations.RareItemType BonusPowerType;
        public int BonusToHit;
        public ItemCategory Category;
        public int Count;
        public int DamageDice;
        public int DamageDiceSides;
        public int Discount;
        public int HoldingMonsterIndex;
        public string Inscription = "";
        public int ItemSubCategory;
        public ItemType ItemType;
        public string LegendaryName = "";
        public bool Marked;
        public int NextInStack;
        public Enumerations.RareItemType RareItemTypeIndex;
        public int RechargeTimeLeft;
        public int TypeSpecificValue;
        public int Weight;
        public int X;
        public int Y;

        public Item()
        {
        }

        public Item(Item original)
        {
            BaseArmourClass = original.BaseArmourClass;
            LegendaryFlags1.Copy(original.LegendaryFlags1);
            LegendaryFlags2.Copy(original.LegendaryFlags2);
            LegendaryFlags3.Copy(original.LegendaryFlags3);
            LegendaryName = original.LegendaryName;
            DamageDice = original.DamageDice;
            Discount = original.Discount;
            DamageDiceSides = original.DamageDiceSides;
            HoldingMonsterIndex = original.HoldingMonsterIndex;
            IdentifyFlags.Copy(original.IdentifyFlags);
            X = original.X;
            Y = original.Y;
            ItemType = original.ItemType;
            Marked = original.Marked;
            ArtifactIndex = original.ArtifactIndex;
            RareItemTypeIndex = original.RareItemTypeIndex;
            NextInStack = original.NextInStack;
            Inscription = original.Inscription;
            Count = original.Count;
            TypeSpecificValue = original.TypeSpecificValue;
            ItemSubCategory = original.ItemSubCategory;
            RechargeTimeLeft = original.RechargeTimeLeft;
            BonusArmourClass = original.BonusArmourClass;
            BonusDamage = original.BonusDamage;
            BonusToHit = original.BonusToHit;
            Category = original.Category;
            Weight = original.Weight;
            BonusPowerType = original.BonusPowerType;
            BonusPowerSubType = original.BonusPowerSubType;
        }

        public void Absorb(Item other)
        {
            var total = Count + other.Count;
            Count = total < Constants.MaxStackSize ? total : Constants.MaxStackSize - 1;
            if (other.IsKnown())
            {
                BecomeKnown();
            }
            if (IdentifyFlags.IsSet(Constants.IdentStoreb) || (other.IdentifyFlags.IsSet(Constants.IdentStoreb) &&
                !(IdentifyFlags.IsSet(Constants.IdentStoreb) && other.IdentifyFlags.IsSet(Constants.IdentStoreb))))
            {
                if (other.IdentifyFlags.IsSet(Constants.IdentStoreb))
                {
                    other.IdentifyFlags.Clear(Constants.IdentStoreb);
                }
                if (IdentifyFlags.IsSet(Constants.IdentStoreb))
                {
                    IdentifyFlags.Clear(Constants.IdentStoreb);
                }
            }
            if (other.IdentifyFlags.IsSet(Constants.IdentMental))
            {
                IdentifyFlags.Set(Constants.IdentMental);
            }
            if (!string.IsNullOrEmpty(other.Inscription))
            {
                Inscription = other.Inscription;
            }
            if (Discount < other.Discount)
            {
                Discount = other.Discount;
            }
        }

        public int AdjustDamageForMonsterType(int tdam, Monster mPtr)
        {
            var mult = 1;
            var rPtr = mPtr.Race;
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            GetMergedFlags(f1, f2, f3);
            switch (Category)
            {
                case ItemCategory.Shot:
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                case ItemCategory.Digging:
                    {
                        if (f1.IsSet(ItemFlag1.SlayAnimal) && (rPtr.Flags3 & MonsterFlag3.Animal) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Animal;
                            }
                            if (mult < 2)
                            {
                                mult = 2;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayEvil) && (rPtr.Flags3 & MonsterFlag3.Evil) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Evil;
                            }
                            if (mult < 2)
                            {
                                mult = 2;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayUndead) && (rPtr.Flags3 & MonsterFlag3.Undead) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Undead;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayDemon) && (rPtr.Flags3 & MonsterFlag3.Demon) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Demon;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayOrc) && (rPtr.Flags3 & MonsterFlag3.Orc) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Orc;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayTroll) && (rPtr.Flags3 & MonsterFlag3.Troll) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Troll;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayGiant) && (rPtr.Flags3 & MonsterFlag3.Giant) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Giant;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.SlayDragon) && (rPtr.Flags3 & MonsterFlag3.Dragon) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Dragon;
                            }
                            if (mult < 3)
                            {
                                mult = 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.KillDragon) && (rPtr.Flags3 & MonsterFlag3.Dragon) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.Dragon;
                            }
                            if (mult < 5)
                            {
                                mult = 5;
                            }
                            if (ArtifactIndex == ArtifactId.SwordLightning)
                            {
                                mult *= 3;
                            }
                        }
                        if (f1.IsSet(ItemFlag1.BrandAcid))
                        {
                            if ((rPtr.Flags3 & MonsterFlag3.ImmuneAcid) != 0)
                            {
                                if (mPtr.IsVisible)
                                {
                                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmuneAcid;
                                }
                            }
                            else
                            {
                                if (mult < 3)
                                {
                                    mult = 3;
                                }
                            }
                        }
                        if (f1.IsSet(ItemFlag1.BrandElec))
                        {
                            if ((rPtr.Flags3 & MonsterFlag3.ImmuneLightning) != 0)
                            {
                                if (mPtr.IsVisible)
                                {
                                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmuneLightning;
                                }
                            }
                            else
                            {
                                if (mult < 3)
                                {
                                    mult = 3;
                                }
                            }
                        }
                        if (f1.IsSet(ItemFlag1.BrandFire))
                        {
                            if ((rPtr.Flags3 & MonsterFlag3.ImmuneFire) != 0)
                            {
                                if (mPtr.IsVisible)
                                {
                                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmuneFire;
                                }
                            }
                            else
                            {
                                if (mult < 3)
                                {
                                    mult = 3;
                                }
                            }
                        }
                        if (f1.IsSet(ItemFlag1.BrandCold))
                        {
                            if ((rPtr.Flags3 & MonsterFlag3.ImmuneCold) != 0)
                            {
                                if (mPtr.IsVisible)
                                {
                                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmuneCold;
                                }
                            }
                            else
                            {
                                if (mult < 3)
                                {
                                    mult = 3;
                                }
                            }
                        }
                        if (f1.IsSet(ItemFlag1.BrandPois))
                        {
                            if ((rPtr.Flags3 & MonsterFlag3.ImmunePoison) != 0)
                            {
                                if (mPtr.IsVisible)
                                {
                                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmunePoison;
                                }
                            }
                            else
                            {
                                if (mult < 3)
                                {
                                    mult = 3;
                                }
                            }
                        }
                        break;
                    }
            }
            return tdam * mult;
        }

        public void ApplyMagic(int lev, bool okay, bool good, bool great)
        {
            var forge = new ItemForge(this);
            forge.ApplyMagic(lev, okay, good, great);
        }

        public void ApplyRandomResistance(int specific)
        {
            var forge = new ItemForge(this);
            forge.ApplyRandomResistance(specific);
        }

        public void AssignItemType(ItemType kIdx)
        {
            var kPtr = kIdx;
            ItemType = kIdx;
            Category = kPtr.Category;
            ItemSubCategory = kPtr.SubCategory;
            TypeSpecificValue = kPtr.Pval;
            Count = 1;
            Weight = kPtr.Weight;
            BonusToHit = kPtr.ToH;
            BonusDamage = kPtr.ToD;
            BonusArmourClass = kPtr.ToA;
            BaseArmourClass = kPtr.Ac;
            DamageDice = kPtr.Dd;
            DamageDiceSides = kPtr.Ds;
            if (kPtr.Cost <= 0)
            {
                IdentifyFlags.Set(Constants.IdentBroken);
            }
            if (kPtr.Flags3.IsSet(ItemFlag3.Cursed))
            {
                IdentifyFlags.Set(Constants.IdentCursed);
            }
        }

        public void BecomeFlavourAware()
        {
            ItemType.FlavourAware = true;
        }

        public void BecomeKnown()
        {
            if (!string.IsNullOrEmpty(Inscription) && IdentifyFlags.IsSet(Constants.IdentSense))
            {
                var q = Inscription;
                if (q == "cursed" || q == "broken" || q == "good" || q == "average" || q == "excellent" ||
                    q == "worthless" || q == "special" || q == "terrible")
                {
                    Inscription = string.Empty;
                }
            }
            IdentifyFlags.Clear(Constants.IdentSense);
            IdentifyFlags.Clear(Constants.IdentEmpty);
            IdentifyFlags.Set(Constants.IdentKnown);
        }

        public int BreakageChance()
        {
            switch (Category)
            {
                case ItemCategory.Flask:
                case ItemCategory.Potion:
                case ItemCategory.Bottle:
                case ItemCategory.Food:
                case ItemCategory.Junk:
                    return 100;

                case ItemCategory.Light:
                case ItemCategory.Scroll:
                case ItemCategory.Skeleton:
                    {
                        return 50;
                    }
                case ItemCategory.Wand:
                case ItemCategory.Arrow:
                case ItemCategory.Shot:
                case ItemCategory.Bolt:
                    {
                        return 25;
                    }
            }
            return 10;
        }

        public bool CanAbsorb(Item other)
        {
            var total = Count + other.Count;
            if (ItemType != other.ItemType)
            {
                return false;
            }
            switch (Category)
            {
                case ItemCategory.Chest:
                    return false;

                case ItemCategory.Food:
                case ItemCategory.Potion:
                case ItemCategory.Scroll:
                    break;

                case ItemCategory.Staff:
                case ItemCategory.Wand:
                    if (!IsKnown() || !other.IsKnown())
                    {
                        return false;
                    }
                    if (TypeSpecificValue != other.TypeSpecificValue)
                    {
                        return false;
                    }
                    break;

                case ItemCategory.Rod:
                    if (TypeSpecificValue != other.TypeSpecificValue)
                    {
                        return false;
                    }
                    break;

                case ItemCategory.Bow:
                case ItemCategory.Digging:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Helm:
                case ItemCategory.Crown:
                case ItemCategory.Shield:
                case ItemCategory.Cloak:
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                case ItemCategory.Ring:
                case ItemCategory.Amulet:
                case ItemCategory.Light:
                    if (!IsKnown() || !other.IsKnown())
                    {
                        return false;
                    }
                    if (IsKnown() != other.IsKnown())
                    {
                        return false;
                    }
                    if (BonusToHit != other.BonusToHit)
                    {
                        return false;
                    }
                    if (BonusDamage != other.BonusDamage)
                    {
                        return false;
                    }
                    if (BonusArmourClass != other.BonusArmourClass)
                    {
                        return false;
                    }
                    if (TypeSpecificValue != other.TypeSpecificValue)
                    {
                        return false;
                    }
                    if (ArtifactIndex != other.ArtifactIndex)
                    {
                        return false;
                    }
                    if (IsLegendary() || other.IsLegendary())
                    {
                        return false;
                    }
                    if (RareItemTypeIndex != other.RareItemTypeIndex)
                    {
                        return false;
                    }
                    if (BonusPowerType != 0 || other.BonusPowerType != 0)
                    {
                        return false;
                    }
                    if (RechargeTimeLeft != 0 || other.RechargeTimeLeft != 0)
                    {
                        return false;
                    }
                    if (BaseArmourClass != other.BaseArmourClass)
                    {
                        return false;
                    }
                    if (DamageDice != other.DamageDice)
                    {
                        return false;
                    }
                    if (DamageDiceSides != other.DamageDiceSides)
                    {
                        return false;
                    }
                    break;

                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                case ItemCategory.Shot:
                    if (IsKnown() != other.IsKnown())
                    {
                        return false;
                    }
                    if (BonusToHit != other.BonusToHit)
                    {
                        return false;
                    }
                    if (BonusDamage != other.BonusDamage)
                    {
                        return false;
                    }
                    if (BonusArmourClass != other.BonusArmourClass)
                    {
                        return false;
                    }
                    if (TypeSpecificValue != other.TypeSpecificValue)
                    {
                        return false;
                    }
                    if (ArtifactIndex != other.ArtifactIndex)
                    {
                        return false;
                    }
                    if (IsLegendary() || other.IsLegendary())
                    {
                        return false;
                    }
                    if (RareItemTypeIndex != other.RareItemTypeIndex)
                    {
                        return false;
                    }
                    if (BonusPowerType != 0 || other.BonusPowerType != 0)
                    {
                        return false;
                    }
                    if (RechargeTimeLeft != 0 || other.RechargeTimeLeft != 0)
                    {
                        return false;
                    }
                    if (BaseArmourClass != other.BaseArmourClass)
                    {
                        return false;
                    }
                    if (DamageDice != other.DamageDice)
                    {
                        return false;
                    }
                    if (DamageDiceSides != other.DamageDiceSides)
                    {
                        return false;
                    }
                    break;

                default:
                    if (!IsKnown() || !other.IsKnown())
                    {
                        return false;
                    }
                    break;
            }
            if (LegendaryFlags1.Value != other.LegendaryFlags1.Value || LegendaryFlags2.Value != other.LegendaryFlags2.Value ||
                LegendaryFlags3.Value != other.LegendaryFlags3.Value)
            {
                return false;
            }
            if (!IdentifyFlags.Matches(other.IdentifyFlags, Constants.IdentCursed))
            {
                return false;
            }
            if (!IdentifyFlags.Matches(other.IdentifyFlags, Constants.IdentBroken))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(Inscription) && !string.IsNullOrEmpty(other.Inscription) &&
                Inscription != other.Inscription)
            {
                return false;
            }
            if (Discount != other.Discount)
            {
                return false;
            }
            return total < Constants.MaxStackSize;
        }

        public bool CreateLegendary(bool fromScroll)
        {
            var forge = new ItemForge(this);
            return forge.CreateLegendary(fromScroll);
        }

        public string Description(bool pref, int mode)
        {
            var aware = false;
            var known = false;
            var appendName = false;
            var showWeapon = false;
            var showArmour = false;
            string s;
            const char p1 = '(';
            const char p2 = ')';
            const char b1 = '[';
            const char b2 = ']';
            const char c1 = '{';
            const char c2 = '}';
            const string tmpVal = "";
            var tmpVal2 = "";
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            var kPtr = ItemType;
            if (kPtr == null)
            {
                return "(nothing)";
            }
            GetMergedFlags(f1, f2, f3);
            if (IsFlavourAware())
            {
                aware = true;
            }
            if (IsKnown())
            {
                known = true;
            }
            var indexx = ItemSubCategory;
            var basenm = kPtr.Name;
            var modstr = "";
            switch (Category)
            {
                case ItemCategory.Skeleton:
                case ItemCategory.Bottle:
                case ItemCategory.Junk:
                case ItemCategory.Spike:
                case ItemCategory.Flask:
                case ItemCategory.Chest:
                    break;

                case ItemCategory.Shot:
                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                case ItemCategory.Bow:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                case ItemCategory.Digging:
                    showWeapon = true;
                    break;

                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Cloak:
                case ItemCategory.Crown:
                case ItemCategory.Helm:
                case ItemCategory.Shield:
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                    showArmour = true;
                    break;

                case ItemCategory.Light:
                    break;

                case ItemCategory.Amulet:
                    if (IsArtifact() && aware)
                    {
                        break;
                    }
                    modstr = SaveGame.Instance.AmuletFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Amulet~" : "& # Amulet~";
                    break;

                case ItemCategory.Ring:
                    if (IsArtifact() && aware)
                    {
                        break;
                    }
                    modstr = SaveGame.Instance.RingFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Ring~" : "& # Ring~";
                    break;

                case ItemCategory.Staff:
                    modstr = SaveGame.Instance.StaffFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Staff~" : "& # Staff~";
                    break;

                case ItemCategory.Wand:
                    modstr = SaveGame.Instance.WandFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Wand~" : "& # Wand~";
                    break;

                case ItemCategory.Rod:
                    modstr = SaveGame.Instance.RodFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Rod~" : "& # Rod~";
                    break;

                case ItemCategory.Scroll:
                    modstr = SaveGame.Instance.ScrollFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Scroll~" : "& Scroll~ titled \"#\"";
                    break;

                case ItemCategory.Potion:
                    modstr = SaveGame.Instance.PotionFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Potion~" : "& # Potion~";
                    break;

                case ItemCategory.Food:
                    if (ItemSubCategory >= Enumerations.FoodType.MinFood)
                    {
                        break;
                    }
                    modstr = SaveGame.Instance.MushroomFlavours[indexx].Name;
                    if (aware)
                    {
                        appendName = true;
                    }
                    basenm = IdentifyFlags.IsSet(Constants.IdentStoreb) ? "& Mushroom~" : "& # Mushroom~";
                    break;

                case ItemCategory.LifeBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Life Magic #"
                        : "& Life Spellbook~ #";
                    break;

                case ItemCategory.SorceryBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Sorcery #"
                        : "& Sorcery Spellbook~ #";
                    break;

                case ItemCategory.NatureBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Nature Magic #"
                        : "& Nature Spellbook~ #";
                    break;

                case ItemCategory.ChaosBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Chaos Magic #"
                        : "& Chaos Spellbook~ #";
                    break;

                case ItemCategory.DeathBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Death Magic #"
                        : "& Death Spellbook~ #";
                    break;

                case ItemCategory.TarotBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Tarot Magic #"
                        : "& Tarot Spellbook~ #";
                    break;

                case ItemCategory.FolkBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Folk Magic #"
                        : "& Folk Spellbook~ #";
                    break;

                case ItemCategory.CorporealBook:
                    modstr = basenm;
                    basenm = SaveGame.Instance.Player.Spellcasting.Type == CastingType.Divine
                        ? "& Book~ of Corporeal Magic #"
                        : "& Corporeal Spellbook~ #";
                    break;

                case ItemCategory.Gold:
                    return basenm;

                default:
                    return "(nothing)";
            }
            var t = tmpVal;
            if (basenm[0] == '&')
            {
                s = basenm.Substring(2);
                if (!pref)
                {
                }
                else if (Count <= 0)
                {
                    t += "no more ";
                }
                else if (Count > 1)
                {
                    t += Count;
                    t += ' ';
                }
                else if (known && (IsArtifact() || IsLegendary()))
                {
                    t += "The ";
                }
                else if (s.StartsWith("#") && modstr[0].IsVowel())
                {
                    t += "an ";
                }
                else if (s[0].IsVowel())
                {
                    t += "an ";
                }
                else
                {
                    t += "a ";
                }
            }
            else
            {
                s = basenm;
                if (!pref)
                {
                }
                else if (Count <= 0)
                {
                    t += "no more ";
                }
                else if (Count > 1)
                {
                    t += Count;
                    t += ' ';
                }
                else if (known && (IsArtifact() || IsLegendary()))
                {
                    t += "The ";
                }
            }
            foreach (var ch in s)
            {
                if (ch == '~')
                {
                    if (Count != 1)
                    {
                        var k = t[t.Length - 1];
                        if (k == 's' || k == 'h')
                        {
                            t += 'e';
                        }
                        t += 's';
                    }
                }
                else if (ch == '#')
                {
                    t += modstr;
                }
                else
                {
                    t += ch;
                }
            }
            if (appendName)
            {
                t += " of ";
                t += kPtr.Name;
            }
            if (known)
            {
                if (IsLegendary())
                {
                    t += " '";
                    t += LegendaryName;
                    t += "'";
                }
                else if (ArtifactIndex != 0)
                {
                    var aPtr = Profile.Instance.Artifacts[ArtifactIndex];
                    t += ' ';
                    t += aPtr.Name;
                }
                else if (RareItemTypeIndex != Enumerations.RareItemType.None)
                {
                    var ePtr = Profile.Instance.RareItemTypes[RareItemTypeIndex];
                    t += ' ';
                    t += ePtr.Name;
                }
            }
            if (mode < 1)
            {
                return t;
            }
            if (Category == ItemCategory.Chest)
            {
                if (!known)
                {
                }
                else if (TypeSpecificValue == 0)
                {
                    t += " (empty)";
                }
                else if (TypeSpecificValue < 0)
                {
                    if (GlobalData.ChestTraps[-TypeSpecificValue] != 0)
                    {
                        t += " (disarmed)";
                    }
                    else
                    {
                        t += " (unlocked)";
                    }
                }
                else
                {
                    switch (GlobalData.ChestTraps[TypeSpecificValue])
                    {
                        case 0:
                            {
                                t += " (Locked)";
                                break;
                            }
                        case ChestTrap.ChestLoseStr:
                            {
                                t += " (Poison Needle)";
                                break;
                            }
                        case ChestTrap.ChestLoseCon:
                            {
                                t += " (Poison Needle)";
                                break;
                            }
                        case ChestTrap.ChestPoison:
                            {
                                t += " (Gas Trap)";
                                break;
                            }
                        case ChestTrap.ChestParalyze:
                            {
                                t += " (Gas Trap)";
                                break;
                            }
                        case ChestTrap.ChestExplode:
                            {
                                t += " (Explosion Device)";
                                break;
                            }
                        case ChestTrap.ChestSummon:
                            {
                                t += " (Summoning Runes)";
                                break;
                            }
                        default:
                            {
                                t += " (Multiple Traps)";
                                break;
                            }
                    }
                }
            }
            if (f3.IsSet(ItemFlag3.ShowMods))
            {
                showWeapon = true;
            }
            if (BonusToHit != 0 && BonusDamage != 0)
            {
                showWeapon = true;
            }
            if (BaseArmourClass != 0)
            {
                showArmour = true;
            }
            switch (Category)
            {
                case ItemCategory.Shot:
                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                case ItemCategory.Digging:
                    t += ' ';
                    t += p1;
                    t += DamageDice;
                    t += 'd';
                    t += DamageDiceSides;
                    t += p2;
                    break;

                case ItemCategory.Bow:
                    var power = ItemSubCategory % 10;
                    if (f3.IsSet(ItemFlag3.XtraMight))
                    {
                        power++;
                    }
                    t += ' ';
                    t += p1;
                    t += 'x';
                    t += power;
                    t += p2;
                    break;
            }
            if (known)
            {
                if (showWeapon)
                {
                    t += ' ';
                    t += p1;
                    if (BonusToHit >= 0)
                    {
                        t += "+";
                    }
                    t += BonusToHit;
                    t += ',';
                    if (BonusDamage >= 0)
                    {
                        t += "+";
                    }
                    t += BonusDamage;
                    t += p2;
                }
                else if (BonusToHit != 0)
                {
                    t += ' ';
                    t += p1;
                    if (BonusToHit >= 0)
                    {
                        t += "+";
                    }
                    t += BonusToHit;
                    t += p2;
                }
                else if (BonusDamage != 0)
                {
                    t += ' ';
                    t += p1;
                    if (BonusDamage >= 0)
                    {
                        t += "+";
                    }
                    t += BonusDamage;
                    t += p2;
                }
            }
            if (known)
            {
                if (showArmour)
                {
                    t += ' ';
                    t += b1;
                    t += BaseArmourClass;
                    t += ',';
                    if (BonusArmourClass >= 0)
                    {
                        t += "+";
                    }
                    t += BonusArmourClass;
                    t += b2;
                }
                else if (BonusArmourClass != 0)
                {
                    t += ' ';
                    t += b1;
                    if (BonusArmourClass >= 0)
                    {
                        t += "+";
                    }
                    t += BonusArmourClass;
                    t += b2;
                }
            }
            else if (showArmour)
            {
                t += ' ';
                t += b1;
                t += BaseArmourClass;
                t += b2;
            }
            if (mode < 2)
            {
                return t;
            }
            if (known && (Category == ItemCategory.Staff || Category == ItemCategory.Wand))
            {
                t += ' ';
                t += p1;
                t += TypeSpecificValue;
                t += " charge";
                if (TypeSpecificValue != 1)
                {
                    t += 's';
                }
                t += p2;
            }
            else if (known && Category == ItemCategory.Rod)
            {
                if (TypeSpecificValue != 0)
                {
                    t += " (charging)";
                }
            }
            else if (Category == ItemCategory.Light &&
                     (ItemSubCategory == Enumerations.LightType.Torch || ItemSubCategory == Enumerations.LightType.Lantern))
            {
                if (TypeSpecificValue == 1)
                {
                    t += $" (with 1 turn of light)";
                }
                else
                {
                    t += $" (with {TypeSpecificValue} turns of light)";
                }
            }
            if (known && f1.IsSet(ItemFlag1.PvalMask))
            {
                t += ' ';
                t += p1;
                if (TypeSpecificValue >= 0)
                {
                    t += "+";
                }
                t += TypeSpecificValue;
                if (f3.IsSet(ItemFlag3.HideType))
                {
                }
                else if (f1.IsSet(ItemFlag1.Speed))
                {
                    t += " speed";
                }
                else if (f1.IsSet(ItemFlag1.Blows))
                {
                    if (TypeSpecificValue > 1)
                    {
                        t += " attacks";
                    }
                    else
                    {
                        t += " attack";
                    }
                }
                else if (f1.IsSet(ItemFlag1.Stealth))
                {
                    t += " stealth";
                }
                else if (f1.IsSet(ItemFlag1.Search))
                {
                    t += " searching";
                }
                else if (f1.IsSet(ItemFlag1.Infra))
                {
                    t += " infravision";
                }
                else if (f1.IsSet(ItemFlag1.Tunnel))
                {
                }
                t += p2;
            }
            if (known && RechargeTimeLeft != 0)
            {
                t += " (charging)";
            }
            if (mode < 3)
            {
                return t;
            }
            if (!string.IsNullOrEmpty(Inscription))
            {
                tmpVal2 = Inscription;
            }
            else if (IsCursed() && (known || IdentifyFlags.IsSet(Constants.IdentSense)))
            {
                tmpVal2 = "cursed";
            }
            else if (!known && IdentifyFlags.IsSet(Constants.IdentEmpty))
            {
                tmpVal2 = "empty";
            }
            else if (!aware && IsTried())
            {
                tmpVal2 = "tried";
            }
            else if (Discount != 0)
            {
                tmpVal2 = Discount.ToString();
                tmpVal2 += "% off";
            }
            if (!string.IsNullOrEmpty(tmpVal2))
            {
                t += ' ';
                t += c1;
                t += tmpVal2;
                t += c2;
            }
            if (t.Length > 75)
            {
                t = t.Substring(0, 75);
            }
            return t;
        }

        public int FlagBasedCost(int plusses)
        {
            var total = 0;
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            GetMergedFlags(f1, f2, f3);
            if (f1.IsSet(ItemFlag1.Str))
            {
                total += 1000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Int))
            {
                total += 1000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Wis))
            {
                total += 1000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Dex))
            {
                total += 1000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Con))
            {
                total += 1000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Cha))
            {
                total += 250 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Chaotic))
            {
                total += 10000;
            }
            if (f1.IsSet(ItemFlag1.Vampiric))
            {
                total += 13000;
            }
            if (f1.IsSet(ItemFlag1.Stealth))
            {
                total += 250 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Search))
            {
                total += 100 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Infra))
            {
                total += 150 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Tunnel))
            {
                total += 175 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Speed) && plusses > 0)
            {
                total += 30000 * plusses;
            }
            if (f1.IsSet(ItemFlag1.Blows) && plusses > 0)
            {
                total += 2000 * plusses;
            }
            if (f1.IsSet(ItemFlag3.AntiTheft))
            {
                total += 0;
            }
            if (f1.IsSet(ItemFlag1.Xxx2))
            {
                total += 0;
            }
            if (f1.IsSet(ItemFlag1.SlayAnimal))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.SlayEvil))
            {
                total += 4500;
            }
            if (f1.IsSet(ItemFlag1.SlayUndead))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.SlayDemon))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.SlayOrc))
            {
                total += 3000;
            }
            if (f1.IsSet(ItemFlag1.SlayTroll))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.SlayGiant))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.SlayDragon))
            {
                total += 3500;
            }
            if (f1.IsSet(ItemFlag1.KillDragon))
            {
                total += 5500;
            }
            if (f1.IsSet(ItemFlag1.Vorpal))
            {
                total += 5000;
            }
            if (f1.IsSet(ItemFlag1.Impact))
            {
                total += 5000;
            }
            if (f1.IsSet(ItemFlag1.BrandPois))
            {
                total += 7500;
            }
            if (f1.IsSet(ItemFlag1.BrandAcid))
            {
                total += 7500;
            }
            if (f1.IsSet(ItemFlag1.BrandElec))
            {
                total += 7500;
            }
            if (f1.IsSet(ItemFlag1.BrandFire))
            {
                total += 5000;
            }
            if (f1.IsSet(ItemFlag1.BrandCold))
            {
                total += 5000;
            }
            if (f2.IsSet(ItemFlag2.SustStr))
            {
                total += 850;
            }
            if (f2.IsSet(ItemFlag2.SustInt))
            {
                total += 850;
            }
            if (f2.IsSet(ItemFlag2.SustWis))
            {
                total += 850;
            }
            if (f2.IsSet(ItemFlag2.SustDex))
            {
                total += 850;
            }
            if (f2.IsSet(ItemFlag2.SustCon))
            {
                total += 850;
            }
            if (f2.IsSet(ItemFlag2.SustCha))
            {
                total += 250;
            }
            if (f2.IsSet(ItemFlag2.Xxx1))
            {
                total += 0;
            }
            if (f2.IsSet(ItemFlag2.Xxx2))
            {
                total += 0;
            }
            if (f2.IsSet(ItemFlag2.ImAcid))
            {
                total += 10000;
            }
            if (f2.IsSet(ItemFlag2.ImElec))
            {
                total += 10000;
            }
            if (f2.IsSet(ItemFlag2.ImFire))
            {
                total += 10000;
            }
            if (f2.IsSet(ItemFlag2.ImCold))
            {
                total += 10000;
            }
            if (f2.IsSet(ItemFlag2.Xxx3))
            {
                total += 0;
            }
            if (f2.IsSet(ItemFlag2.Reflect))
            {
                total += 10000;
            }
            if (f2.IsSet(ItemFlag2.FreeAct))
            {
                total += 4500;
            }
            if (f2.IsSet(ItemFlag2.HoldLife))
            {
                total += 8500;
            }
            if (f2.IsSet(ItemFlag2.ResAcid))
            {
                total += 1250;
            }
            if (f2.IsSet(ItemFlag2.ResElec))
            {
                total += 1250;
            }
            if (f2.IsSet(ItemFlag2.ResFire))
            {
                total += 1250;
            }
            if (f2.IsSet(ItemFlag2.ResCold))
            {
                total += 1250;
            }
            if (f2.IsSet(ItemFlag2.ResPois))
            {
                total += 2500;
            }
            if (f2.IsSet(ItemFlag2.ResFear))
            {
                total += 2500;
            }
            if (f2.IsSet(ItemFlag2.ResLight))
            {
                total += 1750;
            }
            if (f2.IsSet(ItemFlag2.ResDark))
            {
                total += 1750;
            }
            if (f2.IsSet(ItemFlag2.ResBlind))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResConf))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResSound))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResShards))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResNether))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResNexus))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResChaos))
            {
                total += 2000;
            }
            if (f2.IsSet(ItemFlag2.ResDisen))
            {
                total += 10000;
            }
            if (f3.IsSet(ItemFlag3.ShFire))
            {
                total += 5000;
            }
            if (f3.IsSet(ItemFlag3.ShElec))
            {
                total += 5000;
            }
            if (f3.IsSet(ItemFlag3.Xxx3))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag1.Xxx1))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag3.NoTele))
            {
                total += 2500;
            }
            if (f3.IsSet(ItemFlag3.NoMagic))
            {
                total += 2500;
            }
            if (f3.IsSet(ItemFlag3.Wraith))
            {
                total += 250000;
            }
            if (f3.IsSet(ItemFlag3.DreadCurse))
            {
                total -= 15000;
            }
            if (f3.IsSet(ItemFlag3.EasyKnow))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag3.HideType))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag3.ShowMods))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag3.InstaArt))
            {
                total += 0;
            }
            if (f3.IsSet(ItemFlag3.Feather))
            {
                total += 1250;
            }
            if (f3.IsSet(ItemFlag3.Lightsource))
            {
                total += 1250;
            }
            if (f3.IsSet(ItemFlag3.SeeInvis))
            {
                total += 2000;
            }
            if (f3.IsSet(ItemFlag3.Telepathy))
            {
                total += 12500;
            }
            if (f3.IsSet(ItemFlag3.SlowDigest))
            {
                total += 750;
            }
            if (f3.IsSet(ItemFlag3.Regen))
            {
                total += 2500;
            }
            if (f3.IsSet(ItemFlag3.XtraMight))
            {
                total += 2250;
            }
            if (f3.IsSet(ItemFlag3.XtraShots))
            {
                total += 10000;
            }
            if (f3.IsSet(ItemFlag3.IgnoreAcid))
            {
                total += 100;
            }
            if (f3.IsSet(ItemFlag3.IgnoreElec))
            {
                total += 100;
            }
            if (f3.IsSet(ItemFlag3.IgnoreFire))
            {
                total += 100;
            }
            if (f3.IsSet(ItemFlag3.IgnoreCold))
            {
                total += 100;
            }
            if (f3.IsSet(ItemFlag3.Activate))
            {
                total += 100;
            }
            if (f3.IsSet(ItemFlag3.DrainExp))
            {
                total -= 12500;
            }
            if (f3.IsSet(ItemFlag3.Teleport))
            {
                if (IdentifyFlags.IsSet(Constants.IdentCursed))
                {
                    total -= 7500;
                }
                else
                {
                    total += 250;
                }
            }
            if (f3.IsSet(ItemFlag3.Aggravate))
            {
                total -= 10000;
            }
            if (f3.IsSet(ItemFlag3.Blessed))
            {
                total += 750;
            }
            if (f3.IsSet(ItemFlag3.Cursed))
            {
                total -= 5000;
            }
            if (f3.IsSet(ItemFlag3.HeavyCurse))
            {
                total -= 12500;
            }
            if (f3.IsSet(ItemFlag3.PermaCurse))
            {
                total -= 15000;
            }
            if (IsLegendary() && LegendaryFlags3.IsSet(ItemFlag3.Activate))
            {
                var type = BonusPowerSubType;
                if (type == LegendaryPower.ActSunlight)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBoMiss1)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBaPois1)
                {
                    total += 300;
                }
                else if (type == LegendaryPower.ActBoElec1)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBoAcid1)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBoCold1)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBoFire1)
                {
                    total += 250;
                }
                else if (type == LegendaryPower.ActBaCold1)
                {
                    total += 750;
                }
                else if (type == LegendaryPower.ActBaFire1)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActDrain1)
                {
                    total += 500;
                }
                else if (type == LegendaryPower.ActBaCold2)
                {
                    total += 1250;
                }
                else if (type == LegendaryPower.ActBaElec2)
                {
                    total += 1500;
                }
                else if (type == LegendaryPower.ActBaElec3)
                {
                    total += 2000;
                }
                else if (type == LegendaryPower.ActDrain2)
                {
                    total += 750;
                }
                else if (type == LegendaryPower.ActVampire1)
                {
                    total = 1000;
                }
                else if (type == LegendaryPower.ActBoMiss2)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActBaFire2)
                {
                    total += 1750;
                }
                else if (type == LegendaryPower.ActBaCold3)
                {
                    total += 2500;
                }
                else if (type == LegendaryPower.ActBaElec3)
                {
                    total += 2500;
                }
                else if (type == LegendaryPower.ActWhirlwind)
                {
                    total += 7500;
                }
                else if (type == LegendaryPower.ActVampire2)
                {
                    total += 2500;
                }
                else if (type == LegendaryPower.ActCallChaos)
                {
                    total += 5000;
                }
                else if (type == LegendaryPower.ActShard)
                {
                    total += 5000;
                }
                else if (type == LegendaryPower.ActDispEvil)
                {
                    total += 4000;
                }
                else if (type == LegendaryPower.ActDispGood)
                {
                    total += 3500;
                }
                else if (type == LegendaryPower.ActBaMiss3)
                {
                    total += 5000;
                }
                else if (type == LegendaryPower.ActConfuse)
                {
                    total += 500;
                }
                else if (type == LegendaryPower.ActSleep)
                {
                    total += 750;
                }
                else if (type == LegendaryPower.ActQuake)
                {
                    total += 600;
                }
                else if (type == LegendaryPower.ActTerror)
                {
                    total += 2500;
                }
                else if (type == LegendaryPower.ActTeleAway)
                {
                    total += 2000;
                }
                else if (type == LegendaryPower.ActBanishEvil)
                {
                    total += 3000;
                }
                else if (type == LegendaryPower.ActCarnage)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActMassGeno)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActCharmAnimal)
                {
                    total += 7500;
                }
                else if (type == LegendaryPower.ActCharmUndead)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActCharmOther)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActCharmAnimals)
                {
                    total += 12500;
                }
                else if (type == LegendaryPower.ActCharmOthers)
                {
                    total += 17500;
                }
                else if (type == LegendaryPower.ActSummonAnimal)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActSummonPhantom)
                {
                    total += 12000;
                }
                else if (type == LegendaryPower.ActSummonElemental)
                {
                    total += 15000;
                }
                else if (type == LegendaryPower.ActSummonDemon)
                {
                    total += 20000;
                }
                else if (type == LegendaryPower.ActSummonUndead)
                {
                    total += 20000;
                }
                else if (type == LegendaryPower.ActCureLw)
                {
                    total += 500;
                }
                else if (type == LegendaryPower.ActCureMw)
                {
                    total += 750;
                }
                else if (type == LegendaryPower.ActCurePoison)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActRestLife)
                {
                    total += 7500;
                }
                else if (type == LegendaryPower.ActRestAll)
                {
                    total += 15000;
                }
                else if (type == LegendaryPower.ActCure700)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActCure1000)
                {
                    total += 15000;
                }
                else if (type == LegendaryPower.ActEsp)
                {
                    total += 1500;
                }
                else if (type == LegendaryPower.ActBerserk)
                {
                    total += 800;
                }
                else if (type == LegendaryPower.ActProtEvil)
                {
                    total += 5000;
                }
                else if (type == LegendaryPower.ActResistAll)
                {
                    total += 5000;
                }
                else if (type == LegendaryPower.ActSpeed)
                {
                    total += 15000;
                }
                else if (type == LegendaryPower.ActXtraSpeed)
                {
                    total += 25000;
                }
                else if (type == LegendaryPower.ActWraith)
                {
                    total += 25000;
                }
                else if (type == LegendaryPower.ActInvuln)
                {
                    total += 25000;
                }
                else if (type == LegendaryPower.ActLight)
                {
                    total += 150;
                }
                else if (type == LegendaryPower.ActMapLight)
                {
                    total += 500;
                }
                else if (type == LegendaryPower.ActDetectAll)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActDetectXtra)
                {
                    total += 12500;
                }
                else if (type == LegendaryPower.ActIdFull)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActIdPlain)
                {
                    total += 1250;
                }
                else if (type == LegendaryPower.ActRuneExplo)
                {
                    total += 4000;
                }
                else if (type == LegendaryPower.ActRuneProt)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActSatiate)
                {
                    total += 2000;
                }
                else if (type == LegendaryPower.ActDestDoor)
                {
                    total += 100;
                }
                else if (type == LegendaryPower.ActStoneMud)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActRecharge)
                {
                    total += 1000;
                }
                else if (type == LegendaryPower.ActAlchemy)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActDimDoor)
                {
                    total += 10000;
                }
                else if (type == LegendaryPower.ActTeleport)
                {
                    total += 2000;
                }
                else if (type == LegendaryPower.ActRecall)
                {
                    total += 7500;
                }
            }
            return total;
        }

        public void GetArtifactResistances()
        {
            var forge = new ItemForge(this);
            forge.GetArtifactResistances();
        }

        public string GetDetailedFeeling()
        {
            if (IsArtifact() || IsLegendary())
            {
                if (IsCursed() || IsBroken())
                {
                    return "terrible";
                }
                return "special";
            }
            if (IsRare())
            {
                if (IsCursed() || IsBroken())
                {
                    return "worthless";
                }
                return "excellent";
            }
            if (IsCursed())
            {
                return "cursed";
            }
            if (IsBroken())
            {
                return "broken";
            }
            if (BonusArmourClass > 0)
            {
                return "good";
            }
            if (BonusToHit + BonusDamage > 0)
            {
                return "good";
            }
            return "average";
        }

        public void GetMergedFlags(FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f1.Clear();
            f2.Clear();
            f3.Clear();
            if (ItemType == null)
            {
                return;
            }
            f1.Set(ItemType.Flags1);
            f2.Set(ItemType.Flags2);
            f3.Set(ItemType.Flags3);
            if (ArtifactIndex != 0)
            {
                var aPtr = Profile.Instance.Artifacts[ArtifactIndex];
                f1.Set(aPtr.Flags1);
                f2.Set(aPtr.Flags2);
                f3.Set(aPtr.Flags3);
            }
            if (RareItemTypeIndex != Enumerations.RareItemType.None)
            {
                var ePtr = Profile.Instance.RareItemTypes[RareItemTypeIndex];
                f1.Set(ePtr.Flags1);
                f2.Set(ePtr.Flags2);
                f3.Set(ePtr.Flags3);
            }
            if (LegendaryFlags1.IsSet() || LegendaryFlags2.IsSet() || LegendaryFlags3.IsSet())
            {
                f1.Set(LegendaryFlags1);
                f2.Set(LegendaryFlags2);
                f3.Set(LegendaryFlags3);
            }
            if (IsLegendary())
            {
                switch (BonusPowerType)
                {
                    case Enumerations.RareItemType.SpecialSustain:
                        {
                            switch (BonusPowerSubType % 6)
                            {
                                case 0:
                                    f2.Set(ItemFlag2.SustStr);
                                    break;

                                case 1:
                                    f2.Set(ItemFlag2.SustInt);
                                    break;

                                case 2:
                                    f2.Set(ItemFlag2.SustWis);
                                    break;

                                case 3:
                                    f2.Set(ItemFlag2.SustDex);
                                    break;

                                case 4:
                                    f2.Set(ItemFlag2.SustCon);
                                    break;

                                case 5:
                                    f2.Set(ItemFlag2.SustCha);
                                    break;
                            }
                            break;
                        }
                    case Enumerations.RareItemType.SpecialPower:
                        {
                            switch (BonusPowerSubType % 11)
                            {
                                case 0:
                                    f2.Set(ItemFlag2.ResBlind);
                                    break;

                                case 1:
                                    f2.Set(ItemFlag2.ResConf);
                                    break;

                                case 2:
                                    f2.Set(ItemFlag2.ResSound);
                                    break;

                                case 3:
                                    f2.Set(ItemFlag2.ResShards);
                                    break;

                                case 4:
                                    f2.Set(ItemFlag2.ResNether);
                                    break;

                                case 5:
                                    f2.Set(ItemFlag2.ResNexus);
                                    break;

                                case 6:
                                    f2.Set(ItemFlag2.ResChaos);
                                    break;

                                case 7:
                                    f2.Set(ItemFlag2.ResDisen);
                                    break;

                                case 8:
                                    f2.Set(ItemFlag2.ResPois);
                                    break;

                                case 9:
                                    f2.Set(ItemFlag2.ResDark);
                                    break;

                                case 10:
                                    f2.Set(ItemFlag2.ResLight);
                                    break;
                            }
                            break;
                        }
                    case Enumerations.RareItemType.SpecialAbility:
                        {
                            switch (BonusPowerSubType % 8)
                            {
                                case 0:
                                    f3.Set(ItemFlag3.Feather);
                                    break;

                                case 1:
                                    f3.Set(ItemFlag3.Lightsource);
                                    break;

                                case 2:
                                    f3.Set(ItemFlag3.SeeInvis);
                                    break;

                                case 3:
                                    f3.Set(ItemFlag3.Telepathy);
                                    break;

                                case 4:
                                    f3.Set(ItemFlag3.SlowDigest);
                                    break;

                                case 5:
                                    f3.Set(ItemFlag3.Regen);
                                    break;

                                case 6:
                                    f2.Set(ItemFlag2.FreeAct);
                                    break;

                                case 7:
                                    f2.Set(ItemFlag2.HoldLife);
                                    break;
                            }
                            break;
                        }
                }
            }
        }

        public string GetVagueFeeling()
        {
            if (IsCursed())
            {
                return "cursed";
            }
            if (IsBroken())
            {
                return "broken";
            }
            if (IsArtifact() || IsLegendary())
            {
                return "special";
            }
            if (IsRare())
            {
                return "good";
            }
            if (BonusArmourClass > 0)
            {
                return "good";
            }
            if (BonusToHit + BonusDamage > 0)
            {
                return "good";
            }
            return null;
        }

        public bool HatesAcid()
        {
            switch (Category)
            {
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                case ItemCategory.Bow:
                case ItemCategory.Sword:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Helm:
                case ItemCategory.Crown:
                case ItemCategory.Shield:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Cloak:
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                    {
                        return true;
                    }
                case ItemCategory.Staff:
                case ItemCategory.Scroll:
                    {
                        return true;
                    }
                case ItemCategory.Chest:
                    {
                        return true;
                    }
                case ItemCategory.Skeleton:
                case ItemCategory.Bottle:
                case ItemCategory.Junk:
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool HatesCold()
        {
            switch (Category)
            {
                case ItemCategory.Potion:
                case ItemCategory.Flask:
                case ItemCategory.Bottle:
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool HatesElec()
        {
            switch (Category)
            {
                case ItemCategory.Ring:
                case ItemCategory.Wand:
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool HatesFire()
        {
            switch (Category)
            {
                case ItemCategory.Light:
                case ItemCategory.Arrow:
                case ItemCategory.Bow:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Cloak:
                case ItemCategory.SoftArmor:
                    {
                        return true;
                    }
                case ItemCategory.LifeBook:
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.TarotBook:
                case ItemCategory.FolkBook:
                case ItemCategory.CorporealBook:
                    {
                        return true;
                    }
                case ItemCategory.Chest:
                    {
                        return true;
                    }
                case ItemCategory.Staff:
                case ItemCategory.Scroll:
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool IdentifyFully()
        {
            int i = 0, j, k;
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            var info = new string[128];
            GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.Activate))
            {
                info[i++] = "It can be activated for...";
                info[i++] = DescribeActivationEffect();
                info[i++] = "...if it is being worn.";
            }
            if (Category == ItemCategory.Light)
            {
                if (IsArtifact())
                {
                    info[i++] = "It provides light (radius 3) forever.";
                }
                else if (ItemSubCategory == Enumerations.LightType.Lantern)
                {
                    info[i++] = "It provides light (radius 2) when fueled.";
                }
                else if (ItemSubCategory == Enumerations.LightType.Torch)
                {
                    info[i++] = "It provides light (radius 1) when fueled.";
                }
                else
                {
                    info[i++] = "It provides light (radius 2) forever.";
                }
            }
            if (f1.IsSet(ItemFlag1.Str))
            {
                info[i++] = "It affects your strength.";
            }
            if (f1.IsSet(ItemFlag1.Int))
            {
                info[i++] = "It affects your intelligence.";
            }
            if (f1.IsSet(ItemFlag1.Wis))
            {
                info[i++] = "It affects your wisdom.";
            }
            if (f1.IsSet(ItemFlag1.Dex))
            {
                info[i++] = "It affects your dexterity.";
            }
            if (f1.IsSet(ItemFlag1.Con))
            {
                info[i++] = "It affects your constitution.";
            }
            if (f1.IsSet(ItemFlag1.Cha))
            {
                info[i++] = "It affects your charisma.";
            }
            if (f1.IsSet(ItemFlag1.Stealth))
            {
                info[i++] = "It affects your stealth.";
            }
            if (f1.IsSet(ItemFlag1.Search))
            {
                info[i++] = "It affects your searching.";
            }
            if (f1.IsSet(ItemFlag1.Infra))
            {
                info[i++] = "It affects your infravision.";
            }
            if (f1.IsSet(ItemFlag1.Tunnel))
            {
                info[i++] = "It affects your ability to tunnel.";
            }
            if (f1.IsSet(ItemFlag1.Speed))
            {
                info[i++] = "It affects your movement speed.";
            }
            if (f1.IsSet(ItemFlag1.Blows))
            {
                info[i++] = "It affects your attack speed.";
            }
            if (f1.IsSet(ItemFlag1.BrandAcid))
            {
                info[i++] = "It does extra damage from acid.";
            }
            if (f1.IsSet(ItemFlag1.BrandElec))
            {
                info[i++] = "It does extra damage from electricity.";
            }
            if (f1.IsSet(ItemFlag1.BrandFire))
            {
                info[i++] = "It does extra damage from fire.";
            }
            if (f1.IsSet(ItemFlag1.BrandCold))
            {
                info[i++] = "It does extra damage from frost.";
            }
            if (f1.IsSet(ItemFlag1.BrandPois))
            {
                info[i++] = "It poisons your foes.";
            }
            if (f1.IsSet(ItemFlag1.Chaotic))
            {
                info[i++] = "It produces chaotic effects.";
            }
            if (f1.IsSet(ItemFlag1.Vampiric))
            {
                info[i++] = "It drains life from your foes.";
            }
            if (f1.IsSet(ItemFlag1.Impact))
            {
                info[i++] = "It can cause earthquakes.";
            }
            if (f1.IsSet(ItemFlag1.Vorpal))
            {
                info[i++] = "It is very sharp and can cut your foes.";
            }
            if (f1.IsSet(ItemFlag1.KillDragon))
            {
                info[i++] = "It is a great bane of dragons.";
            }
            else if (f1.IsSet(ItemFlag1.SlayDragon))
            {
                info[i++] = "It is especially deadly against dragons.";
            }
            if (f1.IsSet(ItemFlag1.SlayOrc))
            {
                info[i++] = "It is especially deadly against orcs.";
            }
            if (f1.IsSet(ItemFlag1.SlayTroll))
            {
                info[i++] = "It is especially deadly against trolls.";
            }
            if (f1.IsSet(ItemFlag1.SlayGiant))
            {
                info[i++] = "It is especially deadly against giants.";
            }
            if (f1.IsSet(ItemFlag1.SlayDemon))
            {
                info[i++] = "It strikes at demons with holy wrath.";
            }
            if (f1.IsSet(ItemFlag1.SlayUndead))
            {
                info[i++] = "It strikes at undead with holy wrath.";
            }
            if (f1.IsSet(ItemFlag1.SlayEvil))
            {
                info[i++] = "It fights against evil with holy fury.";
            }
            if (f1.IsSet(ItemFlag1.SlayAnimal))
            {
                info[i++] = "It is especially deadly against natural creatures.";
            }
            if (f2.IsSet(ItemFlag2.SustStr))
            {
                info[i++] = "It sustains your strength.";
            }
            if (f2.IsSet(ItemFlag2.SustInt))
            {
                info[i++] = "It sustains your intelligence.";
            }
            if (f2.IsSet(ItemFlag2.SustWis))
            {
                info[i++] = "It sustains your wisdom.";
            }
            if (f2.IsSet(ItemFlag2.SustDex))
            {
                info[i++] = "It sustains your dexterity.";
            }
            if (f2.IsSet(ItemFlag2.SustCon))
            {
                info[i++] = "It sustains your constitution.";
            }
            if (f2.IsSet(ItemFlag2.SustCha))
            {
                info[i++] = "It sustains your charisma.";
            }
            if (f2.IsSet(ItemFlag2.ImAcid))
            {
                info[i++] = "It provides immunity to acid.";
            }
            if (f2.IsSet(ItemFlag2.ImElec))
            {
                info[i++] = "It provides immunity to electricity.";
            }
            if (f2.IsSet(ItemFlag2.ImFire))
            {
                info[i++] = "It provides immunity to fire.";
            }
            if (f2.IsSet(ItemFlag2.ImCold))
            {
                info[i++] = "It provides immunity to cold.";
            }
            if (f2.IsSet(ItemFlag2.FreeAct))
            {
                info[i++] = "It provides immunity to paralysis.";
            }
            if (f2.IsSet(ItemFlag2.HoldLife))
            {
                info[i++] = "It provides resistance to life draining.";
            }
            if (f2.IsSet(ItemFlag2.ResFear))
            {
                info[i++] = "It makes you completely fearless.";
            }
            if (f2.IsSet(ItemFlag2.ResAcid))
            {
                info[i++] = "It provides resistance to acid.";
            }
            if (f2.IsSet(ItemFlag2.ResElec))
            {
                info[i++] = "It provides resistance to electricity.";
            }
            if (f2.IsSet(ItemFlag2.ResFire))
            {
                info[i++] = "It provides resistance to fire.";
            }
            if (f2.IsSet(ItemFlag2.ResCold))
            {
                info[i++] = "It provides resistance to cold.";
            }
            if (f2.IsSet(ItemFlag2.ResPois))
            {
                info[i++] = "It provides resistance to poison.";
            }
            if (f2.IsSet(ItemFlag2.ResLight))
            {
                info[i++] = "It provides resistance to light.";
            }
            if (f2.IsSet(ItemFlag2.ResDark))
            {
                info[i++] = "It provides resistance to dark.";
            }
            if (f2.IsSet(ItemFlag2.ResBlind))
            {
                info[i++] = "It provides resistance to blindness.";
            }
            if (f2.IsSet(ItemFlag2.ResConf))
            {
                info[i++] = "It provides resistance to confusion.";
            }
            if (f2.IsSet(ItemFlag2.ResSound))
            {
                info[i++] = "It provides resistance to sound.";
            }
            if (f2.IsSet(ItemFlag2.ResShards))
            {
                info[i++] = "It provides resistance to shards.";
            }
            if (f2.IsSet(ItemFlag2.ResNether))
            {
                info[i++] = "It provides resistance to nether.";
            }
            if (f2.IsSet(ItemFlag2.ResNexus))
            {
                info[i++] = "It provides resistance to nexus.";
            }
            if (f2.IsSet(ItemFlag2.ResChaos))
            {
                info[i++] = "It provides resistance to chaos.";
            }
            if (f2.IsSet(ItemFlag2.ResDisen))
            {
                info[i++] = "It provides resistance to disenchantment.";
            }
            if (f3.IsSet(ItemFlag3.Wraith))
            {
                info[i++] = "It renders you incorporeal.";
            }
            if (f3.IsSet(ItemFlag3.Feather))
            {
                info[i++] = "It allows you to levitate.";
            }
            if (f3.IsSet(ItemFlag3.Lightsource))
            {
                info[i++] = "It provides permanent light.";
            }
            if (f3.IsSet(ItemFlag3.SeeInvis))
            {
                info[i++] = "It allows you to see invisible monsters.";
            }
            if (f3.IsSet(ItemFlag3.Telepathy))
            {
                info[i++] = "It gives telepathic powers.";
            }
            if (f3.IsSet(ItemFlag3.SlowDigest))
            {
                info[i++] = "It slows your metabolism.";
            }
            if (f3.IsSet(ItemFlag3.Regen))
            {
                info[i++] = "It speeds your regenerative powers.";
            }
            if (f2.IsSet(ItemFlag2.Reflect))
            {
                info[i++] = "It reflects bolts and arrows.";
            }
            if (f3.IsSet(ItemFlag3.ShFire))
            {
                info[i++] = "It produces a fiery sheath.";
            }
            if (f3.IsSet(ItemFlag3.ShElec))
            {
                info[i++] = "It produces an electric sheath.";
            }
            if (f3.IsSet(ItemFlag3.NoMagic))
            {
                info[i++] = "It produces an anti-magic shell.";
            }
            if (f3.IsSet(ItemFlag3.NoTele))
            {
                info[i++] = "It prevents teleportation.";
            }
            if (f3.IsSet(ItemFlag3.XtraMight))
            {
                info[i++] = "It fires missiles with extra might.";
            }
            if (f3.IsSet(ItemFlag3.XtraShots))
            {
                info[i++] = "It fires missiles excessively fast.";
            }
            if (f3.IsSet(ItemFlag3.DrainExp))
            {
                info[i++] = "It drains experience.";
            }
            if (f3.IsSet(ItemFlag3.Teleport))
            {
                info[i++] = "It induces random teleportation.";
            }
            if (f3.IsSet(ItemFlag3.Aggravate))
            {
                info[i++] = "It aggravates nearby creatures.";
            }
            if (f3.IsSet(ItemFlag3.Blessed))
            {
                info[i++] = "It has been blessed by the gods.";
            }
            if (IsCursed())
            {
                if (f3.IsSet(ItemFlag3.PermaCurse))
                {
                    info[i++] = "It is permanently cursed.";
                }
                else if (f3.IsSet(ItemFlag3.HeavyCurse))
                {
                    info[i++] = "It is heavily cursed.";
                }
                else
                {
                    info[i++] = "It is cursed.";
                }
            }
            if (f3.IsSet(ItemFlag3.DreadCurse))
            {
                info[i++] = "It carries an ancient foul curse.";
            }
            if (f3.IsSet(ItemFlag3.IgnoreAcid))
            {
                info[i++] = "It cannot be harmed by acid.";
            }
            if (f3.IsSet(ItemFlag3.IgnoreElec))
            {
                info[i++] = "It cannot be harmed by electricity.";
            }
            if (f3.IsSet(ItemFlag3.IgnoreFire))
            {
                info[i++] = "It cannot be harmed by fire.";
            }
            if (f3.IsSet(ItemFlag3.IgnoreCold))
            {
                info[i++] = "It cannot be harmed by cold.";
            }
            if (i == 0)
            {
                return false;
            }
            Gui.Save();
            for (k = 1; k < 24; k++)
            {
                Gui.PrintLine("", k, 13);
            }
            Gui.PrintLine("     Item Attributes:", 1, 15);
            for (k = 2, j = 0; j < i; j++)
            {
                Gui.PrintLine(info[j], k++, 15);
                if (k == 22 && j + 1 < i)
                {
                    Gui.PrintLine("-- more --", k, 15);
                    Gui.Inkey();
                    for (; k > 2; k--)
                    {
                        Gui.PrintLine("", k, 15);
                    }
                }
            }
            Gui.PrintLine("[Press any key to continue]", k, 15);
            Gui.Inkey();
            Gui.Load();
            return true;
        }

        public bool IsArtifact()
        {
            return ArtifactIndex != 0;
        }

        public bool IsBroken()
        {
            return IdentifyFlags.IsSet(Constants.IdentBroken);
        }

        public bool IsCursed()
        {
            return IdentifyFlags.IsSet(Constants.IdentCursed);
        }

        public bool IsFlavourAware()
        {
            if (ItemType == null)
            {
                return false;
            }
            return ItemType.FlavourAware;
        }

        public bool IsKnown()
        {
            if (ItemType == null)
            {
                return false;
            }
            if (IdentifyFlags.IsSet(Constants.IdentKnown))
            {
                return true;
            }
            if (ItemType.EasyKnow && ItemType.FlavourAware)
            {
                return true;
            }
            return false;
        }

        public bool IsLegendary()
        {
            return !string.IsNullOrEmpty(LegendaryName);
        }

        public bool IsRare()
        {
            return RareItemTypeIndex != 0;
        }

        public bool MakeGold()
        {
            var i = ((Program.Rng.DieRoll(SaveGame.Instance.Level.ObjectLevel + 2) + 2) / 2) - 1;
            if (Program.Rng.RandomLessThan(Constants.GreatObj) == 0)
            {
                i += Program.Rng.DieRoll(SaveGame.Instance.Level.ObjectLevel + 1);
            }
            if (CoinType != 0)
            {
                i = CoinType;
            }
            if (i >= Constants.MaxGold)
            {
                i = Constants.MaxGold - 1;
            }
            ItemType kPtr = null;
            switch (i)
            {
                case 0:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.CopperLow);
                    break;

                case 1:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.CopperMed);
                    break;

                case 2:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.CopperHi);
                    break;

                case 3:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.SilverLow);
                    break;

                case 4:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.SilverMed);
                    break;

                case 5:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.SilverHi);
                    break;

                case 6:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.GarnetsLow);
                    break;

                case 7:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.GarnetsHi);
                    break;

                case 8:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.GoldLow);
                    break;

                case 9:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.GoldMed);
                    break;

                case 10:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.GoldHigh);
                    break;

                case 11:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Opals);
                    break;

                case 12:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Sapphires);
                    break;

                case 13:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Rubies);
                    break;

                case 14:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Diamonds);
                    break;

                case 15:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Emeralds);
                    break;

                case 16:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Mithril);
                    break;

                case 17:
                    kPtr = Profile.Instance.ItemTypes.LookupKind(ItemCategory.Gold, Enumerations.MoneyType.Adamantite);
                    break;
            }
            if (kPtr == null)
            {
                return false;
            }
            AssignItemType(kPtr);
            var bbase = kPtr.Cost;
            TypeSpecificValue = bbase + (8 * Program.Rng.DieRoll(bbase)) + Program.Rng.DieRoll(8);
            return true;
        }

        public bool MakeObject(bool good, bool great)
        {
            var forge = new ItemForge(this);
            return forge.MakeObject(good, great);
        }

        public void ObjectFlagsKnown(FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f1.Clear();
            f2.Clear();
            f3.Clear();
            if (!IsKnown())
            {
                return;
            }
            GetMergedFlags(f1, f2, f3);
        }

        public void ObjectTried()
        {
            ItemType.Tried = true;
        }

        public int RealValue()
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            var kPtr = ItemType;
            if (kPtr.Cost == 0)
            {
                return 0;
            }
            var value = kPtr.Cost;
            GetMergedFlags(f1, f2, f3);
            if (LegendaryFlags1.IsSet() || LegendaryFlags2.IsSet() || LegendaryFlags3.IsSet())
            {
                value += FlagBasedCost(TypeSpecificValue);
            }
            else if (ArtifactIndex != 0)
            {
                var aPtr = Profile.Instance.Artifacts[ArtifactIndex];
                if (aPtr.Cost == 0)
                {
                    return 0;
                }
                value = aPtr.Cost;
            }
            else if (RareItemTypeIndex != Enumerations.RareItemType.None)
            {
                var ePtr = Profile.Instance.RareItemTypes[RareItemTypeIndex];
                if (ePtr.Cost == 0)
                {
                    return 0;
                }
                value += ePtr.Cost;
            }
            switch (Category)
            {
                case ItemCategory.Shot:
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                case ItemCategory.Bow:
                case ItemCategory.Digging:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Helm:
                case ItemCategory.Crown:
                case ItemCategory.Shield:
                case ItemCategory.Cloak:
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                case ItemCategory.Light:
                case ItemCategory.Amulet:
                case ItemCategory.Ring:
                    {
                        if (TypeSpecificValue < 0)
                        {
                            return 0;
                        }
                        if (TypeSpecificValue == 0)
                        {
                            break;
                        }
                        if (f1.IsSet(ItemFlag1.Str))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Int))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Wis))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Dex))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Con))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Cha))
                        {
                            value += TypeSpecificValue * 200;
                        }
                        if (f1.IsSet(ItemFlag1.Stealth))
                        {
                            value += TypeSpecificValue * 100;
                        }
                        if (f1.IsSet(ItemFlag1.Search))
                        {
                            value += TypeSpecificValue * 100;
                        }
                        if (f1.IsSet(ItemFlag1.Infra))
                        {
                            value += TypeSpecificValue * 50;
                        }
                        if (f1.IsSet(ItemFlag1.Tunnel))
                        {
                            value += TypeSpecificValue * 50;
                        }
                        if (f1.IsSet(ItemFlag1.Blows))
                        {
                            value += TypeSpecificValue * 5000;
                        }
                        if (f1.IsSet(ItemFlag1.Speed))
                        {
                            value += TypeSpecificValue * 3000;
                        }
                        break;
                    }
            }
            switch (Category)
            {
                case ItemCategory.Wand:
                case ItemCategory.Staff:
                    {
                        value += value / 20 * TypeSpecificValue;
                        break;
                    }
                case ItemCategory.Ring:
                case ItemCategory.Amulet:
                    {
                        if (BonusArmourClass < 0)
                        {
                            return 0;
                        }
                        if (BonusToHit < 0)
                        {
                            return 0;
                        }
                        if (BonusDamage < 0)
                        {
                            return 0;
                        }
                        value += (BonusToHit + BonusDamage + BonusArmourClass) * 100;
                        break;
                    }
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Cloak:
                case ItemCategory.Crown:
                case ItemCategory.Helm:
                case ItemCategory.Shield:
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.DragArmor:
                    {
                        if (BonusArmourClass < 0)
                        {
                            return 0;
                        }
                        value += (BonusToHit + BonusDamage + BonusArmourClass) * 100;
                        break;
                    }
                case ItemCategory.Bow:
                case ItemCategory.Digging:
                case ItemCategory.Hafted:
                case ItemCategory.Sword:
                case ItemCategory.Polearm:
                    {
                        if (BonusToHit + BonusDamage < 0)
                        {
                            return 0;
                        }
                        value += (BonusToHit + BonusDamage + BonusArmourClass) * 100;
                        if (DamageDice > kPtr.Dd && DamageDiceSides == kPtr.Ds)
                        {
                            value += (DamageDice - kPtr.Dd) * DamageDiceSides * 100;
                        }
                        break;
                    }
                case ItemCategory.Shot:
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                    {
                        if (BonusToHit + BonusDamage < 0)
                        {
                            return 0;
                        }
                        value += (BonusToHit + BonusDamage) * 5;
                        if (DamageDice > kPtr.Dd && DamageDiceSides == kPtr.Ds)
                        {
                            value += (DamageDice - kPtr.Dd) * DamageDiceSides * 5;
                        }
                        break;
                    }
            }
            return value;
        }

        public bool Stompable()
        {
            var t = this;
            var kPtr = ItemType;
            if (!IsKnown())
            {
                if (Inventory.ObjectHasFlavor(ItemType))
                {
                    if (IsFlavourAware())
                    {
                        return kPtr.Stompable[0];
                    }
                }
                if (IdentifyFlags.IsClear(Constants.IdentSense))
                {
                    return false;
                }
            }
            if (kPtr.Category == ItemCategory.Ring || kPtr.Category == ItemCategory.Amulet)
            {
                if (BonusDamage < 0 || BonusArmourClass < 0 || BonusToHit < 0 || TypeSpecificValue < 0)
                {
                    return true;
                }
            }
            if (kPtr.HasQuality())
            {
                switch (GetDetailedFeeling())
                {
                    case "terrible":
                    case "worthless":
                    case "cursed":
                    case "broken":
                        return kPtr.Stompable[0];

                    case "average":
                        return kPtr.Stompable[1];

                    case "good":
                        return kPtr.Stompable[2];

                    case "excellent":
                        return kPtr.Stompable[3];

                    case "special":
                        return false;

                    default:
                        throw new InvalidDataException($"Unrecognised item quality ({GetDetailedFeeling()})");
                }
            }
            if (kPtr.Category == ItemCategory.Chest)
            {
                if (!IsKnown())
                {
                    return false;
                }
                else if (TypeSpecificValue == 0)
                {
                    return kPtr.Stompable[0];
                }
                else if (TypeSpecificValue < 0)
                {
                    return kPtr.Stompable[1];
                }
                else
                {
                    switch (GlobalData.ChestTraps[TypeSpecificValue])
                    {
                        case 0:
                            {
                                return kPtr.Stompable[2];
                            }
                        default:
                            {
                                return kPtr.Stompable[3];
                            }
                    }
                }
            }
            return kPtr.Stompable[0];
        }

        public string StoreDescription(bool pref, int mode)
        {
            var hackAware = ItemType.FlavourAware;
            var hackKnown = IdentifyFlags.IsSet(Constants.IdentKnown);
            IdentifyFlags.Set(Constants.IdentKnown);
            ItemType.FlavourAware = true;
            var buf = Description(pref, mode);
            ItemType.FlavourAware = hackAware;
            if (!hackKnown)
            {
                IdentifyFlags.Clear(Constants.IdentKnown);
            }
            return buf;
        }

        public override string ToString()
        {
            return Description(false, 0);
        }

        public int Value()
        {
            int value;
            if (IsKnown())
            {
                if (IsBroken())
                {
                    return 0;
                }
                if (IsCursed())
                {
                    return 0;
                }
                value = RealValue();
            }
            else
            {
                if (IdentifyFlags.IsSet(Constants.IdentSense) && IsBroken())
                {
                    return 0;
                }
                if (IdentifyFlags.IsSet(Constants.IdentSense) && IsCursed())
                {
                    return 0;
                }
                value = BaseValue();
            }
            if (Discount != 0)
            {
                value -= value * Discount / 100;
            }
            return value;
        }

        private int BaseValue()
        {
            var kPtr = ItemType;
            if (IsFlavourAware())
            {
                return kPtr.Cost;
            }
            switch (Category)
            {
                case ItemCategory.Food:
                    return 5;

                case ItemCategory.Potion:
                    return 20;

                case ItemCategory.Scroll:
                    return 20;

                case ItemCategory.Staff:
                    return 70;

                case ItemCategory.Wand:
                    return 50;

                case ItemCategory.Rod:
                    return 90;

                case ItemCategory.Ring:
                    return 45;

                case ItemCategory.Amulet:
                    return 45;
            }
            return 0;
        }

        private string DescribeActivationEffect()
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            GetMergedFlags(f1, f2, f3);
            if (f3.IsClear(ItemFlag3.Activate))
            {
                return null;
            }
            if (ArtifactIndex == 0 && RareItemTypeIndex == 0 && BonusPowerType == 0 && BonusPowerSubType != 0)
            {
                switch (BonusPowerSubType)
                {
                    case LegendaryPower.ActSunlight:
                        {
                            return "beam of sunlight every 10 turns";
                        }
                    case LegendaryPower.ActBoMiss1:
                        {
                            return "magic missile (2d6) every 2 turns";
                        }
                    case LegendaryPower.ActBaPois1:
                        {
                            return "stinking cloud (12), rad. 3, every 4+d4 turns";
                        }
                    case LegendaryPower.ActBoElec1:
                        {
                            return "lightning bolt (4d8) every 6+d6 turns";
                        }
                    case LegendaryPower.ActBoAcid1:
                        {
                            return "acid bolt (5d8) every 5+d5 turns";
                        }
                    case LegendaryPower.ActBoCold1:
                        {
                            return "frost bolt (6d8) every 7+d7 turns";
                        }
                    case LegendaryPower.ActBoFire1:
                        {
                            return "fire bolt (9d8) every 8+d8 turns";
                        }
                    case LegendaryPower.ActBaCold1:
                        {
                            return "ball of cold (48) every 400 turns";
                        }
                    case LegendaryPower.ActBaFire1:
                        {
                            return "ball of fire (72) every 400 turns";
                        }
                    case LegendaryPower.ActDrain1:
                        {
                            return "drain life (100) every 100+d100 turns";
                        }
                    case LegendaryPower.ActBaCold2:
                        {
                            return "ball of cold (100) every 300 turns";
                        }
                    case LegendaryPower.ActBaElec2:
                        {
                            return "ball of lightning (100) every 500 turns";
                        }
                    case LegendaryPower.ActDrain2:
                        {
                            return "drain life (120) every 400 turns";
                        }
                    case LegendaryPower.ActVampire1:
                        {
                            return "vampiric drain (3*50) every 400 turns";
                        }
                    case LegendaryPower.ActBoMiss2:
                        {
                            return "arrows (150) every 90+d90 turns";
                        }
                    case LegendaryPower.ActBaFire2:
                        {
                            return "fire ball (120) every 225+d225 turns";
                        }
                    case LegendaryPower.ActBaCold3:
                        {
                            return "ball of cold (200) every 325+d325 turns";
                        }
                    case LegendaryPower.ActWhirlwind:
                        {
                            return "whirlwind attack every 250 turns";
                        }
                    case LegendaryPower.ActVampire2:
                        {
                            return "vampiric drain (3*100) every 400 turns";
                        }
                    case LegendaryPower.ActCallChaos:
                        {
                            return "call chaos every 350 turns";
                        }
                    case LegendaryPower.ActShard:
                        {
                            return "shard ball (120+level) every 400 turns";
                        }
                    case LegendaryPower.ActDispEvil:
                        {
                            return "dispel evil (level*5) every 300+d300 turns";
                        }
                    case LegendaryPower.ActDispGood:
                        {
                            return "dispel good (level*5) every 300+d300 turns";
                        }
                    case LegendaryPower.ActBaMiss3:
                        {
                            return "elemental breath (300) every 500 turns";
                        }
                    case LegendaryPower.ActConfuse:
                        {
                            return "confuse monster every 15 turns";
                        }
                    case LegendaryPower.ActSleep:
                        {
                            return "sleep nearby monsters every 55 turns";
                        }
                    case LegendaryPower.ActQuake:
                        {
                            return "earthquake (rad 10) every 50 turns";
                        }
                    case LegendaryPower.ActTerror:
                        {
                            return "terror every 3 * (level+10) turns";
                        }
                    case LegendaryPower.ActTeleAway:
                        {
                            return "teleport away every 200 turns";
                        }
                    case LegendaryPower.ActBanishEvil:
                        {
                            return "banish evil every 250+d250 turns";
                        }
                    case LegendaryPower.ActCarnage:
                        {
                            return "carnage every 500 turns";
                        }
                    case LegendaryPower.ActMassGeno:
                        {
                            return "mass carnage every 1000 turns";
                        }
                    case LegendaryPower.ActCharmAnimal:
                        {
                            return "charm animal every 300 turns";
                        }
                    case LegendaryPower.ActCharmUndead:
                        {
                            return "enslave undead every 333 turns";
                        }
                    case LegendaryPower.ActCharmOther:
                        {
                            return "charm monster every 400 turns";
                        }
                    case LegendaryPower.ActCharmAnimals:
                        {
                            return "animal friendship every 500 turns";
                        }
                    case LegendaryPower.ActCharmOthers:
                        {
                            return "mass charm every 750 turns";
                        }
                    case LegendaryPower.ActSummonAnimal:
                        {
                            return "summon animal every 200+d300 turns";
                        }
                    case LegendaryPower.ActSummonPhantom:
                        {
                            return "summon phantasmal servant every 200+d200 turns";
                        }
                    case LegendaryPower.ActSummonElemental:
                        {
                            return "summon elemental every 750 turns";
                        }
                    case LegendaryPower.ActSummonDemon:
                        {
                            return "summon demon every 666+d333 turns";
                        }
                    case LegendaryPower.ActSummonUndead:
                        {
                            return "summon undead every 666+d333 turns";
                        }
                    case LegendaryPower.ActCureLw:
                        {
                            return "remove fear & heal 30 hp every 10 turns";
                        }
                    case LegendaryPower.ActCureMw:
                        {
                            return "heal 4d8 & wounds every 3+d3 turns";
                        }
                    case LegendaryPower.ActCurePoison:
                        {
                            return "remove fear and cure poison every 5 turns";
                        }
                    case LegendaryPower.ActRestLife:
                        {
                            return "restore life levels every 450 turns";
                        }
                    case LegendaryPower.ActRestAll:
                        {
                            return "restore stats and life levels every 750 turns";
                        }
                    case LegendaryPower.ActCure700:
                        {
                            return "heal 700 hit points every 250 turns";
                        }
                    case LegendaryPower.ActCure1000:
                        {
                            return "heal 1000 hit points every 888 turns";
                        }
                    case LegendaryPower.ActEsp:
                        {
                            return "temporary ESP (dur 25+d30) every 200 turns";
                        }
                    case LegendaryPower.ActBerserk:
                        {
                            return "heroism and berserk (dur 50+d50) every 100+d100 turns";
                        }
                    case LegendaryPower.ActProtEvil:
                        {
                            return "protect evil (dur level*3 + d25) every 225+d225 turns";
                        }
                    case LegendaryPower.ActResistAll:
                        {
                            return "resist elements (dur 40+d40) every 200 turns";
                        }
                    case LegendaryPower.ActSpeed:
                        {
                            return "speed (dur 20+d20) every 250 turns";
                        }
                    case LegendaryPower.ActXtraSpeed:
                        {
                            return "speed (dur 75+d75) every 200+d200 turns";
                        }
                    case LegendaryPower.ActWraith:
                        {
                            return "wraith form (level/2 + d(level/2)) every 1000 turns";
                        }
                    case LegendaryPower.ActInvuln:
                        {
                            return "invulnerability (dur 8+d8) every 1000 turns";
                        }
                    case LegendaryPower.ActLight:
                        {
                            return "light area (dam 2d15) every 10+d10 turns";
                        }
                    case LegendaryPower.ActMapLight:
                        {
                            return "light (dam 2d15) & map area every 50+d50 turns";
                        }
                    case LegendaryPower.ActDetectAll:
                        {
                            return "detection every 55+d55 turns";
                        }
                    case LegendaryPower.ActDetectXtra:
                        {
                            return "detection, probing and identify true every 1000 turns";
                        }
                    case LegendaryPower.ActIdFull:
                        {
                            return "identify true every 750 turns";
                        }
                    case LegendaryPower.ActIdPlain:
                        {
                            return "identify spell every 10 turns";
                        }
                    case LegendaryPower.ActRuneExplo:
                        {
                            return "Yellow Sign every 200 turns";
                        }
                    case LegendaryPower.ActRuneProt:
                        {
                            return "rune of protection every 400 turns";
                        }
                    case LegendaryPower.ActSatiate:
                        {
                            return "satisfy hunger every 200 turns";
                        }
                    case LegendaryPower.ActDestDoor:
                        {
                            return "destroy doors every 10 turns";
                        }
                    case LegendaryPower.ActStoneMud:
                        {
                            return "stone to mud every 5 turns";
                        }
                    case LegendaryPower.ActRecharge:
                        {
                            return "recharging every 70 turns";
                        }
                    case LegendaryPower.ActAlchemy:
                        {
                            return "alchemy every 500 turns";
                        }
                    case LegendaryPower.ActDimDoor:
                        {
                            return "dimension door every 100 turns";
                        }
                    case LegendaryPower.ActTeleport:
                        {
                            return "teleport (range 100) every 45 turns";
                        }
                    case LegendaryPower.ActRecall:
                        {
                            return "word of recall every 200 turns";
                        }
                    default:
                        {
                            return "something undefined";
                        }
                }
            }
            switch (ArtifactIndex)
            {
                case ArtifactId.DaggerOfFaith:
                    {
                        return "fire bolt (9d8) every 8+d8 turns";
                    }
                case ArtifactId.DaggerOfHope:
                    {
                        return "frost bolt (6d8) every 7+d7 turns";
                    }
                case ArtifactId.DaggerOfCharity:
                    {
                        return "lightning bolt (4d8) every 6+d6 turns";
                    }
                case ArtifactId.DaggerOfThoth:
                    {
                        return "stinking cloud (12) every 4+d4 turns";
                    }
                case ArtifactId.DaggerIcicle:
                    {
                        return "frost ball (48) every 5+d5 turns";
                    }
                case ArtifactId.BootsOfDancing:
                    {
                        return "remove fear and cure poison every 5 turns";
                    }
                case ArtifactId.SwordExcalibur:
                    {
                        return "frost ball (100) every 300 turns";
                    }
                case ArtifactId.SwordOfTheDawn:
                    {
                        return "summon a Black Reaver every 500+d500 turns";
                    }
                case ArtifactId.SwordOfEverflame:
                    {
                        return "fire ball (72) every 400 turns";
                    }
                case ArtifactId.MorningStarFirestarter:
                    {
                        return "large fire ball (72) every 100 turns";
                    }
                case ArtifactId.BootsOfIthaqua:
                    {
                        return "haste self (20+d20 turns) every 200 turns";
                    }
                case ArtifactId.AxeOfTheoden:
                    {
                        return "drain life (120) every 400 turns";
                    }
                case ArtifactId.HammerJustice:
                    {
                        return "drain life (90) every 70 turns";
                    }
                case ArtifactId.ArmourOfTheOgreLords:
                    {
                        return "door and trap destruction every 10 turns";
                    }
                case ArtifactId.ScytheOfGharne:
                    {
                        return "word of recall every 200 turns";
                    }
                case ArtifactId.MaceThunder:
                    {
                        return "haste self (20+d20 turns) every 100+d100 turns";
                    }
                case ArtifactId.QuarterstaffEriril:
                    {
                        return "identify every 10 turns";
                    }
                case ArtifactId.QuarterstaffOfAtal:
                    {
                        return "probing, detection and full id  every 1000 turns";
                    }
                case ArtifactId.AxeOfTheTrolls:
                    {
                        return "mass carnage every 1000 turns";
                    }
                case ArtifactId.AxeSpleenSlicer:
                    {
                        return "cure wounds (4d7) every 3+d3 turns";
                    }
                case ArtifactId.CrossbowOfDeath:
                    {
                        return "fire branding of bolts every 999 turns";
                    }
                case ArtifactId.SwordOfKarakal:
                    {
                        return "a getaway every 35 turns";
                    }
                case ArtifactId.SpearGungnir:
                    {
                        return "lightning ball (100) every 500 turns";
                    }
                case ArtifactId.SpearOfDestiny:
                    {
                        return "stone to mud every 5 turns";
                    }
                case ArtifactId.PlateMailSoulkeeper:
                    {
                        return "heal (1000) every 888 turns";
                    }
                case ArtifactId.ArmourOfTheVampireHunter:
                    {
                        return "heal (777), curing and heroism every 300 turns";
                    }
                case ArtifactId.ArmourOfTheOrcs:
                    {
                        return "carnage every 500 turns";
                    }
                case ArtifactId.ShadowCloakOfNyogtha:
                    {
                        return "restore life levels every 450 turns";
                    }
                case ArtifactId.TridentOfTheGnorri:
                    {
                        return "teleport away every 150 turns";
                    }
                case ArtifactId.CloakOfBarzai:
                    {
                        return "resistance (20+d20 turns) every 111 turns";
                    }
                case ArtifactId.CloakDarkness:
                    {
                        return "Sleep II every 55 turns";
                    }
                case ArtifactId.CloakOfTheSwashbuckler:
                    {
                        return "recharge item I every 70 turns";
                    }
                case ArtifactId.CloakShifter:
                    {
                        return "teleport every 45 turns";
                    }
                case ArtifactId.FlailTotila:
                    {
                        return "confuse monster every 15 turns";
                    }
                case ArtifactId.GlovesOfLight:
                    {
                        return "magic missile (2d6) every 2 turns";
                    }
                case ArtifactId.GauntletIronfist:
                    {
                        return "fire bolt (9d8) every 8+d8 turns";
                    }
                case ArtifactId.GauntletsOfGhouls:
                    {
                        return "frost bolt (6d8) every 7+d7 turns";
                    }
                case ArtifactId.GauntletsWhiteSpark:
                    {
                        return "lightning bolt (4d8) every 6+d6 turns";
                    }
                case ArtifactId.GauntletsOfTheDead:
                    {
                        return "acid bolt (5d8) every 5+d5 turns";
                    }
                case ArtifactId.CestiOfCombat:
                    {
                        return "a magical arrow (150) every 90+d90 turns";
                    }
                case ArtifactId.HelmSkullkeeper:
                    {
                        return "detection every 55+d55 turns";
                    }
                case ArtifactId.CrownOfTheSun:
                    {
                        return "heal (700) every 250 turns";
                    }
                case ArtifactId.DragonScaleRazorback:
                    {
                        return "star ball (150) every 1000 turns";
                    }
                case ArtifactId.DragonScaleBladeturner:
                    {
                        return "breathe elements (300), berserk rage, bless, and resistance";
                    }
                case ArtifactId.StarEssenceOfPolaris:
                    {
                        return "illumination every 10+d10 turns";
                    }
                case ArtifactId.StarEssenceOfXoth:
                    {
                        return "magic mapping and light every 50+d50 turns";
                    }
                case ArtifactId.ShiningTrapezohedron:
                    {
                        return "clairvoyance and recall, draining you";
                    }
                case ArtifactId.AmuletOfAbdulAlhazred:
                    {
                        return "dispel evil (x5) every 300+d300 turns";
                    }
                case ArtifactId.AmuletOfLobon:
                    {
                        return "protection from evil every 225+d225 turns";
                    }
                case ArtifactId.RingOfMagic:
                    {
                        return "a strangling attack (100) every 100+d100 turns";
                    }
                case ArtifactId.RingOfBast:
                    {
                        return "haste self (75+d75 turns) every 150+d150 turns";
                    }
                case ArtifactId.RingOfElementalPowerFire:
                    {
                        return "large fire ball (120) every 225+d225 turns";
                    }
                case ArtifactId.RingOfElementalPowerIce:
                    {
                        return "large frost ball (200) every 325+d325 turns";
                    }
                case ArtifactId.RingOfElementalPowerStorm:
                    {
                        return "large lightning ball (250) every 425+d425 turns";
                    }
                case ArtifactId.RingOfSet:
                    {
                        return "bizarre things every 450+d450 turns";
                    }
                case ArtifactId.DragonHelmOfPower:
                case ArtifactId.HelmTerrorMask:
                    {
                        return "rays of fear in every direction";
                    }
            }
            if (RareItemTypeIndex == Enumerations.RareItemType.WeaponPlanarWeapon)
            {
                return "teleport every 50+d50 turns";
            }
            if (Category == ItemCategory.Ring)
            {
                switch (ItemSubCategory)
                {
                    case Enumerations.RingType.Flames:
                        return "ball of fire and resist fire";

                    case Enumerations.RingType.Ice:
                        return "ball of cold and resist cold";

                    case Enumerations.RingType.Acid:
                        return "ball of acid and resist acid";

                    default:
                        return null;
                }
            }
            if (Category != ItemCategory.DragArmor)
            {
                return null;
            }
            switch (ItemSubCategory)
            {
                case Enumerations.DragonArmour.Blue:
                    {
                        return "breathe lightning (100) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.White:
                    {
                        return "breathe frost (110) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.Black:
                    {
                        return "breathe acid (130) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.Green:
                    {
                        return "breathe poison gas (150) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.Red:
                    {
                        return "breathe fire (200) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.MultiHued:
                    {
                        return "breathe multi-hued (250) every 225+d225 turns";
                    }
                case Enumerations.DragonArmour.Bronze:
                    {
                        return "breathe confusion (120) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.Gold:
                    {
                        return "breathe sound (130) every 450+d450 turns";
                    }
                case Enumerations.DragonArmour.Chaos:
                    {
                        return "breathe chaos/disenchant (220) every 300+d300 turns";
                    }
                case Enumerations.DragonArmour.Law:
                    {
                        return "breathe sound/shards (230) every 300+d300 turns";
                    }
                case Enumerations.DragonArmour.Balance:
                    {
                        return "You breathe balance (250) every 300+d300 turns";
                    }
                case Enumerations.DragonArmour.Pseudo:
                    {
                        return "breathe light/darkness (200) every 300+d300 turns";
                    }
                case Enumerations.DragonArmour.Power:
                    {
                        return "breathe the elements (300) every 300+d300 turns";
                    }
            }
            return string.Empty;
        }

        private bool IsTried()
        {
            return ItemType.Tried;
        }
    }
}