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
            buttonGenPin = new Button();
            SuspendLayout();
            // 
            // buttonGenPin
            // 
            buttonGenPin.Location = new Point(82, 66);
            buttonGenPin.Name = "buttonGenPin";
            buttonGenPin.Size = new Size(100, 50);
            buttonGenPin.TabIndex = 0;
            buttonGenPin.Text = "PIN 발급";
            buttonGenPin.UseVisualStyleBackColor = true;
            buttonGenPin.Click += buttonGenPin_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(buttonGenPin);
            Name = "MainForm";
            Text = "C.Order";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button buttonGenPin;
    }
}