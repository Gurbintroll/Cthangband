using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellPolymorphSelf : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.PolymorphSelf();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Polymorph Self";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 45;
                    ManaCost = 55;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 75;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 45;
                    ManaCost = 55;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 42;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 39;
                    ManaCost = 40;
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
            return string.Empty;
        }
    }
}