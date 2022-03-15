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
    internal class PatronNyarlathotep : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Nyarlathotep";
            LongName = "Nyarlathotep, the Crawling Chaos";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.DreadCurse, Reward.HurtLot, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.SummonM,
                Reward.LoseExp, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.SerUnde, Reward.HealFul,
                Reward.HealFul, Reward.GainExp, Reward.GainExp, Reward.ChaosWp, Reward.GoodObj, Reward.GoodObs,
                Reward.GreaObs, Reward.AugmAbl
            };
        }
    }
}