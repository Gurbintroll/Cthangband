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

namespace Cthangband
{
    [Serializable]
    internal class RareItemType
    {
        public readonly FlagSet Flags1 = new FlagSet();
        public readonly FlagSet Flags2 = new FlagSet();
        public readonly FlagSet Flags3 = new FlagSet();
        public int Cost;
        public int Level;
        public int MaxPval;
        public int MaxToA;
        public int MaxToD;
        public int MaxToH;
        public string Name;
        public int Rarity;
        public int Rating;
        public int Slot;

        public RareItemType()
        {
        }

        public RareItemType(BaseRareItemType baseItem)
        {
            Flags1 = new FlagSet();
            Flags2 = new FlagSet();
            Flags3 = new FlagSet();
            Cost = baseItem.Cost;
            Level = baseItem.Level;
            MaxPval = baseItem.MaxPval;
            MaxToA = baseItem.MaxToA;
            MaxToD = baseItem.MaxToD;
            MaxToH = baseItem.MaxToH;
            Name = baseItem.FriendlyName;
            Rarity = baseItem.Rarity;
            Rating = baseItem.Rating;
            Slot = baseItem.Slot;
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
    }
}