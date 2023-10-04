namespace agentcs
{
    partial class FormOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrder));
            picConfirm = new PictureBox();
            picLogo = new PictureBox();
            label2 = new Label();
            lbTableName = new Label();
            lbDateTitle = new Label();
            lbDateTime = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            lbPage = new Label();
            picButtonNext = new PictureBox();
            picButtonPrev = new PictureBox();
            label6 = new Label();
            label7 = new Label();
            lbMenu = new Label();
            lbQty = new Label();
            label8 = new Label();
            lbTotalOrder = new Label();
            ((System.ComponentModel.ISupportInitialize)picConfirm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).BeginInit();
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
            // picLogo
            // 
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(295, 29);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(83, 20);
            picLogo.TabIndex = 1;
            picLogo.TabStop = false;
            picLogo.Click += picLogo_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label2.Location = new Point(24, 30);
            label2.Name = "label2";
            label2.Size = new Size(98, 19);
            label2.TabIndex = 13;
            label2.Text = "모바일 주문서";
            // 
            // lbTableName
            // 
            lbTableName.AutoSize = true;
            lbTableName.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbTableName.ForeColor = Color.FromArgb(255, 108, 0);
            lbTableName.Location = new Point(110, 65);
            lbTableName.Name = "lbTableName";
            lbTableName.Size = new Size(78, 19);
            lbTableName.TabIndex = 12;
            lbTableName.Text = "0번 테이블";
            // 
            // lbDateTitle
            // 
            lbDateTitle.AutoSize = true;
            lbDateTitle.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbDateTitle.Location = new Point(24, 90);
            lbDateTitle.Name = "lbDateTitle";
            lbDateTitle.Size = new Size(90, 19);
            lbDateTitle.TabIndex = 14;
            lbDateTitle.Text = "[ 발행일시 ] ";
            // 
            // lbDateTime
            // 
            lbDateTime.AutoSize = true;
            lbDateTime.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbDateTime.Location = new Point(110, 90);
            lbDateTime.Name = "lbDateTime";
            lbDateTime.Size = new Size(73, 19);
            lbDateTime.TabIndex = 16;
            lbDateTime.Text = "DateTime";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label3.Location = new Point(23, 122);
            label3.Name = "label3";
            label3.Size = new Size(359, 19);
            label3.TabIndex = 18;
            label3.Text = "===================================";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label4.Location = new Point(21, 160);
            label4.Name = "label4";
            label4.Size = new Size(357, 19);
            label4.TabIndex = 19;
            label4.Text = "----------------------------------------------------------";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label5.Location = new Point(21, 400);
            label5.Name = "label5";
            label5.Size = new Size(357, 19);
            label5.TabIndex = 20;
            label5.Text = "----------------------------------------------------------";
            // 
            // lbPage
            // 
            lbPage.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbPage.Location = new Point(165, 430);
            lbPage.Name = "lbPage";
            lbPage.Size = new Size(70, 20);
            lbPage.TabIndex = 21;
            lbPage.Text = "PAGE";
            lbPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picButtonNext
            // 
            picButtonNext.BackColor = Color.Transparent;
            picButtonNext.Image = (Image)resources.GetObject("picButtonNext.Image");
            picButtonNext.Location = new Point(310, 430);
            picButtonNext.Name = "picButtonNext";
            picButtonNext.Size = new Size(70, 24);
            picButtonNext.TabIndex = 23;
            picButtonNext.TabStop = false;
            picButtonNext.Click += picButtonNext_Click;
            // 
            // picButtonPrev
            // 
            picButtonPrev.BackColor = Color.Transparent;
            picButtonPrev.Image = (Image)resources.GetObject("picButtonPrev.Image");
            picButtonPrev.Location = new Point(20, 430);
            picButtonPrev.Name = "picButtonPrev";
            picButtonPrev.Size = new Size(70, 24);
            picButtonPrev.TabIndex = 22;
            picButtonPrev.TabStop = false;
            picButtonPrev.Click += picButtonPrev_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label6.Location = new Point(35, 141);
            label6.Name = "label6";
            label6.Size = new Size(71, 19);
            label6.TabIndex = 24;
            label6.Text = "메  뉴  명";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label7.Location = new Point(317, 141);
            label7.Name = "label7";
            label7.Size = new Size(47, 19);
            label7.TabIndex = 25;
            label7.Text = "수  량";
            // 
            // lbMenu
            // 
            lbMenu.AutoSize = true;
            lbMenu.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbMenu.Location = new Point(35, 180);
            lbMenu.Name = "lbMenu";
            lbMenu.Size = new Size(37, 19);
            lbMenu.TabIndex = 26;
            lbMenu.Text = "메뉴";
            // 
            // lbQty
            // 
            lbQty.AutoSize = true;
            lbQty.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbQty.Location = new Point(331, 180);
            lbQty.Name = "lbQty";
            lbQty.Size = new Size(17, 19);
            lbQty.TabIndex = 27;
            lbQty.Text = "0";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label8.Location = new Point(24, 65);
            label8.Name = "label8";
            label8.Size = new Size(90, 19);
            label8.TabIndex = 28;
            label8.Text = "[ 주문위치 ] ";
            // 
            // lbTotalOrder
            // 
            lbTotalOrder.AutoSize = true;
            lbTotalOrder.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lbTotalOrder.ForeColor = SystemColors.MenuHighlight;
            lbTotalOrder.Location = new Point(128, 31);
            lbTotalOrder.Name = "lbTotalOrder";
            lbTotalOrder.Size = new Size(57, 17);
            lbTotalOrder.TabIndex = 29;
            lbTotalOrder.Text = "(주문수)";
            // 
            // FormOrder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(251, 244, 235);
            ClientSize = new Size(400, 500);
            ControlBox = false;
            Controls.Add(lbTotalOrder);
            Controls.Add(label8);
            Controls.Add(lbQty);
            Controls.Add(lbMenu);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(picButtonNext);
            Controls.Add(picButtonPrev);
            Controls.Add(lbPage);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(lbDateTime);
            Controls.Add(lbDateTitle);
            Controls.Add(label2);
            Controls.Add(lbTableName);
            Controls.Add(picLogo);
            Controls.Add(picConfirm);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormOrder";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FormOrder";
            TopMost = true;
            Load += FormOrder_Load;
            ((System.ComponentModel.ISupportInitialize)picConfirm).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picConfirm;
        private PictureBox picLogo;
        private Label label2;
        private Label lbTableName;
        private Label lbDateTitle;
        private Label lbDateTime;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label lbPage;
        private PictureBox picButtonNext;
        private PictureBox picButtonPrev;
        private Label label6;
        private Label label7;
        private Label lbMenu;
        private Label lbQty;
        private Label label8;
        private Label lbTotalOrder;
    }
}