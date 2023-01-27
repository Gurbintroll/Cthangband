// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellForaging : BaseSpell
    {
        public override int DefaultBaseFailure => 35;

        public override int DefaultLevel => 4;

        public override int DefaultVisCost => 4;

        public override int FirstCastExperience => 4;

        public override string Name => "Foraging";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetFood(Constants.PyFoodMax - 1);
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}