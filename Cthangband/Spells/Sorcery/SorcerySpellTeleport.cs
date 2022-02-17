using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellTeleport : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(player.Level * 5);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 15;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 25;
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
            return $"range {player.Level * 5}";
        }
    }
}