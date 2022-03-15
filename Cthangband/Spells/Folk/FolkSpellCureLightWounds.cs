// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureLightWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 8));
            player.SetTimedBleeding(player.TimedBleeding - 10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Light Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
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
            return "heal 2d8";
        }
    }
}