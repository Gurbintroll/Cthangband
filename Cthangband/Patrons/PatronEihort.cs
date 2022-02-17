using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronEihort : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Eihort";
            LongName = "Eihort, God of the Labyrinth";
            PreferredAbility = Ability.Constitution;
            Rewards = new[]
            {
                Reward.Wrath, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.LoseAbl, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.PolyWnd, Reward.PolySlf, Reward.PolySlf, Reward.PolySlf,
                Reward.GainAbl, Reward.GainAbl, Reward.GainExp, Reward.GoodObj, Reward.ChaosWp, Reward.GreaObj,
                Reward.AugmAbl, Reward.AugmAbl
            };
        }
    }
}