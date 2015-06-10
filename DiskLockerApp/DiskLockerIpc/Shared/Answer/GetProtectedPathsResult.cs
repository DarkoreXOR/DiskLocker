using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Answer
{
    [Serializable]
    public class GetProtectedPathsResult
    {
        public bool Value { get; set; }

        public Dictionary<uint, string> Paths { get; set; }
    }
}
