﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
        public static bool ExitToDesktop;
        public static HighScoreTable HiScores;
        public static string SaveFolder;
        private static string _activeSaveSlot;
        private static string[] _saveSlot;
        private static Settings _settings;

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
            _saveSlot = new string[4];
            for (int i = 0; i < 4; i++)
            {
                _saveSlot[i] = Path.Combine(savePath, $"slot{i + 1}.v_{Constants.VersionMajor}_{Constants.VersionMinor}_savefile");
            }
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

        internal static void ChangeOptions()
        {
            var fonts = Gui.Terminal.EnumerateFonts();
            var font = 0;
            for (int i = 0; i < fonts.Count; i++)
            {
                if (fonts[i] == _settings.Font)
                {
                    font = i;
                    break;
                }
            }
            var resolutions = Gui.Terminal.EnumerateResolutions();
            var styles = new List<string> { "Regular", "Bold", "Italic", "Bold Italic" };
            var resolution = _settings.Resolution;
            var menuItem = 0;
            if (resolution == 0)
            {
                resolution = 5;
            }
            int textStyle = 0;
            if (_settings.Bold)
            {
                textStyle += 1;
            }
            if (_settings.Italic)
            {
                textStyle += 2;
            }
            Gui.Save();
            Gui.FullScreenOverlay = true;
            Gui.SetBackground(BackgroundImage.Normal);
            while (true)
            {
                Gui.Clear();
                Gui.Print(Colour.White, "    Text font:", 1, 1);
                Gui.Print(Colour.White, "   Text style:", 2, 1);
                Gui.Print(Colour.White, "Display style:", 4, 1);
                Gui.Print(Colour.White, "  Window size:", 5, 1);
                Gui.Print(Colour.White, " Music volume:", 7, 1);
                Gui.Print(Colour.White, " Sound volume:", 8, 1);
                Gui.Print(menuItem == 0 ? Colour.BrightPurple : Colour.White, _settings.Font, 1, 16);
                Gui.Print(menuItem == 1 ? Colour.BrightPurple : Colour.White, styles[textStyle], 2, 16);
                Gui.Print(menuItem == 2 ? Colour.BrightPurple : Colour.White, (_settings.Resolution == 0 ? "Fullscreen" : "Windowed"), 4, 16);
                Gui.Print(menuItem == 3 ? Colour.BrightPurple : Colour.White, resolutions[resolution - 1].ToString(), 5, 16);
                Gui.Print(menuItem == 4 ? Colour.BrightPurple : Colour.White, _settings.MusicVolume.ToString() + "%", 7, 16);
                Gui.Print(menuItem == 5 ? Colour.BrightPurple : Colour.White, _settings.SoundVolume.ToString() + "%", 8, 16);
                Gui.HideCursorOnFullScreenInkey = true;
                var c = Gui.Inkey();
                if (c == '\r' || c == ' ' || c == '\x1b')
                {
                    break;
                }
                if (c == '2')
                {
                    menuItem++;
                    if (menuItem == 6)
                    {
                        menuItem = 0;
                    }
                }
                if (c == '8')
                {
                    menuItem--;
                    if (menuItem == -1)
                    {
                        menuItem = 5;
                    }
                }
                if (c == '6')
                {
                    switch (menuItem)
                    {
                        case 0:
                            font++;
                            if (font >= fonts.Count)
                            {
                                font = 0;
                            }
                            _settings.Font = fonts[font];
                            Gui.Terminal.SetNewFont(_settings.Font, _settings.Bold, _settings.Italic);
                            break;

                        case 1:
                            textStyle++;
                            if (textStyle >= styles.Count)
                            {
                                textStyle = 0;
                            }
                            _settings.Bold = (textStyle == 1 || textStyle == 3);
                            _settings.Italic = (textStyle == 2 || textStyle == 3);
                            Gui.Terminal.SetNewFont(_settings.Font, _settings.Bold, _settings.Italic);
                            break;

                        case 2:
                            _settings.Resolution = _settings.Resolution == 0 ? resolution : 0;
                            Gui.Terminal.ResizeWindow(_settings.Resolution == 0, resolutions[resolution - 1].Width, resolutions[resolution - 1].Height);
                            break;

                        case 3:
                            if (resolution < resolutions.Count)
                            {
                                resolution++;
                                if (_settings.Resolution != 0)
                                {
                                    _settings.Resolution = resolution;
                                    Gui.Terminal.ResizeWindow(false, resolutions[resolution - 1].Width, resolutions[resolution - 1].Height);
                                }
                            }
                            break;

                        case 4:
                            if (_settings.MusicVolume < 100)
                            {
                                _settings.MusicVolume += 5;
                            }
                            Gui.Mixer.MusicVolume = _settings.MusicVolume / 100.0f;
                            Gui.Mixer.ResetCurrentMusicVolume();
                            break;

                        case 5:
                            if (_settings.SoundVolume < 100)
                            {
                                _settings.SoundVolume += 5;
                            }
                            Gui.Mixer.SoundVolume = _settings.SoundVolume / 100.0f;
                            break;
                    }
                }
                if (c == '4')
                {
                    switch (menuItem)
                    {
                        case 0:
                            font--;
                            if (font < 0)
                            {
                                font = fonts.Count - 1;
                            }
                            _settings.Font = fonts[font];
                            Gui.Terminal.SetNewFont(_settings.Font, _settings.Bold, _settings.Italic);
                            break;

                        case 1:
                            textStyle--;
                            if (textStyle < 0)
                            {
                                textStyle = styles.Count - 1;
                            }
                            _settings.Bold = (textStyle == 1 || textStyle == 3);
                            _settings.Italic = (textStyle == 2 || textStyle == 3);
                            Gui.Terminal.SetNewFont(_settings.Font, _settings.Bold, _settings.Italic);
                            break;

                        case 2:
                            _settings.Resolution = _settings.Resolution == 0 ? resolution : 0;
                            Gui.Terminal.ResizeWindow(_settings.Resolution == 0, resolutions[resolution - 1].Width, resolutions[resolution - 1].Height);
                            break;

                        case 3:
                            if (resolution > 1)
                            {
                                resolution--;
                                if (_settings.Resolution != 0)
                                {
                                    _settings.Resolution = resolution;
                                    Gui.Terminal.ResizeWindow(false, resolutions[resolution - 1].Width, resolutions[resolution - 1].Height);
                                }
                            }
                            break;

                        case 4:
                            if (_settings.MusicVolume > 0)
                            {
                                _settings.MusicVolume -= 5;
                            }
                            Gui.Mixer.MusicVolume = _settings.MusicVolume / 100.0f;
                            Gui.Mixer.ResetCurrentMusicVolume();
                            break;

                        case 5:
                            if (_settings.SoundVolume > 0)
                            {
                                _settings.SoundVolume -= 5;
                            }
                            Gui.Mixer.SoundVolume = _settings.SoundVolume / 100.0f;
                            break;
                    }
                }
            }
            Gui.FullScreenOverlay = false;
            Gui.Load();
        }

        internal static Dictionary<string, HighScore> GetHighScoreFromSaves()
        {
            var saves = new Dictionary<string, HighScore>();
            for (int i = 0; i < 4; i++)
            {
                var score = GetHighScoreFromSave(_saveSlot[i]);
                if (score != null)
                {
                    saves.Add(_saveSlot[i], score);
                }
            }
            return saves;
        }

        private static int ChooseProfile(int saveIndex, string action)
        {
            Gui.SetBackground(BackgroundImage.Normal);
            Gui.Clear();
            for (int i = 0; i < 4; i++)
            {
                PeekSavefile(_saveSlot[i], i, false);
            }
            Gui.Print(Colour.BrightTurquoise, "Savegame 1", 5, 15);
            Gui.Print(Colour.BrightTurquoise, "Savegame 2", 5, 55);
            Gui.Print(Colour.BrightTurquoise, "Savegame 3", 28, 15);
            Gui.Print(Colour.BrightTurquoise, "Savegame 4", 28, 55);
            Gui.Print(Colour.BrightTurquoise, $"Select a savegame to {action}.".PadCenter(80), 23, 0);
            Gui.Refresh();
            Gui.Save();
            int displayRow = 0;
            int displayCol = 0;
            while (true)
            {
                Gui.Load();
                switch (saveIndex)
                {
                    case 0:
                        displayRow = 9;
                        displayCol = 6;
                        break;

                    case 1:
                        displayRow = 9;
                        displayCol = 46;
                        break;

                    case 2:
                        displayRow = 31;
                        displayCol = 6;
                        break;

                    case 3:
                        displayRow = 31;
                        displayCol = 46;
                        break;
                }
                Gui.Print(Colour.BrightPurple, "+----------------+", displayRow - 2, displayCol + 5);
                Gui.Print(Colour.BrightPurple, "+----------------+", displayRow + 5, displayCol + 5);
                for (int i = -1; i < 5; i++)
                {
                    Gui.Print(Colour.BrightPurple, "|", displayRow + i, displayCol + 5);
                    Gui.Print(Colour.BrightPurple, "|", displayRow + i, displayCol + 22);
                }
                var c = Gui.Inkey();
                if (c == '6' || c == '4')
                {
                    switch (saveIndex)
                    {
                        case 0:
                            saveIndex = 1;
                            break;

                        case 1:
                            saveIndex = 0;
                            break;

                        case 2:
                            saveIndex = 3;
                            break;

                        case 3:
                            saveIndex = 2;
                            break;
                    }
                }
                if (c == '8' || c == '2')
                {
                    switch (saveIndex)
                    {
                        case 0:
                            saveIndex = 2;
                            break;

                        case 1:
                            saveIndex = 3;
                            break;

                        case 2:
                            saveIndex = 0;
                            break;

                        case 3:
                            saveIndex = 1;
                            break;
                    }
                }
                if (c == '\r' || c == ' ')
                {
                    return saveIndex;
                }
                if (c == '\x1b')
                {
                    return -1;
                }
            }
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

        private static int LoadGame(int saveIndex)
        {
            int choice = ChooseProfile(saveIndex, "load");
            if (choice >= 0)
            {
                if (PeekSavefile(_saveSlot[choice], 0, true))
                {
                    return choice;
                }
            }
            return -1;
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
            GetDefaultFolder();
            _settings = DeserializeFromSaveFolder<Settings>("game.settings") ?? new Settings();
            if (!DirCreate(SaveFolder))
            {
                Quit($"Cannot create '{SaveFolder}'");
            }
            StaticResources.LoadOrCreate();
            HiScores = new HighScoreTable();
            Gui.Initialise(_settings);
            while (!ExitToDesktop)
            {
                ShowMainMenu();
            }
            SerializeToSaveFolder(_settings, "game.settings");
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        private static int NewGame(int saveIndex)
        {
            int choice = ChooseProfile(saveIndex, "overwrite");
            if (choice >= 0)
            {
                if (PeekSavefile(_saveSlot[choice], 0, true))
                {
                    var text = new List<string>
                    {
                        $"Savegame {choice} already exists.",
                        "Overwriting it with a new game will",
                        "lose the information about monsters",
                        "that previous characters in that",
                        "savegame have collected. If all you",
                        "want to do is play a new character,",
                        "you should load it instead of over-",
                        "witing it. Are you sure you wish to",
                        $"overwrite savegame {choice}?"
                    };
                    var options = new List<string> { "Overwrite", "Cancel" };
                    var popup = new PopupMenu(options, text, 36);
                    var result = popup.Show();
                    switch (result)
                    {
                        case -1:
                        case 1:
                            return -1;

                        case 0:
                            FileInfo fileInfo = new FileInfo(_saveSlot[choice]);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                            return choice;
                    }
                }
                return choice;
            }
            return -1;
        }

        private static bool PeekSavefile(string save, int index, bool silently)
        {
            int displayRow = 0;
            int displayCol = 0;
            switch (index)
            {
                case 0:
                    displayRow = 9;
                    displayCol = 6;
                    break;

                case 1:
                    displayRow = 9;
                    displayCol = 46;
                    break;

                case 2:
                    displayRow = 31;
                    displayCol = 6;
                    break;

                case 3:
                    displayRow = 31;
                    displayCol = 46;
                    break;
            }
            FileInfo file = new FileInfo(save);
            if (!file.Exists)
            {
                if (!silently)
                {
                    Gui.Print(Colour.BrightTurquoise, "<Empty>", displayRow, displayCol + 11);
                }
                return false;
            }
            Profile tempProfile;
            try
            {
                tempProfile = DeserializeFromSaveFolder<Profile>(save);
            }
            catch (Exception)
            {
                if (!silently)
                {
                    Gui.Print(Colour.BrightRed, "<Unreadable>", displayRow, displayCol + 8);
                }
                return false;
            }
            if (silently)
            {
                return true;
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
            Gui.Print(color, tempName, displayRow, displayCol + 14 - (tempName.Length / 2));
            string tempchar = $"the level {tempLev}";
            Gui.Print(color, tempchar, displayRow + 1, displayCol + 14 - (tempchar.Length / 2));
            tempchar = Race.RaceInfo[tempRace].Title;
            Gui.Print(color, tempchar, displayRow + 2, displayCol + 14 - (tempchar.Length / 2));
            tempchar = Profession.ClassSubName(tempClass, tempRealm);
            Gui.Print(color, tempchar, displayRow + 3, displayCol + 14 - (tempchar.Length / 2));
            if (tempDeath)
            {
                tempchar = "(deceased)";
                Gui.Print(color, tempchar, displayRow + 4, displayCol + 14 - (tempchar.Length / 2));
            }
            else if (tempProfile.Game.Player.IsWizard)
            {
                tempchar = "-=<WIZARD>=-";
                Gui.Print(color, tempchar, displayRow + 4, displayCol + 14 - (tempchar.Length / 2));
            }
            return true;
        }

        private static void ShowMainMenu()
        {
            Gui.CursorVisible = false;
            Gui.Mixer.Play(MusicTrack.Menu);
            while (true)
            {
                Gui.SetBackground(BackgroundImage.Menu);
                Gui.Clear();
                Gui.Print(Colour.BrightRed, $"© 1997-{Constants.CompileTime:yyyy} Dean Anderson".PadCenter(80), 41, 0);
                Gui.Print(Colour.BrightRed, "See Cthangpedia for license and credits.".PadCenter(80), 42, 0);
                Gui.Print(Colour.BrightRed, $"{Constants.VersionStamp} ({Constants.CompileTime})".PadCenter(80), 43, 0);
                // Check to see if we have a savefile that we can continue
                var saveIndex = _settings.LastProfileUsed;
                var canContinue = PeekSavefile(_saveSlot[saveIndex], 0, true); ;
                var profileChoice = -1;
                if (canContinue)
                {
                    var list = new List<string> { "Continue Game", "New Game", "Load Game", "High Scores", "Cthangpedia", "Options", "Quit to Desktop" };
                    var menu = new PopupMenu(list);
                    var menuChoice = menu.Show();
                    {
                        switch (menuChoice)
                        {
                            case -1:
                            case 6:
                                // Exit to Desktop
                                ExitToDesktop = true;
                                return;

                            case 0:
                                // Continue Game
                                ActiveSaveSlot = _saveSlot[saveIndex];
                                Profile.Instance.Run();
                                return;

                            case 1:
                                // New Game
                                profileChoice = NewGame(saveIndex);
                                if (profileChoice >= 0)
                                {
                                    _settings.LastProfileUsed = profileChoice;
                                    ActiveSaveSlot = _saveSlot[profileChoice];
                                    Profile.Instance.Run();
                                    return;
                                }
                                break;

                            case 2:
                                // Load Game
                                profileChoice = LoadGame(saveIndex);
                                if (profileChoice >= 0)
                                {
                                    _settings.LastProfileUsed = profileChoice;
                                    ActiveSaveSlot = _saveSlot[profileChoice];
                                    Profile.Instance.Run();
                                    return;
                                }
                                break;

                            case 3:
                                // High Scores
                                HiScores.DisplayScores();
                                break;

                            case 4:
                                // Cthangpedia
                                using (Manual.ManualViewer manual = new Manual.ManualViewer())
                                {
                                    manual.ShowDialog();
                                }
                                break;

                            case 5:
                                // Options
                                ChangeOptions();
                                break;
                        }
                    }
                }
                else
                {
                    var list = new List<string> { "New Game", "Load Game", "High Scores", "Cthangpedia", "Options", "Quit to Desktop" };
                    var menu = new PopupMenu(list);
                    var menuChoice = menu.Show();
                    {
                        switch (menuChoice)
                        {
                            case -1:
                            case 5:
                                // Exit to Desktop
                                ExitToDesktop = true;
                                return;

                            case 0:
                                // New Game
                                profileChoice = NewGame(saveIndex);
                                if (profileChoice >= 0)
                                {
                                    _settings.LastProfileUsed = profileChoice;
                                    ActiveSaveSlot = _saveSlot[profileChoice];
                                    Profile.Instance.Run();
                                    return;
                                }
                                break;

                            case 1:
                                // Load Game
                                profileChoice = LoadGame(saveIndex);
                                if (profileChoice >= 0)
                                {
                                    _settings.LastProfileUsed = profileChoice;
                                    ActiveSaveSlot = _saveSlot[profileChoice];
                                    Profile.Instance.Run();
                                    return;
                                }
                                break;

                            case 2:
                                // High Scores
                                HiScores.DisplayScores();
                                break;

                            case 3:
                                // Cthangpedia
                                using (Manual.ManualViewer manual = new Manual.ManualViewer())
                                {
                                    manual.ShowDialog();
                                }
                                break;

                            case 4:
                                // Options
                                ChangeOptions();
                                break;
                        }
                    }
                }
            }
        }
    }
}