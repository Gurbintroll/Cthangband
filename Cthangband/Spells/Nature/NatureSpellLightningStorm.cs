﻿using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellLightningStorm : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 90 + player.Level, (player.Level / 12) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Lightning Storm";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Priest:
                    Level = 32;
                    ManaCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClass.Ranger:
                    Level = 32;
                    ManaCost = 29;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 28;
                    ManaCost = 25;
                    BaseFailure = 65;
                    FirstCastExperience = 35;
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
            return $"dam {90 + player.Level}";
        }
    }
}