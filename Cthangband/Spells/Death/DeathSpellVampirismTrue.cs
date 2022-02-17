﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellVampirismTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            for (int dummy = 0; dummy < 3; dummy++)
            {
                if (saveGame.SpellEffects.DrainLife(dir, 100))
                {
                    player.RestoreHealth(100);
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vampirism True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Rogue:
                    Level = 46;
                    ManaCost = 45;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Paladin:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 125;
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
            return "dam 3*100";
        }
    }
}