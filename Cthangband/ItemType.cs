using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    [Serializable]
    internal class ItemType
    {
        public readonly int[] Chance = new int[4];
        public readonly FlagSet Flags1 = new FlagSet();
        public readonly FlagSet Flags2 = new FlagSet();
        public readonly FlagSet Flags3 = new FlagSet();
        public readonly int[] Locale = new int[4];
        public readonly bool[] Stompable = new bool[4];
        public int Ac;
        public ItemCategory Category;
        public char Character;
        public Colour Colour;
        public int Cost;
        public int Dd;
        public int Ds;
        public bool EasyKnow;
        public bool FlavourAware;
        public bool HasFlavor;
        public int Level;
        public string Name;
        public int Pval;
        public int SubCategory;
        public int ToA;
        public int ToD;
        public int ToH;
        public bool Tried;
        public int Weight;

        public ItemType()
        {
        }

        public ItemType(ItemType original)
        {
            Character = original.Character;
            Colour = original.Colour;
            Ac = original.Ac;
            FlavourAware = original.FlavourAware;
            Chance = new int[4];
            for (int i = 0; i < 4; i++)
            {
                Chance[i] = original.Chance[i];
            }
            Cost = original.Cost;
            Dd = original.Dd;
            Ds = original.Ds;
            EasyKnow = original.EasyKnow;
            Flags1 = new FlagSet(original.Flags1);
            Flags2 = new FlagSet(original.Flags2);
            Flags3 = new FlagSet(original.Flags3);
            HasFlavor = original.HasFlavor;
            Level = original.Level;
            Locale = new int[4];
            for (int i = 0; i < 4; i++)
            {
                Locale[i] = original.Locale[i];
            }
            Name = original.Name;
            Pval = original.Pval;
            SubCategory = original.SubCategory;
            ToA = original.ToA;
            ToD = original.ToD;
            ToH = original.ToH;
            Tried = original.Tried;
            Category = original.Category;
            Weight = original.Weight;
        }

        public ItemType(BaseItemType baseItem)
        {
            Character = baseItem.Character;
            Colour = baseItem.Colour;
            Name = baseItem.FriendlyName;
            Ac = baseItem.Ac;
            Category = baseItem.Category;
            Chance[0] = baseItem.Chance1;
            Chance[1] = baseItem.Chance2;
            Chance[2] = baseItem.Chance3;
            Chance[3] = baseItem.Chance4;
            Cost = baseItem.Cost;
            Dd = baseItem.Dd;
            Ds = baseItem.Ds;
            Level = baseItem.Level;
            Locale[0] = baseItem.Locale1;
            Locale[1] = baseItem.Locale2;
            Locale[2] = baseItem.Locale3;
            Locale[3] = baseItem.Locale4;
            Pval = baseItem.Pval;
            SubCategory = baseItem.SubCategory;
            ToA = baseItem.ToA;
            ToD = baseItem.ToD;
            ToH = baseItem.ToH;
            Tried = baseItem.Tried;
            Weight = baseItem.Weight;
            Flags1.Set(baseItem.Blows ? ItemFlag1.Blows : 0);
            Flags1.Set(baseItem.BrandAcid ? ItemFlag1.BrandAcid : 0);
            Flags1.Set(baseItem.BrandCold ? ItemFlag1.BrandCold : 0);
            Flags1.Set(baseItem.BrandElec ? ItemFlag1.BrandElec : 0);
            Flags1.Set(baseItem.BrandFire ? ItemFlag1.BrandFire : 0);
            Flags1.Set(baseItem.BrandPois ? ItemFlag1.BrandPois : 0);
            Flags1.Set(baseItem.Cha ? ItemFlag1.Cha : 0);
            Flags1.Set(baseItem.Chaotic ? ItemFlag1.Chaotic : 0);
            Flags1.Set(baseItem.Con ? ItemFlag1.Con : 0);
            Flags1.Set(baseItem.Dex ? ItemFlag1.Dex : 0);
            Flags1.Set(baseItem.Impact ? ItemFlag1.Impact : 0);
            Flags1.Set(baseItem.Infra ? ItemFlag1.Infra : 0);
            Flags1.Set(baseItem.Int ? ItemFlag1.Int : 0);
            Flags1.Set(baseItem.KillDragon ? ItemFlag1.KillDragon : 0);
            Flags1.Set(baseItem.Search ? ItemFlag1.Search : 0);
            Flags1.Set(baseItem.SlayAnimal ? ItemFlag1.SlayAnimal : 0);
            Flags1.Set(baseItem.SlayDemon ? ItemFlag1.SlayDemon : 0);
            Flags1.Set(baseItem.SlayDragon ? ItemFlag1.SlayDragon : 0);
            Flags1.Set(baseItem.SlayEvil ? ItemFlag1.SlayEvil : 0);
            Flags1.Set(baseItem.SlayGiant ? ItemFlag1.SlayGiant : 0);
            Flags1.Set(baseItem.SlayOrc ? ItemFlag1.SlayOrc : 0);
            Flags1.Set(baseItem.SlayTroll ? ItemFlag1.SlayTroll : 0);
            Flags1.Set(baseItem.SlayUndead ? ItemFlag1.SlayUndead : 0);
            Flags1.Set(baseItem.Speed ? ItemFlag1.Speed : 0);
            Flags1.Set(baseItem.Stealth ? ItemFlag1.Stealth : 0);
            Flags1.Set(baseItem.Str ? ItemFlag1.Str : 0);
            Flags1.Set(baseItem.Tunnel ? ItemFlag1.Tunnel : 0);
            Flags1.Set(baseItem.Vampiric ? ItemFlag1.Vampiric : 0);
            Flags1.Set(baseItem.Vorpal ? ItemFlag1.Vorpal : 0);
            Flags1.Set(baseItem.Wis ? ItemFlag1.Wis : 0);
            Flags2.Set(baseItem.FreeAct ? ItemFlag2.FreeAct : 0);
            Flags2.Set(baseItem.HoldLife ? ItemFlag2.HoldLife : 0);
            Flags2.Set(baseItem.ImAcid ? ItemFlag2.ImAcid : 0);
            Flags2.Set(baseItem.ImCold ? ItemFlag2.ImCold : 0);
            Flags2.Set(baseItem.ImElec ? ItemFlag2.ImElec : 0);
            Flags2.Set(baseItem.ImFire ? ItemFlag2.ImFire : 0);
            Flags2.Set(baseItem.Reflect ? ItemFlag2.Reflect : 0);
            Flags2.Set(baseItem.ResAcid ? ItemFlag2.ResAcid : 0);
            Flags2.Set(baseItem.ResBlind ? ItemFlag2.ResBlind : 0);
            Flags2.Set(baseItem.ResChaos ? ItemFlag2.ResChaos : 0);
            Flags2.Set(baseItem.ResCold ? ItemFlag2.ResCold : 0);
            Flags2.Set(baseItem.ResConf ? ItemFlag2.ResConf : 0);
            Flags2.Set(baseItem.ResDark ? ItemFlag2.ResDark : 0);
            Flags2.Set(baseItem.ResDisen ? ItemFlag2.ResDisen : 0);
            Flags2.Set(baseItem.ResElec ? ItemFlag2.ResElec : 0);
            Flags2.Set(baseItem.ResFear ? ItemFlag2.ResFear : 0);
            Flags2.Set(baseItem.ResFire ? ItemFlag2.ResFire : 0);
            Flags2.Set(baseItem.ResLight ? ItemFlag2.ResLight : 0);
            Flags2.Set(baseItem.ResNether ? ItemFlag2.ResNether : 0);
            Flags2.Set(baseItem.ResNexus ? ItemFlag2.ResNexus : 0);
            Flags2.Set(baseItem.ResPois ? ItemFlag2.ResPois : 0);
            Flags2.Set(baseItem.ResShards ? ItemFlag2.ResShards : 0);
            Flags2.Set(baseItem.ResSound ? ItemFlag2.ResSound : 0);
            Flags2.Set(baseItem.SustCha ? ItemFlag2.SustCha : 0);
            Flags2.Set(baseItem.SustCon ? ItemFlag2.SustCon : 0);
            Flags2.Set(baseItem.SustDex ? ItemFlag2.SustDex : 0);
            Flags2.Set(baseItem.SustInt ? ItemFlag2.SustInt : 0);
            Flags2.Set(baseItem.SustStr ? ItemFlag2.SustStr : 0);
            Flags2.Set(baseItem.SustWis ? ItemFlag2.SustWis : 0);
            Flags3.Set(baseItem.AntiTheft ? ItemFlag3.AntiTheft : 0);
            Flags3.Set(baseItem.Activate ? ItemFlag3.Activate : 0);
            Flags3.Set(baseItem.Aggravate ? ItemFlag3.Aggravate : 0);
            Flags3.Set(baseItem.Blessed ? ItemFlag3.Blessed : 0);
            Flags3.Set(baseItem.Cursed ? ItemFlag3.Cursed : 0);
            Flags3.Set(baseItem.DrainExp ? ItemFlag3.DrainExp : 0);
            Flags3.Set(baseItem.DreadCurse ? ItemFlag3.DreadCurse : 0);
            Flags3.Set(baseItem.EasyKnow ? ItemFlag3.EasyKnow : 0);
            Flags3.Set(baseItem.Feather ? ItemFlag3.Feather : 0);
            Flags3.Set(baseItem.HeavyCurse ? ItemFlag3.HeavyCurse : 0);
            Flags3.Set(baseItem.HideType ? ItemFlag3.HideType : 0);
            Flags3.Set(baseItem.IgnoreAcid ? ItemFlag3.IgnoreAcid : 0);
            Flags3.Set(baseItem.IgnoreCold ? ItemFlag3.IgnoreCold : 0);
            Flags3.Set(baseItem.IgnoreElec ? ItemFlag3.IgnoreElec : 0);
            Flags3.Set(baseItem.IgnoreFire ? ItemFlag3.IgnoreFire : 0);
            Flags3.Set(baseItem.InstaArt ? ItemFlag3.InstaArt : 0);
            Flags3.Set(baseItem.Lightsource ? ItemFlag3.Lightsource : 0);
            Flags3.Set(baseItem.NoMagic ? ItemFlag3.NoMagic : 0);
            Flags3.Set(baseItem.NoTele ? ItemFlag3.NoTele : 0);
            Flags3.Set(baseItem.PermaCurse ? ItemFlag3.PermaCurse : 0);
            Flags3.Set(baseItem.Regen ? ItemFlag3.Regen : 0);
            Flags3.Set(baseItem.SeeInvis ? ItemFlag3.SeeInvis : 0);
            Flags3.Set(baseItem.ShElec ? ItemFlag3.ShElec : 0);
            Flags3.Set(baseItem.ShFire ? ItemFlag3.ShFire : 0);
            Flags3.Set(baseItem.ShowMods ? ItemFlag3.ShowMods : 0);
            Flags3.Set(baseItem.SlowDigest ? ItemFlag3.SlowDigest : 0);
            Flags3.Set(baseItem.Telepathy ? ItemFlag3.Telepathy : 0);
            Flags3.Set(baseItem.Teleport ? ItemFlag3.Teleport : 0);
            Flags3.Set(baseItem.Wraith ? ItemFlag3.Wraith : 0);
            Flags3.Set(baseItem.XtraMight ? ItemFlag3.XtraMight : 0);
            Flags3.Set(baseItem.XtraShots ? ItemFlag3.XtraShots : 0);
        }

        public static bool KindIsGood(int kIdx)
        {
            ItemType kPtr = Profile.Instance.ItemTypes[kIdx];
            switch (kPtr.Category)
            {
                case ItemCategory.HardArmor:
                case ItemCategory.SoftArmor:
                case ItemCategory.DragArmor:
                case ItemCategory.Shield:
                case ItemCategory.Cloak:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                case ItemCategory.Helm:
                case ItemCategory.Crown:
                    return kPtr.ToA >= 0;

                case ItemCategory.Bow:
                case ItemCategory.Sword:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Digging:
                    if (kPtr.ToH < 0)
                    {
                        return false;
                    }
                    if (kPtr.ToD < 0)
                    {
                        return false;
                    }
                    return true;

                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                    return true;

                case ItemCategory.LifeBook:
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.TarotBook:
                case ItemCategory.CorporealBook:
                    return kPtr.SubCategory >= ItemSubCategory.SvBookMinGood;

                case ItemCategory.Ring:
                    return kPtr.SubCategory == RingType.Speed;

                case ItemCategory.Amulet:
                    if (kPtr.SubCategory == AmuletType.TheMagi)
                    {
                        return true;
                    }
                    if (kPtr.SubCategory == AmuletType.Resistance)
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }

        public static ItemType RandomItemType(int level)
        {
            int i;
            int j;
            AllocationEntry[] table = SaveGame.Instance.AllocKindTable;
            if (level > 0)
            {
                if (Program.Rng.RandomLessThan(Constants.GreatObj) == 0)
                {
                    level = 1 + (level * Constants.MaxDepth / Program.Rng.DieRoll(Constants.MaxDepth));
                }
            }
            int total = 0;
            for (i = 0; i < SaveGame.Instance.AllocKindSize; i++)
            {
                if (table[i].Level > level)
                {
                    break;
                }
                table[i].FinalProbability = 0;
                int kIdx = table[i].Index;
                ItemType kPtr = Profile.Instance.ItemTypes[kIdx];
                if (SaveGame.Instance.Level?.OpeningChest == true &&
                    kPtr.Category == ItemCategory.Chest)
                {
                    continue;
                }
                table[i].FinalProbability = table[i].FilteredProbabiity;
                total += table[i].FinalProbability;
            }
            if (total <= 0)
            {
                return null;
            }
            long value = Program.Rng.RandomLessThan(total);
            for (i = 0; i < SaveGame.Instance.AllocKindSize; i++)
            {
                if (value < table[i].FinalProbability)
                {
                    break;
                }
                value -= table[i].FinalProbability;
            }
            int p = Program.Rng.RandomLessThan(100);
            if (p < 60)
            {
                j = i;
                value = Program.Rng.RandomLessThan(total);
                for (i = 0; i < SaveGame.Instance.AllocKindSize; i++)
                {
                    if (value < table[i].FinalProbability)
                    {
                        break;
                    }
                    value -= table[i].FinalProbability;
                }
                if (table[i].Level < table[j].Level)
                {
                    i = j;
                }
            }
            if (p < 10)
            {
                j = i;
                value = Program.Rng.RandomLessThan(total);
                for (i = 0; i < SaveGame.Instance.AllocKindSize; i++)
                {
                    if (value < table[i].FinalProbability)
                    {
                        break;
                    }
                    value -= table[i].FinalProbability;
                }
                if (table[i].Level < table[j].Level)
                {
                    i = j;
                }
            }
            return Profile.Instance.ItemTypes[table[i].Index];
        }

        public bool HasQuality()
        {
            switch (Category)
            {
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                case ItemCategory.Boots:
                case ItemCategory.Bow:
                case ItemCategory.Cloak:
                case ItemCategory.Crown:
                case ItemCategory.Digging:
                case ItemCategory.DragArmor:
                case ItemCategory.Gloves:
                case ItemCategory.Hafted:
                case ItemCategory.HardArmor:
                case ItemCategory.Helm:
                case ItemCategory.Polearm:
                case ItemCategory.Shield:
                case ItemCategory.Shot:
                case ItemCategory.SoftArmor:
                case ItemCategory.Sword:
                    return true;

                case ItemCategory.Light:
                    return SubCategory == LightType.Orb;

                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}