using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellAlterReality : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("The world changes!");
            {
                saveGame.IsAutosave = true;
                saveGame.DoCmdSaveGame();
                saveGame.IsAutosave = false;
            }
            saveGame.NewLevelFlag = true;
            saveGame.CameFrom = LevelStart.StartRandom;
        }

        public override void Initialise(int characterClass)
        {
            Name = "Alter Reality";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Ranger:
                    Level = 38;
                    ManaCost = 35;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Fanatic:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 26;
                    ManaCost = 22;
                    BaseFailure = 75;
                    FirstCastExperience = 150;
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