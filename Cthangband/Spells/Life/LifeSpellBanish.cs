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

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellBanish : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 35;

        public override int DefaultVisCost => 55;

        public override int FirstCastExperience => 115;

        public override string Name => "Banish";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.SpellEffects.BanishEvil(100))
            {
                Profile.Instance.MsgPrint("The power of your god banishes evil!");
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}