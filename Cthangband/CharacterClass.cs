// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class CharacterClass
    {
        public static readonly CharacterClass[] ClassInfo =
        {
            new CharacterClass("Warrior", new[] {5, -2, -2, 2, 2, -1}, 25, 18, 18, 1, 14, 2, 70, 60, 12, 7, 10, 0, 0, 0, 45, 45, 9, 0),
            new CharacterClass("Mage", new[] {-5, 3, 0, 1, -2, 1}, 30, 36, 30, 2, 16, 20, 34, 20, 7, 13, 9, 0, 0, 0, 15, 15, 0, 30),
            new CharacterClass("Priest", new[] {-1, -3, 3, -1, 0, 2}, 25, 30, 32, 2, 16, 8, 48, 36, 7, 10, 12, 0, 0, 0, 20, 20, 2, 20),
            new CharacterClass("Rogue", new[] {2, 1, -2, 3, 1, -1}, 45, 32, 28, 5, 32, 24, 60, 66, 15, 10, 10, 0, 0, 0, 40, 10, 6, 25),
            new CharacterClass("Ranger", new[] {2, 2, 0, 1, 1, 1}, 30, 32, 28, 3, 24, 16, 56, 72, 8, 10, 10, 0, 0, 0, 30, 45, 4, 30),
            new CharacterClass("Paladin", new[] {3, -3, 1, 0, 2, 2}, 20, 24, 26, 1, 12, 2, 68, 40, 7, 10, 11, 0, 0, 0, 35, 30, 6, 35),
            new CharacterClass("Warrior-Mage", new[] {2, 2, 0, 1, 0, 1}, 30, 30, 28, 2, 18, 16, 50, 26, 7, 10, 9, 0, 0, 0, 20, 20, 4, 50),
            new CharacterClass("Fanatic", new[] {2, 1, 0, 1, 2, -2}, 20, 24, 30, 1, 14, 12, 66, 40, 7, 11, 10, 0, 0, 0, 35, 30, 6, 35),
            new CharacterClass("Monk", new[] {2, -1, 1, 3, 2, 1}, 45, 32, 28, 5, 32, 24, 64, 50, 15, 12, 10, 0, 0, 0, 40, 30, 6, 40),
            new CharacterClass("Mindcrafter", new[] {-1, 0, 3, -1, -1, 2}, 30, 30, 30, 3, 22, 16, 50, 40, 10, 10, 10, 0, 0, 0, 20, 30, 2, 25),
            new CharacterClass("High-Mage", new[] {-5, 4, 0, 0, -2, 1}, 30, 38, 30, 2, 16, 20, 34, 20, 7, 13, 9, 0, 0, 0, 15, 15, 0, 30),
            new CharacterClass("Druid", new[] {-1, -3, 4, -2, 0, 3}, 30, 30, 32, 3, 20, 8, 48, 36, 8, 10, 12, 0, 0, 0, 20, 20, 3, 20),
            new CharacterClass("Cultist", new[] {-5, 4, 0, 1, -2, -2}, 30, 36, 32, 1, 16, 18, 30, 20, 7, 13, 10, 0, 0, 0, 15, 15, 0, 30),
            new CharacterClass("Channeler", new[] {-1, 0, 2, -1, -1, 3}, 40, 40, 30, 2, 16, 20, 40, 30, 12, 13, 9, 0, 0, 0, 15, 15, 1, 30),
            new CharacterClass("Chosen One", new[] {3, -2, -2, 2, 2, -1}, 25, 18, 20, 1, 16, 4, 50, 32, 12, 7, 10, 0, 0, 0, 20, 20, 4, 20),
            new CharacterClass("Mystic", new[] {2, -1, 2, 2, 2, 0}, 40, 30, 30, 5, 32, 24, 64, 50, 14, 11, 11, 0, 0, 0, 40, 30, 6, 40)
        };

        public readonly int[] AbilityBonus = new int[6];

        public readonly int BaseDeviceBonus;

        public readonly int BaseDisarmBonus;

        public readonly int BaseMeleeAttackBonus;

        public readonly int BaseRangedAttackBonus;

        public readonly int BaseSaveBonus;

        public readonly int BaseSearchBonus;

        public readonly int BaseSearchFrequency;

        public readonly int BaseStealthBonus;

        public readonly int DeviceBonusPerLevel;

        public readonly int DisarmBonusPerLevel;

        public readonly int ExperienceFactor;

        public readonly int HitDieBonus;

        public readonly int MeleeAttackBonusPerLevel;

        public readonly int RangedAttackBonusPerLevel;

        public readonly int SaveBonusPerLevel;

        public readonly int SearchBonusPerLevel;

        public readonly int SearchFrequencyPerLevel;

        public readonly int StealthBonusPerLevel;

        public readonly string Title;

        public CharacterClass()
        {
        }

        private CharacterClass(string title, int[] cAdj, int cDis, int cDev, int cSav, int cStl, int cSrh, int cFos,
            int cThn, int cThb, int xDis, int xDev, int xSav, int xStl, int xSrh, int xFos, int xThn, int xThb,
            int cMhp, int cExp)
        {
            Title = title;
            AbilityBonus = cAdj;
            BaseDisarmBonus = cDis;
            BaseDeviceBonus = cDev;
            BaseSaveBonus = cSav;
            BaseStealthBonus = cStl;
            BaseSearchBonus = cSrh;
            BaseSearchFrequency = cFos;
            BaseMeleeAttackBonus = cThn;
            BaseRangedAttackBonus = cThb;
            DisarmBonusPerLevel = xDis;
            DeviceBonusPerLevel = xDev;
            SaveBonusPerLevel = xSav;
            StealthBonusPerLevel = xStl;
            SearchBonusPerLevel = xSrh;
            SearchFrequencyPerLevel = xFos;
            MeleeAttackBonusPerLevel = xThn;
            RangedAttackBonusPerLevel = xThb;
            HitDieBonus = cMhp;
            ExperienceFactor = cExp;
        }

        public static string ClassSubName(int pclass, Realm realm)
        {
            switch (pclass)
            {
                case CharacterClassId.Channeler:
                    return "Channeler";

                case CharacterClassId.ChosenOne:
                    return "Chosen One";

                case CharacterClassId.Cultist:
                    return "Cultist";

                case CharacterClassId.Druid:
                    return "Druid";

                case CharacterClassId.Fanatic:
                    return "Fanatic";

                case CharacterClassId.HighMage:
                    switch (realm)
                    {
                        case Realm.Life:
                            return "Vivimancer";

                        case Realm.Sorcery:
                            return "Sorcerer";

                        case Realm.Nature:
                            return "Naturist";

                        case Realm.Chaos:
                            return "Warlock";

                        case Realm.Death:
                            return "Necromancer";

                        case Realm.Tarot:
                            return "Summoner";

                        case Realm.Folk:
                            return "Hedge Wizard";

                        case Realm.Corporeal:
                            return "Zen Master";

                        default:
                            return "High Mage";
                    }
                case CharacterClassId.Mage:
                    return "Mage";

                case CharacterClassId.Monk:
                    switch (realm)
                    {
                        case Realm.Corporeal:
                            return "Ascetic";

                        case Realm.Tarot:
                            return "Ninja";

                        case Realm.Chaos:
                            return "Street Fighter";

                        default:
                            return "Monk";
                    }
                case CharacterClassId.Mindcrafter:
                    return "Mindcrafter";

                case CharacterClassId.Mystic:
                    return "Mystic";

                case CharacterClassId.Paladin:
                    return realm == Realm.Death ? "Death Knight" : "Paladin";

                case CharacterClassId.Priest:
                    return realm == Realm.Death ? "Exorcist" : "Priest";

                case CharacterClassId.Ranger:
                    return "Ranger";

                case CharacterClassId.Rogue:
                    switch (realm)
                    {
                        case Realm.Sorcery:
                            return "Burglar";

                        case Realm.Death:
                            return "Assassin";

                        case Realm.Tarot:
                            return "Card Sharp";

                        default:
                            return "Thief";
                    }
                case CharacterClassId.Warrior:
                    return "Warrior";

                case CharacterClassId.WarriorMage:
                    return "Warrior Mage";

                default:
                    return "Unknown Class!";
            }
        }

        public static int PrimeStat(int pclass)
        {
            switch (pclass)
            {
                case CharacterClassId.Warrior:
                case CharacterClassId.ChosenOne:
                    return Ability.Strength;

                case CharacterClassId.Mage:
                case CharacterClassId.Ranger:
                case CharacterClassId.WarriorMage:
                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                case CharacterClassId.Fanatic:
                    return Ability.Intelligence;

                case CharacterClassId.Priest:
                case CharacterClassId.Paladin:
                case CharacterClassId.Mindcrafter:
                case CharacterClassId.Mystic:
                case CharacterClassId.Druid:
                    return Ability.Wisdom;

                case CharacterClassId.Rogue:
                case CharacterClassId.Monk:
                    return Ability.Dexterity;

                case CharacterClassId.Channeler:
                    return Ability.Charisma;

                default:
                    return Ability.Constitution;
            }
        }
    }
}