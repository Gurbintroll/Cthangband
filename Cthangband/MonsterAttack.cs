using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class MonsterAttack
    {
        public int DDice;
        public int DSide;
        public AttackEffect Effect;
        public AttackType Method;

        public MonsterAttack()
        {
        }

        public MonsterAttack(MonsterAttack original)
        {
            Method = original.Method;
            Effect = original.Effect;
            DDice = original.DDice;
            DSide = original.DSide;
        }

        public override string ToString()
        {
            return $"{Method} to {Effect} ({DDice}d{DSide})";
        }
    }
}