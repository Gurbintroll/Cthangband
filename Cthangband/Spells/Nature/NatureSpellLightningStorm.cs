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
            SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 90 + player.Level, (player.Level / 12) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Lightning Storm";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 30;
                    VisCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClassId.Priest:
                    Level = 32;
                    VisCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClassId.Ranger:
                    Level = 32;
                    VisCost = 29;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 33;
                    VisCost = 33;
                    BaseFailure = 75;
                    FirstCastExperience = 35;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 28;
                    VisCost = 25;
                    BaseFailure = 65;
                    FirstCastExperience = 35;
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
            return $"dam {90 + player.Level}";
        }
    }
}