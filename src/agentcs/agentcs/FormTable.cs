using System.Windows.Forms;

namespace agentcs
{
    public partial class FormTable : Form
    {
        private int pageNo = 0;
        //public PictureBox[] bgTableNo = new PictureBox[10];
        public Label[] bgTableNo = new Label[10];
        public Label[] lbTableNo = new Label[10];
        string table_nm = "";
        public MainForm? mainForm;


        public FormTable()
        {
            InitializeComponent();
            this.Paint += FormTable_Paint;

            labelResetDesc.Text = "*손님이 주문 전 테이블 이동한 경우, 반드시 초기화 해주세요.";
            labelResetDesc.Location = new Point(29, 58);

            picButtonPrev.Location = new Point(20, 420);
            picButtonNext.Location = new Point(310, 420);

            picButtonClose.Location = new Point(0, 470);

            CreateTableUI();
        }

        private void FormTable_Paint(object? sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        //private void picButtonReset_Click(object sender, EventArgs e)
        //{
        //    MainForm form = (MainForm)this.mainForm!;
        //    string table_cd = form.GetTableCodeByName(table_nm);
        //    form.SendClear(0, table_cd);
        //    Close();
        //}

        private void SetUI(int no)
        {
            switch (no)
            {
                case 0:
                    pageNo = 0;
                    table_nm = "";
                    labelResetDesc.Visible = true;

                    picButtonPrev.Visible = true;
                    picButtonNext.Visible = true;

                    for (int i = 0; i < 10; i++)
                    {
                        bgTableNo[i].Visible = true;
                    }

                    break;

                case 1:
                    labelResetDesc.Visible = false;
                    
                    picButtonPrev.Visible = false;
                    picButtonNext.Visible = false;

                    for (int i = 0; i < 10; i++)
                    {
                        bgTableNo[i].Visible = false;
                    }

                    break;
            }
            this.ActiveControl = null;
        }

        private void CreateTableUI()
        {
            //Font font = new Font(FontManager.fontFamilys[0], 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            Color lineColor;

            for (int r = 0; r < 10; r++)
            {
                if (r % 2 == 1)
                {
                    lineColor = Color.FromArgb(246, 246, 246);
                }
                else
                {
                    lineColor = Color.FromArgb(255, 255, 255);
                }

                bgTableNo[r] = new()
                {
                    Text = "",
                    BackColor = lineColor,
                    Size = new Size(400, 30),
                    Visible = true
                };

                this.Controls.Add(bgTableNo[r]);

                Font lbFont = new Font("맑은 고딕", 11);

                lbTableNo[r] = new()
                {
                    Font = lbFont,
                    Text = "00",
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Size = new Size(360, 30),
                    Location = new Point(22, 0),
                    BorderStyle = BorderStyle.None,
                    BackColor = lineColor,
                    ForeColor = Color.Black
                };

                bgTableNo[r].Controls.Add(lbTableNo[r]);
                lbTableNo[r].Click += new EventHandler(DynamicButtonClickHandler!);
            }

            int marginLeft = 0;
            int marginTop = 100;

            int no = 0;
            for (int i = 0; i < 10; i++)
            {
                bgTableNo[no].Location = new Point(marginLeft, marginTop + (i * 30));
                lbTableNo[no].Text = (no + 1).ToString();
                no++;
            }
        }

        public void SetTableData()
        {
            MainForm form = (MainForm)this.mainForm!;

            int startno = (pageNo * 10);
            int totalno = form.dicScdTable.Count;
            int lastno = (10 * pageNo) + 10;
            int totalPage = 0;

            if (form.dicScdTable.Count % 10 == 0)
            {
                totalPage = form.dicScdTable.Count / 10;
            }
            else
            {
                totalPage = form.dicScdTable.Count / 10 + 1;
            }

            lbPage.Text = (pageNo + 1).ToString() + " / " + totalPage.ToString();

            double c = (double)totalno / ((pageNo + 1) * 10);
            double quotient = System.Math.Truncate(c);
            double remainder = totalno % 10;

            if (quotient == 0)
            {
                lastno = (10 * pageNo) + (int)remainder;
            }

            //Console.WriteLine($"SetTableData - startno: {startno}, totalno: {totalno}");
            //Console.WriteLine($"SetTableData - quotient: {quotient}, remainder: {remainder}");
            //Console.WriteLine($"SetTableData - startno: {startno}, lastno: {lastno}");

            for (int i = 0; i < 10; i++)
            {
                lbTableNo[i].Text = "";
            }

            int j = 0;
            for (int idx = startno; idx < lastno; idx++)
            {
                var item = form.dicScdTable.ElementAt(idx);
                lbTableNo[j].Text = item.Key.ToString();
                j++;
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
            SetTableData();
        }

        private void picButtonNext_Click(object sender, EventArgs e)
        {
            MainForm form = (MainForm)this.mainForm!;
            if ((pageNo + 1) * 10 > form.dicScdTable.Count)
            {
                return;
            }
            pageNo += 1;
            SetTableData();
        }

        private void DynamicButtonClickHandler(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            table_nm = label.Text;
            FormClear formClear = new()
            {
                Owner = this,
                TopLevel = true
            };
            Point parentPoint = this.Location;
            formClear.Location = parentPoint;
            formClear.Left += 50;
            formClear.Top += 150;
            formClear.setTableName(table_nm);
            DialogResult result = formClear.ShowDialog();
            if (result == DialogResult.OK)
            {
                MainForm form = (MainForm)this.mainForm!;
                string table_cd = form.GetTableCodeByName(table_nm);
                form.SendClear(0, table_cd);
                Close();
            }
            else
            {
                table_nm = "";
            }
        }

        private void picButtonClose_Click(object sender, EventArgs e)
        {
            SetUI(0);
            Close();
        }

        private void FormTable_Load(object sender, EventArgs e)
        {

        }
    }
}
