using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace agentcs
{
    public partial class FormLogin : Form
    {
        private int currentInput = 0;
        public MainForm? mainForm;
        public string uid = String.Empty;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.Paint += FormLogin_Paint;

            picLogo.Location = new Point(49, 15);

            textID.AutoSize = false;
            textID.Height = 18;
            textPW.AutoSize = false;
            textPW.Height = 18;

            textID.Text = uid;
            //textPW.Text = "3694"; password

            //textID.PlaceholderText = "ID를 입력하세요";
            textPW.PlaceholderText = "암호 입력";

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
            string uid = textID.Text.Trim();
            string pwd = textPW.Text.Trim();

            if (uid.Length < 4)
            {
                MessageBox.Show(this, "아이디를 입력하세요");
                return;
            }
            if (pwd.Length < 4)
            {
                MessageBox.Show(this, "암호를 입력하세요");
                return;
            }

            mainForm?.LoginReq(uid, pwd);
        }

        private void picMenu_Click(object sender, EventArgs e)
        {
            mainForm?.PopupMenu();
        }

        public void ResultMessage(string message)
        {
            MessageBox.Show(this, message);
        }

        public void LoginSuccess()
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}

