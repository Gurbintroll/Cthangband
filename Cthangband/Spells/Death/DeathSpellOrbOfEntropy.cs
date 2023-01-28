// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellOrbOfEntropy : BaseSpell
    {
        public override int DefaultBaseFailure => 40;

        public override int DefaultLevel => 12;

        public override int DefaultVisCost => 12;

        public override int FirstCastExperience => 5;

        public override string Name => "Orb of Entropy";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectOldDrain(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 6) + player.Level + (player.Level / player.PlayerClass.SpellBallSizeFactor), player.Level < 30 ? 2 : 3);
        }

        protected override string Comment(Player player)
        {
            var s = player.Level + (player.Level / player.PlayerClass.SpellBallSizeFactor);
            return $"dam 3d6+{s}";
        }
    }
}