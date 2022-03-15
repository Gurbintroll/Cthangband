// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cthangband.Pantheon
{
    [Serializable]
    internal class Religion
    {
        private const int DecayRate = 10;
        private const int PatronRestingFavour = 30;
        private readonly Dictionary<GodName, God> _gods;
        private GodName _patron;

        public Religion()
        {
            _gods = new Dictionary<GodName, God>
            {
                { GodName.Lobon, new God(GodName.Lobon) },
                { GodName.Nath_Horthah, new God(GodName.Nath_Horthah) },
                { GodName.Hagarg_Ryonis, new God(GodName.Hagarg_Ryonis) },
                { GodName.Tamash, new God(GodName.Tamash) },
                { GodName.Zo_Kalar, new God(GodName.Zo_Kalar) }
            };
        }

        public GodName Deity
        {
            get
            {
                return _patron;
            }
            set
            {
                _patron = value;
                if (_patron == GodName.None)
                {
                    foreach (KeyValuePair<GodName, God> pair in _gods)
                    {
                        pair.Value.IsPatron = false;
                        pair.Value.RestingFavour = 0;
                        pair.Value.Favour = 0;
                    }
                }
                else
                {
                    foreach (KeyValuePair<GodName, God> pair in _gods)
                    {
                        if (pair.Key == _patron)
                        {
                            pair.Value.IsPatron = true;
                            pair.Value.RestingFavour = PatronRestingFavour * 4;
                            pair.Value.Favour = PatronRestingFavour * 4;
                        }
                        else
                        {
                            pair.Value.IsPatron = false;
                            pair.Value.RestingFavour = -PatronRestingFavour;
                            pair.Value.Favour = -PatronRestingFavour;
                        }
                    }
                }
            }
        }

        public void AddFavour(GodName godName, int amount)
        {
            foreach (KeyValuePair<GodName, God> pair in _gods)
            {
                if (pair.Key == godName)
                {
                    pair.Value.Favour += (4 * amount);
                }
                else
                {
                    pair.Value.Favour -= amount;
                }
            }
        }

        public void DecayFavour()
        {
            var max = 0;
            var isMax = GodName.None;
            foreach (KeyValuePair<GodName, God> pair in _gods)
            {
                if (pair.Value.Favour - pair.Value.RestingFavour > max)
                {
                    max = pair.Value.Favour - pair.Value.RestingFavour;
                    isMax = pair.Key;
                }
            }
            if (isMax != GodName.None)
            {
                var decrement = Math.Max((max / DecayRate), 1);
                AddFavour(isMax, -decrement);
            }
        }

        public List<God> GetAllDeities()
        {
            return _gods.Values.ToList();
        }

        public God GetNamedDeity(GodName godName)
        {
            return _gods[godName];
        }

        public God GetPatronDeity()
        {
            if (_patron == GodName.None)
            {
                return null;
            }
            return _gods[_patron];
        }
    }
}