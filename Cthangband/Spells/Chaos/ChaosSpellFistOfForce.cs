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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFistOfForce : BaseSpell
    {
        public override int DefaultBaseFailure => 45;

        public override int DefaultLevel => 14;

        public override int DefaultVisCost => 9;

        public override int FirstCastExperience => 6;

        public override string Name => "Fist of Force";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8), 0);
        }

        protected override string Comment(Player player)
        {
            return $"dam {8 + ((player.Level - 5) / 4)}d8";
        }
    }
}