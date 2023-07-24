namespace agentcs
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            picLogo = new PictureBox();
            picCorderText = new PictureBox();
            picDecoBox = new PictureBox();
            textTable = new TextBox();
            buttonCancel = new Button();
            picClose = new PictureBox();
            picGenPin = new PictureBox();
            picSetting = new PictureBox();
            picTableStatus = new PictureBox();
            picWebpage = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCorderText).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picDecoBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picGenPin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSetting).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTableStatus).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picWebpage).BeginInit();
            SuspendLayout();
            // 
            // picLogo
            // 
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(150, 32);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(100, 20);
            picLogo.TabIndex = 3;
            picLogo.TabStop = false;
            // 
            // picCorderText
            // 
            picCorderText.Image = (Image)resources.GetObject("picCorderText.Image");
            picCorderText.Location = new Point(98, 60);
            picCorderText.Name = "picCorderText";
            picCorderText.Size = new Size(207, 17);
            picCorderText.TabIndex = 4;
            picCorderText.TabStop = false;
            // 
            // picDecoBox
            // 
            picDecoBox.BackColor = Color.Transparent;
            picDecoBox.Image = (Image)resources.GetObject("picDecoBox.Image");
            picDecoBox.Location = new Point(377, 162);
            picDecoBox.Name = "picDecoBox";
            picDecoBox.Size = new Size(30, 60);
            picDecoBox.TabIndex = 27;
            picDecoBox.TabStop = false;
            // 
            // textTable
            // 
            textTable.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            textTable.Location = new Point(515, 162);
            textTable.Name = "textTable";
            textTable.Size = new Size(100, 29);
            textTable.TabIndex = 30;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(621, 156);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(100, 50);
            buttonCancel.TabIndex = 29;
            buttonCancel.Text = "취소 처리";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // picClose
            // 
            picClose.Image = (Image)resources.GetObject("picClose.Image");
            picClose.Location = new Point(340, 20);
            picClose.Name = "picClose";
            picClose.Size = new Size(40, 40);
            picClose.TabIndex = 31;
            picClose.TabStop = false;
            picClose.Click += picClose_Click;
            // 
            // picGenPin
            // 
            picGenPin.Image = (Image)resources.GetObject("picGenPin.Image");
            picGenPin.Location = new Point(58, 108);
            picGenPin.Name = "picGenPin";
            picGenPin.Size = new Size(120, 120);
            picGenPin.TabIndex = 32;
            picGenPin.TabStop = false;
            picGenPin.Click += picGenPin_Click;
            // 
            // picSetting
            // 
            picSetting.Image = (Image)resources.GetObject("picSetting.Image");
            picSetting.Location = new Point(58, 253);
            picSetting.Name = "picSetting";
            picSetting.Size = new Size(120, 120);
            picSetting.TabIndex = 33;
            picSetting.TabStop = false;
            picSetting.Click += picSetting_Click;
            // 
            // picTableStatus
            // 
            picTableStatus.Image = (Image)resources.GetObject("picTableStatus.Image");
            picTableStatus.Location = new Point(202, 108);
            picTableStatus.Name = "picTableStatus";
            picTableStatus.Size = new Size(120, 120);
            picTableStatus.TabIndex = 34;
            picTableStatus.TabStop = false;
            picTableStatus.Click += picTableStatus_Click;
            // 
            // picWebpage
            // 
            picWebpage.Image = (Image)resources.GetObject("picWebpage.Image");
            picWebpage.Location = new Point(202, 253);
            picWebpage.Name = "picWebpage";
            picWebpage.Size = new Size(120, 120);
            picWebpage.TabIndex = 35;
            picWebpage.TabStop = false;
            picWebpage.Click += picWebpage_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(235, 235, 235);
            ClientSize = new Size(800, 400);
            ControlBox = false;
            Controls.Add(picWebpage);
            Controls.Add(picTableStatus);
            Controls.Add(picSetting);
            Controls.Add(picGenPin);
            Controls.Add(picClose);
            Controls.Add(textTable);
            Controls.Add(buttonCancel);
            Controls.Add(picDecoBox);
            Controls.Add(picCorderText);
            Controls.Add(picLogo);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCorderText).EndInit();
            ((System.ComponentModel.ISupportInitialize)picDecoBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)picClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)picGenPin).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSetting).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTableStatus).EndInit();
            ((System.ComponentModel.ISupportInitialize)picWebpage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox picLogo;
        private PictureBox picCorderText;
        private PictureBox picDecoBox;
        private TextBox textTable;
        private Button buttonCancel;
        private PictureBox picClose;
        private PictureBox picGenPin;
        private PictureBox picSetting;
        private PictureBox picTableStatus;
        private PictureBox picWebpage;
    }
}