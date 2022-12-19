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

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureMediumWounds : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 50);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Medium Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 16;
                    VisCost = 14;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 18;
                    VisCost = 17;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 20;
                    VisCost = 19;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    VisCost = 19;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    VisCost = 18;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    VisCost = 11;
                    BaseFailure = 22;
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
            return "heal 4d8";
        }
    }
}