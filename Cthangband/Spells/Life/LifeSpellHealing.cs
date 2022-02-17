﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHealing : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(300);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Healing";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 55;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 20;
                    ManaCost = 16;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Paladin:
                    Level = 30;
                    ManaCost = 25;
                    BaseFailure = 55;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 55;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 45;
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
            return "heal 300";
        }
    }
}