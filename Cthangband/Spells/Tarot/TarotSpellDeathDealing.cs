// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellDeathDealing : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelLiving(player.Level * 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Death Dealing";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 50;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 45;
                    ManaCost = 55;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Rogue:
                    Level = 48;
                    ManaCost = 75;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 55;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 39;
                    ManaCost = 45;
                    BaseFailure = 40;
                    FirstCastExperience = 75;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {player.Level * 3}";
        }
    }
}