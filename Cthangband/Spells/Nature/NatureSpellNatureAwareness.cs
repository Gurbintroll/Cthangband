using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellNatureAwareness : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.MapArea();
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
            saveGame.SpellEffects.DetectMonstersNormal();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Nature Awareness";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 11;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 6;
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