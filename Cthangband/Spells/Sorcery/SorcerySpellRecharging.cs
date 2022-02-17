using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellRecharging : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Recharge(player.Level * 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Recharging";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Rogue:
                    Level = 21;
                    ManaCost = 12;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 75;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 9;
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