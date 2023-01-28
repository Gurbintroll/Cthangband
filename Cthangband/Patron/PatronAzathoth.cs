// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Patron.Base;
using System;

namespace Cthangband.Patron
{
    [Serializable]
    internal class PatronAzathoth : BasePatron
    {
        public PatronAzathoth()
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