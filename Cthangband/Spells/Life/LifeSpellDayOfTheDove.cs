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

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDayOfTheDove : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.CharmMonsters(player.Level * 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Day of the Dove";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 35;
                    VisCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClassId.Priest:
                    Level = 24;
                    VisCost = 20;
                    BaseFailure = 55;
                    FirstCastExperience = 70;
                    break;

                case CharacterClassId.Paladin:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 36;
                    VisCost = 36;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClassId.HighMage:
                    Level = 31;
                    VisCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
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