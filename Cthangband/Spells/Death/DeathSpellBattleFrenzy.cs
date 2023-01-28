// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellBattleFrenzy : BaseSpell
    {
        public override int DefaultBaseFailure => 75;

        public override int DefaultLevel => 30;

        public override int DefaultVisCost => 25;

        public override int FirstCastExperience => 50;

        public override string Name => "Battle Frenzy";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(30);
            player.SetTimedFear(0);
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + (player.Level / 2)) + (player.Level / 2));
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
        }

        protected override string Comment(Player player)
        {
            return "max dur 50";
        }
    }
}