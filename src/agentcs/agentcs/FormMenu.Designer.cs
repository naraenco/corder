namespace agentcs
{
    partial class FormMenu
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
            menuConfig = new Label();
            menuManager = new Label();
            menuExit = new Label();
            SuspendLayout();
            // 
            // menuConfig
            // 
            menuConfig.AutoSize = true;
            menuConfig.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            menuConfig.Location = new Point(12, 14);
            menuConfig.Name = "menuConfig";
            menuConfig.Size = new Size(104, 20);
            menuConfig.TabIndex = 0;
            menuConfig.Text = "프로그램 설정";
            menuConfig.Click += menuConfig_Click;
            // 
            // menuManager
            // 
            menuManager.AutoSize = true;
            menuManager.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            menuManager.Location = new Point(12, 46);
            menuManager.Name = "menuManager";
            menuManager.Size = new Size(89, 20);
            menuManager.TabIndex = 1;
            menuManager.Text = "매장 관리자";
            menuManager.Click += menuManager_Click;
            // 
            // menuExit
            // 
            menuExit.AutoSize = true;
            menuExit.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            menuExit.Location = new Point(12, 77);
            menuExit.Name = "menuExit";
            menuExit.Size = new Size(39, 20);
            menuExit.TabIndex = 3;
            menuExit.Text = "종료";
            menuExit.Click += menuExit_Click;
            // 
            // FormMenu
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(251, 244, 235);
            ClientSize = new Size(200, 110);
            ControlBox = false;
            Controls.Add(menuExit);
            Controls.Add(menuManager);
            Controls.Add(menuConfig);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormMenu";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FormMenu";
            TopMost = true;
            Deactivate += FormMenu_Deactivate;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label menuConfig;
        private Label menuManager;
        private Label menuExit;
    }
}