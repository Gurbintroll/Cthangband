using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronCthulhu : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Cthulhu";
            LongName = "Cthulhu, who Lies Dreaming";
            PreferredAbility = Ability.Constitution;
            Rewards = new[]
            {
                Reward.Wrath, Reward.PissOff, Reward.HurtLot, Reward.RuinAbl, Reward.LoseAbl, Reward.LoseExp,
                Reward.Ignore, Reward.Ignore, Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd,
                Reward.HealFul, Reward.GoodObj, Reward.GainAbl, Reward.GainAbl, Reward.SerUnde, Reward.ChaosWp,
                Reward.GreaObj, Reward.AugmAbl
            };
        }
    }
}