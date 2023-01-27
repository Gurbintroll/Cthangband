// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellVampiricDrain : BaseSpell
    {
        public override int DefaultBaseFailure => 60;

        public override int DefaultLevel => 23;

        public override int DefaultVisCost => 20;

        public override int FirstCastExperience => 16;

        public override string Name => "Vampiric Drain";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            int dummy = player.Level + (Program.Rng.DieRoll(player.Level) * Math.Max(1, player.Level / 10));
            if (!SaveGame.Instance.SpellEffects.DrainLife(dir, dummy))
            {
                return;
            }
            player.RestoreHealth(dummy);
            dummy = player.Food + Math.Min(5000, 100 * dummy);
            if (player.Food < Constants.PyFoodMax)
            {
                player.SetFood(dummy >= Constants.PyFoodMax ? Constants.PyFoodMax - 1 : dummy);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {Math.Max(1, player.Level / 10)}d{player.Level}+{player.Level}";
        }
    }
}