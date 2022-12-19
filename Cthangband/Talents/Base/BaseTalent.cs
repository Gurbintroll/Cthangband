// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Talents.Base
{
    [Serializable]
    internal abstract class BaseTalent : ITalent
    {
        public int BaseFailure { get; set; }

        public int Level
        {
            get; protected set;
        }

        public int VisCost { get; set; }

        public string Name { get; set; }

        public int FailureChance(Player player)
        {
            int chance = BaseFailure;
            chance -= 3 * (player.Level - Level);
            chance -= 3 * (player.AbilityScores[player.Spellcasting.SpellStat].SpellFailureReduction - 1);
            if (VisCost > player.Vis)
            {
                chance += 5 * (VisCost - player.Vis);
            }
            int minfail = player.AbilityScores[player.Spellcasting.SpellStat].SpellMinFailChance;
            if (chance < minfail)
            {
                chance = minfail;
            }
            if (player.TimedStun > 50)
            {
                chance += 25;
            }
            else if (player.TimedStun != 0)
            {
                chance += 15;
            }
            if (chance > 95)
            {
                chance = 95;
            }
            return chance;
        }

        public abstract void Initialise(int characterClass);

        public string SummaryLine(Player player)
        {
            return $"{Name,-30}{Level,2} {VisCost,4} {FailureChance(player),3}% {Comment(player)}";
        }

        public override string ToString()
        {
            return $"{Name} ({Level}, {VisCost}, {BaseFailure})";
        }

        public abstract void Use(Player player, Level level, SaveGame saveGame);

        protected abstract string Comment(Player player);
    }
}