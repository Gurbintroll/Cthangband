// Cthangband: � 1997 - 2023 Dean Anderson; Based on Angband: � 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: � 1985 Robert Alan Koeneke and Umoria: � 1989 James E.Wilson
//
// This game is released under the �Angband License�, defined as: �� 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.�
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System.Collections.Generic;

namespace Cthangband
{
    internal class HighScoreTable
    {
        public string ClassFilter = string.Empty;
        public string RaceFilter = string.Empty;

        private readonly List<HighScore> _scores;

        public HighScoreTable()
        {
            _scores = Program.DeserializeFromSaveFolder<List<HighScore>>($"highscores.v_{Constants.VersionMajor}_{Constants.VersionMinor}_scorefile") ?? new List<HighScore>();
            ClearScoreFlags();
        }

        public int Count => _scores.Count;

        public void DisplayScores()
        {
            ClearScoreFlags();
            DisplayScores(-1, null);
        }

        public void DisplayScores(int centredScore)
        {
            ClearScoreFlags();
            DisplayScores(centredScore, null);
        }

        public void DisplayScores(HighScore living)
        {
            ClearScoreFlags();
            DisplayScores(-1, living);
        }

        public void InsertNewScore(HighScore score)
        {
            InsertScore(_scores, score);
            ClearScoreFlags();
            Save();
        }

        private void ClearScoreFlags()
        {
            foreach (var score in _scores)
            {
                score.Living = false;
                score.Hilight = false;
                score.Index = 0;
            }
        }

        private void DisplayScores(int centredScore, HighScore living)
        {
            List<HighScore> collectedScores;
            // Five above ours, ours, and five below
            if (centredScore >= 0)
            {
                collectedScores = FilterByScore(centredScore);
            }
            // Five above ours, ours, and five below
            else if (living != null)
            {
                collectedScores = FilterByHighScore(living);
            }
            else
            {
                collectedScores = GetAllHighScores();
            }
            if (collectedScores.Count == 0)
            {
                Gui.Clear();
                Gui.SetBackground(Terminal.BackgroundImage.Normal);
                Gui.Print(Colour.Yellow, "High Scores", 1, 34);
                Gui.Print(Colour.Yellow, "===========", 2, 34);
                Gui.AnyKey(43);
                return;
            }

            // collectedScores now holds all the scores we're interested in - probably only a single
            // page of them
            var line = 0;
            do
            {
                if (line == 0)
                {
                    Gui.Clear();
                    Gui.SetBackground(Terminal.BackgroundImage.Normal);
                    Gui.Print(Colour.Yellow, "High Scores", 1, 34);
                    Gui.Print(Colour.Yellow, "===========", 2, 34);
                }
                ShowScore(collectedScores[0], line);
                line++;
                collectedScores.RemoveAt(0);
                if (line > 9 || collectedScores.Count == 0)
                {
                    line = 0;
                    Gui.AnyKey(43);
                }
            } while (collectedScores.Count > 0);
        }

        private List<HighScore> FilterByHighScore(HighScore living)
        {
            var index = 0;
            var useScores = InsertFromSaves(Program.ActiveSaveSlot);
            var list = new List<HighScore>();
            // Gather all the scores above ours
            for (var i = 0; i < useScores.Count; i++)
            {
                if (useScores[index].Pts >= living.Pts)
                {
                    list.Add(useScores[index]);
                    useScores[index].Index = index + 1;
                    index++;
                }
                else
                {
                    break;
                }
            }
            StripUnwanted(list);
            // Add ours
            list.Add(living);
            living.Living = true;
            living.Index = index + 1;
            // Remove the excess
            while (list.Count > 6)
            {
                list.RemoveAt(0);
            }
            var below = 10 - list.Count;
            // Add some more below (shifting their index by one)
            for (var i = 0; i < below; i++)
            {
                if (index < useScores.Count)
                {
                    list.Add(useScores[index]);
                    useScores[index].Index = index + 2;
                    index++;
                }
            }
            StripUnwanted(list);
            return list;
        }

        private List<HighScore> FilterByScore(int centredScore)
        {
            var index = 0;
            var useScores = InsertFromSaves(null);
            var list = new List<HighScore>();
            // Gather all the scores above and including ours
            for (var i = 0; i < useScores.Count; i++)
            {
                if (useScores[index].Pts >= centredScore)
                {
                    list.Add(useScores[index]);
                    useScores[index].Index = index + 1;
                    index++;
                }
                else
                {
                    break;
                }
            }
            StripUnwanted(list);
            // Remove the excess
            while (list.Count > 6)
            {
                list.RemoveAt(0);
            }

            // Hilight the chosen one
            list[list.Count - 1].Hilight = true;
            var below = 10 - list.Count;
            // Add some more below
            for (var i = 0; i < below; i++)
            {
                if (index < useScores.Count)
                {
                    list.Add(useScores[index]);
                    useScores[index].Index = index + 1;
                    index++;
                }
            }
            StripUnwanted(list);
            return list;
        }

        private List<HighScore> GetAllHighScores()
        {
            var useScores = InsertFromSaves(null);
            var list = new List<HighScore>();
            for (var i = 0; i < useScores.Count; i++)
            {
                useScores[i].Index = i + 1;
                list.Add(useScores[i]);
            }
            StripUnwanted(list);
            return list;
        }

        private List<HighScore> InsertFromSaves(string activeSaveSlot)
        {
            var list = new List<HighScore>();
            foreach (var score in _scores)
            {
                list.Add(score);
            }
            var scoresFromSaves = Program.GetHighScoreFromSaves();
            var keys = scoresFromSaves.Keys;
            foreach (var key in keys)
            {
                if (key != activeSaveSlot)
                {
                    InsertScore(list, scoresFromSaves[key]);
                }
            }
            return list;
        }

        private void InsertScore(List<HighScore> list, HighScore score)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Pts < score.Pts)
                {
                    list.Insert(i, score);
                    if (list.Count > 99)
                    {
                        list.RemoveAt(99);
                    }
                    return;
                }
            }

            // We fell out of the loop so either we have no scores or they're all better than us
            if (list.Count <= 99)
            {
                list.Add(score);
            }
        }

        private void Save()
        {
            ClearScoreFlags();
            Program.SerializeToSaveFolder(_scores, $"highscores.v_{Constants.VersionMajor}_{Constants.VersionMinor}_scorefile");
        }

        private void ShowScore(HighScore score, int line)
        {
            var color = Colour.White;
            if (line % 2 == 1)
            {
                color = Colour.Grey;
            }
            if (score.Hilight)
            {
                color = Colour.BrightRed;
            }
            if (score.Living)
            {
                color = Colour.BrightGreen;
                Gui.Print(color, $"{score.Index,2}) {score.Pts,5} {score.Who}", (line * 3) + 5, 1);
                Gui.Print(color, $"killed by {score.How}", (line * 3) + 6, 11);
                Gui.Print(color, $"{score.Where}", (line * 3) + 7, 11);
            }
            else
            {
                Gui.Print(color, $"{score.Index,2}) {score.Pts,5} {score.Who}", (line * 3) + 5, 1);
                Gui.Print(color, $"killed, {score.When}, by {score.How}", (line * 3) + 6, 11);
                Gui.Print(color, $"{score.Where}", (line * 3) + 7, 11);
            }
        }

        private void StripUnwanted(List<HighScore> list)
        {
            var index = 0;
            if (string.IsNullOrEmpty(RaceFilter) && string.IsNullOrEmpty(ClassFilter))
            {
                return;
            }
            if (list.Count == 0)
            {
                return;
            }
            do
            {
                if (!string.IsNullOrEmpty(RaceFilter))
                {
                    if (list[index].CurrentRace != RaceFilter)
                    {
                        list.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                else if (!string.IsNullOrEmpty(ClassFilter))
                {
                    if (list[index].CurrentClass != ClassFilter)
                    {
                        list.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            } while (index < list.Count);
        }
    }
}