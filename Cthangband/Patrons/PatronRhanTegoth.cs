using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronRhanTegoth : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Rhan-Tegoth";
            LongName = "Rhan-Tegoth, He of the Ivory Throne";
            PreferredAbility = Ability.Intelligence;
            Rewards = new[]
            {
                Reward.Wrath, Reward.DreadCurse, Reward.PissOff, Reward.HSummon, Reward.HSummon, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.PolyWnd, Reward.PolySlf, Reward.PolySlf, Reward.SerDemo,
                Reward.HealFul, Reward.GainAbl, Reward.GainAbl, Reward.ChaosWp, Reward.DoHavoc, Reward.GoodObj,
                Reward.GreaObj, Reward.GreaObs
            };
        }
    }
}