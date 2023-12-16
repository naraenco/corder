namespace COrderUpdater
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
            label1 = new Label();
            label2 = new Label();
            lbFileVersion = new Label();
            lbCurrentVersion = new Label();
            lbMessage = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 26);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 0;
            label1.Text = "파일 버전";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(42, 54);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 1;
            label2.Text = "현재 버전";
            // 
            // lbFileVersion
            // 
            lbFileVersion.AutoSize = true;
            lbFileVersion.Location = new Point(123, 26);
            lbFileVersion.Name = "lbFileVersion";
            lbFileVersion.Size = new Size(44, 15);
            lbFileVersion.TabIndex = 2;
            lbFileVersion.Text = "0.0.0.0";
            // 
            // lbCurrentVersion
            // 
            lbCurrentVersion.AutoSize = true;
            lbCurrentVersion.Location = new Point(123, 54);
            lbCurrentVersion.Name = "lbCurrentVersion";
            lbCurrentVersion.Size = new Size(44, 15);
            lbCurrentVersion.TabIndex = 3;
            lbCurrentVersion.Text = "0.0.0.0";
            // 
            // lbMessage
            // 
            lbMessage.AutoSize = true;
            lbMessage.Location = new Point(42, 96);
            lbMessage.Name = "lbMessage";
            lbMessage.Size = new Size(58, 15);
            lbMessage.TabIndex = 4;
            lbMessage.Text = "UPDATER";
            // 
            // button1
            // 
            button1.Location = new Point(148, 129);
            button1.Name = "button1";
            button1.Size = new Size(90, 30);
            button1.TabIndex = 5;
            button1.Text = "닫기";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 181);
            Controls.Add(button1);
            Controls.Add(lbMessage);
            Controls.Add(lbCurrentVersion);
            Controls.Add(lbFileVersion);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Agent Updater";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label lbFileVersion;
        private Label lbCurrentVersion;
        private Label lbMessage;
        private Button button1;
    }
}
