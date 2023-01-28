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
    internal class DeathSpellNetherBolt : BaseSpell
    {
        public override int DefaultBaseFailure => 30;

        public override int DefaultLevel => 13;

        public override int DefaultVisCost => 12;

        public override int FirstCastExperience => 4;

        public override string Name => "Nether Bolt";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var beam = player.PlayerClass.SpellBeamChance(player.Level);
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectNether(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(6 + ((player.Level - 5) / 4), 8));
        }

        protected override string Comment(Player player)
        {
            return $"dam {6 + ((player.Level - 5) / 4)}d8";
        }
    }
}