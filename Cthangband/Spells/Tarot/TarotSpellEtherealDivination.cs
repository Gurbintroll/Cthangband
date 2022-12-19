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
    internal class TarotSpellEtherealDivination : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectAll();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Ethereal Divination";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 32;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 35;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    VisCost = 33;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
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
            return string.Empty;
        }
    }
}