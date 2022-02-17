using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellTeleport : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 13;
                    ManaCost = 11;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 13;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
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
            return $"range {player.Level * 4}";
        }
    }
}