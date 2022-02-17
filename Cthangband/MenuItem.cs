using System;

namespace Cthangband
{
    [Serializable]
    internal class MenuItem
    {
        public readonly int Index;
        public readonly string Text;

        public MenuItem(string text, int index)
        {
            Text = text;
            Index = index;
        }
    }
}