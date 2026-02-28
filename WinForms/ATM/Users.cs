namespace ATM
{
    internal class Users
    {
        public double Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public Users(int _id, string _name, string _username, string _password, double _balance) 
        {
            Id = _id;
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
            if (_amount <= Balance)
            {
                Balance -= _amount;
                return true;
            }
            return false;
        }
    }
}
