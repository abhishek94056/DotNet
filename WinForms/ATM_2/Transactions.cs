using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ATM_2
{
    internal class Transactions
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; } 
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        public Transactions(int _Id, int _AccountNumber, string _Type, decimal _Amount, DateTime _TransactionDate, string _Description)
        {
            Id = _Id;
            AccountNumber = _AccountNumber;
            Type = _Type;
            Amount = _Amount;
            TransactionDate = _TransactionDate;
            Description = _Description;
        }
    }
}
