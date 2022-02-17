﻿using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFlameStrike : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), 0, 150 + (2 * player.Level), 8);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Flame Strike";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    ManaCost = 34;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 39;
                    ManaCost = 37;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 42;
                    BaseFailure = 75;
                    FirstCastExperience = 42;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 42;
                    ManaCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Fanatic:
                    Level = 37;
                    ManaCost = 37;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 34;
                    ManaCost = 32;
                    BaseFailure = 65;
                    FirstCastExperience = 40;
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
            return $"dam {150 + (player.Level * 2)}";
        }
    }
}