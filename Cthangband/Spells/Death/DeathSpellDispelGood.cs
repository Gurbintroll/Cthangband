﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class DeathSpellDispelGood : BaseSpell
    {
        public override int DefaultBaseFailure => 60;

        public override int DefaultLevel => 33;

        public override int DefaultVisCost => 35;

        public override int FirstCastExperience => 16;

        public override string Name => "Dispel Good";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelGood(player.Level * 4);
        }

        protected override string Comment(Player player)
        {
            return $"dam {4 * player.Level}";
        }
    }
}