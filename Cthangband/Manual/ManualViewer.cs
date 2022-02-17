using System;
using System.IO;
using System.Windows.Forms;

namespace Cthangband.Manual
{
    internal partial class ManualViewer : Form
    {
        private Uri _indexUri;
        private bool _suppressList;

        public ManualViewer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void FillList()
        {
            string curDir = Application.ExecutablePath;
            curDir = Path.GetDirectoryName(curDir);
            curDir = Path.Combine(curDir, "Manual");
            DirectoryInfo dir = new DirectoryInfo(curDir);
            FileInfo[] files = dir.GetFiles("*.html");
            foreach (FileInfo file in files)
            {
                string name = file.Name;
                name = Path.GetFileNameWithoutExtension(name);

                listBox1.Items.Add(name);
            }
        }

        private void GoHome()
        {
            webBrowser1.Navigate(_indexUri);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressList)
            {
                return;
            }
            string curDir = Application.ExecutablePath;
            curDir = Path.GetDirectoryName(curDir);
            curDir = Path.Combine(curDir, "Manual");
            string name = listBox1.SelectedItem.ToString().Replace(" ", "%20");
            curDir = Path.Combine(curDir, $"{name}.html");
            webBrowser1.Navigate(new Uri(string.Format("file:///{0}", curDir)));
        }

        private void ManualViewer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                Close();
            }
        }

        private void ManualViewer_Load(object sender, EventArgs e)
        {
            button1.Image = Properties.Resources.Home.ToBitmap();
            button2.Image = Properties.Resources.Forward.ToBitmap();
            button3.Image = Properties.Resources.Back.ToBitmap();
            webBrowser1.CanGoBackChanged += WebBrowser1_CanGoBackChanged;
            webBrowser1.CanGoForwardChanged += WebBrowser1_CanGoForwardChanged;
            webBrowser1.Navigated += WebBrowser1_Navigated;
            webBrowser1.IsWebBrowserContextMenuEnabled = false;
            webBrowser1.AllowWebBrowserDrop = false;
            string curDir = Application.ExecutablePath;
            curDir = Path.GetDirectoryName(curDir);
            curDir = Path.Combine(curDir, "Manual");
            curDir = Path.Combine(curDir, "Introduction.html");
            _indexUri = new Uri(string.Format("file:///{0}", curDir));
            FillList();
            GoHome();
        }

        private void WebBrowser1_CanGoBackChanged(object sender, EventArgs e)
        {
            button3.Enabled = webBrowser1.CanGoBack;
        }

        private void WebBrowser1_CanGoForwardChanged(object sender, EventArgs e)
        {
            button2.Enabled = webBrowser1.CanGoForward;
        }

        private void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string fileName = Path.GetFileNameWithoutExtension(e.Url.LocalPath);
            fileName = fileName.Replace("%20", " ");
            _suppressList = true;
            listBox1.Text = fileName;
            _suppressList = false;
        }
    }
}