namespace ATM
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
            tabControl1 = new TabControl();
            tabLogIn = new TabPage();
            groupBox1 = new GroupBox();
            LoginButton = new Button();
            label2 = new Label();
            tbxPassword = new TextBox();
            tbxUsername = new TextBox();
            tabATM = new TabPage();
            panel1 = new Panel();
            btnDeposit = new Button();
            btnLogout = new Button();
            btnWithdraw = new Button();
            btnZero = new Button();
            btnNine = new Button();
            btnEight = new Button();
            btnSeven = new Button();
            btnSix = new Button();
            btnFive = new Button();
            btnFour = new Button();
            btnClear = new Button();
            btnThree = new Button();
            btn2 = new Button();
            btnOne = new Button();
            lblAmount = new Label();
            lblBalance = new Label();
            lblUserName = new Label();
            label1 = new Label();
            tabControl1.SuspendLayout();
            tabLogIn.SuspendLayout();
            groupBox1.SuspendLayout();
            tabATM.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabLogIn);
            tabControl1.Controls.Add(tabATM);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 450);
            tabControl1.TabIndex = 0;
            // 
            // tabLogIn
            // 
            tabLogIn.Controls.Add(groupBox1);
            tabLogIn.Location = new Point(4, 24);
            tabLogIn.Name = "tabLogIn";
            tabLogIn.Padding = new Padding(3);
            tabLogIn.Size = new Size(792, 422);
            tabLogIn.TabIndex = 0;
            tabLogIn.Text = "Log in";
            tabLogIn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(LoginButton);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tbxPassword);
            groupBox1.Controls.Add(tbxUsername);
            groupBox1.Location = new Point(127, 38);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(529, 308);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log In";
            // 
            // LoginButton
            // 
            LoginButton.BackColor = Color.Gray;
            LoginButton.Location = new Point(131, 161);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(138, 57);
            LoginButton.TabIndex = 4;
            LoginButton.Text = "Log In";
            LoginButton.UseVisualStyleBackColor = false;
            LoginButton.Click += LoginButton_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(35, 111);
            label2.Name = "label2";
            label2.Size = new Size(111, 27);
            label2.TabIndex = 3;
            label2.Text = "Password :";
            // 
            // tbxPassword
            // 
            tbxPassword.Location = new Point(218, 111);
            tbxPassword.Name = "tbxPassword";
            tbxPassword.Size = new Size(195, 23);
            tbxPassword.TabIndex = 1;
            // 
            // tbxUsername
            // 
            tbxUsername.Location = new Point(218, 59);
            tbxUsername.Name = "tbxUsername";
            tbxUsername.Size = new Size(195, 23);
            tbxUsername.TabIndex = 0;
            // 
            // tabATM
            // 
            tabATM.Controls.Add(panel1);
            tabATM.Location = new Point(4, 24);
            tabATM.Name = "tabATM";
            tabATM.Padding = new Padding(3);
            tabATM.Size = new Size(792, 422);
            tabATM.TabIndex = 1;
            tabATM.Text = "ATM";
            tabATM.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnDeposit);
            panel1.Controls.Add(btnLogout);
            panel1.Controls.Add(btnWithdraw);
            panel1.Controls.Add(btnZero);
            panel1.Controls.Add(btnNine);
            panel1.Controls.Add(btnEight);
            panel1.Controls.Add(btnSeven);
            panel1.Controls.Add(btnSix);
            panel1.Controls.Add(btnFive);
            panel1.Controls.Add(btnFour);
            panel1.Controls.Add(btnClear);
            panel1.Controls.Add(btnThree);
            panel1.Controls.Add(btn2);
            panel1.Controls.Add(btnOne);
            panel1.Controls.Add(lblAmount);
            panel1.Controls.Add(lblBalance);
            panel1.Controls.Add(lblUserName);
            panel1.Location = new Point(99, 34);
            panel1.Name = "panel1";
            panel1.Size = new Size(547, 362);
            panel1.TabIndex = 0;
            // 
            // btnDeposit
            // 
            btnDeposit.BackColor = Color.Gray;
            btnDeposit.Location = new Point(109, 271);
            btnDeposit.Name = "btnDeposit";
            btnDeposit.Size = new Size(144, 40);
            btnDeposit.TabIndex = 17;
            btnDeposit.Text = "Deposit";
            btnDeposit.UseVisualStyleBackColor = false;
            btnDeposit.Click += btnDeposit_Click;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.Gray;
            btnLogout.Location = new Point(108, 317);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(317, 40);
            btnLogout.TabIndex = 16;
            btnLogout.Text = "Log out";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnWithdraw
            // 
            btnWithdraw.BackColor = Color.Gray;
            btnWithdraw.Location = new Point(281, 271);
            btnWithdraw.Name = "btnWithdraw";
            btnWithdraw.Size = new Size(144, 40);
            btnWithdraw.TabIndex = 15;
            btnWithdraw.Text = "Withdraw";
            btnWithdraw.UseVisualStyleBackColor = false;
            btnWithdraw.Click += btnWithdraw_Click;
            // 
            // btnZero
            // 
            btnZero.BackColor = Color.Gray;
            btnZero.Location = new Point(365, 225);
            btnZero.Name = "btnZero";
            btnZero.Size = new Size(60, 40);
            btnZero.TabIndex = 13;
            btnZero.Text = "0";
            btnZero.UseVisualStyleBackColor = false;
            btnZero.Click += NumberPad_Click;
            // 
            // btnNine
            // 
            btnNine.BackColor = Color.Gray;
            btnNine.Location = new Point(281, 225);
            btnNine.Name = "btnNine";
            btnNine.Size = new Size(60, 40);
            btnNine.TabIndex = 12;
            btnNine.Text = "9";
            btnNine.UseVisualStyleBackColor = false;
            btnNine.Click += NumberPad_Click;
            // 
            // btnEight
            // 
            btnEight.BackColor = Color.Gray;
            btnEight.Location = new Point(193, 225);
            btnEight.Name = "btnEight";
            btnEight.Size = new Size(60, 40);
            btnEight.TabIndex = 11;
            btnEight.Text = "8";
            btnEight.UseVisualStyleBackColor = false;
            btnEight.Click += NumberPad_Click;
            // 
            // btnSeven
            // 
            btnSeven.BackColor = Color.Gray;
            btnSeven.Location = new Point(108, 225);
            btnSeven.Name = "btnSeven";
            btnSeven.Size = new Size(60, 40);
            btnSeven.TabIndex = 10;
            btnSeven.Text = "7";
            btnSeven.UseVisualStyleBackColor = false;
            btnSeven.Click += NumberPad_Click;
            // 
            // btnSix
            // 
            btnSix.BackColor = Color.Gray;
            btnSix.Location = new Point(281, 179);
            btnSix.Name = "btnSix";
            btnSix.Size = new Size(60, 40);
            btnSix.TabIndex = 9;
            btnSix.Text = "6";
            btnSix.UseVisualStyleBackColor = false;
            btnSix.Click += NumberPad_Click;
            // 
            // btnFive
            // 
            btnFive.BackColor = Color.Gray;
            btnFive.Location = new Point(193, 179);
            btnFive.Name = "btnFive";
            btnFive.Size = new Size(60, 40);
            btnFive.TabIndex = 8;
            btnFive.Text = "5";
            btnFive.UseVisualStyleBackColor = false;
            btnFive.Click += NumberPad_Click;
            // 
            // btnFour
            // 
            btnFour.BackColor = Color.Gray;
            btnFour.Location = new Point(108, 179);
            btnFour.Name = "btnFour";
            btnFour.Size = new Size(60, 40);
            btnFour.TabIndex = 7;
            btnFour.Text = "4";
            btnFour.UseVisualStyleBackColor = false;
            btnFour.Click += NumberPad_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.Gray;
            btnClear.Location = new Point(365, 133);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(60, 86);
            btnClear.TabIndex = 6;
            btnClear.Text = "C";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += ClearButton_Click;
            // 
            // btnThree
            // 
            btnThree.BackColor = Color.Gray;
            btnThree.Location = new Point(281, 133);
            btnThree.Name = "btnThree";
            btnThree.Size = new Size(60, 40);
            btnThree.TabIndex = 5;
            btnThree.Text = "3";
            btnThree.UseVisualStyleBackColor = false;
            btnThree.Click += NumberPad_Click;
            // 
            // btn2
            // 
            btn2.BackColor = Color.Gray;
            btn2.Location = new Point(193, 133);
            btn2.Name = "btn2";
            btn2.Size = new Size(60, 40);
            btn2.TabIndex = 4;
            btn2.Text = "2";
            btn2.UseVisualStyleBackColor = false;
            btn2.Click += NumberPad_Click;
            // 
            // btnOne
            // 
            btnOne.BackColor = Color.Gray;
            btnOne.Location = new Point(108, 133);
            btnOne.Name = "btnOne";
            btnOne.Size = new Size(60, 40);
            btnOne.TabIndex = 3;
            btnOne.Text = "1";
            btnOne.UseVisualStyleBackColor = false;
            btnOne.Click += NumberPad_Click;
            // 
            // lblAmount
            // 
            lblAmount.BackColor = Color.Green;
            lblAmount.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAmount.ForeColor = SystemColors.Window;
            lblAmount.Location = new Point(108, 86);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(317, 31);
            lblAmount.TabIndex = 2;
            lblAmount.Text = "0";
            lblAmount.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblBalance
            // 
            lblBalance.BackColor = Color.Green;
            lblBalance.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBalance.ForeColor = SystemColors.Window;
            lblBalance.Location = new Point(108, 44);
            lblBalance.Name = "lblBalance";
            lblBalance.Size = new Size(317, 32);
            lblBalance.TabIndex = 1;
            lblBalance.Text = "Balance: 0";
            lblBalance.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblUserName
            // 
            lblUserName.Dock = DockStyle.Top;
            lblUserName.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUserName.Location = new Point(0, 0);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(547, 23);
            lblUserName.TabIndex = 0;
            lblUserName.Text = "label3";
            lblUserName.TextAlign = ContentAlignment.TopCenter;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(35, 54);
            label1.Name = "label1";
            label1.Size = new Size(111, 42);
            label1.TabIndex = 5;
            label1.Text = "Username :";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "ATM Bank";
            tabControl1.ResumeLayout(false);
            tabLogIn.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabATM.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabLogIn;
        private TabPage tabATM;
        private GroupBox groupBox1;
        private Label label2;
        private TextBox tbxPassword;
        private TextBox tbxUsername;
        private Button LoginButton;
        private Panel panel1;
        private Label lblBalance;
        private Label lblUserName;
        private Label lblAmount;
        private Button btnZero;
        private Button btnNine;
        private Button btnEight;
        private Button btnSeven;
        private Button btnSix;
        private Button btnFive;
        private Button btnFour;
        private Button btnClear;
        private Button btnThree;
        private Button btn2;
        private Button btnOne;
        private Button btnLogout;
        private Button btnWithdraw;
        private Button btnDeposit;
        private Label label1;
    }
}
