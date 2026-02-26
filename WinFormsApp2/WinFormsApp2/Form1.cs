namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public static string fname ="";
        public static string mname ="";
        public static string sname = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fname = textBox1.Text;
            mname = textBox2.Text;
            sname = textBox3.Text;
            Form2 f = new Form2();
            f.Show();

        }
    }
}
