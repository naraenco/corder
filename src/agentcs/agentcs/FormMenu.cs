using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace agentcs
{
    public partial class FormMenu : Form
    {
        public MainForm? mainForm;

        public FormMenu()
        {
            InitializeComponent();
            this.Paint += FormMenu_Paint;
            this.Focus();
        }

        private void FormMenu_Paint(object? sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void menuConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Config");
        }

        private void menuManager_Click(object sender, EventArgs e)
        {
            string command = $"start http://corder.co.kr/manager";

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = string.Format("/C {0}", command);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            mainForm?.Close();
            Close();
        }

        private void FormMenu_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
