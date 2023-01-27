// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellSummonDemon : BaseSpell
    {
        public override int DefaultBaseFailure => 90;

        public override int DefaultLevel => 47;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 250;

        public override string Name => "Summon Demon";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3) == 1)
            {
                if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level * 3 / 2, Constants.SummonDemon))
                {
                    Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                    Profile.Instance.MsgPrint("'NON SERVIAM! Wretch! I shall feast on thy mortal soul!'");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else
            {
                if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2,
                    Constants.SummonDemon, player.Level == 50))
                {
                    Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                    Profile.Instance.MsgPrint("'What is thy bidding... Master?'");
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