using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellLightArea : Spell
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
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 3;
                    BaseFailure = 65;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 20;
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
            return $"dam {10 + (player.Level / 2)}";
            ;
        }
    }
}