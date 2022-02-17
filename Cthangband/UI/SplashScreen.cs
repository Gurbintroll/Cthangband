using Cthangband.StaticData;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Cthangband.UI
{
    public partial class SplashScreen : Form
    {
        private Settings _settings;

        public SplashScreen()
        {
            InitializeComponent();
        }

        public void OnProgress(int add)
        {
            System.Threading.Thread.Sleep(50);
            progressBar1.Value += add;
            Application.DoEvents();
        }

        private void _fullscreenButton_CheckedChanged(object sender, EventArgs e)
        {
            _SizeCombo.Enabled = _windowedButton.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_fullscreenButton.Checked)
            {
                _settings.Resolution = 0;
            }
            else
            {
                _settings.Resolution = _SizeCombo.SelectedIndex + 1;
            }
            _settings.MusicVolume = _musicSlider.Value;
            _settings.SoundVolume = _soundSlider.Value;
            Program.SerializeToSaveFolder(_settings, "game.settings");
            Gui.Initialise(_settings);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            Font font = fontDialog1.Font;
            string name = font.Name;
            if (font.Bold)
            {
                name += " Bold";
            }
            if (font.Italic)
            {
                name += " Italic";
            }
            textBox1.Text = name;
            _settings.Font = font.Name;
            _settings.Bold = font.Bold;
            _settings.Italic = font.Italic;
        }

        private void FillResolutions()
        {
            _settings = Program.DeserializeFromSaveFolder<Settings>("game.settings") ?? new Settings();
            int heightDifference = Height - ClientSize.Height;
            int widthDifference = Width - ClientSize.Width;
            Rectangle screenSize = System.Windows.Forms.Screen.FromHandle(Handle).Bounds;
            for (int i = 1; i < 200; i++)
            {
                Size windowSize = new Size((i + 2) * 160, (i + 2) * 90);
                Size totalSize = new Size(windowSize.Width + widthDifference, windowSize.Height + heightDifference);
                if (totalSize.Width < screenSize.Width && totalSize.Height < screenSize.Height)
                {
                    _SizeCombo.Items.Add($"{windowSize.Width}x{windowSize.Height}");
                }
                else
                {
                    break;
                }
            }
            if (_settings.Resolution == 0)
            {
                _fullscreenButton.Checked = true;
                _SizeCombo.SelectedIndex = _SizeCombo.Items.Count - 1;
            }
            else
            {
                _windowedButton.Checked = true;
                _SizeCombo.SelectedIndex = Math.Min(_settings.Resolution, _SizeCombo.Items.Count) - 1;
            }
            Font font = FontFromSettings();
            string name = font.Name;
            if (font.Bold)
            {
                name += " Bold";
            }
            if (font.Italic)
            {
                name += " Italic";
            }
            textBox1.Text = name;
            fontDialog1.Font = font;
            _musicSlider.Value = _settings.MusicVolume;
            _soundSlider.Value = _settings.SoundVolume;
        }

        private Font FontFromSettings()
        {
            FontStyle fontStyle = FontStyle.Regular;
            if (_settings.Bold)
            {
                fontStyle |= FontStyle.Bold;
            }
            if (_settings.Italic)
            {
                fontStyle |= FontStyle.Italic;
            }
            Font font = new Font(_settings.Font, 10, fontStyle);
            if (font.Name != _settings.Font)
            {
                font = new Font("Consolas", 10, fontStyle);
                if (font.Name != "Consolas")
                {
                    font = new Font("Courier New", 10, fontStyle);
                }
            }
            return font;
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            Text = Constants.VersionStamp;
            ClientSize = new Size(640, 360);
            Program.GetDefaultFolder();
            if (!Program.DirCreate(Program.SaveFolder))
            {
                Program.Quit($"Cannot create '{Program.SaveFolder}'");
            }
            FillResolutions();
            Application.DoEvents();
            StaticResources.LoadOrCreate(OnProgress);
            Program.HiScores = new HighScoreTable();
            button1.Enabled = true;
        }
    }
}