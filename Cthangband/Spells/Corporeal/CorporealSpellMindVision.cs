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

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellMindVision : BaseSpell
    {
        public override int DefaultBaseFailure => 35;

        public override int DefaultLevel => 5;

        public override int DefaultVisCost => 5;

        public override int FirstCastExperience => 5;

        public override string Name => "Mind Vision";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
        }

        protected override string Comment(Player player)
        {
            return "dur 25+d30";
        }
    }
}