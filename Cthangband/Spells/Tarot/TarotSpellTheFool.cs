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
    internal class TarotSpellTheFool : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 15;

        public override int DefaultVisCost => 15;

        public override int FirstCastExperience => 8;

        public override string Name => "The Fool";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var dummy = 0;
            Profile.Instance.MsgPrint("You concentrate on the Fool card...");
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    dummy = Constants.SummonBizarre1;
                    break;

                case 2:
                    dummy = Constants.SummonBizarre2;
                    break;

                case 3:
                    dummy = Constants.SummonBizarre4;
                    break;

                case 4:
                    dummy = Constants.SummonBizarre5;
                    break;
            }
            if (Program.Rng.DieRoll(2) == 1)
            {
                Profile.Instance.MsgPrint(level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, dummy)
                    ? "The summoned creature gets angry!"
                    : "No-one ever turns up.");
            }
            else
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, dummy, false))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
        }

        protected override string Comment(Player player)
        {
            return "control 50%";
        }
    }
}