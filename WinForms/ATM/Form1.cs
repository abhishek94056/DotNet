using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ATM
{
    public partial class Form1 : Form
    {
        Users wholsLogIn = null;
        string amount = "";

        string connectionString = ConfigurationManager
            .ConnectionStrings["ATMConnection"]
            .ConnectionString;

        public Form1()
        {
            InitializeComponent();
            panel1.Enabled = false;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string enteredUsername = tbxUsername.Text.Trim();
            string enteredPassword = HashPassword(tbxPassword.Text.Trim());

            if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please enter Username and Password");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users WHERE UserName=@username AND Password=@password";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", enteredUsername);
                cmd.Parameters.AddWithValue("@password", enteredPassword);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    wholsLogIn = new Users(
                        Convert.ToInt32(reader["Id"]),
                        reader["Name"].ToString(),
                        reader["UserName"].ToString(),
                        reader["Password"].ToString(),
                        Convert.ToDouble(reader["Balance"])
                    );

                    reader.Close();

                    tabRegister.SelectedTab = tabATM;

                    lblUserName.Text = wholsLogIn.Name;
                    lblAN.Text = wholsLogIn.Id.ToString();
                    lblBalance.Text = wholsLogIn.Balance.ToString("0.00");

                    panel1.Enabled = true;

                    LoadTransactions();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password");
                }
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())    //MD5, SHA-1, SHA-256
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
        private void NumberPad_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            amount += clickedButton.Text;
            lblAmount.Text = amount;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            amount = "";
            lblAmount.Text = "0";
        }

        private void UpdateBalanceInDatabase()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET Balance=@balance WHERE UserName=@username";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@balance", wholsLogIn.Balance);
                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddTransaction(string type, double amountValue)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Transactions (UserName, Type, Amount) VALUES (@username, @type, @amount)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@amount", amountValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void LoadTransactions()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Type, Amount, TransactionDate FROM Transactions WHERE UserName=@username ORDER BY TransactionDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvTransactions.DataSource = dt;
                dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvTransactions.ReadOnly = true;
                dgvTransactions.AllowUserToAddRows = false;
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(amount)) return;

            double depositAmount = double.Parse(amount);

            wholsLogIn.deposit(depositAmount);

            UpdateBalanceInDatabase();
            AddTransaction("Deposit", depositAmount);
            LoadTransactions();

            lblBalance.Text = wholsLogIn.Balance.ToString("0.00");
            lblAmount.Text = "0";
            amount = "";
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(amount)) return;

            double withdrawAmount = double.Parse(amount);

            bool ok = wholsLogIn.Withdrow(withdrawAmount);

            if (!ok)
            {
                MessageBox.Show("Insufficient amount", "Withdraw Error");
                return;
            }

            UpdateBalanceInDatabase();
            AddTransaction("Withdraw", withdrawAmount);
            LoadTransactions();

            lblBalance.Text = wholsLogIn.Balance.ToString("0.00");
            lblAmount.Text = "0";
            amount = "";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            lblUserName.Text = "";
            lblAN.Text = "";
            lblAmount.Text = "0";
            lblBalance.Text = "0";

            amount = "";
            wholsLogIn = null;

            tabRegister.SelectedTab = tabLogIn;
            panel1.Enabled = false;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string name = tbxRegName.Text;
            string username = tbxRegUsername.Text;
            string password = HashPassword(tbxRegPassword.Text);

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Name, UserName, Password, Balance) " +
                               "VALUES (@name, @username, @password, 0)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registration Successful!");

                    // Clear fields
                    tbxRegName.Clear();
                    tbxRegUsername.Clear();
                    tbxRegPassword.Clear();

                    tabRegister.SelectedTab = tabLogIn;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }



        private void btnGoRegister_Click_1(object sender, EventArgs e)
        {
            tabRegister.SelectedTab = tabPage2;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDownloadStatement_Click(object sender, EventArgs e)
        {
            
            if (wholsLogIn == null)
            {
                MessageBox.Show("Please login first.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF files (*.pdf)|*.pdf";
            sfd.FileName = "BankStatement_" + wholsLogIn.Id + ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();

                doc.Add(new Paragraph("BANK STATEMENT\n\n"));

                doc.Add(new Paragraph("Account Holder: " + wholsLogIn.Name));
                doc.Add(new Paragraph("Account Number: " + wholsLogIn.Id));
                doc.Add(new Paragraph("Current Balance: ₹ " + wholsLogIn.Balance.ToString("0.00")));
                doc.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;

                table.AddCell("Date");
                table.AddCell("Type");
                table.AddCell("Amount");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Type, Amount, TransactionDate FROM Transactions WHERE UserName=@username ORDER BY TransactionDate";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        table.AddCell(Convert.ToDateTime(reader["TransactionDate"]).ToString("dd-MM-yyyy HH:mm"));
                        table.AddCell(reader["Type"].ToString());
                        table.AddCell(Convert.ToDecimal(reader["Amount"]).ToString("0.00"));
                    }

                    reader.Close();
                }

                doc.Add(table);
                doc.Close();

                MessageBox.Show("Statement Downloaded Successfully!");
            }
        }
   
    }
}