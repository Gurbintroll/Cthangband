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

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureLightWounds : BaseSpell
    {
        public override int DefaultBaseFailure => 44;

        public override int DefaultLevel => 6;

        public override int DefaultVisCost => 5;

        public override int FirstCastExperience => 5;

        public override string Name => "Cure Light Wounds";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 8));
            player.SetTimedBleeding(player.TimedBleeding - 10);
        }

        protected override string Comment(Player player)
        {
            return "heal 2d8";
        }
    }
}