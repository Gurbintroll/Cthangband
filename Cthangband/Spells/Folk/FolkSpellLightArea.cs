using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellLightArea : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Light Area";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
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