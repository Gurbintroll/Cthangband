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
    internal class FolkSpellTrapAndDoorDestruction : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.DestroyDoor(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Trap & Door Destruction";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Priest:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Rogue:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Ranger:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 23;
                    FirstCastExperience = 7;
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