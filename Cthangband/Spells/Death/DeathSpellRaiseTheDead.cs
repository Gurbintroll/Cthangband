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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellRaiseTheDead : BaseSpell
    {
        public override int DefaultBaseFailure => 75;

        public override int DefaultLevel => 21;

        public override int DefaultVisCost => 20;

        public override int FirstCastExperience => 50;

        public override string Name => "Raise the Dead";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3) == 1)
            {
                if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level * 3 / 2,
                    player.Level > 47 ? Constants.SummonHiUndead : Constants.SummonUndead))
                {
                    Profile.Instance.MsgPrint(
                        "Cold winds begin to swirl around you, carrying with them the stench of decay...");
                    Profile.Instance.MsgPrint("'The dead arise... to punish you for disturbing them!'");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else
            {
                if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2,
                    player.Level > 47 ? Constants.SummonHiUndeadNoUniques : Constants.SummonUndead,
                    player.Level > 24 && Program.Rng.DieRoll(3) == 1))
                {
                    Profile.Instance.MsgPrint(
                        "Cold winds begin to swirl around you, carrying with them the stench of decay...");
                    Profile.Instance.MsgPrint("Ancient, long-dead forms arise from the ground to serve you!");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
        }

        protected override string Comment(Player player)
        {
            return "control 67%";
        }
    }
}