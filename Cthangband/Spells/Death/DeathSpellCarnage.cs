// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellCarnage : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Carnage(true);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Carnage";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    ManaCost = 25;
                    BaseFailure = 95;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 45;
                    ManaCost = 35;
                    BaseFailure = 95;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 44;
                    ManaCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 25;
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
            return string.Empty;
        }
    }
}