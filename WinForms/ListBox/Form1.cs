namespace ListBox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddName_Click(object sender, EventArgs e)
        {
            string name = tbxName.Text;
            if(!string.IsNullOrEmpty(name) )
            {
                lbxName.Items.Add(name);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbxName.Items.Clear();
        }
    }
}
