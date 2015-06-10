using DiskLockerService.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService.Data
{
    public class PathManager
    {
        public PathManager()
        {

        }

        public bool InsertPath( string path, bool weakProtection )
        {
            StringBuilder queryBuilder = new StringBuilder();

            path = path.Replace( "'", "''" );

            int weakProtectionValue = weakProtection ? 1 : 0;

            queryBuilder.AppendFormat( "INSERT INTO FILESYSTEM (PATH, WEAK_PROTECTION) VALUES ('{0}', {1})", path, weakProtectionValue );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = sql.DoNonQuery( queryBuilder.ToString() );
            sql.Close();

            return num > 0;
        }

        public bool DeletePath( string path )
        {
            StringBuilder queryBuilder = new StringBuilder();

            path = path.Replace( "'", "''" );

            queryBuilder.AppendFormat( "DELETE FROM FILESYSTEM WHERE PATH = '{0}'", path );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = sql.DoNonQuery( queryBuilder.ToString() );
            sql.Close();

            return num > 0;
        }

        public Dictionary<string, bool> GetAllPaths()
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat( "SELECT PATH, WEAK_PROTECTION FROM FILESYSTEM" );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );

            sql.Open();

            var reader = sql.DoQuery( queryBuilder.ToString() );

            var dictionary = new Dictionary<string, bool>();

            while ( reader.Read() )
            {
                string path = reader[ "PATH" ].ToString();
                bool weakProtection = Convert.ToInt32( reader[ "WEAK_PROTECTION" ].ToString() ) > 0;

                dictionary.Add( path, weakProtection );
            }

            sql.Close();

            return dictionary;
        }

        public bool ExistsPath( string path )
        {
            StringBuilder queryBuilder = new StringBuilder();

            path = path.Replace( "'", "''" );

            queryBuilder.AppendFormat( "SELECT COUNT(*) FROM FILESYSTEM WHERE PATH = '{0}'", path );

            SqlCore sql = new SqlCore( DataUtils.GetConnectionString() );
            sql.Open();
            int num = Convert.ToInt32( sql.DoScalarQuery( queryBuilder.ToString() ) );
            sql.Close();

            return num > 0;
        }
    }
}
