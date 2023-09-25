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
            picGenPin = new PictureBox();
            picTableStatus = new PictureBox();
            picMenu = new PictureBox();
            picShowOrder = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picGenPin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTableStatus).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picMenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picShowOrder).BeginInit();
            SuspendLayout();
            // 
            // picLogo
            // 
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(49, 15);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(100, 20);
            picLogo.TabIndex = 3;
            picLogo.TabStop = false;
            picLogo.Click += picLogo_Click;
            picLogo.DoubleClick += picLogo_DoubleClick;
            // 
            // picGenPin
            // 
            picGenPin.Image = (Image)resources.GetObject("picGenPin.Image");
            picGenPin.Location = new Point(306, 7);
            picGenPin.Name = "picGenPin";
            picGenPin.Size = new Size(120, 30);
            picGenPin.TabIndex = 32;
            picGenPin.TabStop = false;
            picGenPin.Click += picGenPin_Click;
            // 
            // picTableStatus
            // 
            picTableStatus.Image = (Image)resources.GetObject("picTableStatus.Image");
            picTableStatus.Location = new Point(436, 7);
            picTableStatus.Name = "picTableStatus";
            picTableStatus.Size = new Size(120, 30);
            picTableStatus.TabIndex = 34;
            picTableStatus.TabStop = false;
            picTableStatus.Click += picTableStatus_Click;
            // 
            // picMenu
            // 
            picMenu.Image = (Image)resources.GetObject("picMenu.Image");
            picMenu.Location = new Point(15, 17);
            picMenu.Name = "picMenu";
            picMenu.Size = new Size(20, 11);
            picMenu.TabIndex = 35;
            picMenu.TabStop = false;
            // 
            // picShowOrder
            // 
            picShowOrder.Image = (Image)resources.GetObject("picShowOrder.Image");
            picShowOrder.Location = new Point(566, 7);
            picShowOrder.Name = "picShowOrder";
            picShowOrder.Size = new Size(120, 30);
            picShowOrder.TabIndex = 36;
            picShowOrder.TabStop = false;
            picShowOrder.Click += picShowOrder_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(701, 44);
            ControlBox = false;
            Controls.Add(picShowOrder);
            Controls.Add(picMenu);
            Controls.Add(picTableStatus);
            Controls.Add(picGenPin);
            Controls.Add(picLogo);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CORDER";
            TopMost = true;
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picGenPin).EndInit();
            ((System.ComponentModel.ISupportInitialize)picTableStatus).EndInit();
            ((System.ComponentModel.ISupportInitialize)picMenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)picShowOrder).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox picLogo;
        private PictureBox picGenPin;
        private PictureBox picTableStatus;
        private PictureBox picMenu;
        private PictureBox picShowOrder;
    }
}