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
using System.Reflection;

namespace Cthangband.PlayerClass.Base
{
    internal class PlayerClasses : Dictionary<string, IPlayerClass>
    {
        private static PlayerClasses StaticInstance;

        private PlayerClasses()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlayerClass).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    IPlayerClass playerClass = (IPlayerClass)Activator.CreateInstance(type);
                    Add(playerClass.Title, playerClass);
                }
            }
        }

        public static PlayerClasses Instance
        {
            get
            {
                if (StaticInstance == null)
                {
                    StaticInstance = new PlayerClasses();
                }
                return StaticInstance;
            }
        }

        internal string RandomClassName()
        {
            var keyList = Keys.ToList();
            var size = keyList.Count;
            var randomIndex = Program.Rng.DieRoll(size) - 1;
            return keyList[randomIndex];
        }
    }
}