// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDispelUndeadAndDemons : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelUndead(player.Level * 3);
            saveGame.SpellEffects.DispelDemons(player.Level * 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Dispel Undead & Demons";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                    Level = 17;
                    ManaCost = 14;
                    BaseFailure = 55;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Paladin:
                    Level = 30;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 34;
                    ManaCost = 34;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 50;
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
            return $"dam {3 * player.Level}+{3 * player.Level}";
        }
    }
}