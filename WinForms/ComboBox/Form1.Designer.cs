namespace ComboBox
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
            comboVechicle = new System.Windows.Forms.ComboBox();
            colorDialog1 = new ColorDialog();
            label1 = new Label();
            label2 = new Label();
            lblParkingStatus = new Label();
            btnPark = new Button();
            tbxregNumber = new TextBox();
            SuspendLayout();
            // 
            // comboVechicle
            // 
            comboVechicle.DropDownStyle = ComboBoxStyle.DropDownList;
            comboVechicle.FormattingEnabled = true;
            comboVechicle.Location = new Point(280, 86);
            comboVechicle.Name = "comboVechicle";
            comboVechicle.Size = new Size(121, 23);
            comboVechicle.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(206, 43);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 3;
            label1.Text = "Reg Num";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(206, 89);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 4;
            label2.Text = "Type";
            // 
            // lblParkingStatus
            // 
            lblParkingStatus.AutoSize = true;
            lblParkingStatus.Location = new Point(503, 56);
            lblParkingStatus.Name = "lblParkingStatus";
            lblParkingStatus.Size = new Size(82, 15);
            lblParkingStatus.TabIndex = 5;
            lblParkingStatus.Text = "Parking Hours";
            // 
            // btnPark
            // 
            btnPark.Location = new Point(326, 185);
            btnPark.Name = "btnPark";
            btnPark.Size = new Size(75, 23);
            btnPark.TabIndex = 6;
            btnPark.Text = "Park";
            btnPark.UseVisualStyleBackColor = true;
            btnPark.Click += btnPark_Click;
            // 
            // tbxregNumber
            // 
            tbxregNumber.Location = new Point(307, 50);
            tbxregNumber.Name = "tbxregNumber";
            tbxregNumber.Size = new Size(100, 23);
            tbxregNumber.TabIndex = 7;
            tbxregNumber.TextChanged += tbxregNumber_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tbxregNumber);
            Controls.Add(btnPark);
            Controls.Add(lblParkingStatus);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboVechicle);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox comboVechicle;
        private ColorDialog colorDialog1;
        private Label label1;
        private Label label2;
        private Label lblParkingStatus;
        private Button btnPark;
        private TextBox tbxregNumber;
    }
}
