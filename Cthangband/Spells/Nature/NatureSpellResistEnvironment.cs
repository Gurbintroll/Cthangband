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

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellResistEnvironment : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Environment";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 5;
                    VisCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 7;
                    VisCost = 7;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 8;
                    VisCost = 7;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 4;
                    VisCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
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