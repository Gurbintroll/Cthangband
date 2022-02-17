using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronNyogtha : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Nyogtha";
            LongName = "Nyogtha, the Thing Which Should Not Be";
            PreferredAbility = Ability.Constitution;
            Rewards = new[]
            {
                Reward.Wrath, Reward.DreadCurse, Reward.PissOff, Reward.CurseWp, Reward.RuinAbl, Reward.Ignore,
                Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.GoodObj, Reward.GoodObj,
                Reward.SerMons, Reward.HealFul, Reward.GainExp, Reward.GainAbl, Reward.ChaosWp, Reward.GoodObs,
                Reward.GreaObj, Reward.AugmAbl
            };
        }
    }
}