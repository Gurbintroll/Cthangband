// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDarknessStorm : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectDark(SaveGame.Instance.SpellEffects), dir, 120, 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Darkness Storm";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    VisCost = 40;
                    BaseFailure = 70;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 44;
                    VisCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Rogue:
                    Level = 50;
                    VisCost = 50;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 48;
                    VisCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    VisCost = 50;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 36;
                    VisCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 200;
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
            return "dam 120";
        }
    }
}