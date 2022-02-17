using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronAbhoth : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Abhoth";
            LongName = "Abhoth, the Unclean";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.Wrath, Reward.HurtLot, Reward.HurtLot, Reward.HSummon, Reward.HSummon, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.SerMons, Reward.SerDemo, Reward.PolySlf, Reward.PolyWnd,
                Reward.HealFul, Reward.GoodObj, Reward.GoodObj, Reward.ChaosWp, Reward.GoodObs, Reward.GoodObs,
                Reward.GreaObj, Reward.GreaObs
            };
        }
    }
}