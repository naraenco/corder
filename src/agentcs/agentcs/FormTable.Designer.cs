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
            picButtonReset = new PictureBox();
            picButtonKeep = new PictureBox();
            labelTableName = new Label();
            labelResetConfirm = new Label();
            labelResetDesc = new Label();
            picButtonPrev = new PictureBox();
            picButtonNext = new PictureBox();
            labelSelectTable = new Label();
            picButtonClose = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picButtonReset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonKeep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picButtonClose).BeginInit();
            SuspendLayout();
            // 
            // picButtonReset
            // 
            picButtonReset.Image = (Image)resources.GetObject("picButtonReset.Image");
            picButtonReset.Location = new Point(125, 157);
            picButtonReset.Name = "picButtonReset";
            picButtonReset.Size = new Size(170, 38);
            picButtonReset.TabIndex = 0;
            picButtonReset.TabStop = false;
            picButtonReset.Click += picButtonReset_Click;
            // 
            // picButtonKeep
            // 
            picButtonKeep.Image = (Image)resources.GetObject("picButtonKeep.Image");
            picButtonKeep.Location = new Point(305, 157);
            picButtonKeep.Name = "picButtonKeep";
            picButtonKeep.Size = new Size(170, 38);
            picButtonKeep.TabIndex = 1;
            picButtonKeep.TabStop = false;
            picButtonKeep.Click += picButtonKeep_Click;
            // 
            // labelTableName
            // 
            labelTableName.AutoSize = true;
            labelTableName.BackColor = Color.Transparent;
            labelTableName.Font = new Font("나눔스퀘어 네오 Bold", 27.75F, FontStyle.Bold, GraphicsUnit.Point);
            labelTableName.ForeColor = Color.White;
            labelTableName.Location = new Point(206, 34);
            labelTableName.Name = "labelTableName";
            labelTableName.Size = new Size(193, 40);
            labelTableName.TabIndex = 2;
            labelTableName.Text = "0번 테이블";
            // 
            // labelResetConfirm
            // 
            labelResetConfirm.AutoSize = true;
            labelResetConfirm.BackColor = Color.Transparent;
            labelResetConfirm.Font = new Font("나눔스퀘어 네오 Bold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelResetConfirm.ForeColor = Color.White;
            labelResetConfirm.Location = new Point(142, 121);
            labelResetConfirm.Name = "labelResetConfirm";
            labelResetConfirm.Size = new Size(322, 18);
            labelResetConfirm.TabIndex = 3;
            labelResetConfirm.Text = "위 테이블의 주문 내역을 초기화 하시겠습니까?";
            // 
            // labelResetDesc
            // 
            labelResetDesc.AutoSize = true;
            labelResetDesc.BackColor = Color.Transparent;
            labelResetDesc.Font = new Font("나눔스퀘어 네오 Bold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelResetDesc.ForeColor = Color.White;
            labelResetDesc.Location = new Point(66, 237);
            labelResetDesc.Name = "labelResetDesc";
            labelResetDesc.Size = new Size(467, 26);
            labelResetDesc.TabIndex = 4;
            labelResetDesc.Text = "POS 주문은 삭제되지 않으며, 손님이 보고 있는 모바일 메뉴판의 테이블 번호만 초기화됩니다.\r\n손님이 테이블을 이동한 경우, 잊지 말고 꼭! 초기화 해주세요.";
            labelResetDesc.TextAlign = ContentAlignment.TopCenter;
            // 
            // picButtonPrev
            // 
            picButtonPrev.BackColor = Color.Transparent;
            picButtonPrev.Image = (Image)resources.GetObject("picButtonPrev.Image");
            picButtonPrev.Location = new Point(12, 225);
            picButtonPrev.Name = "picButtonPrev";
            picButtonPrev.Size = new Size(170, 38);
            picButtonPrev.TabIndex = 5;
            picButtonPrev.TabStop = false;
            picButtonPrev.Click += picButtonPrev_Click;
            // 
            // picButtonNext
            // 
            picButtonNext.BackColor = Color.Transparent;
            picButtonNext.Image = (Image)resources.GetObject("picButtonNext.Image");
            picButtonNext.Location = new Point(418, 225);
            picButtonNext.Name = "picButtonNext";
            picButtonNext.Size = new Size(170, 38);
            picButtonNext.TabIndex = 6;
            picButtonNext.TabStop = false;
            picButtonNext.Click += picButtonNext_Click;
            // 
            // labelSelectTable
            // 
            labelSelectTable.AutoSize = true;
            labelSelectTable.BackColor = Color.Transparent;
            labelSelectTable.Font = new Font("나눔스퀘어 네오 Bold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelSelectTable.ForeColor = Color.White;
            labelSelectTable.Location = new Point(46, 87);
            labelSelectTable.Name = "labelSelectTable";
            labelSelectTable.Size = new Size(162, 18);
            labelSelectTable.TabIndex = 7;
            labelSelectTable.Text = "테이블을 선택해주세요";
            // 
            // picButtonClose
            // 
            picButtonClose.BackColor = Color.Transparent;
            picButtonClose.Image = (Image)resources.GetObject("picButtonClose.Image");
            picButtonClose.Location = new Point(493, 12);
            picButtonClose.Name = "picButtonClose";
            picButtonClose.Size = new Size(40, 40);
            picButtonClose.TabIndex = 8;
            picButtonClose.TabStop = false;
            picButtonClose.Click += picButtonClose_Click;
            // 
            // FormTable
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(65, 65, 65);
            ClientSize = new Size(600, 300);
            ControlBox = false;
            Controls.Add(picButtonClose);
            Controls.Add(labelSelectTable);
            Controls.Add(picButtonNext);
            Controls.Add(picButtonPrev);
            Controls.Add(labelResetDesc);
            Controls.Add(labelResetConfirm);
            Controls.Add(labelTableName);
            Controls.Add(picButtonKeep);
            Controls.Add(picButtonReset);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormTable";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormTable";
            ((System.ComponentModel.ISupportInitialize)picButtonReset).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonKeep).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonPrev).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonNext).EndInit();
            ((System.ComponentModel.ISupportInitialize)picButtonClose).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picButtonReset;
        private PictureBox picButtonKeep;
        private Label labelTableName;
        private Label labelResetConfirm;
        private Label labelResetDesc;
        private PictureBox picButtonPrev;
        private PictureBox picButtonNext;
        private Label labelSelectTable;
        private PictureBox picButtonClose;
    }
}