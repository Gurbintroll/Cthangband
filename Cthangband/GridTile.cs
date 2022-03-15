// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    /// <summary>
    /// A single grid tile in either the dungeon, town, or wilderness
    /// </summary>
    [Serializable]
    internal class GridTile
    {
        /// <summary>
        /// Used within the view code to mark tiles that are "easily" visible
        /// </summary>
        public const int EasyVisibility = 0x0080;

        /// <summary>
        /// Set if the grid tile is part of a room
        /// </summary>
        public const int InRoom = 0x0008;

        /// <summary>
        /// Set if the grid tile is part of a vault
        /// </summary>
        public const int InVault = 0x0004;

        /// <summary>
        /// Set if the grid tile is visible to the player
        /// </summary>
        public const int IsVisible = 0x0020;

        /// <summary>
        /// Set if the grid tile is currently lit by the player's light source
        /// </summary>
        public const int PlayerLit = 0x0010;

        /// <summary>
        /// Set if the player should remember the grid's contents
        /// </summary>
        public const int PlayerMemorised = 0x0001;

        /// <summary>
        /// Set if the grid tile is lit independently of the player's light source
        /// </summary>
        public const int SelfLit = 0x0002;

        /// <summary>
        /// Used within the view and light code to mark tiles that might need a redraw
        /// </summary>
        public const int TempFlag = 0x0040;

        /// <summary>
        /// Set if the grid tile has had a Detect Traps used on it
        /// </summary>
        public const int TrapsDetected = 0x0100;

        /// <summary>
        /// Flags for the tile
        /// </summary>
        public readonly FlagSet TileFlags = new FlagSet();

        /// <summary>
        /// The type of feature in this grid tile
        /// </summary>
        public FloorTileType BackgroundFeature = StaticResources.Instance.FloorTileTypes["Nothing"];

        /// <summary>
        /// The type of feature in this grid tile
        /// </summary>
        public FloorTileType FeatureType = StaticResources.Instance.FloorTileTypes["Nothing"];

        /// <summary>
        /// The index of the first item that is in this grid tile
        /// </summary>
        public int ItemIndex;

        /// <summary>
        /// The index of the monster that is in this grid tile
        /// </summary>
        public int MonsterIndex;

        /// <summary>
        /// The time since the player's scent in the tile was calculated
        /// </summary>
        public int ScentAge;

        /// <summary>
        /// The strength of the player's scent in this tile
        /// </summary>
        public int ScentStrength;

        public void RevertToBackground()
        {
            FeatureType = BackgroundFeature;
        }

        public void SetBackgroundFeature(string name)
        {
            BackgroundFeature = StaticResources.Instance.FloorTileTypes[name];
        }

        /// <summary>
        /// Sets the FeatureType of the tile to a named FloorTileType
        /// </summary>
        /// <param name="name"> </param>
        public void SetFeature(string name)
        {
            FeatureType = StaticResources.Instance.FloorTileTypes[name];
        }

        public override string ToString()
        {
            if (FeatureType == null)
            {
                return "Null Feature";
            }
            else
            {
                return FeatureType.Name;
            }
        }
    }
}