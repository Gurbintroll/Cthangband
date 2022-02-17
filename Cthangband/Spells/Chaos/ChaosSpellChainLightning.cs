using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellChainLightning : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            for (int dir = 0; dir <= 9; dir++)
            {
                saveGame.SpellEffects.FireBeam(new ProjectElec(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(5 + (player.Level / 10), 8));
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Chain Lightning";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Priest:
                    Level = 17;
                    ManaCost = 17;
                    BaseFailure = 70;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 17;
                    ManaCost = 16;
                    BaseFailure = 60;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Fanatic:
                    Level = 14;
                    ManaCost = 14;
                    BaseFailure = 60;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
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
            return $"dam {5 + (player.Level / 10)}d8";
        }
    }
}