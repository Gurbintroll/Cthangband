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
    internal class CorporealSpellBravery : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Bravery";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    VrilCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 4;
                    VrilCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    VrilCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 3;
                    VrilCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    VrilCost = 2;
                    BaseFailure = 15;
                    FirstCastExperience = 1;
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