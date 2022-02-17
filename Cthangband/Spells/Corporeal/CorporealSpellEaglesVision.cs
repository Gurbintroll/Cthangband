using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellEaglesVision : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Eagle's Vision";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                    Level = 3;
                    ManaCost = 3;
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
            return string.Empty;
        }
    }
}