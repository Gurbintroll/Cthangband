// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellSonicBoom : BaseSpell
    {
        public override int DefaultBaseFailure => 45;

        public override int DefaultLevel => 21;

        public override int DefaultVisCost => 13;

        public override int FirstCastExperience => 10;

        public override string Name => "Sonic Boom";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Project(0, 2 + (player.Level / 10), player.MapY, player.MapX, 45 + player.Level,
                new ProjectSound(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectItem);
        }

        protected override string Comment(Player player)
        {
            return $"dam {45 + player.Level}";
        }
    }
}