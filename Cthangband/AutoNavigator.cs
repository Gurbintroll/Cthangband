// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband
{
    /// <summary>
    /// Class for automatically navigating when the player is running
    /// </summary>
    [Serializable]
    internal class AutoNavigator
    {
        /// <summary>
        /// The direction the player is currently running in
        /// </summary>
        public int CurrentRunDirection;

        /// <summary>
        /// A list of entry points into the _directionCycle for each direction (except 0 or 5) that
        /// take you to a middling entry for that direction from which we can safely rotate to
        /// either side
        /// </summary>
        private readonly int[] _cycleEntryPoint = { 0, 8, 9, 10, 7, 0, 11, 6, 5, 4 };

        /// <summary>
        /// A list of keypad directions which rotates left when you increment and right when you decrement
        /// </summary>
        private readonly int[] _directionCycle = { 1, 2, 3, 6, 9, 8, 7, 4, 1, 2, 3, 6, 9, 8, 7, 4, 1 };

        private bool _findBreakleft;
        private bool _findBreakright;
        private bool _findOpenarea;

        /// <summary>
        /// The direction in which we were previously running
        /// </summary>
        private int _previousRunDirection;

        /// <summary>
        /// Initialise the navigator with no current direction
        /// </summary>
        public AutoNavigator()
        {
            CurrentRunDirection = 5;
            _previousRunDirection = 5;
            _findOpenarea = true;
            _findBreakright = false;
            _findBreakleft = false;
        }

        /// <summary>
        /// Initialise the navigator with a direction
        /// </summary>
        /// <param name="direction"> The direction in which we wish to run </param>
        public AutoNavigator(int direction)
        {
            CurrentRunDirection = direction;
            _previousRunDirection = direction;
            _findOpenarea = true;
            _findBreakright = false;
            _findBreakleft = false;
            bool wallDoubleAheadLeft = false;
            bool wallDoubleAheadRight = false;
            bool wallAheadRight = false;
            bool wallAheadLeft = false;
            var player = SaveGame.Instance.Player;
            // Get the row and column of the first step in the run
            int row = player.MapY + SaveGame.Instance.Level.KeypadDirectionYOffset[direction];
            int col = player.MapX + SaveGame.Instance.Level.KeypadDirectionXOffset[direction];
            // Get the index of our run direction in the cycle
            int cycleIndex = _cycleEntryPoint[direction];
            // If there's a wall ahead-left of us, remember that
            if (SeeWall(_directionCycle[cycleIndex + 1], player.MapY, player.MapX))
            {
                _findBreakleft = true;
                wallAheadLeft = true;
            }
            // Else check if there's a wall ahead-left of our first step, and remember that
            else if (SeeWall(_directionCycle[cycleIndex + 1], row, col))
            {
                _findBreakleft = true;
                wallDoubleAheadLeft = true;
            }
            // If there's a wall ahead-right of us, remember that
            if (SeeWall(_directionCycle[cycleIndex - 1], player.MapY, player.MapX))
            {
                _findBreakright = true;
                wallAheadRight = true;
            }
            // Else check if there's a wall ahead-right of our first step, and remember that
            else if (SeeWall(_directionCycle[cycleIndex - 1], row, col))
            {
                _findBreakright = true;
                wallDoubleAheadRight = true;
            }
            // If we're looking for breaks on either side, we're not looking for an open area
            if (_findBreakleft && _findBreakright)
            {
                _findOpenarea = false;
                // If we're moving orthogonally and we have a wall double-ahead on either side,
                // nudge our assumed previous direction away from it
                if ((direction & 0x01) != 0)
                {
                    if (wallDoubleAheadLeft && !wallDoubleAheadRight)
                    {
                        _previousRunDirection = _directionCycle[cycleIndex - 1];
                    }
                    else if (wallDoubleAheadRight && !wallDoubleAheadLeft)
                    {
                        _previousRunDirection = _directionCycle[cycleIndex + 1];
                    }
                }
                // If there's a wall directly ahead but not diagonally ahead, nudge our assumed
                // previous direction away from it
                else if (SeeWall(_directionCycle[cycleIndex], row, col))
                {
                    if (wallAheadLeft && !wallAheadRight)
                    {
                        _previousRunDirection = _directionCycle[cycleIndex - 2];
                    }
                    else if (wallAheadRight && !wallAheadLeft)
                    {
                        _previousRunDirection = _directionCycle[cycleIndex + 2];
                    }
                }
            }
        }

        /// <summary>
        /// Check if we can and should move forward on our current path, adjusting our path if
        /// necessary to go around corners and stopping if the way forward becomes ambiguous or we
        /// see a monster.
        /// </summary>
        /// <returns> True if we must stop running, false if we may carry on </returns>
        public bool NavigateNextStep()
        {
            int newDirection;
            int checkDir = 0;
            int row;
            int col;
            int i;
            GridTile tile;
            int option = 0;
            int option2 = 0;
            int previousDirection = _previousRunDirection;
            var level = SaveGame.Instance.Level;
            var player = SaveGame.Instance.Player;
            // Set our search width to 1 if we're moving diagonally, or two if we're moving orthogonally
            int searchWidth = (previousDirection & 0x01) + 1;
            // Search to either side from right to left with a width equal to the search width
            for (i = -searchWidth; i <= searchWidth; i++)
            {
                int nextItemIndex;
                // Pick up the tile 0-2 rotations from the direction we previously moved
                newDirection = _directionCycle[_cycleEntryPoint[previousDirection] + i];
                row = player.MapY + level.KeypadDirectionYOffset[newDirection];
                col = player.MapX + level.KeypadDirectionXOffset[newDirection];
                tile = level.Grid[row][col];
                // If there's a monster there we must stop moving
                if (tile.MonsterIndex != 0)
                {
                    Monster monster = level.Monsters[tile.MonsterIndex];
                    if (monster.IsVisible)
                    {
                        return true;
                    }
                }
                // If there's an item there we weren't previously aware of then we must stop moving
                for (int itemIndex = tile.ItemIndex; itemIndex != 0; itemIndex = nextItemIndex)
                {
                    Item item = level.Items[itemIndex];
                    nextItemIndex = item.NextInStack;
                    if (item.Marked)
                    {
                        return true;
                    }
                }
                bool tileUnseen = true;
                // If the tile is something we should not run past then we must stop moving
                if (tile.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    bool notice = !tile.FeatureType.RunPast;
                    if (notice)
                    {
                        return true;
                    }
                    tileUnseen = false;
                }
                // We can enter the tile or it's unseen
                if (tileUnseen || level.GridPassable(row, col))
                {
                    if (_findOpenarea)
                    {
                        // Ignore the open area
                    }
                    else if (option == 0)
                    {
                        // It's an option for changing direction to
                        option = newDirection;
                    }
                    // We shouldn't be finding open areas, but we did find one so we have to stop
                    else if (option2 != 0)
                    {
                        return true;
                    }
                    // We've previously found an option and it isn't just a diagonal corner-cut from
                    // this one so we have to stop
                    else if (option != _directionCycle[_cycleEntryPoint[previousDirection] + i - 1])
                    {
                        return true;
                    }
                    // If we're running diagonally then we can have a second option two away
                    else if ((newDirection & 0x01) != 0)
                    {
                        checkDir = _directionCycle[_cycleEntryPoint[previousDirection] + i - 2];
                        option2 = newDirection;
                    }
                    // We're running orthogonally so we can have a second option one away
                    else
                    {
                        checkDir = _directionCycle[_cycleEntryPoint[previousDirection] + i + 1];
                        option2 = option;
                        option = newDirection;
                    }
                }
                else
                // If we were looking for an open area we're now just looking for a left or right
                {
                    if (_findOpenarea)
                    {
                        if (i < 0)
                        {
                            _findBreakright = true;
                        }
                        else if (i > 0)
                        {
                            _findBreakleft = true;
                        }
                    }
                }
            }
            // If we're looking for an open area, search both directions
            if (_findOpenarea)
            {
                // Look left first
                for (i = -searchWidth; i < 0; i++)
                {
                    newDirection = _directionCycle[_cycleEntryPoint[previousDirection] + i];
                    row = player.MapY + level.KeypadDirectionYOffset[newDirection];
                    col = player.MapX + level.KeypadDirectionXOffset[newDirection];
                    tile = level.Grid[row][col];
                    if (tile.TileFlags.IsClear(GridTile.PlayerMemorised) || !tile.FeatureType.IsWall)
                    {
                        if (_findBreakright)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (_findBreakleft)
                        {
                            return true;
                        }
                    }
                }
                // Then look left
                for (i = searchWidth; i > 0; i--)
                {
                    newDirection = _directionCycle[_cycleEntryPoint[previousDirection] + i];
                    row = player.MapY + level.KeypadDirectionYOffset[newDirection];
                    col = player.MapX + level.KeypadDirectionXOffset[newDirection];
                    tile = level.Grid[row][col];
                    if (tile.TileFlags.IsClear(GridTile.PlayerMemorised) || !tile.FeatureType.IsWall)
                    {
                        if (_findBreakleft)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (_findBreakright)
                        {
                            return true;
                        }
                    }
                }
            }
            // Not looking for an open area
            else
            {
                // If we have nowhere else to run, we must stop
                if (option == 0)
                {
                    return true;
                }
                // If we only have one option, take it
                if (option2 == 0)
                {
                    CurrentRunDirection = option;
                    _previousRunDirection = option;
                }
                // if we don't see a wall in one of our two options, take that one
                else
                {
                    row = player.MapY + level.KeypadDirectionYOffset[option];
                    col = player.MapX + level.KeypadDirectionXOffset[option];
                    if (!SeeWall(option, row, col) || !SeeWall(checkDir, row, col))
                    {
                        if (SeeNothing(option, row, col) && SeeNothing(option2, row, col))
                        {
                            CurrentRunDirection = option;
                            _previousRunDirection = option2;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        CurrentRunDirection = option2;
                        _previousRunDirection = option2;
                    }
                }
            }
            // No options, so just return whether or not we can move forward
            return SeeWall(CurrentRunDirection, player.MapY, player.MapX);
        }

        /// <summary>
        /// Check whether we can see a wall in a certain direction from a point
        /// </summary>
        /// <param name="direction"> The direction in which we're looking </param>
        /// <param name="y"> The y coordinate of our location </param>
        /// <param name="x"> The x coordinate of our location </param>
        /// <returns> True if we can see a wall, false if not </returns>
        public bool SeeWall(int direction, int y, int x)
        {
            var level = SaveGame.Instance.Level;
            y += level.KeypadDirectionYOffset[direction];
            x += level.KeypadDirectionXOffset[direction];
            // Out of bounds is not a wall
            if (!level.InBounds2(y, x))
            {
                return false;
            }
            // Any passable grid is okay
            if (level.GridPassable(y, x))
            {
                return false;
            }
            // If we don't know what's there it's okay
            if (level.Grid[y][x].TileFlags.IsClear(GridTile.PlayerMemorised))
            {
                return false;
            }
            // It was impassable and we knew it
            return true;
        }

        /// <summary>
        /// Check if we see an open space in a given direction from a given point
        /// </summary>
        /// <param name="direction"> The direction in which we're looking </param>
        /// <param name="y"> The y coordinate of our location </param>
        /// <param name="x"> The x coordinate of our location </param>
        /// <returns> </returns>
        private bool SeeNothing(int direction, int y, int x)
        {
            var level = SaveGame.Instance.Level;
            y += level.KeypadDirectionYOffset[direction];
            x += level.KeypadDirectionXOffset[direction];
            // Out of bounds is empty
            if (!level.InBounds2(y, x))
            {
                return true;
            }
            // Unknown tiles are not empty
            if (level.Grid[y][x].TileFlags.IsSet(GridTile.PlayerMemorised))
            {
                return false;
            }
            // Anything we can walk through is empty
            if (!level.GridPassable(y, x))
            {
                return true;
            }
            // It's empty if we can't see it
            return !level.PlayerCanSeeBold(y, x);
        }
    }
}