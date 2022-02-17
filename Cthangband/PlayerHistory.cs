using System;

namespace Cthangband
{
    [Serializable]
    internal class PlayerHistory
    {
        public readonly int Bonus;
        public readonly int Chart;
        public readonly string Info;
        public readonly int Next;
        public readonly int Roll;

        public PlayerHistory(string info, int roll, int chart, int next, int bonus)
        {
            Info = info;
            Roll = roll;
            Chart = chart;
            Next = next;
            Bonus = bonus;
        }
    }
}