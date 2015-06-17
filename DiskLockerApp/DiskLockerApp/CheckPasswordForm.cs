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
    public partial class CheckPasswordForm : Form
    {
        public CheckPasswordForm()
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
        }

        private void button1_Click( object sender, EventArgs e )
        {

        }

        private void Form2_FormClosing( object sender, FormClosingEventArgs e )
        {
            e.Cancel = this.DialogResult == DialogResult.OK && textBox1.Text.Length <= 0;

            if(e.Cancel)
            {
                MessageBox.Show( "Пароль не может быть пустым!" );
            }

        }
    }
}
