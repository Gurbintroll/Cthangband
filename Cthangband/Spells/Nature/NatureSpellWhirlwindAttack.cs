// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellWhirlwindAttack : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 23;

        public override int DefaultVisCost => 23;

        public override int FirstCastExperience => 50;

        public override string Name => "Whirlwind Attack";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            for (int dir = 0; dir <= 9; dir++)
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile cPtr = level.Grid[y][x];
                Monster mPtr = level.Monsters[cPtr.MonsterIndex];
                if (cPtr.MonsterIndex != 0 && (mPtr.IsVisible || level.GridPassable(y, x)))
                {
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}