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
    internal class NatureSpellResistEnvironment : BaseSpell
    {
        public override int DefaultBaseFailure => 50;

        public override int DefaultLevel => 5;

        public override int DefaultVisCost => 5;

        public override int FirstCastExperience => 5;

        public override string Name => "Resist Environment";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
        }

        protected override string Comment(Player player)
        {
            return "dur 20+d20";
        }
    }
}