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
    internal class FolkSpellBlink : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Blink";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 2;
                    VrilCost = 2;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    VrilCost = 3;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 6;
                    VrilCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    VrilCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    VrilCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    VrilCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 5;
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
            return "range 10";
        }
    }
}