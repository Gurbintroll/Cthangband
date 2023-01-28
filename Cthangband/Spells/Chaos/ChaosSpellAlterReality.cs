// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
        public override int DefaultBaseFailure => 85;

        public override int DefaultLevel => 32;

        public override int DefaultVisCost => 25;

        public override int FirstCastExperience => 150;

        public override string Name => "Alter Reality";

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

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}