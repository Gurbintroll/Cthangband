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
    internal class TarotSpellWordOfRecall : BaseSpell
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
                    Level = 40;
                    VrilCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 42;
                    VrilCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Rogue:
                    Level = 46;
                    VrilCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Ranger:
                    Level = 45;
                    VrilCost = 42;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 44;
                    VrilCost = 42;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                    Level = 35;
                    VrilCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
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