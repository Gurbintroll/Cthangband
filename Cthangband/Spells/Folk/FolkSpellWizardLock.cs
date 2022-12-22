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
    internal class FolkSpellWizardLock : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.WizardLock(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Wizard Lock";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Rogue:
                    Level = 5;
                    VisCost = 2;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 3;
                    VisCost = 2;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 2;
                    VisCost = 1;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                    Level = 1;
                    VisCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 5;
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