using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFlashOfLight : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Flash of Light";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 26;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    ManaCost = 3;
                    BaseFailure = 35;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Fanatic:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 2;
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
            return $"dam 2d{player.Level / 2}";
        }
    }
}