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
            tabRegister = new TabControl();
            tabLogIn = new TabPage();
            groupBox1 = new GroupBox();
            btnGoRegister = new Button();
            label1 = new Label();
            LoginButton = new Button();
            label2 = new Label();
            tbxPassword = new TextBox();
            tbxUsername = new TextBox();
            tabATM = new TabPage();
            panel2 = new Panel();
            label18 = new Label();
            btnDownloadStatement = new Button();
            lblAN = new Label();
            lblUserName = new Label();
            label7 = new Label();
            lblBalance = new Label();
            label6 = new Label();
            panel1 = new Panel();
            txtDescription = new TextBox();
            btnWithdraw = new Button();
            btnDeposit = new Button();
            btnLogout = new Button();
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
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            groupBox2 = new GroupBox();
            btnRegister = new Button();
            tbxRegPassword = new TextBox();
            tbxRegUsername = new TextBox();
            tbxRegName = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            lstTransactions = new ListBox();
            tabRegister.SuspendLayout();
            tabLogIn.SuspendLayout();
            groupBox1.SuspendLayout();
            tabATM.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // tabRegister
            // 
            tabRegister.Controls.Add(tabLogIn);
            tabRegister.Controls.Add(tabATM);
            tabRegister.Controls.Add(tabPage1);
            tabRegister.Controls.Add(tabPage2);
            tabRegister.Dock = DockStyle.Fill;
            tabRegister.Location = new Point(0, 0);
            tabRegister.Name = "tabRegister";
            tabRegister.SelectedIndex = 0;
            tabRegister.Size = new Size(800, 450);
            tabRegister.TabIndex = 0;
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
            groupBox1.BackColor = Color.RosyBrown;
            groupBox1.Controls.Add(btnGoRegister);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(LoginButton);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tbxPassword);
            groupBox1.Controls.Add(tbxUsername);
            groupBox1.Location = new Point(128, 52);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(566, 310);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log In";
            // 
            // btnGoRegister
            // 
            btnGoRegister.BackColor = Color.Green;
            btnGoRegister.Location = new Point(295, 161);
            btnGoRegister.Name = "btnGoRegister";
            btnGoRegister.Size = new Size(138, 57);
            btnGoRegister.TabIndex = 6;
            btnGoRegister.Text = "Create New Account";
            btnGoRegister.UseVisualStyleBackColor = false;
            btnGoRegister.Click += btnGoRegister_Click_1;
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
            // LoginButton
            // 
            LoginButton.BackColor = Color.Green;
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
            tabATM.Controls.Add(panel2);
            tabATM.Controls.Add(panel1);
            tabATM.Location = new Point(4, 24);
            tabATM.Name = "tabATM";
            tabATM.Padding = new Padding(3);
            tabATM.Size = new Size(792, 422);
            tabATM.TabIndex = 1;
            tabATM.Text = "ATM";
            tabATM.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(label18);
            panel2.Controls.Add(btnDownloadStatement);
            panel2.Controls.Add(lblAN);
            panel2.Controls.Add(lblUserName);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(lblBalance);
            panel2.Controls.Add(label6);
            panel2.Location = new Point(8, 27);
            panel2.Name = "panel2";
            panel2.Size = new Size(381, 336);
            panel2.TabIndex = 1;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold);
            label18.Location = new Point(22, 183);
            label18.Name = "label18";
            label18.Size = new Size(96, 25);
            label18.TabIndex = 18;
            label18.Text = "Balance :";
            // 
            // btnDownloadStatement
            // 
            btnDownloadStatement.BackColor = Color.Gray;
            btnDownloadStatement.Location = new Point(62, 282);
            btnDownloadStatement.Name = "btnDownloadStatement";
            btnDownloadStatement.Size = new Size(258, 40);
            btnDownloadStatement.TabIndex = 17;
            btnDownloadStatement.Text = "Download Statement";
            btnDownloadStatement.UseVisualStyleBackColor = false;
            btnDownloadStatement.Click += btnDownloadStatement_Click;
            // 
            // lblAN
            // 
            lblAN.AutoSize = true;
            lblAN.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold);
            lblAN.Location = new Point(117, 158);
            lblAN.Name = "lblAN";
            lblAN.Size = new Size(0, 25);
            lblAN.TabIndex = 3;
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold);
            lblUserName.Location = new Point(117, 67);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(0, 25);
            lblUserName.TabIndex = 2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold);
            label7.Location = new Point(22, 112);
            label7.Name = "label7";
            label7.Size = new Size(180, 25);
            label7.TabIndex = 1;
            label7.Text = "Account Number :";
            // 
            // lblBalance
            // 
            lblBalance.AutoSize = true;
            lblBalance.BackColor = Color.Transparent;
            lblBalance.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalance.ForeColor = SystemColors.ControlText;
            lblBalance.Location = new Point(117, 219);
            lblBalance.Name = "lblBalance";
            lblBalance.Size = new Size(0, 25);
            lblBalance.TabIndex = 1;
            lblBalance.TextAlign = ContentAlignment.TopCenter;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Black", 14.25F, FontStyle.Bold);
            label6.Location = new Point(22, 26);
            label6.Name = "label6";
            label6.Size = new Size(78, 25);
            label6.TabIndex = 0;
            label6.Text = "Name :";
            // 
            // panel1
            // 
            panel1.Controls.Add(txtDescription);
            panel1.Controls.Add(btnWithdraw);
            panel1.Controls.Add(btnDeposit);
            panel1.Controls.Add(btnLogout);
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
            panel1.Location = new Point(407, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(361, 395);
            panel1.TabIndex = 0;
            // 
            // txtDescription
            // 
            txtDescription.BackColor = SystemColors.ScrollBar;
            txtDescription.Location = new Point(53, 283);
            txtDescription.MaxLength = 200;
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.PlaceholderText = "Description";
            txtDescription.Size = new Size(258, 74);
            txtDescription.TabIndex = 19;
            // 
            // btnWithdraw
            // 
            btnWithdraw.BackColor = Color.Gray;
            btnWithdraw.Location = new Point(185, 179);
            btnWithdraw.Name = "btnWithdraw";
            btnWithdraw.Size = new Size(126, 40);
            btnWithdraw.TabIndex = 18;
            btnWithdraw.Text = "Withdraw";
            btnWithdraw.UseVisualStyleBackColor = false;
            btnWithdraw.Click += btnWithdraw_Click_1;
            // 
            // btnDeposit
            // 
            btnDeposit.BackColor = Color.Gray;
            btnDeposit.Location = new Point(53, 179);
            btnDeposit.Name = "btnDeposit";
            btnDeposit.Size = new Size(126, 40);
            btnDeposit.TabIndex = 17;
            btnDeposit.Text = "Deposit";
            btnDeposit.UseVisualStyleBackColor = false;
            btnDeposit.Click += btnDeposit_Click;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.Gray;
            btnLogout.Location = new Point(53, 225);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(258, 40);
            btnLogout.TabIndex = 16;
            btnLogout.Text = "Log out";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnZero
            // 
            btnZero.BackColor = Color.Gray;
            btnZero.Location = new Point(251, 133);
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
            btnNine.Location = new Point(185, 133);
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
            btnEight.Location = new Point(119, 133);
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
            btnSeven.Location = new Point(53, 133);
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
            btnSix.Location = new Point(185, 87);
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
            btnFive.Location = new Point(119, 87);
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
            btnFour.Location = new Point(53, 87);
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
            btnClear.Location = new Point(251, 41);
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
            btnThree.Location = new Point(185, 41);
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
            btn2.Location = new Point(119, 41);
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
            btnOne.Location = new Point(53, 41);
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
            lblAmount.Location = new Point(53, 7);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(258, 31);
            lblAmount.TabIndex = 2;
            lblAmount.Text = "0";
            lblAmount.TextAlign = ContentAlignment.TopCenter;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lstTransactions);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 422);
            tabPage1.TabIndex = 2;
            tabPage1.Text = "Transaction History";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 422);
            tabPage2.TabIndex = 3;
            tabPage2.Text = "Register";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = Color.RosyBrown;
            groupBox2.Controls.Add(btnRegister);
            groupBox2.Controls.Add(tbxRegPassword);
            groupBox2.Controls.Add(tbxRegUsername);
            groupBox2.Controls.Add(tbxRegName);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(34, 31);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(725, 370);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Register";
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.Green;
            btnRegister.Location = new Point(134, 252);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(193, 57);
            btnRegister.TabIndex = 6;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // tbxRegPassword
            // 
            tbxRegPassword.Location = new Point(219, 200);
            tbxRegPassword.Name = "tbxRegPassword";
            tbxRegPassword.Size = new Size(281, 23);
            tbxRegPassword.TabIndex = 5;
            tbxRegPassword.UseSystemPasswordChar = true;
            // 
            // tbxRegUsername
            // 
            tbxRegUsername.Location = new Point(219, 140);
            tbxRegUsername.Name = "tbxRegUsername";
            tbxRegUsername.Size = new Size(281, 23);
            tbxRegUsername.TabIndex = 4;
            // 
            // tbxRegName
            // 
            tbxRegName.Location = new Point(219, 82);
            tbxRegName.Name = "tbxRegName";
            tbxRegName.Size = new Size(281, 23);
            tbxRegName.TabIndex = 3;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(50, 195);
            label5.Name = "label5";
            label5.Size = new Size(100, 25);
            label5.TabIndex = 2;
            label5.Text = "Password :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(50, 135);
            label4.Name = "label4";
            label4.Size = new Size(106, 25);
            label4.TabIndex = 1;
            label4.Text = "Username :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(50, 77);
            label3.Name = "label3";
            label3.Size = new Size(106, 25);
            label3.TabIndex = 0;
            label3.Text = "Full Name :";
            // 
            // lstTransactions
            // 
            lstTransactions.Dock = DockStyle.Fill;
            lstTransactions.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lstTransactions.FormattingEnabled = true;
            lstTransactions.Location = new Point(3, 3);
            lstTransactions.Name = "lstTransactions";
            lstTransactions.Size = new Size(786, 416);
            lstTransactions.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabRegister);
            Name = "Form1";
            Text = "ATM Bank";
            tabRegister.ResumeLayout(false);
            tabLogIn.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabATM.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabRegister;
        private TabPage tabLogIn;
        private TabPage tabATM;
        private GroupBox groupBox1;
        private Label label2;
        private TextBox tbxPassword;
        private TextBox tbxUsername;
        private Button LoginButton;
        private Panel panel1;
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
        private Button btnDeposit;
        private Label label1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox2;
        private Button btnRegister;
        private TextBox tbxRegPassword;
        private TextBox tbxRegUsername;
        private TextBox tbxRegName;
        private Label label5;
        private Label label4;
        private Label label3;
        private Button btnGoRegister;
        private Button btnWithdraw;
        private Panel panel2;
        private Label lblAN;
        private Label lblUserName;
        private Label label7;
        private Label label6;
        private Button btnDownloadStatement;
        private Label label18;
        private Label lblBalance;
        private TextBox txtDescription;
        private ListBox lstTransactions;
    }
}
