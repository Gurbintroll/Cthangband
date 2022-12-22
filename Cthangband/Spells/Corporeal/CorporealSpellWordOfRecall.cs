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

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellWordOfRecall : BaseSpell
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
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClassId.Priest:
                    Level = 27;
                    VisCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClassId.Ranger:
                    Level = 35;
                    VisCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 28;
                    VisCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClassId.HighMage:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 19;
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