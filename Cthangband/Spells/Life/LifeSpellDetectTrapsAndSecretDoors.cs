using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDetectTrapsAndSecretDoors : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Traps and Secret Doors";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 28;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.Paladin:
                    Level = 8;
                    ManaCost = 5;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 30;
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