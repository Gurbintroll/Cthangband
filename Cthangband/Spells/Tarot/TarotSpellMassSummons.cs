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
    internal class TarotSpellMassSummons : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 43;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 200;

        public override string Name => "Mass Summons";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var noneCame = true;
            Profile.Instance.MsgPrint("You concentrate on several images at once...");
            for (var dummy = 0; dummy < 3 + (player.Level / 10); dummy++)
            {
                if (Program.Rng.DieRoll(10) > 3)
                {
                    if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level,
                        Constants.SummonNoUniques, false))
                    {
                        noneCame = false;
                    }
                }
                else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, 0))
                {
                    Profile.Instance.MsgPrint("A summoned creature gets angry!");
                    noneCame = false;
                }
            }
            if (noneCame)
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        protected override string Comment(Player player)
        {
            return "control 70%";
        }
    }
}