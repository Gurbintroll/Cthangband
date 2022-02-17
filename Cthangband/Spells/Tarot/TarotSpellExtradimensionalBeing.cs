using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellExtradimensionalBeing : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You have turned into a Extradimensional Being.");
            player.Dna.GainMutation();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Extradimensional Being";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 41;
                    ManaCost = 110;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Rogue:
                    Level = 45;
                    ManaCost = 150;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 44;
                    ManaCost = 120;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 120;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                    Level = 36;
                    ManaCost = 90;
                    BaseFailure = 80;
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