namespace Cthangband.UI
{
    partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this._SizeCombo = new System.Windows.Forms.ComboBox();
            this._fullscreenButton = new System.Windows.Forms.RadioButton();
            this._windowedButton = new System.Windows.Forms.RadioButton();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._soundSlider = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._musicSlider = new System.Windows.Forms.TrackBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._soundSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._musicSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 326);
            this.progressBar1.Maximum = 16;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(535, 21);
            this.progressBar1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(553, 325);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _SizeCombo
            // 
            this._SizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._SizeCombo.FormattingEnabled = true;
            this._SizeCombo.Location = new System.Drawing.Point(99, 40);
            this._SizeCombo.Name = "_SizeCombo";
            this._SizeCombo.Size = new System.Drawing.Size(257, 21);
            this._SizeCombo.TabIndex = 2;
            // 
            // _fullscreenButton
            // 
            this._fullscreenButton.AutoSize = true;
            this._fullscreenButton.ForeColor = System.Drawing.Color.LimeGreen;
            this._fullscreenButton.Location = new System.Drawing.Point(6, 19);
            this._fullscreenButton.Name = "_fullscreenButton";
            this._fullscreenButton.Size = new System.Drawing.Size(73, 17);
            this._fullscreenButton.TabIndex = 3;
            this._fullscreenButton.TabStop = true;
            this._fullscreenButton.Text = "Fullscreen";
            this._fullscreenButton.UseVisualStyleBackColor = true;
            this._fullscreenButton.CheckedChanged += new System.EventHandler(this._fullscreenButton_CheckedChanged);
            // 
            // _windowedButton
            // 
            this._windowedButton.AutoSize = true;
            this._windowedButton.ForeColor = System.Drawing.Color.LimeGreen;
            this._windowedButton.Location = new System.Drawing.Point(6, 41);
            this._windowedButton.Name = "_windowedButton";
            this._windowedButton.Size = new System.Drawing.Size(76, 17);
            this._windowedButton.TabIndex = 4;
            this._windowedButton.TabStop = true;
            this._windowedButton.Text = "Windowed";
            this._windowedButton.UseVisualStyleBackColor = true;
            // 
            // fontDialog1
            // 
            this.fontDialog1.AllowScriptChange = false;
            this.fontDialog1.AllowVerticalFonts = false;
            this.fontDialog1.FixedPitchOnly = true;
            this.fontDialog1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialog1.FontMustExist = true;
            this.fontDialog1.ShowEffects = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Location = new System.Drawing.Point(333, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._fullscreenButton);
            this.groupBox1.Controls.Add(this._windowedButton);
            this.groupBox1.Controls.Add(this._SizeCombo);
            this.groupBox1.ForeColor = System.Drawing.Color.LimeGreen;
            this.groupBox1.Location = new System.Drawing.Point(12, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 70);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.ForeColor = System.Drawing.Color.LimeGreen;
            this.groupBox2.Location = new System.Drawing.Point(12, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(362, 49);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Font";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(321, 20);
            this.textBox1.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._soundSlider);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this._musicSlider);
            this.groupBox3.ForeColor = System.Drawing.Color.LimeGreen;
            this.groupBox3.Location = new System.Drawing.Point(12, 217);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(362, 107);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Volume";
            // 
            // _soundSlider
            // 
            this._soundSlider.Location = new System.Drawing.Point(55, 60);
            this._soundSlider.Maximum = 100;
            this._soundSlider.Name = "_soundSlider";
            this._soundSlider.Size = new System.Drawing.Size(301, 45);
            this._soundSlider.TabIndex = 3;
            this._soundSlider.TickFrequency = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sounds";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Music";
            // 
            // _musicSlider
            // 
            this._musicSlider.Location = new System.Drawing.Point(55, 15);
            this._musicSlider.Maximum = 100;
            this._musicSlider.Name = "_musicSlider";
            this._musicSlider.Size = new System.Drawing.Size(301, 45);
            this._musicSlider.TabIndex = 0;
            this._musicSlider.TickFrequency = 5;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::Cthangband.Properties.Resources.SplashScreen;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._soundSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._musicSlider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox _SizeCombo;
        private System.Windows.Forms.RadioButton _fullscreenButton;
        private System.Windows.Forms.RadioButton _windowedButton;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TrackBar _soundSlider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar _musicSlider;
    }
}