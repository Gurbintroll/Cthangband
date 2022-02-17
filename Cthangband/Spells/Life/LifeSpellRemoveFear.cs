﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellRemoveFear : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Remove Fear";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Paladin:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
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