﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellHorrify : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FearMonster(dir, player.Level);
            saveGame.SpellEffects.StunMonster(dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Horrify";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 19;
                    ManaCost = 17;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 19;
                    ManaCost = 19;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 20;
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