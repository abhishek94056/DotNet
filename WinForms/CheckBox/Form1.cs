namespace CheckBox
{
    public partial class Form1 : Form
    {
        string toppings = "";
        public Form1()
        {
            InitializeComponent();
            cbChees.Checked = true; 
        }

        private void PzzaSelection_CheckChange(object sender, EventArgs e)
        {
            toppings = "";
            if (cbChees.Checked)
            {
                toppings += cbChees.Text + ", ";
            }
            if (cbMushroom.Checked)
            {
                toppings += cbMushroom.Text + ", ";
            }
            if (cbCake.Checked)
            {
                toppings += cbCake.Text + ", ";
            }

            lblPzzaSelection.Text = toppings != ""
                ? toppings : "You did not select an topping.";
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if(toppings != "")
            {
                MessageBox.Show("You are order summery!\n" +
                    "You have the following toppings\n" +
                    toppings);
            }
            else
            {
                lblPzzaSelection.Text = "You did not select an topping.";
            }
        }
    }
}
