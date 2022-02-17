using Cthangband.Debug;

namespace Cthangband.StaticData
{
    internal class BaseVaultType : EntityType
    {
        public int Category
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Rating
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }
}