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
    [Serializable]
    internal class Race
    {
        public static readonly Race[] RaceInfo =
        {
            new Race("Tcho-Tcho", new[] {3, -2, -1, 1, 2, -2}, -2, -10, 2, -1, 1, 7, 12, 10, 11, 120, 14, 8, 82, 5, 200,
                20, 78, 6, 190, 15, 0, 0xC89D),
            new Race("Miri Nigri", new[] {2, -2, -1, -1, 2, -4}, -5, -2, -1, -1, -1, 5, 12, 5, 11, 140, 14, 6, 65, 6,
                150, 20, 61, 6, 120, 15, 0, 0xDFCF),
            new Race("Cyclops", new[] {4, -3, -3, -3, 4, -6}, -4, -5, -5, -2, -2, 5, 20, 12, 13, 130, 50, 24, 92, 10,
                255, 60, 80, 8, 235, 60, 1, 0x0005),
            new Race("Dark Elf", new[] {-1, 3, 2, 2, -2, 1}, 5, 15, 20, 3, 8, 12, -5, 10, 9, 150, 75, 75, 60, 4, 100, 6,
                54, 4, 80, 6, 5, 0xBFDF),
            new Race("Draconian", new[] {2, 1, 1, 1, 2, -3}, -2, 5, 3, 0, 1, 10, 5, 5, 11, 250, 75, 33, 76, 1, 160, 5,
                72, 1, 130, 5, 2, 0xDF57),
            new Race("Dwarf", new[] {2, -2, 2, -2, 2, -3}, 2, 9, 10, -1, 7, 10, 15, 0, 11, 125, 35, 15, 48, 3, 150, 10,
                46, 3, 120, 10, 5, 0x4805),
            new Race("Elf", new[] {-1, 2, 2, 1, -2, 2}, 5, 6, 6, 2, 8, 12, -5, 15, 8, 120, 75, 75, 60, 4, 100, 6, 54, 4,
                80, 6, 3, 0xFF5F),
            new Race("Gnome", new[] {-1, 2, 0, 2, 1, -2}, 10, 12, 12, 3, 6, 13, -8, 12, 8, 135, 50, 40, 42, 3, 90, 6,
                39, 3, 75, 3, 4, 0x1E0F),
            new Race("Golem", new[] {4, -5, -5, 0, 4, -4}, -5, -5, 10, -1, -1, 8, 20, 0, 12, 200, 1, 100, 66, 1, 200, 6,
                62, 1, 180, 6, 4, 0x4001),
            new Race("Great One", new[] {1, 2, 2, 2, 3, 2}, 4, 5, 5, 2, 3, 13, 15, 10, 10, 225, 50, 50, 82, 5, 190, 20,
                78, 6, 180, 15, 0, 0xFFFF),
            new Race("Half Elf", new[] {-1, 1, 1, 1, -1, 1}, 2, 3, 3, 1, 6, 11, -1, 5, 9, 110, 24, 16, 66, 6, 130, 15,
                62, 6, 100, 10, 2, 0xFFFF),
            new Race("Half Giant", new[] {4, -2, -2, -2, 3, -3}, -6, -8, -6, -2, -1, 5, 25, 5, 13, 150, 40, 10, 100, 10,
                255, 65, 80, 10, 240, 64, 3, 0x0011),
            new Race("Half Ogre", new[] {3, -1, -1, -1, 3, -3}, -3, -5, -5, -2, -1, 5, 20, 0, 12, 130, 40, 10, 92, 10,
                255, 60, 80, 8, 235, 60, 3, 0x0C07),
            new Race("Half Orc", new[] {2, -1, 0, 0, 1, -4}, -3, -3, -3, -1, 0, 7, 12, -5, 10, 110, 11, 4, 66, 1, 150,
                5, 62, 1, 120, 5, 3, 0x898D),
            new Race("Half Titan", new[] {5, 1, 1, -2, 3, 1}, -5, 5, 2, -2, 1, 8, 25, 0, 14, 255, 100, 30, 111, 11, 255,
                86, 99, 11, 250, 86, 0, 0x1F27),
            new Race("Half Troll", new[] {4, -4, -2, -4, 3, -6}, -5, -8, -8, -2, -1, 5, 20, -10, 12, 137, 20, 10, 96,
                10, 250, 50, 84, 8, 225, 40, 3, 0x0805),
            new Race("High Elf", new[] {1, 3, 2, 3, 1, 5}, 4, 20, 20, 4, 3, 14, 10, 25, 10, 200, 100, 30, 90, 10, 190,
                20, 82, 10, 180, 15, 4, 0xBF5F),
            new Race("Hobbit", new[] {-2, 2, 1, 3, 2, 1}, 15, 18, 18, 5, 12, 15, -10, 20, 7, 110, 21, 12, 36, 3, 60, 3,
                33, 3, 50, 3, 4, 0xBC0B),
            new Race("Human", new[] {0, 0, 0, 0, 0, 0}, 0, 0, 0, 0, 0, 10, 0, 0, 10, 100, 14, 6, 72, 6, 180, 25, 66, 4,
                150, 20, 0, 0xFFFF),
            new Race("Imp", new[] {-1, -1, -1, 1, 2, -3}, -3, 2, -1, 1, -1, 10, 5, -5, 10, 110, 13, 4, 68, 1, 150, 5,
                64, 1, 120, 5, 3, 0x97CB),
            new Race("Klackon", new[] {2, -1, -1, 1, 2, -2}, 10, 5, 5, 0, -1, 10, 5, 5, 12, 135, 20, 3, 60, 3, 80, 4,
                54, 3, 70, 4, 2, 0xC011),
            new Race("Kobold", new[] {1, -1, 0, 1, 0, -4}, -2, -3, -2, -1, 1, 8, 10, -8, 9, 125, 11, 3, 60, 1, 130, 5,
                55, 1, 100, 5, 3, 0xC009),
            new Race("Mind Flayer", new[] {-3, 4, 4, 0, -2, -5}, 10, 25, 15, 2, 5, 12, -10, -5, 9, 140, 100, 25, 68, 6,
                142, 15, 63, 6, 112, 10, 4, 0xD746),
            new Race("Nibelung", new[] {1, -1, 2, 0, 2, -4}, 3, 5, 10, 1, 5, 10, 9, 0, 11, 135, 40, 12, 43, 3, 92, 6,
                40, 3, 78, 3, 5, 0xDC0F),
            new Race("Skeleton", new[] {0, -2, -2, 0, 1, -4}, -5, -5, 5, -1, -1, 8, 10, 0, 10, 145, 100, 35, 72, 6, 50,
                5, 66, 4, 50, 5, 2, 0x5F0F),
            new Race("Spectre", new[] {-5, 4, 4, 2, -3, -6}, 10, 25, 20, 5, 5, 14, -15, -5, 7, 180, 100, 30, 72, 6, 100,
                25, 66, 4, 100, 20, 5, 0x5F4E),
            new Race("Sprite", new[] {-4, 3, 3, 3, -2, 2}, 10, 10, 10, 4, 10, 10, -12, 0, 7, 175, 50, 25, 32, 2, 75, 2,
                29, 2, 65, 2, 4, 0xBE5E),
            new Race("Vampire", new[] {3, 3, -1, -1, 1, 2}, 4, 10, 10, 4, 1, 8, 5, 0, 11, 200, 100, 30, 72, 6, 180, 25,
                66, 4, 150, 20, 5, 0xFFFF),
            new Race("Yeek", new[] {-2, 1, 1, 1, -2, -7}, 2, 4, 10, 3, 5, 15, -5, -5, 7, 100, 14, 3, 50, 3, 90, 6, 50,
                3, 75, 3, 2, 0xDE0F),
            new Race("Zombie", new[] {2, -6, -6, 1, 4, -5}, -5, -5, 8, -1, -1, 5, 15, 0, 13, 135, 100, 30, 72, 6, 100,
                25, 66, 4, 100, 20, 2, 0x0001)
        };

        public readonly int[] AbilityBonus = new int[6];
        public readonly int AgeRange;
        public readonly int BaseAge;
        public readonly int BaseDeviceBonus;
        public readonly int BaseDisarmBonus;
        public readonly int BaseMeleeAttackBonus;
        public readonly int BaseRangedAttackBonus;
        public readonly int BaseSaveBonus;
        public readonly int BaseSearchBonus;
        public readonly int BaseSearchFrequency;
        public readonly int BaseStealthBonus;
        public readonly uint Choice;
        public readonly int ExperienceFactor;
        public readonly int FemaleBaseHeight;
        public readonly int FemaleBaseWeight;
        public readonly int FemaleHeightRange;
        public readonly int FemaleWeightRange;
        public readonly int HitDieBonus;
        public readonly int Infravision;
        public readonly int MaleBaseHeight;
        public readonly int MaleBaseWeight;
        public readonly int MaleHeightRange;
        public readonly int MaleWeightRange;
        public readonly string Title;

        public Race()
        {
        }

        private Race(string title, int[] rAdj, int rDis, int rDev, int rSav, int rStl, int rSrh, int rFos, int rThn,
            int rThb, int rMhp, int rExp, int bAge, int mAge, int mBHt, int mMHt, int mBWt, int mMWt, int fBHt,
            int fMHt, int fBWt, int fMWt, int infra, uint choice)
        {
            Title = title;
            AbilityBonus = rAdj;
            BaseDisarmBonus = rDis;
            BaseDeviceBonus = rDev;
            BaseSaveBonus = rSav;
            BaseStealthBonus = rStl;
            BaseSearchBonus = rSrh;
            BaseSearchFrequency = rFos;
            BaseMeleeAttackBonus = rThn;
            BaseRangedAttackBonus = rThb;
            HitDieBonus = rMhp;
            ExperienceFactor = rExp;
            BaseAge = bAge;
            AgeRange = mAge;
            MaleBaseHeight = mBHt;
            MaleHeightRange = mMHt;
            MaleBaseWeight = mBWt;
            MaleWeightRange = mMWt;
            FemaleBaseHeight = fBHt;
            FemaleHeightRange = fMHt;
            FemaleBaseWeight = fBWt;
            FemaleWeightRange = fMWt;
            Infravision = infra;
            Choice = choice;
        }
    }
}