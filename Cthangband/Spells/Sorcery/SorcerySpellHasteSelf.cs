// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellHasteSelf : BaseSpell
    {
        public override int DefaultBaseFailure => 60;

        public override int DefaultLevel => 10;

        public override int DefaultVisCost => 12;

        public override int FirstCastExperience => 8;

        public override string Name => "Haste Self";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + player.Level) + player.Level);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
        }

        protected override string Comment(Player player)
        {
            return $"dur {player.Level}+d{player.Level + 20}";
        }
    }
}