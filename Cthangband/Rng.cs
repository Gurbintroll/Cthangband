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
    /// A wrapper for Random
    /// </summary>
    [Serializable]
    internal class Rng
    {
        /// <summary>
        /// Set true to use the fixed seed, and false to use the generic randomiser
        /// </summary>
        public bool UseFixed = false;

        private const int _randnorNum = 256;
        private const int _randnorStd = 64;
        private readonly Random _mainSequence = new Random();

        private readonly int[] _randnorTable =
        {
            206, 613, 1022, 1430, 1838, 2245, 2652, 3058, 3463, 3867, 4271, 4673, 5075, 5475, 5874, 6271, 6667, 7061,
            7454, 7845, 8234, 8621, 9006, 9389, 9770, 10148, 10524, 10898, 11269, 11638, 12004, 12367, 12727, 13085,
            13440, 13792, 14140, 14486, 14828, 15168, 15504, 15836, 16166, 16492, 16814, 17133, 17449, 17761, 18069,
            18374, 18675, 18972, 19266, 19556, 19842, 20124, 20403, 20678, 20949, 21216, 21479, 21738, 21994, 22245,
            22493, 22737, 22977, 23213, 23446, 23674, 23899, 24120, 24336, 24550, 24759, 24965, 25166, 25365, 25559,
            25750, 25937, 26120, 26300, 26476, 26649, 26818, 26983, 27146, 27304, 27460, 27612, 27760, 27906, 28048,
            28187, 28323, 28455, 28585, 28711, 28835, 28955, 29073, 29188, 29299, 29409, 29515, 29619, 29720, 29818,
            29914, 30007, 30098, 30186, 30272, 30356, 30437, 30516, 30593, 30668, 30740, 30810, 30879, 30945, 31010,
            31072, 31133, 31192, 31249, 31304, 31358, 31410, 31460, 31509, 31556, 31601, 31646, 31688, 31730, 31770,
            31808, 31846, 31882, 31917, 31950, 31983, 32014, 32044, 32074, 32102, 32129, 32155, 32180, 32205, 32228,
            32251, 32273, 32294, 32314, 32333, 32352, 32370, 32387, 32404, 32420, 32435, 32450, 32464, 32477, 32490,
            32503, 32515, 32526, 32537, 32548, 32558, 32568, 32577, 32586, 32595, 32603, 32611, 32618, 32625, 32632,
            32639, 32645, 32651, 32657, 32662, 32667, 32672, 32677, 32682, 32686, 32690, 32694, 32698, 32702, 32705,
            32708, 32711, 32714, 32717, 32720, 32722, 32725, 32727, 32729, 32731, 32733, 32735, 32737, 32739, 32740,
            32742, 32743, 32745, 32746, 32747, 32748, 32749, 32750, 32751, 32752, 32753, 32754, 32755, 32756, 32757,
            32757, 32758, 32758, 32759, 32760, 32760, 32761, 32761, 32761, 32762, 32762, 32763, 32763, 32763, 32764,
            32764, 32764, 32764, 32765, 32765, 32765, 32765, 32766, 32766, 32766, 32766, 32767
        };

        private Random _fixed = new Random();
        private int _fixedSeed;

        public int FixedSeed
        {
            get => _fixedSeed;
            set
            {
                _fixed = new Random(value);
                _fixedSeed = value;
            }
        }

        /// <summary>
        /// Returns a dice roll of (num)d(sides)
        /// </summary>
        /// <param name="num"> The number of dice to roll </param>
        /// <param name="sides"> The number of sides per die </param>
        /// <returns> </returns>
        public int DiceRoll(int num, int sides)
        {
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += DieRoll(sides);
            }
            return sum;
        }

        /// <summary>
        /// Returns the maximum value of a dice roll of (num)d(sides)
        /// </summary>
        /// <param name="num"> The number of dice to roll </param>
        /// <param name="sides"> The number of sides per die </param>
        /// <returns> </returns>
        public int DiceRollMax(int num, int sides)
        {
            return num * sides;
        }

        /// <summary>
        /// Returns a random number greater or equal to one and less than or equal to max
        /// </summary>
        /// <param name="max"> The upper limit (inclusive) </param>
        /// <returns> A random number </returns>
        public int DieRoll(int max)
        {
            return Next(max) + 1;
        }

        /// <summary>
        /// Returns a die roll based on a percentage change
        /// </summary>
        /// <param name="chance"> The chance of success (inclusive) </param>
        /// <returns> Whether the roll succeeded </returns>
        public bool PercentileRoll(int chance)
        {
            return Next(100) < chance;
        }

        /// <summary>
        /// Returns a random number greater than or equal to min and less than or equal to max
        /// </summary>
        /// <param name="min"> The lower limit (inclusive) </param>
        /// <param name="max"> The upper limit (inclusive) </param>
        /// <returns> A random number </returns>
        public int RandomBetween(int min, int max)
        {
            return min + Next(1 + max - min);
        }

        /// <summary>
        /// Returns a random number greater or equal to zero and less than max
        /// </summary>
        /// <param name="max"> The upper limit (exclusive) </param>
        /// <returns> A random number </returns>
        public int RandomLessThan(int max)
        {
            return Next(max);
        }

        /// <summary>
        /// Returns an integer based on a normal distribution
        /// </summary>
        /// <param name="mean"> The mean value for the distribution </param>
        /// <param name="stand"> The standard deviation of the distribution </param>
        /// <returns> A random value </returns>
        public int RandomNormal(int mean, int stand)
        {
            int low = 0;
            int high = _randnorNum;
            if (stand < 1)
            {
                return mean;
            }
            int tmp = (short)Next(32768);
            while (low < high)
            {
                int mid = (low + high) >> 1;
                if (_randnorTable[mid] < tmp)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            int offset = stand * low / _randnorStd;
            if (RandomLessThan(100) < 50)
            {
                return mean - offset;
            }
            return mean + offset;
        }

        /// <summary>
        /// Returns a random number with a flat distrubution around centre and within width of it
        /// </summary>
        /// <param name="centre"> The central value </param>
        /// <param name="width"> The maximum distance (inclusive) from the centre </param>
        /// <returns> A random number </returns>
        public int RandomSpread(int centre, int width)
        {
            return centre + Next(1 + width + width) - width;
        }

        /// <summary>
        /// Generates the actual random number
        /// </summary>
        /// <param name="max"> </param>
        /// <returns> </returns>
        private int Next(int max)
        {
            if (max <= 1)
            {
                return 0;
            }
            Random use = UseFixed ? _fixed : _mainSequence;
            return use.Next(max);
        }
    }
}