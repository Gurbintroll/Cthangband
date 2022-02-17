using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellDetectDoorsAndTraps : Spell
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
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 15;
                    FirstCastExperience = 1;
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