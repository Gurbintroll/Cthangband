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
    internal class PatronYig : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Yig";
            LongName = "Yig, Father of Serpents";
            PreferredAbility = Ability.Strength;
            Rewards = new[] // Yig
            {
                Reward.Wrath, Reward.Wrath, Reward.CurseWp, Reward.CurseAr, Reward.RuinAbl, Reward.Ignore,
                Reward.Ignore, Reward.SerUnde, Reward.Destruct, Reward.Carnage, Reward.MassGen, Reward.MassGen,
                Reward.HealFul, Reward.GainAbl, Reward.GainAbl, Reward.ChaosWp, Reward.GoodObs, Reward.GoodObs,
                Reward.AugmAbl, Reward.AugmAbl
            };
        }
    }
}