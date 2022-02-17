using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellTeleport : Spell
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
                    Level = 18;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 19;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 15;
                    ManaCost = 12;
                    BaseFailure = 40;
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
            return $"range {player.Level * 5}";
        }
    }
}