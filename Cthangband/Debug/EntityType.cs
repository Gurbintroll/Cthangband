// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”

using Cthangband.Enumerations;
using System;
using System.ComponentModel;

namespace Cthangband.Debug
{
    /// <summary>
    /// A serialisable graphical entity on which other classes can be based
    /// </summary>
    [Serializable]
    internal abstract class EntityType : IComparable<EntityType>
    {
        /// <summary>
        /// Create a new entity type with a randomised name
        /// </summary>
        protected EntityType()
        {
            Name = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// The column from which to take the graphical tile
        /// </summary>
        [Category("Base")]
        [Description("The column from which to take the graphical tile.")]
        public char Character
        {
            get; set;
        }

        /// <summary>
        /// The row from which to take the graphical tile
        /// </summary>
        [Category("Base")]
        [Description("The row from which to take the graphical tile.")]
        public Colour Colour
        {
            get; set;
        }

        /// <summary>
        /// A unique identifier for the entity
        /// </summary>
        [Category("Base")]
        [Description("A unique name.")]
        public string Name
        {
            get; set;
        }

        public int CompareTo(EntityType other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}