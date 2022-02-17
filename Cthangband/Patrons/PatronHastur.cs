using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronHastur : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Hastur";
            LongName = "Hastur, the Unspeakable";
            PreferredAbility = -1;
            Rewards = new[]
            {
                Reward.Wrath, Reward.SerDemo, Reward.CurseWp, Reward.CurseAr, Reward.LoseExp, Reward.GainAbl,
                Reward.LoseAbl, Reward.PolyWnd, Reward.PolySlf, Reward.Ignore, Reward.Destruct, Reward.MassGen,
                Reward.ChaosWp, Reward.GreaObj, Reward.HurtLot, Reward.AugmAbl, Reward.RuinAbl, Reward.HSummon,
                Reward.GreaObs, Reward.AugmAbl
            };
        }
    }
}