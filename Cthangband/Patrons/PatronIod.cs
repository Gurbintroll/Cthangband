using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronIod : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Iod";
            LongName = "Iod, the Shining Hunter";
            PreferredAbility = Ability.Charisma;
            Rewards = new[]
            {
                Reward.Wrath, Reward.CurseAr, Reward.CurseWp, Reward.CurseWp, Reward.CurseAr, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.HealFul,
                Reward.HealFul, Reward.GainExp, Reward.AugmAbl, Reward.GoodObj, Reward.GoodObj, Reward.ChaosWp,
                Reward.GreaObj, Reward.GreaObs
            };
        }
    }
}