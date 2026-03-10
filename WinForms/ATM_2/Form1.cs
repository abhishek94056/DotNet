using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ATM_2
{
    public partial class Form1 : Form
    {
        Users whoIsLogin = null;
        string amount = null;
        readonly String connectionstring = ConfigurationManager.ConnectionStrings["ATMConnection"].ConnectionString;
        public Form1()
        {
            InitializeComponent();
            panelATM.Enabled = false;

        }

        private bool isValidUserName(String UserName)
        {
            return Regex.IsMatch(UserName, @"^[a-zA-Z0-9]{4,15}$");
        }

        private bool isValidPassword(String Password)
        {
            return Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$");
        }

        private string HashPassword(String Password, String Salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = Password + Salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private String GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);

            }
            return Convert.ToBase64String(salt);
        }


        private void tbxRegister__Click(object sender, EventArgs e)
        {

            String enteredFullName = tbxFullName.Text;
            String enteredUsername = tbxUserName_.Text;
            String enteredPassword = tbxPassword_.Text;

            if (!isValidUserName(enteredUsername))
            {
                MessageBox.Show(
                    "Username must contain: \n" +
                    "- Be 4 to 15 characters long \n" +
                    "- Contain only letters and numbers \n" +
                    "- No spaces or special characters",
                    "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!isValidPassword(enteredPassword))
            {
                MessageBox.Show("Password must contain:\n" +
                    "- 8+ characters \n" +
                    "- 1 uppercase\n" +
                    "- 1 lowercase\n" +
                    "- 1 number\n" +
                    "- 1 special character",
                    "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            String salt = GenerateSalt();
            String hashPassword = HashPassword(enteredPassword, salt);
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_Register", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", enteredFullName);
                cmd.Parameters.AddWithValue("@UserName", enteredUsername);
                cmd.Parameters.AddWithValue("@Password", hashPassword);
                cmd.Parameters.AddWithValue("@salt", salt);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Registration Successfully..!");

                    tbxFullName.Clear();
                    tbxUserName_.Clear();
                    tbxPassword_.Clear();
                    tabControl1.SelectedTab = tabLogin;
                }
                catch (Exception ex)
                {
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String enteredUserName = tbxUserName.Text.Trim();
            String enteredPassword = tbxPassword.Text.Trim();
            if (String.IsNullOrEmpty(enteredUserName) || String.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please entered Username And Password");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_LoginUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", enteredUserName);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    String storedHash = reader["Password"].ToString();
                    String storedSalt = reader["Salt"].ToString();
                    String EnteredHash = HashPassword(enteredPassword, storedSalt);
                    if (EnteredHash == storedHash)
                    {
                        whoIsLogin = new Users(
                            Convert.ToInt32(reader["AccountNumber"]),
                            reader["Name"].ToString(),
                            reader["UserName"].ToString(),
                            reader["Password"].ToString(),
                            Convert.ToDecimal(reader["Balance"])
                            );
                        reader.Close();

                        lblName.Text = whoIsLogin.Name;
                        lblAccountNumber.Text = whoIsLogin.AccountNumber.ToString();
                        lblBalance.Text = whoIsLogin.Balance.ToString("0.00");
                        panelATM.Enabled = true;

                        MessageBox.Show("Login Successfully");
                        tabControl1.SelectedTab = tabATM;
                        LoadStatement();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Password");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid UserName");
                }
            }
        }

        private void tbxRegister_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabRegister;
        }

        private void NumberPad_Click(object sender, EventArgs e)
        {
            Button clickButton = sender as Button;
            amount += clickButton.Text;
            lblAmount.Text = amount;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            amount = "";
            lblAmount.Text = "0";
        }

        private void UpdateBalance()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_UpdateBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Balance", whoIsLogin.Balance);
                cmd.Parameters.AddWithValue("@AccountNumber", whoIsLogin.AccountNumber);
                con.Open();
                cmd.ExecuteNonQuery();

            }
        }

        private void addTransactions(String type, decimal amount, string description)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_AddTransactions", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AccountNumber", whoIsLogin.AccountNumber);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", description);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(amount)) return;

            decimal depositAmount = decimal.Parse(amount);
            whoIsLogin.Deposit(depositAmount);
            UpdateBalance();
            String Description = tbxDescription.Text.Trim();
            if (String.IsNullOrWhiteSpace(Description))
            {
                Description = "Via ATM";
            }
            addTransactions("Deposit", depositAmount, Description);
            lblBalance.Text = whoIsLogin.Balance.ToString("0.00");
            lblAmount.Text = "0";
            amount = "";
            LoadStatement();
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(amount)) return;
            decimal withdrawAmount = decimal.Parse(amount);
            bool ok = whoIsLogin.Withdraw(withdrawAmount);
            if (!ok)
            {
                MessageBox.Show("Insufficient Amount", "Withdraw error");
                return;
            }
            UpdateBalance();
            String Description = tbxDescription.Text;
            if (String.IsNullOrEmpty(Description))
            {
                Description = "Vie ATM";
            }

            addTransactions("Withdraw", withdrawAmount, Description);
            lblBalance.Text = whoIsLogin.Balance.ToString("0.00");
            lblAmount.Text = "0";
            amount = "";
            LoadStatement();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            lblName.Text = "";
            lblAccountNumber.Text = "";
            lblBalance.Text = "";
            lblAmount.Text = "0";
            whoIsLogin = null;
            tabControl1.SelectedTab = tabLogin;
            panelATM.Enabled = false;
        }

        private void btnStatement_Click(object sender, EventArgs e)
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
            var redFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.RED);

            Paragraph title = new Paragraph("ATM BANK STATEMENT", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Account Holder : " + whoIsLogin.Name, normalFont));
            doc.Add(new Paragraph("Account Number : " + whoIsLogin.AccountNumber, normalFont));
            doc.Add(new Paragraph("Statement Date : " + DateTime.Now.ToString("dd-MM-yyyy"), normalFont));
            doc.Add(new Paragraph("Current Balance : ₹ " + whoIsLogin.Balance.ToString("0.00"), normalFont));
            doc.Add(new Paragraph("\n"));

            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 5f, 20f, 25f, 10f, 20f, 20f });
            

            table.AddCell(new PdfPCell(new Phrase("Id", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Date", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Description", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Type", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Amount", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Balance", headerFont)));

            decimal runningBalance = whoIsLogin.Balance;

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_Statement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AccountNumber", whoIsLogin.AccountNumber);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int serialNo = 1;
                while (reader.Read())
                {
                    //table.AddCell(new Phrase(reader["Id"].ToString(), normalFont));
                    table.AddCell(new Phrase(serialNo.ToString(), normalFont));
                    serialNo++;

                    string type = reader["Type"].ToString();
                    decimal amt = Convert.ToDecimal(reader["Amount"]);

                    table.AddCell(new Phrase(
                        Convert.ToDateTime(reader["TransactionDate"])
                        .ToString("dd-MM-yyyy HH:mm"), normalFont));

                    table.AddCell(new Phrase(reader["Description"].ToString(), normalFont));
                    //table.AddCell(new Phrase(type == "Deposit" ? "CR" : "DR", normalFont));


                    if (type == "Deposit")
                    {
                        table.AddCell(new Phrase("CR", normalFont));
                    }
                    else
                    {
                        table.AddCell(new Phrase("DR", redFont)); 
                    }

                    if (type == "Deposit")
                    {
                        table.AddCell(new Phrase("  ₹ " + amt.ToString("0.00"), normalFont));
                    }
                    else
                    {
                        table.AddCell(new Phrase("- ₹ " + amt.ToString("0.00"), redFont)); 
                    }
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

        private void LoadStatement()
        {
            lbStatement.Items.Clear();

            lbStatement.Items.Add(
            $"{"ID",-4} | {"DATE",-20} | {"DESCRIPTION",-18} | {"TYP",-3} | {"AMOUNT",12} | {"BALANCE",12}");

            lbStatement.Items.Add(new string('-', 90));

            decimal runningBalance = whoIsLogin.Balance;

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("SP_Statement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AccountNumber", whoIsLogin.AccountNumber);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    string type = reader["Type"].ToString();
                    decimal amount = Convert.ToDecimal(reader["Amount"]);
                    string description = reader["Description"].ToString();
                    DateTime date = Convert.ToDateTime(reader["TransactionDate"]);

                    string crdr = type == "Deposit" ? "CR" : "DR";

                    string formattedAmount = type == "Deposit"
                        ? $"+₹{amount:0.00}"
                        : $"-₹{amount:0.00}";

                    string line =
                        $"{id,-4} | {date:dd-MM-yyyy HH:mm} | {description,-18} | {crdr,-3} | {formattedAmount,12} | {runningBalance,12:0.00}";

                    lbStatement.Items.Add(line);
                    if (type == "Deposit")
                        runningBalance -= amount;
                    else
                        runningBalance += amount;
                }

                reader.Close();
            }
        }


    }
}
