using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellBanish : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.BanishMonsters(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Banish";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 40;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Rogue:
                    Level = 49;
                    ManaCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 46;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.HighMage:
                    Level = 39;
                    ManaCost = 36;
                    BaseFailure = 60;
                    FirstCastExperience = 12;
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