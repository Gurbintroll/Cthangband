﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellSummonAnimal : BaseSpell
    {
        public override int DefaultBaseFailure => 90;

        public override int DefaultLevel => 25;

        public override int DefaultVisCost => 25;

        public override int FirstCastExperience => 50;

        public override string Name => "Summon Animal";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonAnimalRanger,
                true))
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        protected override string Comment(Player player)
        {
            return "control 100%";
        }
    }
}