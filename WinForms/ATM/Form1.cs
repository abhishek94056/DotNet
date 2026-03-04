using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

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
            string enteredPassword = tbxPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredUsername) ||
                string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Please enter Username and Password");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_LoginUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", enteredUsername);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedHash = reader["Password"].ToString();
                    string storedSalt = reader["Salt"].ToString();

                    // Hash entered password with stored salt
                    string enteredHash = HashPassword(enteredPassword, storedSalt);

                    if (enteredHash == storedHash)
                    {
                        // Login Success
                        wholsLogIn = new Users(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            reader["UserName"].ToString(),
                            storedHash,
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

                        MessageBox.Show("Login Successful");
                    }
                    else
                    {
                        MessageBox.Show("Invalid Password Or Password");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username Or Password");
                }
            }
        }
        
        private bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9]{4,15}$");
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string name = tbxRegName.Text;
            string username = tbxRegUsername.Text;
            string password = tbxRegPassword.Text.Trim();

            if (!IsValidUsername(username))
            {
                MessageBox.Show(
                    "Username must:\n" +
                    "- Be 4 to 15 characters long\n" +
                    "- Contain only letters and numbers\n" +
                    "- No spaces or special characters",
                    "Invalid Username",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tbxRegUsername.Focus();
                return;
            }

            if (!IsValidPassword(password))
            {
                MessageBox.Show("Password must contain:\n- 8+ characters\n- 1 uppercase\n- 1 lowercase\n- 1 number\n- 1 special character");
                return;
            }

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_RegisterUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Salt", salt);  // store salt

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
                catch (Exception ex)
                {
                    {
                        MessageBox.Show(ex.Message);
                    }
                }



            }
        }
        private void btnGoRegister_Click_1(object sender, EventArgs e)
        {
            tabRegister.SelectedTab = tabPage2;
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

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            Paragraph title = new Paragraph("ATM BANK STATEMENT", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Account Holder : " + wholsLogIn.Name, normalFont));
            doc.Add(new Paragraph("Account Number : " + wholsLogIn.Id, normalFont));
            doc.Add(new Paragraph("Statement Date : " + DateTime.Now.ToString("dd-MM-yyyy"), normalFont));
            doc.Add(new Paragraph("Current Balance : ₹ " + wholsLogIn.Balance.ToString("0.00"), normalFont));
            doc.Add(new Paragraph("\n"));

            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 20f, 30f, 10f, 20f, 20f });

            table.AddCell(new PdfPCell(new Phrase("Date", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Description", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Type", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Amount", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Balance", headerFont)));

            double runningBalance = wholsLogIn.Balance;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string query = @"SELECT Id, Type, Amount, Description, TransactionDate
                //     FROM Transactions
                //     WHERE UserName = @username
                //     ORDER BY TransactionDate DESC, Id DESC";

                SqlCommand cmd = new SqlCommand("SP_GetStatement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", wholsLogIn.UserName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();
                    double amt = Convert.ToDouble(reader["Amount"]);

                    table.AddCell(new Phrase(
                        Convert.ToDateTime(reader["TransactionDate"])
                        .ToString("dd-MM-yyyy HH:mm"), normalFont));

                    table.AddCell(new Phrase(reader["Description"].ToString(), normalFont));
                    table.AddCell(new Phrase(type == "Deposit" ? "CR" : "DR", normalFont));

                    string formattedAmount = type == "Deposit"
                        ? "+ ₹ " + amt.ToString("0.00")
                        : "- ₹ " + amt.ToString("0.00");

                    table.AddCell(new Phrase(formattedAmount, normalFont));
                    table.AddCell(new Phrase("₹ " + runningBalance.ToString("0.00"), normalFont));

                    if (type == "Deposit")
                        runningBalance -= amt;
                    else
                        runningBalance += amt;
                }

                reader.Close();
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n\nThis is a computer generated statement.", normalFont));
            doc.Close();

            MessageBox.Show("Statement Downloaded Successfully!");
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

        private void UpdateBalanceInDatabase()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string query = "UPDATE Users SET Balance=@balance WHERE UserName=@username";

                SqlCommand cmd = new SqlCommand("SP_UpdateBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Balance", wholsLogIn.Balance);
                cmd.Parameters.AddWithValue("@UserName", wholsLogIn.UserName);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddTransaction(string type, double amount, string description)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string query = @"INSERT INTO Transactions 
                //         (UserName, Type, Amount, Description) 
                //         VALUES 
                //         (@username, @type, @amount, @description)";

                SqlCommand cmd = new SqlCommand("SP_AddTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", wholsLogIn.UserName);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Description", description);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

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
                //string query = @"SELECT Id, Type, Amount, Description, TransactionDate
                //         FROM Transactions
                //         WHERE UserName = @username
                //         ORDER BY TransactionDate DESC, Id DESC";

                SqlCommand cmd = new SqlCommand("SP_GetStatement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", wholsLogIn.UserName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();
                    double amt = Convert.ToDouble(reader["Amount"]);
                    DateTime date = Convert.ToDateTime(reader["TransactionDate"]);
                    string description = reader["Description"]?.ToString() ?? "";

                    string crdr = type == "Deposit" ? "CR" : "DR";

                    string formattedAmount = type == "Deposit"
                        ? $"+ ₹ {amt:0.00}"
                        : $"- ₹ {amt:0.00}";

                    string formattedLine =
                        $"{date:dd-MM-yyyy HH:mm,-18} | {description,-15} | {crdr,-4} | {formattedAmount,12} | ₹ {runningBalance,10:0.00}";

                    lstTransactions.Items.Add(formattedLine);

                    if (type == "Deposit")
                        runningBalance -= amt;
                    else
                        runningBalance += amt;
                }

                reader.Close();
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
    }
}