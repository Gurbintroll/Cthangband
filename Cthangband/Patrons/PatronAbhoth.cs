// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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