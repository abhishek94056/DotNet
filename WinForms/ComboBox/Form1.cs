namespace ComboBox
{
    public partial class Form1 : Form
    {
        string[] vheicle = { "car", "Bike", "Bus", "Train" };
        public Form1()
        {
            InitializeComponent();
            comboVechicle.Items.AddRange(vheicle);
            comboVechicle.SelectedIndex = 0;               //default
        }

        private void tbxregNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPark_Click(object sender, EventArgs e)
        {
            string regNumber = tbxregNumber.Text;
            bool regNumberOk = !string.IsNullOrEmpty(regNumber);
            if (comboVechicle.SelectedItem != null && regNumberOk)
            {
                string type = comboVechicle.SelectedItem.ToString();
                lblParkingStatus.Text = regNumber + "(type)" + type;
            }
        }
    }
}
