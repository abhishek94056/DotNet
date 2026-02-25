namespace RadioButton
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
            radioVisa = new System.Windows.Forms.RadioButton();
            radioCash = new System.Windows.Forms.RadioButton();
            radioPayPal = new System.Windows.Forms.RadioButton();
            lblSelection = new Label();
            SuspendLayout();
            // 
            // radioVisa
            // 
            radioVisa.AutoSize = true;
            radioVisa.Location = new Point(384, 85);
            radioVisa.Name = "radioVisa";
            radioVisa.Size = new Size(46, 19);
            radioVisa.TabIndex = 0;
            radioVisa.TabStop = true;
            radioVisa.Text = "Visa";
            radioVisa.UseVisualStyleBackColor = true;
            radioVisa.CheckedChanged += RadioButtonSelection_CheckChange;
            // 
            // radioCash
            // 
            radioCash.AutoSize = true;
            radioCash.Location = new Point(384, 125);
            radioCash.Name = "radioCash";
            radioCash.Size = new Size(51, 19);
            radioCash.TabIndex = 1;
            radioCash.TabStop = true;
            radioCash.Text = "Cash";
            radioCash.UseVisualStyleBackColor = true;
            radioCash.CheckedChanged += RadioButtonSelection_CheckChange;
            // 
            // radioPayPal
            // 
            radioPayPal.AutoSize = true;
            radioPayPal.Location = new Point(384, 167);
            radioPayPal.Name = "radioPayPal";
            radioPayPal.Size = new Size(60, 19);
            radioPayPal.TabIndex = 2;
            radioPayPal.TabStop = true;
            radioPayPal.Text = "PayPal";
            radioPayPal.UseVisualStyleBackColor = true;
            radioPayPal.CheckedChanged += RadioButtonSelection_CheckChange;
            // 
            // lblSelection
            // 
            lblSelection.AutoSize = true;
            lblSelection.Location = new Point(532, 127);
            lblSelection.Name = "lblSelection";
            lblSelection.Size = new Size(38, 15);
            lblSelection.TabIndex = 3;
            lblSelection.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblSelection);
            Controls.Add(radioPayPal);
            Controls.Add(radioCash);
            Controls.Add(radioVisa);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton radioVisa;
        private System.Windows.Forms.RadioButton radioCash;
        private System.Windows.Forms.RadioButton radioPayPal;
        private Label lblSelection;
    }
}
