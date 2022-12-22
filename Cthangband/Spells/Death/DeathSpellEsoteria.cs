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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellEsoteria : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(50) > player.Level)
            {
                saveGame.SpellEffects.IdentifyItem();
            }
            else
            {
                saveGame.SpellEffects.IdentifyFully();
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Esoteria";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 30;
                    VisCost = 40;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Priest:
                    Level = 35;
                    VisCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Rogue:
                    Level = 32;
                    VisCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Ranger:
                    Level = 40;
                    VisCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Paladin:
                    Level = 38;
                    VisCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 35;
                    VisCost = 45;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.HighMage:
                    Level = 26;
                    VisCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
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