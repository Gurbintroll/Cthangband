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
    internal class FolkSpellDetectDoorsAndTraps : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Doors and Traps";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 7;
                    VisCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Priest:
                    Level = 8;
                    VisCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Rogue:
                    Level = 9;
                    VisCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Ranger:
                    Level = 9;
                    VisCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.HighMage:
                    Level = 6;
                    VisCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 7;
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