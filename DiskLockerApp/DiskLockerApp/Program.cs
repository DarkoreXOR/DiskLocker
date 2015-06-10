using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskLockerApp
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            IpcManager manager = new IpcManager();
            Auth auth = new Auth( manager );

            string sessionKey;

            try
            {
                bool created = false;

                if(!manager.HasPasswords())
                {
                    created = auth.CreatePassword();
                }

                if ( created || manager.HasPasswords() )
                {
                    bool authResult = auth.GetSessionKey( out sessionKey );

                    if ( authResult )
                    {
                        Application.Run( new Form1( sessionKey ) );
                    }
                }
            }
            catch ( EndpointNotFoundException )
            {
                MessageBox.Show( "Сервис управления не запущен!" );
            }
        }
    }
}
