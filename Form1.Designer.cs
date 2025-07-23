namespace WinFormsApp4
{
    partial class Form1
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
            labelSure = new Label();
            comboBoxSure = new ComboBox();
            btnConvert = new Button();
            lblStatus = new Label();
            labelOutput = new Label();
            comboBoxOutput = new ComboBox();
            SuspendLayout();
            // 
            // labelSure
            // 
            labelSure.AutoSize = true;
            labelSure.Location = new Point(254, 58);
            labelSure.Name = "labelSure";
            labelSure.Size = new Size(262, 25);
            labelSure.TabIndex = 0;
            labelSure.Text = "Hangi kısmı dinlemek istersiniz?";
            // 
            // comboBoxSure
            // 
            comboBoxSure.FormattingEnabled = true;
            comboBoxSure.Location = new Point(254, 108);
            comboBoxSure.Name = "comboBoxSure";
            comboBoxSure.Size = new Size(182, 33);
            comboBoxSure.TabIndex = 1;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(480, 108);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(112, 34);
            btnConvert.TabIndex = 2;
            btnConvert.Text = "Dönüştür";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(371, 204);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 25);
            lblStatus.TabIndex = 3;
            // 
            // labelOutput
            // 
            labelOutput.AutoSize = true;
            labelOutput.Location = new Point(220, 268);
            labelOutput.Name = "labelOutput";
            labelOutput.Size = new Size(176, 25);
            labelOutput.TabIndex = 4;
            labelOutput.Text = "Dönüştürülen dosya:";
            // 
            // comboBoxOutput
            // 
            comboBoxOutput.FormattingEnabled = true;
            comboBoxOutput.Location = new Point(445, 265);
            comboBoxOutput.Name = "comboBoxOutput";
            comboBoxOutput.Size = new Size(182, 33);
            comboBoxOutput.TabIndex = 5;
            comboBoxOutput.SelectedIndexChanged += comboBoxOutput_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBoxOutput);
            Controls.Add(labelOutput);
            Controls.Add(lblStatus);
            Controls.Add(btnConvert);
            Controls.Add(comboBoxSure);
            Controls.Add(labelSure);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelSure;
        private ComboBox comboBoxSure;
        private Button btnConvert;
        private Label lblStatus;
        private Label labelOutput;
        private ComboBox comboBoxOutput;
    }
}
