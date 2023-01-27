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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellMalediction : BaseSpell
    {
        public override int DefaultBaseFailure => 25;

        public override int DefaultLevel => 2;

        public override int DefaultVisCost => 2;

        public override int FirstCastExperience => 4;

        public override string Name => "Malediction";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectHellFire(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 3), 0);
            if (Program.Rng.DieRoll(5) != 1)
            {
                return;
            }
            int dummy = Program.Rng.DieRoll(1000);
            if (dummy == 666)
            {
                saveGame.SpellEffects.FireBolt(new ProjectDeathRay(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            if (dummy < 500)
            {
                saveGame.SpellEffects.FireBolt(new ProjectTurnAll(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            if (dummy < 800)
            {
                saveGame.SpellEffects.FireBolt(new ProjectOldConf(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            saveGame.SpellEffects.FireBolt(new ProjectStun(SaveGame.Instance.SpellEffects), dir, player.Level);
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 + ((player.Level - 1) / 5)}d3";
        }
    }
}