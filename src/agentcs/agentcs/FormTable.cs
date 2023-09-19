using System.Windows.Forms;

namespace agentcs
{
    public partial class FormTable : Form
    {
        private int pageNo = 0;
        public PictureBox[] bgTableNo = new PictureBox[12];
        public Label[] lbTableNo = new Label[12];
        string table_nm = "";
        public MainForm? mainForm;


        public FormTable()
        {
            InitializeComponent();

            picButtonClose.Location = new Point(520, 10);

            labelTableName.Location = new Point(206, 34);

            labelResetConfirm.Text = "위 테이블의 주문 내역을 초기화 하시겠습니까?";
            labelResetConfirm.Location = new Point(142, 121);

            labelResetDesc.Text = "POS 주문은 삭제되지 않으며, 손님이 보고 있는 모바일 메뉴판의 테이블 번호만 초기화됩니다.\r\n손님이 테이블을 이동한 경우, 잊지 말고 꼭! 초기화 해주세요.";
            labelResetDesc.Location = new Point(66, 237);

            picButtonReset.Location = new Point(125, 157);
            picButtonKeep.Location = new Point(305, 157);

            labelSelectTable.Text = "테이블을 선택해주세요";
            labelSelectTable.Location = new Point(40, 20);
            picButtonPrev.Location = new Point(40, 246);
            picButtonNext.Location = new Point(400, 246);

            CreateTableUI();

            SetUI(0);
        }

        private void picButtonReset_Click(object sender, EventArgs e)
        {
            MainForm form = (MainForm)this.mainForm!;
            string table_cd = form.GetTableCodeByName(table_nm);
            form.SendClear(0, table_cd);
            SetUI(0);
            Close();
        }

        private void picButtonKeep_Click(object sender, EventArgs e)
        {
            SetUI(0);
            Close();
        }

        private void SetUI(int no)
        {
            switch (no)
            {
                case 0:
                    pageNo = 0;
                    table_nm = "";
                    labelTableName.Visible = false;
                    labelResetConfirm.Visible = false;
                    labelResetDesc.Visible = false;
                    picButtonReset.Visible = false;
                    picButtonKeep.Visible = false;

                    labelSelectTable.Visible = true;
                    picButtonPrev.Visible = true;
                    picButtonNext.Visible = true;

                    for (int i = 0; i < 12; i++)
                    {
                        bgTableNo[i].Visible = true;
                    }

                    break;

                case 1:
                    labelTableName.Visible = true;
                    labelResetConfirm.Visible = true;
                    labelResetDesc.Visible = true;
                    picButtonReset.Visible = true;
                    picButtonKeep.Visible = true;

                    labelSelectTable.Visible = false;
                    picButtonPrev.Visible = false;
                    picButtonNext.Visible = false;

                    for (int i = 0; i < 12; i++)
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

            for (int r = 0; r < 12; r++)
            {
                bgTableNo[r] = new()
                {
                    Image = (Bitmap)Properties.Resources.buttn_tableno,
                    Size = new Size(170, 38),
                    Visible = true
                };

                this.Controls.Add(bgTableNo[r]);

                lbTableNo[r] = new()
                {
                    Text = "00",
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    //Font = font,
                    Size = new Size(120, 30),
                    Location = new Point((170 / 2) - (120 / 2), (38 / 2) - (30 / 2)),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.FromArgb(135, 135, 135),
                    //BackColor = Color.FromArgb(135, 0, 0),
                    ForeColor = Color.White
                };

                bgTableNo[r].Controls.Add(lbTableNo[r]);
                lbTableNo[r].Click += new EventHandler(DynamicButtonClickHandler!);
            }

            int marginLeft = 40;
            int marginTop = 55;
            //int marginX = 50;
            //int marginY = 100;

            int no = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    bgTableNo[no].Location = new Point(marginLeft + (j * 180), marginTop + (i * 47));
                    lbTableNo[no].Text = (no + 1).ToString();
                    no++;
                }
            }
        }

        public void SetTableData()
        {
            MainForm form = (MainForm)this.mainForm!;

            int startno = (pageNo * 12);
            int totalno = form.dicScdTable.Count;
            int lastno = (12 * pageNo) + 12;

            double c = (double)totalno / ((pageNo+1) * 12);
            double quotient = System.Math.Truncate(c);
            double remainder = totalno % 12;
            
            if (quotient == 0)
            {
                lastno = (12 * pageNo) + (int)remainder;
            }
            
            //Console.WriteLine($"SetTableData - startno: {startno}, totalno: {totalno}");
            //Console.WriteLine($"SetTableData - quotient: {quotient}, remainder: {remainder}");
            //Console.WriteLine($"SetTableData - startno: {startno}, lastno: {lastno}");

            for (int i=0; i<12; i++)
            {
                lbTableNo[i].Text = "";
            }

            int j = 0;
            for (int idx= startno; idx<lastno; idx++)
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
            if ((pageNo + 1) * 12 > form.dicScdTable.Count)
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
            labelTableName.Text = "테이블 [" + table_nm + "]";

            SetUI(1);
        }

        private void picButtonClose_Click(object sender, EventArgs e)
        {
            SetUI(0);
            Close();
        }
    }
}
