using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace agentcs
{
    public partial class FormClear : Form
    {
        public FormClear()
        {
            InitializeComponent();
            this.Paint += FormClear_Paint;
        }

        private void FormClear_Paint(object? sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        public void setTableName(string tableName)
        {
            lbTableName.Text = tableName;
        }

        private void FormClear_Load(object sender, EventArgs e)
        {

        }

        private void picClear_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void picCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
