using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellEntangle : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.SlowMonsters();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Entangle";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 19;
                    ManaCost = 12;
                    BaseFailure = 55;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 67;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Ranger:
                    Level = 18;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    ManaCost = 15;
                    BaseFailure = 65;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 14;
                    ManaCost = 10;
                    BaseFailure = 65;
                    FirstCastExperience = 7;
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