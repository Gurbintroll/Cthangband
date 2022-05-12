// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Debug;
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.Terminal;
using Cthangband.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Cthangband
{
    internal static class Program
    {
        public static readonly Rng Rng = new Rng();
        public static HighScoreTable HiScores;
        public static string SaveFolder;

        public static bool SuperQuit;
        private static string _activeSaveSlot;
        private static string _saveSlot1;
        private static string _saveSlot2;
        private static string _saveSlot3;

        public static string ActiveSaveSlot
        {
            get => _activeSaveSlot;
            private set
            {
                _activeSaveSlot = value;
                Profile.LoadOrCreate(value);
            }
        }

        public static T DeserializeFromSaveFolder<T>(string filename)
        {
            string path = Path.Combine(SaveFolder, filename);
            FileInfo info = new FileInfo(path);
            T o;
            if (!info.Exists)
            {
                return default;
            }
            using (FileStream stream = info.OpenRead())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                o = (T)formatter.Deserialize(stream);
                stream.Close();
            }
            return o;
        }

        public static bool DirCreate(string path)
        {
            // Path might be empty - this is not an error condition
            if (string.IsNullOrEmpty(path))
            {
                return true;
            }
            DirectoryInfo intended = new DirectoryInfo(path);
            // If it already exists, then we're fine
            if (intended.Exists)
            {
                return true;
            }
            intended.Create();
            return true;
        }

        public static void GetDefaultFolder()
        {
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            savePath = Path.Combine(savePath, "My Games");
            savePath = Path.Combine(savePath, Constants.VersionName);
            SaveFolder = savePath;
            _saveSlot1 = Path.Combine(savePath, $"slot1.v_{Constants.VersionMajor}_{Constants.VersionMinor}_savefile");
            _saveSlot2 = Path.Combine(savePath, $"slot2.v_{Constants.VersionMajor}_{Constants.VersionMinor}_savefile");
            _saveSlot3 = Path.Combine(savePath, $"slot3.v_{Constants.VersionMajor}_{Constants.VersionMinor}_savefile");
        }

        public static void Quit(string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                MessageBox.Show(reason, Constants.VersionName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Environment.Exit(0);
        }

        public static void SerializeToSaveFolder<T>(T o, string filename)
        {
            string path = Path.Combine(SaveFolder, filename);
            FileInfo info = new FileInfo(path);
            using (FileStream stream = info.OpenWrite())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, o);
            }
        }

        internal static Dictionary<string, HighScore> GetHighScoreFromSaves()
        {
            var saves = new Dictionary<string, HighScore>();
            var score1 = GetHighScoreFromSave(_saveSlot1);
            if (score1 != null)
            {
                saves.Add(_saveSlot1, score1);
            }
            var score2 = GetHighScoreFromSave(_saveSlot2);
            if (score2 != null)
            {
                saves.Add(_saveSlot2, score2);
            }
            var score3 = GetHighScoreFromSave(_saveSlot3);
            if (score3 != null)
            {
                saves.Add(_saveSlot3, score3);
            }
            return saves;
        }

        private static HighScore GetHighScoreFromSave(string save)
        {
            FileInfo file = new FileInfo(save);
            if (!file.Exists)
            {
                return null;
            }
            Profile tempProfile;
            try
            {
                tempProfile = DeserializeFromSaveFolder<Profile>(save);
            }
            catch (Exception)
            {
                return null;
            }
            if (tempProfile.Game.Player == null)
            {
                return null;
            }
            if (tempProfile.Game.Player.IsWizard)
            {
                return null;
            }
            return new HighScore(tempProfile.Game.Player, tempProfile.Game);
        }

        [STAThread]
        private static void Main()
        {
#if !DEBUG
            try
            {
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            General.CheckDebugStatus();
            SplashScreen splashScreen = new SplashScreen();
            if (splashScreen.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            while (true)
            {
                ShowMainMenu();
                Profile.Instance.Run();
                if (SuperQuit)
                {
                    return;
                }
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        private static void PeekSavefile(string save, int displayRow)
        {
            FileInfo file = new FileInfo(save);
            if (!file.Exists)
            {
                Gui.Print(Colour.Green, "<Empty>", 34, displayRow + 10);
                return;
            }
            Profile tempProfile;
            try
            {
                tempProfile = DeserializeFromSaveFolder<Profile>(save);
            }
            catch (Exception)
            {
                Gui.Print(Colour.BrightRed, "<Unreadable>", 34, displayRow + 8);
                return;
            }
            bool tempDeath = tempProfile.Game.Player == null;
            Colour color;
            int tempLev;
            int tempRace;
            int tempClass;
            Realm tempRealm;
            string tempName;
            if (tempDeath)
            {
                color = Colour.Grey;
                tempLev = tempProfile.ExPlayer.Level;
                tempRace = tempProfile.ExPlayer.RaceIndex;
                tempClass = tempProfile.ExPlayer.ProfessionIndex;
                tempRealm = tempProfile.ExPlayer.Realm1;
                tempName = tempProfile.ExPlayer.Name.Trim() + tempProfile.ExPlayer.Generation.ToRoman(true);
            }
            else
            {
                color = Colour.White;
                if (tempProfile.Game.Player.IsWizard)
                {
                    color = Colour.Yellow;
                }
                tempLev = tempProfile.Game.Player.Level;
                tempRace = tempProfile.Game.Player.RaceIndex;
                tempClass = tempProfile.Game.Player.ProfessionIndex;
                tempRealm = tempProfile.Game.Player.Realm1;
                tempName = tempProfile.Game.Player.Name.Trim() + tempProfile.Game.Player.Generation.ToRoman(true);
            }
            Gui.Print(color, tempName, 34, displayRow + 14 - (tempName.Length / 2));
            string tempchar = $"the level {tempLev}";
            Gui.Print(color, tempchar, 35, displayRow + 14 - (tempchar.Length / 2));
            tempchar = Race.RaceInfo[tempRace].Title;
            Gui.Print(color, tempchar, 36, displayRow + 14 - (tempchar.Length / 2));
            tempchar = Profession.ClassSubName(tempClass, tempRealm);
            Gui.Print(color, tempchar, 37, displayRow + 14 - (tempchar.Length / 2));
            if (tempDeath)
            {
                tempchar = "(deceased)";
                Gui.Print(color, tempchar, 38, displayRow + 14 - (tempchar.Length / 2));
            }
            else if (tempProfile.Game.Player.IsWizard)
            {
                tempchar = "-=<WIZARD>=-";
                Gui.Print(color, tempchar, 38, displayRow + 14 - (tempchar.Length / 2));
            }
        }

        private static void ShowMainMenu()
        {
            Gui.CursorVisible = false;
            Gui.Mixer.Play(MusicTrack.Menu);
            while (true)
            {
                Gui.SetBackground(BackgroundImage.Menu);
                Gui.Clear();
                Gui.Print(Colour.Red, $"{Constants.VersionStamp} ({Constants.CompileTime})".PadCenter(80), 43, 0);
                Gui.Print(Colour.White, "                      Press 1-3 to load or create a game.", 18, 0);
                if (HiScores.Count > 0)
                {
                    Gui.Print(Colour.White, "                      Press 's' to view the high scores.", 20, 0);
                }
                Gui.Print(Colour.White, "                               Press 'q' to quit.", 22, 0);
                Gui.Print(Colour.White, "                           Press 'h' for help/manual.", 23, 0);
                General.PrintDebugOption();
                Gui.Print(Colour.Red, "                        Press 'x' to delete a save slot.", 26, 0);
                Gui.Print(Colour.BrightTurquoise, "               Slot 1                Slot 2                Slot 3", 32, 0);
                PeekSavefile(_saveSlot1, 4);
                PeekSavefile(_saveSlot2, 26);
                PeekSavefile(_saveSlot3, 48);
                Gui.Refresh();
                while (true)
                {
                    Gui.FullScreenOverlay = false;
                    Gui.HideCursorOnFullScreenInkey = true;
                    char c = Gui.Inkey();
                    Gui.HideCursorOnFullScreenInkey = false;
                    if (c == 'q' || c == 'Q')
                    {
                        Quit(null);
                    }
                    if (c == 's' || c == 'S')
                    {
                        HiScores.DisplayScores();
                        break;
                    }
                    if (c == 'h' || c == 'H')
                    {
                        using (Manual.ManualViewer manual = new Manual.ManualViewer())
                        {
                            manual.ShowDialog();
                        }
                        break;
                    }
                    if (c == '1')
                    {
                        ActiveSaveSlot = _saveSlot1;
                        return;
                    }
                    if (c == '2')
                    {
                        ActiveSaveSlot = _saveSlot2;
                        return;
                    }
                    if (c == '3')
                    {
                        ActiveSaveSlot = _saveSlot3;
                        return;
                    }
                    if (c == 'd' || c == 'D')
                    {
                        General.ShowDebugMenu();
                    }
                    if (c == 'x' || c == 'X')
                    {
                        Gui.Print(Colour.Grey, "                                                                 ", 7,
                            0);
                        Gui.Print(Colour.BrightRed,
                            "  ***  WARNING! This will delete all character history from the save slot. ***", 26, 0);
                        Gui.Print(Colour.BrightRed,
                            "  ***        Press 1-3 if you are sure you want to delete a save slot,     ***", 27, 0);
                        Gui.Print(Colour.BrightRed,
                            "  ***                     or any other key to cancel.                      ***", 28, 0);
                        Gui.HideCursorOnFullScreenInkey = true;
                        c = Gui.Inkey();
                        Gui.HideCursorOnFullScreenInkey = false;
                        if (c == '1')
                        {
                            FileInfo fileInfo = new FileInfo(_saveSlot1);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                        if (c == '2')
                        {
                            FileInfo fileInfo = new FileInfo(_saveSlot2);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                        if (c == '3')
                        {
                            FileInfo fileInfo = new FileInfo(_saveSlot3);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}