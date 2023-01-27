// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellVisBurst : BaseSpell
    {
        public override int DefaultBaseFailure => 50;

        public override int DefaultLevel => 9;

        public override int DefaultVisCost => 6;

        public override int FirstCastExperience => 1;

        public override string Name => "Vis Burst";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 5) + player.Level + (player.Level / player.PlayerClass.SpellBallSizeFactor),
                player.Level < 30 ? 2 : 3);
        }

        protected override string Comment(Player player)
        {
            int i = player.Level + (player.Level / player.PlayerClass.SpellBallSizeFactor);
            return $"dam 3d5+{i}";
        }
    }
}