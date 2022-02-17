using Cthangband.Enumerations;

namespace Cthangband.UI
{
    internal class Screen
    {
        public readonly int[] A; // Was a pointer to part of va, now an index into it
        public readonly int[] C; // Was a pointer to part of va, now an index into it
        public readonly Colour[] Va;
        public readonly char[] Vc;
        public bool Cu;
        public bool Cv;
        public int Cx;
        public int Cy;

        public Screen(int w, int h)
        {
            A = new int[h];
            C = new int[h];
            Va = new Colour[h * w];
            Vc = new char[h * w];
            for (int y = 0; y < h; y++)
            {
                A[y] = w * y;
                C[y] = w * y;
            }
        }

        public void Copy(Screen f, int w, int h)
        {
            for (int y = 0; y < h; y++)
            {
                int fAa = f.A[y];
                int fCc = f.C[y];
                int sAa = A[y];
                int sCc = C[y];
                for (int x = 0; x < w; x++)
                {
                    Va[sAa++] = f.Va[fAa++];
                    Vc[sCc++] = f.Vc[fCc++];
                }
            }
            Cx = f.Cx;
            Cy = f.Cy;
            Cu = f.Cu;
            Cv = f.Cv;
        }
    }
}