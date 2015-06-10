using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskLockerApp
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return textBox1.Text;
            }
        }

        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            textBox1.UseSystemPasswordChar = !checkBox1.Checked;
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void button1_Click( object sender, EventArgs e )
        {

        }

        private void Form3_Load( object sender, EventArgs e )
        {

        }

        private void Form3_FormClosing( object sender, FormClosingEventArgs e )
        {
            bool empty = textBox1.Text.Length <= 0;
            bool isCancel = this.DialogResult == DialogResult.Cancel;
            e.Cancel = ( !textBox1.Text.Equals( textBox2.Text ) || empty ) && !isCancel;

            if(e.Cancel)
            {
                MessageBox.Show("Пароли не совпадают или не заполнены!");
            }

        }

        private void Form3_Shown( object sender, EventArgs e )
        {
            MessageBox.Show("Обнаружен первый запуск программы. Пожалуйста, придумайте и введите пароль!");
        }
    }
}
