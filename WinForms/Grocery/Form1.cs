namespace Grocery
{
    public partial class Form1 : Form
    {
        String[] units = { "cm", "ft", "g", "gallan", "inch", "kg", "lb", "lit", "m", "oz", "piece" };
        List<String> ShopingList = new List<String>();
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult userResponse = MessageBox.Show("Do You Want To Exist The Application?", "Warning", MessageBoxButtons
                .YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (userResponse == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void lblError_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboUnits.Items.AddRange(units);
            comboUnits.SelectedIndex = 5;


            //bind the datasource

            lbxShoppingList.DataSource = ShopingList;
        }
        private void UpdateGUI()
        {
            lbxShoppingList.DataSource = null;
            lbxShoppingList.DataSource = ShopingList;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string item = tbxItem.Text;
            String unit = comboUnits.SelectedItem.ToString();

            double amount = 0;

            bool amountOk = double.TryParse(tbxAmount.Text, out amount);
            if (!string.IsNullOrEmpty(item) && !string.IsNullOrEmpty(unit) && amountOk)
            {
                string row = $"{item}\t\t{amount}{unit}";
                ShopingList.Add(row);
                UpdateGUI();
            }
        }

        private void btmRemove_Click(object sender, EventArgs e)
        {
            int index = lbxShoppingList.SelectedIndex;

            if (index != -1)
            {
                DialogResult DeleteConfirmation = MessageBox.Show("Do You Want To Remove This Item?", "Warning", MessageBoxButtons
               .YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (DeleteConfirmation == DialogResult.Yes)
                {
                    ShopingList.RemoveAt(index);
                    UpdateGUI();
                }
            }
            else
            {
                lblError.Text = "You must select an item to delete it!";
                lblError.Visible = true;
            }
        }

        private void lbxShoppingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblError.Visible = false;
        }
    }
}