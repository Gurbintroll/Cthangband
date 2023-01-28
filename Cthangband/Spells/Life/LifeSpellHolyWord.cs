// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHolyWord : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 39;

        public override int DefaultVisCost => 40;

        public override int FirstCastExperience => 125;

        public override string Name => "Holy Word";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelEvil(player.Level * 4);
            player.RestoreHealth(1000);
            player.SetTimedFear(0);
            player.SetTimedPoison(0);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        protected override string Comment(Player player)
        {
            return $"d {4 * player.Level}/h 1000";
        }
    }
}