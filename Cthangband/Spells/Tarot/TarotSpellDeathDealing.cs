using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellDeathDealing : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelLiving(player.Level * 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Death Dealing";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 50;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 45;
                    ManaCost = 55;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Rogue:
                    Level = 48;
                    ManaCost = 75;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 55;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 39;
                    ManaCost = 45;
                    BaseFailure = 40;
                    FirstCastExperience = 75;
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
            return $"dam {player.Level * 3}";
        }
    }
}