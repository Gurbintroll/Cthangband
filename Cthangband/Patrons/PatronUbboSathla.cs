using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronUbboSathla : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Ubbo-Sathla";
            LongName = "Ubbo-Sathla, the Unbegotten Source";
            PreferredAbility = Ability.Charisma;
            Rewards = new[]
            {
                Reward.Wrath, Reward.PissOff, Reward.PissOff, Reward.RuinAbl, Reward.LoseAbl, Reward.LoseExp,
                Reward.Ignore, Reward.Ignore, Reward.PolyWnd, Reward.SerDemo, Reward.PolySlf, Reward.HealFul,
                Reward.HealFul, Reward.GoodObj, Reward.GainExp, Reward.GainExp, Reward.ChaosWp, Reward.GainAbl,
                Reward.GreaObj, Reward.AugmAbl
            };
        }
    }
}