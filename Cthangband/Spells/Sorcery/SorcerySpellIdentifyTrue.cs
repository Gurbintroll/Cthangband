using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellIdentifyTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Identify True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Rogue:
                    Level = 40;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                    Level = 28;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 20;
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