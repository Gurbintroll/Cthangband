using System;

namespace Cthangband
{
    [Serializable]
    internal class HighScore
    {
        public readonly string How;
        public readonly int Pclass;
        public readonly int Prace;
        public readonly int Pts;
        public readonly string When;
        public readonly string Where;
        public readonly string Who;
        public bool Hilight;
        public int Index;
        public bool Living;

        public HighScore(Player player) : this(player, SaveGame.Instance)
        {
        }

        public HighScore(Player player, SaveGame saveGame)
        {
            if (saveGame == null)
            {
                saveGame = SaveGame.Instance;
            }
            Pts = player.GetScore(saveGame);
            Prace = player.RaceIndex;
            Pclass = player.ProfessionIndex;
            if (player.IsDead)
            {
                When = DateTime.Now.ToString("dd-MMM-yyyy");
                How = SaveGame.Instance.DiedFrom;
            }
            else
            {
                Living = true;
                When = "TODAY";
                How = "nobody (yet!)";
            }
            Who =
                $"{player.Name.Trim()}{player.Generation.ToRoman(true)} the level {player.Level} {Race.RaceInfo[player.RaceIndex].Title} {Profession.ClassSubName(player.ProfessionIndex, player.Realm1)}";
            if (saveGame.DunLevel > 0)
            {
                Where = $"on level {saveGame.DunLevel} of {saveGame.CurDungeon.Name}";
            }
            else
            {
                Where = "in the wilderness";
                foreach (Town t in saveGame.Towns)
                {
                    if (player.WildernessX == t.X && player.WildernessY == t.Y)
                    {
                        Where = $"in {t.Name}";
                    }
                }
            }
        }

        private HighScore()
        {
        }
    }
}