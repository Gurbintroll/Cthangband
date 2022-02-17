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