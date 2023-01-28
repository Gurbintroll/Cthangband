// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using Cthangband.UI;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellResetRecall : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 6;

        public override int DefaultVisCost => 6;

        public override int FirstCastExperience => 8;

        public override string Name => "Reset Recall";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var ppp = $"Reset to which level (1-{player.MaxDlv[SaveGame.Instance.CurDungeon.Index]}): ";
            var def = $"{Math.Max(SaveGame.Instance.CurrentDepth, 1)}";
            if (!Gui.GetString(ppp, out var tmpVal, def, 10))
            {
                return;
            }
            if (!int.TryParse(tmpVal, out var dummy))
            {
                dummy = 1;
            }
            if (dummy < 1)
            {
                dummy = 1;
            }
            if (dummy > player.MaxDlv[SaveGame.Instance.CurDungeon.Index])
            {
                dummy = player.MaxDlv[SaveGame.Instance.CurDungeon.Index];
            }
            Profile.Instance.MsgPrint($"Recall depth set to level {dummy}.");
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}