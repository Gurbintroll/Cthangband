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
    internal class SorcerySpellIdentifyTrue : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Identify True";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.Rogue:
                    Level = 40;
                    VisCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 40;
                    VisCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.HighMage:
                    Level = 28;
                    VisCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 20;
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