using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Use a down staircase or trapdoor
    /// </summary>
    [Serializable]
    internal class GoDownCommand : ICommand
    {
        public char Key => '>';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            bool isTrapDoor = false;
            GridTile tile = level.Grid[player.MapY][player.MapX];
            if (tile.FeatureType.Category == FloorTileTypeCategory.TrapDoor)
            {
                isTrapDoor = true;
            }
            // Need to be on a staircase or trapdoor
            if (tile.FeatureType.Name != "DownStair" && !isTrapDoor)
            {
                Profile.Instance.MsgPrint("I see no down staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            // Going onto a new level takes no energy, so the monsters on that level don't get to
            // move before us
            SaveGame.Instance.EnergyUse = 0;
            if (isTrapDoor)
            {
                Profile.Instance.MsgPrint("You deliberately jump through the trap door.");
            }
            else
            {
                // If we're on the surface, enter the relevant dungeon
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[player.WildernessY][player.WildernessX].Dungeon;
                    Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
                }
                else
                {
                    Profile.Instance.MsgPrint("You enter a maze of down staircases.");
                }
                // Save the game, just in case
                SaveGame.Instance.IsAutosave = true;
                SaveGame.Instance.DoCmdSaveGame();
                SaveGame.Instance.IsAutosave = false;
            }
            // If we're in a tower, a down staircase reduces our level number
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                SaveGame.Instance.CurrentDepth -= stairLength;
                if (SaveGame.Instance.CurrentDepth < 0)
                {
                    SaveGame.Instance.CurrentDepth = 0;
                }
                // If we left the dungeon, remember where we are
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            else
            {
                // We're not in a tower, so a down staircase increases our level number
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                // Check if we're about to go past a quest level
                for (int i = 0; i < stairLength; i++)
                {
                    SaveGame.Instance.CurrentDepth++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
                    {
                        // Stop on the quest level
                        break;
                    }
                }
                // Don't go past the max dungeon level
                if (SaveGame.Instance.CurrentDepth > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.CurrentDepth = SaveGame.Instance.CurDungeon.MaxLevel;
                }
                // From the surface we always go to the first level
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    SaveGame.Instance.CurrentDepth++;
                }
            }
            // We need a new level
            SaveGame.Instance.NewLevelFlag = true;
            if (!isTrapDoor)
            {
                // Create an up staircase if we went down a staircase
                SaveGame.Instance.CreateUpStair = true;
            }
        }
    }
}
