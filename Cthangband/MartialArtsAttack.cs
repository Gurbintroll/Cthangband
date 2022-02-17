using System;

namespace Cthangband
{
    [Serializable]
    internal class MartialArtsAttack
    {
        public readonly int Chance;
        public readonly int Dd;
        public readonly string Desc;
        public readonly int Ds;
        public readonly int Effect;
        public readonly int MinLevel;

        public MartialArtsAttack(string desc, int minLevel, int chance, int dd, int ds, int effect)
        {
            Desc = desc;
            MinLevel = minLevel;
            Chance = chance;
            Dd = dd;
            Ds = ds;
            Effect = effect;
        }
    }
}