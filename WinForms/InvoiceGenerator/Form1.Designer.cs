namespace InvoiceGenerator
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
            btnGeneratePDF = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            tbxIN = new TextBox();
            tbxID = new TextBox();
            tbxRate = new TextBox();
            tbxQty = new TextBox();
            tbxHC = new TextBox();
            panel1 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            label7 = new Label();
            tbxpdf = new TextBox();
            tbxSN = new TextBox();
            label8 = new Label();
            btnSubmit = new Button();
            btnClear = new Button();
            tabPage2 = new TabPage();
            dgvItems = new DataGridView();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            SuspendLayout();
            // 
            // btnGeneratePDF
            // 
            btnGeneratePDF.BackColor = SystemColors.HotTrack;
            btnGeneratePDF.Location = new Point(515, 209);
            btnGeneratePDF.Name = "btnGeneratePDF";
            btnGeneratePDF.Size = new Size(222, 34);
            btnGeneratePDF.TabIndex = 0;
            btnGeneratePDF.Text = "PDF";
            btnGeneratePDF.UseVisualStyleBackColor = false;
            btnGeneratePDF.Click += btnGeneratePDF_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(301, 3);
            label1.Name = "label1";
            label1.Size = new Size(129, 25);
            label1.TabIndex = 1;
            label1.Text = "Register Item";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(65, 74);
            label2.Name = "label2";
            label2.Size = new Size(91, 20);
            label2.TabIndex = 2;
            label2.Text = "Invoice No :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(65, 134);
            label3.Name = "label3";
            label3.Size = new Size(129, 20);
            label3.TabIndex = 3;
            label3.Text = "Item Description :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(65, 209);
            label4.Name = "label4";
            label4.Size = new Size(87, 20);
            label4.TabIndex = 4;
            label4.Text = "HSN Code :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(65, 238);
            label5.Name = "label5";
            label5.Size = new Size(47, 20);
            label5.TabIndex = 5;
            label5.Text = "Rate :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(65, 267);
            label6.Name = "label6";
            label6.Size = new Size(41, 20);
            label6.TabIndex = 6;
            label6.Text = "Qty :";
            // 
            // tbxIN
            // 
            tbxIN.BackColor = SystemColors.InactiveCaption;
            tbxIN.Location = new Point(244, 71);
            tbxIN.Name = "tbxIN";
            tbxIN.PlaceholderText = "Invoice No";
            tbxIN.Size = new Size(222, 23);
            tbxIN.TabIndex = 8;
            // 
            // tbxID
            // 
            tbxID.BackColor = SystemColors.InactiveCaption;
            tbxID.Location = new Point(244, 134);
            tbxID.Multiline = true;
            tbxID.Name = "tbxID";
            tbxID.PlaceholderText = "Item Description";
            tbxID.Size = new Size(222, 66);
            tbxID.TabIndex = 9;
            // 
            // tbxRate
            // 
            tbxRate.BackColor = SystemColors.InactiveCaption;
            tbxRate.Location = new Point(244, 235);
            tbxRate.Name = "tbxRate";
            tbxRate.PlaceholderText = "Rate";
            tbxRate.Size = new Size(222, 23);
            tbxRate.TabIndex = 10;
            // 
            // tbxQty
            // 
            tbxQty.BackColor = SystemColors.InactiveCaption;
            tbxQty.Location = new Point(244, 264);
            tbxQty.Name = "tbxQty";
            tbxQty.PlaceholderText = "Qty";
            tbxQty.Size = new Size(222, 23);
            tbxQty.TabIndex = 11;
            // 
            // tbxHC
            // 
            tbxHC.BackColor = SystemColors.InactiveCaption;
            tbxHC.Location = new Point(244, 206);
            tbxHC.Name = "tbxHC";
            tbxHC.PlaceholderText = "HSN Code";
            tbxHC.Size = new Size(222, 23);
            tbxHC.TabIndex = 10;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.HighlightText;
            panel1.Controls.Add(tabControl1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 450);
            panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 450);
            tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.DarkTurquoise;
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(tbxpdf);
            tabPage1.Controls.Add(tbxSN);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(btnGeneratePDF);
            tabPage1.Controls.Add(btnSubmit);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(btnClear);
            tabPage1.Controls.Add(tbxIN);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(tbxQty);
            tabPage1.Controls.Add(tbxHC);
            tabPage1.Controls.Add(tbxRate);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(tbxID);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label5);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 422);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Register Item";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(515, 154);
            label7.Name = "label7";
            label7.Size = new Size(207, 20);
            label7.TabIndex = 18;
            label7.Text = "Enter Invoice no to print PDF";
            // 
            // tbxpdf
            // 
            tbxpdf.BackColor = SystemColors.InactiveCaption;
            tbxpdf.Location = new Point(555, 177);
            tbxpdf.Name = "tbxpdf";
            tbxpdf.PlaceholderText = "Invoice No";
            tbxpdf.Size = new Size(132, 23);
            tbxpdf.TabIndex = 17;
            // 
            // tbxSN
            // 
            tbxSN.BackColor = SystemColors.InactiveCaption;
            tbxSN.Location = new Point(244, 100);
            tbxSN.Name = "tbxSN";
            tbxSN.PlaceholderText = "Sr No";
            tbxSN.Size = new Size(222, 23);
            tbxSN.TabIndex = 16;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(65, 103);
            label8.Name = "label8";
            label8.Size = new Size(56, 20);
            label8.TabIndex = 15;
            label8.Text = "Sr No :";
            // 
            // btnSubmit
            // 
            btnSubmit.BackColor = SystemColors.ControlDarkDark;
            btnSubmit.Location = new Point(281, 299);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(150, 30);
            btnSubmit.TabIndex = 14;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = SystemColors.ControlDarkDark;
            btnClear.Location = new Point(81, 299);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(150, 30);
            btnClear.TabIndex = 13;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dgvItems);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 422);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Show Item";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvItems
            // 
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.Location = new Point(3, 3);
            dgvItems.Name = "dgvItems";
            dgvItems.Size = new Size(786, 416);
            dgvItems.TabIndex = 0;
            dgvItems.CellContentClick += dgvItems_CellContentClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnGeneratePDF;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox tbxIN;
        private TextBox tbxID;
        private TextBox tbxRate;
        private TextBox tbxQty;
        private TextBox tbxHC;
        private Panel panel1;
        private Button btnClear;
        private Button btnSubmit;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dgvItems;
        private TextBox tbxSN;
        private Label label8;
        private Label label7;
        private TextBox tbxpdf;
    }
}
