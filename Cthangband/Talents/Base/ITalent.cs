﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.Talents.Base
{
    internal interface ITalent
    {
        int BaseFailure { get; }
        int Level { get; }
        int ManaCost { get; }
        string Name { get; }

        int FailureChance(Player player);

        void Initialise(int characterClass);

        string SummaryLine(Player player);

        void Use(Player player, Level level, SaveGame saveGame);
    }
}