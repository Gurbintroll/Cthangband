using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTeleportSelf : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(player.Level * 5);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Self";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    ManaCost = 9;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 17;
                    ManaCost = 11;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 18;
                    ManaCost = 17;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Fanatic:
                    Level = 16;
                    ManaCost = 10;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 7;
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