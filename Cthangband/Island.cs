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
    [Serializable]
    internal class Island
    {
        private readonly WildernessRegion[][] _array;
        private byte[][] _elevationMap;

        public Island()
        {
            _array = new WildernessRegion[12][];
            for (int i = 0; i < 12; i++)
            {
                _array[i] = new WildernessRegion[12];
                for (int j = 0; j < 12; j++)
                {
                    _array[i][j] = new WildernessRegion();
                }
            }
        }

        public WildernessRegion[] this[int index] => _array[index];

        public byte Elevation(int wildY, int wildX, int y, int x)
        {
            return _elevationMap[((wildY - 1) * (Constants.WildernessHeight - 1)) + y + 1][
                ((wildX - 1) * (Constants.WildernessWidth - 1)) + x + 1];
        }

        public void MakeIslandContours()
        {
            bool reject = false;
            do
            {
                PerlinNoise perlinNoise = new PerlinNoise(Program.Rng.RandomLessThan(int.MaxValue - 1));
                const int mapWidth = (10 * (Constants.WildernessWidth - 1)) + 3;
                const int mapHeight = (10 * (Constants.WildernessHeight - 1)) + 3;
                const double widthDivisor = 1 / (double)mapWidth;
                const double heightDivisor = 1 / (double)mapHeight;
                _elevationMap = new byte[mapHeight][];
                for (int row = 0; row < mapHeight; row++)
                {
                    _elevationMap[row] = new byte[mapWidth];
                }
                for (int row = 0; row < mapHeight; row++)
                {
                    for (int col = 0; col < mapWidth; col++)
                    {
                        double v = perlinNoise.Noise(10 * col * widthDivisor, 10 * row * heightDivisor, -0.5);
                        v = (v + 1) / 2;
                        double dX = Math.Abs(col - (mapWidth / 2)) * widthDivisor;
                        double dY = Math.Abs(row - (mapHeight / 2)) * heightDivisor;
                        double d = Math.Max(dX, dY);
                        const double elevation = 0.05;
                        const double steepness = 6.0;
                        const double dropoff = 50.0;
                        v += elevation - (dropoff * Math.Pow(d, steepness));
                        v = Math.Min(1, Math.Max(0, v));
                        byte rounded = (byte)(v * 10);
                        _elevationMap[row][col] = rounded;
                    }
                }
                for (int row = 0; row < mapHeight; row++)
                {
                    if (_elevationMap[row][1] != 0)
                    {
                        reject = true;
                    }
                    if (_elevationMap[row][mapWidth - 2] != 0)
                    {
                        reject = true;
                    }
                }
                for (int col = 0; col < mapWidth; col++)
                {
                    if (_elevationMap[1][col] != 0)
                    {
                        reject = true;
                    }
                    if (_elevationMap[mapHeight - 2][col] != 0)
                    {
                        reject = true;
                    }
                }
                if (reject)
                {
                    continue;
                }
            } while (reject);
        }
    }
}