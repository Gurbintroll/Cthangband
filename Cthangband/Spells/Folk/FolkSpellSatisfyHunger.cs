// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellSatisfyHunger : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetFood(Constants.PyFoodMax - 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Satisfy Hunger";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 24;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Rogue:
                    Level = 29;
                    ManaCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Ranger:
                    Level = 29;
                    ManaCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 29;
                    ManaCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 16;
                    BaseFailure = 60;
                    FirstCastExperience = 12;
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