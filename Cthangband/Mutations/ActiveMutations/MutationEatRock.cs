// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationEatRock : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(8, 12, Ability.Constitution, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetRepDir(out int dir))
            {
                return;
            }
            int y = player.MapY + level.KeypadDirectionYOffset[dir];
            int x = player.MapX + level.KeypadDirectionXOffset[dir];
            GridTile cPtr = level.Grid[y][x];
            if (level.GridPassable(y, x))
            {
                Profile.Instance.MsgPrint("You bite into thin air!");
                return;
            }
            if (cPtr.FeatureType.IsPermanent)
            {
                Profile.Instance.MsgPrint("Ouch!  This wall is harder than your teeth!");
                return;
            }
            if (cPtr.MonsterIndex != 0)
            {
                Profile.Instance.MsgPrint("There's something in the way!");
                return;
            }
            if (cPtr.FeatureType.Category == FloorTileTypeCategory.Tree)
            {
                Profile.Instance.MsgPrint("You don't like the woody taste!");
                return;
            }
            if (cPtr.FeatureType.IsClosedDoor || cPtr.FeatureType.Category == FloorTileTypeCategory.SecretDoor || cPtr.FeatureType.Category == FloorTileTypeCategory.Rubble)
            {
                player.SetFood(player.Food + 3000);
            }
            else if (cPtr.FeatureType.Category == FloorTileTypeCategory.Vein)
            {
                player.SetFood(player.Food + 5000);
            }
            else
            {
                Profile.Instance.MsgPrint("This granite is very filling!");
                player.SetFood(player.Food + 10000);
            }
            saveGame.SpellEffects.WallToMud(dir);
            int oy = player.MapY;
            int ox = player.MapX;
            player.MapY = y;
            player.MapX = x;
            level.RedrawSingleLocation(player.MapY, player.MapX);
            level.RedrawSingleLocation(oy, ox);
            targetEngine.RecenterScreenAroundPlayer();
            player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 8 ? "eat rock         (unusable until level 8)" : "eat rock         (cost 12, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "The walls look delicious.";
            HaveMessage = "You can consume solid rock.";
            LoseMessage = "The walls look unappetizing.";
        }
    }
}