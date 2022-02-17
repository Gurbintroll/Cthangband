using System;

namespace Cthangband.Pantheon
{
    [Serializable]
    internal class God
    {
        public string LongName;
        public string ShortName;
        private const int PatronMultiplier = 2;

        private int _favour = 0;
        private bool _isPatron;
        private GodName _name;
        private int _restingFavour = 0;

        public God(GodName name)
        {
            _name = name;
            switch (name)
            {
                case GodName.Lobon:
                    ShortName = "Lobon";
                    LongName = "Lobon, god of youth";
                    break;

                case GodName.Nath_Horthah:
                    ShortName = "Nath-Horthath";
                    LongName = "Nath-Horthath, god of war";
                    break;

                case GodName.Tamash:
                    ShortName = "Tamash";
                    LongName = "Tamash, god of illusion and magic";
                    break;

                case GodName.Zo_Kalar:
                    ShortName = "Zo-Kalar";
                    LongName = "Zo-Kalar, god of birth and death";
                    break;

                case GodName.Hagarg_Ryonis:
                    ShortName = "Hagarg Ryonis";
                    LongName = "Hagarg Ryonis, goddess of beasts";
                    break;
            }
        }

        public int AdjustedFavour
        {
            get
            {
                if (Favour <= 0)
                {
                    return 0;
                }
                var adjusted = IsPatron ? Favour * PatronMultiplier : Favour;
                return Math.Min(adjusted.ToString().Length, 10);
            }
        }

        public int Favour { get => _favour; set => _favour = value; }
        public bool IsPatron { get => _isPatron; internal set => _isPatron = value; }
        public GodName Name { get => _name; set => _name = value; }
        public int RestingFavour { get => _restingFavour; set => _restingFavour = value; }
    }
}