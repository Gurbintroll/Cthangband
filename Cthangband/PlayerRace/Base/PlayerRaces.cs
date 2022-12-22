// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cthangband.PlayerRace.Base
{
    internal class PlayerRaces : Dictionary<string, IPlayerRace>
    {
        private static PlayerRaces StaticInstance;

        private PlayerRaces()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlayerRace).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    IPlayerRace race = (IPlayerRace)Activator.CreateInstance(type);
                    Add(race.Title, race);
                }
            }
        }

        public static PlayerRaces Instance
        {
            get
            {
                if (StaticInstance == null)
                {
                    StaticInstance = new PlayerRaces();
                }
                return StaticInstance;
            }
        }

        internal string RandomRaceName()
        {
            var keyList = Keys.ToList();
            var size = keyList.Count;
            var randomIndex = Program.Rng.DieRoll(size) - 1;
            return keyList[randomIndex];
        }
    }
}