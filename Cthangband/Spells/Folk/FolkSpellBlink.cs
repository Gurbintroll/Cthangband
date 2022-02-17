using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellBlink : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Blink";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 6;
                    ManaCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 5;
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
            return "range 10";
        }
    }
}