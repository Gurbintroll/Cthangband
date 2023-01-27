// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using System;

namespace Cthangband.Spells.Base
{
    [Serializable]
    internal abstract class BaseSpell : ISpell
    {
        public abstract int DefaultBaseFailure { get; }
        public abstract int DefaultLevel { get; }
        public abstract int DefaultVisCost { get; }
        public abstract int FirstCastExperience { get; }

        public bool Forgotten { get; set; }
        public bool Learned { get; set; }

        public int Level
        {
            get; set;
        }

        public abstract string Name { get; }
        public int VisCost { get; set; }
        public bool Worked { get; set; }

        protected int BaseFailure { get; set; }

        public abstract void Cast(SaveGame saveGame, Player player, Level level);

        public int FailureChance(Player player)
        {
            if (player.Spellcasting.Type == CastingType.None)
            {
                return 100;
            }
            int chance = BaseFailure;
            chance -= 3 * (player.Level - Level);
            chance -= 3 * (player.AbilityScores[player.Spellcasting.SpellStat].SpellFailureReduction - 1);
            if (VisCost > player.Vis)
            {
                chance += 5 * (VisCost - player.Vis);
            }
            int minfail = player.AbilityScores[player.Spellcasting.SpellStat].SpellMinFailChance;
            if (player.PlayerClass.HasMinimumSpellFailure)
            {
                if (minfail < 5)
                {
                    minfail = 5;
                }
            }
            if ((player.PlayerClass.BluntWeaponsOnly) && player.HasUnpriestlyWeapon)
            {
                chance += 25;
            }
            if (player.PlayerClass.ChaosWeaponsOnly && player.HasUnpriestlyWeapon)
            {
                chance += 25;
            }
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

        public string GetComment(Player player)
        {
            if (Forgotten)
            {
                return "forgotten";
            }
            if (!Learned)
            {
                return "unknown";
            }
            return !Worked ? "untried" : Comment(player);
        }

        public void Initialise(IPlayerClass playerClass)
        {
            Level = (int)Math.Round(20 * playerClass.SpellLevelMultiplier(Realm.Corporeal), MidpointRounding.AwayFromZero);
            VisCost = (int)Math.Round(18 * playerClass.SpellVisCostMultiplier(Realm.Corporeal), MidpointRounding.AwayFromZero);
            BaseFailure = (int)Math.Round(85 * playerClass.SpellBaseFailureMultiplier(Realm.Corporeal), MidpointRounding.AwayFromZero);
            if (Level < 1)
            {
                Level = 1;
            }
            if (VisCost < 1)
            {
                VisCost = 1;
            }
            if (Level > 50)
            {
                Level = 99;
                VisCost = 0;
                BaseFailure = 0;
            }
        }

        public string SummaryLine(Player player)
        {
            return Level >= 99
                ? "(illegible)"
                : $"{Name,-30} {Level,3} {VisCost,4} {FailureChance(player),3}% {GetComment(player)}";
        }

        public override string ToString()
        {
            return $"{Name} ({Level}, {VisCost}, {BaseFailure}, {FirstCastExperience})";
        }

        protected abstract string Comment(Player player);
    }
}