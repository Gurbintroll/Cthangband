﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”

using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;

namespace Cthangband.Spells.Base
{
    internal interface ISpell
    {
        int FirstCastExperience { get; }
        bool Forgotten { get; set; }
        bool Learned { get; set; }
        int Level { get; set; }
        string Name { get; }
        int VisCost { get; set; }
        bool Worked { get; set; }

        void Cast(SaveGame saveGame, Player player, Level level);

        int FailureChance(Player player);
        void Initialise(IPlayerClass playerClass, Realm realm);
        string SummaryLine(Player player);
    }
}