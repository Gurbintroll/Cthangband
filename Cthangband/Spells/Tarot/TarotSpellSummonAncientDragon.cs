﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonAncientDragon : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 48;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 200;

        public override string Name => "Summon Ancient Dragon";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of an ancient dragon...");
            if (Program.Rng.DieRoll(10) > 3)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level,
                    Constants.SummonHiDragonNoUniques, true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonHiDragonNoUniques))
            {
                Profile.Instance.MsgPrint("The summoned dragon gets angry!");
            }
            else
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