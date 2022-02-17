using Cthangband.Enumerations;
using System;

namespace Cthangband.Patrons
{
    [Serializable]
    internal class PatronShubNiggurath : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Shub Niggurath";
            LongName = "Shub Niggurath, Black Goat of the Woods";
            PreferredAbility = Ability.Intelligence;
            Rewards = new[]
            {
                Reward.Wrath, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.LoseAbl, Reward.LoseExp,
                Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd,
                Reward.HealFul, Reward.ChaosWp, Reward.GreaObj, Reward.GainAbl, Reward.GainAbl, Reward.GainExp,
                Reward.GainExp, Reward.AugmAbl
            };
        }
    }
}