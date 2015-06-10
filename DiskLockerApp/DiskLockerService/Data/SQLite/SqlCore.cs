using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService.Data.SQLite
{
    public class SqlCore
    {
        private string connectionString;
        private SQLiteConnection connection;
        private SQLiteTransaction currentTransaction;

        public SqlCore( string connectionString )
        {
            this.connectionString = connectionString;
            this.InitializeConnection();
        }

        public void InitializeConnection()
        {
            this.connection = new SQLiteConnection( this.connectionString );
        }

        public void Open()
        {
            this.connection.SetPassword( new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xDC, 0xC0, 0xDE } );
            this.connection.Open();
            //this.connection.ChangePassword( new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xBA, 0xDC, 0xC0, 0xDE } );
            //this.connection.ChangePassword( "" );
        }

        public void Close()
        {
            this.connection.Close();
            this.connection.Dispose();
        }

        public void BeginTransaction()
        {
            this.currentTransaction = this.connection.BeginTransaction();
        }

        public void EndTransaction()
        {
            this.currentTransaction.Commit();
        }

        public SQLiteCommand CreateQuery( string query )
        {
            SQLiteCommand command = new SQLiteCommand( query, this.connection );

            if ( this.currentTransaction != null )
            {
                command.Transaction = currentTransaction;
            }

            return command;
        }

        public SQLiteDataReader DoQuery( string query )
        {
            var command = this.CreateQuery( query );
            return command.ExecuteReader();
        }

        public int DoNonQuery( string query )
        {
            var command = this.CreateQuery( query );
            return command.ExecuteNonQuery();
        }

        public Object DoScalarQuery( string query )
        {
            var command = this.CreateQuery( query );
            return command.ExecuteScalar();
        }

        public void FreeQuery( SQLiteDataReader dataReader )
        {
            dataReader.Dispose();
        }

    }
}
