// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellAnnihilation : BaseSpell
    {
        public override int DefaultBaseFailure => 95;

        public override int DefaultLevel => 44;

        public override int DefaultVisCost => 100;

        public override int FirstCastExperience => 250;

        public override string Name => "Annihilation";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.Vis -= 100;
            for (var i = 1; i < level.MMax; i++)
            {
                var mPtr = level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
                {
                    continue;
                }
                level.Monsters.DeleteMonsterByIndex(i, true);
                player.TakeHit(Program.Rng.DieRoll(4), "the strain of casting Annihilation");
                player.Vis++;
                level.MoveCursorRelative(player.MapY, player.MapX);
                player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
                SaveGame.Instance.HandleStuff();
                Gui.Refresh();
                Gui.Pause(GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor);
            }
            player.Vis += 100;
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}