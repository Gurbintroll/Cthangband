// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal class IllegibleSpell : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
        }

        public override void Initialise(int characterClass)
        {
            Name = "(illegible)";
            Level = 99;
            ManaCost = 0;
            BaseFailure = 0;
            FirstCastExperience = 0;
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}