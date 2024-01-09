namespace agentcs
{
    partial class FormPager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPager));
            picConfirm = new PictureBox();
            lbDateTitle = new Label();
            lbDateTime = new Label();
            label3 = new Label();
            label4 = new Label();
            label6 = new Label();
            lbMenu = new Label();
            picTitlebar = new PictureBox();
            lbTotalOrder = new Label();
            lbTableName = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)picConfirm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTitlebar).BeginInit();
            SuspendLayout();
            // 
            // picConfirm
            // 
            picConfirm.Image = (Image)resources.GetObject("picConfirm.Image");
            picConfirm.Location = new Point(0, 470);
            picConfirm.Name = "picConfirm";
            picConfirm.Size = new Size(400, 30);
            picConfirm.TabIndex = 0;
            picConfirm.TabStop = false;
            picConfirm.Click += picConfirm_Click;
            // 
            // lbDateTitle
            // 
            lbDateTitle.AutoSize = true;
            lbDateTitle.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbDateTitle.Location = new Point(24, 75);
            lbDateTitle.Name = "lbDateTitle";
            lbDateTitle.Size = new Size(90, 19);
            lbDateTitle.TabIndex = 14;
            lbDateTitle.Text = "[ 발행일시 ] ";
            // 
            // lbDateTime
            // 
            lbDateTime.AutoSize = true;
            lbDateTime.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbDateTime.Location = new Point(110, 75);
            lbDateTime.Name = "lbDateTime";
            lbDateTime.Size = new Size(73, 19);
            lbDateTime.TabIndex = 16;
            lbDateTime.Text = "DateTime";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label3.Location = new Point(23, 107);
            label3.Name = "label3";
            label3.Size = new Size(359, 19);
            label3.TabIndex = 18;
            label3.Text = "===================================";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label4.Location = new Point(21, 145);
            label4.Name = "label4";
            label4.Size = new Size(357, 19);
            label4.TabIndex = 19;
            label4.Text = "----------------------------------------------------------";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label6.Location = new Point(35, 126);
            label6.Name = "label6";
            label6.Size = new Size(80, 19);
            label6.TabIndex = 24;
            label6.Text = "요 청 사 항";
            // 
            // lbMenu
            // 
            lbMenu.AutoSize = true;
            lbMenu.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbMenu.Location = new Point(35, 165);
            lbMenu.Name = "lbMenu";
            lbMenu.Size = new Size(37, 19);
            lbMenu.TabIndex = 26;
            lbMenu.Text = "메뉴";
            // 
            // picTitlebar
            // 
            picTitlebar.Image = (Image)resources.GetObject("picTitlebar.Image");
            picTitlebar.Location = new Point(0, 0);
            picTitlebar.Name = "picTitlebar";
            picTitlebar.Size = new Size(400, 50);
            picTitlebar.TabIndex = 30;
            picTitlebar.TabStop = false;
            picTitlebar.Click += picTitlebar_Click;
            // 
            // lbTotalOrder
            // 
            lbTotalOrder.Font = new Font("맑은 고딕", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbTotalOrder.ForeColor = Color.Black;
            lbTotalOrder.Location = new Point(328, 28);
            lbTotalOrder.Name = "lbTotalOrder";
            lbTotalOrder.Size = new Size(50, 17);
            lbTotalOrder.TabIndex = 31;
            lbTotalOrder.Text = "0";
            lbTotalOrder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbTableName
            // 
            lbTableName.BackColor = Color.Transparent;
            lbTableName.Font = new Font("맑은 고딕", 20F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbTableName.ForeColor = Color.White;
            lbTableName.Location = new Point(110, 9);
            lbTableName.Name = "lbTableName";
            lbTableName.Size = new Size(180, 30);
            lbTableName.TabIndex = 32;
            lbTableName.Text = "테이블";
            // 
            // label1
            // 
            label1.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Pixel);
            label1.Location = new Point(327, 11);
            label1.Name = "label1";
            label1.Size = new Size(59, 12);
            label1.TabIndex = 33;
            label1.Text = "남은 호츌";
            // 
            // FormPager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(251, 244, 235);
            ClientSize = new Size(400, 500);
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(lbTableName);
            Controls.Add(lbTotalOrder);
            Controls.Add(picTitlebar);
            Controls.Add(lbMenu);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(lbDateTime);
            Controls.Add(lbDateTitle);
            Controls.Add(picConfirm);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPager";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FormPager";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            Load += FormPager_Load;
            ((System.ComponentModel.ISupportInitialize)picConfirm).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTitlebar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picConfirm;
        private Label lbDateTitle;
        private Label lbDateTime;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label lbMenu;
        private PictureBox picTitlebar;
        private Label lbTotalOrder;
        private Label lbTableName;
        private Label label1;
    }
}