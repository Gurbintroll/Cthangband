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
        public override int DefaultBaseFailure => 75;

        public override int DefaultLevel => 30;

        public override int DefaultVisCost => 27;

        public override int FirstCastExperience => 35;

        public override string Name => "Lightning Storm";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 90 + player.Level, (player.Level / 12) + 1);
        }

        protected override string Comment(Player player)
        {
            return $"dam {90 + player.Level}";
        }
    }
}