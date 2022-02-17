using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronAzathoth : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Azathoth";
            LongName = "Azathoth, the Daemon Sultan";
            PreferredAbility = Ability.Strength;
            Rewards = new[] // Azathoth
            {
                Reward.DreadCurse, Reward.DreadCurse, Reward.PissOff, Reward.RuinAbl, Reward.LoseAbl, Reward.Ignore,
                Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.PolyWnd, Reward.Carnage, Reward.DispelC,
                Reward.GoodObj, Reward.GoodObj, Reward.SerMons, Reward.GainAbl, Reward.ChaosWp, Reward.GainExp,
                Reward.AugmAbl, Reward.GoodObs
            };
        }
    }
}