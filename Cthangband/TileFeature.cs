using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class TileFeature
    {
        public Colour Attr;
        public char Char;
        public int Mimic;
        public string Name;

        public TileFeature()
        {
        }

        public TileFeature(TileFeature original)
        {
            Attr = original.Attr;
            Char = original.Char;
            Mimic = original.Mimic;
            Name = original.Name;
        }
    }
}