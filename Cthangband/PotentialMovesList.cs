// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband
{
    /// <summary>
    /// A matrix of possible moves that can be made by a monster
    /// </summary>
    [Serializable]
    internal class PotentialMovesList
    {
        private readonly int[] _values = new int[8];

        public int this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }
    }
}