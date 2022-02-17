using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronCyaegha : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Cyaegha";
            LongName = "Cyaegha, the Destroying Eye";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.Wrath, Reward.HurtLot, Reward.PissOff, Reward.LoseAbl, Reward.LoseExp, Reward.Ignore,
                Reward.Ignore, Reward.DispelC, Reward.DoHavoc, Reward.DoHavoc, Reward.PolySlf, Reward.PolySlf,
                Reward.GainExp, Reward.GainAbl, Reward.GainAbl, Reward.SerMons, Reward.GoodObj, Reward.ChaosWp,
                Reward.GreaObj, Reward.GoodObs
            };
        }
    }
}