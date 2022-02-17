using System;

namespace Cthangband
{
    [Serializable]
    internal class Gender
    {
        public readonly string Title;
        public readonly string Winner;

        public Gender()
        {
        }

        public Gender(string title, string winner)
        {
            Title = title;
            Winner = winner;
        }
    }
}