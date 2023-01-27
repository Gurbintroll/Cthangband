// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellMeteorSwarm : BaseSpell
    {
        public override int DefaultBaseFailure => 85;

        public override int DefaultLevel => 35;

        public override int DefaultVisCost => 32;

        public override int FirstCastExperience => 35;

        public override string Name => "Meteor Swarm";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int x = player.MapX;
            int y = player.MapY;
            int count = 0;
            int b = 10 + Program.Rng.DieRoll(10);
            for (int i = 0; i < b; i++)
            {
                int d;
                do
                {
                    count++;
                    if (count > 1000)
                    {
                        break;
                    }
                    x = player.MapX - 5 + Program.Rng.DieRoll(10);
                    y = player.MapY - 5 + Program.Rng.DieRoll(10);
                    int dx = player.MapX > x ? player.MapX - x : x - player.MapX;
                    int dy = player.MapY > y ? player.MapY - y : y - player.MapY;
                    d = dy > dx ? dy + (dx >> 1) : dx + (dy >> 1);
                } while (d > 5 || !level.PlayerHasLosBold(y, x));
                if (count > 1000)
                {
                    break;
                }
                count = 0;
                saveGame.SpellEffects.Project(0, 2, y, x, player.Level * 3 / 2, new ProjectMeteor(SaveGame.Instance.SpellEffects),
                    ProjectionFlag.ProjectKill | ProjectionFlag.ProjectJump | ProjectionFlag.ProjectItem);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 * player.Level / 2} each";
        }
    }
}