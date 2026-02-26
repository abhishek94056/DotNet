namespace ATM
{
    public partial class Form1 : Form
    {
        List<Users> atmUsers = new List<Users>();

        Users wholsLogIn = null;
        string amount = "";
        public Form1()
        {
            InitializeComponent();
            atmUsers.Add(new Users("Abhishek More", "abhi123", "Pass@123", 1000));
            panel1.Enabled = false;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            bool userFound = false;
            string enteredUsername = tbxUsername.Text;
            string enteredPassword = tbxPassword.Text;
            if (!string.IsNullOrEmpty(enteredUsername) && !string.IsNullOrEmpty(enteredPassword))
            {
                foreach (Users user in atmUsers)
                {
                    if (enteredUsername == user.UserName && enteredPassword == user.Password)
                    {
                        userFound = true;
                        wholsLogIn = user;
                        break;
                    }
                }
            }

            if (userFound)
            {
                tabControl1.SelectedTab = tabATM;
                lblUserName.Text = wholsLogIn.Name;
                lblBalance.Text = wholsLogIn.Balance.ToString();
                panel1.Enabled = true;
            }
        }

        private void NumberPad_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            amount += clickedButton.Text;
            lblAmount.Text = amount.ToString();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            amount = "";
            lblAmount.Text = "0";
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            wholsLogIn.deposit(double.Parse(amount));
            lblBalance.Text = wholsLogIn.Balance.ToString();
            lblAmount.Text = "0";
            amount = "";
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            bool ok = wholsLogIn.Withdrow(double.Parse(amount));
            lblBalance.Text = wholsLogIn.Balance.ToString();
            lblAmount.Text = "0";
            amount = "";
            if (!ok)
            {
                MessageBox.Show("Insufficient amount", "withdraw Error");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            //Reset GUI

            lblUserName.Text = "";
            lblAmount.Text = "0";
            lblBalance.Text = "0";

            //Reset Variable

            amount = "";
            wholsLogIn = null;

            tabControl1.SelectedTab = tabLogIn;
            panel1.Enabled = false;
        }
    }
}
