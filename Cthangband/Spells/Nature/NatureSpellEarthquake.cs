using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellEarthquake : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Earthquake";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Priest:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 24;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 28;
                    BaseFailure = 60;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
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
            return "rad 10";
        }
    }
}