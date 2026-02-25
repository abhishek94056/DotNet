namespace Calculator
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
            label1 = new Label();
            tbxNumber1 = new TextBox();
            tbxNumber2 = new TextBox();
            tbxOperator = new TextBox();
            btnCalculate = new Button();
            lblResult = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(800, 51);
            label1.TabIndex = 0;
            label1.Text = "Simple Calculator";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbxNumber1
            // 
            tbxNumber1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbxNumber1.Location = new Point(282, 100);
            tbxNumber1.Name = "tbxNumber1";
            tbxNumber1.PlaceholderText = "Enter Number 1";
            tbxNumber1.Size = new Size(237, 33);
            tbxNumber1.TabIndex = 1;
            tbxNumber1.TextAlign = HorizontalAlignment.Center;
            // 
            // tbxNumber2
            // 
            tbxNumber2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbxNumber2.Location = new Point(282, 158);
            tbxNumber2.Name = "tbxNumber2";
            tbxNumber2.PlaceholderText = "Enter Number 2";
            tbxNumber2.Size = new Size(237, 33);
            tbxNumber2.TabIndex = 2;
            tbxNumber2.TextAlign = HorizontalAlignment.Center;
            // 
            // tbxOperator
            // 
            tbxOperator.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbxOperator.Location = new Point(282, 214);
            tbxOperator.Name = "tbxOperator";
            tbxOperator.PlaceholderText = "(+,-,*,/)";
            tbxOperator.Size = new Size(237, 33);
            tbxOperator.TabIndex = 3;
            tbxOperator.TextAlign = HorizontalAlignment.Center;
            // 
            // btnCalculate
            // 
            btnCalculate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCalculate.Location = new Point(282, 274);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(237, 29);
            btnCalculate.TabIndex = 4;
            btnCalculate.Text = "Calculate";
            btnCalculate.UseVisualStyleBackColor = true;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // lblResult
            // 
            lblResult.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblResult.Location = new Point(282, 343);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(237, 23);
            lblResult.TabIndex = 5;
            lblResult.Text = "0";
            lblResult.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblResult);
            Controls.Add(btnCalculate);
            Controls.Add(tbxOperator);
            Controls.Add(tbxNumber2);
            Controls.Add(tbxNumber1);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Calculator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tbxNumber1;
        private TextBox tbxNumber2;
        private TextBox tbxOperator;
        private Button btnCalculate;
        private Label lblResult;
    }
}
