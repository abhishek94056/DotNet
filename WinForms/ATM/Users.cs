using System;
using System.Collections.Generic;
using System.Text;

namespace ATM
{
    internal class Users
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public Users(string _name, string _username, string _password, int _balance) 
        { 
            Name = _name;
            UserName = _username;
            Password = _password;
            Balance = _balance;
        }

        public void deposit(double _amount)
        {
            Balance += _amount;
        }

        public bool Withdrow(double _amount)
        {
            if(_amount <= Balance)
            {
                Balance -= _amount;
                return true;
            }
            return false;
        }
    }
}
