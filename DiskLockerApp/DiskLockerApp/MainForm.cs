using DiskLockerIpc;
using DiskLockerIpc.Shared;
using DiskLockerIpc.Shared.Answer;
using DiskLockerIpc.Shared.Request;
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
    public partial class MainForm : Form
    {
        public string SessionKey { get; set; }

        public MainForm( string sessionKey )
        {
            InitializeComponent();
            this.SessionKey = sessionKey;
        }

        private void button1_Click( object sender, EventArgs e )
        {

        }

        private bool ReloadPaths()
        {
            try
            {
                IpcManager ipc = new IpcManager();
                var result = ipc.GetProtectedPaths( SessionKey );

                if ( !result.Value )
                {
                    return false;
                }

                listView1.Items.Clear();

                foreach ( var entry in result.Paths )
                {
                    var items = listView1.Items.Add( entry.Key.ToString() );
                    items.SubItems.Add( entry.Value.ToString() );
                }

                return true;
            }
            catch ( EndpointNotFoundException )
            {
                return false;
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {

        }

        private void button2_Click( object sender, EventArgs e )
        {
            
        }

        private void button4_Click( object sender, EventArgs e )
        {

        }

        private void button5_Click( object sender, EventArgs e )
        {

        }

        private void button6_Click( object sender, EventArgs e )
        {

        }

        private void button7_Click( object sender, EventArgs e )
        {

        }

        private void Form1_Load( object sender, EventArgs e )
        {

        }

        private void сменитьПарольToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ChangePasswordForm form = new ChangePasswordForm( this.SessionKey );
            form.ShowDialog( this );
        }

        private void выходToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void toolStripMenuItem2_Click( object sender, EventArgs e )
        {
            var form = new AddProtectionForm( this.SessionKey );
            form.ShowDialog( this );
            this.ReloadPaths();
        }

        private void выбраннуюЗаписьToolStripMenuItem_Click( object sender, EventArgs e )
        {
            
        }

        private void обновитьToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( !this.ReloadPaths() )
            {
                MessageBox.Show( "Сервис управления не запущен!" );
            }
        }

        private void удалитьToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( listView1.SelectedItems.Count == 0 )
            {
                return;
            }

            try
            {
                IpcManager ipc = new IpcManager();

                uint key = Convert.ToUInt32( listView1.SelectedItems[ 0 ].Text );

                bool result = ipc.RemovePathFromProtection( SessionKey, key );

                if ( result )
                {
                    this.ReloadPaths();
                    MessageBox.Show( "Запись успешно удалена!" );
                }
                else
                {
                    MessageBox.Show( "Ошибка удаления записи!" );
                }
            }
            catch ( EndpointNotFoundException )
            {
                MessageBox.Show( "Сервис управления не запущен!" );
            }
        }

        private void Form1_Shown( object sender, EventArgs e )
        {
            this.ReloadPaths();
        }
    }
}
