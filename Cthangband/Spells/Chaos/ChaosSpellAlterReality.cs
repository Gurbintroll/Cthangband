// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellAlterReality : BaseSpell
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
                    VisCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    VisCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Ranger:
                    Level = 38;
                    VisCost = 35;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 35;
                    VisCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Fanatic:
                    Level = 30;
                    VisCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 26;
                    VisCost = 22;
                    BaseFailure = 75;
                    FirstCastExperience = 150;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
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