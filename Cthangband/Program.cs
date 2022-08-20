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
            Gui.InPopupMenu = true;
            Gui.SetBackground(BackgroundImage.Options);
            PrintOptionsScreen();
            var blank = new string(' ', 34);
            while (true)
            {
                for (int i = 4; i < 10; i++)
                {
                    Gui.Print(Colour.White, blank, i, 16);
                }
                Gui.Print(menuItem == 0 ? Colour.Pink : Colour.Purple, _settings.Font, 4, 16);
                Gui.Print(menuItem == 1 ? Colour.Pink : Colour.Purple, styles[textStyle], 5, 16);
                Gui.Print(menuItem == 2 ? Colour.Pink : Colour.Purple, (_settings.Resolution == 0 ? "Fullscreen" : "Windowed"), 6, 16);
                Gui.Print(menuItem == 3 ? Colour.Pink : Colour.Purple, resolutions[resolution - 1].ToString(), 7, 16);
                Gui.Print(menuItem == 4 ? Colour.Pink : Colour.Purple, _settings.MusicVolume.ToString() + "%", 8, 16);
                Gui.Print(menuItem == 5 ? Colour.Pink : Colour.Purple, _settings.SoundVolume.ToString() + "%", 9, 16);
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
            var settingsFile = $"game.v_{Constants.VersionMajor}_{Constants.VersionMinor}_settings";
            SerializeToSaveFolder(_settings, settingsFile);
            Gui.InPopupMenu = false;
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
                Gui.HideCursorOnFullScreenInkey = true;
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
            GetDefaultFolder();
            var settingsFile = $"game.v_{Constants.VersionMajor}_{Constants.VersionMinor}_settings";
            if (DirCreate(SaveFolder))
            {
                _settings = DeserializeFromSaveFolder<Settings>(settingsFile) ?? new Settings();
                StaticResources.LoadOrCreate();
                HiScores = new HighScoreTable();
                Gui.Initialise(_settings);
                while (!ExitToDesktop)
                {
                    ShowMainMenu();
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

        private static void PrintOptionsScreen()
        {
            Gui.Clear();
            Gui.Print(Colour.Green, "Options", 1, 10);
            Gui.Print(Colour.Green, "=======", 2, 10);
            Gui.Print(Colour.Blue, "    Text font:", 4, 1);
            Gui.Print(Colour.Blue, "   Text style:", 5, 1);
            Gui.Print(Colour.Blue, "Display style:", 6, 1);
            Gui.Print(Colour.Blue, "  Window size:", 7, 1);
            Gui.Print(Colour.Blue, " Music volume:", 8, 1);
            Gui.Print(Colour.Blue, " Sound volume:", 9, 1);

            Gui.Print(Colour.Green, "Sample Text", 12, 8);
            Gui.Print(Colour.Green, "===========", 13, 8);
            Gui.Print(Colour.Red, "Red", 15, 1);
            Gui.Print(Colour.BrightRed, "Bright Red", 15, 15);
            Gui.Print(Colour.Orange, "Orange", 16, 1);
            Gui.Print(Colour.BrightOrange, "Bright Orange", 16, 15);
            Gui.Print(Colour.Yellow, "Yellow", 17, 1);
            Gui.Print(Colour.BrightYellow, "Bright Yellow", 17, 15);
            Gui.Print(Colour.Chartreuse, "Chartreuse", 18, 1);
            Gui.Print(Colour.BrightChartreuse, "Bright Chartreuse", 18, 15);
            Gui.Print(Colour.Green, "Green", 19, 1);
            Gui.Print(Colour.BrightGreen, "Bright Green", 19, 15);
            Gui.Print(Colour.Turquoise, "Turquoise", 20, 1);
            Gui.Print(Colour.BrightTurquoise, "Bright Turquoise", 20, 15);
            Gui.Print(Colour.Blue, "Blue", 21, 1);
            Gui.Print(Colour.BrightBlue, "Bright Blue", 21, 15);
            Gui.Print(Colour.Purple, "Purple", 22, 1);
            Gui.Print(Colour.BrightPurple, "Bright Purple", 22, 15);
            Gui.Print(Colour.Pink, "Pink", 23, 1);
            Gui.Print(Colour.BrightPink, "Bright Pink", 23, 15);
            Gui.Print(Colour.Brown, "Brown", 25, 1);
            Gui.Print(Colour.BrightBrown, "Bright Brown", 25, 15);
            Gui.Print(Colour.Beige, "Beige", 26, 1);
            Gui.Print(Colour.BrightBeige, "Bright Beige", 26, 15);
            Gui.Print(Colour.Black, "Black", 28, 1);
            Gui.Print(Colour.Grey, "Grey", 29, 1);
            Gui.Print(Colour.BrightGrey, "Bright Grey", 30, 1);
            Gui.Print(Colour.White, "White", 31, 1);
            Gui.Print(Colour.BrightWhite, "Bright White", 32, 1);
            Gui.Print(Colour.Copper, "Copper", 28, 15);
            Gui.Print(Colour.Silver, "Silver", 29, 15);
            Gui.Print(Colour.Gold, "Gold", 30, 15);
            Gui.Print(Colour.Diamond, "Diamond", 31, 15);
            Gui.Print(Colour.Black, "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG", 35, 1);
            Gui.Print(Colour.Black, "the quick brown fox jumps over the lazy dog", 36, 1);
            Gui.Print(Colour.Black, "1234567890 !\"$%^&·*()_+-={}[]<>.,/?:@~;'#\\|", 37, 1);

            Gui.Print(Colour.Green, "Sample Layout", 1, 58);
            Gui.Print(Colour.Green, "=============", 2, 58);
            Gui.Print(Colour.Grey, "                   # #       ", 4, 50);
            Gui.Print(Colour.Grey, "            ########+########", 5, 50);
            Gui.Print(Colour.Grey, "            #      + +       ", 6, 50);
            Gui.Print(Colour.Grey, "            # ###############", 7, 50);
            Gui.Print(Colour.Grey, "            # #              ", 8, 50);
            Gui.Print(Colour.Grey, "      #######·########       ", 9, 50);
            Gui.Print(Colour.Grey, "      #··············#       ", 10, 50);
            Gui.Print(Colour.Grey, "      #·!·······@····#       ", 11, 50);
            Gui.Print(Colour.Grey, "#######···o··········#       ", 12, 50);
            Gui.Print(Colour.Grey, "··^·k·'··············#       ", 13, 50);
            Gui.Print(Colour.Grey, "#######·oo······$····#       ", 14, 50);
            Gui.Print(Colour.Grey, "      #··o···········#       ", 15, 50);
            Gui.Print(Colour.Grey, "      #######c########       ", 16, 50);
            Gui.Print(Colour.Grey, "            #·#              ", 17, 50);
            Gui.Print(Colour.Grey, "            #·#######        ", 18, 50);
            Gui.Print(Colour.Grey, "            #·······#        ", 19, 50);
            Gui.Print(Colour.Grey, "            #######·#        ", 20, 50);
            Gui.Print(Colour.Grey, "                  #'#########", 21, 50);
            Gui.Print(Colour.Grey, "                  #·'········", 22, 50);
            Gui.Print(Colour.Grey, "                  #'#########", 23, 50);
            Gui.Print(Colour.Grey, "                  #·#        ", 24, 50);
            Gui.Print(Colour.Grey, "      #############·#        ", 25, 50);
            Gui.Print(Colour.Grey, "      #·············#        ", 26, 50);
            Gui.Print(Colour.Grey, "      *·#############        ", 27, 50);
            Gui.Print(Colour.Grey, "      #·# ################   ", 28, 50);
            Gui.Print(Colour.Grey, "      #·###·····>········#   ", 29, 50);
            Gui.Print(Colour.Grey, "      #···'··········^···#   ", 30, 50);
            Gui.Print(Colour.Grey, "      #####·······|······#   ", 31, 50);
            Gui.Print(Colour.Grey, "          #·~············#   ", 32, 50);
            Gui.Print(Colour.Grey, "          #··············#   ", 33, 50);
            Gui.Print(Colour.Grey, "          ######'############", 34, 50);
            Gui.Print(Colour.Grey, "               #·##··········", 35, 50);
            Gui.Print(Colour.Grey, "               #····#########", 36, 50);
            Gui.Print(Colour.Grey, "               ######        ", 37, 50);

            Gui.Print(Colour.Brown, "+", 5, 70);
            Gui.Print(Colour.Brown, "+ +", 6, 69);
            Gui.Print(Colour.White, "#######·########       ", 9, 56);
            Gui.Print(Colour.White, "#··············#       ", 10, 56);
            Gui.Print(Colour.White, "#·!·······@····#       ", 11, 56);
            Gui.Print(Colour.White, "#···o··········#       ", 12, 56);
            Gui.Print(Colour.White, "^·k·'··············#       ", 13, 52);
            Gui.Print(Colour.White, "#######·oo······$····#       ", 14, 50);
            Gui.Print(Colour.White, "      #··o···········#       ", 15, 50);
            Gui.Print(Colour.White, "      #######c########       ", 16, 50);
            Gui.Print(Colour.BrightYellow, "···", 10, 65);
            Gui.Print(Colour.BrightYellow, "·@·", 11, 65);
            Gui.Print(Colour.BrightYellow, "···", 12, 65);
            Gui.Print(Colour.BrightWhite, "@", 11, 66);
            Gui.Print(Colour.Orange, "!", 11, 58);
            Gui.Print(Colour.BrightGreen, "o", 12, 60);
            Gui.Print(Colour.Red, "^", 13, 52);
            Gui.Print(Colour.Green, "k", 13, 54);
            Gui.Print(Colour.BrightBrown, "'", 13, 56);
            Gui.Print(Colour.Black, "oo", 14, 58);
            Gui.Print(Colour.Copper, "$", 14, 66);
            Gui.Print(Colour.Black, "o", 15, 59);
            Gui.Print(Colour.BrightBlue, "c", 16, 63);
            Gui.Print(Colour.White, "#", 17, 62);
            Gui.Print(Colour.Black, "·", 17, 63);
            Gui.Print(Colour.Black, "·", 18, 63);
            Gui.Print(Colour.Black, "·······", 19, 63);
            Gui.Print(Colour.Black, "·", 20, 69);
            Gui.Print(Colour.Brown, "'", 21, 69);
            Gui.Print(Colour.Black, "·'········", 22, 69);
            Gui.Print(Colour.Brown, "'", 22, 70);
            Gui.Print(Colour.Brown, "'", 23, 69);
            Gui.Print(Colour.Black, "·", 24, 69);
            Gui.Print(Colour.Black, "·", 25, 69);
            Gui.Print(Colour.Black, "·············", 26, 57);
            Gui.Print(Colour.Red, "*", 27, 56);
            Gui.Print(Colour.Black, "·", 27, 57);
            Gui.Print(Colour.Black, "·", 28, 57);
            Gui.Print(Colour.Black, "·", 29, 57);
            Gui.Print(Colour.Brown, ">", 29, 66);
            Gui.Print(Colour.Black, "···", 30, 57);
            Gui.Print(Colour.Brown, "'", 30, 60);
            Gui.Print(Colour.Purple, "^", 30, 71);
            Gui.Print(Colour.BrightWhite, "|", 31, 68);
            Gui.Print(Colour.Beige, "~", 32, 62);
            Gui.Print(Colour.Brown, "'", 34, 66);
            Gui.Print(Colour.Black, "·", 35, 66);
            Gui.Print(Colour.Black, "··········", 35, 69);
            Gui.Print(Colour.Black, "····", 36, 66);
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
                                Gui.ShowManual();
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
                                Gui.ShowManual();
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