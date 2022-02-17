﻿using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellDisintegrate : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), dir, 80 + player.Level,
                3 + (player.Level / 40));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Disintegrate";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 32;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 27;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Fanatic:
                    Level = 23;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 21;
                    ManaCost = 21;
                    BaseFailure = 75;
                    FirstCastExperience = 100;
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
            return $"dam {80 + player.Level}";
        }
    }
}