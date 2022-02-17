using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellTelekinesis : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.CommandEngine.Fetch(dir, player.Level * 15, false);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Telekinesis";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Rogue:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    ManaCost = 19;
                    BaseFailure = 75;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 70;
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
            return $"max wgt {player.Level * 15 / 10}";
        }
    }
}