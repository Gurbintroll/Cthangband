using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Go up a staircase
    /// </summary>
    [Serializable]
    internal class GoUpCommand : ICommand
    {
        public char Key => '<';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            // We need to actually be on an up staircase
            GridTile tile = level.Grid[player.MapY][player.MapX];
            if (tile.FeatureType.Name != "UpStair")
            {
                Profile.Instance.MsgPrint("I see no up staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            // Use no energy, so monsters in the new level don't get to go first
            SaveGame.Instance.EnergyUse = 0;
            // If we're outside then we must be entering a tower
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[player.WildernessY][player.WildernessX].Dungeon;
                Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
            }
            else
            {
                Profile.Instance.MsgPrint("You enter a maze of up staircases.");
            }
            // Autosave, just in case
            SaveGame.Instance.IsAutosave = true;
            SaveGame.Instance.DoCmdSaveGame();
            SaveGame.Instance.IsAutosave = false;
            // In a tower, going up increases our level number
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                // Make sure we don't go past a quest level
                for (int i = 0; i < stairLength; i++)
                {
                    SaveGame.Instance.CurrentDepth++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
                    {
                        break;
                    }
                }
                // Make sure we don't go deeper than the dungeon depth
                if (SaveGame.Instance.CurrentDepth > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.CurrentDepth = SaveGame.Instance.CurDungeon.MaxLevel;
                }
            }
            else
            {
                // We're not in a tower, so going up decreases our level number
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.CurrentDepth)
                {
                    j = 1;
                }
                SaveGame.Instance.CurrentDepth -= j;
                if (SaveGame.Instance.CurrentDepth < 0)
                {
                    SaveGame.Instance.CurrentDepth = 0;
                }
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            SaveGame.Instance.NewLevelFlag = true;
            SaveGame.Instance.CreateDownStair = true;
        }
    }
}
