namespace agentcs
{
    partial class FormTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTable));
            labelResetDesc = new Label();
            picButtonPrev = new PictureBox();
            picButtonNext = new PictureBox();
            picButtonClose = new PictureBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            lbPage = new Label();
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // labelResetDesc
            // 
            labelResetDesc.AutoSize = true;
            labelResetDesc.BackColor = Color.Transparent;
            labelResetDesc.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
            labelResetDesc.ForeColor = Color.Black;
            labelResetDesc.ImageAlign = ContentAlignment.TopLeft;
            labelResetDesc.Location = new Point(25, 58);
            labelResetDesc.Name = "labelResetDesc";
            labelResetDesc.Size = new Size(338, 15);
            labelResetDesc.TabIndex = 4;
            labelResetDesc.Text = "*손님이 주문 전 테이블 이동한 경우, 반드시 초기화 해주세요.";
            // 
            // picButtonPrev
            // 
            picButtonPrev.BackColor = Color.Transparent;
            picButtonPrev.Image = (Image)resources.GetObject("picButtonPrev.Image");
            picButtonPrev.Location = new Point(20, 420);
            picButtonPrev.Name = "picButtonPrev";
            picButtonPrev.Size = new Size(70, 24);
            picButtonPrev.TabIndex = 5;
            picButtonPrev.TabStop = false;
            picButtonPrev.Click += picButtonPrev_Click;
            // 
            // picButtonNext
            // 
            picButtonNext.BackColor = Color.Transparent;
            picButtonNext.Image = (Image)resources.GetObject("picButtonNext.Image");
            picButtonNext.Location = new Point(310, 420);
            picButtonNext.Name = "picButtonNext";
            picButtonNext.Size = new Size(70, 24);
            picButtonNext.TabIndex = 6;
            picButtonNext.TabStop = false;
            picButtonNext.Click += picButtonNext_Click;
            // 
            // picButtonClose
            // 
            picButtonClose.BackColor = Color.Transparent;
            picButtonClose.Image = (Image)resources.GetObject("picButtonClose.Image");
            picButtonClose.Location = new Point(0, 470);
            picButtonClose.Name = "picButtonClose";
            picButtonClose.Size = new Size(400, 30);
            picButtonClose.TabIndex = 8;
            picButtonClose.TabStop = false;
            picButtonClose.Click += picButtonClose_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(286, 29);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(83, 20);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label1.ForeColor = Color.FromArgb(255, 108, 0);
            label1.Location = new Point(24, 30);
            label1.Name = "label1";
            label1.Size = new Size(51, 19);
            label1.TabIndex = 10;
            label1.Text = "테이블";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            label2.Location = new Point(70, 30);
            label2.Name = "label2";
            label2.Size = new Size(51, 19);
            label2.TabIndex = 11;
            label2.Text = "초기화";
            // 
            // lbPage
            // 
            lbPage.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbPage.Location = new Point(166, 423);
            lbPage.Name = "lbPage";
            lbPage.Size = new Size(70, 20);
            lbPage.TabIndex = 12;
            lbPage.Text = "PAGE";
            lbPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormTable
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(251, 244, 235);
            ClientSize = new Size(400, 500);
            ControlBox = false;
            Controls.Add(lbPage);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(picButtonClose);
            Controls.Add(picButtonNext);
            Controls.Add(picButtonPrev);
            Controls.Add(labelResetDesc);
            Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormTable";
            StartPosition = FormStartPosition.Manual;
            Text = "FormTable";
            TopMost = true;
            Load += FormTable_Load;
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label labelResetDesc;
        private PictureBox picButtonPrev;
        private PictureBox picButtonNext;
        private PictureBox picButtonClose;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label lbPage;
    }
}