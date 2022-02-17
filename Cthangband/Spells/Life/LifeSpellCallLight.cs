using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellCallLight : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Call Light";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 27;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.Paladin:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 25;
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
            return $"dam {10 + (player.Level / 2)}";
        }
    }
}