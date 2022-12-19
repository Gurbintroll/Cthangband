// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    internal class PlayerStatus
    {
        private static readonly int[][] _blowsTable =
        {
            new[] {1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3},
            new[] {1, 1, 1, 1, 2, 2, 3, 3, 3, 4, 4, 4},
            new[] {1, 1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 5},
            new[] {1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 5, 5},
            new[] {1, 2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5},
            new[] {2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5, 6},
            new[] {2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 5, 6},
            new[] {2, 3, 3, 4, 4, 4, 5, 5, 5, 5, 5, 6},
            new[] {3, 3, 3, 4, 4, 4, 5, 5, 5, 5, 6, 6},
            new[] {3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6},
            new[] {3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
            new[] {3, 3, 4, 4, 4, 4, 5, 5, 6, 6, 6, 6}
        };

        private readonly Level _level;
        private readonly Player _player;

        public PlayerStatus(Player player, Level level)
        {
            _player = player;
            _level = level;
        }

        public void CalcBonuses()
        {
            int i;
            int extraShots;
            Item oPtr;
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            int oldSpeed = _player.Speed;
            bool oldTelepathy = _player.HasTelepathy;
            bool oldSeeInv = _player.HasSeeInvisibility;
            int oldDisAc = _player.DisplayedBaseArmourClass;
            int oldDisToA = _player.DisplayedArmourClassBonus;
            int extraBlows = extraShots = 0;
            for (i = 0; i < 6; i++)
            {
                _player.AbilityScores[i].Bonus = 0;
            }
            _player.DisplayedBaseArmourClass = 0;
            _player.BaseArmourClass = 0;
            _player.DisplayedAttackBonus = 0;
            _player.AttackBonus = 0;
            _player.DisplayedDamageBonus = 0;
            _player.DamageBonus = 0;
            _player.DisplayedArmourClassBonus = 0;
            _player.ArmourClassBonus = 0;
            _player.HasAggravation = false;
            _player.HasRandomTeleport = false;
            _player.HasExperienceDrain = false;
            _player.HasBlessedBlade = false;
            _player.HasExtraMight = false;
            _player.HasQuakeWeapon = false;
            _player.HasSeeInvisibility = false;
            _player.HasFreeAction = false;
            _player.HasSlowDigestion = false;
            _player.HasRegeneration = false;
            _player.HasFeatherFall = false;
            _player.HasHoldLife = false;
            _player.HasTelepathy = false;
            _player.HasGlow = false;
            _player.HasSustainStrength = false;
            _player.HasSustainIntelligence = false;
            _player.HasSustainWisdom = false;
            _player.HasSustainConstitution = false;
            _player.HasSustainDexterity = false;
            _player.HasSustainCharisma = false;
            _player.HasAcidResistance = false;
            _player.HasLightningResistance = false;
            _player.HasFireResistance = false;
            _player.HasColdResistance = false;
            _player.HasPoisonResistance = false;
            _player.HasConfusionResistance = false;
            _player.HasSoundResistance = false;
            _player.HasTimeResistance = false;
            _player.HasLightResistance = false;
            _player.HasDarkResistance = false;
            _player.HasChaosResistance = false;
            _player.HasDisenchantResistance = false;
            _player.HasShardResistance = false;
            _player.HasNexusResistance = false;
            _player.HasBlindnessResistance = false;
            _player.HasNetherResistance = false;
            _player.HasFearResistance = false;
            _player.HasElementalVulnerability = false;
            _player.HasReflection = false;
            _player.HasFireShield = false;
            _player.HasLightningShield = false;
            _player.HasAntiMagic = false;
            _player.HasAntiTeleport = false;
            _player.HasAntiTheft = false;
            _player.HasAcidImmunity = false;
            _player.HasLightningImmunity = false;
            _player.HasFireImmunity = false;
            _player.HasColdImmunity = false;
            _player.InfravisionRange = _player.Race.Infravision;
            _player.SkillDisarmTraps = _player.Race.BaseDisarmBonus + _player.Profession.BaseDisarmBonus;
            _player.SkillUseDevice = _player.Race.BaseDeviceBonus + _player.Profession.BaseDeviceBonus;
            _player.SkillSavingThrow = _player.Race.BaseSaveBonus + _player.Profession.BaseSaveBonus;
            _player.SkillStealth = _player.Race.BaseStealthBonus + _player.Profession.BaseStealthBonus;
            _player.SkillSearching = _player.Race.BaseSearchBonus + _player.Profession.BaseSearchBonus;
            _player.SkillSearchFrequency = _player.Race.BaseSearchFrequency + _player.Profession.BaseSearchFrequency;
            _player.SkillMelee = _player.Race.BaseMeleeAttackBonus + _player.Profession.BaseMeleeAttackBonus;
            _player.SkillRanged = _player.Race.BaseRangedAttackBonus + _player.Profession.BaseRangedAttackBonus;
            _player.SkillThrowing = _player.Race.BaseRangedAttackBonus + _player.Profession.BaseRangedAttackBonus;
            _player.SkillDigging = 0;
            if ((_player.ProfessionIndex == CharacterClass.Warrior && _player.Level > 29) ||
                (_player.ProfessionIndex == CharacterClass.Paladin && _player.Level > 39) ||
                (_player.ProfessionIndex == CharacterClass.Fanatic && _player.Level > 39))
            {
                _player.HasFearResistance = true;
            }
            if (_player.ProfessionIndex == CharacterClass.Fanatic && _player.Level > 29)
            {
                _player.HasChaosResistance = true;
            }
            if (_player.ProfessionIndex == CharacterClass.Cultist && _player.Level > 19)
            {
                _player.HasChaosResistance = true;
            }
            if (_player.ProfessionIndex == CharacterClass.Mindcrafter)
            {
                if (_player.Level > 9)
                {
                    _player.HasFearResistance = true;
                }
                if (_player.Level > 19)
                {
                    _player.HasSustainWisdom = true;
                }
                if (_player.Level > 29)
                {
                    _player.HasConfusionResistance = true;
                }
                if (_player.Level > 39)
                {
                    _player.HasTelepathy = true;
                }
            }
            if (_player.ProfessionIndex == CharacterClass.Monk && _player.Level > 24 && !MartialArtistHeavyArmour())
            {
                _player.HasFreeAction = true;
            }
            if (_player.ProfessionIndex == CharacterClass.Mystic)
            {
                if (_player.Level > 9)
                {
                    _player.HasConfusionResistance = true;
                }
                if (_player.Level > 24)
                {
                    _player.HasFearResistance = true;
                }
                if (_player.Level > 29 && !MartialArtistHeavyArmour())
                {
                    _player.HasFreeAction = true;
                }
                if (_player.Level > 39)
                {
                    _player.HasTelepathy = true;
                }
            }
            if (_player.ProfessionIndex == CharacterClass.ChosenOne)
            {
                _player.HasGlow = true;
                if (_player.Level >= 2)
                {
                    _player.HasConfusionResistance = true;
                }
                if (_player.Level >= 4)
                {
                    _player.HasFearResistance = true;
                }
                if (_player.Level >= 6)
                {
                    _player.HasBlindnessResistance = true;
                }
                if (_player.Level >= 8)
                {
                    _player.HasFeatherFall = true;
                }
                if (_player.Level >= 10)
                {
                    _player.HasSeeInvisibility = true;
                }
                if (_player.Level >= 12)
                {
                    _player.HasSlowDigestion = true;
                }
                if (_player.Level >= 14)
                {
                    _player.HasSustainConstitution = true;
                }
                if (_player.Level >= 16)
                {
                    _player.HasPoisonResistance = true;
                }
                if (_player.Level >= 18)
                {
                    _player.HasSustainDexterity = true;
                }
                if (_player.Level >= 20)
                {
                    _player.HasSustainStrength = true;
                }
                if (_player.Level >= 22)
                {
                    _player.HasHoldLife = true;
                }
                if (_player.Level >= 24)
                {
                    _player.HasFreeAction = true;
                }
                if (_player.Level >= 26)
                {
                    _player.HasTelepathy = true;
                }
                if (_player.Level >= 28)
                {
                    _player.HasDarkResistance = true;
                }
                if (_player.Level >= 30)
                {
                    _player.HasLightResistance = true;
                }
                if (_player.Level >= 32)
                {
                    _player.HasSustainCharisma = true;
                }
                if (_player.Level >= 34)
                {
                    _player.HasSoundResistance = true;
                }
                if (_player.Level >= 36)
                {
                    _player.HasDisenchantResistance = true;
                }
                if (_player.Level >= 38)
                {
                    _player.HasRegeneration = true;
                }
                if (_player.Level >= 40)
                {
                    _player.HasSustainIntelligence = true;
                }
                if (_player.Level >= 42)
                {
                    _player.HasChaosResistance = true;
                }
                if (_player.Level >= 44)
                {
                    _player.HasSustainWisdom = true;
                }
                if (_player.Level >= 46)
                {
                    _player.HasNexusResistance = true;
                }
                if (_player.Level >= 48)
                {
                    _player.HasShardResistance = true;
                }
                if (_player.Level >= 50)
                {
                    _player.HasNetherResistance = true;
                }
            }
            if (_player.RaceIndex == RaceId.Elf)
            {
                _player.HasLightResistance = true;
            }
            if (_player.RaceIndex == RaceId.Hobbit)
            {
                _player.HasSustainDexterity = true;
            }
            if (_player.RaceIndex == RaceId.Gnome)
            {
                _player.HasFreeAction = true;
            }
            if (_player.RaceIndex == RaceId.Dwarf)
            {
                _player.HasBlindnessResistance = true;
            }
            if (_player.RaceIndex == RaceId.HalfOrc)
            {
                _player.HasDarkResistance = true;
            }
            if (_player.RaceIndex == RaceId.HalfTroll)
            {
                _player.HasSustainStrength = true;
                if (_player.Level > 14)
                {
                    _player.HasRegeneration = true;
                    _player.HasSlowDigestion = true;
                }
            }
            if (_player.RaceIndex == RaceId.Great)
            {
                _player.HasSustainConstitution = true;
                _player.HasRegeneration = true;
            }
            if (_player.RaceIndex == RaceId.HighElf)
            {
                _player.HasSeeInvisibility = true;
                _player.HasLightResistance = true;
            }
            if (_player.RaceIndex == RaceId.TchoTcho)
            {
                _player.HasFearResistance = true;
            }
            else if (_player.RaceIndex == RaceId.HalfOgre)
            {
                _player.HasDarkResistance = true;
                _player.HasSustainStrength = true;
            }
            else if (_player.RaceIndex == RaceId.HalfGiant)
            {
                _player.HasSustainStrength = true;
                _player.HasShardResistance = true;
            }
            else if (_player.RaceIndex == RaceId.HalfTitan)
            {
                _player.HasChaosResistance = true;
            }
            else if (_player.RaceIndex == RaceId.Cyclops)
            {
                _player.HasSoundResistance = true;
            }
            else if (_player.RaceIndex == RaceId.Yeek)
            {
                _player.HasAcidResistance = true;
                if (_player.Level > 19)
                {
                    _player.HasAcidImmunity = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Klackon)
            {
                _player.HasConfusionResistance = true;
                _player.HasAcidResistance = true;
            }
            else if (_player.RaceIndex == RaceId.Kobold)
            {
                _player.HasPoisonResistance = true;
            }
            else if (_player.RaceIndex == RaceId.Nibelung)
            {
                _player.HasDisenchantResistance = true;
                _player.HasDarkResistance = true;
            }
            else if (_player.RaceIndex == RaceId.DarkElf)
            {
                _player.HasDarkResistance = true;
                if (_player.Level > 19)
                {
                    _player.HasSeeInvisibility = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Draconian)
            {
                _player.HasFeatherFall = true;
                if (_player.Level > 4)
                {
                    _player.HasFireResistance = true;
                }
                if (_player.Level > 9)
                {
                    _player.HasColdResistance = true;
                }
                if (_player.Level > 14)
                {
                    _player.HasAcidResistance = true;
                }
                if (_player.Level > 19)
                {
                    _player.HasLightningResistance = true;
                }
                if (_player.Level > 34)
                {
                    _player.HasPoisonResistance = true;
                }
            }
            else if (_player.RaceIndex == RaceId.MindFlayer)
            {
                _player.HasSustainIntelligence = true;
                _player.HasSustainWisdom = true;
                if (_player.Level > 14)
                {
                    _player.HasSeeInvisibility = true;
                }
                if (_player.Level > 29)
                {
                    _player.HasTelepathy = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Imp)
            {
                _player.HasFireResistance = true;
                if (_player.Level > 9)
                {
                    _player.HasSeeInvisibility = true;
                }
                if (_player.Level > 19)
                {
                    _player.HasFireImmunity = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Golem)
            {
                if (_player.Level > 34)
                {
                    _player.HasHoldLife = true;
                }
                _player.HasSlowDigestion = true;
                _player.HasFreeAction = true;
                _player.HasSeeInvisibility = true;
                _player.HasPoisonResistance = true;
            }
            else if (_player.RaceIndex == RaceId.Skeleton)
            {
                _player.HasShardResistance = true;
                _player.HasHoldLife = true;
                _player.HasSeeInvisibility = true;
                _player.HasPoisonResistance = true;
                if (_player.Level > 9)
                {
                    _player.HasColdResistance = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Zombie)
            {
                _player.HasNetherResistance = true;
                _player.HasHoldLife = true;
                _player.HasSeeInvisibility = true;
                _player.HasPoisonResistance = true;
                _player.HasSlowDigestion = true;
                if (_player.Level > 4)
                {
                    _player.HasColdResistance = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Vampire)
            {
                _player.HasDarkResistance = true;
                _player.HasHoldLife = true;
                _player.HasNetherResistance = true;
                _player.HasColdResistance = true;
                _player.HasPoisonResistance = true;
                _player.HasGlow = true;
            }
            else if (_player.RaceIndex == RaceId.Spectre)
            {
                _player.HasFeatherFall = true;
                _player.HasNetherResistance = true;
                _player.HasHoldLife = true;
                _player.HasSeeInvisibility = true;
                _player.HasPoisonResistance = true;
                _player.HasSlowDigestion = true;
                _player.HasColdResistance = true;
                _player.HasGlow = true;
                if (_player.Level > 34)
                {
                    _player.HasTelepathy = true;
                }
            }
            else if (_player.RaceIndex == RaceId.Sprite)
            {
                _player.HasFeatherFall = true;
                _player.HasGlow = true;
                _player.HasLightResistance = true;
            }
            else if (_player.RaceIndex == RaceId.MiriNigri)
            {
                _player.HasConfusionResistance = true;
                _player.HasSoundResistance = true;
            }
            _player.Speed = 110;
            _player.MeleeAttacksPerRound = 1;
            _player.MissileAttacksPerRound = 1;
            _player.AmmunitionItemCategory = 0;
            for (i = 0; i < 6; i++)
            {
                _player.AbilityScores[i].Bonus += _player.Race.AbilityBonus[i] + _player.Profession.AbilityBonus[i];
            }
            _player.AbilityScores[Ability.Strength].Bonus += _player.Dna.StrengthBonus;
            _player.AbilityScores[Ability.Intelligence].Bonus += _player.Dna.IntelligenceBonus;
            _player.AbilityScores[Ability.Wisdom].Bonus += _player.Dna.WisdomBonus;
            _player.AbilityScores[Ability.Dexterity].Bonus += _player.Dna.DexterityBonus;
            _player.AbilityScores[Ability.Constitution].Bonus += _player.Dna.ConstitutionBonus;
            _player.AbilityScores[Ability.Charisma].Bonus += _player.Dna.CharismaBonus;
            _player.Speed += _player.Dna.SpeedBonus;
            _player.HasRegeneration |= _player.Dna.Regen;
            _player.SkillSearchFrequency += _player.Dna.SearchBonus;
            _player.SkillSearching += _player.Dna.SearchBonus;
            _player.InfravisionRange += _player.Dna.InfravisionBonus;
            _player.HasLightningShield |= _player.Dna.ElecHit;
            _player.HasFireShield |= _player.Dna.FireHit;
            _player.HasGlow |= _player.Dna.FireHit;
            _player.ArmourClassBonus += _player.Dna.ArmourClassBonus;
            _player.DisplayedArmourClassBonus += _player.Dna.ArmourClassBonus;
            _player.HasFeatherFall |= _player.Dna.FeatherFall;
            _player.HasFearResistance |= _player.Dna.ResFear;
            _player.HasTimeResistance |= _player.Dna.ResTime;
            _player.HasTelepathy |= _player.Dna.Esp;
            _player.SkillStealth += _player.Dna.StealthBonus;
            _player.HasFreeAction |= _player.Dna.FreeAction;
            _player.HasElementalVulnerability |= _player.Dna.Vulnerable;
            if (_player.Dna.MagicResistance)
            {
                _player.SkillSavingThrow += 15 + (_player.Level / 5);
            }
            if (_player.Dna.SuppressRegen)
            {
                _player.HasRegeneration = false;
            }
            if (_player.Dna.CharismaOverride)
            {
                _player.AbilityScores[Ability.Charisma].Bonus = 0;
            }
            if (_player.Dna.SustainAll)
            {
                _player.HasSustainConstitution = true;
                if (_player.Level > 9)
                {
                    _player.HasSustainStrength = true;
                }
                if (_player.Level > 19)
                {
                    _player.HasSustainDexterity = true;
                }
                if (_player.Level > 29)
                {
                    _player.HasSustainWisdom = true;
                }
                if (_player.Level > 39)
                {
                    _player.HasSustainIntelligence = true;
                }
                if (_player.Level > 49)
                {
                    _player.HasSustainCharisma = true;
                }
            }
            for (i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                oPtr = _player.Inventory[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                oPtr.GetMergedFlags(f1, f2, f3);
                if (f1.IsSet(ItemFlag1.Str))
                {
                    _player.AbilityScores[Ability.Strength].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Int))
                {
                    _player.AbilityScores[Ability.Intelligence].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Wis))
                {
                    _player.AbilityScores[Ability.Wisdom].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Dex))
                {
                    _player.AbilityScores[Ability.Dexterity].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Con))
                {
                    _player.AbilityScores[Ability.Constitution].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Cha))
                {
                    _player.AbilityScores[Ability.Charisma].Bonus += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Stealth))
                {
                    _player.SkillStealth += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Search))
                {
                    _player.SkillSearching += oPtr.TypeSpecificValue * 5;
                }
                if (f1.IsSet(ItemFlag1.Search))
                {
                    _player.SkillSearchFrequency += oPtr.TypeSpecificValue * 5;
                }
                if (f1.IsSet(ItemFlag1.Infra))
                {
                    _player.InfravisionRange += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Tunnel))
                {
                    _player.SkillDigging += oPtr.TypeSpecificValue * 20;
                }
                if (f1.IsSet(ItemFlag1.Speed))
                {
                    _player.Speed += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Blows))
                {
                    extraBlows += oPtr.TypeSpecificValue;
                }
                if (f1.IsSet(ItemFlag1.Impact))
                {
                    _player.HasQuakeWeapon = true;
                }
                if (f3.IsSet(ItemFlag3.AntiTheft))
                {
                    _player.HasAntiTheft = true;
                }
                if (f3.IsSet(ItemFlag3.XtraShots))
                {
                    extraShots++;
                }
                if (f3.IsSet(ItemFlag3.Aggravate))
                {
                    _player.HasAggravation = true;
                }
                if (f3.IsSet(ItemFlag3.Teleport))
                {
                    _player.HasRandomTeleport = true;
                }
                if (f3.IsSet(ItemFlag3.DrainExp))
                {
                    _player.HasExperienceDrain = true;
                }
                if (f3.IsSet(ItemFlag3.Blessed))
                {
                    _player.HasBlessedBlade = true;
                }
                if (f3.IsSet(ItemFlag3.XtraMight))
                {
                    _player.HasExtraMight = true;
                }
                if (f3.IsSet(ItemFlag3.SlowDigest))
                {
                    _player.HasSlowDigestion = true;
                }
                if (f3.IsSet(ItemFlag3.Regen))
                {
                    _player.HasRegeneration = true;
                }
                if (f3.IsSet(ItemFlag3.Telepathy))
                {
                    _player.HasTelepathy = true;
                }
                if (f3.IsSet(ItemFlag3.Lightsource))
                {
                    _player.HasGlow = true;
                }
                if (f3.IsSet(ItemFlag3.SeeInvis))
                {
                    _player.HasSeeInvisibility = true;
                }
                if (f3.IsSet(ItemFlag3.Feather))
                {
                    _player.HasFeatherFall = true;
                }
                if (f2.IsSet(ItemFlag2.FreeAct))
                {
                    _player.HasFreeAction = true;
                }
                if (f2.IsSet(ItemFlag2.HoldLife))
                {
                    _player.HasHoldLife = true;
                }
                if (f3.IsSet(ItemFlag3.Wraith))
                {
                    _player.TimedEtherealness = Math.Max(_player.TimedEtherealness, 20);
                }
                if (f2.IsSet(ItemFlag2.ImFire))
                {
                    _player.HasFireImmunity = true;
                }
                if (f2.IsSet(ItemFlag2.ImAcid))
                {
                    _player.HasAcidImmunity = true;
                }
                if (f2.IsSet(ItemFlag2.ImCold))
                {
                    _player.HasColdImmunity = true;
                }
                if (f2.IsSet(ItemFlag2.ImElec))
                {
                    _player.HasLightningImmunity = true;
                }
                if (f2.IsSet(ItemFlag2.ResAcid))
                {
                    _player.HasAcidResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResElec))
                {
                    _player.HasLightningResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResFire))
                {
                    _player.HasFireResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResCold))
                {
                    _player.HasColdResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResPois))
                {
                    _player.HasPoisonResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResFear))
                {
                    _player.HasFearResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResConf))
                {
                    _player.HasConfusionResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResSound))
                {
                    _player.HasSoundResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResLight))
                {
                    _player.HasLightResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResDark))
                {
                    _player.HasDarkResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResChaos))
                {
                    _player.HasChaosResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResDisen))
                {
                    _player.HasDisenchantResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResShards))
                {
                    _player.HasShardResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResNexus))
                {
                    _player.HasNexusResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResBlind))
                {
                    _player.HasBlindnessResistance = true;
                }
                if (f2.IsSet(ItemFlag2.ResNether))
                {
                    _player.HasNetherResistance = true;
                }
                if (f2.IsSet(ItemFlag2.Reflect))
                {
                    _player.HasReflection = true;
                }
                if (f3.IsSet(ItemFlag3.ShFire))
                {
                    _player.HasFireShield = true;
                }
                if (f3.IsSet(ItemFlag3.ShElec))
                {
                    _player.HasLightningShield = true;
                }
                if (f3.IsSet(ItemFlag3.NoMagic))
                {
                    _player.HasAntiMagic = true;
                }
                if (f3.IsSet(ItemFlag3.NoTele))
                {
                    _player.HasAntiTeleport = true;
                }
                if (f2.IsSet(ItemFlag2.SustStr))
                {
                    _player.HasSustainStrength = true;
                }
                if (f2.IsSet(ItemFlag2.SustInt))
                {
                    _player.HasSustainIntelligence = true;
                }
                if (f2.IsSet(ItemFlag2.SustWis))
                {
                    _player.HasSustainWisdom = true;
                }
                if (f2.IsSet(ItemFlag2.SustDex))
                {
                    _player.HasSustainDexterity = true;
                }
                if (f2.IsSet(ItemFlag2.SustCon))
                {
                    _player.HasSustainConstitution = true;
                }
                if (f2.IsSet(ItemFlag2.SustCha))
                {
                    _player.HasSustainCharisma = true;
                }
                _player.BaseArmourClass += oPtr.BaseArmourClass;
                _player.DisplayedBaseArmourClass += oPtr.BaseArmourClass;
                _player.ArmourClassBonus += oPtr.BonusArmourClass;
                if (oPtr.IsKnown())
                {
                    _player.DisplayedArmourClassBonus += oPtr.BonusArmourClass;
                }
                if (i == InventorySlot.MeleeWeapon)
                {
                    continue;
                }
                if (i == InventorySlot.RangedWeapon)
                {
                    continue;
                }
                _player.AttackBonus += oPtr.BonusToHit;
                _player.DamageBonus += oPtr.BonusDamage;
                if (oPtr.IsKnown())
                {
                    _player.DisplayedAttackBonus += oPtr.BonusToHit;
                }
                if (oPtr.IsKnown())
                {
                    _player.DisplayedDamageBonus += oPtr.BonusDamage;
                }
            }
            if ((_player.ProfessionIndex == CharacterClass.Monk || _player.ProfessionIndex == CharacterClass.Mystic) && !MartialArtistHeavyArmour())
            {
                if (_player.Inventory[InventorySlot.Body].ItemType == null)
                {
                    _player.ArmourClassBonus += _player.Level * 3 / 2;
                    _player.DisplayedArmourClassBonus += _player.Level * 3 / 2;
                }
                if (_player.Inventory[InventorySlot.Cloak].ItemType == null && _player.Level > 15)
                {
                    _player.ArmourClassBonus += (_player.Level - 13) / 3;
                    _player.DisplayedArmourClassBonus += (_player.Level - 13) / 3;
                }
                if (_player.Inventory[InventorySlot.Arm].ItemType == null && _player.Level > 10)
                {
                    _player.ArmourClassBonus += (_player.Level - 8) / 3;
                    _player.DisplayedArmourClassBonus += (_player.Level - 8) / 3;
                }
                if (_player.Inventory[InventorySlot.Head].ItemType == null && _player.Level > 4)
                {
                    _player.ArmourClassBonus += (_player.Level - 2) / 3;
                    _player.DisplayedArmourClassBonus += (_player.Level - 2) / 3;
                }
                if (_player.Inventory[InventorySlot.Hands].ItemType == null)
                {
                    _player.ArmourClassBonus += _player.Level / 2;
                    _player.DisplayedArmourClassBonus += _player.Level / 2;
                }
                if (_player.Inventory[InventorySlot.Feet].ItemType == null)
                {
                    _player.ArmourClassBonus += _player.Level / 3;
                    _player.DisplayedArmourClassBonus += _player.Level / 3;
                }
            }
            if (_player.HasFireShield)
            {
                _player.HasGlow = true;
            }
            if (_player.RaceIndex == RaceId.Golem)
            {
                _player.ArmourClassBonus += 20 + (_player.Level / 5);
                _player.DisplayedArmourClassBonus += 20 + (_player.Level / 5);
            }
            for (i = 0; i < 6; i++)
            {
                int ind;
                int top = _player.AbilityScores[i]
                    .ModifyStatValue(_player.AbilityScores[i].InnateMax, _player.AbilityScores[i].Bonus);
                if (_player.AbilityScores[i].AdjustedMax != top)
                {
                    _player.AbilityScores[i].AdjustedMax = top;
                    _player.RedrawNeeded.Set(RedrawFlag.PrStats);
                }
                int use = _player.AbilityScores[i]
                    .ModifyStatValue(_player.AbilityScores[i].Innate, _player.AbilityScores[i].Bonus);
                if (i == Ability.Charisma && _player.Dna.CharismaOverride)
                {
                    if (use < 8 + (2 * _player.Level))
                    {
                        use = 8 + (2 * _player.Level);
                    }
                }
                if (_player.AbilityScores[i].Adjusted != use)
                {
                    _player.AbilityScores[i].Adjusted = use;
                    _player.RedrawNeeded.Set(RedrawFlag.PrStats);
                }
                if (use <= 18)
                {
                    ind = use - 3;
                }
                else if (use <= 18 + 219)
                {
                    ind = 15 + ((use - 18) / 10);
                }
                else
                {
                    ind = 37;
                }
                if (_player.AbilityScores[i].TableIndex != ind)
                {
                    _player.AbilityScores[i].TableIndex = ind;
                    if (i == Ability.Constitution)
                    {
                        _player.UpdatesNeeded.Set(UpdateFlags.UpdateHealth);
                    }
                    else if (i == Ability.Intelligence)
                    {
                        if (_player.Spellcasting.SpellStat == Ability.Intelligence)
                        {
                            _player.UpdatesNeeded.Set(UpdateFlags.UpdateVis | UpdateFlags.UpdateSpells);
                        }
                    }
                    else if (i == Ability.Wisdom)
                    {
                        if (_player.Spellcasting.SpellStat == Ability.Wisdom)
                        {
                            _player.UpdatesNeeded.Set(UpdateFlags.UpdateVis | UpdateFlags.UpdateSpells);
                        }
                    }
                    else if (i == Ability.Charisma)
                    {
                        if (_player.Spellcasting.SpellStat == Ability.Charisma)
                        {
                            _player.UpdatesNeeded.Set(UpdateFlags.UpdateVis | UpdateFlags.UpdateSpells);
                        }
                    }
                }
            }
            if (_player.TimedStun > 50)
            {
                _player.AttackBonus -= 20;
                _player.DisplayedAttackBonus -= 20;
                _player.DamageBonus -= 20;
                _player.DisplayedDamageBonus -= 20;
            }
            else if (_player.TimedStun != 0)
            {
                _player.AttackBonus -= 5;
                _player.DisplayedAttackBonus -= 5;
                _player.DamageBonus -= 5;
                _player.DisplayedDamageBonus -= 5;
            }
            if (_player.TimedInvulnerability != 0)
            {
                _player.ArmourClassBonus += 100;
                _player.DisplayedArmourClassBonus += 100;
            }
            if (_player.TimedEtherealness != 0)
            {
                _player.ArmourClassBonus += 100;
                _player.DisplayedArmourClassBonus += 100;
                _player.HasReflection = true;
            }
            if (_player.TimedBlessing != 0)
            {
                _player.ArmourClassBonus += 5;
                _player.DisplayedArmourClassBonus += 5;
                _player.AttackBonus += 10;
                _player.DisplayedAttackBonus += 10;
            }
            if (_player.TimedStoneskin != 0)
            {
                _player.ArmourClassBonus += 50;
                _player.DisplayedArmourClassBonus += 50;
            }
            if (_player.TimedHeroism != 0)
            {
                _player.AttackBonus += 12;
                _player.DisplayedAttackBonus += 12;
            }
            if (_player.TimedSuperheroism != 0)
            {
                _player.AttackBonus += 24;
                _player.DisplayedAttackBonus += 24;
                _player.ArmourClassBonus -= 10;
                _player.DisplayedArmourClassBonus -= 10;
            }
            if (_player.TimedHaste != 0)
            {
                _player.Speed += 10;
            }
            if (_player.TimedSlow != 0)
            {
                _player.Speed -= 10;
            }
            if (_player.RaceIndex == RaceId.Klackon || _player.RaceIndex == RaceId.Sprite ||
                ((_player.ProfessionIndex == CharacterClass.Monk || _player.ProfessionIndex == CharacterClass.Mystic) && !MartialArtistHeavyArmour()))
            {
                _player.Speed += _player.Level / 10;
            }
            if (_player.TimedTelepathy != 0)
            {
                _player.HasTelepathy = true;
            }
            if (_player.TimedSeeInvisibility != 0)
            {
                _player.HasSeeInvisibility = true;
            }
            if (_player.TimedInfravision != 0)
            {
                _player.InfravisionRange++;
            }
            if (_player.HasChaosResistance)
            {
                _player.HasConfusionResistance = true;
            }
            if (_player.TimedHeroism != 0 || _player.TimedSuperheroism != 0)
            {
                _player.HasFearResistance = true;
            }
            if (_player.HasTelepathy != oldTelepathy)
            {
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            }
            if (_player.HasSeeInvisibility != oldSeeInv)
            {
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            }
            int j = _player.WeightCarried;
            i = WeightLimit();
            if (j > i / 2)
            {
                _player.Speed -= (j - (i / 2)) / (i / 10);
            }
            if (_player.Food >= Constants.PyFoodMax)
            {
                _player.Speed -= 10;
            }
            if (_player.IsSearching)
            {
                _player.Speed -= 10;
            }
            if (_player.Speed != oldSpeed)
            {
                _player.RedrawNeeded.Set(RedrawFlag.PrSpeed);
            }
            _player.ArmourClassBonus += _player.AbilityScores[Ability.Dexterity].DexArmourClassBonus;
            _player.DamageBonus += _player.AbilityScores[Ability.Strength].StrDamageBonus;
            _player.AttackBonus += _player.AbilityScores[Ability.Dexterity].DexAttackBonus;
            _player.AttackBonus += _player.AbilityScores[Ability.Strength].StrAttackBonus;
            _player.DisplayedArmourClassBonus += _player.AbilityScores[Ability.Dexterity].DexArmourClassBonus;
            _player.DisplayedDamageBonus += _player.AbilityScores[Ability.Strength].StrDamageBonus;
            _player.DisplayedAttackBonus += _player.AbilityScores[Ability.Dexterity].DexAttackBonus;
            _player.DisplayedAttackBonus += _player.AbilityScores[Ability.Strength].StrAttackBonus;
            if (_player.DisplayedBaseArmourClass != oldDisAc || _player.DisplayedArmourClassBonus != oldDisToA)
            {
                _player.RedrawNeeded.Set(RedrawFlag.PrArmor);
            }
            int hold = _player.AbilityScores[Ability.Strength].StrMaxWeaponWeight;
            oPtr = _player.Inventory[InventorySlot.RangedWeapon];
            _player.HasHeavyBow = false;
            if (hold < oPtr.Weight / 10)
            {
                _player.AttackBonus += 2 * (hold - (oPtr.Weight / 10));
                _player.DisplayedAttackBonus += 2 * (hold - (oPtr.Weight / 10));
                _player.HasHeavyBow = true;
            }
            if (oPtr.ItemType != null && !_player.HasHeavyBow)
            {
                switch (oPtr.ItemSubCategory)
                {
                    case BowType.Sling:
                        {
                            _player.AmmunitionItemCategory = ItemCategory.Shot;
                            break;
                        }
                    case BowType.Shortbow:
                    case BowType.Longbow:
                        {
                            _player.AmmunitionItemCategory = ItemCategory.Arrow;
                            break;
                        }
                    case BowType.LightCrossbow:
                    case BowType.HeavyCrossbow:
                        {
                            _player.AmmunitionItemCategory = ItemCategory.Bolt;
                            break;
                        }
                }
                if (_player.ProfessionIndex == CharacterClass.Ranger && _player.AmmunitionItemCategory == ItemCategory.Arrow)
                {
                    if (_player.Level >= 20)
                    {
                        _player.MissileAttacksPerRound++;
                    }
                    if (_player.Level >= 40)
                    {
                        _player.MissileAttacksPerRound++;
                    }
                }
                if (_player.ProfessionIndex == CharacterClass.Warrior && _player.AmmunitionItemCategory <= ItemCategory.Bolt &&
                    _player.AmmunitionItemCategory >= ItemCategory.Shot)
                {
                    if (_player.Level >= 25)
                    {
                        _player.MissileAttacksPerRound++;
                    }
                    if (_player.Level >= 50)
                    {
                        _player.MissileAttacksPerRound++;
                    }
                }
                _player.MissileAttacksPerRound += extraShots;
                if (_player.MissileAttacksPerRound < 1)
                {
                    _player.MissileAttacksPerRound = 1;
                }
            }
            oPtr = _player.Inventory[InventorySlot.MeleeWeapon];
            _player.HasHeavyWeapon = false;
            if (hold < oPtr.Weight / 10)
            {
                _player.AttackBonus += 2 * (hold - (oPtr.Weight / 10));
                _player.DisplayedAttackBonus += 2 * (hold - (oPtr.Weight / 10));
                _player.HasHeavyWeapon = true;
            }
            if (oPtr.ItemType != null && !_player.HasHeavyWeapon)
            {
                int num = 0, wgt = 0, mul = 0;
                switch (_player.ProfessionIndex)
                {
                    case CharacterClass.Warrior:
                        num = 6;
                        wgt = 30;
                        mul = 5;
                        break;

                    case CharacterClass.Mage:
                    case CharacterClass.HighMage:
                    case CharacterClass.Cultist:
                    case CharacterClass.Channeler:
                        num = 4;
                        wgt = 40;
                        mul = 2;
                        break;

                    case CharacterClass.Priest:
                    case CharacterClass.Mindcrafter:
                    case CharacterClass.Druid:
                    case CharacterClass.ChosenOne:
                        num = 5;
                        wgt = 35;
                        mul = 3;
                        break;

                    case CharacterClass.Rogue:
                        num = 5;
                        wgt = 30;
                        mul = 3;
                        break;

                    case CharacterClass.Ranger:
                        num = 5;
                        wgt = 35;
                        mul = 4;
                        break;

                    case CharacterClass.Paladin:
                        num = 5;
                        wgt = 30;
                        mul = 4;
                        break;

                    case CharacterClass.WarriorMage:
                        num = 5;
                        wgt = 35;
                        mul = 3;
                        break;

                    case CharacterClass.Fanatic:
                        num = 5;
                        wgt = 30;
                        mul = 4;
                        break;

                    case CharacterClass.Monk:
                    case CharacterClass.Mystic:
                        num = _player.Level < 40 ? 3 : 4;
                        wgt = 40;
                        mul = 4;
                        break;
                }
                int div = oPtr.Weight < wgt ? wgt : oPtr.Weight;
                int strIndex = _player.AbilityScores[Ability.Strength].StrAttackSpeedComponent * mul / div;
                if (strIndex > 11)
                {
                    strIndex = 11;
                }
                int dexIndex = _player.AbilityScores[Ability.Dexterity].DexAttackSpeedComponent;
                if (dexIndex > 11)
                {
                    dexIndex = 11;
                }
                _player.MeleeAttacksPerRound = _blowsTable[strIndex][dexIndex];
                if (_player.MeleeAttacksPerRound > num)
                {
                    _player.MeleeAttacksPerRound = num;
                }
                _player.MeleeAttacksPerRound += extraBlows;
                if (_player.ProfessionIndex == CharacterClass.Warrior)
                {
                    _player.MeleeAttacksPerRound += _player.Level / 15;
                }
                if (_player.MeleeAttacksPerRound < 1)
                {
                    _player.MeleeAttacksPerRound = 1;
                }
                _player.SkillDigging += oPtr.Weight / 10;
            }
            else if ((_player.ProfessionIndex == CharacterClass.Monk || _player.ProfessionIndex == CharacterClass.Mystic) && MartialArtistEmptyHands())
            {
                _player.MeleeAttacksPerRound = 0;
                if (_player.Level > 9)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 19)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 29)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 34)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 39)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 44)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (_player.Level > 49)
                {
                    _player.MeleeAttacksPerRound++;
                }
                if (MartialArtistHeavyArmour())
                {
                    _player.MeleeAttacksPerRound /= 2;
                }
                _player.MeleeAttacksPerRound += 1 + extraBlows;
                if (!MartialArtistHeavyArmour())
                {
                    _player.AttackBonus += _player.Level / 3;
                    _player.DamageBonus += _player.Level / 3;
                    _player.DisplayedAttackBonus += _player.Level / 3;
                    _player.DisplayedDamageBonus += _player.Level / 3;
                }
            }
            _player.HasUnpriestlyWeapon = false;
            SaveGame.Instance.MartialArtistArmourAux = false;
            if (_player.ProfessionIndex == CharacterClass.Warrior)
            {
                _player.AttackBonus += _player.Level / 5;
                _player.DamageBonus += _player.Level / 5;
                _player.DisplayedAttackBonus += _player.Level / 5;
                _player.DisplayedDamageBonus += _player.Level / 5;
            }
            if ((_player.ProfessionIndex == CharacterClass.Priest || _player.ProfessionIndex == CharacterClass.Druid) &&
                !_player.HasBlessedBlade && (oPtr.Category == ItemCategory.Sword ||
                                        oPtr.Category == ItemCategory.Polearm))
            {
                _player.AttackBonus -= 2;
                _player.DamageBonus -= 2;
                _player.DisplayedAttackBonus -= 2;
                _player.DisplayedDamageBonus -= 2;
                _player.HasUnpriestlyWeapon = true;
            }
            if (_player.ProfessionIndex == CharacterClass.Cultist &&
                _player.Inventory[InventorySlot.MeleeWeapon].ItemType != null &&
                (oPtr.Category != ItemCategory.Sword || oPtr.ItemSubCategory != SwordType.BladeOfChaos))
            {
                oPtr.GetMergedFlags(f1, f2, f3);
                if (f1.IsClear(ItemFlag1.Chaotic))
                {
                    _player.AttackBonus -= 10;
                    _player.DamageBonus -= 10;
                    _player.DisplayedAttackBonus -= 10;
                    _player.DisplayedDamageBonus -= 10;
                    _player.HasUnpriestlyWeapon = true;
                }
            }
            if (MartialArtistHeavyArmour())
            {
                SaveGame.Instance.MartialArtistArmourAux = true;
            }
            _player.SkillStealth++;
            _player.SkillDisarmTraps += _player.AbilityScores[Ability.Dexterity].DexDisarmBonus;
            _player.SkillDisarmTraps += _player.AbilityScores[Ability.Intelligence].IntDisarmBonus;
            _player.SkillUseDevice += _player.AbilityScores[Ability.Intelligence].IntUseDeviceBonus;
            _player.SkillSavingThrow += _player.AbilityScores[Ability.Wisdom].WisSavingThrowBonus;
            _player.SkillDigging += _player.AbilityScores[Ability.Strength].StrDiggingBonus;
            _player.SkillDisarmTraps += (_player.Profession.DisarmBonusPerLevel * _player.Level) / 10;
            _player.SkillUseDevice += (_player.Profession.DeviceBonusPerLevel * _player.Level) / 10;
            _player.SkillSavingThrow += (_player.Profession.SaveBonusPerLevel * _player.Level) / 10;
            _player.SkillStealth += (_player.Profession.StealthBonusPerLevel * _player.Level) / 10;
            _player.SkillSearching += (_player.Profession.SearchBonusPerLevel * _player.Level) / 10;
            _player.SkillSearchFrequency += (_player.Profession.SearchFrequencyPerLevel * _player.Level) / 10;
            _player.SkillMelee += (_player.Profession.MeleeAttackBonusPerLevel * _player.Level) / 10;
            _player.SkillRanged += (_player.Profession.RangedAttackBonusPerLevel * _player.Level) / 10;
            _player.SkillThrowing += (_player.Profession.RangedAttackBonusPerLevel * _player.Level) / 10;
            if (_player.SkillStealth > 30)
            {
                _player.SkillStealth = 30;
            }
            if (_player.SkillStealth < 0)
            {
                _player.SkillStealth = 0;
            }
            if (_player.SkillDigging < 1)
            {
                _player.SkillDigging = 1;
            }
            if (_player.HasAntiMagic && _player.SkillSavingThrow < 95)
            {
                _player.SkillSavingThrow = 95;
            }
            if (SaveGame.Instance.CharacterXtra)
            {
                return;
            }
            if (_player.OldHeavyBow != _player.HasHeavyBow)
            {
                if (_player.HasHeavyBow)
                {
                    Profile.Instance.MsgPrint("You have trouble wielding such a heavy bow.");
                }
                else if (_player.Inventory[InventorySlot.RangedWeapon].ItemType != null)
                {
                    Profile.Instance.MsgPrint("You have no trouble wielding your bow.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You feel relieved to put down your heavy bow.");
                }
                _player.OldHeavyBow = _player.HasHeavyBow;
            }
            if (_player.OldHeavyWeapon != _player.HasHeavyWeapon)
            {
                if (_player.HasHeavyWeapon)
                {
                    Profile.Instance.MsgPrint("You have trouble wielding such a heavy weapon.");
                }
                else if (_player.Inventory[InventorySlot.MeleeWeapon].ItemType != null)
                {
                    Profile.Instance.MsgPrint("You have no trouble wielding your weapon.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You feel relieved to put down your heavy weapon.");
                }
                _player.OldHeavyWeapon = _player.HasHeavyWeapon;
            }
            if (_player.OldUnpriestlyWeapon != _player.HasUnpriestlyWeapon)
            {
                if (_player.HasUnpriestlyWeapon)
                {
                    Profile.Instance.MsgPrint(_player.ProfessionIndex == CharacterClass.Cultist
                        ? "Your weapon restricts the flow of chaos through you."
                        : "You do not feel comfortable with your weapon.");
                }
                else if (_player.Inventory[InventorySlot.MeleeWeapon].ItemType != null)
                {
                    Profile.Instance.MsgPrint("You feel comfortable with your weapon.");
                }
                else
                {
                    Profile.Instance.MsgPrint(_player.ProfessionIndex == CharacterClass.Cultist
                        ? "Chaos flows freely through you again."
                        : "You feel more comfortable after removing your weapon.");
                }
                _player.OldUnpriestlyWeapon = _player.HasUnpriestlyWeapon;
            }
            if ((_player.ProfessionIndex == CharacterClass.Monk || _player.ProfessionIndex == CharacterClass.Mystic) && SaveGame.Instance.MartialArtistArmourAux !=
                SaveGame.Instance.MartialArtistNotifyAux)
            {
                Profile.Instance.MsgPrint(MartialArtistHeavyArmour()
                    ? "The weight of your armour disrupts your balance."
                    : "You regain your balance.");
                SaveGame.Instance.MartialArtistNotifyAux = SaveGame.Instance.MartialArtistArmourAux;
            }
        }

        public void CalcHitpoints()
        {
            int bonus = _player.AbilityScores[Ability.Constitution].ConHealthBonus;
            int mhp = _player.PlayerHp[_player.Level - 1] + (bonus * _player.Level / 2);
            if (mhp < _player.Level + 1)
            {
                mhp = _player.Level + 1;
            }
            if (_player.TimedHeroism != 0)
            {
                mhp += 10;
            }
            if (_player.TimedSuperheroism != 0)
            {
                mhp += 30;
            }
            var mult = _player.Religion.GetNamedDeity(Pantheon.GodName.Nath_Horthah).AdjustedFavour + 10;
            mhp *= mult;
            mhp /= 10;
            if (_player.MaxHealth != mhp)
            {
                if (_player.Health >= mhp)
                {
                    _player.Health = mhp;
                    _player.FractionalHealth = 0;
                }
                _player.MaxHealth = mhp;
                _player.RedrawNeeded.Set(RedrawFlag.PrHp);
            }
        }

        public void CalcSpells()
        {
            int i, j;
            ISpell sPtr;
            if (_player == null)
            {
                return;
            }
            string p = _player.Spellcasting.Type == CastingType.Arcane ? "spell" : "prayer";
            if (_player.Spellcasting.Type == CastingType.None)
            {
                return;
            }
            if (_player.Realm1 == Realm.None)
            {
                return;
            }
            if (SaveGame.Instance.CharacterXtra)
            {
                return;
            }
            int levels = _player.Level - _player.Spellcasting.SpellFirst + 1;
            if (levels < 0)
            {
                levels = 0;
            }
            int numAllowed = _player.AbilityScores[_player.Spellcasting.SpellStat].HalfSpellsPerLevel * levels / 2;
            int numKnown = 0;
            for (j = 0; j < 64; j++)
            {
                if (_player.Spellcasting.Spells[j / 32][j % 32].Learned)
                {
                    numKnown++;
                }
            }
            _player.SpareSpellSlots = numAllowed - numKnown;
            for (i = 63; i >= 0; i--)
            {
                if (numKnown == 0)
                {
                    break;
                }
                j = _player.Spellcasting.SpellOrder[i];
                if (j >= 99)
                {
                    continue;
                }
                sPtr = _player.Spellcasting.Spells[j / 32][j % 32];
                if (sPtr.Level <= _player.Level)
                {
                    continue;
                }
                if (!sPtr.Learned)
                {
                    continue;
                }
                sPtr.Forgotten = true;
                sPtr.Learned = false;
                numKnown--;
                Profile.Instance.MsgPrint($"You have forgotten the {p} of {sPtr.Name}.");
                _player.SpareSpellSlots++;
            }
            for (i = 63; i >= 0; i--)
            {
                if (_player.SpareSpellSlots >= 0)
                {
                    break;
                }
                if (numKnown == 0)
                {
                    break;
                }
                j = _player.Spellcasting.SpellOrder[i];
                if (j >= 99)
                {
                    continue;
                }
                sPtr = _player.Spellcasting.Spells[j / 32][j % 32];
                if (!sPtr.Learned)
                {
                    continue;
                }
                sPtr.Forgotten = true;
                sPtr.Learned = false;
                numKnown--;
                Profile.Instance.MsgPrint($"You have forgotten the {p} of {sPtr.Name}.");
                _player.SpareSpellSlots++;
            }
            int forgottenTotal = 0;
            for (int l = 0; l < 64; l++)
            {
                if (_player.Spellcasting.Spells[l / 32][l % 32].Forgotten)
                {
                    forgottenTotal++;
                }
            }
            for (i = 0; i < 64; i++)
            {
                if (_player.SpareSpellSlots <= 0)
                {
                    break;
                }
                if (forgottenTotal == 0)
                {
                    break;
                }
                j = _player.Spellcasting.SpellOrder[i];
                if (j >= 99)
                {
                    break;
                }
                sPtr = _player.Spellcasting.Spells[j / 32][j % 32];
                if (sPtr.Level > _player.Level)
                {
                    continue;
                }
                if (!sPtr.Forgotten)
                {
                    continue;
                }
                sPtr.Forgotten = false;
                sPtr.Learned = true;
                forgottenTotal--;
                if (!Gui.FullScreenOverlay)
                {
                    Profile.Instance.MsgPrint($"You have remembered the {p} of {sPtr.Name}.");
                }
                _player.SpareSpellSlots--;
            }
            int k = 0;
            int limit = _player.Realm2 == Realm.None ? 32 : 64;
            for (j = 0; j < limit; j++)
            {
                sPtr = _player.Spellcasting.Spells[j / 32][j % 32];
                if (sPtr.Level > _player.Level)
                {
                    continue;
                }
                if (sPtr.Learned)
                {
                    continue;
                }
                k++;
            }
            if (_player.Realm2 == 0)
            {
                if (k > 32)
                {
                    k = 32;
                }
            }
            else
            {
                if (k > 64)
                {
                    k = 64;
                }
            }
            if (_player.SpareSpellSlots > k)
            {
                _player.SpareSpellSlots = k;
            }
            if (_player.OldSpareSpellSlots != _player.SpareSpellSlots)
            {
                if (_player.SpareSpellSlots != 0)
                {
                    if (!Gui.FullScreenOverlay)
                    {
                        string suffix = _player.SpareSpellSlots != 1 ? "s" : "";
                        Profile.Instance.MsgPrint($"You can learn {_player.SpareSpellSlots} more {p}{suffix}.");
                    }
                }
                _player.OldSpareSpellSlots = _player.SpareSpellSlots;
                _player.RedrawNeeded.Set(RedrawFlag.PrStudy);
            }
        }

        public void CalcTorch()
        {
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            _player.LightLevel = 0;
            for (int i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                Item oPtr = _player.Inventory[i];
                if (i == InventorySlot.Lightsource && oPtr.ItemType != null &&
                    oPtr.Category == ItemCategory.Light)
                {
                    if (oPtr.ItemSubCategory == LightType.Torch && oPtr.TypeSpecificValue > 0)
                    {
                        _player.LightLevel++;
                        continue;
                    }
                    if (oPtr.ItemSubCategory == LightType.Lantern && oPtr.TypeSpecificValue > 0)
                    {
                        _player.LightLevel += 2;
                        continue;
                    }
                    if (oPtr.ItemSubCategory == LightType.Orb)
                    {
                        _player.LightLevel += 2;
                        continue;
                    }
                    if (oPtr.IsArtifact())
                    {
                        _player.LightLevel += 3;
                    }
                }
                else
                {
                    if (oPtr.ItemType == null)
                    {
                        continue;
                    }
                    oPtr.GetMergedFlags(f1, f2, f3);
                    if (f3.IsSet(ItemFlag3.Lightsource))
                    {
                        _player.LightLevel++;
                    }
                }
            }
            if (_player.LightLevel > 5)
            {
                _player.LightLevel = 5;
            }
            if (_player.LightLevel == 0 && _player.HasGlow)
            {
                _player.LightLevel = 1;
            }
            if (_player.OldLightLevel != _player.LightLevel)
            {
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateLight);
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                _player.OldLightLevel = _player.LightLevel;
            }
        }

        public void CalcVis()
        {
            int levels;
            switch (_player.Spellcasting.Type)
            {
                case CastingType.None:
                    return;

                case CastingType.Arcane:
                case CastingType.Divine:
                    levels = _player.Level - _player.Spellcasting.SpellFirst + 1;
                    break;

                case CastingType.Mentalism:
                case CastingType.Channeling:
                    levels = _player.Level;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (levels < 0)
            {
                levels = 0;
            }
            int msp = _player.AbilityScores[_player.Spellcasting.SpellStat].VisBonus * levels / 2;
            if (msp != 0)
            {
                msp++;
            }
            if (msp != 0 && _player.ProfessionIndex == CharacterClass.HighMage)
            {
                msp += msp / 4;
            }
            if (_player.Spellcasting.Type == CastingType.Arcane)
            {
                FlagSet f1 = new FlagSet();
                FlagSet f2 = new FlagSet();
                FlagSet f3 = new FlagSet();
                _player.HasRestrictingGloves = false;
                Item oPtr = _player.Inventory[InventorySlot.Hands];
                oPtr.GetMergedFlags(f1, f2, f3);
                if (oPtr.ItemType != null && f2.IsClear(ItemFlag2.FreeAct) && f1.IsClear(ItemFlag1.Dex) &&
                    oPtr.TypeSpecificValue > 0)
                {
                    _player.HasRestrictingGloves = true;
                    msp = 3 * msp / 4;
                }
                _player.HasRestrictingArmour = false;
                int curWgt = 0;
                curWgt += _player.Inventory[InventorySlot.Body].Weight;
                curWgt += _player.Inventory[InventorySlot.Head].Weight;
                curWgt += _player.Inventory[InventorySlot.Arm].Weight;
                curWgt += _player.Inventory[InventorySlot.Cloak].Weight;
                curWgt += _player.Inventory[InventorySlot.Hands].Weight;
                curWgt += _player.Inventory[InventorySlot.Feet].Weight;
                int maxWgt = _player.Spellcasting.SpellWeight;
                if ((curWgt - maxWgt) / 10 > 0)
                {
                    _player.HasRestrictingArmour = true;
                    msp -= (curWgt - maxWgt) / 10;
                }
            }
            if (msp < 0)
            {
                msp = 0;
            }
            var mult = _player.Religion.GetNamedDeity(Pantheon.GodName.Tamash).AdjustedFavour + 10;
            msp *= mult;
            msp /= 10;
            if (_player.MaxVis != msp)
            {
                if (_player.Vis >= msp)
                {
                    _player.Vis = msp;
                    _player.FractionalVis = 0;
                }
                _player.MaxVis = msp;
                _player.RedrawNeeded.Set(RedrawFlag.PrVis);
            }
            if (SaveGame.Instance.CharacterXtra)
            {
                return;
            }
            if (_player.OldRestrictingGloves != _player.HasRestrictingGloves)
            {
                Profile.Instance.MsgPrint(_player.HasRestrictingGloves
                    ? "Your covered hands feel unsuitable for spellcasting."
                    : "Your hands feel more suitable for spellcasting.");
                _player.OldRestrictingGloves = _player.HasRestrictingGloves;
            }
            if (_player.OldRestrictingArmour != _player.HasRestrictingArmour)
            {
                Profile.Instance.MsgPrint(_player.HasRestrictingArmour
                    ? "The weight of your armour encumbers your movement."
                    : "You feel able to move more freely.");
                _player.OldRestrictingArmour = _player.HasRestrictingArmour;
            }
        }

        public void HealthRedraw()
        {
            if (SaveGame.Instance.TrackedMonsterIndex == 0)
            {
                Gui.Erase(ScreenLocation.RowInfo, ScreenLocation.ColInfo, 12);
                Gui.Erase(ScreenLocation.RowInfo - 3, ScreenLocation.ColInfo, 12);
                Gui.Erase(ScreenLocation.RowInfo - 2, ScreenLocation.ColInfo, 12);
                Gui.Erase(ScreenLocation.RowInfo - 1, ScreenLocation.ColInfo, 12);
            }
            else if (!_level.Monsters[SaveGame.Instance.TrackedMonsterIndex].IsVisible)
            {
                Gui.Print(Colour.White, "[----------]", ScreenLocation.RowInfo, ScreenLocation.ColInfo, 12);
            }
            else if (_player.TimedHallucinations != 0)
            {
                Gui.Print(Colour.White, "[----------]", ScreenLocation.RowInfo, ScreenLocation.ColInfo, 12);
            }
            else if (_level.Monsters[SaveGame.Instance.TrackedMonsterIndex].Health < 0)
            {
                Gui.Print(Colour.White, "[----------]", ScreenLocation.RowInfo, ScreenLocation.ColInfo, 12);
            }
            else
            {
                Monster mPtr = _level.Monsters[SaveGame.Instance.TrackedMonsterIndex];
                Colour attr = Colour.Red;
                string smb = "**********";
                int pct = 100 * mPtr.Health / mPtr.MaxHealth;
                if (pct >= 10)
                {
                    attr = Colour.BrightRed;
                }
                if (pct >= 25)
                {
                    attr = Colour.Orange;
                }
                if (pct >= 60)
                {
                    attr = Colour.Yellow;
                }
                if (pct >= 100)
                {
                    attr = Colour.BrightGreen;
                }
                if (mPtr.FearLevel != 0)
                {
                    attr = Colour.Purple;
                    smb = "AFRAID****";
                }
                if (mPtr.SleepLevel != 0)
                {
                    attr = Colour.Blue;
                    smb = "SLEEPING**";
                }
                if ((mPtr.Mind & Constants.SmFriendly) != 0)
                {
                    attr = Colour.BrightBrown;
                    smb = "FRIENDLY**";
                }
                int len = pct < 10 ? 1 : pct < 90 ? (pct / 10) + 1 : 10;
                Gui.Print(Colour.White, "[----------]", ScreenLocation.RowInfo, ScreenLocation.ColInfo);
                Gui.Print(attr, smb, ScreenLocation.RowInfo, ScreenLocation.ColInfo + 1, len);
                Gui.Print(Colour.White, mPtr.Race.SplitName1, ScreenLocation.RowInfo - 3, ScreenLocation.ColInfo, 12);
                Gui.Print(Colour.White, mPtr.Race.SplitName2, ScreenLocation.RowInfo - 2, ScreenLocation.ColInfo, 12);
                Gui.Print(Colour.White, mPtr.Race.SplitName3, ScreenLocation.RowInfo - 1, ScreenLocation.ColInfo, 12);
            }
        }

        public bool MartialArtistEmptyHands()
        {
            if (_player.ProfessionIndex != CharacterClass.Monk && _player.ProfessionIndex != CharacterClass.Mystic)
            {
                return false;
            }
            return _player.Inventory[InventorySlot.MeleeWeapon].ItemType == null;
        }

        public bool MartialArtistHeavyArmour()
        {
            int martialArtistArmWgt = 0;
            if (_player.ProfessionIndex != CharacterClass.Monk && _player.ProfessionIndex != CharacterClass.Mystic)
            {
                return false;
            }
            martialArtistArmWgt += _player.Inventory[InventorySlot.Body].Weight;
            martialArtistArmWgt += _player.Inventory[InventorySlot.Head].Weight;
            martialArtistArmWgt += _player.Inventory[InventorySlot.Arm].Weight;
            martialArtistArmWgt += _player.Inventory[InventorySlot.Cloak].Weight;
            martialArtistArmWgt += _player.Inventory[InventorySlot.Hands].Weight;
            martialArtistArmWgt += _player.Inventory[InventorySlot.Feet].Weight;
            return martialArtistArmWgt > 100 + (_player.Level * 4);
        }

        public void PrintVisPoints()
        {
            if (_player.Spellcasting.Type == CastingType.None)
            {
                return;
            }
            Gui.Print("Max VP ", ScreenLocation.RowMaxsp, ScreenLocation.ColMaxsp);
            string tmp = _player.MaxVis.ToString().PadLeft(5);
            Colour colour = Colour.BrightGreen;
            Gui.Print(colour, tmp, ScreenLocation.RowMaxsp, ScreenLocation.ColMaxsp + 7);
            Gui.Print("Cur VP ", ScreenLocation.RowCursp, ScreenLocation.ColCursp);
            tmp = _player.Vis.ToString().PadLeft(5);
            if (_player.Vis >= _player.MaxVis)
            {
                colour = Colour.BrightGreen;
            }
            else if (_player.Vis > _player.MaxVis * GlobalData.HitpointWarn / 5)
            {
                colour = Colour.BrightYellow;
            }
            else if (_player.Vis > _player.MaxVis * GlobalData.HitpointWarn / 10)
            {
                colour = Colour.Orange;
            }
            else
            {
                colour = Colour.BrightRed;
            }
            Gui.Print(colour, tmp, ScreenLocation.RowCursp, ScreenLocation.ColCursp + 7);
        }

        public void PrtAc()
        {
            Gui.Print("Cur AC ", ScreenLocation.RowAc, ScreenLocation.ColAc);
            string tmp = (_player.DisplayedBaseArmourClass + _player.DisplayedArmourClassBonus).ToString().PadLeft(5);
            Gui.Print(Colour.BrightGreen, tmp, ScreenLocation.RowAc, ScreenLocation.ColAc + 7);
        }

        public void PrtAfraid()
        {
            if (_player.TimedFear > 0)
            {
                Gui.Print(Colour.Orange, "Afraid", ScreenLocation.RowAfraid, ScreenLocation.ColAfraid);
            }
            else
            {
                Gui.Print("      ", ScreenLocation.RowAfraid, ScreenLocation.ColAfraid);
            }
        }

        public void PrtBlind()
        {
            if (_player.TimedBlindness > 0)
            {
                Gui.Print(Colour.Orange, "Blind", ScreenLocation.RowBlind, ScreenLocation.ColBlind);
            }
            else
            {
                Gui.Print("     ", ScreenLocation.RowBlind, ScreenLocation.ColBlind);
            }
        }

        public void PrtConfused()
        {
            if (_player.TimedConfusion > 0)
            {
                Gui.Print(Colour.Orange, "Confused", ScreenLocation.RowConfused, ScreenLocation.ColConfused);
            }
            else
            {
                Gui.Print("        ", ScreenLocation.RowConfused, ScreenLocation.ColConfused);
            }
        }

        public void PrtCut()
        {
            int c = _player.TimedBleeding;
            if (c > 1000)
            {
                Gui.Print(Colour.BrightRed, "Mortal wound", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 200)
            {
                Gui.Print(Colour.Red, "Deep gash   ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 100)
            {
                Gui.Print(Colour.Red, "Severe cut  ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 50)
            {
                Gui.Print(Colour.Orange, "Nasty cut   ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 25)
            {
                Gui.Print(Colour.Orange, "Bad cut     ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 10)
            {
                Gui.Print(Colour.Yellow, "Light cut   ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else if (c > 0)
            {
                Gui.Print(Colour.Yellow, "Graze       ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
            else
            {
                Gui.Print("            ", ScreenLocation.RowCut, ScreenLocation.ColCut);
            }
        }

        public void PrtDepth()
        {
            string depths;
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                if (SaveGame.Instance.Wilderness[_player.WildernessY][_player.WildernessX].Dungeon != null)
                {
                    depths = SaveGame.Instance.Wilderness[_player.WildernessY][_player.WildernessX].Dungeon.Shortname;
                    SaveGame.Instance.Wilderness[_player.WildernessY][_player.WildernessX].Dungeon.Visited = true;
                }
                else
                {
                    depths = $"Wild ({_player.WildernessX},{_player.WildernessY})";
                }
            }
            else
            {
                depths = $"lvl {SaveGame.Instance.CurrentDepth}+{SaveGame.Instance.DungeonDifficulty}";
                SaveGame.Instance.CurDungeon.KnownOffset = true;
                if (SaveGame.Instance.CurrentDepth == SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.CurDungeon.KnownDepth = true;
                }
            }
            Gui.PrintLine(depths.PadLeft(9), ScreenLocation.RowDepth, ScreenLocation.ColDepth);
        }

        public void PrtDtrap()
        {
            int count = 0;
            if (_level.Grid[_player.MapY][_player.MapX].TileFlags.IsClear(GridTile.TrapsDetected))
            {
                Gui.Print(Colour.Green, "     ", ScreenLocation.RowDtrap, ScreenLocation.ColDtrap);
                return;
            }
            if (_level.Grid[_player.MapY - 1][_player.MapX].TileFlags.IsSet(GridTile.TrapsDetected))
            {
                count++;
            }
            if (_level.Grid[_player.MapY + 1][_player.MapX].TileFlags.IsSet(GridTile.TrapsDetected))
            {
                count++;
            }
            if (_level.Grid[_player.MapY][_player.MapX - 1].TileFlags.IsSet(GridTile.TrapsDetected))
            {
                count++;
            }
            if (_level.Grid[_player.MapY][_player.MapX + 1].TileFlags.IsSet(GridTile.TrapsDetected))
            {
                count++;
            }
            if (count == 4)
            {
                Gui.Print(Colour.Green, "DTrap", ScreenLocation.RowDtrap, ScreenLocation.ColDtrap);
            }
            else
            {
                Gui.Print(Colour.Yellow, "DTRAP", ScreenLocation.RowDtrap, ScreenLocation.ColDtrap);
            }
        }

        public void PrtExp()
        {
            Colour colour = Colour.BrightGreen;
            if (_player.ExperiencePoints < _player.MaxExperienceGained)
            {
                colour = Colour.Yellow;
            }
            Gui.Print("NEXT", ScreenLocation.RowExp, 0);
            if (_player.Level >= Constants.PyMaxLevel)
            {
                Gui.Print(Colour.BrightGreen, "   *****", ScreenLocation.RowExp, ScreenLocation.ColExp + 4);
            }
            else
            {
                string outVal = ((GlobalData.PlayerExp[_player.Level - 1] * _player.ExperienceMultiplier / 100) - _player.ExperiencePoints).ToString()
                    .PadLeft(8);
                Gui.Print(colour, outVal, ScreenLocation.RowExp, ScreenLocation.ColExp + 4);
            }
        }

        public void PrtField(string info, int row, int col)
        {
            Gui.Print(Colour.White, "             ", row, col);
            Gui.Print(Colour.BrightBlue, info, row, col);
        }

        public void PrtFrameBasic()
        {
            PrtField(_player.Name, ScreenLocation.RowName, ScreenLocation.ColName);
            PrtField(_player.Race.Title, ScreenLocation.RowRace, ScreenLocation.ColRace);
            PrtField(Profession.ClassSubName(_player.ProfessionIndex, _player.Realm1), ScreenLocation.RowClass,
                ScreenLocation.ColClass);
            PrtTitle();
            PrtLevel();
            PrtExp();
            for (int i = 0; i < 6; i++)
            {
                PrtStat(i);
            }
            PrtAc();
            PrtHp();
            PrintVisPoints();
            PrtGold();
            PrtDepth();
            HealthRedraw();
        }

        public void PrtFrameExtra()
        {
            PrtCut();
            PrtStun();
            PrtHunger();
            PrtDtrap();
            PrtBlind();
            PrtConfused();
            PrtAfraid();
            PrtPoisoned();
            PrtState();
            PrtSpeed();
            PrtStudy();
        }

        public void PrtGold()
        {
            Gui.Print("GP ", ScreenLocation.RowGold, ScreenLocation.ColGold);
            string tmp = _player.Gold.ToString().PadLeft(9);
            Gui.Print(Colour.BrightGreen, tmp, ScreenLocation.RowGold, ScreenLocation.ColGold + 3);
        }

        public void PrtHp()
        {
            Gui.Print("Max HP ", ScreenLocation.RowMaxhp, ScreenLocation.ColMaxhp);
            string tmp = _player.MaxHealth.ToString().PadLeft(5);
            Colour colour = Colour.BrightGreen;
            Gui.Print(colour, tmp, ScreenLocation.RowMaxhp, ScreenLocation.ColMaxhp + 7);
            Gui.Print("Cur HP ", ScreenLocation.RowCurhp, ScreenLocation.ColCurhp);
            tmp = _player.Health.ToString().PadLeft(5);
            if (_player.Health >= _player.MaxHealth)
            {
                colour = Colour.BrightGreen;
            }
            else if (_player.Health > _player.MaxHealth * GlobalData.HitpointWarn / 5)
            {
                colour = Colour.BrightYellow;
            }
            else if (_player.Health > _player.MaxHealth * GlobalData.HitpointWarn / 10)
            {
                colour = Colour.Orange;
            }
            else
            {
                colour = Colour.BrightRed;
            }
            Gui.Print(colour, tmp, ScreenLocation.RowCurhp, ScreenLocation.ColCurhp + 7);
        }

        public void PrtHunger()
        {
            if (_player.Food < Constants.PyFoodFaint)
            {
                Gui.Print(Colour.Red, "Weak  ", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
            else if (_player.Food < Constants.PyFoodWeak)
            {
                Gui.Print(Colour.Orange, "Weak  ", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
            else if (_player.Food < Constants.PyFoodAlert)
            {
                Gui.Print(Colour.Yellow, "Hungry", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
            else if (_player.Food < Constants.PyFoodFull)
            {
                Gui.Print(Colour.BrightGreen, "      ", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
            else if (_player.Food < Constants.PyFoodMax)
            {
                Gui.Print(Colour.BrightGreen, "Full  ", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
            else
            {
                Gui.Print(Colour.Green, "Gorged", ScreenLocation.RowHungry, ScreenLocation.ColHungry);
            }
        }

        public void PrtLevel()
        {
            string tmp = _player.Level.ToString().PadLeft(6);
            if (_player.Level >= _player.MaxLevelGained)
            {
                Gui.Print("LEVEL ", ScreenLocation.RowLevel, 0);
                Gui.Print(Colour.BrightGreen, tmp, ScreenLocation.RowLevel, ScreenLocation.ColLevel + 6);
            }
            else
            {
                Gui.Print("Level ", ScreenLocation.RowLevel, 0);
                Gui.Print(Colour.Yellow, tmp, ScreenLocation.RowLevel, ScreenLocation.ColLevel + 6);
            }
        }

        public void PrtPoisoned()
        {
            if (_player.TimedPoison > 0)
            {
                Gui.Print(Colour.Orange, "Poisoned", ScreenLocation.RowPoisoned, ScreenLocation.ColPoisoned);
            }
            else
            {
                Gui.Print("        ", ScreenLocation.RowPoisoned, ScreenLocation.ColPoisoned);
            }
        }

        public void PrtSpeed()
        {
            int i = _player.Speed;
            Colour attr = Colour.White;
            string buf = "";
            if (_player.IsSearching)
            {
                i += 10;
            }
            int energy = GlobalData.ExtractEnergy[i];
            if (i > 110)
            {
                attr = Colour.BrightGreen;
                buf = $"Fast {energy / 10.0}";
            }
            else if (i < 110)
            {
                attr = Colour.BrightBrown;
                buf = $"Slow {energy / 10.0}";
            }
            Gui.Print(attr, buf.PadRight(14), ScreenLocation.RowSpeed, ScreenLocation.ColSpeed);
        }

        public void PrtStat(int stat)
        {
            string tmp;
            if (_player.AbilityScores[stat].Innate < _player.AbilityScores[stat].InnateMax)
            {
                Gui.Print(GlobalData.StatNamesReduced[stat], ScreenLocation.RowStat + stat, 0);
                tmp = _player.AbilityScores[stat].Adjusted.StatToString();
                Gui.Print(Colour.Yellow, tmp, ScreenLocation.RowStat + stat, ScreenLocation.ColStat + 6);
            }
            else
            {
                Gui.Print(GlobalData.StatNames[stat], ScreenLocation.RowStat + stat, 0);
                tmp = _player.AbilityScores[stat].Adjusted.StatToString();
                Gui.Print(Colour.BrightGreen, tmp, ScreenLocation.RowStat + stat, ScreenLocation.ColStat + 6);
            }
            if (_player.AbilityScores[stat].InnateMax == 18 + 100)
            {
                Gui.Print("!", ScreenLocation.RowStat + stat, 3);
            }
        }

        public void PrtState()
        {
            Colour attr = Colour.White;
            string text;
            if (_player.TimedParalysis > 0)
            {
                attr = Colour.Red;
                text = "Paralyzed!";
            }
            else if (SaveGame.Instance.Resting != 0)
            {
                text = "Rest ";
                if (SaveGame.Instance.Resting > 0)
                {
                    text += SaveGame.Instance.Resting.ToString().PadLeft(5);
                }
                else if (SaveGame.Instance.Resting == -1)
                {
                    text += "*****";
                }
                else if (SaveGame.Instance.Resting == -2)
                {
                    text += "&&&&&";
                }
                else
                {
                    text += "?????";
                }
            }
            else if (SaveGame.Instance.Command.CommandRepeat != 0)
            {
                if (SaveGame.Instance.Command.CommandRepeat > 999)
                {
                    text = "Rep. " + SaveGame.Instance.Command.CommandRepeat.ToString().PadRight(5);
                }
                else
                {
                    text = "Repeat " + SaveGame.Instance.Command.CommandRepeat.ToString().PadRight(3);
                }
            }
            else if (_player.IsSearching)
            {
                text = "Searching ";
            }
            else
            {
                text = "          ";
            }
            Gui.Print(attr, text, ScreenLocation.RowState, ScreenLocation.ColState);
        }

        public void PrtStudy()
        {
            Gui.Print(_player.SpareSpellSlots != 0 ? "Study" : "     ", ScreenLocation.RowStudy, ScreenLocation.ColStudy);
        }

        public void PrtStun()
        {
            int s = _player.TimedStun;
            if (s > 100)
            {
                Gui.Print(Colour.Red, "Knocked out ", ScreenLocation.RowStun, ScreenLocation.ColStun);
            }
            else if (s > 50)
            {
                Gui.Print(Colour.Orange, "Heavy stun  ", ScreenLocation.RowStun, ScreenLocation.ColStun);
            }
            else if (s > 0)
            {
                Gui.Print(Colour.Orange, "Stun        ", ScreenLocation.RowStun, ScreenLocation.ColStun);
            }
            else
            {
                Gui.Print("            ", ScreenLocation.RowStun, ScreenLocation.ColStun);
            }
        }

        public void PrtTime()
        {
            Gui.Print(Colour.White, "Time", ScreenLocation.RowTime, ScreenLocation.ColTime);
            Gui.Print(Colour.White, "Day", ScreenLocation.RowDate, ScreenLocation.colDate);
            Gui.Print(Colour.BrightGreen, _player.GameTime.TimeText.PadLeft(8), ScreenLocation.RowTime, ScreenLocation.ColTime + 4);
            Gui.Print(Colour.BrightGreen, _player.GameTime.DateText.PadLeft(8), ScreenLocation.RowDate, ScreenLocation.colDate + 4);
        }

        public void PrtTitle()
        {
            string p;
            if (_player.IsWizard)
            {
                p = "-=<WIZARD>=-";
                PrtField(p, ScreenLocation.RowTitle, ScreenLocation.ColTitle);
            }
            else if (_player.IsWinner || _player.Level > Constants.PyMaxLevel)
            {
                p = "***WINNER***";
                PrtField(p, ScreenLocation.RowTitle, ScreenLocation.ColTitle);
            }
        }

        private int WeightLimit()
        {
            int i = _player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100;
            return i;
        }
    }
}