namespace ATM_2
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
            tabLogin = new TabPage();
            panelLogin = new Panel();
            tbxRegister = new Button();
            btnLogin = new Button();
            tbxPassword = new TextBox();
            tbxUserName = new TextBox();
            label2 = new Label();
            label1 = new Label();
            tabRegister = new TabPage();
            panelRegister = new Panel();
            tbxRegister_ = new Button();
            tbxPassword_ = new TextBox();
            tbxUserName_ = new TextBox();
            tbxFullName = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            tabATM = new TabPage();
            panelATM = new Panel();
            lblAmount = new Label();
            btnDeposit = new Button();
            btnLogOut = new Button();
            tbxDescription = new TextBox();
            btnWithdraw = new Button();
            btnNine = new Button();
            btnEight = new Button();
            btnSeven = new Button();
            btnZero = new Button();
            btnSix = new Button();
            btnFive = new Button();
            btnFour = new Button();
            btnClear = new Button();
            btnThree = new Button();
            btnTwo = new Button();
            btnOne = new Button();
            btnStatement = new Button();
            lblBalance = new Label();
            lblAccountNumber = new Label();
            lblName = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            tabTransactionHistory = new TabPage();
            lbStatement = new ListBox();
            tabControl1.SuspendLayout();
            tabLogin.SuspendLayout();
            panelLogin.SuspendLayout();
            tabRegister.SuspendLayout();
            panelRegister.SuspendLayout();
            tabATM.SuspendLayout();
            panelATM.SuspendLayout();
            tabTransactionHistory.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabLogin);
            tabControl1.Controls.Add(tabRegister);
            tabControl1.Controls.Add(tabATM);
            tabControl1.Controls.Add(tabTransactionHistory);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 450);
            tabControl1.TabIndex = 0;
            // 
            // tabLogin
            // 
            tabLogin.Controls.Add(panelLogin);
            tabLogin.Location = new Point(4, 24);
            tabLogin.Name = "tabLogin";
            tabLogin.Padding = new Padding(3);
            tabLogin.Size = new Size(792, 422);
            tabLogin.TabIndex = 0;
            tabLogin.Text = "Log in";
            tabLogin.UseVisualStyleBackColor = true;
            // 
            // panelLogin
            // 
            panelLogin.BackColor = Color.Cyan;
            panelLogin.Controls.Add(tbxRegister);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(tbxPassword);
            panelLogin.Controls.Add(tbxUserName);
            panelLogin.Controls.Add(label2);
            panelLogin.Controls.Add(label1);
            panelLogin.Dock = DockStyle.Fill;
            panelLogin.Location = new Point(3, 3);
            panelLogin.Name = "panelLogin";
            panelLogin.Size = new Size(786, 416);
            panelLogin.TabIndex = 0;
            // 
            // tbxRegister
            // 
            tbxRegister.BackColor = Color.SlateGray;
            tbxRegister.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbxRegister.Location = new Point(393, 232);
            tbxRegister.Name = "tbxRegister";
            tbxRegister.Size = new Size(193, 48);
            tbxRegister.TabIndex = 5;
            tbxRegister.Text = "Register";
            tbxRegister.UseVisualStyleBackColor = false;
            tbxRegister.Click += tbxRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Green;
            btnLogin.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.Location = new Point(136, 232);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(203, 48);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Log in";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // tbxPassword
            // 
            tbxPassword.BackColor = SystemColors.InactiveCaption;
            tbxPassword.Location = new Point(348, 145);
            tbxPassword.Name = "tbxPassword";
            tbxPassword.PlaceholderText = "Enter Password";
            tbxPassword.Size = new Size(178, 23);
            tbxPassword.TabIndex = 3;
            // 
            // tbxUserName
            // 
            tbxUserName.BackColor = SystemColors.InactiveCaption;
            tbxUserName.Location = new Point(348, 90);
            tbxUserName.Name = "tbxUserName";
            tbxUserName.PlaceholderText = "Enter UserName";
            tbxUserName.Size = new Size(178, 23);
            tbxUserName.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlText;
            label2.Location = new Point(181, 145);
            label2.Name = "label2";
            label2.Size = new Size(101, 25);
            label2.TabIndex = 1;
            label2.Text = "Password :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(181, 88);
            label1.Name = "label1";
            label1.Size = new Size(108, 25);
            label1.TabIndex = 0;
            label1.Text = "Username :";
            // 
            // tabRegister
            // 
            tabRegister.Controls.Add(panelRegister);
            tabRegister.Location = new Point(4, 24);
            tabRegister.Name = "tabRegister";
            tabRegister.Padding = new Padding(3);
            tabRegister.Size = new Size(792, 422);
            tabRegister.TabIndex = 1;
            tabRegister.Text = "Register";
            tabRegister.UseVisualStyleBackColor = true;
            // 
            // panelRegister
            // 
            panelRegister.BackColor = Color.Cyan;
            panelRegister.Controls.Add(tbxRegister_);
            panelRegister.Controls.Add(tbxPassword_);
            panelRegister.Controls.Add(tbxUserName_);
            panelRegister.Controls.Add(tbxFullName);
            panelRegister.Controls.Add(label5);
            panelRegister.Controls.Add(label4);
            panelRegister.Controls.Add(label3);
            panelRegister.Dock = DockStyle.Fill;
            panelRegister.Location = new Point(3, 3);
            panelRegister.Name = "panelRegister";
            panelRegister.Size = new Size(786, 416);
            panelRegister.TabIndex = 0;
            // 
            // tbxRegister_
            // 
            tbxRegister_.BackColor = Color.Green;
            tbxRegister_.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbxRegister_.Location = new Point(207, 254);
            tbxRegister_.Name = "tbxRegister_";
            tbxRegister_.Size = new Size(238, 55);
            tbxRegister_.TabIndex = 7;
            tbxRegister_.Text = "Register";
            tbxRegister_.UseVisualStyleBackColor = false;
            tbxRegister_.Click += tbxRegister__Click;
            // 
            // tbxPassword_
            // 
            tbxPassword_.BackColor = SystemColors.InactiveCaption;
            tbxPassword_.Location = new Point(325, 181);
            tbxPassword_.Name = "tbxPassword_";
            tbxPassword_.PlaceholderText = "Enter Password";
            tbxPassword_.Size = new Size(178, 23);
            tbxPassword_.TabIndex = 6;
            // 
            // tbxUserName_
            // 
            tbxUserName_.BackColor = SystemColors.InactiveCaption;
            tbxUserName_.Location = new Point(325, 127);
            tbxUserName_.Name = "tbxUserName_";
            tbxUserName_.PlaceholderText = "Enter UserName";
            tbxUserName_.Size = new Size(178, 23);
            tbxUserName_.TabIndex = 5;
            // 
            // tbxFullName
            // 
            tbxFullName.BackColor = SystemColors.InactiveCaption;
            tbxFullName.Location = new Point(325, 80);
            tbxFullName.Name = "tbxFullName";
            tbxFullName.PlaceholderText = "Enter Full Name";
            tbxFullName.Size = new Size(178, 23);
            tbxFullName.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ControlText;
            label5.Location = new Point(166, 181);
            label5.Name = "label5";
            label5.Size = new Size(101, 25);
            label5.TabIndex = 3;
            label5.Text = "Password :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlText;
            label4.Location = new Point(166, 125);
            label4.Name = "label4";
            label4.Size = new Size(108, 25);
            label4.TabIndex = 2;
            label4.Text = "Username :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlText;
            label3.Location = new Point(166, 75);
            label3.Name = "label3";
            label3.Size = new Size(110, 25);
            label3.TabIndex = 1;
            label3.Text = "Full Name :";
            // 
            // tabATM
            // 
            tabATM.Controls.Add(panelATM);
            tabATM.Location = new Point(4, 24);
            tabATM.Name = "tabATM";
            tabATM.Padding = new Padding(3);
            tabATM.Size = new Size(792, 422);
            tabATM.TabIndex = 2;
            tabATM.Text = "ATM";
            tabATM.UseVisualStyleBackColor = true;
            // 
            // panelATM
            // 
            panelATM.BackColor = Color.Cyan;
            panelATM.Controls.Add(lblAmount);
            panelATM.Controls.Add(btnDeposit);
            panelATM.Controls.Add(btnLogOut);
            panelATM.Controls.Add(tbxDescription);
            panelATM.Controls.Add(btnWithdraw);
            panelATM.Controls.Add(btnNine);
            panelATM.Controls.Add(btnEight);
            panelATM.Controls.Add(btnSeven);
            panelATM.Controls.Add(btnZero);
            panelATM.Controls.Add(btnSix);
            panelATM.Controls.Add(btnFive);
            panelATM.Controls.Add(btnFour);
            panelATM.Controls.Add(btnClear);
            panelATM.Controls.Add(btnThree);
            panelATM.Controls.Add(btnTwo);
            panelATM.Controls.Add(btnOne);
            panelATM.Controls.Add(btnStatement);
            panelATM.Controls.Add(lblBalance);
            panelATM.Controls.Add(lblAccountNumber);
            panelATM.Controls.Add(lblName);
            panelATM.Controls.Add(label8);
            panelATM.Controls.Add(label7);
            panelATM.Controls.Add(label6);
            panelATM.Dock = DockStyle.Fill;
            panelATM.Location = new Point(3, 3);
            panelATM.Name = "panelATM";
            panelATM.Size = new Size(786, 416);
            panelATM.TabIndex = 0;
            panelATM.Click += NumberPad_Click;
            // 
            // lblAmount
            // 
            lblAmount.BackColor = Color.Green;
            lblAmount.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmount.Location = new Point(379, 39);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(383, 37);
            lblAmount.TabIndex = 26;
            lblAmount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnDeposit
            // 
            btnDeposit.BackColor = Color.SlateGray;
            btnDeposit.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeposit.Location = new Point(379, 253);
            btnDeposit.Name = "btnDeposit";
            btnDeposit.Size = new Size(188, 35);
            btnDeposit.TabIndex = 25;
            btnDeposit.Text = "Deposit";
            btnDeposit.UseVisualStyleBackColor = false;
            btnDeposit.Click += btnDeposit_Click;
            // 
            // btnLogOut
            // 
            btnLogOut.BackColor = Color.Green;
            btnLogOut.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogOut.Location = new Point(47, 312);
            btnLogOut.Name = "btnLogOut";
            btnLogOut.Size = new Size(246, 40);
            btnLogOut.TabIndex = 24;
            btnLogOut.Text = "Log Out";
            btnLogOut.UseVisualStyleBackColor = false;
            btnLogOut.Click += btnLogOut_Click;
            // 
            // tbxDescription
            // 
            tbxDescription.BackColor = SystemColors.InactiveCaption;
            tbxDescription.Location = new Point(379, 294);
            tbxDescription.Multiline = true;
            tbxDescription.Name = "tbxDescription";
            tbxDescription.PlaceholderText = "Enter Description";
            tbxDescription.Size = new Size(383, 58);
            tbxDescription.TabIndex = 23;
            // 
            // btnWithdraw
            // 
            btnWithdraw.BackColor = Color.SlateGray;
            btnWithdraw.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnWithdraw.Location = new Point(574, 253);
            btnWithdraw.Name = "btnWithdraw";
            btnWithdraw.Size = new Size(188, 35);
            btnWithdraw.TabIndex = 21;
            btnWithdraw.Text = "Withdraw";
            btnWithdraw.UseVisualStyleBackColor = false;
            btnWithdraw.Click += btnWithdraw_Click;
            // 
            // btnNine
            // 
            btnNine.BackColor = Color.SlateGray;
            btnNine.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNine.Location = new Point(574, 195);
            btnNine.Name = "btnNine";
            btnNine.Size = new Size(91, 52);
            btnNine.TabIndex = 19;
            btnNine.Text = "9";
            btnNine.UseVisualStyleBackColor = false;
            btnNine.Click += NumberPad_Click;
            // 
            // btnEight
            // 
            btnEight.BackColor = Color.SlateGray;
            btnEight.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEight.Location = new Point(476, 195);
            btnEight.Name = "btnEight";
            btnEight.Size = new Size(91, 52);
            btnEight.TabIndex = 18;
            btnEight.Text = "8";
            btnEight.UseVisualStyleBackColor = false;
            btnEight.Click += NumberPad_Click;
            // 
            // btnSeven
            // 
            btnSeven.BackColor = Color.SlateGray;
            btnSeven.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSeven.Location = new Point(379, 195);
            btnSeven.Name = "btnSeven";
            btnSeven.Size = new Size(91, 52);
            btnSeven.TabIndex = 17;
            btnSeven.Text = "7";
            btnSeven.UseVisualStyleBackColor = false;
            btnSeven.Click += NumberPad_Click;
            // 
            // btnZero
            // 
            btnZero.BackColor = Color.SlateGray;
            btnZero.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnZero.Location = new Point(671, 195);
            btnZero.Name = "btnZero";
            btnZero.Size = new Size(91, 52);
            btnZero.TabIndex = 16;
            btnZero.Text = "0";
            btnZero.UseVisualStyleBackColor = false;
            btnZero.Click += NumberPad_Click;
            // 
            // btnSix
            // 
            btnSix.BackColor = Color.SlateGray;
            btnSix.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSix.Location = new Point(574, 137);
            btnSix.Name = "btnSix";
            btnSix.Size = new Size(91, 52);
            btnSix.TabIndex = 15;
            btnSix.Text = "6";
            btnSix.UseVisualStyleBackColor = false;
            btnSix.Click += NumberPad_Click;
            // 
            // btnFive
            // 
            btnFive.BackColor = Color.SlateGray;
            btnFive.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFive.Location = new Point(476, 137);
            btnFive.Name = "btnFive";
            btnFive.Size = new Size(91, 52);
            btnFive.TabIndex = 14;
            btnFive.Text = "5";
            btnFive.UseVisualStyleBackColor = false;
            btnFive.Click += NumberPad_Click;
            // 
            // btnFour
            // 
            btnFour.BackColor = Color.SlateGray;
            btnFour.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFour.Location = new Point(379, 137);
            btnFour.Name = "btnFour";
            btnFour.Size = new Size(91, 52);
            btnFour.TabIndex = 13;
            btnFour.Text = "4";
            btnFour.UseVisualStyleBackColor = false;
            btnFour.Click += NumberPad_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.SlateGray;
            btnClear.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClear.Location = new Point(671, 79);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(91, 110);
            btnClear.TabIndex = 12;
            btnClear.Text = "C";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // btnThree
            // 
            btnThree.BackColor = Color.SlateGray;
            btnThree.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnThree.Location = new Point(574, 79);
            btnThree.Name = "btnThree";
            btnThree.Size = new Size(91, 52);
            btnThree.TabIndex = 11;
            btnThree.Text = "3";
            btnThree.UseVisualStyleBackColor = false;
            btnThree.Click += NumberPad_Click;
            // 
            // btnTwo
            // 
            btnTwo.BackColor = Color.SlateGray;
            btnTwo.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTwo.Location = new Point(476, 79);
            btnTwo.Name = "btnTwo";
            btnTwo.Size = new Size(91, 52);
            btnTwo.TabIndex = 10;
            btnTwo.Text = "2";
            btnTwo.UseVisualStyleBackColor = false;
            btnTwo.Click += NumberPad_Click;
            // 
            // btnOne
            // 
            btnOne.BackColor = Color.SlateGray;
            btnOne.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnOne.Location = new Point(379, 79);
            btnOne.Name = "btnOne";
            btnOne.Size = new Size(91, 52);
            btnOne.TabIndex = 9;
            btnOne.Text = "1";
            btnOne.UseVisualStyleBackColor = false;
            btnOne.Click += NumberPad_Click;
            // 
            // btnStatement
            // 
            btnStatement.BackColor = Color.SlateGray;
            btnStatement.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStatement.Location = new Point(47, 258);
            btnStatement.Name = "btnStatement";
            btnStatement.Size = new Size(246, 40);
            btnStatement.TabIndex = 7;
            btnStatement.Text = "Statement";
            btnStatement.UseVisualStyleBackColor = false;
            btnStatement.Click += btnStatement_Click;
            // 
            // lblBalance
            // 
            lblBalance.AutoSize = true;
            lblBalance.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalance.ForeColor = SystemColors.ControlText;
            lblBalance.Location = new Point(118, 207);
            lblBalance.Name = "lblBalance";
            lblBalance.Size = new Size(0, 25);
            lblBalance.TabIndex = 6;
            // 
            // lblAccountNumber
            // 
            lblAccountNumber.AutoSize = true;
            lblAccountNumber.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAccountNumber.ForeColor = SystemColors.ControlText;
            lblAccountNumber.Location = new Point(115, 148);
            lblAccountNumber.Name = "lblAccountNumber";
            lblAccountNumber.Size = new Size(0, 25);
            lblAccountNumber.TabIndex = 5;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblName.ForeColor = SystemColors.ControlText;
            lblName.Location = new Point(115, 70);
            lblName.Name = "lblName";
            lblName.Size = new Size(0, 25);
            lblName.TabIndex = 4;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ControlText;
            label8.Location = new Point(33, 182);
            label8.Name = "label8";
            label8.Size = new Size(88, 25);
            label8.TabIndex = 3;
            label8.Text = "Balance :";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ControlText;
            label7.Location = new Point(33, 106);
            label7.Name = "label7";
            label7.Size = new Size(169, 25);
            label7.TabIndex = 2;
            label7.Text = "Account Number :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ControlText;
            label6.Location = new Point(33, 39);
            label6.Name = "label6";
            label6.Size = new Size(74, 25);
            label6.TabIndex = 1;
            label6.Text = "Name :";
            // 
            // tabTransactionHistory
            // 
            tabTransactionHistory.Controls.Add(lbStatement);
            tabTransactionHistory.Location = new Point(4, 24);
            tabTransactionHistory.Name = "tabTransactionHistory";
            tabTransactionHistory.Padding = new Padding(3);
            tabTransactionHistory.Size = new Size(792, 422);
            tabTransactionHistory.TabIndex = 3;
            tabTransactionHistory.Text = "Transaction History";
            tabTransactionHistory.UseVisualStyleBackColor = true;
            // 
            // lbStatement
            // 
            lbStatement.Dock = DockStyle.Fill;
            lbStatement.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbStatement.FormattingEnabled = true;
            lbStatement.HorizontalScrollbar = true;
            lbStatement.Location = new Point(3, 3);
            lbStatement.Name = "lbStatement";
            lbStatement.Size = new Size(786, 416);
            lbStatement.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabLogin.ResumeLayout(false);
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            tabRegister.ResumeLayout(false);
            panelRegister.ResumeLayout(false);
            panelRegister.PerformLayout();
            tabATM.ResumeLayout(false);
            panelATM.ResumeLayout(false);
            panelATM.PerformLayout();
            tabTransactionHistory.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabLogin;
        private TabPage tabRegister;
        private TabPage tabATM;
        private TabPage tabTransactionHistory;
        private Panel panelLogin;
        private Panel panelRegister;
        private Panel panelATM;
        private Label label2;
        private Label label1;
        private TextBox tbxPassword;
        private TextBox tbxUserName;
        private TextBox tbxPassword_;
        private TextBox tbxUserName_;
        private TextBox tbxFullName;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label lblBalance;
        private Label lblAccountNumber;
        private Label lblName;
        private Label label8;
        private Label label7;
        private Label label6;
        private Button tbxRegister;
        private Button btnLogin;
        private Button tbxRegister_;
        private Button btnStatement;
        private Button btnOne;
        private Button btnClear;
        private Button btnThree;
        private Button btnTwo;
        private TextBox tbxDescription;
        private Button btnWithdraw;
        private Button btnNine;
        private Button btnEight;
        private Button btnSeven;
        private Button btnZero;
        private Button btnSix;
        private Button btnFive;
        private Button btnFour;
        private Button btnLogOut;
        private Button btnDeposit;
        private Label lblAmount;
        private ListBox lbStatement;
    }
}
