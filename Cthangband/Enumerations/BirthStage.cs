// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    /// <summary>
    /// The stages of character generation
    /// </summary>
    internal static class BirthStage
    {
        public const int ClassSelection = 1;
        public const int Confirmation = 6;
        public const int GenderSelection = 5;
        public const int Introduction = 0;
        public const int Naming = 7;
        public const int RaceSelection = 2;
        public const int RealmSelection1 = 3;
        public const int RealmSelection2 = 4;
    }
}