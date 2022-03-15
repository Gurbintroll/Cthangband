// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellRemoveCurse : Spell
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
                    ManaCost = 16;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 38;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Paladin:
                    Level = 14;
                    ManaCost = 11;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 18;
                    ManaCost = 18;
                    BaseFailure = 45;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
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