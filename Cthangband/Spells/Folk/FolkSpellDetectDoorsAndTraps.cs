﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetectDoorsAndTraps : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Doors and Traps";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Priest:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 7;
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