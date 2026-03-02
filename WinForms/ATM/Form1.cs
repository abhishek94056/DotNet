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
            panel2.Enabled = false;
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
                    panel2.Enabled = true;

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

        private void AddTransaction(string type, double amount, string description)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Transactions 
                         (UserName, Type, Amount, Description) 
                         VALUES 
                         (@username, @type, @amount, @description)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", description);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //private void LoadTransactions()
        //{
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT Type, Amount, Description, TransactionDate FROM Transactions WHERE UserName=@username ORDER BY TransactionDate DESC";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        dgvTransactions.DataSource = dt;
        //        dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //        dgvTransactions.ReadOnly = true;
        //        dgvTransactions.AllowUserToAddRows = false;
        //    }
        //}

        private void LoadTransactions()
        {
            lstTransactions.Items.Clear();

           
            lstTransactions.Items.Add(
                $"{"Date",-31} | {"Description",-18} | {"Type",-4} | {"Amount",12} | {"Balance",12}");

            lstTransactions.Items.Add(
                new string('-', 75));  

            double runningBalance = Convert.ToDouble(wholsLogIn.Balance);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Id, Type, Amount, Description, TransactionDate
                         FROM Transactions
                         WHERE UserName = @username
                         ORDER BY TransactionDate DESC, Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();
                    double amount = Convert.ToDouble(reader["Amount"]);
                    DateTime date = Convert.ToDateTime(reader["TransactionDate"]);
                    string description = reader["Description"]?.ToString() ?? "";

                    string crdr = type == "Deposit" ? "CR" : "DR";

                    string formattedAmount = type == "Deposit"
                        ? $"+ ₹ {amount:0.00}"
                        : $"- ₹ {amount:0.00}";

                    string formattedLine =
                        $"{date:dd-MM-yyyy HH:mm,-18} | {description,-15} | {crdr,-4} | {formattedAmount,12} | ₹ {runningBalance,10:0.00}";

                    lstTransactions.Items.Add(formattedLine);

                    if (type == "Deposit")
                        runningBalance -= amount;
                    else
                        runningBalance += amount;
                }

                reader.Close();
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(amount)) return;

            double depositAmount = double.Parse(amount);

            wholsLogIn.deposit(depositAmount);

            UpdateBalanceInDatabase();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                description = " via ATM";
            }

            AddTransaction("Deposit", depositAmount, description);
            LoadTransactions();

            lblBalance.Text = wholsLogIn.Balance.ToString("0.00");
            lblAmount.Text = "0";
            amount = "";
        }

        private void btnWithdraw_Click_1(object sender, EventArgs e)
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
            string description = txtDescription.Text;

            if (string.IsNullOrWhiteSpace(description))
            {
                description = " via ATM";
            }

            AddTransaction("Withdraw", withdrawAmount, description);
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
            panel2.Enabled = false;
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

        private void btnDownloadStatement_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF files (*.pdf)|*.pdf";
            sfd.FileName = "BankStatement_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
            doc.Open();

            // Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // ===== TITLE =====
            Paragraph title = new Paragraph("ATM BANK STATEMENT", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);
            doc.Add(new Paragraph("\n"));

            // ===== ACCOUNT INFO =====
            doc.Add(new Paragraph("Account Holder : " + wholsLogIn.Name, normalFont));
            doc.Add(new Paragraph("Account Number : " + wholsLogIn.Id, normalFont));
            doc.Add(new Paragraph("Statement Date : " + DateTime.Now.ToString("dd-MM-yyyy"), normalFont));
            doc.Add(new Paragraph("Current Balance : ₹ " + wholsLogIn.Balance.ToString("0.00"), normalFont));
            doc.Add(new Paragraph("\n"));

            // ===== TABLE =====
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 20f, 30f, 10f, 20f, 20f });

            // Header Row
            table.AddCell(new PdfPCell(new Phrase("Date", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Description", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Type", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Amount", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Balance", headerFont)));

            double runningBalance = wholsLogIn.Balance;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Id, Type, Amount, Description, TransactionDate
                     FROM Transactions
                     WHERE UserName = @username
                     ORDER BY TransactionDate DESC, Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", wholsLogIn.UserName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();
                    double amount = Convert.ToDouble(reader["Amount"]);

                    // Print row
                    table.AddCell(new Phrase(
                        Convert.ToDateTime(reader["TransactionDate"])
                        .ToString("dd-MM-yyyy HH:mm"), normalFont));

                    table.AddCell(new Phrase(reader["Description"].ToString(), normalFont));
                    table.AddCell(new Phrase(type == "Deposit" ? "CR" : "DR", normalFont));

                    // Show + or - sign
                    string formattedAmount = type == "Deposit"
                        ? "+ ₹ " + amount.ToString("0.00")
                        : "- ₹ " + amount.ToString("0.00");

                    table.AddCell(new Phrase(formattedAmount, normalFont));
                    table.AddCell(new Phrase("₹ " + runningBalance.ToString("0.00"), normalFont));

                    // Reverse balance logic (because DESC)
                    if (type == "Deposit")
                        runningBalance -= amount;
                    else
                        runningBalance += amount;
                }

                reader.Close();
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n\nThis is a computer generated statement.", normalFont));
            doc.Close();

            MessageBox.Show("Statement Downloaded Successfully!");
        }
    }
}