using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskLockerApp
{
    public partial class Form4 : Form
    {
        private string sessionKey;

        public Form4( string sessionKey )
        {
            InitializeComponent();
            this.sessionKey = sessionKey;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            bool res = textBox2.Text.Equals( textBox3.Text ) && textBox2.Text.Length > 0;

            if ( !res )
            {
                MessageBox.Show( "Пароли не совпадают или не заполнены!" );
            }
            else
            {
                try
                {
                    IpcManager manager = new IpcManager();

                    bool changed = manager.ChangePassword( this.sessionKey, textBox1.Text, textBox2.Text );

                    if(changed)
                    {
                        MessageBox.Show( "Пароль успешно изменен!" );
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show( "Проверьте правильность текущего пароля!" );
                    }
                }
                catch ( EndpointNotFoundException )
                {
                    MessageBox.Show( "Сервис управления не запущен!" );
                }

            }
        }

        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            textBox1.UseSystemPasswordChar = !checkBox1.Checked;
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
            textBox3.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void button2_Click( object sender, EventArgs e )
        {

        }
    }
}
