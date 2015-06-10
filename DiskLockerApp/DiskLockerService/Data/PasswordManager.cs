using DiskLockerService.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService.Data
{
    public class PasswordManager
    {
        public PasswordManager()
        {

        }

        public bool HasPasswords()
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat( "SELECT COUNT(*) FROM ACCOUNT" );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = Convert.ToInt32( sql.DoScalarQuery( queryBuilder.ToString() ) );
            sql.Close();

            return num > 0;
        }

        public bool InsertPassword( string password )
        {
            StringBuilder queryBuilder = new StringBuilder();

            string pswd = password.Replace( "'", "''" );

            queryBuilder.AppendFormat( "INSERT INTO ACCOUNT (PASSWORD) VALUES ('{0}')", pswd );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = sql.DoNonQuery( queryBuilder.ToString() );
            sql.Close();

            return num > 0;
        }

        public bool CheckPassword( string password )
        {
            StringBuilder queryBuilder = new StringBuilder();

            string pswd = password.Replace( "'", "''" );

            queryBuilder.AppendFormat( "SELECT COUNT(*) FROM ACCOUNT WHERE PASSWORD = '{0}'", pswd );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = Convert.ToInt32(sql.DoScalarQuery( queryBuilder.ToString() ));
            sql.Close();

            return num > 0;
        }

        public bool DeletePassword(string password)
        {
            StringBuilder queryBuilder = new StringBuilder();

            string pswd = password.Replace( "'", "''" );

            queryBuilder.AppendFormat( "DELETE FROM ACCOUNT WHERE PASSWORD = '{0}'", pswd );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = sql.DoNonQuery( queryBuilder.ToString() );
            sql.Close();

            return num > 0;
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            StringBuilder queryBuilder = new StringBuilder();

            string oldPswd = oldPassword.Replace( "'", "''" );
            string newPswd = newPassword.Replace( "'", "''" );

            queryBuilder.AppendFormat( "UPDATE ACCOUNT SET PASSWORD = '{1}' WHERE PASSWORD = '{0}'", oldPswd, newPswd );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = sql.DoNonQuery( queryBuilder.ToString() );
            sql.Close();

            return num > 0;
        }

    }
}
