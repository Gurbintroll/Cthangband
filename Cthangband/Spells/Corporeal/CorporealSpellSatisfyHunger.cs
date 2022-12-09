﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellSatisfyHunger : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetFood(Constants.PyFoodMax - 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Satisfy Hunger";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    VrilCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    VrilCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 17;
                    VrilCost = 17;
                    BaseFailure = 90;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 8;
                    VrilCost = 8;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    VrilCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 9;
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