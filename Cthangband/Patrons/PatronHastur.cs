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