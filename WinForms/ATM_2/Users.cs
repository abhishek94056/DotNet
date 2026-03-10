using System;
using System.Collections.Generic;
using System.Text;

namespace ATM_2
{
    internal class Users
    {
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }

        public Users(int _AccountNumber, string _Name, string _UserName, string _Password, decimal _Balance)
        {
            AccountNumber = _AccountNumber;
            Name = _Name;
            UserName = _UserName;
            Password = _Password;
            Balance = _Balance;
        }

        public void Deposit(decimal Amount)
        {
            Balance += Amount;
        }

        public bool Withdraw(decimal Amount)
        {
            if (Balance >= Amount)
            {
                Balance -= Amount;
                return true;
            }
            return false;
        }
    }
}
