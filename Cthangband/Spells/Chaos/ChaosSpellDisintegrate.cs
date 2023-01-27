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
    internal class ChaosSpellDisintegrate : BaseSpell
    {
        public override int DefaultBaseFailure => 85;

        public override int DefaultLevel => 25;

        public override int DefaultVisCost => 25;

        public override int FirstCastExperience => 100;

        public override string Name => "Disintegrate";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), dir, 80 + player.Level,
                3 + (player.Level / 40));
        }

        protected override string Comment(Player player)
        {
            return $"dam {80 + player.Level}";
        }
    }
}