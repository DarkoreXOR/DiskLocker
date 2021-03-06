﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskLockerApp
{
    public class AuthManager
    {
        private IpcManager manager;

        public AuthManager(IpcManager manager)
        {
            this.manager = manager;
        }

        public bool GetSessionKey( out string sessionKey )
        {
            bool authResult = true;

            do
            {
                if ( !authResult )
                {
                    MessageBox.Show( "Ошибка входа. Проверьте правильность введенного пароля!" );
                }

                authResult = false;

                var form = new CheckPasswordForm();

                DialogResult result = form.ShowDialog();

                sessionKey = "";

                if ( result == DialogResult.Cancel )
                {
                    sessionKey = "";
                    return false;
                }

                if ( result == DialogResult.OK )
                {
                    authResult = manager.CreateAuthSessionKey( form.Password, out sessionKey );
                }

            } while ( !authResult );

            return authResult;
        }

        public bool CreatePassword()
        {
            var form = new CreatePasswordForm();

            DialogResult result = form.ShowDialog();

            if ( result == DialogResult.Cancel )
            {
                return false;
            }

            if ( result == DialogResult.OK )
            {
                return manager.AddPassword( null, form.Password );
            }

            return false;
        }

    }
}
