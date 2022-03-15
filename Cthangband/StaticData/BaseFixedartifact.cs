// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Debug;
using Cthangband.Enumerations;

namespace Cthangband.StaticData
{
    internal class BaseFixedartifact : EntityType
    {
        public int Ac
        {
            get;
            set;
        }

        public bool Activate
        {
            get;
            set;
        }

        public bool Aggravate
        {
            get;
            set;
        }

        public bool Blessed
        {
            get;
            set;
        }

        public bool Blows
        {
            get;
            set;
        }

        public bool BrandAcid
        {
            get;
            set;
        }

        public bool BrandCold
        {
            get;
            set;
        }

        public bool BrandElec
        {
            get;
            set;
        }

        public bool BrandFire
        {
            get;
            set;
        }

        public bool BrandPois
        {
            get;
            set;
        }

        public bool Cha
        {
            get;
            set;
        }

        public bool Chaotic
        {
            get;
            set;
        }

        public bool Con
        {
            get;
            set;
        }

        public int Cost
        {
            get;
            set;
        }

        public bool Cursed
        {
            get;
            set;
        }

        public int Dd
        {
            get;
            set;
        }

        public bool Dex
        {
            get;
            set;
        }

        public bool DrainExp
        {
            get;
            set;
        }

        public bool DreadCurse
        {
            get;
            set;
        }

        public int Ds
        {
            get;
            set;
        }

        public bool EasyKnow
        {
            get;
            set;
        }

        public bool Feather
        {
            get;
            set;
        }

        public FixedArtifactId FixedArtifactID
        {
            get;
            set;
        }

        public bool FreeAct
        {
            get;
            set;
        }

        public string FriendlyName
        {
            get;
            set;
        }

        public bool HasOwnType
        {
            get;
            set;
        }

        public bool HeavyCurse
        {
            get;
            set;
        }

        public bool HideType
        {
            get;
            set;
        }

        public bool HoldLife
        {
            get;
            set;
        }

        public bool IgnoreAcid
        {
            get;
            set;
        }

        public bool IgnoreCold
        {
            get;
            set;
        }

        public bool IgnoreElec
        {
            get;
            set;
        }

        public bool IgnoreFire
        {
            get;
            set;
        }

        public bool ImAcid
        {
            get;
            set;
        }

        public bool ImCold
        {
            get;
            set;
        }

        public bool ImElec
        {
            get;
            set;
        }

        public bool ImFire
        {
            get;
            set;
        }

        public bool Impact
        {
            get;
            set;
        }

        public bool Infra
        {
            get;
            set;
        }

        public bool InstaArt
        {
            get;
            set;
        }

        public bool Int
        {
            get;
            set;
        }

        public bool KillDragon
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public bool Lightsource
        {
            get;
            set;
        }

        public bool NoMagic
        {
            get;
            set;
        }

        public bool NoTele
        {
            get;
            set;
        }

        public bool PermaCurse
        {
            get;
            set;
        }

        public int Pval
        {
            get;
            set;
        }

        public int Rarity
        {
            get;
            set;
        }

        public bool Reflect
        {
            get;
            set;
        }

        public bool Regen
        {
            get;
            set;
        }

        public bool ResAcid
        {
            get;
            set;
        }

        public bool ResBlind
        {
            get;
            set;
        }

        public bool ResChaos
        {
            get;
            set;
        }

        public bool ResCold
        {
            get;
            set;
        }

        public bool ResConf
        {
            get;
            set;
        }

        public bool ResDark
        {
            get;
            set;
        }

        public bool ResDisen
        {
            get;
            set;
        }

        public bool ResElec
        {
            get;
            set;
        }

        public bool ResFear
        {
            get;
            set;
        }

        public bool ResFire
        {
            get;
            set;
        }

        public bool ResLight
        {
            get;
            set;
        }

        public bool ResNether
        {
            get;
            set;
        }

        public bool ResNexus
        {
            get;
            set;
        }

        public bool ResPois
        {
            get;
            set;
        }

        public bool ResShards
        {
            get;
            set;
        }

        public bool ResSound
        {
            get;
            set;
        }

        public bool Search
        {
            get;
            set;
        }

        public bool SeeInvis
        {
            get;
            set;
        }

        public bool ShElec
        {
            get;
            set;
        }

        public bool ShFire
        {
            get;
            set;
        }

        public bool ShowMods
        {
            get;
            set;
        }

        public bool SlayAnimal
        {
            get;
            set;
        }

        public bool SlayDemon
        {
            get;
            set;
        }

        public bool SlayDragon
        {
            get;
            set;
        }

        public bool SlayEvil
        {
            get;
            set;
        }

        public bool SlayGiant
        {
            get;
            set;
        }

        public bool SlayOrc
        {
            get;
            set;
        }

        public bool SlayTroll
        {
            get;
            set;
        }

        public bool SlayUndead
        {
            get;
            set;
        }

        public bool SlowDigest
        {
            get;
            set;
        }

        public bool Speed
        {
            get;
            set;
        }

        public bool Stealth
        {
            get;
            set;
        }

        public bool Str
        {
            get;
            set;
        }

        public bool SustCha
        {
            get;
            set;
        }

        public bool SustCon
        {
            get;
            set;
        }

        public bool SustDex
        {
            get;
            set;
        }

        public bool SustInt
        {
            get;
            set;
        }

        public bool SustStr
        {
            get;
            set;
        }

        public bool SustWis
        {
            get;
            set;
        }

        public int Sval
        {
            get;
            set;
        }

        public bool Telepathy
        {
            get;
            set;
        }

        public bool Teleport
        {
            get;
            set;
        }

        public int ToA
        {
            get;
            set;
        }

        public int ToD
        {
            get;
            set;
        }

        public int ToH
        {
            get;
            set;
        }

        public bool Tunnel
        {
            get;
            set;
        }

        public ItemCategory Tval
        {
            get;
            set;
        }

        public bool Vampiric
        {
            get;
            set;
        }

        public bool Vorpal
        {
            get;
            set;
        }

        public int Weight
        {
            get;
            set;
        }

        public bool Wis
        {
            get;
            set;
        }

        public bool Wraith
        {
            get;
            set;
        }

        public bool XtraMight
        {
            get;
            set;
        }

        public bool XtraShots
        {
            get;
            set;
        }
    }
}