using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellBurnResistance : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Burn Resistance";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 13;
                    ManaCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 19;
                    BaseFailure = 85;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 9;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
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
            return "dur 20+d20";
        }
    }
}