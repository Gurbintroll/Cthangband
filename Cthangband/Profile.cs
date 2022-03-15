// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cthangband
{
    [Serializable]
    internal class Profile
    {
        public ExPlayer ExPlayer;
        public FixedArtifactArray FixedArtifacts;
        public ItemTypeArray ItemTypes;
        public MonsterRaceArray MonsterRaces;
        public bool MsgFlag;
        public RareItemTypeArray RareItemTypes;
        public VaultTypeArray VaultTypes;

        private readonly List<string> _messageBuf = new List<string>();
        private readonly List<int> _messageCounts = new List<int>();
        private int _msgPrintP;

        public static Profile Instance
        {
            get; private set;
        }

        public SaveGame Game
        {
            get; private set;
        }

        public static void LoadOrCreate(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Exists)
            {
                Instance = Program.DeserializeFromSaveFolder<Profile>(fileName);
            }
            else
            {
                Instance = new Profile();
                Instance.Initialise();
            }
            SaveGame.Instance = Instance.Game;
        }

        public void MessageAdd(string str)
        {
            // simple case - list is empty
            if (_messageBuf.Count == 0)
            {
                _messageBuf.Add(str);
                _messageCounts.Add(1);
                return;
            }

            // If it's not blank it might be a repeat
            if (!string.IsNullOrEmpty(str))
            {
                if (_messageBuf[_messageBuf.Count - 1] == str)
                {
                    // Same as last - just increment the count
                    _messageCounts[_messageCounts.Count - 1]++;
                    return;
                }
            }

            // We're still here, so we just add ourselves
            _messageBuf.Add(str);
            _messageCounts.Add(1);
            // Limit the size
            if (_messageBuf.Count > 2048)
            {
                _messageBuf.RemoveAt(0);
                _messageCounts.RemoveAt(0);
            }
        }

        public int MessageNum()
        {
            return _messageBuf.Count;
        }

        public string MessageStr(int age)
        {
            if (age >= _messageBuf.Count)
            {
                return string.Empty;
            }
            string message = _messageBuf[_messageBuf.Count - age - 1];
            int count = _messageCounts[_messageCounts.Count - age - 1];
            if (count > 1)
            {
                message += $" (x{count})";
            }
            return message;
        }

        public void MsgPrint(string msg)
        {
            if (!MsgFlag)
            {
                _msgPrintP = 0;
            }
            int n = string.IsNullOrEmpty(msg) ? 0 : msg.Length;
            if (_msgPrintP != 0 && (string.IsNullOrEmpty(msg) || _msgPrintP + n > 72))
            {
                MsgFlush(_msgPrintP);
                MsgFlag = false;
                _msgPrintP = 0;
            }
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            if (msg.Length > 2)
            {
                msg = msg.Substring(0, 1).ToUpper() + msg.Substring(1);
            }
            if (n > 1000)
            {
                return;
            }
            if (Game.Player != null)
            {
                MessageAdd(msg);
            }
            string buf = msg;
            string t = buf;
            while (n > 72)
            {
                int split = 72;
                for (int check = 40; check < 72; check++)
                {
                    if (t[check] == ' ')
                    {
                        split = check;
                    }
                }
                Gui.Print(Colour.White, t.Substring(0, split), 0, 0, split);
                MsgFlush(split + 1);
                t = t.Substring(split);
                n -= split;
            }
            Gui.Print(Colour.White, t, 0, _msgPrintP, n);
            MsgFlag = true;
            _msgPrintP += n + 1;
        }

        public void Run()
        {
            MsgFlag = false;
            MsgPrint(null);
            MsgFlag = false;
            // Create a new game if necessary
            if (Game == null)
            {
                SaveGame game = new SaveGame();
                game.Initialise();
                Game = game;
            }

            // Set a globally accessible reference to our game
            SaveGame.Instance = Game;
            // And play it!
            Game.Play();
            // Remove the global reference
            SaveGame.Instance = null;
        }

        private void Initialise()
        {
            GlobalData.PopulateNewProfile(this);
        }

        private void MsgFlush(int x)
        {
            const Colour a = Colour.BrightBlue;
            Gui.Print(a, "-more-", 0, x);
            while (true)
            {
                Gui.Inkey();
                break;
            }
            Gui.Erase(0, 0, 255);
        }
    }
}