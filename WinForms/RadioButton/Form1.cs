namespace RadioButton
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioPayPal.Checked = true;
        }

        private void RadioButtonSelection_CheckChange(object sender, EventArgs e)
        {
            if (radioVisa.Checked)
            {
                lblSelection.Text = radioVisa.Text;
            }
            else if(radioCash.Checked)
            {
                lblSelection.Text = radioCash.Text;
            }else if (radioPayPal.Checked)
            {
                lblSelection.Text = radioPayPal.Text;
            }
        }
    }
}
