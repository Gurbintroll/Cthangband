using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellCallTheVoid : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.CallTheVoid();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Call the Void";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 49;
                    ManaCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 50;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 50;
                    ManaCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 49;
                    ManaCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 90;
                    BaseFailure = 75;
                    FirstCastExperience = 250;
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
            return "dam 3 * 175";
        }
    }
}