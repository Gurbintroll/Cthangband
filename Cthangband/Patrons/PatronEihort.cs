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