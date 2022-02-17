using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHolyWord : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelEvil(player.Level * 4);
            player.RestoreHealth(1000);
            player.SetTimedFear(0);
            player.SetTimedPoison(0);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Holy Word";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 39;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Priest:
                    Level = 39;
                    ManaCost = 32;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Paladin:
                    Level = 39;
                    ManaCost = 38;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.HighMage:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
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
            return $"d {4 * player.Level}/h 1000";
        }
    }
}