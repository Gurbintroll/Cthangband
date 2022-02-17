using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronGlaaki : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Glaaki";
            LongName = "Glaaki, Lord of Dead Dreams";
            PreferredAbility = Ability.Constitution;
            Rewards = new[]
            {
                Reward.Wrath, Reward.CurseWp, Reward.CurseAr, Reward.HSummon, Reward.SummonM, Reward.SummonM,
                Reward.Ignore, Reward.Ignore, Reward.PolyWnd, Reward.PolyWnd, Reward.PolySlf, Reward.HealFul,
                Reward.HealFul, Reward.GainAbl, Reward.SerUnde, Reward.ChaosWp, Reward.GoodObj, Reward.GoodObj,
                Reward.GoodObs, Reward.GoodObs
            };
        }
    }
}