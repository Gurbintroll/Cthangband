// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureMediumWounds : BaseSpell
    {
        public override int DefaultBaseFailure => 33;

        public override int DefaultLevel => 16;

        public override int DefaultVisCost => 14;

        public override int FirstCastExperience => 6;

        public override string Name => "Cure Medium Wounds";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 50);
        }

        protected override string Comment(Player player)
        {
            return "heal 4d8";
        }
    }
}