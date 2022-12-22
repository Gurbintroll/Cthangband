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

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellHaste : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + player.Level) + player.Level);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Haste";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 22;
                    VisCost = 12;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClassId.Priest:
                    Level = 27;
                    VisCost = 17;
                    BaseFailure = 65;
                    FirstCastExperience = 10;
                    break;

                case CharacterClassId.Ranger:
                    Level = 34;
                    VisCost = 35;
                    BaseFailure = 70;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 27;
                    VisCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClassId.HighMage:
                    Level = 17;
                    VisCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
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
            return $"dur {player.Level}+d{20 + player.Level}";
        }
    }
}