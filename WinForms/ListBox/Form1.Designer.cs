namespace ListBox
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
            lbxName = new System.Windows.Forms.ListBox();
            tbxName = new TextBox();
            btnAddName = new Button();
            btnClear = new Button();
            SuspendLayout();
            // 
            // lbxName
            // 
            lbxName.FormattingEnabled = true;
            lbxName.Location = new Point(12, 49);
            lbxName.Name = "lbxName";
            lbxName.Size = new Size(244, 229);
            lbxName.TabIndex = 0;
            // 
            // tbxName
            // 
            tbxName.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbxName.Location = new Point(316, 49);
            tbxName.Name = "tbxName";
            tbxName.Size = new Size(186, 29);
            tbxName.TabIndex = 1;
            // 
            // btnAddName
            // 
            btnAddName.Location = new Point(316, 104);
            btnAddName.Name = "btnAddName";
            btnAddName.Size = new Size(186, 23);
            btnAddName.TabIndex = 2;
            btnAddName.Text = "Add Name";
            btnAddName.UseVisualStyleBackColor = true;
            btnAddName.Click += btnAddName_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(316, 143);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(186, 23);
            btnClear.TabIndex = 3;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnClear);
            Controls.Add(btnAddName);
            Controls.Add(tbxName);
            Controls.Add(lbxName);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox lbxName;
        private TextBox tbxName;
        private Button btnAddName;
        private Button btnClear;
    }
}
