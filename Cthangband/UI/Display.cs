using Cthangband.Enumerations;

namespace Cthangband.UI
{
    internal class Display
    {
        public readonly Colour AttrBlank;
        public readonly char CharBlank;
        public readonly int Height;
        public readonly char[] KeyQueue;
        public readonly int KeySize;
        public readonly Screen Old;
        public readonly Screen Scr;
        public readonly int Width;
        public readonly int[] X1;
        public readonly int[] X2;
        public int KeyHead;
        public int KeyTail;
        public Screen Mem;
        public bool TotalErase;
        public int Y1;
        public int Y2;

        public Display(int w, int h, int k)
        {
            KeyHead = 0;
            KeyTail = 0;
            KeySize = k;
            KeyQueue = new char[k];
            Width = w;
            Height = h;
            X1 = new int[h];
            X2 = new int[h];
            Old = new Screen(w, h);
            Scr = new Screen(w, h);
            for (int y = 0; y < h; y++)
            {
                X1[y] = 0;
                X2[y] = w - 1;
            }
            Y1 = 0;
            Y2 = h - 1;
            TotalErase = true;
            AttrBlank = 0;
            CharBlank = ' ';
        }
    }
}