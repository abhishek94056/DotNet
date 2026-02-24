namespace ToString
{
    public partial class ToString : Form
    {
        public ToString()
        {
            InitializeComponent();
        }

        private void Count_Click(object sender, EventArgs e)
        {
            One.Text = (1 + 1).ToString();
            Two.Text = (2 + 2).ToString();
            Three.Text = (3 + 3).ToString();
        }
    }
}
