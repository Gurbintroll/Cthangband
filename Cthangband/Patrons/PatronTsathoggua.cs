using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronTsathoggua : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Tsathoggua";
            LongName = "Tsathoggua, the Sleeper of N'Kai";
            PreferredAbility = Ability.Intelligence;
            Rewards = new[]
            {
                Reward.Wrath, Reward.PissOff, Reward.RuinAbl, Reward.LoseExp, Reward.HSummon, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.MassGen,
                Reward.SerDemo, Reward.HealFul, Reward.ChaosWp, Reward.ChaosWp, Reward.GoodObj, Reward.GainExp,
                Reward.GreaObj, Reward.AugmAbl
            };
        }
    }
}