﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class PatronIod : BasePatron
    {
        public PatronIod()
        {
            ShortName = "Iod";
            LongName = "Iod, the Shining Hunter";
            PreferredAbility = Ability.Charisma;
            Rewards = new[]
            {
                Reward.Wrath, Reward.CurseAr, Reward.CurseWp, Reward.CurseWp, Reward.CurseAr, Reward.Ignore,
                Reward.Ignore, Reward.Ignore, Reward.PolySlf, Reward.PolySlf, Reward.PolyWnd, Reward.HealFul,
                Reward.HealFul, Reward.GainExp, Reward.AugmAbl, Reward.GoodObj, Reward.GoodObj, Reward.ChaosWp,
                Reward.GreaObj, Reward.GreaObs
            };
        }
    }
}