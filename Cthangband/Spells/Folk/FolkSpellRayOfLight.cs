// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellRayOfLight : BaseSpell
    {
        public override int DefaultBaseFailure => 60;

        public override int DefaultLevel => 23;

        public override int DefaultVisCost => 18;

        public override int FirstCastExperience => 9;

        public override string Name => "Ray of Light";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            Profile.Instance.MsgPrint("A line of light appears.");
            saveGame.SpellEffects.LightLine(dir);
        }

        protected override string Comment(Player player)
        {
            return "dam 6d8";
        }
    }
}