﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellWraithform : BaseSpell
    {
        public override int DefaultBaseFailure => 90;

        public override int DefaultLevel => 47;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 250;

        public override string Name => "Wraithform";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedEtherealness(player.TimedEtherealness + Program.Rng.DieRoll(player.Level / 2) + (player.Level / 2));
        }

        protected override string Comment(Player player)
        {
            return $"dur {player.Level / 2}+d{player.Level / 2}";
        }
    }
}