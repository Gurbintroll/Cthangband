﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class PatronUbboSathla : BasePatron
    {
        public PatronUbboSathla()
        {
            ShortName = "Ubbo-Sathla";
            LongName = "Ubbo-Sathla, the Unbegotten Source";
            PreferredAbility = Ability.Charisma;
            Rewards = new[]
            {
                Reward.Wrath, Reward.PissOff, Reward.PissOff, Reward.RuinAbl, Reward.LoseAbl, Reward.LoseExp,
                Reward.Ignore, Reward.Ignore, Reward.PolyWnd, Reward.SerDemo, Reward.PolySlf, Reward.HealFul,
                Reward.HealFul, Reward.GoodObj, Reward.GainExp, Reward.GainExp, Reward.ChaosWp, Reward.GainAbl,
                Reward.GreaObj, Reward.AugmAbl
            };
        }
    }
}