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
    internal class PatronYogSothoth : Patron
    {
        protected override void Initialise()
        {
            ShortName = "Yog Sothoth";
            LongName = "Yog Sothoth, the Gate and the Key";
            PreferredAbility = Ability.Strength;
            Rewards = new[]
            {
                Reward.Wrath, Reward.Wrath, Reward.HurtLot, Reward.PissOff, Reward.HSummon, Reward.SummonM,
                Reward.Ignore, Reward.Ignore, Reward.Destruct, Reward.SerUnde, Reward.Carnage, Reward.MassGen,
                Reward.MassGen, Reward.DispelC, Reward.GoodObj, Reward.ChaosWp, Reward.GoodObs, Reward.GoodObs,
                Reward.AugmAbl, Reward.AugmAbl
            };
        }
    }
}