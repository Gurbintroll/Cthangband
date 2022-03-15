// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband
{
    /// <summary>
    /// A 32-bit set of individual bit flags
    /// </summary>
    [Serializable]
    internal class FlagSet
    {
        /// <summary>
        /// Creates a new flagset with all flags cleared
        /// </summary>
        public FlagSet()
        {
            Value = 0;
        }

        /// <summary>
        /// Creates a new flagset with all flags set equal to those in the original
        /// </summary>
        /// <param name="original"> The original flagset from which to copy flags </param>
        public FlagSet(FlagSet original)
        {
            Value = original.Value;
        }

        /// <summary>
        /// Returns the upper half of the flags (shifted down) as a value
        /// </summary>
        public uint HighOrder => Value >> 16;

        /// <summary>
        /// Returns the lower half of the flags as a value
        /// </summary>
        public uint LowOrder => Value & 0xFFFF;

        /// <summary>
        /// Returns the flags as a value
        /// </summary>
        public uint Value
        {
            get; private set;
        }

        /// <summary>
        /// Clears one or more individual flags
        /// </summary>
        /// <param name="flags"> The individual flags to clear </param>
        public void Clear(uint flags)
        {
            Value &= ~flags;
        }

        /// <summary>
        /// Clears all flags
        /// </summary>
        public void Clear()
        {
            Value = 0;
        }

        /// <summary>
        /// Clears all flags that are set in another flagset. Flags that are clear in the other
        /// flagset are NOT changed
        /// </summary>
        /// <param name="flags"> </param>
        public void Clear(FlagSet flags)
        {
            Value &= ~flags.Value;
        }

        /// <summary>
        /// Sets all flags equal to those in another flagset
        /// </summary>
        /// <param name="flags"> The flagset from which to get the flags </param>
        public void Copy(FlagSet flags)
        {
            Value = flags.Value;
        }

        /// <summary>
        /// Checks whether one or more individual flags are clear
        /// </summary>
        /// <param name="flags"> The flags to check </param>
        /// <returns> True if ALL the flags are clear </returns>
        public bool IsClear(uint flags)
        {
            return (Value & flags) == 0;
        }

        /// <summary>
        /// Checkes whether any flags are set
        /// </summary>
        /// <returns> True if NO flags are set </returns>
        public bool IsClear()
        {
            return Value == 0;
        }

        /// <summary>
        /// Checks whether one or more individual flags are set
        /// </summary>
        /// <param name="flags"> The flags to check </param>
        /// <returns> True if at least one of the flags is set </returns>
        public bool IsSet(uint flags)
        {
            return (Value & flags) != 0;
        }

        /// <summary>
        /// Checks whether any flags are set
        /// </summary>
        /// <returns> True if ANY flags are set </returns>
        public bool IsSet()
        {
            return Value != 0;
        }

        /// <summary>
        /// Checks whether specific flags match in both flagsets
        /// </summary>
        /// <param name="other"> The other flagset to check against </param>
        /// <param name="flags"> The specific flag or flags to check </param>
        /// <returns> True if the flags are the same in both sets </returns>
        public bool Matches(FlagSet other, uint flags)
        {
            return (Value & flags) == (other.Value & flags);
        }

        /// <summary>
        /// Checks whether all flags match in both flagsets
        /// </summary>
        /// <param name="other"> The other flagset to check against </param>
        /// <returns> True if the flags are the same in both sets </returns>
        public bool Matches(FlagSet other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// Sets one or more individual flags
        /// </summary>
        /// <param name="flags"> One or more individual flags </param>
        public void Set(uint flags)
        {
            Value |= flags;
        }

        /// <summary>
        /// Sets all flags
        /// </summary>
        public void Set()
        {
            Value = uint.MaxValue;
        }

        /// <summary>
        /// Sets all flags that are set in another flagset. Flags that are not set in the other
        /// flagset are NOT changed.
        /// </summary>
        /// <param name="flags"> The flagset from which to get the flags </param>
        public void Set(FlagSet flags)
        {
            Value |= flags.Value;
        }
    }
}