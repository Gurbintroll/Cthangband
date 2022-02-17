using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronNyarlathotep : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Nyarlathotep";
            LongName = "Nyarlathotep, the Crawling Chaos";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.DreadCurse, Reward.HurtLot, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.SummonM,
                Reward.LoseExp, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.SerUnde, Reward.HealFul,
                Reward.HealFul, Reward.GainExp, Reward.GainExp, Reward.ChaosWp, Reward.GoodObj, Reward.GoodObs,
                Reward.GreaObs, Reward.AugmAbl
            };
        }
    }
}