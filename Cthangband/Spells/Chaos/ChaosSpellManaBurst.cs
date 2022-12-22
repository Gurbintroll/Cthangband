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
    internal class ChaosSpellVisBurst : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 5) + player.Level + (player.Level /
                (player.CharacterClassIndex == CharacterClassId.Mage || player.CharacterClassIndex == CharacterClassId.HighMage ? 2 : 4)),
                player.Level < 30 ? 2 : 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vis Burst";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 9;
                    VisCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 1;
                    break;

                case CharacterClassId.Priest:
                    Level = 10;
                    VisCost = 6;
                    BaseFailure = 30;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 14;
                    VisCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 7;
                    VisCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 6;
                    VisCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 1;
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
            int i = player.Level + (player.Level /
                    (player.CharacterClassIndex == CharacterClassId.Mage || player.CharacterClassIndex == CharacterClassId.HighMage ? 2 : 4));
            return $"dam 3d5+{i}";
        }
    }
}