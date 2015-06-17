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
    public partial class AddProtectionForm : Form
    {
        private string sessionKey;

        public AddProtectionForm(string sessionKey)
        {
            InitializeComponent();
            this.sessionKey = sessionKey;
        }

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                IpcManager ipc = new IpcManager();
                bool result = ipc.AddPathForProtection( sessionKey, textBox1.Text, checkBox1.Checked );

                if ( result )
                {
                    MessageBox.Show( "Запись успешно добавлена!" );
                    this.Close();
                }
                else
                {
                    MessageBox.Show( "Ошибка добавления!" );
                }
            }
            catch ( EndpointNotFoundException )
            {
                MessageBox.Show( "Сервис управления не запущен!" );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {

        }

        private void button4_Click( object sender, EventArgs e )
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog() == DialogResult.OK )
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }
    }
}
