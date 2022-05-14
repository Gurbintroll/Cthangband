// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Terminal
{
    internal class Resolution
    {
        public int Height;
        public int Size;
        public int Width;

        public Resolution(int size)
        {
            Size = size;
            Width = (size + 2) * 160;
            Height = (size + 2) * 90;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}