﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellPoisonBranding : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.BrandWeapon(2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Poison Branding";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 75;
                    BaseFailure = 50;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Priest:
                    Level = 33;
                    ManaCost = 75;
                    BaseFailure = 90;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Rogue:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    ManaCost = 80;
                    BaseFailure = 95;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Paladin:
                    Level = 35;
                    ManaCost = 75;
                    BaseFailure = 90;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 35;
                    ManaCost = 75;
                    BaseFailure = 50;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.HighMage:
                    Level = 26;
                    ManaCost = 65;
                    BaseFailure = 40;
                    FirstCastExperience = 30;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}