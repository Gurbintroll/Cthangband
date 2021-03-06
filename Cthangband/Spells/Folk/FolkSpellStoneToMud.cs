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
    internal class FolkSpellStoneToMud : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.WallToMud(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stone to Mud";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 16;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 23;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 9;
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