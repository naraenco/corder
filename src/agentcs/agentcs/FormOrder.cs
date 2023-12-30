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

        //private const int WM_NCLBUTTONDOWN = 0xA1;
        //private const int HT_CAPTION = 0x2; const int WH_KEYBOARD_LL = 13;

        string tableName = String.Empty;
        string datetime = String.Empty;
        List<string> productList = new List<string>();
        List<string> qtyList = new List<string>();
        int total = 0;
        int pageNo = 0;
        int totalPage = 0;

        public MainForm? mainForm;

        List<OrderData> orderList;


        public FormOrder()
        {
            InitializeComponent();
            this.Paint += FormOrder_Paint;
            this.MouseDown += FormOrder_MouseDown;

            orderList = new List<OrderData>();

            lbTableName.Parent = picTitlebar;
            lbTableName.BackColor = Color.Transparent;
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            Log.Debug("FormOrder_Load");
            SetData();
        }

        private void FormOrder_MouseDown(object? sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    IntPtr handle = this.Handle;
            //    ReleaseCapture();
            //    SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            //}
        }

        private void FormOrder_Paint(object? sender, PaintEventArgs e)
        {
            //using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            //{
            //    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            //}

        }

        private void picConfirm_Click(object sender, EventArgs e)
        {
            if (orderList.Count == 0)
            {
                Hide();
            }
            else
            {
                SetData();
            }
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


        public void AddData(string tableName,
            string datetime,
            List<string> productList,
            List<string> qtyList)
        {
            OrderData order = new();
            order.tableName = tableName;
            order.datetime = datetime;
            order.productList = productList;
            order.qtyList = qtyList;

            orderList.Add(order);

            lbTotalOrder.Text = orderList.Count.ToString();
        }

        public void SetData()
        {
            if (orderList.Count == 0) return;

            OrderData order = orderList[0];
            pageNo = 0;

            lbTableName.Text = String.Empty;
            lbDateTime.Text = String.Empty;
            lbMenu.Text = String.Empty;
            lbQty.Text = String.Empty;

            this.tableName = order.tableName;
            this.datetime = order.datetime;
            this.productList = order.productList;
            this.qtyList = order.qtyList;
            this.total = productList.Count;

            if (this.total % 10 == 0)
            {
                this.totalPage = this.total / 10;
            }
            else
            {
                this.totalPage = this.total / 10 + 1;
            }
            orderList.RemoveAt(0);
            lbTotalOrder.Text = orderList.Count.ToString();
            ShowMenu();
        }

        public void ShowMenu()
        {
            lbPage.Text = (pageNo + 1).ToString() + " / " + totalPage.ToString();

            string menus = String.Empty;
            string qtys = String.Empty;

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
                    menus = menus + this.productList[i] + "\r\n";
                    qtys = qtys + this.qtyList[i] + "\r\n";
                }

                lbTableName.Text = this.tableName + " 테이블";
                lbDateTime.Text = this.datetime;
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

        private void picLogo_Click(object sender, EventArgs e)
        {
            //Point parentPoint = mainForm!.Location;
            //this.Location = parentPoint;
            //this.Top += 54;
        }
    }

    public class OrderData
    {
        public string tableName = String.Empty;
        public string datetime = String.Empty;
        public List<string> productList = new List<string>();
        public List<string> qtyList = new List<string>();
    }
}
