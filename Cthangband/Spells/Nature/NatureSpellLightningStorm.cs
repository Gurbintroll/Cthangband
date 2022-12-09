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

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellLightningStorm : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 90 + player.Level, (player.Level / 12) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Lightning Storm";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    VrilCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Priest:
                    Level = 32;
                    VrilCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClass.Ranger:
                    Level = 32;
                    VrilCost = 29;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 33;
                    VrilCost = 33;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 28;
                    VrilCost = 25;
                    BaseFailure = 65;
                    FirstCastExperience = 35;
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
            return $"dam {90 + player.Level}";
        }
    }
}