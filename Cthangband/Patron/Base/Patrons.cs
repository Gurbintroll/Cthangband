// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cthangband.Patron.Base
{
    internal class Patrons : Dictionary<string, IPatron>
    {
        private static Patrons StaticInstance;

        private Patrons()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPatron).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var patron = (IPatron)Activator.CreateInstance(type);
                    Add(patron.LongName, patron);
                }
            }
        }

        public static Patrons Instance
        {
            get
            {
                if (StaticInstance == null)
                {
                    StaticInstance = new Patrons();
                }
                return StaticInstance;
            }
        }

        internal string RandomPatronName()
        {
            var keyList = Keys.ToList();
            var size = keyList.Count;
            var randomIndex = Program.Rng.DieRoll(size) - 1;
            return keyList[randomIndex];
        }
    }
}