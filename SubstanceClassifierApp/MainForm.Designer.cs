
namespace SubstanceClassifierApp
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
            this.inputPanel = new System.Windows.Forms.Panel();
            this.inputGroup = new System.Windows.Forms.GroupBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.viewDbButton = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressText = new System.Windows.Forms.Label();
            this.classifyButton = new System.Windows.Forms.Button();
            this.updateDbButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.errorsTextBox = new System.Windows.Forms.RichTextBox();
            this.warningTextBox = new System.Windows.Forms.RichTextBox();
            this.classificationTextBox = new System.Windows.Forms.RichTextBox();
            this.outputPanel = new System.Windows.Forms.Panel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.inputPanel.SuspendLayout();
            this.inputGroup.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.outputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputPanel
            // 
            this.inputPanel.Controls.Add(this.inputGroup);
            this.inputPanel.Location = new System.Drawing.Point(17, 118);
            this.inputPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(1076, 1035);
            this.inputPanel.TabIndex = 0;
            // 
            // inputGroup
            // 
            this.inputGroup.Controls.Add(this.vScrollBar1);
            this.inputGroup.Location = new System.Drawing.Point(6, 7);
            this.inputGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.inputGroup.Name = "inputGroup";
            this.inputGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.inputGroup.Size = new System.Drawing.Size(1066, 1023);
            this.inputGroup.TabIndex = 0;
            this.inputGroup.TabStop = false;
            this.inputGroup.Text = "Duomenys";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(1029, 12);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(23, 1007);
            this.vScrollBar1.TabIndex = 0;
            this.vScrollBar1.Visible = false;
            // 
            // viewDbButton
            // 
            this.viewDbButton.Location = new System.Drawing.Point(4, 5);
            this.viewDbButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.viewDbButton.Name = "viewDbButton";
            this.viewDbButton.Size = new System.Drawing.Size(150, 78);
            this.viewDbButton.TabIndex = 3;
            this.viewDbButton.Text = "Peržiūrėti duomenų bazę";
            this.viewDbButton.UseVisualStyleBackColor = true;
            this.viewDbButton.Click += new System.EventHandler(this.viewDbButton_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.button1);
            this.buttonPanel.Controls.Add(this.progressBar1);
            this.buttonPanel.Controls.Add(this.progressText);
            this.buttonPanel.Controls.Add(this.classifyButton);
            this.buttonPanel.Controls.Add(this.updateDbButton);
            this.buttonPanel.Controls.Add(this.viewDbButton);
            this.buttonPanel.Location = new System.Drawing.Point(17, 20);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(1950, 88);
            this.buttonPanel.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1637, 5);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 78);
            this.button1.TabIndex = 8;
            this.button1.Text = "Išvalyti duomenis";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ClearData);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(1084, 5);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(544, 77);
            this.progressBar1.TabIndex = 6;
            // 
            // progressText
            // 
            this.progressText.AutoSize = true;
            this.progressText.Location = new System.Drawing.Point(344, 32);
            this.progressText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(0, 25);
            this.progressText.TabIndex = 7;
            // 
            // classifyButton
            // 
            this.classifyButton.Location = new System.Drawing.Point(1796, 5);
            this.classifyButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.classifyButton.Name = "classifyButton";
            this.classifyButton.Size = new System.Drawing.Size(150, 78);
            this.classifyButton.TabIndex = 5;
            this.classifyButton.Text = "Klasifikuoti";
            this.classifyButton.UseVisualStyleBackColor = true;
            this.classifyButton.Click += new System.EventHandler(this.Classify);
            // 
            // updateDbButton
            // 
            this.updateDbButton.Location = new System.Drawing.Point(163, 5);
            this.updateDbButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.updateDbButton.Name = "updateDbButton";
            this.updateDbButton.Size = new System.Drawing.Size(150, 78);
            this.updateDbButton.TabIndex = 4;
            this.updateDbButton.Text = "Atnaujinti duomenų bazę";
            this.updateDbButton.UseVisualStyleBackColor = true;
            this.updateDbButton.Click += new System.EventHandler(this.updateDbButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.errorsTextBox);
            this.groupBox2.Controls.Add(this.warningTextBox);
            this.groupBox2.Controls.Add(this.classificationTextBox);
            this.groupBox2.Location = new System.Drawing.Point(10, 12);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(851, 1017);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Klasifikacija";
            // 
            // errorsTextBox
            // 
            this.errorsTextBox.ForeColor = System.Drawing.Color.Red;
            this.errorsTextBox.Location = new System.Drawing.Point(10, 848);
            this.errorsTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.errorsTextBox.Name = "errorsTextBox";
            this.errorsTextBox.Size = new System.Drawing.Size(831, 156);
            this.errorsTextBox.TabIndex = 2;
            this.errorsTextBox.Text = "";
            // 
            // warningTextBox
            // 
            this.warningTextBox.Location = new System.Drawing.Point(10, 175);
            this.warningTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.warningTextBox.Name = "warningTextBox";
            this.warningTextBox.Size = new System.Drawing.Size(831, 661);
            this.warningTextBox.TabIndex = 1;
            this.warningTextBox.Text = "";
            // 
            // classificationTextBox
            // 
            this.classificationTextBox.Location = new System.Drawing.Point(10, 38);
            this.classificationTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.classificationTextBox.Name = "classificationTextBox";
            this.classificationTextBox.Size = new System.Drawing.Size(831, 124);
            this.classificationTextBox.TabIndex = 0;
            this.classificationTextBox.Text = "";
            // 
            // outputPanel
            // 
            this.outputPanel.Controls.Add(this.groupBox2);
            this.outputPanel.Location = new System.Drawing.Point(1101, 118);
            this.outputPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.Size = new System.Drawing.Size(866, 1040);
            this.outputPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1991, 1180);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.outputPanel);
            this.Controls.Add(this.inputPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Mišinių klasifikatorius";
            this.inputPanel.ResumeLayout(false);
            this.inputGroup.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.outputPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Button viewDbButton;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button updateDbButton;
        private System.Windows.Forms.Button classifyButton;
        private System.Windows.Forms.GroupBox inputGroup;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label progressText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel outputPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.RichTextBox classificationTextBox;
        private System.Windows.Forms.RichTextBox warningTextBox;
        private System.Windows.Forms.RichTextBox errorsTextBox;
    }
}

