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
    internal class LifeSpellCureMediumWounds : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 10));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Medium Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 12;
                    VrilCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    VrilCost = 4;
                    BaseFailure = 32;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 11;
                    VrilCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    VrilCost = 14;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    VrilCost = 9;
                    BaseFailure = 30;
                    FirstCastExperience = 3;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "heal 4d10";
        }
    }
}