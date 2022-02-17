﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDispelGood : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelGood(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Dispel Good";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.Rogue:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 60;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Ranger:
                    Level = 45;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Paladin:
                    Level = 40;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 16;
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
            return $"dam {4 * player.Level}";
        }
    }
}