using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTrapAndDoorDestruction : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DestroyDoorsTouch();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Trap and Door Destruction";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 2;
                    BaseFailure = 22;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 24;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 35;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 22;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Fanatic:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 22;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 15;
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