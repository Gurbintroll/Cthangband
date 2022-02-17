﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellExorcism : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelUndead(player.Level);
            saveGame.SpellEffects.DispelDemons(player.Level);
            saveGame.SpellEffects.TurnEvil(player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Exorcism";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 26;
                    ManaCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 25;
                    ManaCost = 22;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 40;
                    FirstCastExperience = 75;
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
            return $"dam {player.Level}+{player.Level}";
        }
    }
}