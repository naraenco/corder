namespace agentcs
{
    partial class FormClear
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClear));
            picClear = new PictureBox();
            picCancel = new PictureBox();
            pictureBox1 = new PictureBox();
            lbTableName = new Label();
            ((System.ComponentModel.ISupportInitialize)picClear).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // picClear
            // 
            picClear.Image = (Image)resources.GetObject("picClear.Image");
            picClear.Location = new Point(1, 170);
            picClear.Name = "picClear";
            picClear.Size = new Size(149, 29);
            picClear.TabIndex = 0;
            picClear.TabStop = false;
            picClear.Click += picClear_Click;
            // 
            // picCancel
            // 
            picCancel.Image = (Image)resources.GetObject("picCancel.Image");
            picCancel.Location = new Point(150, 170);
            picCancel.Name = "picCancel";
            picCancel.Size = new Size(149, 29);
            picCancel.TabIndex = 1;
            picCancel.TabStop = false;
            picCancel.Click += picCancel_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(1, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(298, 29);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // lbTableName
            // 
            lbTableName.Font = new Font("맑은 고딕", 20F, FontStyle.Bold, GraphicsUnit.Pixel);
            lbTableName.Location = new Point(1, 30);
            lbTableName.Name = "lbTableName";
            lbTableName.Size = new Size(298, 140);
            lbTableName.TabIndex = 3;
            lbTableName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormClear
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(300, 200);
            ControlBox = false;
            Controls.Add(lbTableName);
            Controls.Add(pictureBox1);
            Controls.Add(picCancel);
            Controls.Add(picClear);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormClear";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FormClear";
            TopMost = true;
            Load += FormClear_Load;
            ((System.ComponentModel.ISupportInitialize)picClear).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCancel).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox picClear;
        private PictureBox picCancel;
        private PictureBox pictureBox1;
        private Label lbTableName;
    }
}