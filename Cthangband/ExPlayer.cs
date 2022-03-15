// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class ExPlayer
    {
        public readonly int Generation;
        public readonly int Lev;
        public readonly string Name;
        public readonly int PBirthRace;
        public readonly int Pclass;
        public readonly int Prace;
        public readonly int Psex;
        public readonly Realm Realm1;
        public readonly Realm Realm2;

        public ExPlayer(Player player)
        {
            Psex = player.GenderIndex;
            Prace = player.RaceIndex;
            PBirthRace = player.RaceIndexAtBirth;
            Pclass = player.ProfessionIndex;
            Realm1 = player.Realm1;
            Realm2 = player.Realm2;
            Name = player.Name;
            Lev = player.Level;
            Generation = player.Generation;
        }
    }
}