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
            buttonCancel = new Button();
            textTable = new TextBox();
            SuspendLayout();
            // 
            // buttonGenPin
            // 
            buttonGenPin.Location = new Point(188, 73);
            buttonGenPin.Name = "buttonGenPin";
            buttonGenPin.Size = new Size(100, 50);
            buttonGenPin.TabIndex = 0;
            buttonGenPin.Text = "PIN 발급";
            buttonGenPin.UseVisualStyleBackColor = true;
            buttonGenPin.Click += buttonGenPin_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(188, 165);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(100, 50);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "취소 처리";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // textTable
            // 
            textTable.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            textTable.Location = new Point(71, 175);
            textTable.Name = "textTable";
            textTable.Size = new Size(100, 29);
            textTable.TabIndex = 2;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(textTable);
            Controls.Add(buttonCancel);
            Controls.Add(buttonGenPin);
            Name = "MainForm";
            Text = "C.Order";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGenPin;
        private Button buttonCancel;
        private TextBox textTable;
    }
}