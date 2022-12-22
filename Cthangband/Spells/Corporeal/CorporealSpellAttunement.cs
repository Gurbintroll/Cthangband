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
    internal class CorporealSpellAttunement : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Attunement";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 25;
                    VisCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClassId.Priest:
                    Level = 27;
                    VisCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClassId.Ranger:
                    Level = 37;
                    VisCost = 60;
                    BaseFailure = 95;
                    FirstCastExperience = 120;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 30;
                    VisCost = 35;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClassId.HighMage:
                    Level = 20;
                    VisCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 160;
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
            return string.Empty;
        }
    }
}