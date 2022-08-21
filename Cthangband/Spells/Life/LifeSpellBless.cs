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
    internal class LifeSpellBless : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(12) + 12);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Bless";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 1;
                    ManaCost = 2;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
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
            return "dur 12+d12 turns";
        }
    }
}