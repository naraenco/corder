using Serilog;
using Serilog.Core;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace agentcs
{
    public partial class FormOrder : Form
    {

        //[DllImport("user32.dll")]
        //private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //[DllImport("user32.dll")]
        //private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2; const int WH_KEYBOARD_LL = 13;

        List<string> productList;
        List<string> qtyList;
        string tableName = string.Empty;
        string datetime = string.Empty;
        int total = 0;
        int pageNo = 0;
        int totalPage = 0;


        public FormOrder()
        {
            InitializeComponent();
            this.Paint += FormOrder_Paint;
            //this.MouseDown += FormOrder_MouseDown;

            productList = new List<string>();
            qtyList = new List<string>();
        }

        //private void FormOrder_MouseDown(object? sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        IntPtr handle = this.Handle;
        //        ReleaseCapture();
        //        SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        //    }
        //}

        private void FormOrder_Paint(object? sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void picConfirm_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void picButtonPrev_Click(object sender, EventArgs e)
        {
            if (pageNo <= 0)
            {
                pageNo = 0;
                return;
            }
            pageNo -= 1;
            ShowMenu();
        }

        private void picButtonNext_Click(object sender, EventArgs e)
        {
            if ((pageNo + 1) * 10 > total)
            {
                return;
            }
            pageNo += 1;
            ShowMenu();
        }

        public void SetData(string tableName,
            string datetime,
            List<string> productList,
            List<string> qtyList)
        {
            this.tableName = tableName;
            this.datetime = datetime;
            this.productList = productList;
            this.qtyList = qtyList;
            this.total = productList.Count;

            if (this.total % 10 == 0)
            {
                this.totalPage = this.total / 10;
            }
            else
            {
                this.totalPage = this.total / 10 + 1;
            }

            ShowMenu();
        }

        public void ShowMenu()
        {
            lbPage.Text = (pageNo + 1).ToString() + " / " + totalPage.ToString();

            string menus = string.Empty;
            string qtys = string.Empty;

            int start = pageNo * 10;
            int end = (pageNo + 1) * 10;

            double c = (double)total / ((pageNo + 1) * 10);
            double quotient = System.Math.Truncate(c);
            double remainder = total % 10;

            try
            {
                if (quotient == 0)
                {
                    end = (10 * pageNo) + (int)remainder;
                }
                for (int i = start; i < end; i++)
                {
                    menus = menus + productList[i] + "\r\n";
                    qtys = qtys + qtyList[i] + "\r\n";
                }

                lbTableName.Text = tableName + " 테이블";
                lbDateTime.Text = datetime;
                lbMenu.Text = menus;
                lbQty.Text = qtys;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
            }
        }
    }
}

