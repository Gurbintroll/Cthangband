﻿using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellSatisfyHunger : Spell
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
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 17;
                    ManaCost = 17;
                    BaseFailure = 90;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 9;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
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