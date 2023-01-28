// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.PlayerClass.Base;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMinorDisplacement : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Minor Displacement";
            Level = 3;
            VisCost = 2;
            BaseFailure = 25;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            var targetEngine = new TargetEngine(player, level);
            if (player.Level < 25)
            {
                saveGame.SpellEffects.TeleportPlayer(10);
            }
            else
            {
                Profile.Instance.MsgPrint("Choose a destination.");
                if (!targetEngine.TgtPt(out var i, out var j))
                {
                    return;
                }
                player.Energy -= 60 - player.Level;
                if (!level.GridPassableNoCreature(j, i) || level.Grid[j][i].TileFlags.IsSet(GridTile.InVault) ||
                    level.Grid[j][i].FeatureType.Name != "Water" ||
                    level.Distance(j, i, player.MapY, player.MapX) > player.Level + 2 ||
                    Program.Rng.RandomLessThan(player.Level * player.Level / 2) == 0)
                {
                    Profile.Instance.MsgPrint("Something disrupts your concentration!");
                    player.Energy -= 100;
                    SaveGame.Instance.SpellEffects.TeleportPlayer(20);
                }
                else
                {
                    SaveGame.Instance.SpellEffects.TeleportPlayerTo(j, i);
                }
            }
        }

        protected override string Comment(Player player)
        {
            return $"range {(player.Level < 25 ? 10 : player.Level + 2)}";
        }
    }
}