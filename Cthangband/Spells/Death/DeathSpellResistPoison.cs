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
    internal class DeathSpellResistPoison : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Poison";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 7;
                    VisCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Priest:
                    Level = 9;
                    VisCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Rogue:
                    Level = 17;
                    VisCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClassId.Ranger:
                    Level = 17;
                    VisCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Paladin:
                    Level = 10;
                    VisCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 9;
                    VisCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.HighMage:
                    Level = 5;
                    VisCost = 9;
                    BaseFailure = 55;
                    FirstCastExperience = 6;
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
            return "dur 20+d20";
        }
    }
}