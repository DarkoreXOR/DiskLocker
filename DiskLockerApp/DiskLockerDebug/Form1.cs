using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click( object sender, EventArgs e )
        {
            if ( !DevIoCore.IoStartService() )
            {
                MessageBox.Show( "Ошибка!" );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            if ( !DevIoCore.IoStopService() )
            {
                MessageBox.Show( "Ошибка!" );
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            UInt32 uk;
            if ( DevIoCore.IoAddPathForProtection( textBox1.Text, checkBox1.Checked, out uk ) )
            {
                textBox2.AppendText( uk.ToString() );
            }
            else
                MessageBox.Show( "Ошибка!" );
        }

        private void button4_Click( object sender, EventArgs e )
        {
            bool res;
            if ( DevIoCore.IoRemovePathFromProtection( Convert.ToUInt32( textBox1.Text ), out res ) )
            {
                textBox2.AppendText( res.ToString() );
            }
            else
                MessageBox.Show( "Ошибка!" );
        }

        private void button5_Click( object sender, EventArgs e )
        {
            if ( DevIoCore.IoRemoveAllPathsFromProtection() )
            {
            }
            else
                MessageBox.Show( "Ошибка!" );
        }

        private void button6_Click( object sender, EventArgs e )
        {
            string physicalPath;
            bool res = DevIoCore.LogicalPathToPhysical( textBox1.Text, out physicalPath );

            if(!res)
            {
                MessageBox.Show( "Ошибка!" );
            }

            textBox1.Text = physicalPath;
        }

        private void button7_Click( object sender, EventArgs e )
        {
            if ( !DevIoCore.IoSetUnloadAccess( checkBox1.Checked ) )
            {
                MessageBox.Show( "Ошибка!" );
            }
        }

        private void button8_Click( object sender, EventArgs e )
        {
            
        }

        private void button8_Click_1( object sender, EventArgs e )
        {
            SQLiteConnection connection = new SQLiteConnection( GetConnectionString() );
            connection.Open();
            connection.ChangePassword( new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xDC, 0xC0, 0xDE } );
        }

        private static string GetExeFile()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public static string GetExeFilePath()
        {
            return Path.GetDirectoryName( GetExeFile() ) + Path.DirectorySeparatorChar;
        }

        private static string GetDatabaseFile()
        {
            return String.Format( "{0}data.dat", GetExeFilePath() );
        }

        public static string GetConnectionString()
        {
            return String.Format( "Data Source={0};Version=3;", GetDatabaseFile() );
        }

        private void Form1_Load( object sender, EventArgs e )
        {

        }

        private void button9_Click( object sender, EventArgs e )
        {
            SQLiteConnection connection = new SQLiteConnection( GetConnectionString() );
            connection.SetPassword( new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xDC, 0xC0, 0xDE } );
            connection.Open();
            connection.ChangePassword( "" );
        }
    }
}
