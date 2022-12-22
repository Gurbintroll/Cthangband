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
    internal class FolkSpellRecharging : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Recharge(player.Level * 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Recharging";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 28;
                    VisCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClassId.Priest:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Rogue:
                    Level = 38;
                    VisCost = 36;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Ranger:
                    Level = 38;
                    VisCost = 36;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 35;
                    VisCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.HighMage:
                    Level = 24;
                    VisCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 30;
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