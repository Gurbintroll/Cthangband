using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Display a list of all the keyboard commands
    /// </summary>
    [Serializable]
    internal class CommandListCommand : ICommand
    {
        public char Key => '?';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.Refresh();
            Gui.Clear();
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            Gui.Print(Colour.Yellow, "Numpad", 1, 1);
            Gui.Print("7 8 9", 3, 1);
            Gui.Print(" \\|/", 4, 1);
            Gui.Print("4- -6 = Move", 5, 1);
            Gui.Print(" /|\\    (+Shift = run)", 6, 1);
            Gui.Print("1 2 3", 7, 1);
            Gui.Print("5 = Stand still", 8, 1);
            Gui.Print(Colour.Yellow, "Other Symbols", 10, 1);
            Gui.Print(". = Run", 12, 1);
            Gui.Print("< = Go up stairs", 13, 1);
            Gui.Print("> = Go down stairs", 14, 1);
            Gui.Print("+ = Auto-alter a space", 15, 1);
            Gui.Print("* = Target a creature", 16, 1);
            Gui.Print("/ = Identify a symbol", 17, 1);
            Gui.Print("? = Command list", 18, 1);
            Gui.Print("Esc = Save and quit", 20, 1);
            Gui.Print(Colour.Yellow, "Without Shift", 1, 25);
            Gui.Print("a = Aim a wand", 3, 25);
            Gui.Print("b = Browse a book", 4, 25);
            Gui.Print("c = Close a door", 5, 25);
            Gui.Print("d = Drop object", 6, 25);
            Gui.Print("e = Show equipment", 7, 25);
            Gui.Print("f = Fire a missile weapon", 8, 25);
            Gui.Print("g = Get (pick up) object", 9, 25);
            Gui.Print("h = View game help", 10, 25);
            Gui.Print("i = Show Inventory", 11, 25);
            Gui.Print("j = Jam spike in a door", 12, 25);
            Gui.Print("k = Destroy an item", 13, 25);
            Gui.Print("l = Look around", 14, 25);
            Gui.Print("m = Cast spell/Use talent", 15, 25);
            Gui.Print("n =", 16, 25);
            Gui.Print("o = Open a door/chest", 17, 25);
            Gui.Print("p = Mutant/Racial power", 18, 25);
            Gui.Print("q = Quaff a potion", 19, 25);
            Gui.Print("r = Read a scroll", 20, 25);
            Gui.Print("s = Search for traps", 21, 25);
            Gui.Print("t = Take off an item", 22, 25);
            Gui.Print("u = Use a staff", 23, 25);
            Gui.Print("v = Throw object", 24, 25);
            Gui.Print("w = Wield/wear an item", 25, 25);
            Gui.Print("x = Examine an object", 26, 25);
            Gui.Print("y =", 27, 25);
            Gui.Print("z = Zap a rod", 28, 25);
            Gui.Print(Colour.Yellow, "With Shift", 1, 52);
            Gui.Print("A = Activate an artifact", 3, 52);
            Gui.Print("B = Bash a stuck door", 4, 52);
            Gui.Print("C = View your character", 5, 52);
            Gui.Print("D = Disarm a trap", 6, 52);
            Gui.Print("E = Eat some food", 7, 52);
            Gui.Print("F = Fuel a light source", 8, 52);
            Gui.Print("H = How you feel here", 10, 52);
            Gui.Print("J = View your journal", 12, 52);
            Gui.Print("K = Destroy trash objects", 13, 52);
            Gui.Print("L = Locate player", 14, 52);
            Gui.Print("M = View the map", 15, 52);
            Gui.Print("O = Show last message", 17, 52);
            Gui.Print("P = Show previous messages", 18, 52);
            Gui.Print("Q = Quit (Retire character)", 19, 52);
            Gui.Print("R = Rest", 20, 52);
            Gui.Print("S = Auto-search on/off", 21, 52);
            Gui.Print("T = Tunnel", 22, 52);
            Gui.Print("V = Version info", 24, 52);
            if (player.IsWizard)
            {
                Gui.Print("W = Wizard command", 25, 52);
            }
            Gui.AnyKey(44);
            Gui.Load();
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.FullScreenOverlay = false;
        }
    }
}
