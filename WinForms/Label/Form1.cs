namespace Label
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "Name: ";
            label3.Text = "Age: ";
            label4.Text = "Gender: ";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String name = "Abhishek";
            int age = 22;
            String gender = "Male";
            label2.Text = "Name: "+name;
            label3.Text = "Age: "+age;
            label4.Text = "Gender: "+gender;
            label5.Visible = true;
        }

       
    }
}
