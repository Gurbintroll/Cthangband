using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellPhaseDoor : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Phase Door";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Rogue:
                    Level = 5;
                    ManaCost = 2;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Ranger:
                    Level = 3;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
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