namespace Grocery
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            groupBox1 = new GroupBox();
            comboUnits = new ComboBox();
            lblError = new Label();
            lbxShoppingList = new ListBox();
            btnRemove = new Button();
            btnAdd = new Button();
            tbxAmount = new TextBox();
            tbxItem = new TextBox();
            label2 = new Label();
            label1 = new Label();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(92, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboUnits);
            groupBox1.Controls.Add(lblError);
            groupBox1.Controls.Add(lbxShoppingList);
            groupBox1.Controls.Add(btnRemove);
            groupBox1.Controls.Add(btnAdd);
            groupBox1.Controls.Add(tbxAmount);
            groupBox1.Controls.Add(tbxItem);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(78, 61);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(480, 263);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // comboUnits
            // 
            comboUnits.FormattingEnabled = true;
            comboUnits.Location = new Point(221, 52);
            comboUnits.Name = "comboUnits";
            comboUnits.Size = new Size(121, 23);
            comboUnits.TabIndex = 9;
            // 
            // lblError
            // 
            lblError.BackColor = Color.Red;
            lblError.Dock = DockStyle.Bottom;
            lblError.Enabled = false;
            lblError.ForeColor = Color.White;
            lblError.Location = new Point(3, 237);
            lblError.Name = "lblError";
            lblError.Size = new Size(474, 23);
            lblError.TabIndex = 8;
            lblError.Text = "label3";
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Click += lblError_Click;
            // 
            // lbxShoppingList
            // 
            lbxShoppingList.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbxShoppingList.FormattingEnabled = true;
            lbxShoppingList.Location = new Point(25, 91);
            lbxShoppingList.Name = "lbxShoppingList";
            lbxShoppingList.Size = new Size(410, 124);
            lbxShoppingList.TabIndex = 7;
            lbxShoppingList.SelectedIndexChanged += lbxShoppingList_SelectedIndexChanged;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(360, 55);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 6;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btmRemove_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(360, 23);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // tbxAmount
            // 
            tbxAmount.Location = new Point(115, 52);
            tbxAmount.Name = "tbxAmount";
            tbxAmount.Size = new Size(100, 23);
            tbxAmount.TabIndex = 3;
            // 
            // tbxItem
            // 
            tbxItem.Location = new Point(115, 23);
            tbxItem.Name = "tbxItem";
            tbxItem.Size = new Size(227, 23);
            tbxItem.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 59);
            label2.Name = "label2";
            label2.Size = new Size(51, 15);
            label2.TabIndex = 1;
            label2.Text = "Amount";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 31);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 0;
            label1.Text = "Description";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private GroupBox groupBox1;
        private Label label2;
        private Label label1;
        private Label lblError;
        private ListBox lbxShoppingList;
        private Button btnRemove;
        private Button btnAdd;
        private TextBox tbxAmount;
        private TextBox tbxItem;
        private ComboBox comboUnits;
    }
}
