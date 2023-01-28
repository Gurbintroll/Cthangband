// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellPhantasmalServant : BaseSpell
    {
        public override int DefaultBaseFailure => 60;

        public override int DefaultLevel => 28;

        public override int DefaultVisCost => 24;

        public override int FirstCastExperience => 8;

        public override string Name => "Phantasmal Servant";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint(
                level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2, Constants.SummonPhantom,
                    false)
                    ? "'Your wish, master?'"
                    : "No-one ever turns up.");
        }

        protected override string Comment(Player player)
        {
            return "control 100%";
        }
    }
}