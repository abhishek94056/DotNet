namespace Calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            double number1 = 0;
            double number2 = 0;

            bool ok1 = double.TryParse(tbxNumber1.Text, out number1);
            bool ok2 = double.TryParse(tbxNumber2.Text, out number2);

            if (!ok1 || !ok2)
            {
                MessageBox.Show("Incorrect Number Provided");
                return;
            }

            string ope = tbxOperator.Text;

            if (string.IsNullOrEmpty(ope))
            {
                MessageBox.Show("Please enter operator");
                return;
            }

            switch (ope)
            {
                case "+":
                    lblResult.Text = (number1 + number2).ToString();
                    break;

                case "-":
                    lblResult.Text = (number1 - number2).ToString();
                    break;

                case "*":
                    lblResult.Text = (number1 * number2).ToString();
                    break;

                case "/":
                    if (number2 != 0)
                        lblResult.Text = (number1 / number2).ToString();
                    else
                        MessageBox.Show("Division by zero is not allowed");
                    break;

                default:
                    lblResult.Text = "Unknown Operator";
                    break;
            }
        }
    }
}