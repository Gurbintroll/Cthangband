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
    internal class NatureSpellNaturesWrath : BaseSpell
    {
        public override int DefaultBaseFailure => 95;

        public override int DefaultLevel => 40;

        public override int DefaultVisCost => 90;

        public override int FirstCastExperience => 200;

        public override string Name => "Nature’s Wrath";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 20 + (player.Level / 2));
            saveGame.SpellEffects.Project(0, 1 + (player.Level / 12), player.MapY, player.MapX, 100 + player.Level,
                new ProjectDisintegrate(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectItem);
        }

        protected override string Comment(Player player)
        {
            return $"dam {4 * player.Level}+{100 + player.Level}";
        }
    }
}