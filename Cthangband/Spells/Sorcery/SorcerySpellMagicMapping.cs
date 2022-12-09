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
    internal class SorcerySpellMagicMapping : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.MapArea();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Magic Mapping";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    VrilCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    VrilCost = 14;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    VrilCost = 9;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    VrilCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
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
            return string.Empty;
        }
    }
}