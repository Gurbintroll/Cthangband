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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellInvokeChaos : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), dir, 66 + player.Level, player.Level / 5);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Invoke Chaos";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 35;
                    VisCost = 40;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Priest:
                    Level = 37;
                    VisCost = 42;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Ranger:
                    Level = 48;
                    VisCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 30;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 48;
                    VisCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 40;
                    VisCost = 45;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 30;
                    VisCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
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
            return $"dam {66 + player.Level}";
        }
    }
}