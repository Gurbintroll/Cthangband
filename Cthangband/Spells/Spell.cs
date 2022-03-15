// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal abstract class Spell
    {
        public bool Forgotten;
        public bool Learned;
        public string Name;
        public bool Worked;

        public int FirstCastExperience
        {
            get; protected set;
        }

        public int Level
        {
            get; protected set;
        }

        public int ManaCost
        {
            get; protected set;
        }

        protected int BaseFailure
        {
            get; set;
        }

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
            if (ManaCost > player.Mana)
            {
                chance += 5 * (ManaCost - player.Mana);
            }
            int minfail = player.AbilityScores[player.Spellcasting.SpellStat].SpellMinFailChance;
            if (player.ProfessionIndex != CharacterClass.Priest && player.ProfessionIndex != CharacterClass.Druid &&
                player.ProfessionIndex != CharacterClass.Mage && player.ProfessionIndex != CharacterClass.HighMage &&
                player.ProfessionIndex != CharacterClass.Cultist)
            {
                if (minfail < 5)
                {
                    minfail = 5;
                }
            }
            if ((player.ProfessionIndex == CharacterClass.Priest || player.ProfessionIndex == CharacterClass.Druid) && player.HasUnpriestlyWeapon)
            {
                chance += 25;
            }
            if (player.ProfessionIndex == CharacterClass.Cultist && player.HasUnpriestlyWeapon)
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

        public abstract void Initialise(int characterClass);

        public string SummaryLine(Player player)
        {
            return Level >= 99
                ? "(illegible)"
                : $"{Name,-30} {Level,3} {ManaCost,4} {FailureChance(player),3}% {GetComment(player)}";
        }

        public override string ToString()
        {
            return $"{Name} ({Level}, {ManaCost}, {BaseFailure}, {FirstCastExperience})";
        }

        protected abstract string Comment(Player player);
    }
}