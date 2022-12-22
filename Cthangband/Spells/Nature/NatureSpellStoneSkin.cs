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
    internal class NatureSpellStoneSkin : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedStoneskin(player.TimedStoneskin + Program.Rng.DieRoll(20) + 30);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stone Skin";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 10;
                    VisCost = 12;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClassId.Priest:
                    Level = 12;
                    VisCost = 13;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClassId.Ranger:
                    Level = 14;
                    VisCost = 15;
                    BaseFailure = 70;
                    FirstCastExperience = 60;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 15;
                    VisCost = 15;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 65;
                    FirstCastExperience = 120;
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
            return "dur 20+d30";
        }
    }
}