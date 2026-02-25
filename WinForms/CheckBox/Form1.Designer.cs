namespace CheckBox
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
            cbChees = new System.Windows.Forms.CheckBox();
            cbMushroom = new System.Windows.Forms.CheckBox();
            cbCake = new System.Windows.Forms.CheckBox();
            lblPzzaSelection = new Label();
            btnOrder = new Button();
            SuspendLayout();
            // 
            // cbChees
            // 
            cbChees.AutoSize = true;
            cbChees.Location = new Point(263, 89);
            cbChees.Name = "cbChees";
            cbChees.Size = new Size(58, 19);
            cbChees.TabIndex = 0;
            cbChees.Text = "Chees";
            cbChees.UseVisualStyleBackColor = true;
            cbChees.CheckedChanged += PzzaSelection_CheckChange;
            // 
            // cbMushroom
            // 
            cbMushroom.AutoSize = true;
            cbMushroom.Location = new Point(263, 154);
            cbMushroom.Name = "cbMushroom";
            cbMushroom.Size = new Size(85, 19);
            cbMushroom.TabIndex = 1;
            cbMushroom.Text = "Mushroom";
            cbMushroom.UseVisualStyleBackColor = true;
            cbMushroom.CheckedChanged += PzzaSelection_CheckChange;
            // 
            // cbCake
            // 
            cbCake.AutoSize = true;
            cbCake.Location = new Point(263, 218);
            cbCake.Name = "cbCake";
            cbCake.Size = new Size(52, 19);
            cbCake.TabIndex = 2;
            cbCake.Text = "Cake";
            cbCake.UseVisualStyleBackColor = true;
            cbCake.CheckedChanged += PzzaSelection_CheckChange;
            // 
            // lblPzzaSelection
            // 
            lblPzzaSelection.AutoSize = true;
            lblPzzaSelection.Location = new Point(483, 158);
            lblPzzaSelection.Name = "lblPzzaSelection";
            lblPzzaSelection.Size = new Size(38, 15);
            lblPzzaSelection.TabIndex = 3;
            lblPzzaSelection.Text = "label1";
            // 
            // btnOrder
            // 
            btnOrder.Location = new Point(481, 218);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(75, 23);
            btnOrder.TabIndex = 4;
            btnOrder.Text = "PlaceOrder";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnOrder);
            Controls.Add(lblPzzaSelection);
            Controls.Add(cbCake);
            Controls.Add(cbMushroom);
            Controls.Add(cbChees);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox cbChees;
        private System.Windows.Forms.CheckBox cbMushroom;
        private System.Windows.Forms.CheckBox cbCake;
        private Label lblPzzaSelection;
        private Button btnOrder;
    }
}
