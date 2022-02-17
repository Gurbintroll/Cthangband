using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronYig : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Yig";
            LongName = "Yig, Father of Serpents";
            PreferredAbility = Ability.Strength;
            Rewards = new[] // Yig
            {
                Reward.Wrath, Reward.Wrath, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.Ignore,
                Reward.Ignore, Reward.SerUnde, Reward.Destruct, Reward.Carnage, Reward.MassGen, Reward.MassGen,
                Reward.HealFul, Reward.GainAbl, Reward.GainAbl, Reward.ChaosWp, Reward.GoodObs, Reward.GoodObs,
                Reward.AugmAbl, Reward.AugmAbl
            };
        }
    }
}