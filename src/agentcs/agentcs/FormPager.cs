using Serilog;
using Serilog.Core;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Windows.Forms;


namespace agentcs
{
    public partial class FormPager : Form
    {
        string tableName = String.Empty;
        string datetime = String.Empty;
        bool bFold = false;

        public MainForm? mainForm;

        List<PagerData> pagerList;


        public FormPager()
        {
            InitializeComponent();
            this.Paint += FormPager_Paint;
            this.MouseDown += FormPager_MouseDown;

            pagerList = new List<PagerData>();

            lbTableName.Top = 10;
            lbTotalOrder.Top = 30;

            lbTableName.Parent = picTitlebar;
            lbTableName.BackColor = Color.Transparent;
        }

        private void FormPager_Load(object sender, EventArgs e)
        {
            Log.Debug("FormPager_Load");
            SetData();
        }

        private void FormPager_MouseDown(object? sender, MouseEventArgs e)
        {
        }

        private void FormPager_Paint(object? sender, PaintEventArgs e)
        {
        }

        private void picConfirm_Click(object sender, EventArgs e)
        {
            if (pagerList.Count == 0)
            {
                Hide();
            }
            else
            {
                SetData();
            }
        }


        public void AddData(string tableName,
            string datetime,
            string desc)
        {
            string tmp_desc = String.Empty;
            try
            {
                var json = JsonNode.Parse(desc);
                if (json != null)
                {
                    foreach (var node in json.AsArray())
                    {
                        tmp_desc += node!.ToString() + "\n";
                    }
                }
            }
            catch (Exception exception)
            {
                tmp_desc = desc;
                Console.WriteLine(exception.ToString());
            }

            PagerData pager = new();
            pager.tableName = tableName;
            pager.datetime = datetime;
            pager.desc = tmp_desc;
            pagerList.Add(pager);

            lbTotalOrder.Text = pagerList.Count.ToString();
        }

        public void SetData()
        {
            if (pagerList.Count == 0) return;

            PagerData pager = pagerList[0];

            lbTableName.Text = String.Empty;
            lbDateTime.Text = String.Empty;

            this.tableName = pager.tableName;
            this.datetime = pager.datetime;
            this.lbMenu.Text = pager.desc;

            pagerList.RemoveAt(0);
            lbTotalOrder.Text = pagerList.Count.ToString();
            ShowMenu();
        }

        public void ShowMenu()
        {
            try
            {
                lbTableName.Text = this.tableName;
                lbDateTime.Text = this.datetime;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
            }
        }

        private void picTitlebar_Click(object sender, EventArgs e)
        {
            if (bFold == false)
            {
                this.Height = 50;
                bFold = true;
            }
            else
            {
                this.Height = 500;
                bFold = false;
            }
        }
    }

    public class PagerData
    {
        public string tableName = String.Empty;
        public string datetime = String.Empty;
        public string desc = String.Empty;
    }
}
