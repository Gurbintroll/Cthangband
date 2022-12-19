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

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellYellowSign : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.YellowSign();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Yellow Sign";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    VisCost = 39;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClass.Rogue:
                    Level = 35;
                    VisCost = 40;
                    BaseFailure = 95;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    VisCost = 35;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    VisCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 160;
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
            return $"dam 7d7+{player.Level / 2}";
        }
    }
}