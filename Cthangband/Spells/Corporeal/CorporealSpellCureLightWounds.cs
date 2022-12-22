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
    internal class CorporealSpellCureLightWounds : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 10));
            player.SetTimedBleeding(player.TimedBleeding - 10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Light Wounds";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Priest:
                    Level = 2;
                    VisCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Ranger:
                    Level = 3;
                    VisCost = 2;
                    BaseFailure = 35;
                    FirstCastExperience = 2;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.HighMage:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 15;
                    FirstCastExperience = 4;
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
            return "heal 2d10";
        }
    }
}