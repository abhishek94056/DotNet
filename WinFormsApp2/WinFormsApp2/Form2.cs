using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            label2.Text = Form1.fname;
            label3.Text = Form1.mname;
            label4.Text = Form1.sname;
        }


        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
