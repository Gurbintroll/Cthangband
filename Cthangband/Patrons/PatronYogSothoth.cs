using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronYogSothoth : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Yog Sothoth";
            LongName = "Yog Sothoth, the Gate and the Key";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.Wrath, Reward.Wrath, Reward.HurtLot, Reward.PissOff, Reward.HSummon, Reward.SummonM,
                Reward.Ignore, Reward.Ignore, Reward.Destruct, Reward.SerUnde, Reward.Carnage, Reward.MassGen,
                Reward.MassGen, Reward.DispelC, Reward.GoodObj, Reward.ChaosWp, Reward.GoodObs, Reward.GoodObs,
                Reward.AugmAbl, Reward.AugmAbl
            };
        }
    }
}