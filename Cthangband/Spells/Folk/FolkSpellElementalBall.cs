// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using Cthangband.Projection.Base;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellElementalBall : BaseSpell
    {
        public override int DefaultBaseFailure => 66;

        public override int DefaultLevel => 41;

        public override int DefaultVisCost => 30;

        public override int FirstCastExperience => 30;

        public override string Name => "Elemental Ball";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            IProjection dummy;
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    dummy = new ProjectFire(SaveGame.Instance.SpellEffects);
                    break;

                case 2:
                    dummy = new ProjectElectricity(SaveGame.Instance.SpellEffects);
                    break;

                case 3:
                    dummy = new ProjectCold(SaveGame.Instance.SpellEffects);
                    break;

                default:
                    dummy = new ProjectAcid(SaveGame.Instance.SpellEffects);
                    break;
            }
            saveGame.SpellEffects.FireBall(dummy, dir, 75 + player.Level, 2);
        }

        protected override string Comment(Player player)
        {
            return $"dam {75 + player.Level}";
        }
    }
}