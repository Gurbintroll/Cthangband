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

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellRemoveCurse : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.RemoveCurse();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Remove Curse";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 16;
                    VisCost = 16;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    VisCost = 6;
                    BaseFailure = 38;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Paladin:
                    Level = 14;
                    VisCost = 11;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 18;
                    VisCost = 18;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    VisCost = 12;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
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