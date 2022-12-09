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

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellAstralSpying : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Astral Spying";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 14;
                    VrilCost = 12;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 17;
                    VrilCost = 14;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 19;
                    VrilCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    VrilCost = 17;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 18;
                    VrilCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 10;
                    VrilCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
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
            return "dur 25+d30";
        }
    }
}