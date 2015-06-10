using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Answer
{
    [Serializable]
    public class CreateAuthSessionKeyResult
    {
        public bool Value { get; set; }

        public string SessionKey { get; set; }
    }
}
