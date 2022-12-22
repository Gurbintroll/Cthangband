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

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellWordOfRecall : BaseSpell
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
                case CharacterClassId.Mage:
                    Level = 45;
                    VisCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Priest:
                    Level = 47;
                    VisCost = 55;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Rogue:
                    Level = 49;
                    VisCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Ranger:
                    Level = 49;
                    VisCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 48;
                    VisCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.HighMage:
                    Level = 43;
                    VisCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
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