using Cthangband.Debug;
using Cthangband.Enumerations;
using System;
using System.ComponentModel;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class FloorTileType : EntityType
    {
        [Browsable(true)]
        [Category("Misc")]
        [Description("The action to be taken when this tile is altered")]
        public FloorTileAlterAction AlterAction
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Display")]
        [Description("The the tile this one should appear to be when looked at")]
        public string AppearAs
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile blocks line of sight")]
        public bool BlocksLos
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Misc")]
        [Description("The category of this tile")]
        public FloorTileTypeCategory Category
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Misc")]
        [Description("A text description of the tile")]
        public string Description
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Display")]
        [Description("The the tile this one should appear to be when looked at")]
        public bool DimsOutsideLOS
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is a basic (not permanent) dungeon wall")]
        public bool IsBasicWall
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is a closed door (not including a secret door")]
        public bool IsClosedDoor
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is 'interesting' and should be noticed when the player looks around")]
        public bool IsInteresting
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is open floor safe to drop items on")]
        public bool IsOpenFloor
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("A text description of the tile")]
        public bool IsPassable
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("A text description of the tile")]
        public bool IsPermanent
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is a shop entrance")]
        public bool IsShop
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is a known trap")]
        public bool IsTrap
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The tile is a wall (not including a secret door)")]
        public bool IsWall
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Display")]
        [Description("The priority on the map")]
        public int MapPriority
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Flags")]
        [Description("The player will run past the tile rather than stopping at it")]
        public bool RunPast
        {
            get; set;
        }

        [Browsable(true)]
        [Category("Display")]
        [Description("The the tile this one should appear to be when looked at")]
        public bool YellowInTorchlight
        {
            get; set;
        }
    }
}