using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace agentcs
{
    public partial class FormLogin : Form
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2; const int WH_KEYBOARD_LL = 13;

        private int currentInput = 0;
        public MainForm? mainForm;


        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.Paint += FormLogin_Paint;
            this.MouseDown += FormLogin_MouseDown;

            picLogo.Location = new Point(49, 15);

            // login
            textID.AutoSize = false;
            textPW.AutoSize = false;
            textID.Size = new Size(100, 24);
            textPW.Size = new Size(100, 24);
            textID.Location = new Point(315, 12);
            textPW.Location = new Point(495, 12);
            picBtnLogin.Location = new Point(626, 11);

            //textID.PlaceholderText = "ID를 입력하세요";
            //textPW.PlaceholderText = "암호를 입력하세요";

            //Font font = new Font(FontManager.fontFamilys[0], 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            //textID.Font = font;
            //textPW.Font = font;

            this.ActiveControl = null;
        }

        private void FormLogin_Paint(object? sender, PaintEventArgs e)
        {
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();

            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(255, 239, 237, 238));
            formGraphics.FillRectangle(myBrush, new Rectangle(0, 44, 701, 9));

            myBrush = new System.Drawing.SolidBrush(Color.FromArgb(255, 251, 244, 235));
            formGraphics.FillRectangle(myBrush, new Rectangle(470, 53, 231, 311));

            myBrush.Dispose();
            formGraphics.Dispose();

            //using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            //{
            //    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            //}
        }

        private void FormLogin_MouseDown(object? sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    MainForm form = (MainForm)this.mainForm!;
            //    IntPtr handle = form.Handle;
            //    ReleaseCapture();
            //    SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            //}
        }

        private void NumClick(string value)
        {
            if (currentInput == 0)
            {
                textID.Text += value;
                textID.SelectionStart = textID.Text.Length;
                textID.ScrollToCaret();
            }
            else if (currentInput == 1)
            {
                textPW.Text += value;
                textPW.SelectionStart = textID.Text.Length;
                textPW.ScrollToCaret();
            }
        }

        private void picNo0_Click(object sender, EventArgs e)
        {
            NumClick("0");
        }

        private void picNo1_Click(object sender, EventArgs e)
        {
            NumClick("1");
        }

        private void picNo2_Click(object sender, EventArgs e)
        {
            NumClick("2");
        }

        private void picNo3_Click(object sender, EventArgs e)
        {
            NumClick("3");
        }

        private void picNo4_Click(object sender, EventArgs e)
        {
            NumClick("4");
        }

        private void picNo5_Click(object sender, EventArgs e)
        {
            NumClick("5");
        }

        private void picNo6_Click(object sender, EventArgs e)
        {
            NumClick("6");
        }

        private void picNo7_Click(object sender, EventArgs e)
        {
            NumClick("7");
        }

        private void picNo8_Click(object sender, EventArgs e)
        {
            NumClick("8");
        }

        private void picNo9_Click(object sender, EventArgs e)
        {
            NumClick("9");
        }

        private void picBack_Click(object sender, EventArgs e)
        {
            if (currentInput == 0)
            {
                if (textID.Text.Length == 0) return;
                string value = textID.Text;
                textID.Text = value.Substring(0, value.Length - 1);
                textID.SelectionStart = textID.Text.Length;
                textID.ScrollToCaret();
            }
            else if (currentInput == 1)
            {
                if (textPW.Text.Length == 0) return;
                string value = textPW.Text;
                textPW.Text = value.Substring(0, value.Length - 1);
                textPW.SelectionStart = textID.Text.Length;
                textPW.ScrollToCaret();
            }
        }

        private void picClear_Click(object sender, EventArgs e)
        {
            if (currentInput == 0)
                textID.Text = "";
            else if (currentInput == 1)
                textPW.Text = "";
        }

        private void textID_Enter(object sender, EventArgs e)
        {
            currentInput = 0;
        }

        private void textPW_Enter(object sender, EventArgs e)
        {
            currentInput = 1;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void picBtnLogin_Click(object sender, EventArgs e)
        {
            MainForm form = (MainForm)this.Parent;
            form.SetDialog(1);

            Close();
        }
    }
}
