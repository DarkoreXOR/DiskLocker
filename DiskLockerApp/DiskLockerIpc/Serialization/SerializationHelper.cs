using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Serialization
{
    public static class SerializationHelper
    {
        public static string Serialize<T>( T obj )
        {
            using ( var ms = new MemoryStream() )
            {
                new BinaryFormatter().Serialize( ms, obj );
                ms.Seek( 0, SeekOrigin.Begin );
                return Convert.ToBase64String( ms.ToArray() );
            }
        }

        public static T Deserialize<T>( String str )
        {
            using ( var ms = new MemoryStream( Convert.FromBase64String( str ) ) )
            {
                ms.Seek( 0, SeekOrigin.Begin );
                return ( T )new BinaryFormatter().Deserialize( ms );
            }
        }
    }
}

