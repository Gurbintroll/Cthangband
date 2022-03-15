// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellWordOfRecall : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.ToggleRecall();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Word of Recall";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 47;
                    ManaCost = 55;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 49;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 49;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 43;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
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
            return "delay 15+d21";
        }
    }
}