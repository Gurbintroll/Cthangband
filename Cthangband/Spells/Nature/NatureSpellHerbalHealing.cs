﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellHerbalHealing : BaseSpell
    {
        public override int DefaultBaseFailure => 95;

        public override int DefaultLevel => 40;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 50;

        public override string Name => "Herbal Healing";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(1000);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
            player.SetTimedPoison(0);
        }

        protected override string Comment(Player player)
        {
            return "heal 1000";
        }
    }
}