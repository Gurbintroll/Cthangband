﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellResistTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Priest:
                    Level = 36;
                    ManaCost = 33;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                    Level = 28;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 20;
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