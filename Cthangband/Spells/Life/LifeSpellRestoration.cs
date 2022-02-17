﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellRestoration : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.TryRestoringAbilityScore(Ability.Strength);
            player.TryRestoringAbilityScore(Ability.Intelligence);
            player.TryRestoringAbilityScore(Ability.Wisdom);
            player.TryRestoringAbilityScore(Ability.Dexterity);
            player.TryRestoringAbilityScore(Ability.Constitution);
            player.TryRestoringAbilityScore(Ability.Charisma);
            player.RestoreLevel();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Restoration";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 100;
                    BaseFailure = 82;
                    FirstCastExperience = 225;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 70;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Paladin:
                    Level = 40;
                    ManaCost = 80;
                    BaseFailure = 80;
                    FirstCastExperience = 225;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 90;
                    BaseFailure = 80;
                    FirstCastExperience = 225;
                    break;

                case CharacterClass.HighMage:
                    Level = 40;
                    ManaCost = 80;
                    BaseFailure = 60;
                    FirstCastExperience = 225;
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