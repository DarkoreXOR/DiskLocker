﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService.Data
{
    public static class HashManager
    {
        public static string Sha256( string password )
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash( Encoding.ASCII.GetBytes( password ), 0, Encoding.ASCII.GetByteCount( password ) );
            foreach ( byte bit in crypto )
            {
                hash += bit.ToString( "x2" );
            }
            return hash;
        }
    }
}
