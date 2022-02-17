using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellSelfKnowledge : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.SelfKnowledge();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Self Knowledge";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 85;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 17;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    ManaCost = 19;
                    BaseFailure = 85;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 15;
                    ManaCost = 12;
                    BaseFailure = 65;
                    FirstCastExperience = 50;
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