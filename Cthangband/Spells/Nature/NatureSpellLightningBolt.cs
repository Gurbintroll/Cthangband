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
    internal class NatureSpellLightningBolt : BaseSpell
    {
        public override int DefaultBaseFailure => 30;

        public override int DefaultLevel => 5;

        public override int DefaultVisCost => 5;

        public override int FirstCastExperience => 6;

        public override string Name => "Lightning Bolt";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam = player.PlayerClass.SpellBeamChance(player.Level);
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectElectricity(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 5) / 4), 8));
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 + ((player.Level - 5) / 4)}d8";
        }
    }
}